using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using TBRBooker.Model.Enums;
using TBRBooker.Base;

namespace TBRBooker.Model.DTO
{

    public class CalendarItemDTO
    {
        public readonly CalendarItemTypes Type;
        public readonly string Name;
        public readonly DateTime Date;
        public readonly int Time;

        ///// <summary>
        ///// in case BookingTime is not set
        ///// </summary>
        //public TimeSlots TimeSlot { get; set; }

        public CalendarItemDTO(CalendarItemTypes type, string name, 
            DateTime date, int time)
        {
            Type = type;
            Name = name;
            Date = date;
            Time = time;
        }

    }
}
