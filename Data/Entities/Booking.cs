using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using TBRBooker.Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace TBRBooker.Model.Entities
{
    public class Booking : BaseItem
    {
        [Required(ErrorMessage = "A Booking Date is required")]
        public Customer Customer { get; set; }

        public CorporateAccount Account { get; set; }
        public BookingStatus Status { get; set; }
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

        /// <summary>
        /// Examples 'Smith', 'Little Jon Party', 'Browns GC', 'PCYC Bris'
        /// </summary>
        public string BookingNickame { get; set; }

        private string ChooseNameForBooking()
        {
            if (!string.IsNullOrEmpty(BookingNickame))
                return BookingNickame;
            else if (Account != null)
                return Account.CompanyName;
            else if (!string.IsNullOrEmpty(LastName))
                return LastName;
            else
                return FirstName;
        }

        public Booking(string tableName) : base("booking")
        {
        }

        public override Dictionary<string, AttributeValue> WriteAttributes()
        {
            var atts = new Dictionary<string, AttributeValue>();
            SetIdIfNeeded();
            AddAttribute(atts, "id", new AttributeValue { S = Id }, Id);
            AddAttribute(atts, "CustomerId", new AttributeValue { S = FirstName }, FirstName);
            AddAttribute(atts, "lastName", new AttributeValue { S = LastName }, LastName);
            AddAttribute(atts, "mobileNumber", new AttributeValue { S = MobileNumber }, MobileNumber);
            AddAttribute(atts, "otherNumbers", new AttributeValue { S = OtherNumbers }, OtherNumbers);
            AddAttribute(atts, "emailAddress", new AttributeValue { S = EmailAddress }, EmailAddress);
            return atts;
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
            if (IsBooked)
            {

            }

            return errors;
        }
    }
}