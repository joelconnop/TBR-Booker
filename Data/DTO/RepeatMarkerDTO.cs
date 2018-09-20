using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using TBRBooker.Model.Enums;
using TBRBooker.Base;
using TBRBooker.Model.Entities;

namespace TBRBooker.Model.DTO
{

    public class RepeatMarkerDTO : CalendarItemDTO
    {
        public readonly string ScheduleId;
        public readonly Booking HypotheticalBooking;

        public RepeatMarkerDTO(string name, DateTime date, int time, string scheduleId, Booking placeholder)
            : base(CalendarItemTypes.RepeatMarker, name, date, time)
        {
            ScheduleId = scheduleId;
            HypotheticalBooking = placeholder;
        }

    }
}
