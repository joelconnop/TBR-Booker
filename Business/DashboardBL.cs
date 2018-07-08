using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBRBooker.Base;
using TBRBooker.Model;
using TBRBooker.Model.DTO;
using TBRBooker.Model.Entities;

namespace TBRBooker.Business
{
    public class DashboardBL
    {

        public static List<DashboardCategory> GetDashboard()
        {
            var dashboard = new List<DashboardCategory>();

            foreach (DashboardCategories e in Enum.GetValues(typeof(DashboardCategories)))
            {
                dashboard.Add(new DashboardCategory()
                {
                    Category = e,
                    Items = new List<DashboardItemDTO>()
                });
            }

            var today = DTUtils.StartOfDay();
            foreach (var calItm in DBBox.GetCalendarItems(false))
            {

                if (calItm.ConfirmationDate != null && calItm.ConfirmationDate <= today)
                {
                    dashboard.Single(x => x.Category == DashboardCategories.ConfirmationCalls)
                        .Items.Add(new DashboardItemDTO()
                        {
                            CalendarItem = calItm,
                            FollowupDate = calItm.ConfirmationDate.Value,
                            IsOverdue = calItm.ConfirmationDate.Value < today
                        });
                }

                if (calItm.FollowupDate != null && calItm.FollowupDate <= today)
                {
                    dashboard.Single(x => x.Category == DashboardCategories.Followups)
                    .Items.Add(new DashboardItemDTO()
                    {
                        CalendarItem = calItm,
                        FollowupDate = calItm.FollowupDate.Value,
                        IsOverdue = calItm.FollowupDate.Value < today
                    });
                }

                if (calItm.BookingStatus == Model.Enums.BookingStates.PaymentDue)
                {
                    dashboard.Single(x => x.Category == DashboardCategories.PaymentsDue)
                    .Items.Add(new DashboardItemDTO()
                    {
                        CalendarItem = calItm,
                        FollowupDate = GetWeekdayIfPossible(calItm.Date, calItm.Date.AddDays(7)),
                        IsOverdue = calItm.Date.AddDays(Settings.Inst().DaysBeforeOverdue)
                            <= today
                    });
                }
            }

            return dashboard;
        }

        public static Followup CreateFirstEnquiryFollowup(DateTime bookingDate)
        {
            var preferredDate = DTUtils.StartOfDay().AddDays(1);
            preferredDate = GetWeekdayIfPossible(preferredDate, bookingDate);
            if (preferredDate > bookingDate || preferredDate == DTUtils.StartOfDay())
                return null;

            return new Followup()
            {
                FollowupDate = preferredDate,
                Purpose = EnquiryFollowupText
            };
        }

        public static string EnquiryFollowupText = "Are you ready to book?";

        public static Followup CreateConfirmationCall(DateTime bookingDate)
        {
            // resulting date should be:
            // 1. The Wednesday prior to the booking
            // 2. Booking Date, if the booking is Today or already transpired
            // 3. The day before service, if its in the next few days
            var preferredDate = DTUtils.StartOfDay(bookingDate);
            var today = DTUtils.StartOfDay();

            //first, get away from booking date if its already a Wedneday
            if (preferredDate.DayOfWeek == Settings.Inst().ConfirmationCallDay
                && preferredDate > today)
                preferredDate = preferredDate.AddDays(-1);  

            while (preferredDate.DayOfWeek != Settings.Inst().ConfirmationCallDay
            && preferredDate > today)
            {
                preferredDate = preferredDate.AddDays(-1);
            }

            if (preferredDate == today && bookingDate > today.AddDays(1))
                preferredDate = bookingDate.AddDays(-1);

            return new Followup()
            {
                IsConfirmationCall = true,
                FollowupDate = preferredDate,
                Purpose = "Confirmation Call"
            };
        }

        public static DateTime GetWeekdayIfPossible(DateTime preferred, DateTime tooLate)
        {
            preferred = DTUtils.StartOfDay(preferred);
            tooLate = DTUtils.StartOfDay(tooLate);

            if (preferred.DayOfWeek == DayOfWeek.Saturday)
            {
                preferred = preferred.AddDays(2);
            }
            else if (preferred.DayOfWeek == DayOfWeek.Sunday)
            {
                preferred = preferred.AddDays(1);
            }

            if (preferred >= tooLate)
            {
                if (preferred.DayOfWeek == DayOfWeek.Monday)
                    preferred = preferred.AddDays(-3);
                if (preferred > DateTime.Now)
                    return DTUtils.StartOfDay();
            }
            return preferred;
        }

    }
}
