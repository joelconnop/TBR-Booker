using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBRBooker.Model.Enums
{
    public enum Penalties
    {
        General = 0,
        MissedFollowup = 2,
        MissedConfirmationCall = 3,
        PayAndClose7DaysLate = 4,
        PayAndCloseVeryLate = 5,
        MissedRecurringConfirmation = 6,
    }
}
