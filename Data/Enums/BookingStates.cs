using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBRBooker.Model.Enums
{
    public enum BookingStates
    {
        [Description("Open Enquiry")]
        OpenEnquiry,
        [Description("Lost Enquiry")]
        LostEnquiry,
        [Description("Booked")]
        Booked,
        [Description("Completed")]
        Completed,
        [Description("Cancelled")]
        Cancelled,
        [Description("Payment Due")]
        PaymentDue,
        [Description("Never Paid")]
        CancelledWithoutPayment
    }
}
