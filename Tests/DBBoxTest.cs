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
        public void WriteBookings()
        {

            //old Booking
            DBBox.WriteItem(new Booking
            {
                CustomerId = "89f5e12e-8e37-49d7-84ee-becb1e216761",
                Status = BookingStatus.OpenLead,
                Priority = BookingPriorities.PossibleTyreKicker,
                BirthdayName = "little Joel",
                BirthdayAge = 6,
                BookingDate = DateTime.Now.AddMonths(-3),
                BookingTime = 1430,
                LocationRegion = LocationRegions.GoldCoastCentral,
                Address = new Address("", "1 Test St", "Trashmoar", "Qld", "4214"),
                BookingNickame = "ConnTest",
            });

            //current booking
            DBBox.WriteItem(new Booking
            {
                CustomerId = "89f5e12e-8e37-49d7-84ee-becb1e216761",
                Status = BookingStatus.Booked,
                Priority = BookingPriorities.HighestImportance,
                BirthdayName = "Child A",
                BirthdayAge = 10,
                BookingDate = DateTime.Now.AddDays(1),
                BookingTime = 1200,
                LocationRegion = LocationRegions.Brisbane,
                Address = new Address("", "2 Test St", "Trashless", "Qld", "4214"),
                BookingNickame = "BigjobA",
            });

            //open lead
            DBBox.WriteItem(new Booking
            {
                CustomerId = "89f5e12e-8e37-49d7-84ee-becb1e216761",
                Status = BookingStatus.OpenLead,
                Priority = BookingPriorities.Standard,
                BirthdayName = "Child B",
                BirthdayAge = 10,
                BookingDate = DateTime.Now.AddDays(-7),
                BookingTime = 1645,
                LocationRegion = LocationRegions.GoldCoastSouth,
                Address = new Address("", "2 Test St", "Trashless", "Qld", "4214"),
                BookingNickame = "JobB",
            });

            //cancelled job
            DBBox.WriteItem(new Booking
            {
                CustomerId = "89f5e12e-8e37-49d7-84ee-becb1e216761",
                Status = BookingStatus.Cancelled,
                Priority = BookingPriorities.KeenCustomer,
                BirthdayName = "Child C",
                BirthdayAge = 7,
                BookingDate = DateTime.Now.AddDays(3),
                BookingTime = 900,
                LocationRegion = LocationRegions.GoldCoastNorth,
                Address = new Address("", "2 Test St", "Trashless", "Qld", "4214"),
                BookingNickame = "SomeKidsSurname",
            });
        }

        [TestMethod]
        public void ReadBookings()
        {
            var bookings = DBBox.GetBookingsForCalendar();
            bookings.Count.ShouldBe(3);
     //       bookings.Where(x => x.Address.Suburb.Equals("Trashmoar")).Count().ShouldBe(1);
            bookings.Where(x => x.BookingNickame.Equals("JobB")).Count().ShouldBe(1);
            bookings.Where(x => x.BirthdayName.Equals("Child C")).Count().ShouldBe(1);
            bookings.Where(x => x.Status == BookingStatus.Booked).Count().ShouldBe(1);
        }

        [TestMethod]
        public void UpdateBooking()
        {
            var booking = DBBox.ReadItem<Booking>("14/10/2017-124979da-7434-49c1-b93d-ee364945");
            booking.Status = BookingStatus.LostLead;
            booking.LostDealReason = LostDealReasons.Test;
            DBBox.WriteItem(booking);
        }

    }
}
