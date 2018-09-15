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
        private List<Control> _slotControls;

        public DayPanel(DateTime day, List<CalendarItemDTO> items, bool showCancelled, MainFrm owner,
            bool isAllHistoryAvailable)
        {
            InitializeComponent();

            Styles.SetColours(this);
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
            _items = items.OrderBy(x => x.Time).ToList();
            _showCancelled = showCancelled;

            dateLbl.Text = day.Day.ToString();

            _slotControls = new List<Control>();
        }

        private void DayPanel_Load(object sender, EventArgs e)
        {
            ReloadItems();
        }

        private void ReloadItems()
        {
            _slotControls.ForEach(x => Controls.Remove(x));

            for (int i = 0; i < _items.Count; i++)
            {
                var slot = new BookingSlotPnl(_items[i], _owner);
                _slotControls.Add(slot);
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
                _slotControls.Add(divider);
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
                },
                HighlightedControls = new List<string>()
            };
            _owner.ShowBooking(booking);
        }

        private void createGoogleEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var frm = new GoogleEventFrm(_owner, DTUtils.StartOfDay(_day));
                if (frm.ShowDialog(_owner) == DialogResult.OK)
                {
                    var calItems = frm.GetValue();
                    foreach (var calItm in calItems)
                    {
                        TheGoogle.AddGoogleCalendarEvent(calItm);
                    }
                    _items.Add(calItems[0]);
                    _items = _items.OrderBy(x => x.Time).ToList();
                    ReloadItems();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(_owner, "creating google event", ex, true);
            }
        }
    }
}
