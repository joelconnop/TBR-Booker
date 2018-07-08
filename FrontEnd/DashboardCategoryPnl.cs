using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TBRBooker.Model;
using TBRBooker.Base;
using TBRBooker.Model.DTO;
using TBRBooker.Business;

namespace TBRBooker.FrontEnd
{
    public partial class DashboardCategoryPnl : UserControl
    {

        private MainFrm _owner;
        private DashboardCategory _category;

        public DashboardCategoryPnl(MainFrm owner, DashboardCategory category)
        {
            InitializeComponent();

            _owner = owner;
            _category = category;
        }

        public void RefreshList()
        {
            titleLbl.Text = EnumHelper.GetEnumDescription(_category.Category);

            itemsLst.BeginUpdate();
            itemsLst.Items.Clear();

            var today = DTUtils.StartOfDay();
            foreach (var item in _category.Items.OrderBy(x => x.FollowupDate))
            {
                var lvi = new ListViewItem(new string[] {item.CalendarItem.BookingNum + " " + item.CalendarItem.Name,
                    item.CalendarItem.Date.ToString("d"), item.FollowupDate.ToString("d"),
                    item.FollowupDate < today ? "Y" : ""});
                lvi.Tag = item;
                itemsLst.Items.Add(lvi);
            }

            itemsLst.EndUpdate();
        }

        private void itemsLst_ItemActivate(object sender, EventArgs e)
        {
            try
            {
                var selected = (DashboardItemDTO)itemsLst.SelectedItems[0].Tag;
                _owner.ShowBooking(BookingBL.GetBookingFull(selected.CalendarItem.BookingNum.ToString()));
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Opening Followup", ex, true);
            }
        }
    }
}
