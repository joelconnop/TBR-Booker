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
            timeLbl.Text = Booking.BookingTimeStr(_calItm.BookingTime);
            bookingLbl.Text = _calItm.BookingNum + " " + _calItm.BookingName;
            SetBackColour();
        }

        private void SetBackColour()
        {
            switch (_calItm.BookingStatus)
            {
                case Model.Enums.BookingStates.Cancelled:
                case Model.Enums.BookingStates.LostEnquiry:
                    BackColor = Color.Silver;
                    break;
                case Model.Enums.BookingStates.Completed:
                    BackColor = Color.DarkSeaGreen;
                    break;
                case Model.Enums.BookingStates.Booked:
                    BackColor = Color.LightSkyBlue;
                    break;
                case Model.Enums.BookingStates.OpenEnquiry:
                    BackColor = Color.Transparent;
                    break;
                case Model.Enums.BookingStates.PaymentDue:
                case Model.Enums.BookingStates.CancelledWithoutPayment:
                    BackColor = Color.LightPink;
                    break;
                default:
                    throw new Exception("Unknown back colour for " + _calItm.BookingStatus);
            }
        }

        private void BookingSlotPnl_MouseUp(object sender, MouseEventArgs e)
        {
            SetBackColour();
            _owner.ShowBooking(BookingBL.GetBookingFull(_calItm.BookingNum.ToString()));
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
            SetBackColour();
            _owner.ShowBooking(BookingBL.GetBookingFull(_calItm.BookingNum.ToString()));
        }

        private void bookingLbl_Click(object sender, EventArgs e)
        {
            SetBackColour();
            _owner.ShowBooking(BookingBL.GetBookingFull(_calItm.BookingNum.ToString()));
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
