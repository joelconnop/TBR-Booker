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
    public class CalendarBL
    {

        

        public static List<GoogleCalendarItemDTO> GetGoogleEventsForMainCalendar
            (bool isForceReadAll, DateTime minStart, DateTime minEnd)
        {
            DateTime start;
            if (isForceReadAll)
            {
                start = new DateTime(2018, 7, 1);   // date from which google calendar feature went live
            }
            else
            {
                start = DateTime.Now.AddMonths(-1);
            }
            start = DTUtils.StartOfDay(DTUtils.Min(start, minStart));
            DateTime end = DTUtils.StartOfDay(DTUtils.Max(DateTime.Now, minEnd))
                .AddHours(23.9);
            return TheGoogle.GetGoogleCalendar(start, end, isForceReadAll);
        }

        public static List<BookingCalendarItemDTO> BuildCalendarFromDbRead
            (List<BookingCalendarItemDTO> dbCalendar, bool isForceReadAll)
        {
            var calendar = new List<BookingCalendarItemDTO>();

            //update calendar items that are out of date due to the passing of time
            foreach (var item in dbCalendar)
            {
                if (isForceReadAll)
                {
                    calendar.Add(item);
                }
                else
                {
                    var revisedItem = BookingBL.UpdateCalendarItemAndUnderlyingBooking(item);
                    if (revisedItem != null)    //null means the item should no longer be shown on calendars
                        calendar.Add(revisedItem);
                }
            }

            return calendar;
        }

        // see GoogleEventFrm.GetValue()
        //public static GoogleCalendarItemDTO CreateNewGoogleEvent(
        //string name, string description, DateTime start, int time, int duration, bool isBooking)
        //{
        //    List<string> attendeesNotImplemented = null;
        //    var calItem = new GoogleCalendarItemDTO(start, time, duration, name, description,
        //        attendeesNotImplemented, Blockout + Guid.NewGuid().ToString());
        //    TheGoogle.AddGoogleCalendarEvent(calItem);
        //    return calItem;
        //}

        public static string AddOrUpdateBookingOnGoogle(Booking booking)
        {
            if (Settings.Inst().IsTestMode)
                return Guid.NewGuid().ToString();

            List<string> attendeesNotImplemented = null;

            bool isDeleteFromCalendar = !booking.IsBooked();
            if (isDeleteFromCalendar)
            {
                if (!string.IsNullOrEmpty(booking.GoogleCalendarId))
                {
                    try
                    {
                        TheGoogle.DeleteGoogleCalendarEvent(booking.GoogleCalendarId);
                    }
                    catch (Exception ex)
                    {
                        // log it, but don't want to interrupt user or fail their operation because of this
                        ErrorLogger.LogError($"Adding or Updating Booking {booking.Id} on Google Calendar", ex);
                    }
                }
                return null;
            }

            // a bit messy, creating our google calendar item from a booking, but we won't add it to our calendar as one of these
            var calItem = new GoogleCalendarItemDTO(booking.BookingDate,
                booking.BookingTime, booking.Duration, booking.Id + " " + booking.BookingName,
                booking.GoogleEventSummary(), attendeesNotImplemented,
                booking.GoogleCalendarId, //!string.IsNullOrEmpty(booking.GoogleCalendarId) ? booking.GoogleCalendarId : TBRBooking + booking.Id,
                booking.Address);
            var id = booking.GoogleCalendarId;
            if (string.IsNullOrEmpty(booking.GoogleCalendarId))
                id = TheGoogle.AddGoogleCalendarEvent(calItem);
            else
                TheGoogle.UpdateGoogleCalendarEvent(calItem);

            return id;
        }

    }

    
}
