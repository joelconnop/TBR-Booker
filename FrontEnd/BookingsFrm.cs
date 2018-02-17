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

        public BookingsFrm(MainFrm owner)
        {
            InitializeComponent();

            _owner = owner;
            _panels = new Dictionary<string, BookingPnl>();
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

        public void ShowBooking(Booking booking, string navigateFromBookingId = "")
        {
            ShowOnAppropriateMonitor();

            if (tabs1.TabPages.ContainsKey(booking.Id))
            {
                tabs1.SelectedTab = tabs1.TabPages[booking.Id];
            }
            else if (tabs2.TabPages.ContainsKey(booking.Id))
            {
                tabs2.SelectedTab = tabs2.TabPages[booking.Id];
            }
            else
            {
                var tabs = tabs1;
                if ((tabs1.TabCount > tabs2.TabCount
                    && !tabs2.TabPages.ContainsKey(navigateFromBookingId))
                    || tabs1.TabPages.ContainsKey(navigateFromBookingId))
                    tabs = tabs2;

                tabs.TabPages.Add(booking.Id, booking.Id);
                var page = tabs.TabPages[booking.Id];
                var panel = new BookingPnl(booking, this);
                page.Controls.Add(panel);
                tabs.SelectedTab = page;
                _panels.Add(booking.Id, panel);
            }
        }

        public void CloseBooking(string id)
        {
            try
            {
                if (tabs1.TabPages.ContainsKey(id))
                    tabs1.TabPages.RemoveByKey(id);
                else
                    tabs2.TabPages.RemoveByKey(id);
                _panels.Remove(id);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, $"Could not close booking '{id}'", ex);
            }
        }

        public void OnBookingSave()
        {
            UpdateTimeLinesForTabs(tabs1);
            UpdateTimeLinesForTabs(tabs2);
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

        private void BookingsFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isQuitting)
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
                                        e.Cancel = true;
                                        return;
                                    }
                                    CloseBooking(id);
                                }
                                break;

                            case DialogResult.No:
                                return; //(and continue to close)

                            default:    //(cancel)
                                toClose.ForEach(x => CloseBooking(x));  //at least close the ones that dont need saving
                                e.Cancel = true;
                                break;
                        }
                    }

                    //WindowState = FormWindowState.Minimized;
                    //e.Cancel = true;
                }
                catch (Exception ex)
                {
                    ErrorHandler.HandleError(this, 
                        $"Could not close bookings. You should save each one manually and restart the application", ex);
                }
            }
        }

        private void BookingsFrm_Load(object sender, EventArgs e)
        {

        }
    }
}
