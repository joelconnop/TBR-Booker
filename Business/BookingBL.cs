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
            List<string> errors;
            
        }

        public static void SaveBookingEtc(Booking booking)
        {
            DBBox.WriteItem(booking.Customer)
        }
    }
}
