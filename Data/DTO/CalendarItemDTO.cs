using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using TBRBooker.Model.Enums;
using Base;

namespace TBRBooker.Model.DTO
{

    public class CalendarItemDTO
    {
        public int BookingNum { get; set; }
        public string BookingName { get; set; }
        public DateTime BookingDate { get; set; }
        public int BookingTime { get; set; }
        public BookingStates BookingStatus { get; set; }

        ///// <summary>
        ///// in case BookingTime is not set
        ///// </summary>
        //public TimeSlots TimeSlot { get; set; }

    }
}
