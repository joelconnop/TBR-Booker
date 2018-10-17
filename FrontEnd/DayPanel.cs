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
using TBRBooker.Model.Enums;

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
            if (items.Count > 4 || !showCancelled)
            {
                var hideStates = new[] { BookingStates.Cancelled, BookingStates.CancelledWithoutPayment,
                    BookingStates.LostEnquiry};
                items = items.Where(x => !(x is BookingCalendarItemDTO) ||
                !hideStates.Contains(((BookingCalendarItemDTO)x).BookingStatus)).ToList();
            }
            _items = items.OrderBy(
                x => x is GoogleCalendarItemDTO && ((GoogleCalendarItemDTO)x).IsAllDay())
                .ThenBy(x => x.Time).ToList();
            _showCancelled = showCancelled;

            dateLbl.Text = day.Day.ToString();

            _slotControls = new List<Control>();
        }

        public void RemoveSlot(CalendarItemDTO slot)
        {
            _items.Remove(slot);
            ReloadItems();
        }

        private void DayPanel_Load(object sender, EventArgs e)
        {
            ReloadItems();
        }

        private void ReloadItems()
        {
            _slotControls.ForEach(x => Controls.Remove(x));

            int y = 2;  // 2 is just a little offset for between dividers
            for (int i = 0; i < _items.Count; i++)
            {
                var slot = new BookingSlotPnl(_items[i], _owner, this, Height - y);
                _slotControls.Add(slot);
                Controls.Add(slot);
                slot.Location = new Point(0, y);
                var divider = new Label()
                {
                    Name = "divider" + i + "Lbl",
                    Location = new Point(1, slot.Location.Y + slot.Height - 5),
                    Text = "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -",
                    ForeColor = Styles.MainColour(),
                    Size = new Size(this.Width - 2, 10)
                };
                _slotControls.Add(divider);
                Controls.Add(divider);
                divider.SendToBack();
                y += slot.Height;
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
