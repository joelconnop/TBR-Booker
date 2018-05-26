using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TBRBooker.Business;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class TheGoogleTest
    {

        [TestMethod]
        public void ConnectAndDisplay()
        {
            var result = TheGoogle.GetGoogleCalendar(
                DateTime.Now, DateTime.Now.AddMonths(1));

            result.ForEach(x => Console.WriteLine(x));
        }
    }
}
