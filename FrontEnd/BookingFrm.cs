using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TBRBooker.Business;
using TBRBooker.Model.Entities;

namespace TBRBooker.FrontEnd
{
    public partial class BookingFrm : Form
    {

        private Booking _booking;
        private bool _isNewEnquiry;
        private Timeline _timeline;

        public Booking GetBooking()
        {
            return _booking;
        }

        public BookingFrm(Booking booking)
        {
            InitializeComponent();

            if (booking == null)
            {
                _isNewEnquiry = true;
                _booking = new Booking();
            }
        }

        private void datePnl_Enter(object sender, EventArgs e)
        {

        }

        private void copyLastToNickBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(contactLastNameFld.Text))
                contactNicknameFld.Text = contactLastNameFld.Text.Trim();
        }

        private void BookingFrm_Load(object sender, EventArgs e)
        {
            //Contact
            if (_booking.Customer != null)
            {
                var customer = _booking.Customer;
                contactFirstNameFld.Text = customer.FirstName;
                contactLastNameFld.Text = customer.LastName;
                contactPrimaryNumFld.Text = customer.PrimaryNumber;
                contactSecondaryNumFld.Text = customer.SecondaryNumber;
                contactOtherNumFld.Text = customer.OtherNumbers;
                contactEmailFld.Text = customer.EmailAddress;
                contactNicknameFld.Text = _booking.BookingName;
                contactCompanyFld.Text = customer.CompanyName;
            }
            if (_booking.Account != null && _booking.IsOpen())
            {
                var account = _booking.Account;
                contactCompanyFld.Text = account.CompanyName;
                contactCompanyFld.Enabled = false;
            }


            //configure time
            timePick.CustomFormat = "HH:mm";
            timePick.Value = DateTime.Parse("1/1/2000 12:00:00 AM") + Booking.GetBookingTime(_booking.BookingTime);

            _timeline = new Timeline(_booking, new List<Booking>());
            dateGrp.Controls.Add(_timeline);
            _timeline.Location = new Point(13, 82);



        }

        private void timePick_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
                timePick.Value = timePick.Value.AddMinutes(-15);
            else if (e.KeyCode == Keys.Up)
                timePick.Value = timePick.Value.AddMinutes(15);
            e.Handled = true;
        }

    }
}
