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

            string dbError = "";
            try
            {
                DBBox.AddOrUpdate(errorLog);
            }
            catch (Exception dbEx)
            {
                dbError = dbEx.Message;
            }

            var msg = isStable ? "" :
                "An unexpected error has occurred, and the program could be unstable. You should make a note of what you were working on (hint: Print Screen) and restart the program."
                + Environment.NewLine + Environment.NewLine + action + ": "
                + Environment.NewLine + Environment.NewLine + ex.Message;
            if (!string.IsNullOrEmpty(dbError))
            {
                msg += Environment.NewLine + Environment.NewLine + "Additionally, this error failed to log because:"
                + Environment.NewLine + dbError;
            }
            MessageBox.Show(owner, msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static List<ErrorLog> GetPastErrors(string action)
        {
            return DBBox.SearchErrorLog(action);
        }

    }
}
