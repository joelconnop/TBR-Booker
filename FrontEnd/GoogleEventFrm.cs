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
using TBRBooker.Model.DTO;
using TBRBooker.Business;

namespace TBRBooker.FrontEnd
{
    public partial class GoogleEventFrm : Form
    {
        public MainFrm _owner;
        DateTime _day;
        int _duration;
        bool _isChangingTime;

        public GoogleEventFrm(MainFrm owner, DateTime day)
        {
            InitializeComponent();

            Styles.SetFormStyles(this);
            _owner = owner;
            _day = day;
            dateFld.Text = day.ToShortDateString();
        }

        private void GoogleEventFrm_Load(object sender, EventArgs e)
        {
            _isChangingTime = true;

            startPick.SetEventHandler(StartTimeChanged);
            endPick.SetEventHandler(EndTimeChanged);

            startPick.SetValues(600, 1900, 15, 900);
            endPick.SetValues(600, 1900, 15, 1700);

            _duration = 8 * 60;
            durationFld.Text = _duration.ToString();
            SetDurationDesc(_duration);

            _isChangingTime = false;
        }

        public List<GoogleCalendarItemDTO> GetValue()
        {
            var events = new List<GoogleCalendarItemDTO>();
            int numDays = 1;
            int.TryParse(numDaysFld.Text, out numDays);
            numDays = Math.Min(numDays, 30);    // would be hard to undo a typo of an extra 0 or two!
            var currentDay = _day;

            while (numDays > 0)
            {
                List<string> attendeesNotImplemented = null;
                events.Add(new GoogleCalendarItemDTO(currentDay, startPick.GetSelected().Value,
                    _duration, nameFld.Text, GoogleCalendarItemDTO.Blockout + descFld.Text, attendeesNotImplemented,
                    null, //CalendarBL.Blockout + Guid.NewGuid().ToString(), 
                    locationFld.Text));
                numDays--;
                currentDay = currentDay.AddDays(1);
            }

            return events;
        }

        private void allDayChk_CheckedChanged(object sender, EventArgs e)
        {
            startPick.Enabled = endPick.Enabled = durationFld.Enabled = !allDayChk.Checked;
            if (allDayChk.Checked)
            {
                _isChangingTime = true;
                startPick.SetValues(600, 1900, 15, 900);
                endPick.SetValues(600, 1900, 15, 1700);
                _duration = 8 * 60;
                durationFld.Text = _duration.ToString();
                SetDurationDesc(_duration);
            }
        }


        #region Date and Timeline

        public void StartTimeChanged(TimePicker.TimePickerValue tpv)
        {
            try
            {
                if (_isChangingTime || !startPick.Visible)
                    return;

                UpdateEndTime(tpv.Value);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to update the timeline", ex);
            }
        }

        private void UpdateEndTime(int newStartTime)
        {
            var newEndTime = 0;
            if (_duration > 0)
            {
                newEndTime = DTUtils.EndTime(newStartTime, _duration);
            }
            endPick.SetValues(newStartTime, 1900, 15, newEndTime);
        }

        public void EndTimeChanged(TimePicker.TimePickerValue tpv)
        {
            //this field doesn't manage a property directly, it instead affects the duration
            try
            {
                if (_isChangingTime)
                    return;

                if (startPick.GetSelected().Value > 0)
                {
                    int newDuration = DTUtils.MinuteDifference(startPick.GetSelected().Value, tpv.Value);
                    if (newDuration != _duration)
                    {
                        _duration = newDuration;
                        durationFld.Text = _duration.ToString();
                        SetDurationDesc(_duration);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to update the timeline", ex);
            }
        }

        private void timePick_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                e.Handled = true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to change the time", ex);
            }
        }

        private void durationFld_Leave(object sender, EventArgs e)
        {
            try
            {
                if (_isChangingTime)
                    return;

                int newDuration = 0;
                if (!int.TryParse(durationFld.Text.Trim(), out newDuration))
                {
                    //revert
                    durationFld.Text = _duration.ToString();
                }
                else if (newDuration != _duration)
                {
                    _duration = newDuration;
                    UpdateEndTime(startPick.GetSelected().Value);
                    SetDurationDesc(newDuration);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to update the timeline", ex);
            }
        }

        private void SetDurationDesc(int newDuration)
        {
            if (newDuration > 0)
            {
                durationDescFld.Text = "(" + DTUtils.DurationToDisplayStr(newDuration) + ")";
            }
            else
            {
                durationDescFld.Text = "";
            }
        }

        public void SetTime(int time, int duration)
        {
            _isChangingTime = true;

            startPick.SetValues(600, 1900, 15, time);

            endPick.SetValues(time > 0 ? time : 600, 1900, 15,
                duration > 0 ? DTUtils.EndTime(time, duration) : 0);

            _duration = duration;
            durationFld.Text = duration.ToString();
            SetDurationDesc(duration);

            _isChangingTime = false;
        }

        #endregion

        private void saveBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(nameFld.Text))
            {
                MessageBox.Show(this, "Please enter a name for the event", "Blockout Event");
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
