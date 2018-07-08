using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TBRBooker.Business;
using TBRBooker.Model.Enums;
using TBRBooker.Model.Entities;
using TBRBooker.Base;
using TBRBooker.Model.DTO;

namespace TBRBooker.FrontEnd
{
    public partial class MainFrm : Form
    {
        //private 
        private DayPanel[,] _days;
        private DateTime _calendarStartDate;
        private BookingsFrm _bookingsFrm;
        private int _screenId;
        private bool _isAllHistoryAvailable;
        private bool _isFirstLoad;

        public MainFrm()
        {
            InitializeComponent();

            _calendarStartDate = PickCalendarStartDate(DTUtils.StartOfDay());
            datePicker.Value = _calendarStartDate;
            _isFirstLoad = true;
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            _screenId = Settings.Inst().MainScreenDefaultId;
            MoveFormToCurrentScreenId();

            testingToolStripMenuItem.Visible = Settings.Inst().IsTestMode;
            //(below not needed - updates calendar when initial date value set)
            //UpdateCalendar();
        }

        public void UpdateCalendar()
        {
            try
            {
                AddDayPanels(false);

                //display date range
                string dateRangeStr = _calendarStartDate.ToString("MMM yy");
                var lastDay = _calendarStartDate.AddDays(28);
                if (lastDay.Month != _calendarStartDate.Month)
                    dateRangeStr += $" - {lastDay.ToString("MMM yy")}";
                monthsLbl.Text = dateRangeStr;

                //dashboards
                dashboardPnl.Controls.Clear();
                int yOffset = 10;
                foreach (var category in DashboardBL.GetDashboard())
                {
                    var categoryPnl = new DashboardCategoryPnl(this, category);
                    categoryPnl.Location = new Point(3, yOffset);
                    categoryPnl.RefreshList();
                    dashboardPnl.Controls.Add(categoryPnl);
                    yOffset += 10 + categoryPnl.Height;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to update the calendar", ex);
            }
        }

        /// <summary>
        /// Should always be a Monday
        /// </summary>
        private DateTime PickCalendarStartDate(DateTime targetInclude)
        {
            int numDaysToSubtract;
            switch (targetInclude.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    numDaysToSubtract = 5;
                    break;
                case DayOfWeek.Sunday:
                    numDaysToSubtract = 6;
                    break;
                default:
                    numDaysToSubtract = 6 + Convert.ToInt32(targetInclude.DayOfWeek);
                    break;
            }

            return targetInclude.AddDays(-1 * numDaysToSubtract);
        }

        private void AddDayPanels(bool isForceReadAll)
        {
            daysPanel.Controls.Clear();
            _days = new DayPanel[4,7];

            var calItems = new List<CalendarItemDTO>();
            calItems.AddRange(DBBox.GetCalendarItems(isForceReadAll));

            // read list from google calendar takes a good moment, so read 12 months upfront
            // then, going forward a week or a month does not require any reading.
            // Going forward 18 months or backewards 3 months does.
            calItems.AddRange(CalendarBL.GetGoogleEventsForMainCalendar(
                isForceReadAll, _calendarStartDate, 
                _calendarStartDate.AddDays(_isFirstLoad ? 365 : 30)));
                        
            var day = _calendarStartDate;
            for (int i = 0; i <= 3; i++)
            {
                for (int j = 0; j <= 6; j++)
                {
                    var dayPnl = new DayPanel(day,
                        calItems.Where(x => x.Date.DayOfYear == day.DayOfYear).ToList(),
                        true, this, isForceReadAll || _isAllHistoryAvailable);
                    daysPanel.Controls.Add(dayPnl);
                    dayPnl.Location = new Point(j * (dayPnl.Size.Height + 5) + 5, i * (dayPnl.Size.Width + 5) + 5);
                    _days[i, j] = dayPnl;
                    day = day.AddDays(1);
                }
            }

            _isFirstLoad = false;
        }

        private void databaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var customer = new Customer()
            //{
            //    FirstName = "Joeltest",
            //    LastName = "Conntest",
            //    MobileNumber = "0412345678",
            //    OtherNumbers = "",
            //    EmailAddress = "joel.connop@gmail.com",
            //    CreatedDate = DateTime.Now,
            //    CompanyName = "Happy Testers"
            //};
            //DBBox.WriteItem(customer);

            
        }

        private void databaseReadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var customer = DBBox.ReadItem<Customer>("89f5e12e-8e37-49d7-84ee-becb1e216761");
            MessageBox.Show(customer.DirectoryName());
        }

        private void datePicker_ValueChanged(object sender, EventArgs e)
        {
            _calendarStartDate = DTUtils.StartOfDay(PickCalendarStartDate(datePicker.Value));
            UpdateCalendar();
        }

        private void prevBtn_Click(object sender, EventArgs e)
        {
            _calendarStartDate = DTUtils.StartOfDay(_calendarStartDate.AddDays(-7));
            UpdateCalendar();
        }

        private void nextBtn_Click(object sender, EventArgs e)
        {
            _calendarStartDate = DTUtils.StartOfDay(_calendarStartDate.AddDays(7));
            UpdateCalendar();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_bookingsFrm != null)
            {
                _bookingsFrm.Quit();
            }
            Close();
        }

        private void switchMonitorBtn_Click(object sender, EventArgs e)
        {
            _screenId = _screenId == 0 ? 1 : 0;
            MoveFormToCurrentScreenId();
        }

        private void MoveFormToCurrentScreenId()
        {
            Screen[] screens = Screen.AllScreens;
            if (screens.Length > 1)
            {
                WindowState = FormWindowState.Normal;
                Location = screens[_screenId].WorkingArea.Location;

                WindowState = FormWindowState.Maximized;
            }
        }

        private void showBookingsBtn_Click(object sender, EventArgs e)
        {
            InitBookingsFrm();
            _bookingsFrm.ShowOnAppropriateMonitor();
        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Reloading the entire calendar is an expensive read operation to scan every " +
                "booking since the beginning of time (normally we just show you < 3 months expired, or still " +
                "having outstanding payments).\r\nYou should only do this if you need to view old bookings." +
                " Proceed anyway?", "Reload Calendar", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                == DialogResult.OK)
            {
                try
                {
                    _isFirstLoad = true;
                    AddDayPanels(true);
                    _isAllHistoryAvailable = true;
                }
                catch (Exception ex)
                {
                    ErrorHandler.HandleError(this, "Failed to update the calendar", ex);
                }
            }
        }

        private void InitBookingsFrm()
        {
            if (_bookingsFrm == null || _bookingsFrm.IsDisposed)
            {
                _bookingsFrm = new BookingsFrm(this);
                _bookingsFrm.Show(this);
            }
        }

        public void ShowBooking(Booking booking)
        {
            InitBookingsFrm();
            _bookingsFrm.ShowBooking(booking);
        }

        private void googleCalendarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = TheGoogle.GetGoogleCalendar(
                DateTime.Now, DateTime.Now.AddMonths(1), true);

            var sb = new StringBuilder();
            result.ForEach(x => sb.AppendLine(x.ToString()));
            MessageBox.Show(sb.ToString());
        }
    }
}
