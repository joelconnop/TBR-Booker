using TBRBooker.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TBRBooker.Model.Entities;

namespace TBRBooker.FrontEnd
{
    public partial class BookingsFrm : Form
    {

        public Dictionary<string, BookingPnl> _panels;
        public MainFrm _owner;

        private const int FullWidth = 1920;
        private const int HalfWidth = 970;

        public BookingsFrm(MainFrm owner)
        {
            InitializeComponent();

            _owner = owner;
            _panels = new Dictionary<string, BookingPnl>();
            Styles.SetColours(this);
        }

        public void ShowOnAppropriateMonitor()
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Screen[] screens = Screen.AllScreens;
                if (screens.Length > 1)
                {
                    WindowState = FormWindowState.Normal;
                    Location = screens[Settings.Inst().BookingsScreenId].WorkingArea.Location;
                    WindowState = FormWindowState.Maximized;
                }
                else
                {
                    WindowState = FormWindowState.Maximized;
                }
                WindowState = FormWindowState.Normal;
            }
        }

        public void SwitchTabGroup(string bookingId, bool isLeftToRight)
        {
            if (isLeftToRight)
            {
                if (!leftTabs.TabPages.ContainsKey(bookingId))
                    throw new Exception($"Failed to move {bookingId} from left to right.");
            }
            else
            {
                if (!rightTabs.TabPages.ContainsKey(bookingId))
                    throw new Exception($"Failed to move {bookingId} from right to left.");
            }

            // delete + add tab pages, preserving the same booking panel
            var panel = _panels[bookingId];
            var from = isLeftToRight ? leftTabs : rightTabs;
            
            from.TabPages.RemoveByKey(bookingId);
            var to = isLeftToRight ? rightTabs : leftTabs;
            to.TabPages.Add(bookingId, bookingId);
            var page = to.TabPages[bookingId];
            page.Controls.Add(panel);
            to.SelectedTab = page;

            panel.ConfigureMoveButtons(!isLeftToRight);

            AssessWidth();
        }

        private void AssessWidth()
        {
            if (rightTabs.TabPages.Count > 0 && Width < FullWidth)
                Width = FullWidth;
            else if (rightTabs.TabPages.Count == 0 && Width > HalfWidth)
                Width = HalfWidth;
        }

        public void ShowBooking(Booking booking, string navigateFromBookingId = "")
        {
            ShowOnAppropriateMonitor();

            bool isLeft = string.IsNullOrEmpty(navigateFromBookingId);
            if (leftTabs.TabPages.ContainsKey(booking.Id))
            {
                if (isLeft)
                    leftTabs.SelectedTab = leftTabs.TabPages[booking.Id];
                else
                    SwitchTabGroup(booking.Id, true);
            }
            else if (rightTabs.TabPages.ContainsKey(booking.Id))
            {
                if (!isLeft)
                    rightTabs.SelectedTab = rightTabs.TabPages[booking.Id];
                else
                    SwitchTabGroup(booking.Id, false);
            }
            else
            {
                var tabs = isLeft ? leftTabs : rightTabs;
                //if ((leftTabs.TabCount > rightTabs.TabCount
                //    && !rightTabs.TabPages.ContainsKey(navigateFromBookingId))
                //    || leftTabs.TabPages.ContainsKey(navigateFromBookingId))
                //    tabs = rightTabs;

                tabs.TabPages.Add(booking.Id, booking.Id);
                var page = tabs.TabPages[booking.Id];
                var panel = new BookingPnl(booking, this, isLeft);
                page.Controls.Add(panel);
                tabs.SelectedTab = page;
                _panels.Add(booking.Id, panel);
            }

            AssessWidth();
        }

        public void CloseBooking(string id)
        {
            try
            {
                if (leftTabs.TabPages.ContainsKey(id))
                    leftTabs.TabPages.RemoveByKey(id);
                else
                    rightTabs.TabPages.RemoveByKey(id);
                _panels.Remove(id);

                //if nothing is on left tab, move the selected right tab to left
                if (leftTabs.TabPages.Count == 0 && rightTabs.TabPages.Count > 0)
                {
                    try
                    {
                        var bookingPnl = (BookingPnl)rightTabs.SelectedTab.Controls[0];
                        SwitchTabGroup(bookingPnl.GetBooking().Id, false);
                        bookingPnl.ConfigureMoveButtons(true);
                    }
                    catch
                    {
                        //don't really care if we couldn't do this
                    }
                }
                else if (leftTabs.TabPages.Count == 0 && rightTabs.TabPages.Count == 0)
                {
                    Close();
                }
                else if (rightTabs.TabPages.Count == 0)
                {
                    AssessWidth();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, $"Could not close booking '{id}'", ex);
            }
        }

        public void OnBookingSave()
        {
            UpdateTimeLinesForTabs(leftTabs);
            UpdateTimeLinesForTabs(rightTabs);
            _owner.UpdateCalendar();
        }

        private void UpdateTimeLinesForTabs(TabControl tabs)
        {
            foreach (var panel in _panels.Values)
            {
                panel.Timeline.UpdateOtherBookings();
            }
        }


        private bool _isQuitting { get; set; }
        public void Quit()
        {
            _isQuitting = true;
            Close();
        }

        private bool TryAndCloseClose()
        {
            try
            {
                string notSaved = "";
                var toSave = new List<string>();
                List<string> toClose = new List<string>();
                foreach (var kvp in _panels)
                {
                    if (!string.IsNullOrEmpty(kvp.Value.UnsavedChanges()))
                    {
                        notSaved += $"{kvp.Key}, ";
                        toSave.Add(kvp.Key);
                    }
                    else
                        toClose.Add(kvp.Key);
                }

                if (toSave.Any())
                {
                    var result = MessageBox.Show(this, "Save changes to the following bookings? "
                        + notSaved.Trim().Trim(','), "Closing Bookings",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    switch (result)
                    {
                        case DialogResult.Yes:
                            toClose.ForEach(x => CloseBooking(x));
                            foreach (var id in toSave)
                            {
                                if (!_panels[id].Save())
                                {
                                    return false;
                                }
                                CloseBooking(id);
                            }
                            return true;

                        case DialogResult.No:
                            return true; //(and continue to close)

                        default:    //(cancel)
                            toClose.ForEach(x => CloseBooking(x));  //at least close the ones that dont need saving
                            return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this,
                    $"Could not close bookings. You should save each one manually and restart the application", ex);
                return false;
            }
        }

        private void BookingsFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isQuitting)
            {
                if (!TryAndCloseClose())
                {
                    e.Cancel = true;
                }
            }
        }

    }
}
