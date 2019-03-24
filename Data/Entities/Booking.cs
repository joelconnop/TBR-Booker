using System;
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
        public const string RepeatingBookingId = "1337";    // has to be a number or expect crashes

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

        // ! PROCESS FOR ADDING NEW FIELDS !
        // consider sync in RepeatScheduleBL.CreateMarkersFromSchedule()
        // consider add to PookingPnl? If so, also consider BookingPnl.CopyBookingDetails()
        // objects that need initialising to do so in DayPanel

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

        /// <summary>
        /// hhmm
        /// </summary>
        public int EndTime => DTUtils.AddTimeInts(BookingTime, Duration);


        /// <summary>
        /// might as well store travel info, but it isn't entirely accurate (if booking A is changed
        /// and saved, booking B could have changed but not been saved)
        /// </summary>
        public int TravelDistance { get; set; }
        public int TravelTime { get; set; }
        [JsonIgnore]
        public int TravelStart => DTUtils.AddTimeInts(BookingTime, TravelTime * -1);

        


        public LocationRegions LocationRegion { get; set; }
        // public string VenueName { get; set; }   // now all part of (google places) address
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

        public Service Service { get; set; }

        public decimal AmountPaid => PaymentHistory == null ? 0 : PaymentHistory.Sum(x => x.Amount);
        public decimal Balance => Service == null ? 0
            : (PaymentHistory == null ? Service.TotalPrice 
            : Service.TotalPrice - AmountPaid);
        public bool IsFullyPaid => IsBooked(true) && Service != null && Service.TotalPrice > 0 && Balance == 0;

        public string QbListId { get; set; }

        public List<Payment> PaymentHistory {get; set;}

        public bool IsInvoiced { get; set; }

        public bool IsPayOnDay { get; set; }

        /// <summary>
        /// like a foreign key to the Followup object, except that there is no table to Followup
        /// </summary>
        public List<Followup> Followups { get; set; }

        public Followup ConfirmationCall { get; set; }

        public int EditSequence { get; set; }

        /// <summary>
        /// Typically the customer's surname or company name but freely changeable.
        /// Needs to be on booking so we can display on calendar without reading other tables
        /// Examples 'Smith', 'Little Jon Party', 'Browns GC', 'PCYC Bris'
        /// </summary>
        [Required(ErrorMessage = "Booking Name")]
        public string BookingNickname { get; set; }

        public List<string> HighlightedControls { get; set; }

        public string GoogleCalendarId { get; set; }

        public string EnquiryCredit { get; set; }
        public string SaleCredit { get; set; }

        private string ChooseNameForBooking()
        {
            if (!string.IsNullOrEmpty(BookingNickname))
            {
                return BookingNickname;
            }
            else if (Account != null)
            {
                return Account.BusinessName;
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

            var parsed = DTUtils.ParseTime(bookingTime);
            return $"{(parsed.Hour >= 10 ? parsed.Hour.ToString() : "0" + parsed.Hour.ToString())}:{(parsed.Minute >= 10 ? parsed.Minute.ToString() : "0" + parsed.Minute.ToString())}";
        }

        public static TimeSpan GetBookingTime(int bookingTime)
        {
            var parsed = DTUtils.ParseTime(bookingTime);
            return new TimeSpan(parsed.Hour, parsed.Minute, 0);
        }

        public static string CombineAddress(string address, string venue)
        {
            string combined = venue;
            if (!string.IsNullOrEmpty(combined) && !string.IsNullOrEmpty(address))
                combined += ", ";
            return combined + address;
        }

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

            //followup for calendar
            doc["followupDate"] = GetCurrentFollowup()?.FollowupDate.Ticks ?? 0;
            doc["confirmationDate"] = ConfirmationCall != null && ConfirmationCall.IsOutstanding()
                ? ConfirmationCall.FollowupDate.Ticks : 0;

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

        public static bool IsCancelled(BookingStates status)
        {
            switch (status)
            {
                case BookingStates.Booked:
                case BookingStates.Completed:
                case BookingStates.PaymentDue:
                case BookingStates.OpenEnquiry:
                    return false;
                case BookingStates.LostEnquiry:
                case BookingStates.Cancelled:
                case BookingStates.CancelledWithoutPayment:
                    return true;
                default:
                    throw new Exception($"Unknown if status {status} is a booking or enquiry.");
            }
        }

        public bool IsOpen()
        {
            //the program should also push user to cancelling/completing old deals/bookings (how about a spank per week for each unresolved booking status)
            return IsBookingOpen(Status, BookingDate, GetCurrentFollowup() != null
                || (!ConfirmationCall?.CompletedDate.HasValue ?? false));
        }

        public static bool IsBookingOpen(BookingStates status, DateTime bookingDate, bool isFollowupSet)
        {
            //it atleast has to be 3 months until we have solved the problem of reading older bookings
            //in a date range, then can probably drop back to 1 month
            //in MainFrm, the idea is: for older dates, if 'show cancelled' ticked, it needs to scan the whole table (maybe make user re-tick show cancelled when changing date range)
            return IsOpenStatus(status) || 
                bookingDate.AddMonths(Settings.Inst().MonthsForBookingHistory) > DateTime.Now;

            // NOTICED THAT isFollowupSet is not been used. Can revisit this once followups
            // in production are cleaned up
        }

        public bool BookingDateBeenAndGone(DateTime? compareDateIfOtherThanNow = null)
        {
            if (!compareDateIfOtherThanNow.HasValue)
                compareDateIfOtherThanNow = DTUtils.StartOfDay();

            if (!IsBooked())
                return false;

            return BookingDate <= compareDateIfOtherThanNow;
        }

        public bool IsOverdue(DateTime? compareDateOtherThanNow = null)
        {
            if (!compareDateOtherThanNow.HasValue)
                compareDateOtherThanNow = DTUtils.StartOfDay();

            if (Status == BookingStates.PaymentDue)
                return true;

            if (!BookingDateBeenAndGone(compareDateOtherThanNow))
                return false;

            return !IsFullyPaid;
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

        public Followup GetCurrentFollowup()
        {
            if (Followups == null)
                return null;
            return Followups.SingleOrDefault(x => x.IsOutstanding());
        }

        public string ValidationErrors()
        {
            ValidationContext context = new ValidationContext(this, null, null);
            IList<ValidationResult> validationErrors = new List<ValidationResult>();

            if (!Validator.TryValidateObject(this, context, validationErrors, true))
            {
                string criticalMissing = $"The {Status} is missing the following: ";
                foreach (ValidationResult result in validationErrors)
                    criticalMissing += $"{result.ErrorMessage}, ";
                criticalMissing = criticalMissing.Trim().TrimEnd(',');
                return criticalMissing;
            }

            string missing = "";

            if (BookingDate == default(DateTime))
                missing += "booking date, ";
            if (string.IsNullOrEmpty(BookingName))
                missing += "booking name, ";
            if (string.IsNullOrEmpty(Customer.DirectoryName()))
                missing += "at least one customer detail for the customer directory, ";

            //we are fussier about what is filled in if this is now a booking (but not much fussier, right?)
            if (IsBooked())
            {
                if (LocationRegion == LocationRegions.NotSet)
                    missing += "location region, ";
                if (BookingTime == 0)
                    missing += "time of service, ";
                if (Duration == 0)
                    missing += "service duration, ";
               
            }

            if (!string.IsNullOrEmpty(missing))
                return $"The booking is missing the following: " + missing.Trim().TrimEnd(',');

            return null;
        }

        public string Summary()
        {
            return $"{Id} {BookingNickname} - {Status.ToString().ToUpper()} - {BookingDate.ToShortDateString()}";
        }

        public string GoogleEventSummary()
        {
            return GoogleCalendarItemDTO.TBRBooking + Service.ToString() + "; " + Customer.SmartName() + "; " + Customer.SmartPhone();
        }

        public BookingCalendarItemDTO ToCalendarItem()
        {
            return new BookingCalendarItemDTO(int.Parse(Id), this.BookingName, this.BookingDate,
                this.BookingTime, Status, GetCurrentFollowup()?.FollowupDate.Date ?? null,
                (ConfirmationCall != null && ConfirmationCall.IsOutstanding())
                ? ConfirmationCall?.FollowupDate : null);
        }

        public bool IsFullyRead()
        {
            return (string.IsNullOrEmpty(CustomerId) || Customer != null)
                && (string.IsNullOrEmpty(AccountId) || Account != null);
        }

        public int BookingNum()
        {
            int num;
            if (Id == null)
                throw new Exception("Booking Id should never be null!");

            if (int.TryParse(Id, out num))
                return num;
            else
                throw new Exception($"Booking Id {Id} is not a number!");

        }
    }
}