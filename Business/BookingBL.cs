using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBRBooker.Business;
using TBRBooker.Model.Entities;
using System.ComponentModel.DataAnnotations;

namespace TBRBooker.Business
{
    public class BookingBL
    {
        public List<string> ValidateBooking(Booking booking)
        {
            var errors = new List<string>();
            errors.AddRange(booking.ValidationErrors());
            return errors;
        }

        public static Booking GetBookingFull(string id)
        {
            var booking = DBBox.ReadItem<Booking>(id);

            if (!string.IsNullOrEmpty(booking.CustomerId))
            {
                booking.Customer = DBBox.ReadItem<Customer>(booking.CustomerId);
            }
            if (!string.IsNullOrEmpty(booking.AccountId))
            {
                booking.Account = DBBox.ReadItem<CorporateAccount>(booking.AccountId);
            }

            return booking;
        }

        public static void SaveBookingEtc(Booking booking)
        {
            bool isNewBooking = string.IsNullOrEmpty(booking.Id);

            DBBox.AddOrUpdate(booking.Customer);

            try
            {
                DBBox.AddOrUpdate(booking);
            }
            catch (Amazon.DynamoDBv2.Model.ConditionalCheckFailedException ex)
            {
                //means the Id is already in use (could happen if system not used for a month)
                var nextId = Convert.ToInt32(booking.Id) + 1;
                while (DBBox.ReadItem<Booking>(nextId.ToString()) != null)
                {
                    nextId++;
                }
                booking.Id = nextId.ToString();
                DBBox.AddOrUpdate(booking);
            }
        }

        public static List<Booking> GetClashBookings(List<Booking> bookings, int startTime, int endTime)
        {
            //HACK: DOES NOT YET ACCOUNT FOR TRAVEL (this will be the hardest part. Will also need to store.
            //      But maybe its separate call to distinguish travel clash (different colour on timeline)
            return bookings.Where(x => (x.BookingTime >= startTime && x.EndTime <= endTime)
                || (startTime <= x.BookingTime && endTime <= x.EndTime)).ToList();
        }

        public static string GetNextBookingNum()
        {
            // int year = itm.BookingDate != null ? itm.BookingDate.Value.Year : DateTime.Now.Year;
            int year = DateTime.Now.Year;   //itm.BookingDate.Year;
            int minorNum = (int)Math.Ceiling((9.0 / 365.0) * DateTime.Now.DayOfYear) * 1000;
            int nextNum = int.Parse(year.ToString().Substring(2) + minorNum.ToString());
            //int nextYear = nextNum + minorNum;

            var relevantCalendarItems = DBBox.GetCalendarItems()
                .Where(x => x.BookingNum > nextNum)
                .Select(y => y.BookingNum).ToList();
            if (relevantCalendarItems.Count == 0)
                nextNum++;    //eg 181001
            else
                nextNum = relevantCalendarItems.Max() + 1;   //eg 180123

            return nextNum.ToString();
        }
    }
}
