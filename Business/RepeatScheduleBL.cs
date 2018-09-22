using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBRBooker.Base;
using TBRBooker.Model.DTO;
using TBRBooker.Model.Entities;

namespace TBRBooker.Business
{
    public class RepeatScheduleBL
    {

        private static List<RepeatMarkerDTO> _repeatMarkers { get; set; }

        private static (DateTime Start, DateTime End) CalendarRange { get; set; }

        public static BookingCalendarItemDTO
            ConfirmBookingFromRepeatMarker(RepeatMarkerDTO marker)
        {
            var booking = marker.HypotheticalBooking;
            booking.Id = BookingBL.GetNextBookingNum();
            booking.Status = Model.Enums.BookingStates.Booked;
            // No longer just hypothetical :) this call also updates it on Bookings Calendar and updates customer booking id list
            BookingBL.SaveBookingEtc(booking);  
            _repeatMarkers.Remove(marker);
            return DBBox.GetCalendarItems(false, false)
                .Single(x => x.BookingNum.ToString() == booking.Id);
        }

        public static void RejectRepeatBooking(RepeatMarkerDTO marker, string reason)
        {
            var booking = marker.HypotheticalBooking;
            var schedule = DBBox.ReadItem<RepeatSchedule>(marker.ScheduleId);
            schedule.Cancellations.Add((marker.Date, reason));
            DBBox.AddOrUpdate(schedule);
            _repeatMarkers.Remove(marker);
        }

        private static List<RepeatMarkerDTO> GetMarkersInRange(
            DateTime startDate, DateTime endDate)
        {
            return _repeatMarkers.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();
        }

        public static List<RepeatMarkerDTO> GetMarkersInRange(
            DateTime startDate, DateTime endDate, bool isForceReadAll)
        {
            // can we use the cached calendar ?
            if (_repeatMarkers != null && !isForceReadAll &&
                startDate >= CalendarRange.Start && endDate <= CalendarRange.End)
                return GetMarkersInRange(startDate, endDate);

            (DateTime start, DateTime end) newSearchRange;

            if (_repeatMarkers == null || isForceReadAll ||
                (startDate < CalendarRange.Start && endDate > CalendarRange.End))
            {
                // create a new Calendar if it has been requested, or if the new range more than
                // completely encompasses the old one
                _repeatMarkers = new List<RepeatMarkerDTO>();
                newSearchRange = (startDate, endDate);
                CalendarRange = (startDate, endDate);
            }
            else if (startDate >= CalendarRange.Start && endDate <= CalendarRange.End)
                throw new Exception("Unexpected attempt to get repeat markers inside of cached results: "
                    + $"{CalendarRange.Start} - {CalendarRange.End} encompasses {startDate} - {endDate}.");

            // for all other cases, only search the time period not searched previously
            else if (startDate >= CalendarRange.Start)
            {
                newSearchRange = (CalendarRange.End.AddHours(0.1), endDate);
                CalendarRange = (CalendarRange.Start, endDate);
            }
            else if (endDate <= CalendarRange.End)
            {
                newSearchRange = (startDate, CalendarRange.Start.AddHours(-0.1));
                CalendarRange = (startDate, CalendarRange.End);
            }
            else
            {
                throw new Exception("Unexpected Repeat Marker date ranges: "
                   + $"{CalendarRange.Start} - {CalendarRange.End}, {startDate} - {endDate}.");
            }

            // create list
            foreach (var schedule in DBBox.GetRepeatSchedules())
            {
                var markers = CreateMarkersFromSchedule(schedule, newSearchRange.start, newSearchRange.end);
                foreach (var m in markers)
                {
                    if (_repeatMarkers.Any(y => y.ScheduleId.Equals(schedule.Id) &&
                        y.Date == m.Date))
                        throw new Exception("Tried to add marker to cache twice: " + m.ToString());
                }
                _repeatMarkers.AddRange(markers);
            }

            return GetMarkersInRange(startDate, endDate);
        }

        private static List<RepeatMarkerDTO> CreateMarkersFromSchedule(
            RepeatSchedule schedule, DateTime startDate, DateTime endDate)
        {
            var markers = new List<RepeatMarkerDTO>();

            if (schedule.StartDate < startDate)
                return markers;

            var customer = DBBox.ReadItem<Customer>(schedule.CustomerId);

            // don't need the 'full' booking, but we aren't willing to read one (and cache it)
            // without reading its dependencies too
            var lastBooking = BookingBL.GetBookingFull(customer.BookingIds.Last());
            var bookingsInRange = DBBox.GetCalendarItems(false, false)
                .Where(x => x.Name.Trim().Equals(lastBooking.BookingName.Trim(),
                StringComparison.InvariantCultureIgnoreCase)
                && x.Date >= startDate && x.Date <= endDate).ToList();

            var currentDate = new DateTime(Math.Max(DTUtils.StartOfDay().Ticks,
                DTUtils.StartOfDay(startDate).Ticks));
            int weeksSinceLast = 1;
            while (currentDate < endDate)
            {
                if (!schedule.IsByDayOfWeek)
                    throw new Exception("Not defined how to schedule without a day of week.");

                if (currentDate.DayOfWeek == schedule.RepeatDay
                    && (schedule.WeekNumOfEveryMonth == 0 ||
                    DTUtils.WeekOfMonthForDay(currentDate) == schedule.WeekNumOfEveryMonth))
                {
                    if (weeksSinceLast == schedule.Frequency)
                    {
                        if (!schedule.Cancellations.Any(x => x.Date == currentDate)
                            && !bookingsInRange.Any(x => x.Date == currentDate))
                        {
                            var placeholder = new Booking()
                            {
                                Id = Booking.RepeatingBookingId,
                                IsNewBooking = true,
                                Customer = customer,
                                CustomerId = customer.Id,
                                Account = lastBooking.Account,
                                AccountId = lastBooking.AccountId,
                                Status = Model.Enums.BookingStates.OpenEnquiry,
                                Priority = lastBooking.Priority,
                                BookingDate = currentDate,
                                BookingTime = lastBooking.BookingTime,
                                Duration = lastBooking.Duration,
                                LocationRegion = lastBooking.LocationRegion,
                                VenueName = lastBooking.VenueName,
                                Address = lastBooking.Address,
                                BookingNotes = lastBooking.BookingNotes,
                                PaymentHistory = new List<Payment>(),
                                Followups = new List<Followup>(),
                                Service = (Service)lastBooking.Service.Clone(),  // also clones price items
                                HighlightedControls = new List<string>(),
                                IsPayOnDay = lastBooking.IsPayOnDay,
                                BookingNickname = lastBooking.BookingNickname,
                                EnquiryCredit = lastBooking.EnquiryCredit,
                                SaleCredit = lastBooking.SaleCredit,
                            };

                            markers.Add(new RepeatMarkerDTO(lastBooking.BookingName,
                                currentDate, lastBooking.BookingTime,
                                schedule.Id, placeholder));
                        }
                        weeksSinceLast = 1;
                    }
                    else
                    {
                        weeksSinceLast++;
                    }
                }

                currentDate = DTUtils.StartOfDay(currentDate.AddDays(1));
            }

            return markers;
        }
    }
}
