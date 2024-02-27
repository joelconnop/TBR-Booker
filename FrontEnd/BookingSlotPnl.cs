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
using TBRBooker.Model.Enums;
using TBRBooker.Base;

namespace TBRBooker.FrontEnd
{
    public partial class BookingSlotPnl : UserControl
    {

        private CalendarItemDTO _calItm;
        private MainFrm _mainFrm;
        private DayPanel _owner;
        private int _maxHeight;
        
        public BookingSlotPnl(CalendarItemDTO calItm, MainFrm mainFrm, DayPanel owner, int maxHeight)
        {
            InitializeComponent();

            Styles.SetColours(this);
            _calItm = calItm;
            _mainFrm = mainFrm;
            _owner = owner;
            _maxHeight = maxHeight;
        }

        private void BookingSlotPnl_Load(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            timeLbl.Text = Booking.BookingTimeStr(_calItm.Time);
            switch (_calItm.Type)
            {
                case CalendarItemTypes.Booking:
                    var bookingItm = _calItm as BookingCalendarItemDTO;
                    bookingLbl.Text = bookingItm.BookingNum + " " + bookingItm.Name;
                    BackColor = BackColourForBooking(bookingItm.BookingStatus);
                    break;
                case CalendarItemTypes.GoogleEvent:
                    var googleItm = _calItm as GoogleCalendarItemDTO;
                    bookingLbl.Text = _calItm.Name;
                    bookingLbl.ForeColor = timeLbl.ForeColor = Styles.MainColour();
                    bookingLbl.Font = timeLbl.Font = new Font(bookingLbl.Font, FontStyle.Bold);
                    if (googleItm.IsAllDay())
                    {
                        timeLbl.Visible = false;
                        Height = Math.Min(_maxHeight, Height * 3);
                    }
                        
                    SetBackColour();
                    break;
                case CalendarItemTypes.RepeatMarker:
                    var repeatItm = _calItm as RepeatMarkerDTO;
                    bookingLbl.Text = _calItm.Name;
                    confirmBtn.Visible = rejectBtn.Visible = true;
                    BackColor = BackColourForBooking(BookingStates.OpenEnquiry);
                    break;
                default:
                    ErrorHandler.HandleError(_mainFrm, "Update calendar item slot",
                        new Exception("Unknown CalendarItemTypes " + _calItm.Type), true);
                    break;
            }

        }

        private void SetBackColour()
        {
            switch (_calItm.Type)
            {
                case CalendarItemTypes.Booking:
                    var bookingItm = _calItm as BookingCalendarItemDTO;
                    BackColor = BackColourForBooking(bookingItm.BookingStatus);
                    break;
                case CalendarItemTypes.GoogleEvent:
                    BackColor = Color.DarkRed;
                    break;
                case CalendarItemTypes.RepeatMarker:
                    BackColor = BackColourForBooking(BookingStates.OpenEnquiry);
                    break;
                default:
                    ErrorHandler.HandleError(_mainFrm, "Update calendar item slot",
                        new Exception("Unknown CalendarItemTypes " + _calItm.Type), true);
                    break;
            }
        }

        public static Color BackColourForBooking(BookingStates bookingState)
        {
            switch (bookingState)
            {
                case BookingStates.Cancelled:
                case BookingStates.LostEnquiry:
                    return Color.Silver;
                case BookingStates.Completed:
                    return Color.Transparent;
                case BookingStates.Booked:
                    return Color.LightSkyBlue;
                case BookingStates.OpenEnquiry:
                    return Color.Orange;
                case BookingStates.PaymentDue:
                case BookingStates.BadDept:
                    return Color.LightPink;
                default:
                    throw new Exception("Unknown back colour for " + bookingState);
            }
        }

        private void BookingSlotPnl_MouseUp(object sender, MouseEventArgs e)
        {
            ActivateSlot();
        }

        private void ActivateSlot()
        {
            switch (_calItm.Type)
            {
                case CalendarItemTypes.Booking:
                    SetBackColour();
                    var bookingItm = _calItm as BookingCalendarItemDTO;
                    _mainFrm.ShowBooking(BookingBL.GetBookingFull(bookingItm.BookingNum.ToString()));
                    break;
                case CalendarItemTypes.GoogleEvent:
                    SetBackColour();
                    var googleItm = _calItm as GoogleCalendarItemDTO;
                    MessageBox.Show(_mainFrm, googleItm.ToString(),
                        "Google Calendar Event", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    break;
                case CalendarItemTypes.RepeatMarker:
                    break;
                default:
                    ErrorHandler.HandleError(_mainFrm, "Activate Calendar Item",
                        new Exception("Unknown CalendarItemTypes " + _calItm.Type), true);
                    break;
            }
        }

        private void BookingSlotPnl_MouseEnter(object sender, EventArgs e)
        {
            if (_calItm.Type != CalendarItemTypes.RepeatMarker)
                BackColor = Color.LightGoldenrodYellow;
        }

        private void BookingSlotPnl_MouseLeave(object sender, EventArgs e)
        {
            SetBackColour();
        }

        private void bookingLbl_MouseEnter(object sender, EventArgs e)
        {
            if (_calItm.Type != CalendarItemTypes.RepeatMarker)
                BackColor = Color.LightGoldenrodYellow;
        }

        private void timeLbl_Click(object sender, EventArgs e)
        {
            ActivateSlot();
        }

        private void bookingLbl_Click(object sender, EventArgs e)
        {
            ActivateSlot();
        }

        private void timeLbl_MouseEnter(object sender, EventArgs e)
        {
            if (_calItm.Type != CalendarItemTypes.RepeatMarker)
                BackColor = Color.LightGoldenrodYellow;
        }

        private void bookingLbl_MouseLeave(object sender, EventArgs e)
        {
            SetBackColour();
        }

        private void timeLbl_MouseLeave(object sender, EventArgs e)
        {
            SetBackColour();
        }

        private void rejectBtn_Click(object sender, EventArgs e)
        {
            var reasonFrm = new InputDialog(
                "Please record why this repeat is not going ahead", "", true);
            if (reasonFrm.ShowDialog(this) == DialogResult.Cancel)
                return;
            RepeatScheduleBL.RejectRepeatBooking((RepeatMarkerDTO)_calItm, reasonFrm.InputValue);
            _owner.RemoveSlot((RepeatMarkerDTO)_calItm);
        }

        private void confirmBtn_Click(object sender, EventArgs e)
        {
            _calItm = 
                RepeatScheduleBL.ConfirmBookingFromRepeatMarker((RepeatMarkerDTO)_calItm);
            confirmBtn.Visible = rejectBtn.Visible = false;
            UpdateDisplay();
        }
    }
}
