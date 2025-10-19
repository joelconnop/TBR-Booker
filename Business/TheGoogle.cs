using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using GoogleMapsApi;
using GoogleMapsApi.Entities.Common;
using GoogleMapsApi.Entities.Directions.Request;
using GoogleMapsApi.Entities.Directions.Response;
using GoogleMapsApi.Entities.Geocoding.Request;
using GoogleMapsApi.Entities.Geocoding.Response;
using GoogleMapsApi.Entities.PlaceAutocomplete.Request;
using GoogleMapsApi.StaticMaps;
using GoogleMapsApi.StaticMaps.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using TBRBooker.Base;
using TBRBooker.Model.DTO;

namespace TBRBooker.Business
{
    public class TheGoogle
    {
        public static bool GoogleMapsOn = true;
        private static List<GoogleCalendarItemDTO> Calendar { get; set; }
        private static (DateTime Start, DateTime End) CalendarRange { get; set; }
        private static readonly object CalendarLock = new object();
        public static bool TestSlowGoogle = false; // for local diagnostics only

        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/calendar-dotnet-quickstart.json
        static string[] Scopes = { CalendarService.Scope.Calendar };
        static string ApplicationName = "TBR Booker";
        static int SearchRadius = 150000;   // 150km from Nerang

        private static UserCredential _creds;

        private class TravelInfoCacheEntry
        {
            public TravelInfoCacheEntry(int[] durations, int[] distances)
            {
                Durations = durations;
                Distances = distances;
                CachedAt = DateTime.UtcNow;
            }

            public int[] Durations { get; }
            public int[] Distances { get; }
            public DateTime CachedAt { get; }
        }

        private static readonly ConcurrentDictionary<string, TravelInfoCacheEntry> TravelInfoCache = new ConcurrentDictionary<string, TravelInfoCacheEntry>();
        private const int TravelInfoCacheLimit = 2000; // ~100MB assuming ~50KB per route entry

        private static string NormalizeForCache(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().ToUpperInvariant();
        }

        private static string BuildTravelInfoCacheKey(string origin, string destination, IEnumerable<string> waypoints, DateTime arrivalTime, bool finishAtStart)
        {
            var builder = new StringBuilder();
            builder.Append(NormalizeForCache(origin)).Append("|");
            builder.Append(NormalizeForCache(destination)).Append("|");

            if (waypoints != null)
            {
                foreach (var wp in waypoints)
                {
                    builder.Append(NormalizeForCache(wp)).Append("|");
                }
            }

            builder.Append("FA=").Append(finishAtStart ? "1" : "0").Append("|");
            builder.Append("AT=").Append(arrivalTime == default ? "NA" : arrivalTime.ToUniversalTime().ToString("yyyyMMddHHmm"));

            using (var sha = SHA256.Create())
            {
                var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(builder.ToString()));
                return Convert.ToBase64String(hashBytes);
            }
        }

        private static readonly HttpClient GoogleHttpClient = CreateGoogleHttpClient();

        private static HttpClient CreateGoogleHttpClient()
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("TBRBooker", "1.0"));
            client.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            return client;
        }

        private static async Task<string> GetGoogleJsonAsync(string url, CancellationToken cancellationToken)
        {
            try
            {
                if (TestSlowGoogle)
                {
                    await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken).ConfigureAwait(false);
                    throw new TimeoutException("Google request timed out (test mode).");
                }

                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    var response = await GoogleHttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                throw new TimeoutException($"Google request timed out: {url}", ex);
            }
        }

        private static UserCredential GetCreds()
        {
            if (_creds == null)
                _creds = Connect();
            return _creds;
        }

        public static UserCredential Connect()
        {
            var path = Base.Settings.Inst().WorkingDir + "\\config";
            using (var stream =
                new FileStream(System.IO.Path.Combine(path, "google-api-key.json"), 
                FileMode.Open, FileAccess.Read))
            {
                string credPath = System.IO.Path.Combine(path, "google-api-creds.json");

                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "sarahjane@truebluereptiles.com.au",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

        }

        private static CalendarService CreateCalendarService()
        {
            return new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GetCreds(),
                ApplicationName = ApplicationName,
            });
        }

        public static string AddGoogleCalendarEvent(GoogleCalendarItemDTO calItem)
        {
            var service = CreateCalendarService();
            var calEvent = EventFromCalendarItem(calItem);
            EventsResource.InsertRequest request = service.Events.Insert(calEvent, "primary");
            var response = request.Execute();
            if (calItem.Description.StartsWith(GoogleCalendarItemDTO.Blockout))
                Calendar.Add(calItem);
            return response.Id;
        }

        public static void UpdateGoogleCalendarEvent(GoogleCalendarItemDTO calItem)
        {
            var service = CreateCalendarService();
            var calEvent = EventFromCalendarItem(calItem);
            EventsResource.UpdateRequest request = 
                service.Events.Update(calEvent, "primary", calItem.Id);
            request.Execute();
        }

        public static void DeleteGoogleCalendarEvent(string googleEventId)
        {
            var service = CreateCalendarService();
            EventsResource.DeleteRequest request =
                service.Events.Delete("primary", googleEventId);
            request.Execute();
        }

        static Event EventFromCalendarItem(GoogleCalendarItemDTO calItem)
        {
            return new Event()
            {
                //Id = calItem.Id,
                Summary = calItem.Name,
                Location = calItem.Location,
                Description = calItem.Description,
                Start = new EventDateTime()
                {
                    DateTime =
        DTUtils.DateTimeFromInt(calItem.Date, calItem.Time),
                    TimeZone = "Australia/Brisbane"
                },
                End = new EventDateTime()
                {
                    DateTime =
        DTUtils.DateTimeFromInt(calItem.Date, calItem.Time, calItem.Duration),
                    TimeZone = "Australia/Brisbane"
                },
            };
        }

        public static List<GoogleCalendarItemDTO>
            GetGoogleCalendar(DateTime startDate, DateTime endDate,
            bool isForceReadAll)
        {
            lock (CalendarLock)
            {
                // can we use the cached calendar ?
                if (Calendar != null && !isForceReadAll &&
                    startDate >= CalendarRange.Start && endDate <= CalendarRange.End)
                    return Calendar.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();


                // Create Google Calendar API service.
                var service = CreateCalendarService();

                // get request
                var request = GetGoogleEventRequest(service);


                if (Calendar == null || isForceReadAll ||
                (startDate < CalendarRange.Start && endDate > CalendarRange.End))
                {
                    // create a new Calendar if it has been requested, or if the new range more than
                    // completely encompasses the old one
                    Calendar = new List<GoogleCalendarItemDTO>();
                    CalendarRange = (startDate, endDate);
                    request.TimeMin = startDate;
                    request.TimeMax = endDate;
                }
                else if (startDate >= CalendarRange.Start && endDate <= CalendarRange.End)
                    throw new Exception("Unexpected attempt to read Google events inside of cached results: "
                        + $"{CalendarRange.Start} - {CalendarRange.End} encompasses {startDate} - {endDate}.");

                // for all other cases, only search the time period not searched previously
                else if (startDate >= CalendarRange.Start)
                {
                    request.TimeMin = CalendarRange.End.AddHours(0.1);
                    request.TimeMax = endDate;
                    CalendarRange = (CalendarRange.Start, endDate);
                }
                else if (endDate <= CalendarRange.End)
                {
                    request.TimeMin = startDate;
                    request.TimeMax = CalendarRange.Start.AddHours(-0.1);
                    CalendarRange = (startDate, CalendarRange.End);
                }
                else
                {
                    throw new Exception("Unexpected Google Events date ranges: "
                       + $"{CalendarRange.Start} - {CalendarRange.End}, {startDate} - {endDate}.");
                }

                // List events.
                Events events = request.Execute();
                if (events.Items != null && events.Items.Count > 0)
                {
                    foreach (var eventItem in events.Items)
                    {
                        if (Calendar == null)
                            Calendar = new List<GoogleCalendarItemDTO>();

                        if (string.IsNullOrEmpty(eventItem.Description)
                            || !eventItem.Description.StartsWith(GoogleCalendarItemDTO.Blockout))
                            // ignore these for now (possible alternative to scanning dynamodb)
                            continue;
                        else if (eventItem.Start.DateTime.HasValue)
                            // add the calendar item if it isn't already on our calendar
                            // (dupes should be avoided by the date range smarts)
                            if (Calendar.Any(x => x.Id.Equals(eventItem.Id)))
                                Console.WriteLine("Event was already on calendar: " + eventItem);
                            else
                                Calendar.Add(MakeTBREvent(eventItem));
                        else
                            Console.WriteLine("Event did not have a start date: " + eventItem);
                    }
                }

                return Calendar.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();
            }
        }

        private static EventsResource.ListRequest GetGoogleEventRequest(CalendarService service)
        {
            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.ShowDeleted = false;
            request.SingleEvents = true;
            //request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            return request;
        }

        private static GoogleCalendarItemDTO MakeTBREvent(Event gei)
        {
            int time, duration;
            time = DTUtils.TimeInt(gei.Start.DateTime.Value);
            if (gei.EndTimeUnspecified.HasValue && !gei.EndTimeUnspecified.Value)
            {
                duration = 1; //default to 1 minute if no end time specified
            }
            else
            {
                duration = DTUtils.MinuteDifference(
                    gei.Start.DateTime.Value, gei.End.DateTime.Value);
            }

            var attendees = new List<string>();
            if (gei.Attendees != null)
            {
                attendees.AddRange(gei.Attendees.Select(
                x => string.IsNullOrEmpty(x.DisplayName) ? x.DisplayName : x.Email));
            }
            return new GoogleCalendarItemDTO(
                DTUtils.StartOfDay(gei.Start.DateTime.Value), time, duration,
                gei.Summary, gei.Description, attendees, gei.Id, 
                gei.Location);
        }

        public static Task<string[]> PlacesSearchAsync(string searchTerm, string sessionToken = null, CancellationToken cancellationToken = default)
        {
            return PlacesSearchAsyncInternal(searchTerm, sessionToken, cancellationToken);
        }

        public static string[] PlacesSearch(string searchTerm, string sessionToken = null)
        {
            return PlacesSearchAsyncInternal(searchTerm, sessionToken, CancellationToken.None).GetAwaiter().GetResult();
        }

        private static async Task<string[]> PlacesSearchAsyncInternal(string searchTerm, string sessionToken, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(Base.Settings.Inst().GoogleAPIKey)
                || string.IsNullOrEmpty(searchTerm))
                return Array.Empty<string>();

            var queryParams = new List<string>
            {
                $"key={Uri.EscapeDataString(Base.Settings.Inst().GoogleAPIKey)}",
                $"input={Uri.EscapeDataString(searchTerm)}",
                $"location={Uri.EscapeDataString("-27.982645,153.340282")}",
                $"radius={SearchRadius}",
                "strictbounds"
            };

            if (!string.IsNullOrEmpty(sessionToken))
            {
                queryParams.Add($"sessiontoken={Uri.EscapeDataString(sessionToken)}");
            }

            var url = "https://maps.googleapis.com/maps/api/place/autocomplete/json?" + string.Join("&", queryParams);

            var responseJson = await GetGoogleJsonAsync(url, cancellationToken).ConfigureAwait(false);
            var response = JsonConvert.DeserializeObject<GoogleMapsApi.Entities.PlaceAutocomplete.Response.PlaceAutocompleteResponse>(responseJson);
            if (response == null)
            {
                throw new Exception($"Places Search failed for '{searchTerm}'. Response was empty.");
            }

            switch (response.Status)
            {
                case GoogleMapsApi.Entities.PlaceAutocomplete.Response.Status.OK:
                    return response.Results.Select(x =>
                        x.Description.Replace(" QLD, Australia", "").Trim().Trim(','))
                        .ToArray();
                case GoogleMapsApi.Entities.PlaceAutocomplete.Response.Status.ZERO_RESULTS:
                    return new[] { "(no results)" };
                default:
                    throw new Exception($"Places Search failed for '{searchTerm}'. Status = {response.Status}.");
            }
        }

        private static (string Origin, string Destination, List<string> Waypoints)
            RoutesParams(string startLocation, List<string> addresses)
        {
            var waypoints = new List<string>();
            startLocation = startLocation.Trim();

            if (string.IsNullOrEmpty(startLocation))
            {
                if (addresses.Count < 2)
                    throw new Exception("No origin was provided.");
                startLocation = addresses[0];
                addresses.RemoveAt(0);
            }
            if (addresses.Count == 0)
                throw new Exception("No destination was provided.");

            var destination = addresses[addresses.Count - 1].Trim();

            if (addresses.Count > 1)
            {
                for (int i = 0; i < addresses.Count - 1; i++)
                {
                    waypoints.Add(addresses[i].Trim());
                }
            }

            return (startLocation, destination, waypoints);
        }

        public static string DayPlannerMap(List<string> addresses, string startLocation = "666 Beechmont Road, Lower Beechmont, Qld 4211")
        {
            var routesParams = RoutesParams(startLocation, addresses);
            string url = "https://www.google.com/maps/dir/?api=1";
            if (!string.IsNullOrEmpty(startLocation))
            {
                url += "&origin=" + WebUtility.UrlEncode(routesParams.Origin);
            }
            else
            {
                return "";
            }

            url += "&destination=" + WebUtility.UrlEncode(routesParams.Destination);

            if (routesParams.Waypoints.Count > 0)
            {
                url += "&waypoints=";
                for (int i = 0; i < routesParams.Waypoints.Count; i++)
                {
                    url += routesParams.Waypoints[i] + "|";
                }
                url = url.Trim('|');
            }

            return url;
        }

        /// <summary>
        /// Gets the travel times and distances between for any number of places on a route
        /// </summary>
        /// <param name="addresses">can include start location if leaving startLocation blank
        /// arrivalTimes is addresses - 1 if including startlocation in addresses</param>
        /// <param name="startLocation">leave blank if prefer to have it in the array</param>
        /// <returns></returns>
        public static Task<(int[] Durations, int[] Distances)> TravelInfoAsync(
            List<string> addresses, DateTime roughDateAndTime,
             string startLocation = "666 Beechmont Road, Lower Beechmont, Qld 4211", bool finishAtStart = false, CancellationToken cancellationToken = default)
        {
            return TravelInfoAsyncInternal(addresses, roughDateAndTime, startLocation, finishAtStart, cancellationToken);
        }

        public static (int[] Durations, int[] Distances) TravelInfo(
            List<string> addresses, DateTime roughDateAndTime,
             string startLocation = "666 Beechmont Road, Lower Beechmont, Qld 4211", bool finishAtStart = false)
        {
            return TravelInfoAsyncInternal(addresses, roughDateAndTime, startLocation, finishAtStart, CancellationToken.None).GetAwaiter().GetResult();
        }

        private static async Task<(int[] Durations, int[] Distances)> TravelInfoAsyncInternal(
            List<string> addresses, DateTime roughDateAndTime,
             string startLocation, bool finishAtStart, CancellationToken cancellationToken)
        {
            if (addresses == null || addresses.Count == 0)
            {
                return (Array.Empty<int>(), Array.Empty<int>());
            }

            if (finishAtStart && !string.IsNullOrEmpty(startLocation))
            {
                addresses.Add(startLocation);
            }

            var numPoints = addresses.Count;
            if (string.IsNullOrEmpty(startLocation))
                numPoints--;

            var emptyRoute = (new int[numPoints], new int[numPoints]);

            if (string.IsNullOrEmpty(Base.Settings.Inst().GoogleAPIKey)
                || addresses.All(x => string.IsNullOrEmpty(x)))
            {
                return emptyRoute;
            }

            var addressesForRequest = new List<string>(addresses);
            var routesParams = RoutesParams(startLocation, addressesForRequest);
            if (!GoogleMapsOn)
            {
                addresses.Clear();
                addresses.AddRange(addressesForRequest);
                return emptyRoute;
            }

            var cacheKey = BuildTravelInfoCacheKey(routesParams.Origin, routesParams.Destination, routesParams.Waypoints, roughDateAndTime, finishAtStart);
            if (TravelInfoCache.TryGetValue(cacheKey, out var cachedEntry))
            {
                addresses.Clear();
                addresses.AddRange(addressesForRequest);
                return ((int[])cachedEntry.Durations.Clone(), (int[])cachedEntry.Distances.Clone());
            }

            var queryParams = new List<string>
            {
                $"origin={Uri.EscapeDataString(routesParams.Origin)}",
                $"destination={Uri.EscapeDataString(routesParams.Destination)}",
                "mode=driving",
                $"key={Uri.EscapeDataString(Base.Settings.Inst().GoogleAPIKey)}"
            };

            if (roughDateAndTime != default)
            {
                var arrivalSeconds = new DateTimeOffset(roughDateAndTime).ToUnixTimeSeconds();
                queryParams.Add($"arrival_time={arrivalSeconds}");
            }

            if (routesParams.Waypoints.Count > 0)
            {
                var waypoints = string.Join("|", routesParams.Waypoints.Select(Uri.EscapeDataString));
                queryParams.Add($"waypoints={waypoints}");
            }

            var url = "https://maps.googleapis.com/maps/api/directions/json?" + string.Join("&", queryParams);

            var responseJson = await GetGoogleJsonAsync(url, cancellationToken).ConfigureAwait(false);
            var response = JObject.Parse(responseJson);
            var status = response["status"]?.Value<string>();

            switch (status)
            {
                case "ZERO_RESULTS":
                case "NOT_FOUND":
                    addresses.Clear();
                    addresses.AddRange(addressesForRequest);
                    if (TravelInfoCache.Count < TravelInfoCacheLimit && !TravelInfoCache.ContainsKey(cacheKey))
                    {
                        TravelInfoCache[cacheKey] = new TravelInfoCacheEntry(Array.Empty<int>(), Array.Empty<int>());
                    }
                    return (Array.Empty<int>(), Array.Empty<int>());
                case "OK":
                    break;
                default:
                    throw new Exception($"Failed to get travel info for {addressesForRequest.FirstOrDefault()}. Result: {status}");
            }

            var legs = response["routes"]?.FirstOrDefault()?["legs"] as JArray;
            if (legs == null)
            {
                throw new Exception($"Failed to get travel info for {addressesForRequest.FirstOrDefault()}. Result missing legs.");
            }

            if (legs.Count != numPoints)
                throw new Exception($"Expected {numPoints} legs, but there were {legs.Count}.");

            var routeDurations = new int[numPoints];
            var routeDistances = new int[numPoints];

            for (int i = 0; i < legs.Count; i++)
            {
                var leg = legs[i];
                var durationSeconds = leg["duration"]?["value"]?.Value<double>() ?? 0;
                var distanceMeters = leg["distance"]?["value"]?.Value<int>() ?? 0;
                routeDurations[i] = (int)Math.Round(durationSeconds / 60d);
                routeDistances[i] = distanceMeters;
            }

            addresses.Clear();
            addresses.AddRange(addressesForRequest);

            if (TravelInfoCache.Count < TravelInfoCacheLimit && !TravelInfoCache.ContainsKey(cacheKey))
            {
                TravelInfoCache[cacheKey] = new TravelInfoCacheEntry((int[])routeDurations.Clone(), (int[])routeDistances.Clone());
            }

            return (routeDurations, routeDistances);
        }
        public static Task<List<string>> GetDirectionsAsync(string destination,
            DateTime arrivalTime,
            string startLocation = "666 Beechmont Road, Lower Beechmont, Qld 4211", CancellationToken cancellationToken = default)
        {
            return GetDirectionsAsyncInternal(destination, arrivalTime, startLocation, cancellationToken);
        }

        public static List<string> GetDirections(string destination,
            DateTime arrivalTime,
            string startLocation = "666 Beechmont Road, Lower Beechmont, Qld 4211")
        {
            return GetDirectionsAsyncInternal(destination, arrivalTime, startLocation, CancellationToken.None).GetAwaiter().GetResult();
        }

        private static async Task<List<string>> GetDirectionsAsyncInternal(string destination,
            DateTime arrivalTime,
            string startLocation,
            CancellationToken cancellationToken)
        {
            var dirs = new List<string>();
            destination = destination.Trim();
            startLocation = startLocation.Trim();

            if (!GoogleMapsOn)
            {
                dirs.Add("Google Maps is currently disabled.");
                return dirs;
            }

            if (string.IsNullOrEmpty(Base.Settings.Inst().GoogleAPIKey)
                || string.IsNullOrEmpty(startLocation) || string.IsNullOrEmpty(destination))
                return dirs;

            var queryParams = new List<string>
            {
                $"origin={Uri.EscapeDataString(startLocation)}",
                $"destination={Uri.EscapeDataString(destination)}",
                "mode=driving",
                $"key={Uri.EscapeDataString(Base.Settings.Inst().GoogleAPIKey)}"
            };

            if (arrivalTime != default)
            {
                var arrivalSeconds = new DateTimeOffset(arrivalTime).ToUnixTimeSeconds();
                queryParams.Add($"arrival_time={arrivalSeconds}");
            }

            var url = "https://maps.googleapis.com/maps/api/directions/json?" + string.Join("&", queryParams);

            try
            {
                var responseJson = await GetGoogleJsonAsync(url, cancellationToken).ConfigureAwait(false);
                var response = JObject.Parse(responseJson);
                var status = response["status"]?.Value<string>();

                if (status != "OK")
                {
                    if (status == "ZERO_RESULTS" || status == "NOT_FOUND")
                        return dirs;

                    var errorMessage = response["error_message"]?.Value<string>();
                    ErrorLogger.LogError("Get directions", new Exception($"Failed to get directions for {destination}. Status: {status}. {errorMessage}"));
                    dirs.Add("Directions unavailable.");
                    return dirs;
                }

                var leg = response["routes"]?.FirstOrDefault()?["legs"]?.FirstOrDefault() as JObject;
                if (leg == null)
                {
                    dirs.Add("Directions unavailable.");
                    return dirs;
                }

                var steps = leg["steps"] as JArray;
                if (steps == null)
                    return dirs;

                foreach (var step in steps)
                {
                    var instruction = step["html_instructions"]?.Value<string>();
                    if (!string.IsNullOrEmpty(instruction))
                        dirs.Add(instruction);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError("Get directions", ex);
                dirs.Add("Directions unavailable.");
            }

            return dirs;
        }

    }
}
