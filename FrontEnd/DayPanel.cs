using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TBRBooker.FrontEnd
{
    public partial class DayPanel : UserControl
    {
        public DayPanel(int dayOfMonth)
        {
            InitializeComponent();

            dateLbl.Text = dayOfMonth.ToString();
            int maxSlots = 3;

            for (int i = 0; i < maxSlots; i++)
            {
                var slot = new BookingSlotPnl();
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

        private void DayPanel_Load(object sender, EventArgs e)
        {

        }
    }
}
