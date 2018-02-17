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

namespace TBRBooker.FrontEnd
{
    public partial class MainFrm : Form
    {
        //private 
        private DayPanel[,] _days;
        private DateTime _calendarStartDate;
        private BookingsFrm _bookingsFrm;
        private int _screenId;

        public MainFrm()
        {
            InitializeComponent();

            _calendarStartDate = PickCalendarStartDate(DateTime.Now);
            datePicker.Value = _calendarStartDate;
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            _screenId = Settings.Inst().MainScreenDefaultId;
            MoveFormToCurrentScreenId();

            UpdateCalendar();
        }

        public void UpdateCalendar()
        {
            AddDayPanels(false);

            //display date range
            string dateRangeStr = _calendarStartDate.ToString("MMM yy");
            var lastDay = _calendarStartDate.AddDays(28);
            if (lastDay.Month != _calendarStartDate.Month)
                dateRangeStr += $" - {lastDay.ToString("MMM yy")}";
            monthsLbl.Text = dateRangeStr;
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

        private void AddDayPanels(bool isForceRefresh)
        {
            daysPanel.Controls.Clear();
            _days = new DayPanel[4,7];

            //HACK: parse in date, and have DBBox track which dates have been read.
            //      for older dates, if 'show cancelled' ticked, it needs to scan the whole table (maybe make user re-tick show cancelled when changing date range)
            var items = DBBox.GetCalendarItems(isForceRefresh);

            var day = _calendarStartDate;
            for (int i = 0; i <= 3; i++)
            {
                for (int j = 0; j <= 6; j++)
                {
                    var dayPnl = new DayPanel(day, 
                        items.Where(x => x.BookingDate.DayOfYear == day.DayOfYear).ToList(),
                        true, this);
                    daysPanel.Controls.Add(dayPnl);
                    dayPnl.Location = new Point(j * (dayPnl.Size.Height + 5) + 5, i * (dayPnl.Size.Width + 5) + 5);
                    _days[i, j] = dayPnl;
                    day = day.AddDays(1);
                }
            }
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
            _calendarStartDate = PickCalendarStartDate(datePicker.Value);
            UpdateCalendar();
        }

        private void prevBtn_Click(object sender, EventArgs e)
        {
            _calendarStartDate = _calendarStartDate.AddDays(-7);
            UpdateCalendar();
        }

        private void nextBtn_Click(object sender, EventArgs e)
        {
            _calendarStartDate = _calendarStartDate.AddDays(7);
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
            AddDayPanels(true);
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
    }
}
