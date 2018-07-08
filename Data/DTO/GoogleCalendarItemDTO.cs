using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBRBooker.Base;
using TBRBooker.Model.Enums;

namespace TBRBooker.Model.DTO
{
    public class GoogleCalendarItemDTO : CalendarItemDTO
    {
        public const string Blockout = "Blockout: ";
        public const string TBRBooking = "TBR: ";

        public readonly int Duration;
        public readonly string Description;
        public List<string> Attendees;
        public readonly string Id;
        public readonly string Location;


        public int EndTime => DTUtils.EndTime(Time, Duration);

        public GoogleCalendarItemDTO(DateTime date, int time, int duration, 
            string name, string desc, List<string> attendees, string id, 
            string location) 
            : base(CalendarItemTypes.GoogleEvent, name, date, time)
        {
            Duration = duration;
            Description = desc;
            Attendees = attendees;
            Id = id;
            Location = location;
        }

        public override string ToString()
        {
            return $"{Name}; {(string.IsNullOrEmpty(Description) ? "" : Description.Trim(':') + ": ")}"
           //     + $"{String.Join(",", Attendees).Trim().Trim(',')} "
                + (Duration > 1 ? ("for " + DTUtils.DurationToDisplayStr(Duration)) : "(unknown duration)");
        }
    }
}
