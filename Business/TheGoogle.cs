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

        public TheGoogle Instance { get; set; }

        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/calendar-dotnet-quickstart.json
        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        static string ApplicationName = "TBR Booker";
        private static UserCredential _creds;

        private UserCredential GetCreds()
        {
            if (_creds == null)
                _creds = Connect();
            return _creds;
        }

        public static UserCredential Connect()
        {
            using (var stream =
                new FileStream("google_api_key.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = Base.Settings.Inst().WorkingDir + "\\config";
                credPath = Path.Combine(credPath, "google-api-creds.json");

                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

        }

        public static List<GoogleCalendarEventDTO> 
            GetGoogleCalendar(DateTime startDate, DateTime endDate)
        {
            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _creds,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = startDate;
            request.TimeMax = endDate;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            //request.MaxResults = 10;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = request.Execute();
            var calItems = new List<GoogleCalendarEventDTO>();
            if (events.Items != null && events.Items.Count > 0)
            {
                foreach (var eventItem in events.Items)
                {
                    if (eventItem.Start.DateTime.HasValue)
                        calItems.Add(MakeTBREvent(eventItem));
                    else
                        Console.WriteLine("Event did not have a start date: " + eventItem);
                }
            }

            return calItems;
        }

        private static GoogleCalendarEventDTO MakeTBREvent(Event gei)
        {
            int time, duration;
            time = Utils.TimeInt(gei.Start.DateTime.Value);
            if (!gei.EndTimeUnspecified.HasValue || !gei.EndTimeUnspecified.Value)
            {
                duration = 2; //default to 2 hours if no end time specified
            }
            else
            {
                duration = Utils.MinuteDifference(
                    gei.Start.DateTime.Value, gei.End.DateTime.Value);
            }

            var attendees = gei.Attendees.Select(
                x => string.IsNullOrEmpty(x.DisplayName) ? x.DisplayName : x.Email).ToList();
            return new GoogleCalendarEventDTO(
                Utils.StartOfDay(gei.Start.DateTime.Value), time, duration,
                gei.Summary, gei.Description, attendees);
        }

    }
}
