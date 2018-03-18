using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBRBooker.Model.DTO;

namespace TBRBooker.Model
{

    public enum DashboardCategories
    {
        ConfirmationCalls,
        Followups,
        [Description("Pay and Close")]
        PaymentsDue
    }

    public class DashboardCategory
    {
        public DashboardCategories Category;
        public List<DashboardItemDTO> Items;

    }
}
