using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TBRBooker.Business;
using TBRBooker.Model.Entities;
using TBRBooker.Model.Enums;
using TBRBooker.Model;
using Shouldly;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class DBBoxTest
    {

        [TestMethod]
        public void IsInUnitTest()
        {
            DBBox.InitDynamoDbClient();
            DBBox.IsTestEnvironment.ShouldBe(true);
        }


        [TestMethod]
        public void GetCalendarItems()
        {
            var calendar = DBBox.GetCalendarItems(false);
            calendar.Count.ShouldBe(3);
            calendar.Where(x => x.BookingName.Equals("JobB")).Count().ShouldBe(1);
            calendar.Where(x => x.BookingStatus == BookingStates.Booked).Count().ShouldBe(1);
        }

        [TestMethod]
        public void AddCustomers()
        {
            DBBox.AddOrUpdate(new Customer
            {
                FirstName = "Test",
                LastName = "Connop",
                CompanyName = "TestComp",
                PrimaryNumber = "0412345678",
                EmailAddress = "joel.connop@gmail.com",
                CreatedDate = DateTime.Now
            });

            DBBox.AddOrUpdate(new Customer
            {
                FirstName = "Typical",
                LastName = "Mum",
                PrimaryNumber = "0412345678",
                CreatedDate = DateTime.Now
            });
        }

  
        [TestMethod]
        public void GetCustomerDirectory()
        {
            var calendar = DBBox.GetCustomerDirectory();
            calendar.Count.ShouldBe(2);
            calendar.Where(x => x.DirectoryName.Contains("gmail")).Count().ShouldBe(1);
            calendar.Where(x => x.DirectoryName.Contains("Mum")).Count().ShouldBe(1);
            calendar.ForEach(x => x.CustomerId.ShouldNotBeNullOrEmpty());
        }

    }
}
