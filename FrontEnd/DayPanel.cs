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
using TBRBooker.Business;
using TBRBooker.Base;

namespace TBRBooker.FrontEnd
{
    public partial class DayPanel : UserControl
    {

        private MainFrm _owner;
        private DateTime _day;
        private List<CalendarItemDTO> _items;
        private bool _showCancelled;

        public DayPanel(DateTime day, List<CalendarItemDTO> items, bool showCancelled, MainFrm owner,
            bool isAllHistoryAvailable)
        {
            InitializeComponent();

            if (!isAllHistoryAvailable
                && day.AddMonths(Settings.Inst().MonthsForBookingHistory) < DateTime.Now)
            {
                //pink or purple for bookings days that might be hiding old bookings
                if (day.Month % 2 == 0)
                    BackColor = Color.FromArgb(213, 199, 234);
                else
                    BackColor = Color.FromArgb(237, 211, 230);  
            }
            else if (day.Month % 2 == 0)
                BackColor = Color.FromArgb(200, 237, 189);   //this is the green for alternate months

            _owner = owner;
            _day = day;
            _items = items.OrderBy(x => x.BookingTime).ToList();
            _showCancelled = showCancelled;

            dateLbl.Text = day.Day.ToString();

            
        }

        private void DayPanel_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                var slot = new BookingSlotPnl(_items[i], _owner);
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
            var booking = new Booking() {
                Id = BookingBL.GetNextBookingNum(),
                BookingDate = _day,
                IsNewBooking = true,
                Status = Model.Enums.BookingStates.OpenEnquiry,
                Priority = Model.Enums.BookingPriorities.Standard,
                PaymentHistory = new List<Payment>(),
                Followups = new List<Followup>(),
                Service = new Service()
                {
                    ServiceType = Model.Enums.ServiceTypes.NotSet,
                    PriceItems = new List<PriceItem>(),
                }
            };
            _owner.ShowBooking(booking);
        }
    }
}
