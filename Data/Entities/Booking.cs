﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using TBRBooker.Model.Enums;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using TBRBooker.Model.DTO;
using TBRBooker.Base;

namespace TBRBooker.Model.Entities
{
    public class Booking : BaseItem
    {
        public const string TABLE_NAME = "booking";
        public const string IS_OPEN_STR = "Y";

        public Booking()
        {
            TableName = TABLE_NAME;
            CustomerId = "";
            AccountId = "";

        }

        public override bool IsCacheItems()
        {
            return true;
        }

        //public override bool IsNew()
        //{
        //    return IsNewBooking;
        //}

        /// <summary>
        /// because in this case we can't rely on checking the Id to decide if new
        /// (because its created straight away)
        /// </summary>
        [JsonIgnore]
        public bool IsNewBooking { get; set; }

        [JsonIgnore]
        public Customer Customer { get; set; }

        public string CustomerId { get; set; }


        [JsonIgnore]
        public CorporateAccount Account { get; set; }

        public string AccountId { get; set; }

        public BookingStates Status { get; set; }

        [JsonIgnore]
        private bool IsOpenInDatabase { get; set; }

        public BookingPriorities Priority { get; set; }



        [Required(ErrorMessage = "Booking Date")]
        public DateTime BookingDate { get; set; }

        //[Required(ErrorMessage = "Time Slot")]
        //public TimeSlots TimeSlot { get; set; }

        /// <summary>
        /// eg 1558 = 15:58, 0 = midnight (notset, not valid for booked status), 110 = 1:10AM
        /// </summary>
        [Required(ErrorMessage = "Time Slot")]
        public int BookingTime { get; set; }

        /// <summary>
        /// eg hour and half = 90
        /// </summary>
        public int Duration { get; set; }

        public int EndTime => GetEndTime();
        private int GetEndTime()
        {
            var parsed = Utils.ParseTime(BookingTime);
            var ts = new TimeSpan(parsed.Hour, parsed.Minute, 0);
            return int.Parse(ts.Add(new TimeSpan(0, Duration, 0)).ToString("hhmm"));
        }


        [JsonIgnore]
        public int TravelTimeTo { get; set; }

        [JsonIgnore]
        public int TravelTimeFrom { get; set; }



        public LocationRegions LocationRegion { get; set; }
        public string VenueName { get; set; } //(part of address if using address entity)
        public string Address { get; set; }
        //public Address Address { get; set; }


        /// <summary>
        /// this comes from customer, when relevant (eg. Gold Coast City Council)
        /// </summary>
        public string PurchaseOrderRef { get; set; }

        // public bool IsPaid { get; set; }

        public string BookingNotes { get; set; }

        public string BookingName => ChooseNameForBooking();

        public LostJobReasons LostJobReason { get; set; }

        public decimal AmountPaid => PaymentHistory == null ? 0 : PaymentHistory.Sum(x => x.Amount);

        public string QbListId { get; set; }

        public List<Payment> PaymentHistory {get; set;}

        public bool IsInvoiced { get; set; }

        public bool IsPayOnDay { get; set; }

        /// <summary>
        /// Typically the customer's surname or company name but freely changeable.
        /// Needs to be on booking so we can display on calendar without reading other tables
        /// Examples 'Smith', 'Little Jon Party', 'Browns GC', 'PCYC Bris'
        /// </summary>
        public string BookingNickname { get; set; }

        private string ChooseNameForBooking()
        {
            if (!string.IsNullOrEmpty(BookingNickname))
            {
                return BookingNickname;
            }
            else if (Account != null)
            {
                return Account.CompanyName;
            }
            else if (Customer != null)
            {
                if (!string.IsNullOrEmpty(Customer.LastName))
                    return Customer.LastName;
                else if (!string.IsNullOrEmpty(Customer.FirstName))
                    return Customer.FirstName;
            }

                return string.Empty;

        }

        public static string BookingTimeStr(int bookingTime)
        {
            if (bookingTime == 0)
                return "";

            var parsed = Utils.ParseTime(bookingTime);
            return $"{(parsed.Hour >= 10 ? parsed.Hour.ToString() : "0" + parsed.Hour.ToString())}:{(parsed.Minute >= 10 ? parsed.Minute.ToString() : "0" + parsed.Minute.ToString())}";
        }

        public static TimeSpan GetBookingTime(int bookingTime)
        {
            var parsed = Utils.ParseTime(bookingTime);
            return new TimeSpan(parsed.Hour, parsed.Minute, 0);
        }

        //public Booking(string tableName) : base("booking")  //, "bookingDateTicks")
        //{

        //}

            //we will read/write this separately?
           // [JsonIgnore]
        public Service Service { get; set; }


        public override Document GetAddUpdateDoc()
        {
            var doc = base.GetAddUpdateDoc();

            //for CalendarItemDTO
            doc["isOpen"] = IsOpen() ? IS_OPEN_STR : "";
            doc["bookingName"] = BookingName;
            doc["bookingTime"] = BookingTime;
            doc["bookingDate"] = BookingDate.Ticks;
            //doc["timeSlot"] = Convert.ToInt32(TimeSlot);
            doc["status"] = Convert.ToInt32(Status);

            //fields we will likely want to report on
            doc["customerId"] = CustomerId;
            doc["accountId"] = AccountId;
            doc["locationRegion"] = Convert.ToInt32(LocationRegion);
            doc["priority"] = Convert.ToInt32(Priority);
            doc["lostJobReason"] = Convert.ToInt32(LostJobReason);

            return doc;
        }


        public override List<string> GetReadAttributes()
        {
            var atts = base.GetReadAttributes();
            atts.Add("isOpen");
            //other attributes are redundantly stored in json
            return atts;
        }

        public override void LoadAttributes(Document doc)
        {
            if (doc.ContainsKey("isOpen"))
                IsOpenInDatabase = doc["isOpen"].AsString().Equals(IS_OPEN_STR);
            else
                IsOpenInDatabase = false;
        }

        //private long GetBookingDateTicks()
        //{
        //    if (BookingDate.HasValue)
        //    {
        //        return BookingDate.Value.Ticks;
        //    }
        //    else
        //    {
        //        return DateTime.MaxValue.Ticks;
        //    }
        //}

        public bool IsBooked(bool isIncludeCancelled = false)
        {
            return IsBooked(Status, isIncludeCancelled);
        }

        public static bool IsBooked(BookingStates status, bool isIncludeCancelled)
        {
            switch (status)
            {
                case BookingStates.Booked:
                case BookingStates.Completed:
                case BookingStates.PaymentDue:
                    return true;
                case BookingStates.OpenEnquiry:
                case BookingStates.LostEnquiry:
                    return false;
                case BookingStates.Cancelled:
                case BookingStates.CancelledWithoutPayment:
                    return isIncludeCancelled;
                default:
                    throw new Exception($"Unknown if status {status} is a booking or enquiry.");
            }
        }

        public bool IsOpen()
        {
            //the program should also push user to cancelling/completing old deals/bookings (how about a spank per week for each unresolved booking status)
            return IsBookingOpen(Status, BookingDate);
        }

        public static bool IsBookingOpen(BookingStates status, DateTime bookingDate)
        {
            //it atleast has to be 3 months until we have solved the problem of reading older bookings
            //in a date range, then can probably drop back to 1 month
            //in MainFrm, the idea is: for older dates, if 'show cancelled' ticked, it needs to scan the whole table (maybe make user re-tick show cancelled when changing date range)
            return IsOpenStatus(status) || bookingDate.AddMonths(3) > DateTime.Now;
        }

        public static bool IsOpenStatus(BookingStates status)
        {
            switch (status)
            {
                case BookingStates.Completed:
                case BookingStates.Cancelled:
                case BookingStates.LostEnquiry:
                case BookingStates.CancelledWithoutPayment:
                    return false;
                case BookingStates.OpenEnquiry:
                case BookingStates.Booked:
                case BookingStates.PaymentDue:
                    return true;
                default:
                    throw new Exception($"Unknown if status {status} is open.");
            }
        }

        public string ValidationErrors()
        {
            ValidationContext context = new ValidationContext(this, null, null);
            IList<ValidationResult> validationErrors = new List<ValidationResult>();

            if (!Validator.TryValidateObject(this, context, validationErrors, true))
            {
                string missing = $"The {Status} is missing the following: ";
                foreach (ValidationResult result in validationErrors)
                    missing += $"{result.ErrorMessage}, ";
                missing = missing.Trim().TrimEnd(',');
                return missing;
            }

            //we are fussier about what is filled in if this is now a booking (but not much fussier, right?)
            if (IsBooked())
            {
                string missing = "";
                if (LocationRegion == LocationRegions.NotSet)
                    missing += "location region, ";
                if (BookingTime == 0)
                    missing += "time of service, ";
                if (Duration == 0)
                    missing += "service duration, ";
                if (!string.IsNullOrEmpty(missing))
                    return $"The booking is missing the following: " + missing.TrimEnd(',');
            }

            return null;
        }

        public string Summary()
        {
            return $"{Id} {BookingNickname} - {Status.ToString().ToUpper()} - {BookingDate.ToShortDateString()}";
        }

        public CalendarItemDTO ToCalendarItem()
        {
            return new CalendarItemDTO(int.Parse(Id), this.BookingName, this.BookingDate,
                this.BookingTime, Status);
        }
    }
}