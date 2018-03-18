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
        public readonly int BookingNum;
        public readonly string BookingName;
        public readonly DateTime BookingDate;
        public readonly int BookingTime;
        public readonly BookingStates BookingStatus;
        public readonly DateTime? FollowupDate;
        public readonly DateTime? ConfirmationDate;

        ///// <summary>
        ///// in case BookingTime is not set
        ///// </summary>
        //public TimeSlots TimeSlot { get; set; }

        public CalendarItemDTO(int num, string name, DateTime date, int time, 
            BookingStates status, DateTime? followupDate, DateTime? confirmationDate)
        {
            BookingNum = num;
            BookingName = name;
            BookingDate = date;
            BookingTime = time;
            BookingStatus = status;
            FollowupDate = followupDate;
            ConfirmationDate = confirmationDate;
        }

    }
}
