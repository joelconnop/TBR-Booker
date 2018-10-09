using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBRBooker.Business;
using TBRBooker.Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
using TBRBooker.Model.Properties;
using TBRBooker.Base;
using TBRBooker.Model.DTO;
using TBRBooker.Model.Enums;

namespace TBRBooker.Business
{
    public class BookingBL
    {

        public static Booking GetBookingFull(string id)
        {
            var booking = DBBox.ReadItem<Booking>(id);
            PopulateBookingChildren(booking);
            return booking;
        }

        public static void PopulateBookingChildren(Booking booking)
        {
            if (!string.IsNullOrEmpty(booking.CustomerId))
            {
                booking.Customer = DBBox.ReadItem<Customer>(booking.CustomerId);
            }
            if (!string.IsNullOrEmpty(booking.AccountId))
            {
                booking.Account = DBBox.ReadItem<CorporateAccount>(booking.AccountId);
            }

            // this is to catch up legacy data
            if (booking.Followups == null)
            {
                booking.Followups = new List<Followup>();
            }
            if (booking.HighlightedControls == null)
                booking.HighlightedControls = new List<string>();
        }

        public static void SaveBookingEtc(Booking booking)
        {
            if (booking.Id.Equals(Booking.RepeatingBookingId))
            {
                throw new Exception("Booking still has the TBD status. Cannot save.");
            }

            DBBox.AddOrUpdate(booking.Customer);
            booking.CustomerId = booking.Customer.Id;

            // saved independently
            //if (booking.Account != null)
            //{
            //    DBBox.AddOrUpdate(booking.Account);
            //    booking.AccountId = booking.Account.Id;
            //}

            // update google calendar (sets the id result so need to do dynamodb save after)
            booking.GoogleCalendarId = CalendarBL.AddOrUpdateBookingOnGoogle(booking);

            try
            {
                if (string.IsNullOrEmpty(booking.BookingNickname))
                    booking.BookingNickname = booking.BookingName;
                booking.EditSequence++;
                DBBox.AddOrUpdate(booking);

                if (!booking.Customer.BookingIds.Contains(booking.Id))
                {
                    booking.Customer.BookingIds.Add(booking.Id);
                    DBBox.AddOrUpdate(booking.Customer);
                }

                // this will be the case if account is selected rather than creating new
                if (booking.Account != null && !booking.Account.BookingIds.Contains(booking.Id))
                {
                    booking.Account.BookingIds.Add(booking.Id);
                    DBBox.AddOrUpdate(booking.Account);
                }

                GenerateBookingHtmlFile(booking, true);
                booking.IsNewBooking = false;   //not a database field, just so calling functions know its in DB

            }
            catch (Amazon.DynamoDBv2.Model.ConditionalCheckFailedException)
            {
                //means the Id is already in use (could happen if system not used for a month)
                var nextId = booking.BookingNum() + 1;
                while (DBBox.ReadItem<Booking>(nextId.ToString()) != null)
                {
                    nextId++;
                }
                booking.Id = nextId.ToString();
                DBBox.AddOrUpdate(booking);
            }

            //update the calendar
            var calendar = DBBox.GetCalendarItems(false, false);
            var calendarItem = calendar.FirstOrDefault(x => x.BookingNum == int.Parse(booking.Id));
            if (calendarItem != null)
                calendar.Remove(calendarItem);
            calendar.Add(booking.ToCalendarItem());

            //update the customer directory
            var directory = DBBox.GetCustomerDirectory();
            var existing = directory.FirstOrDefault(x => x.CustomerId == booking.Customer.Id);
            if (existing != null)
                directory.Remove(existing);
            directory.Add(new ExistingCustomerDTO(
                booking.Customer.Id, booking.Customer.DirectoryName()));
        }

        public static List<Booking> GetClashBookings(List<Booking> bookings, int startTime, int endTime)
        {
            return bookings.Where(x => (x.TravelStart >= startTime && x.EndTime <= endTime)
                    || (startTime >= x.TravelStart && endTime <= x.EndTime)).ToList();
        }

        private static int LastBookingNum { get; set; }

        public static string GetNextBookingNum()
        {
            // caching the last num so if multiple new enquiries are started, they don't get the same Id
            // only downside is that if an enquiry is closed without any save, the Id is lost/skipped
            if (LastBookingNum > 0)
            {
                LastBookingNum++;
                return LastBookingNum.ToString();
            }

            //A booking number represents when the enquiry was made, not when the job is scheduled for
            //even bookings made way into the future will use this year's booking number.
            // int year = itm.BookingDate != null ? itm.BookingDate.Value.Year : DateTime.Now.Year;
            int year = DateTime.Now.Year;   //itm.BookingDate.Year;
            int minorNum = (int)Math.Ceiling((9.0 / 365.0) * DateTime.Now.DayOfYear) * 1000;
            int nextNum = int.Parse(year.ToString().Substring(2) + minorNum.ToString());
            //int nextYear = nextNum + minorNum;

            var relevantCalendarItems = DBBox.GetCalendarItems(false, false)
                .Where(x => x.BookingNum > nextNum)
                .Select(y => y.BookingNum).ToList();
            if (relevantCalendarItems.Count == 0)
                nextNum++;    //eg 181001
            else
                nextNum = relevantCalendarItems.Max() + 1;   //eg 180123

            LastBookingNum = nextNum;
            return nextNum.ToString();
        }

        public static string GenerateBookingHtmlFile(
            Booking booking, bool isForArchive, string fromAddress = "")
        {
            string form = Resources.BookingForm;
            //var assembly = Assembly.GetExecutingAssembly();
            //var resourceName = "TBRBooker.Model.BookingForm.html";

            //using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            //using (StreamReader reader = new StreamReader(stream))
            //{
            //    string result = reader.ReadToEnd();
            //}
            var title = $"{booking.BookingDate.DayOfWeek.ToString()} - {booking.Id} {booking.BookingNickname}";
            form = form.Replace("[title]", title)
                .Replace("[name]", booking.Customer.SmartName())
                .Replace("[phone]", booking.Customer.SmartPhone())
                .Replace("[email]", booking.Customer.EmailAddress)
                .Replace("[date]", booking.BookingDate.ToString("D"))
                .Replace("[time]", DTUtils.DisplayTime(booking.BookingTime))
                .Replace("[address]", booking.Address)
                .Replace("[service]", booking.Service.ToString())
                .Replace("[particulars]", booking.Service.GetParticularsText())
                .Replace("[pax]", booking.Service.Pax.ToString())
                .Replace("[total]", booking.Service.TotalPrice.ToString("C"))
                .Replace("[deposit]", booking.AmountPaid.ToString("C"))
                .Replace("[balance]", (booking.Service.TotalPrice - booking.AmountPaid).ToString("C"))
                .Replace("[invoiced]", booking.IsInvoiced ? "Yes" : "No")
                .Replace("[animals]", booking.Service.SpecificAnimalsToCome)
                .Replace("[notes]", booking.BookingNotes);

            if (booking.IsPayOnDay)
                form = form.Replace("[payonday]", "<td style='font-weight: bold'>Yes</td>");
            else
                form = form.Replace("[payonday]", "<td>No</td>");

            if (booking.Service.AddCrocodile)
                form = form.Replace("[crocodile]", "<h4>Crocodile to come: YES</h4>");
            else
                form = form.Replace("[crocodile]", "<label>Crocodile to come: No</label>");

            if (isForArchive)
                form = form.Replace("[directions]", "<p>Not requested</p>");
            else
            {
                var dirHtml = $"<label>From {(string.IsNullOrEmpty(fromAddress) ? "Home" : fromAddress)}:</label><ol>";
                List<string> steps;
                if (string.IsNullOrEmpty(fromAddress))
                    steps = TheGoogle.GetDirections(booking.Address,
                        DTUtils.DateTimeFromInt(booking.BookingDate, booking.BookingTime));
                else
                    steps = TheGoogle.GetDirections(booking.Address,
                        DTUtils.DateTimeFromInt(booking.BookingDate, booking.BookingTime),
                        fromAddress);
                steps.ForEach(x => dirHtml += $"<li>{x}</li>");
                dirHtml += "</ol>";
                form = form.Replace("[directions]", dirHtml);
            }

            string archivePrefix = isForArchive ? "Backups\\" : "";
            string archiveSuffix = isForArchive ? $"_v{booking.EditSequence}" : "";
            var filename = Settings.Inst().WorkingDir + $"\\Booking Forms\\{archivePrefix}{booking.BookingDate.ToString("yyyyMMdd")}-{booking.BookingDate.DayOfWeek.ToString().Substring(0, 3)} {booking.BookingNickname}{archiveSuffix}.html";
            File.WriteAllText(filename, form);
            return filename;
        }

        public static BookingCalendarItemDTO UpdateCalendarItemAndUnderlyingBooking(BookingCalendarItemDTO item)
        {
            bool isRemoveFromFutureCalendars = 
                !Booking.IsBookingOpen(item.BookingStatus, item.Date, item.FollowupDate.HasValue);
            bool isJobRecentlyCompleted = item.BookingStatus == BookingStates.Booked
                && GetBookingDateAndTime(item) < DateTime.Now;

            // clean up followups should theoretically not be needed, they should happen
            // when a state transitions on form. Example, We want old open enquiries to get
            // cancelled manually. If wanting to do automatics then consider are there
            // any bugs on booking form?
            //bool isRemoveFollowup = item.FollowupDate.HasValue
            //    && item.Date < DateTime.Now.AddDays(-7);    // nothing to followup 
            //    && new[] { BookingStates.Completed, BookingStates.PaymentDue, BookingStates.Cancelled, BookingStates.CancelledWithoutPayment }
            //    .Contains(item.BookingStatus);  // looking for PaymentDue because chasing payment is an ad hoc followup which will still happen
            //bool isRemoveConfirmation = item.ConfirmationDate.HasValue
            //    && item.ConfirmationDate < DTUtils.StartOfDay()
            //    && new[] { BookingStates.Completed, BookingStates.PaymentDue, BookingStates.Cancelled, BookingStates.CancelledWithoutPayment }
            //    .Contains(item.BookingStatus);


            if (isRemoveFromFutureCalendars || isJobRecentlyCompleted) // || isRemoveFollowup)
            {
                //not reading the related objects, that can happen if user opens the booking
                var booking = DBBox.ReadItem<Booking>(item.BookingNum.ToString());

                if (isJobRecentlyCompleted)
                {
                    //even if job appears to be fully paid, this needs to be reviewed post job completion
                    booking.Status = Model.Enums.BookingStates.PaymentDue;
                    item = booking.ToCalendarItem();
                }

                //if (isRemoveFollowup)
                //{
                //    var fu = booking.GetCurrentFollowup();
                //    if (fu != null)
                //    {
                //        fu.CompletedDate = DateTime.Now;
                //        fu.CompleteNote = "Expired followup was automatically completed";
                //    }
                //}

                //for isRemoveFromFutureCalendars, just saving the entity again is enough to change its 'isOpen' flag
                DBBox.AddOrUpdate(booking);
            }

            return item;
            //could filter out the items to be removed from future calendars, but we've already
            //read them, so might as well show them?
            //return isRemoveFromFutureCalendars ? null : item;
        }

        public static DateTime GetBookingDateAndTime(CalendarItemDTO item)
        {
            var parsed = DTUtils.ParseTime(item.Time);
            var ts = new TimeSpan(parsed.Hour, parsed.Minute, 0);
         //   var finishTs = ts.Add(new TimeSpan(0, item.Duration, 0));
            var bookingDateStart = DTUtils.StartOfDay(item.Date);  //should always be the start of day, but wasn't from day 1
            return bookingDateStart.AddTicks(ts.Ticks);
        }

    }
}
