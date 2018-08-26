using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBRBooker.Base;
using TBRBooker.Model.DTO;
using TBRBooker.Model.Entities;
using TBRBooker.Model.Enums;
using TBRBooker.Model.Properties;

namespace TBRBooker.Business
{
    public class ReportBL
    {

        public static List<string> GetAllReports()
        {
            var files = new List<string>();

            var reportData = BookingsForReport(DateTime.MinValue, DateTime.MaxValue);
            files.Add(MakeReportHtml(FetchReport("All Time", DateTime.MinValue, DateTime.MaxValue, reportData)));
            files.Add(GetPreviousYearReport(reportData));
            files.Add(GetCurrentYearReport(reportData));
            files.Add(MakeReportHtml(FetchReport("Last 30 Days", DTUtils.StartOfDay().AddDays(-30), DTUtils.StartOfDay(), reportData)));

            return files;
        }

        public static string GetCurrentYearReport(List<Booking> reportData = null)
        {
            DateTime start = new DateTime(DateTime.Now.Year, 7, 1);
            DateTime end = new DateTime(DateTime.Now.Year + 1, 7, 1).AddHours(-1);
            if (DateTime.Now.Year <= 6)
            {
                start = new DateTime(DateTime.Now.Year - 1, 7, 1);
                end = new DateTime(DateTime.Now.Year, 7, 1).AddHours(-1);
            }

            return MakeReportHtml(FetchReport("Current Financial Year", start, end, reportData));
        }

        public static string GetPreviousYearReport(List<Booking> reportData = null)
        {
            DateTime start = new DateTime(DateTime.Now.Year - 1, 7, 1);
            DateTime end = new DateTime(DateTime.Now.Year, 7, 1).AddHours(-1);
            if (DateTime.Now.Year <= 6)
            {
                start = new DateTime(DateTime.Now.Year - 2, 7, 1);
                end = new DateTime(DateTime.Now.Year - 1, 7, 1).AddHours(-1);
            }

            return MakeReportHtml(FetchReport("Last Financial Year", start, end, reportData));
        }

        public static string GetReport(string name, DateTime start, DateTime end)
        {
            return MakeReportHtml(FetchReport(name, start, end));
        }

        private static List<Booking> BookingsForReport(DateTime start, DateTime end)
        {
            // expensive scan, but we need everything even old things before we can
            // even check if it is in the date range
            var calItems = DBBox.GetCalendarItems(true, true);
            calItems = calItems.Where(x => x.Date >= start && x.Date <= end).ToList();

            var bookings = new List<Booking>();

            foreach (var cal in calItems)
            {
                // read each booking that hasn't been read before
                bookings.Add(DBBox.ReadItem<Booking>(cal.BookingNum.ToString()));
            }

            return bookings;
        }

        private static ReportDTO FetchReport(string name, DateTime start, DateTime end,
            List<Booking> reportData = null)
        {
            if (reportData == null)
                reportData = BookingsForReport(start, end);

            var filtered = reportData.Where(x => x.BookingDate >= start && x.BookingDate <= end).ToList();

            var today = DTUtils.StartOfDay();

            var booked = filtered.Where(x => x.IsBooked()).ToList();
            var enquiries = filtered.Where(x => !x.IsBooked()).ToList();

            var payments = new Dictionary<PaymentMethods, (int Number, decimal TotalAmt)>();
            foreach (var payment in booked.SelectMany(x => x.PaymentHistory))
            {
                var method = payment.Method;
                switch (method)
                {
                    case PaymentMethods.Cash:
                    case PaymentMethods.DD:
                    case PaymentMethods.CC:
                        break;
                    case PaymentMethods.Refund:
                    case PaymentMethods.Reversal:
                        continue;
                    default:
                        method = PaymentMethods.NotSet;
                        break;
                }
                if (payments.ContainsKey(method))
                {
                    payments[method] = (payments[method].Number + 1,
                        payments[method].TotalAmt + payment.Amount);
                }
                else
                {
                    payments.Add(method, (1, payment.Amount));
                }
            }
            AddUnusedPayments(PaymentMethods.Cash);
            AddUnusedPayments(PaymentMethods.CC);
            AddUnusedPayments(PaymentMethods.DD);
            AddUnusedPayments(PaymentMethods.NotSet);
            void AddUnusedPayments(PaymentMethods method)
            {
                if (!payments.ContainsKey(method))
                    payments.Add(method, (0, 0));
            }

            return new ReportDTO(
                name, start, end,
                Math.Round(booked.Sum(y => y.Service.TotalPrice)),
                Math.Round(booked.Sum(y => y.AmountPaid)),
                Math.Round(booked.Where(x => !x.IsOverdue()).Sum(y => y.Balance)),
                Math.Round(booked.Where(x => x.IsOverdue(today)).Sum(y => y.Balance)),
                Math.Round(filtered.Where(x => x.Status == Model.Enums.BookingStates.CancelledWithoutPayment)
                    .Sum(y => y.Balance)),
                filtered.Count,
                booked.Count,
                booked.Count(x => x.IsCompleted()),
                enquiries.Count(x => x.Status == BookingStates.OpenEnquiry),
                enquiries.Count(x => x.Status == Model.Enums.BookingStates.Cancelled
                    || x.Status == Model.Enums.BookingStates.LostEnquiry),
                filtered.Count(x => x.Status == Model.Enums.BookingStates.CancelledWithoutPayment),
                booked.Count == 0 ? 0 : Convert.ToInt32(Math.Round((double)booked.Count / (double)filtered.Count, 2) * 100),
                booked.Count == 0 ? 0 : Math.Round(booked.Sum(x => x.Service.TotalPrice) / booked.Count),
                payments
            );
        }

        private static string MakeReportHtml(ReportDTO report)
        {
            string form = Resources.ReportForm;

            form = form.Replace("[name]", report.Name)
                .Replace("[start]", report.StartDate.ToShortDateString())
                .Replace("[end]", report.EndDate.ToShortDateString())
                .Replace("[totalSales]", report.TotalSales.ToString("C"))
                .Replace("[collected]", report.CollectedSales.ToString("C"))
                .Replace("[outstanding]", report.OutstandingSales.ToString("C"))
                .Replace("[overdue]", report.OverdueSales.ToString("C"))
                .Replace("[baddebt]", report.BadDebt.ToString("C"))
                .Replace("[numcash]", report.Payments[PaymentMethods.Cash].Number.ToString())
                .Replace("[cash]", Math.Round(report.Payments[PaymentMethods.Cash].TotalAmt).ToString("C"))
                .Replace("[numdd]", report.Payments[PaymentMethods.DD].Number.ToString())
                .Replace("[dd]", Math.Round(report.Payments[PaymentMethods.DD].TotalAmt).ToString("C"))
                .Replace("[numcc]", report.Payments[PaymentMethods.CC].Number.ToString())
                .Replace("[cc]", Math.Round(report.Payments[PaymentMethods.CC].TotalAmt).ToString("C"))
                .Replace("[numother]", report.Payments[PaymentMethods.NotSet].Number.ToString())
                .Replace("[other]", Math.Round(report.Payments[PaymentMethods.NotSet].TotalAmt).ToString("C"))
                .Replace("[totalEnquiries]", report.TotalEnquiries.ToString())
                .Replace("[converted]", report.ConvertedJobs.ToString())
                .Replace("[completed]", report.CompletedJobs.ToString())
                .Replace("[lost]", report.LostEnquiries.ToString())
                .Replace("[open]", report.OpenEnquiries.ToString())
                .Replace("[badcount]", report.BadDebtBookings.ToString())
                .Replace("[conversion]", report.ConversionRate.ToString())
                .Replace("[average]", report.AverageSale.ToString());

            var filename = Settings.Inst().WorkingDir + $"\\Reports\\{report.Name} {report.StartDate.ToString("yyyyMMdd")}-{report.EndDate.ToString("yyyyMMdd")} - {DateTime.Now.Ticks}.html";
            File.WriteAllText(filename, form);
            return filename;
        }
    }

}
