using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                new FileStream(Path.Combine(path, "google-api-key.json"), 
                FileMode.Open, FileAccess.Read))
            {
                string credPath = Path.Combine(path, "google-api-creds.json");

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

    }
}
