using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TBRBooker.Model.Entities;
using TBRBooker.Model.DTO;

namespace TBRBooker.FrontEnd
{
    public partial class DayPanel : UserControl
    {

        private DateTime _day;
        private List<CalendarItemDTO> _items;
        private bool _showCancelled;

        public DayPanel(DateTime day, List<CalendarItemDTO> items, bool showCancelled)
        {
            InitializeComponent();

            _day = day;
            _items = items;
            _showCancelled = showCancelled;

            dateLbl.Text = day.Day.ToString();

            
        }

        private void DayPanel_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                var slot = new BookingSlotPnl(_items[i]);
                Controls.Add(slot);
                slot.Location = new Point(0, i * (slot.Height + 2));
                var divider = new Label()
                {
                    Name = "divider" + i + "Lbl",
                    Location = new Point(1, slot.Location.Y + slot.Height - 5),
                    Text = "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -",
                    ForeColor = Color.FromArgb(35, 168, 239),
                    Size = new Size(this.Width - 2, 10)
                };
                Controls.Add(divider);
                divider.SendToBack();
            }
        }

        private void addLeadBtn_Click(object sender, EventArgs e)
        {
            var booking = new Booking() { BookingDate = _day };
            var frm = new BookingFrm(null);
            frm.Show();
        }
    }
}
