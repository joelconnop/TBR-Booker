using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBRBooker.Model.Enums
{
    public enum PaymentMethods
    {
        NotSet,
        [Description("CC")]
        CC,
        [Description("DD")]
        DD,
        Cash,
        Cheque,
        Refund,
        Reversal
    }
}
