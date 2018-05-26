using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBRBooker.Base;

namespace TBRBooker.Model.DTO
{
    public class GoogleCalendarEventDTO
    {
        public readonly DateTime Date;
        public readonly int Time;
        public readonly int Duration;
        public readonly string Name;
        public readonly string Description;
        public List<string> Attendees;

        public GoogleCalendarEventDTO(DateTime start, int time, int duration, 
            string name, string desc, List<string> attendees)
        {
            Date = start;
            Time = time;
            Duration = duration;
            Name = name;
            Description = desc;
            Attendees = attendees;
        }

        public override string ToString()
        {
            return $"{(string.IsNullOrEmpty(Description) ? "" : Description + ": ")}{String.Join(",", Attendees).Trim().Trim(',')} for {Utils.DurationToDisplayStr(Duration)}";
        }
    }
}
