using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
        private CancellationTokenSource _calendarRefreshCts;
        private readonly object _calendarLoadingSync = new object();
        public SaveWorker SaveWorker;

        private (DateTime Start, DateTime End) CalculateEventWindow(DateTime calendarStartSnapshot, bool isForceReadAll, bool isFirstLoadSnapshot)
        {
            if (isForceReadAll || isFirstLoadSnapshot)
            {
                return (calendarStartSnapshot.AddMonths(-1), calendarStartSnapshot.AddMonths(3));
            }

            return (calendarStartSnapshot.AddDays(-7), calendarStartSnapshot.AddDays(35));
        }

        private static string FormatRange(DateTime start, DateTime end)
        {
            return $"{start:dd MMM yyyy} - {end:dd MMM yyyy}";
        }

        private void UpdateCalendarStatus(string message)
        {
            if (calendarLbl == null)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateCalendarStatus(message)));
                return;
            }

            calendarLbl.Text = message ?? string.Empty;
        }

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
                SaveWorker = new SaveWorker();
                SaveWorker.InitializeBackgroundWorker(SavingPic);
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
            StartCalendarRefresh(false);
        }

        private void StartCalendarRefresh(bool isForceReadAll)
        {
            CancellationTokenSource cts;
            lock (_calendarLoadingSync)
            {
                _calendarRefreshCts?.Cancel();
                _calendarRefreshCts = new CancellationTokenSource();
                cts = _calendarRefreshCts;
            }

            var token = cts.Token;
            var calendarStartSnapshot = _calendarStartDate;
            var isFirstLoadSnapshot = _isFirstLoad;
            var window = CalculateEventWindow(calendarStartSnapshot, isForceReadAll, isFirstLoadSnapshot);
            UpdateCalendarStatus($"Loading bookings {FormatRange(window.Start, window.End)}...");

            ShowCalendarLoading(true);

            Task.Run(() =>
            {
                try
                {
                    var baseItems = BuildBaseCalendarItems(isForceReadAll, token, window.Start, window.End) ?? new List<CalendarItemDTO>();
                    var baseSnapshot = new List<CalendarItemDTO>(baseItems);

                    if (token.IsCancellationRequested)
                    {
                        UpdateCalendarStatus(string.Empty);
                        return;
                    }

                    BeginInvoke(new Action(() =>
                    {
                        if (token.IsCancellationRequested)
                        {
                            UpdateCalendarStatus(string.Empty);
                            return;
                        }

                        try
                        {
                            RenderDayPanels(baseSnapshot, isForceReadAll);
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.HandleError(this, "Failed to update the calendar", ex);
                        }
                        finally
                        {
                            ShowCalendarLoading(false);
                        }
                    }));

                    if (token.IsCancellationRequested)
                    {
                        UpdateCalendarStatus(string.Empty);
                        return;
                    }

                    if (!TheGoogle.GoogleMapsOn)
                    {
                        UpdateCalendarStatus("Google Maps/Calendar are turned off");
                        return;
                    }

                    UpdateCalendarStatus($"Loading Google events {FormatRange(window.Start, window.End)}...");

                    Task.Run(() =>
                    {
                        try
                        {
                            var googleItems = CalendarBL.GetGoogleEventsForMainCalendar(
                                isForceReadAll, window.Start, window.End);

                            if (token.IsCancellationRequested)
                                return;

                            if (googleItems == null || googleItems.Count == 0)
                            {
                                UpdateCalendarStatus(string.Empty);
                                return;
                            }

                            var combined = new List<CalendarItemDTO>(baseSnapshot);
                            combined.AddRange(googleItems);

                            BeginInvoke(new Action(() =>
                            {
                                if (token.IsCancellationRequested)
                                    return;

                                try
                                {
                                    RenderDayPanels(combined, isForceReadAll);
                                    UpdateCalendarStatus(string.Empty);
                                }
                                catch (Exception ex)
                                {
                                    ErrorHandler.HandleError(this, "Failed to update the calendar", ex);
                                }
                            }));
                        }
                        catch (OperationCanceledException)
                        {
                            UpdateCalendarStatus(string.Empty);
                        }
                        catch (Exception ex)
                        {
                            ErrorLogger.LogError("load blockouts", ex);
                            UpdateCalendarStatus($"Google events failed to load {FormatRange(window.Start, window.End)}");
                        }
                    }, token);
                }
                catch (OperationCanceledException)
                {
                    BeginInvoke(new Action(() => ShowCalendarLoading(false)));
                    UpdateCalendarStatus(string.Empty);
                }
                catch (Exception ex)
                {
                    BeginInvoke(new Action(() =>
                    {
                        ShowCalendarLoading(false);
                        ErrorHandler.HandleError(this, "Failed to update the calendar", ex);
                        UpdateCalendarStatus("Calendar failed to load.");
                    }));
                }
            }, token);
        }

        private List<CalendarItemDTO> BuildBaseCalendarItems(
            bool isForceReadAll,
            CancellationToken token,
            DateTime eventsStart,
            DateTime eventsEnd)
        {
            var calItems = new List<CalendarItemDTO>(DBBox.GetCalendarItems(isForceReadAll, false));
            token.ThrowIfCancellationRequested();

            var repeatStart = DTUtils.StartOfDay(eventsStart);
            var repeatEnd = DTUtils.StartOfDay(eventsEnd);

            var repeatMarkers = RepeatScheduleBL.GetMarkersInRange(
                repeatStart, repeatEnd, isForceReadAll);
            calItems.AddRange(repeatMarkers);

            token.ThrowIfCancellationRequested();

            return calItems;
        }

        private void ShowCalendarLoading(bool isLoading)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => ShowCalendarLoading(isLoading)));
                return;
            }

            UseWaitCursor = isLoading;
            Cursor = isLoading ? Cursors.AppStarting : Cursors.Default;
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

        private void RenderDayPanels(List<CalendarItemDTO> calItems, bool isForceReadAll)
        {
            daysPanel.SuspendLayout();
            daysPanel.Controls.Clear();
            _days = new DayPanel[4,7];

            var day = _calendarStartDate;
            for (int i = 0; i <= 3; i++)
            {
                for (int j = 0; j <= 6; j++)
                {
                    var dayPnl = new DayPanel(day,
                        calItems.Where(x => DTUtils.SameDay(x.Date, day)).ToList(),
                        true, this, isForceReadAll || _isAllHistoryAvailable);
                    daysPanel.Controls.Add(dayPnl);
                    dayPnl.Location = new Point(j * (dayPnl.Size.Height + 5) + 5, i * (dayPnl.Size.Width + 5) + 5);
                    _days[i, j] = dayPnl;
                    day = day.AddDays(1);
                }
            }

            _isFirstLoad = false;
            daysPanel.ResumeLayout();

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
            if (!dateTmr.Enabled)
            {
                dateTmr.Enabled = true;
                dateTmr.Start();
                _calendarStartDate = DTUtils.StartOfDay(PickCalendarStartDate(datePicker.Value));
                UpdateCalendar();
            }
        }

        private void dateTmr_Tick(object sender, EventArgs e)
        {
            // they have waited 3 secs since last calendar refresh. Allow it to happen again
            dateTmr.Stop();
            dateTmr.Enabled = false;
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
                    _isAllHistoryAvailable = true;
                    StartCalendarRefresh(true);
                }
                catch (Exception ex)
                {
                    ErrorHandler.HandleError(this, "Failed to update the calendar", ex);
                }
            }
        }

        private void InitBookingsFrm()
        {
            // PenaltyBL.UpdatePenalties(); // abandoned this idea, and it carries unneccessary risk of program/data instability
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
            Cursor = Cursors.WaitCursor;
            var file = ReportBL.GetReportFile("Last 30 Days", DTUtils.StartOfDay(datePicker.Value).AddMonths(-1), DTUtils.EndOfDay(datePicker.Value.AddDays(-1)));
            System.Diagnostics.Process.Start(file);
            Cursor = Cursors.Default;
        }

        private void currentFinancialYearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            var file = ReportBL.GetCurrentYearReport(datePicker.Value);
            System.Diagnostics.Process.Start(file);
            Cursor = Cursors.Default;
        }

        private void previousFinancialYearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            var file = ReportBL.GetPreviousYearReport(datePicker.Value);
            System.Diagnostics.Process.Start(file);
            Cursor = Cursors.Default;
        }

        private void last12MonthsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            var file = ReportBL.GetReportFile("Last 12 Months", DTUtils.StartOfDay(datePicker.Value).AddDays(-365), DTUtils.EndOfDay(datePicker.Value.AddDays(-1)));
            System.Diagnostics.Process.Start(file);
            Cursor = Cursors.Default;
        }

        private void allTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            var file = ReportBL.GetReportFile("All Time", DateTime.MinValue, DateTime.MaxValue);
            System.Diagnostics.Process.Start(file);
            Cursor = Cursors.Default;
        }

        private void selectedMonth1YearAgoJobkeeperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            var targetDate = datePicker.Value.AddDays(-365);
            var file = ReportBL.GetReportFile(targetDate.ToString("MMMM yyyy"), DTUtils.StartOfMonth(targetDate), DTUtils.EndOfMonth(targetDate));
            System.Diagnostics.Process.Start(file);
            Cursor = Cursors.Default;
        }

        private void allGeneralSummariesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (var filename in ReportBL.GetAllReports(datePicker.Value))
            {
                //just open in chrome, let user decide whether to print now or just view
                System.Diagnostics.Process.Start(filename);
            }
        }

        private void travelLogLastFYearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                this,
                "This report uses a lot of google api calls and will be slow and possibly cost a tiny amount of real money. Proceed?",
                "Travel Log",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning)
                == DialogResult.No)
            {
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                var file = ReportBL.GetTravelLog(datePicker.Value);
                System.Diagnostics.Process.Start(file);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Unexpected error generating the Travel Log", ex, true);          
            }
            finally
            {
                Cursor = Cursors.Default;
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

                var booking = BookingBL.SearchBooking(searchTerm);
                if (booking != null)
                {
                    var lvi = new ListViewItem(booking.Summary());
                    lvi.Tag = booking.Id;
                    bookingLst.Items.Add(lvi);
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
                if (searchLst.SelectedItems[0].Tag is null)
                {
                    MessageBox.Show(
                        this,
                        "The selected item is not a valid customer.", "Customer Search",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

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

        private void penaltiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var dlg = new InputDialog("Enter the password to print and reset penalties:",
                "", false, ValidatingTextbox.TextBoxValidationType.GeneralDatabase, true);
            dlg.ShowDialog(this);
            if (dlg.DialogResult == DialogResult.OK)
                PenaltyBL.PrintAndAbsolvePenalties();
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                ErrorHandler.HandleError(this, "Failed to select print and reset penalties", ex, true);
            }
        }


        private BackgroundWorker bgWorker = new BackgroundWorker();

        public void InitializeBackgroundWorker()
        {
            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
            bgWorker.ProgressChanged += new ProgressChangedEventHandler(bgWorker_ProgressChanged);
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var booking = e.Argument as Booking;
            // Perform the save operation here
            DBBox.AddOrUpdate(booking);
            // You can report progress if needed
            bgWorker.ReportProgress(50);
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("Operation was canceled.");
            }
            else if (e.Error != null)
            {
                MessageBox.Show("Error occurred: " + e.Error.Message);
            }
            else
            {
                MessageBox.Show("Booking saved successfully!");
            }
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // progressBar.Value = e.ProgressPercentage; // Update progress bar if needed
        }

        // To start the operation:
        public void SaveBooking(Booking booking)
        {
            if (!bgWorker.IsBusy)
            {
                bgWorker.RunWorkerAsync(booking);
            }
            else
            {
                MessageBox.Show("A save operation is already running.");
            }
        }

        private void googleOnMenuItem_Click(object sender, EventArgs e)
        {
            TheGoogle.GoogleMapsOn = true;
            googleOnMenuItem.Enabled = false;
            googleOnMenuItem.Checked = true;
            googleOffMenuItem.Enabled = true;
            googleOffMenuItem.Checked = false;
        }

        private void googleOffMenuItem_Click(object sender, EventArgs e)
        {
            TheGoogle.GoogleMapsOn = false;
            googleOnMenuItem.Enabled = true;
            googleOnMenuItem.Checked = false;
            googleOffMenuItem.Enabled = false;
            googleOffMenuItem.Checked = true;
        }
    }
}
