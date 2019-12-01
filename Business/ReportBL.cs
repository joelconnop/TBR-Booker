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
                booked.Count(x => x.BookingDateBeenAndGone()),
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

        public static string BookingReport(
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

        public static string BookingsReport(List<Booking> bookings, string reportName)
        {
            var completedStatus = new[] { BookingStates.Completed, BookingStates.PaymentDue };
            var booked = bookings.Where(x => completedStatus.Contains(x.Status))
                .OrderBy(x => x.BookingDate).ToList();
            var report = FetchReport(reportName,
                booked.First().BookingDate, booked.Last().BookingDate,
                booked);

            string form = Resources.BookingsReport;

            var sb = new StringBuilder();
            foreach (var booking in booked)
            {
                sb.Append("<tr class='stripedRow'>");
                sb.Append($"<td class='stripedCol'>{booking.Id}</td>");
                sb.Append($"<td class='stripedCol'>{booking.BookingDate.ToShortDateString()}</td>");
                sb.Append($"<td class='stripedCol'>{booking.BookingName}</td>");
                sb.Append($"<td class='stripedCol'>{booking.Service.TotalPrice.ToString("C")}</td>");
                sb.Append($"<td class='stripedCol'>{booking.Balance.ToString("C")}</td>");
                sb.Append("</tr>");
            }

            form = form.Replace("[name]", report.Name)
                .Replace("[start]", report.StartDate.ToShortDateString())
                .Replace("[end]", report.EndDate.ToShortDateString())
                .Replace("[printed]", DateTime.Now.ToShortDateString())
                .Replace("[numBookings]", bookings.Count.ToString())
                .Replace("[totalSales]", report.TotalSales.ToString("C"))
                .Replace("[collected]", report.CollectedSales.ToString("C"))
                .Replace("[outstanding]", report.OverdueSales.ToString("C"))
                .Replace("[data]", sb.ToString());
                

            var filename = Settings.Inst().WorkingDir + $"\\Reports\\{report.Name} {report.StartDate.ToString("yyyyMMdd")}-{report.EndDate.ToString("yyyyMMdd")} - {DateTime.Now.Ticks}.html";
            File.WriteAllText(filename, form);
            return filename;
        }

        public static string PrintPenalties(List<Penalty> penalties)
        {
            penalties = penalties.OrderBy(x => x.RelevantDate).ToList();
            string form = Resources.PenaltyForm;
            var start = penalties.First().RelevantDate;

            

            var sb = new StringBuilder();
            foreach (var penGroup in penalties.GroupBy(x => x.PenaltyType))
            {
                sb.AppendLine($"<h3>{EnumHelper.ReplaceCamelCaseWithSpace(penGroup.Key.ToString(), true)}</h3><table>");
                foreach (var pen in penGroup)
                {
                    sb.AppendLine($"<tr><td>{pen.Description}</td><td>{pen.RelevantDate.ToShortDateString()}</td><td>{(pen.Value - pen.AbsolvedValue)}</td></tr>");
                }
                sb.AppendLine($"</table><br>");
            }

            form = form.Replace("[start]", start.ToShortDateString())
                .Replace("[end]", DateTime.Today.ToShortDateString())
                .Replace("[total]", penalties.Sum(x => x.Value - x.AbsolvedValue).ToString())
                .Replace("[details]", sb.ToString());

            var filename = Settings.Inst().WorkingDir + $"\\Reports\\Penalties {start.ToString("yyyyMMdd")} - {DateTime.Now.Ticks}.html";
            File.WriteAllText(filename, form);
            return filename;
        }

    }

}
