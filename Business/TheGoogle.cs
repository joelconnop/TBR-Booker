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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TBRBooker.Base;
using TBRBooker.Model.DTO;

namespace TBRBooker.Business
{
    public class TheGoogle
    {

        private static List<GoogleCalendarItemDTO> Calendar { get; set; }
        private static (DateTime Start, DateTime End) CalendarRange { get; set; }

        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/calendar-dotnet-quickstart.json
        static string[] Scopes = { CalendarService.Scope.Calendar };
        static string ApplicationName = "TBR Booker";
        static int SearchRadius = 150000;   // 150km from Nerang

        private static UserCredential _creds;

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
            // can we use the cached calendar ?
            if (Calendar != null && !isForceReadAll &&
                startDate >= CalendarRange.Start && endDate <= CalendarRange.End)
                return Calendar.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();


            // Create Google Calendar API service.
            var service = CreateCalendarService();

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.ShowDeleted = false;
            request.SingleEvents = true;
            //request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

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

        public static string[] PlacesSearch(string searchTerm)
        {
            if (string.IsNullOrEmpty(Base.Settings.Inst().GoogleAPIKey)
                || string.IsNullOrEmpty(searchTerm))
                return new string[0];
            var req = new PlaceAutocompleteRequest()
            {
                ApiKey = Base.Settings.Inst().GoogleAPIKey,
                Input = searchTerm,
                Location = new Location(-27.982645, 153.340282),    // M1, Nerang
                Radius = SearchRadius, // look for places within 250km of Nerang
                StrinctBounds = true
                // Type = "address"  // we want "address" AND "establishment"
            };
            var response = GoogleMaps.PlaceAutocomplete.Query(req);
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

        public static string DayPlannerMap(List<string> addresses,
            string startLocation = "666 Beechmont Road, Lower Beechmont, Qld 4211")
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
        public static (int[] Durations, int[] Distances) TravelInfo(
            List<string> addresses, DateTime roughDateAndTime,
             string startLocation = "666 Beechmont Road, Lower Beechmont, Qld 4211")
        {
            int numPoints = addresses.Count;
            if (numPoints == 0)
                return (new int[0], new int[0]);

            if (string.IsNullOrEmpty(startLocation))
                numPoints--;
            var route = (new int[numPoints], new int[numPoints]);
            if (string.IsNullOrEmpty(Base.Settings.Inst().GoogleAPIKey) 
                || addresses.All(x => string.IsNullOrEmpty(x))                )
            {
                return route;
            }

            var routesParams = RoutesParams(startLocation, addresses);
            var req = new DirectionsRequest()
            {
                ApiKey = Base.Settings.Inst().GoogleAPIKey,
                Origin = routesParams.Origin,
                Destination = routesParams.Destination,
                ArrivalTime = roughDateAndTime,
                Waypoints = routesParams.Waypoints.ToArray(),
                TravelMode = TravelMode.Driving,
                Alternatives = false
            };
            var response = GoogleMaps.Directions.Query(req);
            if (!string.IsNullOrEmpty(response.ErrorMessage))
                throw new Exception(response.ErrorMessage);
            switch (response.Status)
            {
                case DirectionsStatusCodes.ZERO_RESULTS:
                case DirectionsStatusCodes.NOT_FOUND:
                    return (new int[0], new int[0]);
                case DirectionsStatusCodes.OK:
                    break; //proceed
                default:
                    throw new Exception($"Failed to get travel info for {addresses[0]}. Result: {response.Status}");
            }

            var groute = response.Routes.Single();
            int legsCount = groute.Legs.Count();
            if (legsCount != numPoints)
                throw new Exception($"Expected {numPoints} legs, but there were {legsCount}.");
            int i = 0;
            foreach (var leg in groute.Legs)
            {
                route.Item1[i] = (int)Math.Round(leg.Duration.Value.TotalMinutes);
                route.Item2[i] = leg.Distance.Value;
                i++;
            }

            return route;
        }

        public static List<string> GetDirections(string destination,
            DateTime arrivalTime,
            string startLocation = "666 Beechmont Road, Lower Beechmont, Qld 4211")
        {
            var dirs = new List<string>();
            destination = destination.Trim();
            startLocation = startLocation.Trim();

            if (string.IsNullOrEmpty(Base.Settings.Inst().GoogleAPIKey)
                || string.IsNullOrEmpty(startLocation) || string.IsNullOrEmpty(destination))
                return dirs;

            var req = new DirectionsRequest()
            {
                ApiKey = Base.Settings.Inst().GoogleAPIKey,
                Origin = startLocation,
                Destination = destination,
                ArrivalTime = arrivalTime,
                TravelMode = TravelMode.Driving,
                Alternatives = false
            };
            var response = GoogleMaps.Directions.Query(req);
            if (!string.IsNullOrEmpty(response.ErrorMessage))
                throw new Exception(response.ErrorMessage);
            if (response.Status != DirectionsStatusCodes.OK)
            {
                throw new Exception(response.StatusStr);
            }

            var leg = response.Routes.Single().Legs.Single();
            foreach (var step in leg.Steps)
            {
                dirs.Add(step.HtmlInstructions);
            }

            return dirs;
        }

    }
}
