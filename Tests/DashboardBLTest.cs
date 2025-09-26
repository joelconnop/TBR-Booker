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
            var result = DashboardBL.GetWeekdayIfPossible(new DateTime(2020, 7, 25),
                new DateTime(2020, 7, 27));
            result.Day.ShouldBe(24);
            result.DayOfWeek.ShouldBe(DayOfWeek.Friday);    //test will fail when we get to 2030
        }

        [TestMethod]
        public void DashboardBLTest_GetWeekdayIfPossible_Monday()
        {
            //preferred Saturday, but toolate is Monday
            var result = DashboardBL.GetWeekdayIfPossible(new DateTime(2020, 7, 26),
                new DateTime(2020, 7, 28));
            result.Day.ShouldBe(27);
            result.DayOfWeek.ShouldBe(DayOfWeek.Monday);
        }

    }
}
