using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBRBooker.Model.Entities;

namespace TBRBooker.Business
{
    public class ErrorLogger
    {
        public static string LogError(string action, Exception ex)
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

            return dbError;
        }
    }
}
