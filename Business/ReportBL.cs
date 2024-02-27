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

        private static (List<Booking> Bookings, DateTime Expiry) _reportData = (null, DateTime.MinValue);

        public static List<Booking> GetReportData()
        {
            if (_reportData.Expiry < DateTime.Now)
            {
                var allBookings = BookingsForReport(DateTime.MinValue, DateTime.MaxValue);
                _reportData = (allBookings, DateTime.Now.AddHours(12));
                return allBookings;
            }

            return _reportData.Bookings;
        }

        public static List<string> GetAllReports(DateTime selectedDay)
        {
            var files = new List<string>();

            files.Add(GetPreviousYearReport(selectedDay));
            files.Add(GetCurrentYearReport(selectedDay));
            files.Add(GetReportFile("Last 30 Days", DTUtils.StartOfDay(selectedDay).AddMonths(-1), DTUtils.EndOfDay(selectedDay.AddDays(-1))));
            files.Add(GetReportFile("Last 12 Months", DTUtils.StartOfDay(selectedDay).AddDays(-365), DTUtils.EndOfDay(selectedDay.AddDays(-1))));

            return files;
        }

        public static string GetReportFile(string name, DateTime startDate, DateTime endDate)
        {
            var reportData = GetReportData();
            return MakeReportHtml(FetchReport(name, startDate, endDate));
        }

        public static string GetCurrentYearReport(DateTime selectedDay)
        {
            var reportData = GetReportData();
            DateTime start = new DateTime(selectedDay.Year, 7, 1);
            DateTime end = new DateTime(selectedDay.Year + 1, 7, 1).AddHours(-1);
            if (selectedDay.Month <= 6)
            {
                start = new DateTime(selectedDay.Year - 1, 7, 1);
                end = new DateTime(selectedDay.Year, 7, 1).AddHours(-1);
            }

            return MakeReportHtml(FetchReport($"Financial Year {start.Year}-{end.Year}", start, end, reportData));
        }

        public static string GetPreviousYearReport(DateTime selectedDay)
        {
            var reportData = GetReportData();
            var (start, end) = PreviousFinancialYear(selectedDay);
            return MakeReportHtml(FetchReport($"Financial Year {start.Year}-{end.Year}", start, end, reportData));
        }

        private static (DateTime Start, DateTime End) PreviousFinancialYear(DateTime selectedDay)
        {
            DateTime start = new DateTime(selectedDay.Year - 1, 7, 1);
            DateTime end = new DateTime(selectedDay.Year, 7, 1).AddHours(-1);
            if (selectedDay.Month <= 6)
            {
                start = new DateTime(selectedDay.Year - 2, 7, 1);
                end = new DateTime(selectedDay.Year - 1, 7, 1).AddHours(-1);
            }

            return (start, end);
        }

        public static string GetReport(string name, DateTime start, DateTime end)
        {
            return MakeReportHtml(FetchReport(name, start, end));
        }

        public static string GetTravelLog(DateTime selectedDay)
        {
            var reportData = GetReportData();
            var (start, end) = PreviousFinancialYear(selectedDay);
            var withinRange = reportData.Where(f => f.BookingDate >= start && f.BookingDate < end && f.IsBooked());
            var workDaySummaries = new List<(string Day, string Bookings, int SubtotalKm)>();
            var homeAddress = "666 Beechmont Road, Lower Beechmont, Qld 4211";

            foreach (var dailyBookings in withinRange.GroupBy(f => DTUtils.StartOfDay(f.BookingDate)).OrderBy(f => f.Key))
            {
                var bookingStr = string.Join(", ", dailyBookings.Select(f => $"{f.BookingName} ({ExtractSuburb(f.Address)})")).Trim();
                var finalArrivalTime = dailyBookings.Last().BookingDate;
                try
                {
                    var route = TheGoogle.TravelInfo(dailyBookings.Select(f => f.Address).ToList(), finalArrivalTime, homeAddress, finishAtStart: true);
                    var SubtotalKm = route.Distances.Sum() / 1000;
                    workDaySummaries.Add((dailyBookings.Key.ToShortDateString(), bookingStr, SubtotalKm));
                }
                catch
                {
                    workDaySummaries.Add((dailyBookings.Key.ToShortDateString(), bookingStr + " (CALCULATION ERROR)", 0));
                }
            }

            var travelLog = new TravelLogDTO(
                start.ToShortDateString(), end.ToShortDateString(), workDaySummaries.Sum(f => f.SubtotalKm), workDaySummaries);
            var blankForm = Resources.TravelLog;
            var form = blankForm.Replace("[start]", travelLog.StartDate)
                .Replace("[end]", travelLog.EndDate)
                .Replace("[homeAddress]", homeAddress)
                .Replace("[totalKm]", travelLog.TotalKm.ToString())
                .Replace("[rows]", String.Join(Environment.NewLine,
                    travelLog.WorkDays.Select(f => $"<tr><td>{f.Day}</td><td>{f.Bookings}</td><td>{f.SubtotalKm}</td>")));
            
            var filename = Settings.Inst().WorkingDir + $"\\Reports\\Travel Log {start.ToString("yyyyMMdd")}-{end.ToString("yyyyMMdd")} - {DateTime.Now.Ticks}.html";
            File.WriteAllText(filename, form);
            return filename;
        }

        private static string ExtractSuburb(string address)
        {
            var pieces = address.Split(',').ToList();
            if (pieces.Last().Trim().ToUpper() == "AUSTRALIA")
            {
                pieces.RemoveAt(pieces.Count - 1);
            }

            if (pieces.Count == 3)
            {
                return pieces[1].Trim();
            }

            if (pieces.Last().Trim().ToUpper().StartsWith("QLD") && pieces.Count > 1)
            {
                return pieces[pieces.Count - 2].Trim();
            }

            var relevantPiece = pieces.Last();
            
            if (relevantPiece.ToUpper().Contains(" QLD"))
            {
                return relevantPiece.Substring(0, relevantPiece.ToUpper().IndexOf(" QLD")).Trim();
            }

            return pieces.Last().Trim();
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
                Math.Round(filtered.Where(x => x.Status == Model.Enums.BookingStates.BadDept)
                    .Sum(y => y.Balance)),
                filtered.Count,
                booked.Count,
                booked.Count(x => x.BookingDateBeenAndGone()),
                enquiries.Count(x => x.Status == BookingStates.OpenEnquiry),
                enquiries.Count(x => x.Status == Model.Enums.BookingStates.Cancelled
                    || x.Status == Model.Enums.BookingStates.LostEnquiry),
                filtered.Count(x => x.Status == Model.Enums.BookingStates.BadDept),
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
