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
using TBRBooker.Model.Enums;
using TBRBooker.Business;
using TBRBooker.Base;

namespace TBRBooker.FrontEnd
{
    public partial class BookingPnl : UserControl
    {

        BookingsFrm _owner;
        private Booking _booking;
        private Customer _customer;
        private CorporateAccount _corporateAccount;
        private Service _service;
        private bool _isLoading;
        private BookingStates _newStatus;
        public Timeline Timeline;
        private bool _isChangingTime;
        private bool _isSearchingCustomers;
        private bool _isSearchModeForBookings;
        private List<ValidatingTextbox> _validators;

        
        public Booking GetBooking()
        {
            return _booking;
        }

        public BookingPnl(Booking booking, BookingsFrm owner)
        {
            InitializeComponent();

            _owner = owner;
            _booking = booking;
            _newStatus = booking.Status;
            _isLoading = true;

            if (booking.IsNewBooking)
            {
                contactSearchChk.Checked = true;
            }
            else
            {
                _customer = booking.Customer;
            }
        }


        private void copyLastToNickBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(contactLastNameFld.Text))
                contactNicknameFld.Text = contactLastNameFld.Text.Trim();
            else if (!string.IsNullOrEmpty(contactFirstNameFld.Text))
                contactNicknameFld.Text = contactFirstNameFld.Text.Trim();
        }

        private void BookingPnl_Load(object sender, EventArgs e)
        {
            try
            {
                LoadEverything();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to fully load the booking", ex);
            }
        }

        private void LoadEverything()
        {
            LoadAccount();  //load account first so customer can ovveride company field

            LoadCustomer();

            LoadTitlebar();

            SetCustomerEmblem(GetPastBookings());

            //configure time
            _isChangingTime = true;
            datePick.Value = _booking.BookingDate;
            //timePick.CustomFormat = "h:mm tt";
            startPick.SetEventHandler(StartTimeChanged);
            SetTime(_booking.BookingTime, _booking.Duration);

            Timeline = new Timeline(_booking, _owner, this);
            dateGrp.Controls.Add(Timeline);
            Timeline.Location = new Point(13, 82);

            //address
            addressRegionBox.DataSource = Enum.GetValues(typeof(LocationRegions));
            addressRegionBox.SelectedIndex = (int)_booking.LocationRegion;
            addressFld.Text = _booking.Address;
            addressVenuFld.Text = _booking.VenueName;

            //service 
            _service = _booking.Service;
            serviceBox.DataSource = Enum.GetValues(typeof(ServiceTypes));
            serviceBox.SelectedIndex = (int)(_service?.ServiceType ?? ServiceTypes.NotSet);
            if (_service.ServiceType == ServiceTypes.ReptileParty)
            {
                var party = _service.Party;
                var package = party.Package;
                if (package == PartyPackages.NotSet)
                {
                    var partyPriceItem = _service.PriceItems.FirstOrDefault(
                        x => PriceItemsBL.WhichPartyPackage(x.ProductId) != PartyPackages.NotSet);
                    if (partyPriceItem != null)
                        package = PriceItemsBL.WhichPartyPackage(partyPriceItem.ProductId);
                }
            
                switch (package)
                {
                    case PartyPackages.Party:
                        partyStandardRdo.Checked = true;
                        break;
                    case PartyPackages.PartyPlus:
                        partyPlusRdo.Checked = true;
                        break;
                    case PartyPackages.PremiumParty:
                        partyPremiumRdo.Checked = true;
                        break;
                }
                partyBirthdayNameFld.Text = party.BirthdayName;
                partyAgeFld.Text = party.BirthdayAge.ToString();
            }
            serviceAddCrocChk.Checked = _service.AddCrocodile;
            servicePaxFld.Text = _service.Pax.ToString();
            serviceAnimalsToCome.Text = _service.SpecificAnimalsToCome;
            shortDemosChk.Checked = (_service.PriceItems
                .Any(x => x.ProductId == ProductIds.ShortDemonstrations));
            

            //pricing
            foreach (var item in _service.PriceItems)
            {
                //clone it so we don't have any impact until Save()
                AddPriceItem((PriceItem)item.Clone());
            }
            //if (_service.TotalPrice != _service.PriceItems.Sum(x => x.Total))
            //{
            //    priceOverrideChk.Checked = true;
            //    priceOverrideFld.Text = _service.TotalPrice.ToString();
            //}

            var skus = new List<ProductIds>();
            foreach (ProductIds sku in Enum.GetValues(typeof(ProductIds)))
            {
                if (!PriceItemsBL.IsAService(sku))
                    skus.Add(sku);
            }
            priceProductBox.DataSource = skus.ToArray<ProductIds>();

            //payments
            pricePaidLbl.Text = _booking.AmountPaid.ToString("C");
            priceInvoiceBtn.Enabled = !_booking.IsInvoiced;
            pricingPayOnDayChk.Checked = _booking.IsPayOnDay;
            priceMethodFld.DataSource = Enum.GetValues(typeof(PaymentMethods));
            _booking.PaymentHistory.ForEach(x => priceHistoryLst.Items.Add(x.ToString()));

            notesBookingFld.Text = _booking.BookingNotes;

            completeBtn.Enabled = new[] { BookingStates.Booked, BookingStates.PaymentDue }
            .Contains(_newStatus);

            //validators
            _validators = new List<ValidatingTextbox>();
            _validators.Add(new ValidatingTextbox(this, contactFirstNameFld, ValidatingTextbox.TextBoxValidationType.Name));
            _validators.Add(new ValidatingTextbox(this, contactLastNameFld, ValidatingTextbox.TextBoxValidationType.Name));
            _validators.Add(new ValidatingTextbox(this, contactNicknameFld, ValidatingTextbox.TextBoxValidationType.Name));
            _validators.Add(new ValidatingTextbox(this, contactCompanyFld, ValidatingTextbox.TextBoxValidationType.LongDatabase));
            _validators.Add(new ValidatingTextbox(this, contactPrimaryNumFld, ValidatingTextbox.TextBoxValidationType.PhoneNumberAny));
            _validators.Add(new ValidatingTextbox(this, contactSecondaryNumFld, ValidatingTextbox.TextBoxValidationType.PhoneNumberAny));
            _validators.Add(new ValidatingTextbox(this, contactEmailFld, ValidatingTextbox.TextBoxValidationType.EmailAddress));

            _validators.Add(new ValidatingTextbox(this, durationFld, ValidatingTextbox.TextBoxValidationType.IntegerZeroPlus));

            _validators.Add(new ValidatingTextbox(this, addressFld, ValidatingTextbox.TextBoxValidationType.AddressBasic));
            _validators.Add(new ValidatingTextbox(this, addressVenuFld, ValidatingTextbox.TextBoxValidationType.LongDatabase));

            _validators.Add(new ValidatingTextbox(this, servicePaxFld, ValidatingTextbox.TextBoxValidationType.IntegerZeroPlus));
            _validators.Add(new ValidatingTextbox(this, partyBirthdayNameFld, ValidatingTextbox.TextBoxValidationType.Name));
            _validators.Add(new ValidatingTextbox(this, partyAgeFld, ValidatingTextbox.TextBoxValidationType.IntegerZeroPlus));

            _validators.Add(new ValidatingTextbox(this, priceDescFld, ValidatingTextbox.TextBoxValidationType.LongDatabase));
            _validators.Add(new ValidatingTextbox(this, priceAmtFld, ValidatingTextbox.TextBoxValidationType.DollarAmountAny));
            _validators.Add(new ValidatingTextbox(this, priceQtyFld, ValidatingTextbox.TextBoxValidationType.IntegerPositive));
            _validators.Add(new ValidatingTextbox(this, priceOverrideFld, ValidatingTextbox.TextBoxValidationType.DollarAmountAny));
            _validators.Add(new ValidatingTextbox(this, priceDepositFld, ValidatingTextbox.TextBoxValidationType.DollarAmountAny));
            _validators.Add(new ValidatingTextbox(this, priceSubmitFld, ValidatingTextbox.TextBoxValidationType.DollarAmountAny));

            _isLoading = false;
        }

        private void LoadTitlebar()
        {
            titleLbl.Text = _booking.Id + " " + contactNicknameFld.Text;

            switch (_newStatus)
            {
                case BookingStates.OpenEnquiry:
                    statusLbl.Text = _booking.IsNewBooking ? "NEW ENQUIRY" : "OPEN ENQUIRY";
                    statusLbl.ForeColor = Color.Gray;
                    break;
                case BookingStates.LostEnquiry:
                    statusLbl.Text = "LOST ENQUIRY";
                    statusLbl.ForeColor = Color.Orange;
                    break;
                case BookingStates.Booked:
                    statusLbl.Text = "BOOKED";
                    statusLbl.ForeColor = Color.Blue;
                    break;
                case BookingStates.Completed:
                    statusLbl.Text = "COMPLETED";
                    statusLbl.ForeColor = Color.Green;
                    break;
                case BookingStates.Cancelled:
                    statusLbl.Text = "CANCELLED";
                    statusLbl.ForeColor = Color.DarkOrange;
                    break;
                case BookingStates.PaymentDue:
                    statusLbl.Text = "PAYMENT DUE";
                    statusLbl.ForeColor = Color.DarkRed;
                    break;
                case BookingStates.CancelledWithoutPayment:
                    statusLbl.Text = "NEVER PAID!";
                    statusLbl.ForeColor = Color.OrangeRed;
                    break;
            }
        }

        private void LoadCustomer()
        {
            //Contact
            contactLeadBox.DataSource = Enum.GetValues(typeof(LeadSources));
            if (_customer != null)
            {
                var customer = _customer;
                contactFirstNameFld.Text = customer.FirstName;
                contactLastNameFld.Text = customer.LastName;
                contactPrimaryNumFld.Text = customer.PrimaryNumber;
                contactSecondaryNumFld.Text = customer.SecondaryNumber;
                contactEmailFld.Text = customer.EmailAddress;
                contactNicknameFld.Text = _booking.BookingName;
                contactCompanyFld.Text = customer.CompanyName;
                contactLeadBox.SelectedItem = customer.LeadSource;
                notesPastFld.Text = _customer.PastNotes;
            }
        }

        private void LoadAccount()
        {
            if (_corporateAccount != null)  // && _booking.IsOpen()) //<-- why?
            {
                contactCompanyFld.Text = _corporateAccount.CompanyName;
                contactCompanyFld.Enabled = false;
            }
            else
            {
                contactCompanyFld.Text = "";
                contactCompanyFld.Enabled = true;
            }
        }

        private void SetCustomerEmblem(List<Booking> pastBookings)
        {
            int bookings = pastBookings.Count(x => x.IsBooked());
            int didntBook = pastBookings.Count() - bookings;
            int overdue = pastBookings.Count(x => x.Status == BookingStates.PaymentDue 
                || x.Status == BookingStates.CancelledWithoutPayment);

            if (overdue > 1 || overdue == 1 && bookings <= 2)
                contactEmblemPic.Image = Properties.Resources.customer_overdue;
            else if (bookings >= 2)
                contactEmblemPic.Image = Properties.Resources.customer_gold;
            else if (pastBookings.Count(x => x.IsBooked()) == 1)
                contactEmblemPic.Image = Properties.Resources.customer_return;
            else if (didntBook > 0)
                contactEmblemPic.Image = Properties.Resources.customer_rebook;
            else
                contactEmblemPic.Image = Properties.Resources.customer_new;

            contactEmblemPic.Invalidate();  //redraws the picture
        }

        /// <summary>
        /// Can call this all we like because only the first time will need to hit database
        /// </summary>
        /// <returns></returns>
        private List<Booking> GetPastBookings()
        {
            var pastBookings = new List<Booking>();

            if (_customer != null)
            {
                _customer.BookingIds.ForEach(x => pastBookings.Add(DBBox.ReadItem<Booking>(x)));
            }

            if (_corporateAccount != null)
            {
                _corporateAccount.BookingIds.ForEach(x => {
                    if (!pastBookings.Any(y => y.Id == x))
                        pastBookings.Add(DBBox.ReadItem<Booking>(x));
                });
            }

            if (_booking != null)
            {
                var thisBooking = pastBookings.SingleOrDefault(x => x.Id == _booking.Id);
                if (thisBooking != null)
                    pastBookings.Remove(thisBooking);
            }

            return pastBookings;
        }

        public void StartTimeChanged(TimePicker.TimePickerValue tpv)
        {
            try
            {
                if (_isChangingTime)
                    return;

                if (Timeline != null)
                {
                    Timeline.Time = tpv.Value;
                    Timeline.Redraw();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to update the timeline", ex);
            }
        }

        //private void timePick_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    { 
        //    if (e.KeyCode == Keys.Down)
        //        timePick.Value = timePick.Value.AddMinutes(-15);
        //    else if (e.KeyCode == Keys.Up)
        //        timePick.Value = timePick.Value.AddMinutes(15);
        //    e.Handled = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.HandleError(this, "Failed to change the time", ex);
        //    }
        //}

        //private void timePick_ValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (_isChangingTime)
        //            return;

        //        if (Timeline != null)
        //        {
        //            Timeline.Time = Utils.TimeInt(timePick.Value);
        //            Timeline.Redraw();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.HandleError(this, "Failed to update the timeline", ex);
        //    }
        //}

        private void durationFld_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (_isChangingTime)
                    return;

                int duration = 0;
                if (int.TryParse(durationFld.Text.Trim(), out duration))
                {
                    Timeline.Duration = duration;
                    Timeline.Redraw();
                    durationDescFld.Text = "(" + Utils.DurationToDisplayStr(duration) + ")";
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to update the timeline", ex);
            }
        }

        private void datePick_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (_isChangingTime)
                    return;

                if (Timeline != null)
                {
                    Timeline.BookingDate = datePick.Value;
                    Timeline.Redraw();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to update the timeline", ex);
            }
        }

        public void SetTime(int time, int duration)
        {
            _isChangingTime = true;
            //// var parsed = Utils.ParseTime(time);
            ////timePick.Value = new DateTime(2000, 1, 1, parsed.Hour, parsed.Minute, 0, 0);
            //timePick.Value = DateTime.Parse("1/1/2000 12:00:00 AM") 
            //    + Booking.GetBookingTime(time);

            startPick.InitTimes(600, 1900, 15, time);

            durationFld.Text = duration.ToString();
            durationDescFld.Text = "(" + Utils.DurationToDisplayStr(duration) + ")";
            _isChangingTime = false;
        }



        private void SearchCustomers(string searchTerm)
        {
            if (searchTerm.Length < 3 || _isSearchingCustomers || !contactSearchChk.Checked)
                return;

            _isSearchingCustomers = true;

            try
            {

                searchLst.Items.Clear();
                if (searchPnl.Visible == false)
                {
                    searchPnl.Height = 180;
                    searchPnl.Visible = true;
                }

                foreach (var match in CustomerBL.SearchCustomers(searchTerm))
                {
                    var itm = new ListViewItem(match.DirectoryName);
                    itm.Tag = match.CustomerId;
                    searchLst.Items.Add(itm);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Unexpected error searching past customers", ex);
                contactSearchChk.Checked = false;   //disable searching for now                
            }
            finally
            {
                _isSearchingCustomers = false;
            }

        }

        private void contactCloseBtn_Click(object sender, EventArgs e)
        {
            searchPnl.Visible = false;
            contactSearchChk.Checked = false;
        }

        private void contactFirstNameFld_TextChanged(object sender, EventArgs e)
        {
            //first name is a vague thing to search on, but try including it until required otherwise
            SearchCustomers(contactFirstNameFld.Text);
        }

        private void contactLastNameFld_TextChanged(object sender, EventArgs e)
        {
            SearchCustomers(contactLastNameFld.Text);
        }

        private void contactNicknameFld_TextChanged(object sender, EventArgs e)
        {
            titleLbl.Text = contactNicknameFld.Text;
            SearchCustomers(contactNicknameFld.Text);
        }

        private void contactCompanyFld_TextChanged(object sender, EventArgs e)
        {
            SearchCustomers(contactCompanyFld.Text);
        }

        private void contactPrimaryNumFld_TextChanged(object sender, EventArgs e)
        {
            SearchCustomers(contactPrimaryNumFld.Text);
        }

        private void contactSecondaryNumFld_TextChanged(object sender, EventArgs e)
        {
            SearchCustomers(contactSecondaryNumFld.Text);
        }

        private void contactEmailFld_TextChanged(object sender, EventArgs e)
        {
            SearchCustomers(contactEmailFld.Text);
        }

        private void contactResetBtn_Click(object sender, EventArgs e)
        {
            if (_customer == null)
            {
                var choice = MessageBox.Show("Are you sure you would like to clear all fields?",
                                    "Reset Customer", MessageBoxButtons.YesNo);
                switch (choice)
                {
                    case DialogResult.Yes:
                        contactFirstNameFld.Text = "";
                        contactLastNameFld.Text = "";
                        contactNicknameFld.Text = "";
                        contactCompanyFld.Text = "";
                        contactPrimaryNumFld.Text = "";
                        contactSecondaryNumFld.Text = "";
                        contactEmailFld.Text = "";
                        break;
                    case DialogResult.No:
                        return;
                }
            }

            if (MessageBox.Show("This will un-link the current customer from this booking. Are you sure?",
                "Reset Customer", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                var choice = MessageBox.Show("Would you also like to clear all fields? (use 'no' if you want to duplicate the customer)",
                    "Reset Customer", MessageBoxButtons.YesNoCancel);
                switch (choice)
                {
                    case DialogResult.Yes:
                        contactFirstNameFld.Text = "";
                        contactLastNameFld.Text = "";
                        contactNicknameFld.Text = "";
                        contactCompanyFld.Text = "";
                        contactPrimaryNumFld.Text = "";
                        contactSecondaryNumFld.Text = "";
                        contactEmailFld.Text = "";
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        return;
                }

                _customer = null;
            }
        }

        private void searchLst_ItemActivated(object sender, EventArgs e)
        {
            if (_isSearchModeForBookings)
                SelectBooking();
            else
                SelectCustomer();
        }

        private void SelectCustomer()
        {
            try
            {
                if (_customer != null && !_customer.IsNew())
                {
                    //we could do this programatically but its potentially unintentional from the user. Make them push reset.
                    MessageBox.Show("Please 'reset' the current customer before selecting a different existing customer",
                        "Returning Customer", MessageBoxButtons.OK);
                    return;
                }

                _customer = DBBox.ReadItem<Customer>((string)searchLst.SelectedItems[0].Tag);

                var pastBookings = GetPastBookings();

                var accountId = pastBookings.OrderByDescending(x => x.BookingDate)
                    .FirstOrDefault(x => !string.IsNullOrEmpty(x.AccountId))?.AccountId ?? null;
                if (!string.IsNullOrEmpty(accountId))
                {
                    _corporateAccount = DBBox.ReadItem<CorporateAccount>(accountId);
                }
                else
                {
                    _corporateAccount = null;
                }

                LoadAccount();
                LoadCustomer();
                SetCustomerEmblem(pastBookings);

                //this flag is kind of dangerous as it determines different types to put on LVI tags. Planning to make booking search a separate window
                //_isSearchModeForBookings = true;


                searchPnl.Visible = false;
                contactSearchChk.Checked = false;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to select customer", ex);
            }
        }

        private void SelectBooking()
        {
            _owner.ShowBooking((Booking)searchLst.SelectedItems[0].Tag);
        }

        //private void ShowBookings(List<Booking> pastBookings)
        //{
        //    Enabled = false;
        //    Cursor = Cursors.WaitCursor;

        //    try
        //    {
        //        searchLst.Items.Clear();
        //        if (searchPnl.Visible == false)
        //        {
        //            searchPnl.Height = 180;
        //            searchPnl.Visible = true;
        //        }

        //        foreach (var pastBooking in pastBookings)
        //        {
        //            var itm = new ListViewItem(pastBooking.Summary());
        //            itm.Tag = pastBooking;
        //            searchLst.Items.Add(itm);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorHandler.HandleError(this, "Failed to search past customers", ex, true);
        //        contactSearchChk.Checked = false;   //disable searching for now                
        //    }
        //    finally
        //    {
        //        Enabled = true;
        //        Cursor = Cursors.Default;
        //    }
        //}

        private void statusLbl_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Is it worth having a way to forcibly change the status?");
        }

        private void priceAddBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (priceProductBox.SelectedItem == null || string.IsNullOrEmpty(priceDescFld.Text)
                    || string.IsNullOrEmpty(priceQtyFld.Text))
                    MessageBox.Show(this, "You must fill in all fields for the Price Item.",
                    "Add/Update Price Item", MessageBoxButtons.OK, MessageBoxIcon.Hand);

                AddPriceItem(new PriceItem((ProductIds)priceProductBox.SelectedItem,
                    decimal.Parse(priceAmtFld.Text), int.Parse(priceQtyFld.Text)));

                _isLoading = true;
                priceProductBox.SelectedItem = null;
                priceDescFld.Text = priceAmtFld.Text = priceQtyFld.Text = string.Empty;
                _isLoading = false;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to add the price item", ex);
            }
        }

        private void priceOverrideChk_CheckedChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("Don't think we should have this because then the items on invoice won't add up to total. Instead, override price of individual items.");
            priceOverrideFld.Enabled = priceOverrideChk.Checked;
        }

        private void priceInvoiceBtn_Click(object sender, EventArgs e)
        {
            //just load QB URL for now
            //set invoiced to true and save
            MessageBox.Show("Later phase. For now, create manually in Quickbooks.");
        }

        private void priceSubmitBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Submitting a payment will save everything on the form and (eventually) send to Quickbooks on the spot. Continue?",
               "Add Payment", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                != DialogResult.OK)
                return;

            try
            {
                decimal paymentAmount;
                if (string.IsNullOrEmpty(priceSubmitFld.Text) 
                    || !decimal.TryParse(priceSubmitFld.Text, out paymentAmount)
                    || paymentAmount == 0 || (PaymentMethods)priceMethodFld.SelectedItem == PaymentMethods.NotSet)
                {
                    MessageBox.Show(this, "You missed a payment field (or two)",
                        "Add Payment", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }

                //quickbooks stuff!
                //use wait cursor
                if (paymentAmount < 0)
                {

                }
                var payment = new Payment(DateTime.Now, paymentAmount,
                    (PaymentMethods)priceMethodFld.SelectedItem);
                _booking.PaymentHistory.Add(payment);

                Save();   //this is only way that payment gets saved into history, and will need to do once quickbooks done anyway
                pricePaidLbl.Text = _booking.AmountPaid.ToString("C");
                priceHistoryLst.Items.Add(payment.ToString());

            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to save submit payment", ex);
            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Save();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to save Booking", ex);
            }
        }

        public string UnsavedChanges()
        {
            string unsavedChanges = "";

            if (_booking.IsNewBooking)
            {
                return "New Enquiry " + _booking.Id;
            }

            if (_newStatus != _booking.Status)
                unsavedChanges += "status, ";
            if (!contactFirstNameFld.Text.Equals(_booking.Customer.FirstName))
                unsavedChanges += "first name, ";
            if (!contactLastNameFld.Text.Equals(_booking.Customer.LastName))
                unsavedChanges += "last name, ";
            if (!contactNicknameFld.Text.Equals(_booking.BookingName) && !contactNicknameFld.Text.Equals(string.Empty))
                unsavedChanges += "booking nickname, ";
            if (!contactPrimaryNumFld.Text.Equals(_booking.Customer.PrimaryNumber))
                unsavedChanges += "contact number, ";
            if (!contactSecondaryNumFld.Text.Equals(_booking.Customer.SecondaryNumber))
                unsavedChanges += "secondary number, ";
            if (!contactEmailFld.Text.Equals(_booking.Customer.EmailAddress))
                unsavedChanges += "e-mail, ";
            if ((LeadSources)contactLeadBox.SelectedItem != _booking.Customer.LeadSource)
                unsavedChanges += "lead source, ";

            if (!datePick.Value.ToShortDateString().Equals(_booking.BookingDate.ToShortDateString()))
                unsavedChanges += "booking date, ";
            if (startPick.GetSelected().Value != _booking.BookingTime)
                unsavedChanges += "booking time, ";
            if (!durationFld.Text.Equals(_booking.Duration.ToString()))
                unsavedChanges += "duration, ";

            if ((LocationRegions)addressRegionBox.SelectedItem != _booking.LocationRegion)
                unsavedChanges += "region, ";
            if (!addressFld.Text.Equals(_booking.Address))
                unsavedChanges += "e-mail, ";
            if (!addressVenuFld.Text.Equals(_booking.VenueName))
                unsavedChanges += "venue, ";

            if (//_booking.Service != null && at the least it should be NotSet, for an existing booking
                (ServiceTypes)serviceBox.SelectedItem != _booking.Service.ServiceType)
            {
                unsavedChanges += "service type, ";
            }
            else if (_booking.Service.ServiceType == ServiceTypes.ReptileParty)
            {
                var party = _booking.Service.Party;
                if (SelectedPartyPackage() != party.Package)
                    unsavedChanges += "party package, ";
                if (!partyBirthdayNameFld.Text.Equals(party.BirthdayName))
                    unsavedChanges += "birthday name, ";
                if (!partyAgeFld.Text.Equals(party.BirthdayAge.ToString())
                    && !(string.IsNullOrEmpty(partyAgeFld.Text) && party.BirthdayAge == 0))
                    unsavedChanges += "birthday age, ";
            }

            if (!servicePaxFld.Text.Equals(_booking.Service.Pax.ToString())
                && !(string.IsNullOrEmpty(servicePaxFld.Text) && _booking.Service.Pax == 0))
                unsavedChanges += "pax, ";
            if (!serviceAnimalsToCome.Text.Equals(_booking.Service.SpecificAnimalsToCome))
                unsavedChanges += "animals to come, ";
            if (serviceAddCrocChk.Checked != _booking.Service.AddCrocodile)
                unsavedChanges += "add croc, ";


            int pricingItemCnt = priceItemsLst.Items.Count;
            if (pricingItemCnt != _booking.Service.PriceItems.Count)
                unsavedChanges += "number pricing items, ";
            else
            {
                for (int i = 0; i < pricingItemCnt; i++)
                {
                    var itm1 = (PriceItem)priceItemsLst.Items[i].Tag;
                    var itm2 = _booking.Service.PriceItems[i];
                    if (!itm1.Equals(itm2))
                        unsavedChanges += itm1.Description + ", ";
                }
            }
            
            if (priceOverrideChk.Checked && Convert.ToDecimal(priceOverrideFld.Text)
                != _booking.Service.TotalPrice)
                unsavedChanges += "overidden total price, ";

            if (pricingPayOnDayChk.Checked != _booking.IsPayOnDay)
                unsavedChanges += "pay on day, ";

            if (!notesBookingFld.Text.Equals(_booking.BookingNotes))
                unsavedChanges += "booking notes, ";
            if (!notesPastFld.Text.Equals(_booking.Customer.PastNotes))
                unsavedChanges += "past notes, ";

            return unsavedChanges.Trim().Trim(',');
        }

        public bool Save()
        {
            Cursor = Cursors.WaitCursor;

            try
            {
                _booking.Status = _newStatus;

                if (_customer == null)
                    _customer = new Customer();

                _customer.FirstName = contactFirstNameFld.Text;
                _customer.LastName = contactLastNameFld.Text;
                _customer.PrimaryNumber = contactPrimaryNumFld.Text;
                _customer.SecondaryNumber = contactSecondaryNumFld.Text;
                _customer.CompanyName = contactCompanyFld.Text;
                _customer.CompanyId = _corporateAccount?.Id ?? null;
                _customer.EmailAddress = contactEmailFld.Text;
                _customer.LeadSource = (LeadSources)contactLeadBox.SelectedItem;

                //always set bookingNickname, even if empty (will copy from customer during save), and even if same as customer name
                _booking.BookingNickname = contactNicknameFld.Text;
                _booking.BookingDate = datePick.Value;
                _booking.BookingTime = startPick.GetSelected().Value;
                _booking.Duration = int.Parse(durationFld.Text);

                _booking.LocationRegion = (LocationRegions)addressRegionBox.SelectedValue;
                _booking.Address = addressFld.Text;
                _booking.VenueName = addressVenuFld.Text;

                _booking.Service = _service;
                var serviceType = (ServiceTypes)serviceBox.SelectedItem;
                _service = new Service()
                {
                    ServiceType = serviceType,
                };
                if (serviceType == ServiceTypes.ReptileParty)
                {
                    int age = 0;
                    int.TryParse(partyAgeFld.Text, out age);
                    _service.Party = new Party()
                    {
                        Package = SelectedPartyPackage(),
                        BirthdayName = partyBirthdayNameFld.Text,
                        BirthdayAge = age
                    };
                }

                int pax = 0;
                int.TryParse(servicePaxFld.Text, out pax);
                _service.Pax = pax;
                _service.AddCrocodile = serviceAddCrocChk.Checked;
                _service.SpecificAnimalsToCome = serviceAnimalsToCome.Text;

                _service.PriceItems = new List<PriceItem>();
                foreach (ListViewItem lvi in priceItemsLst.Items)
                {
                    _service.PriceItems.Add((PriceItem)lvi.Tag);
                }
                _booking.IsPayOnDay = pricingPayOnDayChk.Checked;
                //if (priceOverrideChk.Checked && !string.IsNullOrEmpty(priceOverrideFld.Text))
                //{
                //    _service.TotalPrice = decimal.Parse(priceOverrideFld.Text);
                //}
                //else
                //{
                //    _service.TotalPrice = decimal.Parse(priceCalculatedFld.Text.Trim('$'));
                //}

                _booking.BookingNotes = notesBookingFld.Text;
                _customer.PastNotes = notesPastFld.Text;

                //don't need to save payment history as it is saved when payment added

                _booking.Service = _service;
                _booking.Customer = _customer;
                _booking.Account = _corporateAccount;

                var errors = _booking.ValidationErrors();
                if (!string.IsNullOrEmpty(errors))
                {
                    Cursor = Cursors.Default;
                    MessageBox.Show(this, errors, "Save Booking",
                        MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return false;
                }

                BookingBL.SaveBookingEtc(_booking);
                _owner.OnBookingSave();
                Cursor = Cursors.Default;

                savedFld.Visible = true;
                savedTmr.Enabled = true;
                savedTmr.Start();

                return true;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                ErrorHandler.HandleError(this, "Failed to save Booking", ex);
                return false;
            }
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var unsavedChanges = UnsavedChanges();
                if (string.IsNullOrEmpty(unsavedChanges) ||
                    MessageBox.Show(this,
                    "Are you sure you want to close the form? There are unsaved changes that will be lost: "
                     + unsavedChanges,
                    "Unsaved Changes", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)
                    == DialogResult.OK)
                {
                    _owner.CloseBooking(_booking.Id);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to check for unsaved changes", ex);
                _owner.CloseBooking(_booking.Id);
            }

        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            bool isConfirmed = false;

            switch (_newStatus)
            {
                case BookingStates.LostEnquiry:
                    if (MessageBox.Show("Are you sure you want to re-open this enquiry?",
                        "Re-open Enquiry", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
                    {
                        _newStatus = BookingStates.OpenEnquiry;
                        bookBtn.Enabled = true;
                        completeBtn.Enabled = false;
                        isConfirmed = true;
                    }
                    break;
                case BookingStates.Cancelled:
                case BookingStates.CancelledWithoutPayment:
                    if (MessageBox.Show("Are you sure you want to restore this booking?",
                        "Restore Booking", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
                    {
                        //ideally should check for overdue payment etc but that shouldn't happen
                        _newStatus = BookingStates.Booked;
                        isConfirmed = true;
                        completeBtn.Enabled = true;
                        bookBtn.Enabled = false;
                    }
                    break;
                case BookingStates.OpenEnquiry:
                    if (MessageBox.Show("Are you sure you want to dismiss this enquiry?",
                        "Dismiss Booking", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
                    {
                        //ideally should check for overdue payment etc but that shouldn't happen
                        _newStatus = BookingStates.LostEnquiry;
                        isConfirmed = true;
                    }
                    break;
                case BookingStates.Booked:
                    if (MessageBox.Show("Are you sure you want to cancel this booking?",
                        "Dismiss Booking", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
                    {
                        //ideally should check for overdue payment etc but that shouldn't happen
                        _newStatus = BookingStates.Cancelled;
                        isConfirmed = true;
                        completeBtn.Enabled = false;
                    }
                    break;
                case BookingStates.Completed:
                case BookingStates.PaymentDue:
                    if (MessageBox.Show("Are you sure you want to cancel this booking? WARNING: Booking has already been marked as completed!",
                        "Dismiss Booking", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation)
                        == DialogResult.Yes)
                    {
                        //ideally should check for overdue payment etc but that shouldn't happen
                        _newStatus = BookingStates.Cancelled;
                        isConfirmed = true;
                        completeBtn.Enabled = false;
                        bookBtn.Enabled = true;
                    }
                    break;
                default:
                    ErrorHandler.HandleError(this, "Cannot cancel/restore",
                        new Exception("Unrecognised status: " + _newStatus), true);
                    break;
            }

            if (isConfirmed)
            {
                Save();
                LoadTitlebar();
            }

        }

        private void bookBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var backupStatus = _newStatus;
                _newStatus = BookingStates.Booked;
                if (Save())
                {
                    bookBtn.Enabled = false;
                    completeBtn.Enabled = true;
                    cancelBtn.Enabled = true;
                    LoadTitlebar();
                }
                else
                {
                    _newStatus = backupStatus;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to Book it", ex);
            }
        }

        private void printBtn_Click(object sender, EventArgs e)
        {
            if (_booking.IsNewBooking)
            {
                MessageBox.Show(this, "You need to save the new booking before printing a form",
                    "Print Booking Form", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            try
            {
                var unsavedChanges = UnsavedChanges();
                if (!string.IsNullOrEmpty(unsavedChanges))
                {
                    switch (MessageBox.Show(this,
                        "Would you like to save the following changes so that they will be included on the Booking Form? "
                        + unsavedChanges, "Print Booking Form",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                    {
                        case DialogResult.Yes:
                            Save();
                            break;
                        case DialogResult.No:
                            //just continue
                            break;
                        default:
                            return;
                    }
                }

                //just open in chrome, let user decide whether to print now or just view
                System.Diagnostics.Process.Start(BookingBL.GenerateBookingFormFilename(_booking));
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Cannot Print Booking Form", ex, true);
            }
            

        }

        private void addressRegionFld_SelectedIndexChanged(object sender, EventArgs e)
        {
            TryAddBasePriceItem();
        }


        private void serviceBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            partyPnl.Visible = false;
            displayPnl.Visible = false;
            switch ((ServiceTypes)serviceBox.SelectedItem)
            {
                case ServiceTypes.ReptileParty:
                    partyPnl.Visible = true;
                    break;
                case ServiceTypes.Display:
                    displayPnl.Visible = true;
                    break;
                default:
                    TryAddBasePriceItem();
                    break;
            }
            
        }

        private void partyStandardRdo_CheckedChanged(object sender, EventArgs e)
        {
            TryAddBasePriceItem();
        }

        private void partyPlusRdo_CheckedChanged(object sender, EventArgs e)
        {
            TryAddBasePriceItem();
        }

        private void partyPremiumRdo_CheckedChanged(object sender, EventArgs e)
        {
            TryAddBasePriceItem();
        }

        private void TryAddBasePriceItem()
        {
            try
            {
                if (_isLoading)
                    return;

                //remove the old service
                ListViewItem serviceLvi = null;
                foreach (ListViewItem lvi in priceItemsLst.Items)
                {
                    if (PriceItemsBL.IsAService(((PriceItem)lvi.Tag).ProductId))
                    {
                        serviceLvi = lvi;
                        break;
                    }
                }
                if (serviceLvi != null)
                {
                    priceItemsLst.Items.Remove(serviceLvi);
                    UpdateTotal();
                }

                if ((LocationRegions)addressRegionBox.SelectedItem == LocationRegions.NotSet
                    || (ServiceTypes)serviceBox.SelectedItem == ServiceTypes.NotSet
                    || ((ServiceTypes)serviceBox.SelectedItem == ServiceTypes.ReptileParty
                        && SelectedPartyPackage() == PartyPackages.NotSet))
                    return;

                var serviceType = (ServiceTypes)serviceBox.SelectedItem;
                var item = PriceItemsBL.GetBaseItem((LocationRegions)addressRegionBox.SelectedItem,
                    serviceType, SelectedPartyPackage());
                AddPriceItem(item);

            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to add base item", ex);
            }
        }

        private void AddPriceItem(PriceItem item)
        {
            var lvi = new ListViewItem(
                new[] { item.Description, item.UnitPrice.ToString(),
                    item.Quantity.ToString(), item.Total.ToString() });
            lvi.Tag = item;
            priceItemsLst.Items.Add(lvi);
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            priceCalculatedFld.Text = CalculateTotal().ToString("C");
        }

        private decimal CalculateTotal()
        {
            decimal total = 0;
            foreach (ListViewItem lvi in priceItemsLst.Items)
            {
                total += ((PriceItem)lvi.Tag).Total;
            }
            return total;
        }

        private PartyPackages SelectedPartyPackage()
        {
            if (partyStandardRdo.Checked)
                return PartyPackages.Party;
            else if (partyPlusRdo.Checked)
                return PartyPackages.PartyPlus;
            else if (partyPremiumRdo.Checked)
                return PartyPackages.PremiumParty;
            else
                return PartyPackages.NotSet;
        }

        private void contactEmblemPic_Click(object sender, EventArgs e)
        {
            try
            { 
            MessageBox.Show("Planned feature: displays list of previous bookings in new dialog.");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to show previous bookings", ex);
            }
        }

        private void priceProductBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (_isLoading)
                    return;

                var pi = PriceItemsBL.Get((ProductIds)priceProductBox.SelectedValue);
                priceDescFld.Text = pi.Description;
                priceAmtFld.Text = pi.UnitPrice.ToString("C");
                priceQtyFld.Text = "1";
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to select price item", ex);
            }
        }

        private void pricingMnu_delete_click(object sender, EventArgs e)
        {
            try
            {
                if (priceItemsLst.SelectedIndices.Count == 0)
                    return;

                int deleteIdx = priceItemsLst.SelectedIndices[0];
                priceItemsLst.SelectedIndices.Clear();
                priceItemsLst.Items.RemoveAt(deleteIdx);
                UpdateTotal();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to delete price item", ex);
            }
        }

        private void shortDemosChk_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                AddOrRemovePriceProduct(ProductIds.ShortDemonstrations, shortDemosChk.Checked);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to toggle Short Demonstrations", ex);
            }
        }

        private void savedTmr_Tick(object sender, EventArgs e)
        {
            savedFld.Visible = false;
            savedTmr.Stop();
            savedTmr.Enabled = false;
        }

        private void serviceAddCrocChk_CheckedChanged(object sender, EventArgs e)
        {
            if (_isLoading)
                return;

            try
            {
                AddOrRemovePriceProduct(ProductIds.AddCrocodile, serviceAddCrocChk.Checked);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to toggle Add Croc", ex);
            }
        }

        private void AddOrRemovePriceProduct(ProductIds product, bool isAdd)
        {
            if (isAdd)
            {
                AddPriceItem(PriceItemsBL.Get(product));
            }
            else
            {
                //remove
                ListViewItem item = null;
                foreach (ListViewItem lvi in priceItemsLst.Items)
                {
                    if (((PriceItem)lvi.Tag).ProductId == product)
                    {
                        item = lvi;
                        break;
                    }
                }
                if (item != null)
                {
                    priceItemsLst.Items.Remove(item);
                    UpdateTotal();
                }
            }
        }

        private void priceOverrideFld_Leave(object sender, EventArgs e)
        {
            try
            {
                if (!priceOverrideChk.Checked)
                {
                    MessageBox.Show(this, "You must tick off 'Override' to change this field",
                        "Price Override", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }

                decimal calculated;
                if (decimal.TryParse(priceCalculatedFld.Text.Trim('$'), out calculated))
                {
                    if (calculated > 0 && priceItemsLst.Items.Count > 0)
                    {
                        decimal overrideVal;
                        if (decimal.TryParse(priceOverrideFld.Text.Trim(), out overrideVal))
                        {
                            decimal difference = overrideVal - calculated;
                            var updatedItems = new List<PriceItem>();
                            foreach (ListViewItem lvi in priceItemsLst.Items)
                            {
                                var priceItem = (PriceItem)lvi.Tag;
                                var targetChange = difference;
                                var oldSubtotal = priceItem.Total;
                                if (oldSubtotal < 0)
                                {
                                    //exclude discount item from this technique
                                }
                                else
                                {
                                    if (oldSubtotal + targetChange < 0)
                                    {
                                        targetChange = -1 * priceItem.Total;
                                    }
                                    if (targetChange != 0)
                                    {
                                        priceItem.UnitPrice = (oldSubtotal + targetChange)
                                            / priceItem.Quantity;
                                        if (priceItem.Total != oldSubtotal + targetChange)
                                            throw new Exception($"Failed to adjust the price item {priceItem.Description} to ${(oldSubtotal + targetChange)} (got ${priceItem.Total})");
                                        difference -= targetChange;
                                    }
                                }
                                updatedItems.Add(priceItem);
                            }
                            if (difference != 0)
                            {
                                MessageBox.Show(this,
                                    $"Failed to automatically adjust price items for the override price of ${overrideVal} (${difference}) could not be achieved)",
                                    "Price Override", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            priceItemsLst.BeginUpdate();
                            priceItemsLst.Items.Clear();
                            updatedItems.ForEach(x => AddPriceItem(x));
                            priceItemsLst.EndUpdate();

                            priceOverrideFld.Text = "";
                            priceOverrideChk.Checked = false;
                        }
                        return; //including if no amount was set

                    }
                }

                MessageBox.Show(this, "You must have pricing items on the booking before you can override the price",
                    "Price Override", MessageBoxButtons.OK, MessageBoxIcon.Hand);

            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to override price", ex);
            }
        }

        private void completeBtn_Click(object sender, EventArgs e)
        {
            if (_newStatus != BookingStates.Booked && _newStatus != BookingStates.PaymentDue)
            {
                MessageBox.Show(this, "The booking should be in a 'Booked' or 'Payment Due' status before you can close it", "Close Booking",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            if (_booking.BookingDate > DateTime.Now)
            {
                MessageBox.Show(this, "The job is not scheduled to have been completed yet.", "Close Booking",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            if (_booking.AmountPaid < CalculateTotal())
            {
                MessageBox.Show(this, "The booking is not yet fully paid.", "Close Booking",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            var backupStatus = _newStatus;
            try
            {
                _newStatus = BookingStates.Completed;
                if (Save())
                {
                    completeBtn.Enabled = false;
                    bookBtn.Enabled = false;
                    cancelBtn.Enabled = true;
                    LoadTitlebar();
                }
                else
                {
                    _newStatus = backupStatus;
                }
            }
            catch (Exception ex)
            {
                _newStatus = backupStatus;
                ErrorHandler.HandleError(this, "Failed to Book it", ex);
            }
        }
    }
}
