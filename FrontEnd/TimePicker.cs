using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TBRBooker.Base;

namespace TBRBooker.FrontEnd
{
    public partial class TimePicker : UserControl
    {
        public TimePicker()
        {
            InitializeComponent();
        }

        private Action<TimePickerValue> _onChange;
        private bool _isSettingUp;

        public void SetEventHandler(Action<TimePickerValue> onChange)
        {
            _onChange = onChange;
        }

        public void InitTimes(int startTime = 0, int endTime = 0, int interval = 15, int startingValue = 0)
        {
            _isSettingUp = true;
            picker.BeginUpdate();
            picker.Items.Clear();

            startingValue = Math.Max(Math.Min(startingValue, endTime), startTime);
            TimePickerValue selected = null;

            for (int i = startTime; i <= endTime; i++)
            {
                //the loop isn't great; need to skip numbers that aren't legit minutes
                if (i - (i / 100 * 100) >= 60)
                    continue;

                var parsed = Utils.ParseTime(i);

                if (parsed.Minute % interval == 0 || i == startingValue)
                {
                    var tpv = new TimePickerValue(i, Utils.DisplayTime(parsed));
                    if (i == startingValue)
                        selected = tpv;
                    picker.Items.Add(tpv);
                }
            }

            picker.EndUpdate();
            if (selected != null)
                picker.SelectedItem = selected;

            _isSettingUp = false;
        }

        public TimePickerValue GetSelected()
        {
            if (picker.SelectedIndex >= 0)
                return (TimePickerValue)picker.SelectedItem;
            return (TimePickerValue)picker.Items[0];
        }

        public class TimePickerValue
        {
            public readonly int Value;
            public readonly string DisplayStr;

            public TimePickerValue(int value, string displayStr)
            {
                Value = value;
                DisplayStr = displayStr;
            }

            public override string ToString()
            {
                return DisplayStr;
            }
        }

        private void picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_isSettingUp)
                _onChange((TimePickerValue)picker.SelectedItem);
        }
    }
}
