using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBRBooker.Model.DTO
{
    public class DashboardItemDTO
    {
        public CalendarItemDTO CalendarItem;
        public bool IsOverdue;
        public DateTime FollowupDate;
    }
}
