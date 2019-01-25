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

    public partial class CorporateAccountFrm : Form
    {

        private BookingsFrm _bookingsFrm;
        private Booking _booking;
        private CorporateAccount _account;
        private List<ValidatingTextbox> _validators;
        private string _selectedBookingId;

        public CorporateAccountFrm(Booking booking, BookingsFrm bookingsFrm,
            CorporateAccount account)
        {
            InitializeComponent();

            Styles.SetFormStyles(this);
            _bookingsFrm = bookingsFrm;
            _booking = booking;
            _account = account;
            _selectedBookingId = _account.DefaultBookingId;
        }

        private void CorporateAccountFrm_Load(object sender, EventArgs e)
        {
            if (_account == null)
            {
                //NOT SUPPORTED
                bookingsFld.Text = "A booking hasn't yet been saved for this company.";
                copyContactBtn.Enabled = false;
                copyAddressBtn.Enabled = false;
                throw new Exception("A booking hasn't yet been saved for this company.");   //return;
            }

            //company group
            tradingNameFld.Text = _account.TradingName;
            businessNameFld.Text = _account.BusinessName;
            abnFld.Text = _account.Abn;

            //billing group
            contactNameFld.Text = _account.BillingContact;
            contactPrimaryNumFld.Text = _account.PhoneNumber;
            contactSecondaryNumFld.Text = _account.OtherNumbers;
            contactEmailFld.Text = _account.BillingEmail;
            addressFld.Text = _account.BillingAddress;

            //bookings group
            bookingsFld.Text = bookingsFld.Text.Replace("<X>", 
                _account.BookingIds.Count.ToString());
            specialFld.Text = _account.SpecialArrangements;
            notesFld.Text = _account.Notes;

            //bookings list
            pastBookingsLst.Items.Clear();
            foreach (var bid in _account.BookingIds)
            {
                var lvi = new ListViewItem(bid);
                lvi.Tag = bid;
                lvi.Checked = bid == _account.DefaultBookingId;
                pastBookingsLst.Items.Add(lvi);
            }

            //validators
            _validators = new List<ValidatingTextbox>();
            //_validators.Add(new ValidatingTextbox(this, businessNameFld, ValidatingTextbox.TextBoxValidationType.LongDatabase));
            //_validators.Add(new ValidatingTextbox(this, tradingNameFld, ValidatingTextbox.TextBoxValidationType.LongDatabase));
            //_validators.Add(new ValidatingTextbox(this, abnFld, ValidatingTextbox.TextBoxValidationType.NumbersAndSpaces));
            //_validators.Add(new ValidatingTextbox(this, contactNameFld, ValidatingTextbox.TextBoxValidationType.Name));
            //_validators.Add(new ValidatingTextbox(this, contactPrimaryNumFld, ValidatingTextbox.TextBoxValidationType.PhoneNumberAny));
            //_validators.Add(new ValidatingTextbox(this, contactSecondaryNumFld, ValidatingTextbox.TextBoxValidationType.PhoneNumberAny));
            _validators.Add(new ValidatingTextbox(this, contactEmailFld, ValidatingTextbox.TextBoxValidationType.EmailAddress));
           // _validators.Add(new ValidatingTextbox(this, addressFld, ValidatingTextbox.TextBoxValidationType.AddressBasic));

        }



        private void copyContactBtn_Click(object sender, EventArgs e)
        {
            contactNameFld.Text = _booking.Customer.FullName();
            contactEmailFld.Text = _booking.Customer.EmailAddress;
            contactPrimaryNumFld.Text = _booking.Customer.PrimaryNumber;
            contactSecondaryNumFld.Text = _booking.Customer.SecondaryNumber;
        }

        private void copyAddressBtn_Click(object sender, EventArgs e)
        {
            addressFld.Text = _booking.Address;
        }

        private void pastBookingsLst_ItemActivate(object sender, EventArgs e)
        {
            OpenBooking();
        }

        private void openBookingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenBooking();
        }

        private void OpenBooking()
        {
            if (pastBookingsLst.SelectedItems.Count == 0)
                return;

            var selected = pastBookingsLst.SelectedItems[0];
            _bookingsFrm.ShowBooking(BookingBL.GetBookingFull((string)selected.Tag),
                _booking.Id);
        }

        private void useAsDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pastBookingsLst.SelectedItems.Count == 0)
                return;

            pastBookingsLst.SelectedItems[0].Checked = true;
        }

        private void pastBookingsLst_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (!e.Item.Checked)
                return;

            foreach (var i in pastBookingsLst.CheckedItems)
            {
                var lvi = i as ListViewItem;
                if (lvi != e.Item)
                    lvi.Checked = false;
            }

        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                _account.TradingName = tradingNameFld.Text;
                _account.BusinessName = businessNameFld.Text;
                _account.Abn = abnFld.Text;

                _account.BillingContact = contactNameFld.Text;
                _account.BillingAddress = addressFld.Text;
                _account.PhoneNumber = contactPrimaryNumFld.Text;
                _account.OtherNumbers = contactSecondaryNumFld.Text;
                _account.BillingEmail = contactEmailFld.Text;

                _account.SpecialArrangements = specialFld.Text;
                _account.Notes = notesFld.Text;
                
                if (pastBookingsLst.CheckedItems.Count > 0)
                    _account.DefaultBookingId = (string)pastBookingsLst.CheckedItems[0].Tag;
                else
                    _account.DefaultBookingId = null;

                DBBox.AddOrUpdate(_account);

                Cursor = Cursors.Default;
                savedFld.Visible = true;
                savedTmr.Enabled = true;
                savedTmr.Start();
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                ErrorHandler.HandleError(this, "Failed to save Account", ex);
            }

            this.DialogResult = DialogResult.OK;
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void savedTmr_Tick(object sender, EventArgs e)
        {
            savedFld.Visible = false;
            savedTmr.Stop();
            savedTmr.Enabled = false;
        }

        private void ledgerBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var bookings = new List<Booking>();
                foreach (var bid in _account.BookingIds)
                {
                    bookings.Add(BookingBL.GetBookingFull(bid));
                }

                System.Diagnostics.Process.Start(
                    ReportBL.BookingsReport(bookings, _account.TradingName));
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                ErrorHandler.HandleError(this, "Failed to Print Corporate Account Ledger", ex);
            }

        }
    }
}
