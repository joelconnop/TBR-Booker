using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TBRBooker.Business;
using TBRBooker.Model.Entities;

namespace TBRBooker.FrontEnd
{
    public class ErrorHandler
    {
        public static void HandleError(IWin32Window owner, string action, Exception ex, bool isStable = false)
        {
            var errorLog = new ErrorLog()
            {
                Action = action,
                CreatedDate = DateTime.Now,
                ExceptionMsg = ex.Message,
                StackTrace = ex.StackTrace,
            };
            DBBox.AddOrUpdate(errorLog);

            MessageBox.Show(owner, (isStable ? "" : 
                "An unexpected error has occurred, and the program could be unstable. You should make a note of what you were working on (hint: Print Screen) and restart the program. ")
                + action + ": " + ex.Message, "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static List<ErrorLog> GetPastErrors(string action)
        {
            return DBBox.SearchErrorLog(action);
        }

    }
}
