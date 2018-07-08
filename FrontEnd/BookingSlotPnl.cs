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

namespace TBRBooker.FrontEnd
{
    public partial class BookingSlotPnl : UserControl
    {

        private CalendarItemDTO _calItm;
        private MainFrm _owner;
        
        public BookingSlotPnl(CalendarItemDTO calItm, MainFrm owner)
        {
            InitializeComponent();

            _calItm = calItm;
            _owner = owner;
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
                    SetBackColourForBooking(bookingItm.BookingStatus);
                    break;
                case CalendarItemTypes.GoogleEvent:
                    // var googleItm = _calItm as GoogleCalendarItemDTO;
                    bookingLbl.Text = _calItm.Name;
                    SetBackColour();
                    break;
                default:
                    ErrorHandler.HandleError(_owner, "Update calendar item slot",
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
                    SetBackColourForBooking(bookingItm.BookingStatus);
                    break;
                case CalendarItemTypes.GoogleEvent:
                    BackColor = Color.PaleVioletRed;
                    break;
                default:
                    ErrorHandler.HandleError(_owner, "Update calendar item slot",
                        new Exception("Unknown CalendarItemTypes " + _calItm.Type), true);
                    break;
            }
        }

        private void SetBackColourForBooking(BookingStates bookingState)
        {
            switch (bookingState)
            {
                case BookingStates.Cancelled:
                case BookingStates.LostEnquiry:
                    BackColor = Color.Silver;
                    break;
                case BookingStates.Completed:
                    BackColor = Color.Transparent;
                    break;
                case BookingStates.Booked:
                    BackColor = Color.LightSkyBlue;
                    break;
                case BookingStates.OpenEnquiry:
                    BackColor = Color.Orange;
                    break;
                case BookingStates.PaymentDue:
                case BookingStates.CancelledWithoutPayment:
                    BackColor = Color.LightPink;
                    break;
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
            SetBackColour();

            switch (_calItm.Type)
            {
                case CalendarItemTypes.Booking:
                    var bookingItm = _calItm as BookingCalendarItemDTO;
                    _owner.ShowBooking(BookingBL.GetBookingFull(bookingItm.BookingNum.ToString()));
                    break;
                case CalendarItemTypes.GoogleEvent:
                    var googleItm = _calItm as GoogleCalendarItemDTO;
                    MessageBox.Show(_owner, googleItm.ToString(),
                        "Google Calendar Event", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    break;
                default:
                    ErrorHandler.HandleError(_owner, "Activate Calendar Item",
                        new Exception("Unknown CalendarItemTypes " + _calItm.Type), true);
                    break;
            }
        }

        private void BookingSlotPnl_MouseEnter(object sender, EventArgs e)
        {
            BackColor = Color.LightGoldenrodYellow;
        }

        private void BookingSlotPnl_MouseLeave(object sender, EventArgs e)
        {
            SetBackColour();
        }

        private void bookingLbl_MouseEnter(object sender, EventArgs e)
        {
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
    }
}
