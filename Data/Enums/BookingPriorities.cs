using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBRBooker.Model.Enums
{
    public enum BookingPriorities
    {
        [Description("Example high dollar value, or important business relationship")]
        HighestImportance = 0,
        [Description("The customer is likely to proceed with booking if followed up")]
        KeenCustomer = 1,
        [Description("Standard followup procedures will suffice")]
        Standard = 2,
        [Description("If we don't intend to chase this one up for whatever reason")]
        PossibleTyreKicker = 3,
    }
}
