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
using Microsoft.Win32;
using System.IO;
using Newtonsoft.Json;

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

            if (!InitSettings())
            {
                Close();    // exit the program with error state
            }
            else
            {
                Styles.SetFormStyles(this);
                _calendarStartDate = PickCalendarStartDate(DTUtils.StartOfDay());
                _isFirstLoad = true;
                datePicker.Value = _calendarStartDate;

            }
        }

        private bool InitSettings()
        {
            string username;
            string workingDir;

            if (Settings.IsForcedToTestMode())
            {
                username = "Test";
                workingDir = "C:\\Programming\\TBR Booker Instance";
            }
            else
            {
                username = (string)Registry.GetValue(Settings.EnvironmentVarsRoot,
        Settings.UserKey, "");
                workingDir = (string)Registry.GetValue(Settings.EnvironmentVarsRoot,
                    Settings.WorkingDirKey, "");
            }


            if (string.IsNullOrEmpty(workingDir) || string.IsNullOrEmpty(username))
            {
                MessageBox.Show("First time starting up TBR Booker. You will be taken to the Settings screen, please be sure to enter a username and confirm the Google Drive location.");

                // open settings (settings will save enviornment variable)
                var settingsFrm = new SettingsManagementFrm(Settings.CreateDefaultInst(), true);
                if (settingsFrm.ShowDialog(this) == DialogResult.Cancel)
                    return false;
            }
            else
            {
                var filename = workingDir + "\\config\\" + username + "_settings.json";
                try
                {
                    Settings.SetInst(JsonConvert.DeserializeObject<Settings>(File.ReadAllText(filename)));
                }
                catch (Exception ex)
                {
                    Clipboard.SetText(filename);
                    if (MessageBox.Show(this, "Failed to read the settings file at:"
                        + Environment.NewLine + filename
                        + Environment.NewLine + Environment.NewLine + ex.Message
                        + Environment.NewLine + Environment.NewLine
                        + "This is the file specified in the Windows Registry. Would you like to start a new configuration?"
                        + Environment.NewLine + Environment.NewLine
                        + "- Choose YES to proceed to Settings screen to setup a new configuration."
                        + Environment.NewLine + "- Choose NO to exit the program for now (check your file system and/or registry entries). We have copied the file path to clipboard for you."
                        + Environment.NewLine + Environment.NewLine + "HINT: If using Google Drive Stream, and it is not running, choose NO, and then startup Google Drive Stream before trying again.",
                        "TBR Booker Startup Failed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                        == DialogResult.Yes)
                    {
                        // open settings (settings will save enviornment variable)
                        var settingsFrm = new SettingsManagementFrm(Settings.CreateDefaultInst(), true);
                        return settingsFrm.ShowDialog(this) == DialogResult.OK;
                    }
                    return false;
                }
            }

            Styles.InitStyles(Settings.Inst().MainColour, Settings.Inst().ContrastColour);

            return true;
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
            calItems.AddRange(DBBox.GetCalendarItems(isForceReadAll, false));

            // read list from google calendar takes a good moment, so read 12 months upfront
            // then, going forward a week or a month does not require any reading.
            // Going forward 18 months or backewards 3 months does.
            // generate repeat markers first load: from a month ago and for the next year
            DateTime eventsStart;
            DateTime eventsEnd;
            DateTime repeatStart;
            DateTime repeatEnd;
            if (_isFirstLoad)
            {
                eventsStart = _calendarStartDate.AddMonths(-3);
                eventsEnd = _calendarStartDate.AddMonths(12);
                repeatStart = new DateTime(Math.Max(DTUtils.StartOfDay().Ticks,
                    DTUtils.StartOfDay(_calendarStartDate).Ticks)).AddDays(-30);
                repeatEnd = repeatStart.AddMonths(12);
            }
            else
            {
                eventsStart = repeatStart = _calendarStartDate;
                eventsEnd = repeatEnd = _calendarStartDate.AddDays(30);
            }

            // add the repeat markers for desired date range
            var repeatMarkers = RepeatScheduleBL.GetMarkersInRange(
                repeatStart, repeatEnd, isForceReadAll);
            calItems.AddRange(repeatMarkers);

            // add the google events for desired date range
            calItems.AddRange(CalendarBL.GetGoogleEventsForMainCalendar(
                isForceReadAll, eventsStart, eventsEnd));
                        
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

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new SettingsManagementFrm(Settings.Inst(), false);
            frm.ShowDialog(this);
        }

        private void last30DaysToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void currentFinancialYearToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void previousFinancialYearToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void allTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void allGeneralSummariesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var filename in ReportBL.GetAllReports())
            {
                //just open in chrome, let user decide whether to print now or just view
                System.Diagnostics.Process.Start(filename);
            }
        }

        private void createRecurringEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var repeat = new RepeatSchedule()
            {
                Id = "SlitherLovers",
                CustomerId = "636572137048488459-21/03/2018-102",
                Cancellations = new List<(DateTime, string)>(),
                StartDate = DTUtils.StartOfDay(new DateTime(2018, 9, 23)),
                RepeatDay = DayOfWeek.Wednesday,
                IsByDayOfWeek = true,
                Frequency = 1,
                WeekNumOfEveryMonth = 3
            };
            DBBox.AddOrUpdate(repeat);
        }

        private void searchFld_TextChanged(object sender, EventArgs e)
        {
            SearchCustomers(searchFld.Text);
        }

        private void SearchCustomers(string searchTerm)
        {
            if (searchTerm.Length < 3)  // || _isSearchingCustomers || !contactSearchChk.Checked)
                return;

           // _isSearchingCustomers = true;

            try
            {

                searchLst.Items.Clear();
                if (searchPnl.Visible == false)
                {
                    searchPnl.Visible = true;
                }

                foreach (var match in CustomerBL.SearchCustomers(searchTerm))
                {
                    var itm = new ListViewItem(match.DirectoryName);
                    itm.Tag = match.CustomerId;
                    searchLst.Items.Add(itm);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Unexpected error searching past customers", ex);          
            }
            finally
            {
              //  _isSearchingCustomers = false;
            }

        }

        private void searcCloseBtn_Click(object sender, EventArgs e)
        {
            searchLst.Items.Clear();
            bookingLst.Items.Clear();
            searchPnl.Visible = false;
        }

        private void searchLst_ItemActivate(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                var customer = DBBox.ReadItem<Customer>((string)searchLst.SelectedItems[0].Tag);
                CorporateAccount acct = null;
                if (!string.IsNullOrEmpty(customer.CompanyId))
                    acct = DBBox.ReadItem<CorporateAccount>(customer.CompanyId);
                var pastBookings = CustomerBL.GetPastBookings(customer, acct);

                bookingLst.BeginUpdate();
                bookingLst.Items.Clear();
                foreach (var booking in pastBookings
                    .OrderByDescending(x => x.BookingDate))
                {
                    var lvi = new ListViewItem(booking.Summary());
                    lvi.Tag = booking.Id;   // not using the whole booking as tag because it isn't fully read
                    bookingLst.Items.Add(lvi);
                }
                bookingLst.EndUpdate();

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                ErrorHandler.HandleError(this, "Failed to select customer", ex);
            }
        }

        private void bookingLst_ItemActivate(object sender, EventArgs e)
        {
            try
            {
                var booking = BookingBL.GetBookingFull((string)bookingLst.SelectedItems[0].Tag);
                ShowBooking(booking);
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                ErrorHandler.HandleError(this, "Failed to select booking", ex);
            }

        }
    }
}
