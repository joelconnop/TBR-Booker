using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBRBooker.Model.DTO;
using TBRBooker.Model.Entities;

namespace TBRBooker.Business
{
    public class CustomerBL
    {


        public static List<ExistingCustomerDTO> SearchCustomers(string searchTerm)
        {
            var directory = DBBox.GetCustomerDirectory();
            var matches = new List<ExistingCustomerDTO>();

            foreach (var c in directory)
            {
                string cname = c.DirectoryName.Replace(" from ", " ");
                if (cname.ToLower().Contains(searchTerm.ToLower()))
                    matches.Add(c);
            }

            return matches;
        }


        /// <summary>
        /// Can call this all we like because only the first time will need to hit database
        /// Careful - this does not read the related records for booking (careful about BookingPnl implications
        /// if changing to read full booking)
        /// </summary>
        /// <returns></returns>
        public static List<Booking> GetPastBookings(Customer customer,
            CorporateAccount corpAcct, Booking beforeThisOneOnly = null)
        {
            var otherBookings = new List<Booking>();

            if (customer != null)
            {
                customer.BookingIds.ForEach(x => otherBookings.Add(DBBox.ReadItem<Booking>(x)));
            }

            if (corpAcct != null)
            {
                corpAcct.BookingIds.ForEach(x =>
                {
                    if (!otherBookings.Any(y => y.Id == x))
                        otherBookings.Add(DBBox.ReadItem<Booking>(x));
                });
            }

            if (beforeThisOneOnly != null)
                // only want to consider bookings that are 'before' this one
                // ('before' meaning either the Id or the booking date is before this one)
                return otherBookings.Where(x => x.Id != beforeThisOneOnly.Id
                    && (x.BookingNum() < beforeThisOneOnly.BookingNum() ||
                        x.BookingDate < beforeThisOneOnly.BookingDate)).ToList();
            else
                return otherBookings;
        }

    }
}
