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

namespace TBRBooker.Model.Entities
{
    public class Booking : BaseItem
    {
        public const string TABLE_NAME = "booking";
        public const string IS_OPEN_STR = "Y";

        [JsonIgnore]
        public Customer Customer { get; set; }

        public string CustomerId { get; set; }

        /// <summary>
        /// A little bit redundant with Customer.Company, but might need to later change Customer's account - but Booking's account is a historic value
        /// </summary>
        [JsonIgnore]
        public CorporateAccount Company { get; set; }

        public string CompanyId { get; set; }

        public BookingStatus Status { get; set; }

        [JsonIgnore]
        private bool IsOpenInDatabase { get; set; }

        public BookingPriorities Priority { get; set; }

        public string BirthdayName { get; set; }
        public int BirthdayAge { get; set; }

        [Required(ErrorMessage = "Booking Date")]
        public DateTime? BookingDate { get; set; }

        /// <summary>
        /// eg 1015 = 10:15, 
        /// </summary>
        public int BookingTime { get; set; }

        /// <summary>
        /// in minutes
        /// </summary>
        public int Duration { get; set; }

        public int EndTime => BookingTime + Duration;

        [Required(ErrorMessage = "Time Slot")]
        public TimeSlots TimeSlot { get; set; }

        public LocationRegions LocationRegion { get; set; }
        public string VenueName { get; set; }
        public Address Address { get; set; }

        /// <summary>
        /// this comes from customer, when relevant (eg. Gold Coast City Council)
        /// </summary>
        public string PurchaseOrderRef { get; set; }

        public List<string> Notes { get; set; }

        public string BookingName => ChooseNameForBooking();

        public LostDealReasons LostDealReason { get; set; }

        /// <summary>
        /// Typically the customer's surname or company name but freely changeable.
        /// Needs to be on booking so we can display on calendar without reading other tables
        /// Examples 'Smith', 'Little Jon Party', 'Browns GC', 'PCYC Bris'
        /// </summary>
        public string BookingNickame { get; set; }

        private string ChooseNameForBooking()
        {
            if (!string.IsNullOrEmpty(BookingNickame))
            {
                return BookingNickame;
            }
            else if (Company != null)
            {
                return Company.CompanyName;
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

        public Booking()
        {
            TableName = TABLE_NAME;
        }

        //public Booking(string tableName) : base("booking")  //, "bookingDateTicks")
        //{

        //}

        

        public override Dictionary<string, AttributeValue> WriteAttributes()
        {
            //Filter = BookingDate.Value.Ticks.ToString();
            var atts = base.WriteAttributes();
            AddAttribute(atts, "customerId", new AttributeValue { S = CustomerId }, CustomerId);
            var bookingDateTicks = GetBookingDateTicks().ToString();
            AddAttribute(atts, "bookingDate", new AttributeValue { N = bookingDateTicks }, bookingDateTicks);
            AddAttribute(atts, "status", new AttributeValue { N = Convert.ToInt32(Status).ToString() }, Convert.ToInt32(Status).ToString());
            if (IsOpen())
                AddAttribute(atts, "isOpen", new AttributeValue { S = IS_OPEN_STR }, IS_OPEN_STR);
            else
                throw new Exception("use case for intially closed booking?");
            return atts;
        }

        public override UpdateItemRequest GetUpdateRequest()
        {
            //Filter = BookingDate.Value.Ticks.ToString();
            var request = base.GetUpdateRequest();

            //update the status
            request.UpdateExpression += ", #s = :newstatus";
            request.ExpressionAttributeNames.Add("#s", "status");
            request.ExpressionAttributeValues.Add("newstatus", new AttributeValue { N = Convert.ToInt32(Status).ToString() });

            //update the booking date
            request.UpdateExpression += ", #b = :bookingdate";
            request.ExpressionAttributeNames.Add("#b", "bookingDate");
            request.ExpressionAttributeValues.Add("bookingdate", new AttributeValue { N = GetBookingDateTicks().ToString() });

            //open or close the booking if needed (for super efficient table scanning)
            if (IsOpen() && !IsOpenInDatabase)
            {
                //make the booking super fast to scan
                request.UpdateExpression += " ADD #o = :openstr";
                request.ExpressionAttributeNames.Add("#o", "isOpen");
                request.ExpressionAttributeValues.Add("openstr", new AttributeValue { S = IS_OPEN_STR });
            }
            else if (!IsOpen() && IsOpenInDatabase)
            {
                //remove the booking from super fast scans
                request.UpdateExpression += " REMOVE #o = :openstr";
                request.ExpressionAttributeNames.Add("#o", "isOpen");
            }

            return request;
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
            IsOpenInDatabase = doc["isOpen"].AsString().Equals(IS_OPEN_STR);
        }

        private long GetBookingDateTicks()
        {
            if (BookingDate.HasValue)
            {
                return BookingDate.Value.Ticks;
            }
            else
            {
                return DateTime.MaxValue.Ticks;
            }
        }

        public bool IsBooked()
        {
            return IsBooked(Status);
        }

        public static bool IsBooked(BookingStatus status)
        {
            switch (status)
            {
                case BookingStatus.Booked:
                case BookingStatus.Completed:
                case BookingStatus.Cancelled:
                    return true;
                case BookingStatus.OpenLead:
                case BookingStatus.LostLead:
                    return false;
                default:
                    throw new Exception($"Unknown if status {status} is a booking or lead.");
            }
        }

        public bool IsOpen()
        {
            //the program should also push user to cancelling/completing old deals/bookings (how about a spank per week for each unresolved booking status)
            return IsOpenStatus(Status) || BookingDate.Value.AddMonths(1) > DateTime.Now;
        }

        public static bool IsOpenStatus(BookingStatus status)
        {
            switch (status)
            {
                case BookingStatus.Completed:
                case BookingStatus.Cancelled:
                case BookingStatus.LostLead:
                    return false;
                case BookingStatus.Booked:
                case BookingStatus.OpenLead:
                    return true;
                default:
                    throw new Exception($"Unknown if status {status} is open.");
            }
        }

        public List<string> ValidationErrors()
        {
            ValidationContext context = new ValidationContext(this, null, null);
            IList<ValidationResult> validationErrors = new List<ValidationResult>();

            if (!Validator.TryValidateObject(this, context, validationErrors, true))
            {
                string missing = $"The {Status} is missing the following: ";
                foreach (ValidationResult result in validationErrors)
                    missing += $"{result.ErrorMessage}, ";
                missing = missing.Trim().TrimEnd(',');
            }
            
            //we are fussier about what is filled in if this is now a booking
            if (IsBooked())
            {

            }

            return new List<string>();
        }
    }
}