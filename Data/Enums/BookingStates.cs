using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBRBooker.Model.Enums
{
    public enum BookingStates
    {
        OpenEnquiry,
        LostEnquiry,
        Booked,
        Completed,
        Cancelled,
        PaymentDue
    }
}
