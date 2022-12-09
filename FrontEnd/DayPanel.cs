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
        private List<CalendarItemDTO> _rawItems;
        private List<CalendarItemDTO> _items;
        private int _pageNum = 0;
        private bool _showCancelled;
        private List<Control> _slotControls;

        public DayPanel(DateTime day, List<CalendarItemDTO> startingItems, bool showCancelled, MainFrm owner,
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
            _rawItems = startingItems;
            OrderAndFilter();

            _showCancelled = showCancelled;

            dateLbl.Text = day.Day.ToString();

            _slotControls = new List<Control>();
        }

        private void OrderAndFilter()
        {
            if (_rawItems.Count > 4 || !_showCancelled)
            {
                var hideStates = new[] { BookingStates.Cancelled, BookingStates.CancelledWithoutPayment,
                    BookingStates.LostEnquiry};
                _items = _rawItems.Where(x => !(x is BookingCalendarItemDTO) ||
                !hideStates.Contains(((BookingCalendarItemDTO)x).BookingStatus)).ToList();

                if (_items.Count > 4)
                {
                    // paginate!
                    nextBtn.Visible = true;
                    prevBtn.Visible = true;
                    if (_showCancelled)
                    {
                        _items = _rawItems; // and might as well restore cancelled items
                    }
                }
            }

            _items = _items.OrderBy(
                x => x is GoogleCalendarItemDTO && ((GoogleCalendarItemDTO)x).IsAllDay())
                .ThenBy(x => x.Time).ToList();
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
            var startIdx = _pageNum * 3;
            var numSlots = _items.Count > 4 ? Math.Min(3, _items.Count - startIdx) : _items.Count;
            for (int i = 0; i < numSlots; i++)
            {
                var slot = new BookingSlotPnl(_items[startIdx + i], _owner, this, Height - y);
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
                    _rawItems.Add(calItems[0]);
                    OrderAndFilter();
                    ReloadItems();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(_owner, "creating google event", ex, true);
            }
        }

        private void nextBtn_Click(object sender, EventArgs e)
        {
            var maxPageNum = (int)Math.Ceiling(_items.Count / 3.0) - 1;
            prevBtn.Enabled = true;
            _pageNum++;
            if (_pageNum == maxPageNum)
            {
                nextBtn.Enabled = false;
            }

            ReloadItems();
        }

        private void prevBtn_Click(object sender, EventArgs e)
        {
            nextBtn.Enabled = true;
            _pageNum--;
            if (_pageNum == 0)
            {
                prevBtn.Enabled = false;
            }

            ReloadItems();
        }
    }
}
