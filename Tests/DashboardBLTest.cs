using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBRBooker.Business;

namespace Tests
{
    [TestClass]
    public class DashboardBLTest
    {

        [TestMethod]
        public void DashboardBLTest_GetWeekdayIfPossible_Friday()
        {
            //preferred Saturday, but toolate is Monday
            var result = DashboardBL.GetWeekdayIfPossible(new DateTime(2030, 1, 5),
                new DateTime(2020, 1, 7));
            result.Day.ShouldBe(4);
            result.DayOfWeek.ShouldBe(DayOfWeek.Friday);    //test will fail when we get to 2030
        }

        [TestMethod]
        public void DashboardBLTest_GetWeekdayIfPossible_Monday()
        {
            //preferred Saturday, but toolate is Monday
            var result = DashboardBL.GetWeekdayIfPossible(new DateTime(2030, 1, 6),
                new DateTime(2020, 1, 8));
            result.Day.ShouldBe(7);
            result.DayOfWeek.ShouldBe(DayOfWeek.Monday);
        }

    }
}
