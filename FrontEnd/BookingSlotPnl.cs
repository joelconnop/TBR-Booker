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
    public partial class BookingSlotPnl : UserControl
    {

        private CalendarItemDTO _calItm;
        
        public BookingSlotPnl(CalendarItemDTO calItm)
        {
            InitializeComponent();

            _calItm = calItm;
        }

        private void BookingSlotPnl_Load(object sender, EventArgs e)
        {
            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            timeLbl.Text = Booking.BookingTimeStr(_calItm.BookingTime);
            bookingLbl.Text = _calItm.BookingNum + " " + _calItm.BookingName;
            
            switch (_calItm.BookingStatus)
            {
                case Model.Enums.BookingStates.Cancelled:
                case Model.Enums.BookingStates.LostEnquiry:
                    BackColor = Color.LightGray;
                    break;
                case Model.Enums.BookingStates.Completed:
                    BackColor = Color.GreenYellow;
                    break;
                case Model.Enums.BookingStates.Booked:
                    BackColor = Color.LightSkyBlue;
                    break;
                case Model.Enums.BookingStates.OpenEnquiry:
                    BackColor = Color.Transparent;
                    break;
                case Model.Enums.BookingStates.PaymentDue:
                    BackColor = Color.LightPink;
                    break;
                default:
                    throw new Exception("Unknown back colour for " + _calItm.BookingStatus);
            }
        }
    }
}
