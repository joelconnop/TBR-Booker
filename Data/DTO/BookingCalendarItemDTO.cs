﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using TBRBooker.Model.Enums;
using TBRBooker.Base;

namespace TBRBooker.Model.DTO
{

    public class BookingCalendarItemDTO : CalendarItemDTO
    {
        public readonly int BookingNum;
        public readonly BookingStates BookingStatus;
        public readonly DateTime? FollowupDate;
        public readonly DateTime? ConfirmationDate;

        ///// <summary>
        ///// in case BookingTime is not set
        ///// </summary>
        //public TimeSlots TimeSlot { get; set; }

        public BookingCalendarItemDTO(int num, string name, DateTime date, int time, 
            BookingStates status, DateTime? followupDate, DateTime? confirmationDate)
            : base(CalendarItemTypes.Booking, name, date, time)
        {
            BookingNum = num;
            BookingStatus = status;
            FollowupDate = followupDate;
            ConfirmationDate = confirmationDate;
        }

    }
}
