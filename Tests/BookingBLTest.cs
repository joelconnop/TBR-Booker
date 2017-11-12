using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBRBooker.Business;
using TBRBooker.Model;
using TBRBooker.Model.Enums;
using TBRBooker.Model.Entities;

namespace Tests
{
    [TestClass]
    public class BookingBLTest
    {
        [TestMethod]
        public void WriteBookings()
        {

            //old Booking
            BookingBL.SaveBookingEtc(new Booking
            {
                CustomerId = "636448923715552119-29/10/2017-101",
                Customer = DBBox.ReadItem<Customer>("636448923715552119-29/10/2017-101"),
                Status = BookingStates.LostEnquiry,
                LostJobReason = LostJobReasons.Test,
                Priority = BookingPriorities.PossibleTyreKicker,
                BirthdayName = "little Joel",
                BirthdayAge = 6,
                BookingDate = DateTime.Now.AddMonths(-3),
                BookingTime = 1430,
              //  TimeSlot = TimeSlots.PM,
                LocationRegion = LocationRegions.GoldCoastCentral,
                Address = new Address("", "1 Test St", "Trashmoar", "Qld", "4214"),
                BookingNickame = "ConnTest",
            });

            //current booking
            BookingBL.SaveBookingEtc(new Booking
            {
                CustomerId = "636448923715552119-29/10/2017-101",
                Customer = DBBox.ReadItem<Customer>("636448923715552119-29/10/2017-101"),
                Status = BookingStates.Booked,
                Priority = BookingPriorities.HighestImportance,
                BirthdayName = "Child A",
                BirthdayAge = 10,
                BookingDate = DateTime.Now.AddDays(1),
                BookingTime = 1200,
            //    TimeSlot = TimeSlots.Midday,
                LocationRegion = LocationRegions.Brisbane,
                Address = new Address("", "2 Test St", "Trashless", "Qld", "4214"),
                BookingNickame = "BigjobA",
            });

            //open lead
            BookingBL.SaveBookingEtc(new Booking
            {
                CustomerId = "636448923717452227-29/10/2017-102",
                Customer = DBBox.ReadItem<Customer>("636448923717452227-29/10/2017-102"),
                Status = BookingStates.OpenEnquiry,
                Priority = BookingPriorities.Standard,
                BirthdayName = "Child B",
                BirthdayAge = 10,
                BookingDate = DateTime.Now.AddDays(-7),
                BookingTime = 1645,
              //  TimeSlot = TimeSlots.PM,
                LocationRegion = LocationRegions.GoldCoastSouth,
                Address = new Address("", "2 Test St", "Trashless", "Qld", "4214"),
                BookingNickame = "JobB",
            });

            //cancelled job
            BookingBL.SaveBookingEtc(new Booking
            {
                CustomerId = "636448923717452227-29/10/2017-102",
                Customer = DBBox.ReadItem<Customer>("636448923717452227-29/10/2017-102"),
                Status = BookingStates.Cancelled,
                Priority = BookingPriorities.KeenCustomer,
                BirthdayName = "Child C",
                BirthdayAge = 7,
                BookingDate = DateTime.Now.AddDays(3),
                BookingTime = 900,
             //   TimeSlot = TimeSlots.AM,
                LocationRegion = LocationRegions.GoldCoastNorth,
                Address = new Address("", "2 Test St", "Trashless", "Qld", "4214"),
                BookingNickame = "SomeKidsSurname",
            });
        }

        //   [TestMethod]
        //   public void ReadBookings()
        //   {
        //       var bookings = DBBox.BookingsCache();
        //       bookings.Count.ShouldBe(3);
        ////       bookings.Where(x => x.Address.Suburb.Equals("Trashmoar")).Count().ShouldBe(1);
        //       bookings.Where(x => x.BookingNickame.Equals("JobB")).Count().ShouldBe(1);
        //       bookings.Where(x => x.BirthdayName.Equals("Child C")).Count().ShouldBe(1);
        //       bookings.Where(x => x.Status == BookingStatus.Booked).Count().ShouldBe(1);
        //   }

        [TestMethod]
        public void UpdateBooking()
        {
            var booking = DBBox.ReadItem<Booking>("171001");
            booking.Status = BookingStates.LostEnquiry;
            booking.LostJobReason = LostJobReasons.Test;
            booking.BookingDate = booking.BookingDate.AddMonths(-3);
            BookingBL.SaveBookingEtc(booking);
        }
    }
}
