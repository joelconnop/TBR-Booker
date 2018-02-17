//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using TBRBooker.Business;
//using TBRBooker.Model.Entities;
//using TBRBooker.Model.Enums;

//namespace TBRBooker.FrontEnd
//{
//    public partial class BookingFrm : Form
//    {

//        private Booking _booking;
//        private Customer _customer;
//        private CorporateAccount _corporateAccount;
//        private bool _isNewEnquiry;
//        private BookingStates _newStatus;
//        private Timeline _timeline;
//        private bool _isSearchingCustomers;
//        private bool _isSearchModeForBookings;

//        public Booking GetBooking()
//        {
//            return _booking;
//        }

//        public BookingFrm(Booking booking)
//        {
//            InitializeComponent();

//            if (booking == null)
//            {
//                _isNewEnquiry = true;
//                _booking = new Booking();
//                _newStatus = BookingStates.OpenEnquiry;

//                //this breaks the rule that the DBBox handles the Id, but, we want a specially
//                //arranged Id that is presented to the user from the start
//                _booking.Id = BookingBL.GetNextBookingNum();
//                contactSearchChk.Checked = true;
//            }
//            else
//            {
//                _customer = booking.Customer;
//            }
//        }

//        private void datePnl_Enter(object sender, EventArgs e)
//        {

//        }

//        private void copyLastToNickBtn_Click(object sender, EventArgs e)
//        {
//            if (!string.IsNullOrEmpty(contactLastNameFld.Text))
//                contactNicknameFld.Text = contactLastNameFld.Text.Trim();
//        }

//        private void BookingFrm_Load(object sender, EventArgs e)
//        {
//            LoadAccount();  //load account first so customer can ovveride company field

//            LoadCustomer();

//            LoadTitlebar();
            
//            SetCustomerEmblem(GetPastBookings());

//            //configure time
//            timePick.CustomFormat = "HH:mm";
//            timePick.Value = DateTime.Parse("1/1/2000 12:00:00 AM") + Booking.GetBookingTime(_booking.BookingTime);

//            _timeline = new Timeline(_booking, new List<Booking>());
//            dateGrp.Controls.Add(_timeline);
//            _timeline.Location = new Point(13, 82);



//        }

//        private void LoadTitlebar()
//        {
//            titleLbl.Text = _booking.Id + " " + contactNicknameFld.Text;

//            switch (_newStatus)
//            {
//                case BookingStates.OpenEnquiry:
//                    statusLbl.Text = _isNewEnquiry ? "NEW ENQUIRY" : "OPEN ENQUIRY";
//                    statusLbl.ForeColor = Color.Gray;
//                    break;
//                case BookingStates.LostEnquiry:
//                    statusLbl.Text = "LOST ENQUIRY";
//                    statusLbl.ForeColor = Color.Orange;
//                    break;
//                case BookingStates.Booked:
//                    statusLbl.Text = "BOOKED";
//                    statusLbl.ForeColor = Color.Blue;
//                    break;
//                case BookingStates.Completed:
//                    statusLbl.Text = "COMPLETED";
//                    statusLbl.ForeColor = Color.Green;
//                    break;
//                case BookingStates.Cancelled:
//                    statusLbl.Text = "CANCELLED";
//                    statusLbl.ForeColor = Color.DarkOrange;
//                    break;
//                case BookingStates.PaymentDue:
//                    statusLbl.Text = "PAYMENT DUE";
//                    statusLbl.ForeColor = Color.DarkRed;
//                    break;
//            }
//        }

//        private void LoadCustomer()
//        {
//            //Contact
//            if (_customer != null)
//            {
//                var customer = _customer;
//                contactFirstNameFld.Text = customer.FirstName;
//                contactLastNameFld.Text = customer.LastName;
//                contactPrimaryNumFld.Text = customer.PrimaryNumber;
//                contactSecondaryNumFld.Text = customer.SecondaryNumber;
//                contactEmailFld.Text = customer.EmailAddress;
//                contactNicknameFld.Text = _booking.BookingName;
//                contactCompanyFld.Text = customer.CompanyName;
//            }
//        }

//        private void LoadAccount()
//        {
//            if (_corporateAccount != null)  // && _booking.IsOpen()) //<-- why?
//            {
//                var account = _corporateAccount;
//                contactCompanyFld.Text = account.CompanyName;
//                contactCompanyFld.Enabled = false;
//            }
//            else
//            {
//                contactCompanyFld.Text = "";
//                contactCompanyFld.Enabled = true;
//            }
//        }

//        private void SetCustomerEmblem(List<Booking> pastBookings)
//        {
//            int bookings = pastBookings.Count(x => x.IsBooked());
//            int didntBook = pastBookings.Count() - bookings;
//            int overdue = pastBookings.Count(x => x.Status == Model.Enums.BookingStates.PaymentDue);
            
//            if (overdue > 1 || overdue == 1 && bookings <= 2)
//                contactEmblemPic.Image = Properties.Resources.customer_overdue;
//            else if (bookings >= 2)
//                contactEmblemPic.Image = Properties.Resources.customer_gold;
//            else if (pastBookings.Count(x => x.IsBooked()) == 1)
//                contactEmblemPic.Image = Properties.Resources.customer_return;
//            else if (didntBook > 0)
//                contactEmblemPic.Image = Properties.Resources.customer_rebook;
//            else
//                contactEmblemPic.Image =  Properties.Resources.customer_new;

//            contactEmblemPic.Invalidate();  //redraws the picture
//        }

//        /// <summary>
//        /// Can call this all we like because only the first time will need to hit database
//        /// </summary>
//        /// <returns></returns>
//        private List<Booking> GetPastBookings()
//        {
//            var pastBookings = new List<Booking>();

//            if (_customer != null)
//            {
//                _customer.BookingIds.ForEach(x => pastBookings.Add(DBBox.ReadItem<Booking>(x)));
//            }

//            if (_corporateAccount != null)
//            {
//                _corporateAccount.BookingIds.ForEach(x => {
//                    if (!pastBookings.Any(y => y.Id == x))
//                        pastBookings.Add(DBBox.ReadItem<Booking>(x));
//                });
//            }

//            if (_booking != null)
//            {
//                var thisBooking = pastBookings.SingleOrDefault(x => x.Id == _booking.Id);
//                if (thisBooking != null)
//                    pastBookings.Remove(thisBooking);
//            }

//            return pastBookings;
//        }

//        private void timePick_KeyDown(object sender, KeyEventArgs e)
//        {
//            if (e.KeyCode == Keys.Down)
//                timePick.Value = timePick.Value.AddMinutes(-15);
//            else if (e.KeyCode == Keys.Up)
//                timePick.Value = timePick.Value.AddMinutes(15);
//            e.Handled = true;
//        }

//        private void SearchCustomers(string searchTerm)
//        {
//            if (searchTerm.Length < 3 || _isSearchingCustomers || !contactSearchChk.Checked)
//                return;

//            _isSearchingCustomers = true;

//            try
//            {

//                searchLst.Items.Clear();
//                if (searchPnl.Visible == false)
//                {
//                    searchPnl.Height = 180;
//                    searchPnl.Visible = true;
//                }

//                foreach (var match in CustomerBL.SearchCustomers(searchTerm))
//                {
//                    var itm = new ListViewItem(match.DirectoryName);
//                    itm.Tag = match.CustomerId;
//                    searchLst.Items.Add(itm);
//                }
//            }
//            catch (Exception ex)
//            {
//                ErrorHandler.HandleError(this, "Unexpected error searching past customers", ex);
//                contactSearchChk.Checked = false;   //disable searching for now                
//            }
//            finally
//            {
//                _isSearchingCustomers = false;
//            }
            
//        }

//        private void contactCloseBtn_Click(object sender, EventArgs e)
//        {
//            searchPnl.Visible = false;
//            contactSearchChk.Checked = false;
//        }

//        private void contactFirstNameFld_TextChanged(object sender, EventArgs e)
//        {
//            SearchCustomers(contactFirstNameFld.Text);
//        }

//        private void contactLastNameFld_TextChanged(object sender, EventArgs e)
//        {
//            SearchCustomers(contactFirstNameFld.Text);
//        }

//        private void contactNicknameFld_TextChanged(object sender, EventArgs e)
//        {
//            titleLbl.Text = contactNicknameFld.Text;
//            SearchCustomers(contactFirstNameFld.Text);
//        }

//        private void contactCompanyFld_TextChanged(object sender, EventArgs e)
//        {
//            SearchCustomers(contactFirstNameFld.Text);
//        }

//        private void contactPrimaryNumFld_TextChanged(object sender, EventArgs e)
//        {
//            SearchCustomers(contactFirstNameFld.Text);
//        }

//        private void contactSecondaryNumFld_TextChanged(object sender, EventArgs e)
//        {
//            SearchCustomers(contactFirstNameFld.Text);
//        }

//        private void contactEmailFld_TextChanged(object sender, EventArgs e)
//        {
//            SearchCustomers(contactFirstNameFld.Text);
//        }

//        private void contactResetBtn_Click(object sender, EventArgs e)
//        {
//            if (_customer == null)
//            {
//                var choice = MessageBox.Show("Are you sure you would like to clear all fields?",
//                                    "Reset Customer", MessageBoxButtons.YesNo);
//                switch (choice)
//                {
//                    case DialogResult.Yes:
//                        contactFirstNameFld.Text = "";
//                        contactLastNameFld.Text = "";
//                        contactNicknameFld.Text = "";
//                        contactCompanyFld.Text = "";
//                        contactPrimaryNumFld.Text = "";
//                        contactSecondaryNumFld.Text = "";
//                        contactEmailFld.Text = "";
//                        break;
//                    case DialogResult.No:
//                        return;
//                }
//            }

//            if (MessageBox.Show("This will un-link the current customer from this booking. Are you sure?",
//                "Reset Customer", MessageBoxButtons.OKCancel) == DialogResult.OK)
//            {
//                var choice = MessageBox.Show("Would you also like to clear all fields? (use 'no' if you want to duplicate the customer)",
//                    "Reset Customer", MessageBoxButtons.YesNoCancel);
//                switch (choice)
//                {
//                    case DialogResult.Yes:
//                        contactFirstNameFld.Text = "";
//                        contactLastNameFld.Text = "";
//                        contactNicknameFld.Text = "";
//                        contactCompanyFld.Text = "";
//                        contactPrimaryNumFld.Text = "";
//                        contactSecondaryNumFld.Text = "";
//                        contactEmailFld.Text = "";
//                        break;
//                    case DialogResult.No:
//                        break;
//                    case DialogResult.Cancel:
//                        return;
//                }

//                _customer = null;
//            }
//        }

//        private void searchLst_ItemActivated(object sender, EventArgs e)
//        {
//            if (_isSearchModeForBookings)
//                SelectBooking();
//            else
//                SelectCustomer();
//        }

//        private void SelectCustomer()
//        {
//            if (_customer != null && !_customer.IsNew())
//            {
//                //we could do this programatically but its potentially unintentional from the user. Make them push reset.
//                MessageBox.Show("Please 'reset' the current customer before selecting a different existing customer",
//                    "Returning Customer", MessageBoxButtons.OK);
//                return;
//            }

//            _customer = DBBox.ReadItem<Customer>((string)searchLst.SelectedItems[0].Tag);

//            var pastBookings = GetPastBookings();

//            var accountId = pastBookings.OrderByDescending(x => x.BookingDate)
//                .FirstOrDefault(x => !string.IsNullOrEmpty(x.AccountId))?.AccountId ?? null;
//            if (!string.IsNullOrEmpty(accountId))
//            {
//                _corporateAccount = DBBox.ReadItem<CorporateAccount>(accountId);
//            }
//            else
//            {
//                _corporateAccount = null;
//            }

//            LoadAccount();
//            LoadCustomer();
//            SetCustomerEmblem(pastBookings);

//            _isSearchModeForBookings = true;


//            searchPnl.Visible = false;
//            contactSearchChk.Checked = false;
//        }

//        private void SelectBooking()
//        {
//            var otherBookingFrm = new BookingFrm((Booking)searchLst.SelectedItems[0].Tag);
//            otherBookingFrm.Show();
//        }

//        private void ShowBookings(List<Booking> pastBookings)
//        {
//            Enabled = false;
//            Cursor = Cursors.WaitCursor;

//            try
//            {
//                searchLst.Items.Clear();
//                if (searchPnl.Visible == false)
//                {
//                    searchPnl.Height = 180;
//                    searchPnl.Visible = true;
//                }

//                foreach (var pastBooking in pastBookings)
//                {
//                    var itm = new ListViewItem(pastBooking.Summary());
//                    itm.Tag = pastBooking;
//                    searchLst.Items.Add(itm);
//                }
//            }
//            catch (Exception ex)
//            {
//                ErrorHandler.HandleError(this, "Unexpected error searching past customers", ex);
//                contactSearchChk.Checked = false;   //disable searching for now                
//            }
//            finally
//            {
//                Enabled = true;
//                Cursor = Cursors.Default;
//            }
//        }

//        private void statusLbl_Click(object sender, EventArgs e)
//        {
//            MessageBox.Show("Is it worth having a way to forcibly change the status?");
//        }

//        private void closeBtn_Click(object sender, EventArgs e)
//        {
//            if (!IsChangesMade() ||
//                MessageBox.Show(this, "Are you sure you want to close the form? There are unsaved changes that will be lost.",
//                "Unsaved Changes", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)
//                == DialogResult.OK)
//                Close();
//        }

//        private bool IsChangesMade()
//        {
//            if (_isNewEnquiry)
//                return true;

//            //boring! compare every field to last saved values!
//            return true;
//        }
//    }
//}
