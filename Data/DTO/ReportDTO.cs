using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBRBooker.Model.Enums;

namespace TBRBooker.Model.DTO
{
    public class ReportDTO
    {
        public readonly string Name;
        public readonly DateTime StartDate;
        public readonly DateTime EndDate;
        public readonly decimal TotalSales;
        public readonly decimal CollectedSales;
        public readonly decimal OutstandingSales;
        public readonly decimal OverdueSales;
        public readonly decimal BadDebt;
        public readonly int TotalEnquiries;
        public readonly int ConvertedJobs;
        public readonly int CompletedJobs;
        public readonly int OpenEnquiries;
        public readonly int LostEnquiries;
        public readonly int BadDebtBookings;
        public readonly Dictionary<PaymentMethods, (int Number, decimal TotalAmt)> Payments;


        public readonly int ConversionRate;
        public readonly decimal AverageSale;


        public ReportDTO(string name, DateTime startDate, DateTime endDate,
            decimal totalSales, decimal collected, decimal outstanding, decimal overdue,
            decimal baddebt,
            int totalEnquiries, int converted, int completed, int lost, int open, int badcount,
            int conversion, decimal average,
            Dictionary<PaymentMethods, (int Number, decimal TotalAmt)> payments)
        {
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            TotalSales = totalSales;
            CollectedSales = collected;
            OutstandingSales = outstanding;
            OverdueSales = overdue;
            BadDebt = baddebt;
            TotalEnquiries = totalEnquiries;
            ConvertedJobs = converted;
            ConversionRate = conversion;
            AverageSale = average;
            LostEnquiries = lost;
            OpenEnquiries = open;
            BadDebtBookings = badcount;
            Payments = payments;
        }
        
    }
}
