using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBRBooker.Base;
using TBRBooker.Model.Entities;
using TBRBooker.Model.Enums;

namespace TBRBooker.Business
{
    public class PenaltyBL
    {
        public static void UpdatePenalties()
        {
            try
            {
                var today = DTUtils.StartOfDay();
                var lastScan = GetLastScan();

                if (today <= lastScan)//!Settings.Inst().Username.Equals("Sarah Jane") ||
                    return;

                // update last scanned
                Settings.Inst().LastScan = DTUtils.StartOfDay().Ticks;
                Settings.PersistSettings(Settings.Inst(), null, true);

                // scan!
                var penalties = DBBox.GetUnpaidPenalties();

                // DONE ELSEWHERE!
                // Job cancelled because TBR tardy etc. Currently not collecting lost deal reasons! simple Dialog is fine.

                foreach (var booking in DBBox.GetCalendarItems(false, true))
                {
                    try
                    {
                        // pay and close
                        if (Booking.IsOpenStatus(booking.BookingStatus))
                        {
                            if (booking.Date < today.AddDays(-30))
                            {
                                var monthsLate = new TimeSpan(today.Ticks - booking.Date.Ticks).Days / 30;
                                var validDate = booking.Date.AddMonths(monthsLate - 1);
                                var relevantDate = booking.Date.AddDays(7);
                                AddPenaltyIfValid(penalties, today, lastScan, Penalties.PayAndCloseVeryLate,
                                    relevantDate, validDate, 0 + (monthsLate - 1), booking.BookingNum.ToString() + " " + booking.Name);
                            }
                            else if (booking.Date < today.AddDays(-7))
                            {
                                var relevantDate = booking.Date.AddDays(7);
                                AddPenaltyIfValid(penalties, today, lastScan, Penalties.PayAndClose7DaysLate,
                                    relevantDate, null, 3, booking.BookingNum.ToString() + " " + booking.Name);
                            }
                        }

                        // confirmation calls
                        if (booking.ConfirmationDate == null && booking.Date < today && booking.Date > new DateTime(2019, 8, 1))
                        {
                            AddPenaltyIfValid(penalties, today, lastScan, Penalties.MissedConfirmationCall,
                                   booking.Date, null, 5, booking.BookingNum.ToString() + " " + booking.Name);
                        }

                    }
                    catch
                    {
                        // do nothing if some fail (to save?)
                    }
                }



                // repeat markers
                foreach (var schedule in DBBox.GetRepeatSchedules())
                {
                    var markers = RepeatScheduleBL.CreateMarkersFromSchedule(schedule, lastScan, today);
                    foreach (var m in markers)
                    {
                        AddPenaltyIfValid(penalties, today, lastScan, Penalties.MissedRecurringConfirmation,
                            m.Date, null, 20, m.Name + m.Date.ToShortDateString());
                    }
                }
            }
            catch (Exception ex)
            {
                // silently log and abort
                ErrorLogger.LogError("updating penalties", ex);
            }
        }

        private static void AddPenaltyIfValid(List<Penalty> penalties, DateTime today, DateTime lastScan,
            Penalties penType, DateTime relevantDate, DateTime? validDate, int value, string bookingId)
        {
            if (relevantDate > DTUtils.StartOfDay()
                && penType != Penalties.PayAndCloseVeryLate)
                return;             // not late yet

            if (relevantDate < lastScan)
                return;             // dealt with last time

            var existing = penalties.Where(x => x.Description == bookingId && x.PenaltyType == penType)
                .OrderByDescending(y => y.CreatedDate).FirstOrDefault();

            if (existing != null)
            {
                if (penType != Penalties.PayAndCloseVeryLate)
                    return;     // penalty does not stack

                if (existing.CreatedDate >= validDate == true)
                    return;     // not due to stack yet

                // stack that shit!
                if (penType == Penalties.PayAndCloseVeryLate && existing != null)
                {
                    var existingThisCycle = existing.Value - existing.AbsolvedValue;
                    existing.Value += value - existingThisCycle;
                    DBBox.AddOrUpdate(existing);
                    return;
                }
            }

            
            var penalty = new Penalty()
            {
                PenaltyType = penType,
                CreatedDate = today,
                RelevantDate = relevantDate,
                Value = value,
                Description = bookingId
            };
            DBBox.AddOrUpdate(penalty);
            penalties.Add(penalty);
        }

        public static DateTime GetLastScan()
        {
            var ticks = Settings.Inst().LastScan;
            if (ticks > 0)
                return new DateTime(ticks);
            return DTUtils.StartOfDay(new DateTime(2019, 7, 1));
        }

        public static void PrintAndAbsolvePenalties()
        {
            var penalties = DBBox.GetUnpaidPenalties().Where(x => x.Value - x.AbsolvedValue > 0).ToList();

            if (penalties.Count == 0)
                return;

            System.Diagnostics.Process.Start(ReportBL.PrintPenalties(penalties));

            foreach (var pen in penalties)
            {
                pen.AbsolvedValue = pen.Value;
                if (pen.PenaltyType != Penalties.PayAndCloseVeryLate)
                    pen.DateAbsolved = DTUtils.StartOfDay();
                DBBox.AddOrUpdate(pen);
            }
        }

    }
}
