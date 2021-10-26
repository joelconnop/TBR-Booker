using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBRBooker.Model.DTO
{
    public class TravelLogDTO
    {
        public string StartDate { get; }
        public string EndDate { get; }
        public int TotalKm { get; }
        public IReadOnlyList<(string Day, string Bookings, int SubtotalKm)> WorkDays { get; }

        public TravelLogDTO(
            string startDate, string endDate, int totalKm, IReadOnlyList<(string Day, string Bookings, int SubtotalKm)> workDays)
        {
            StartDate = startDate;
            EndDate = endDate;
            TotalKm = totalKm;
            WorkDays = workDays;
        }
    }
}
