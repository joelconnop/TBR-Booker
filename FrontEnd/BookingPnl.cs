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
        private Followup _currentFu;
        private Followup _nextFu;
        private Followup _confirmationCall;
        private bool _isLoading;
        private BookingStates _newStatus;
        public Timeline Timeline;
        private bool _isChangingTime;
        private bool _isSearchingCustomers;
        private bool _isSearchModeForBookings;
        private int _duration;
        private List<ValidatingTextbox> _validators;

        // HIGHLIGHTS IDEA
        // pink for important fields that are not filled in
        // yellow for user highlights, click an icon of a texta to go in and out of highlighting mode,
        // or use context menu. Change the mouse icon even if it is just Cursor = Cursors.Hand
        // any control can be highlighted, saved to list of their names in Booking
        // allow for controls to be renamed (ignore instead of exception)
        // is it possible to drag and drop an area to highlight,
        // rather than remembering to put click events on all

        public Booking GetBooking()
        {
            return _booking;
        }

        public BookingPnl(Booking booking, BookingsFrm owner, bool isOnLeftTab)
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
                _corporateAccount = booking.Account;
            }

            ConfigureMoveButtons(isOnLeftTab);
        }


        private void copyLastToNickBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(contactLastNameFld.Text))
            {
                string nickname = contactLastNameFld.Text.Trim();
                if (!string.IsNullOrEmpty(contactFirstNameFld.Text.Trim()))
                    nickname = contactFirstNameFld.Text.Trim().Substring(0, 1) + ". "
                        + nickname;
                contactNicknameFld.Text = nickname;
            }
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
            startPick.SetEventHandler(StartTimeChanged);
            endPick.SetEventHandler(EndTimeChanged);
            SetTime(_booking.BookingTime, _booking.Duration);

            Timeline = new Timeline(_booking, _owner, this);
            dateGrp.Controls.Add(Timeline);
            Timeline.Location = new Point(13, 65);

            //address
            ComboBoxItem.InitComboBox(addressRegionBox, typeof(LocationRegions), _booking.LocationRegion);
            addressFld.Text = _booking.Address;
            addressVenuFld.Text = _booking.VenueName;

            //service 
            _service = _booking.Service;
            ComboBoxItem.InitComboBox(serviceBox, typeof(ServiceTypes),
                _service?.ServiceType ?? ServiceTypes.NotSet);
            LoadService(_service);
            

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

            var ignoreSkus = new List<Enum>();
            foreach (ProductIds sku in Enum.GetValues(typeof(ProductIds)))
            {
                if (PriceItemsBL.IsAService(sku))
                    ignoreSkus.Add(sku);
            }
            ComboBoxItem.InitComboBox(priceProductBox, typeof(ProductIds), ProductIds.NotSet, ignoreSkus);

            //payments (must be done after price items added)
            UpdatePaymentFields();
            priceInvoiceBtn.Enabled = !_booking.IsInvoiced;
            pricingPayOnDayChk.Checked = _booking.IsPayOnDay;
            ComboBoxItem.InitComboBox(priceMethodBox, typeof(PaymentMethods), PaymentMethods.NotSet);
            _booking.PaymentHistory.ForEach(x => priceHistoryLst.Items.Add(x.ToString()));

            //followups
            if (_booking.IsNewBooking)
            {
                //kick off the first followup
                _nextFu = DashboardBL.CreateFirstEnquiryFollowup(_booking.BookingDate);
            }
            else
            {
                var bookingFu = _booking.GetCurrentFollowup();
                if (bookingFu != null)
                    _currentFu = (Followup)bookingFu.Clone();
                if (_booking.ConfirmationCall != null)
                {
                    _confirmationCall = (Followup)_booking.ConfirmationCall.Clone();
                    //added these two lines in a bit of a rush, because uncertainty about assigning these
                    //values when through ConfigureFollowupControls if CCall tab is already showing,
                    //because that method gets called for dealing with the current followup
                    //more investigation needed
                    fuConfirmationPick.Value = _confirmationCall.FollowupDate;
                    fuConfirmationNoteFld.Text = _confirmationCall.CompleteNote;
                }
            }
            BuildFollowupHistory(false);
            ConfigureFollowupControls();

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
            _validators.Add(new ValidatingTextbox(this, priceQtyFld, ValidatingTextbox.TextBoxValidationType.IntegerZeroPlus));
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
            ComboBoxItem.InitComboBox(contactLeadBox, typeof(LeadSources), _customer?.LeadSource ?? LeadSources.NotSet);

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
                notesPastFld.Text = _customer.PastNotes;
            }
        }

        private void LoadAccount()
        {
            if (_corporateAccount != null)  // && _booking.IsOpen()) //<-- why?
            {
                contactCompanyFld.Text = _corporateAccount.BusinessName;
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

        private void LoadService(Service service)
        {
            if (service.ServiceType == ServiceTypes.ReptileParty)
            {
                var party = service.Party;
                var package = party.Package;
                if (package == PartyPackages.NotSet)
                {
                    var partyPriceItem = service.PriceItems.FirstOrDefault(
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
            serviceAddCrocChk.Checked = service.AddCrocodile;
            servicePaxFld.Text = service.Pax.ToString();
            serviceAnimalsToCome.Text = service.SpecificAnimalsToCome;
            shortDemosChk.Checked = (service.PriceItems
                .Any(x => x.ProductId == ProductIds.ShortDemonstrations));
        }

        public void StartTimeChanged(TimePicker.TimePickerValue tpv)
        {
            try
            {
                if (_isChangingTime || !startPick.Visible)
                    return;

                UpdateEndTime(tpv.Value);

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

        private void UpdateEndTime(int newStartTime)
        {
            var newEndTime = 0;
            if (_duration > 0)
            {
                newEndTime = Utils.EndTime(newStartTime, _duration);
            }
            endPick.SetValues(newStartTime, 1900, 15, newEndTime);
        }

        public void EndTimeChanged(TimePicker.TimePickerValue tpv)
        {
            //this field doesn't manage a property directly, it instead affects the duration
            try
            {
                if (_isChangingTime)
                    return;

                if (startPick.GetSelected().Value > 0)
                {
                    int newDuration = Utils.MinuteDifference(startPick.GetSelected().Value, tpv.Value);
                    if (newDuration != _duration)
                    {
                        _duration = newDuration;
                        durationFld.Text = _duration.ToString();
                        SetDurationDesc(_duration);
                        if (Timeline != null)
                        {
                            Timeline.Duration = newDuration;
                            Timeline.Redraw();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to update the timeline", ex);
            }
        }

        private void timePick_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                    timePick.Value = timePick.Value.AddMinutes(-15);
                else if (e.KeyCode == Keys.Up)
                    timePick.Value = timePick.Value.AddMinutes(15);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to change the time", ex);
            }
        }

        private void timePick_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (_isChangingTime || !timePick.Visible)
                    return;

                var newStartTime = Utils.TimeInt(timePick.Value);
                startPick.SetValues(600, 1900, 15, newStartTime);

                UpdateEndTime(newStartTime);

                if (Timeline != null)
                {    
                    Timeline.Time = newStartTime;
                    Timeline.Redraw();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to update the timeline", ex);
            }
        }

        private void timeSwitchChk_CheckedChanged(object sender, EventArgs e)
        {
            startPick.Visible = !timeSwitchChk.Checked;
            timePick.Visible = timeSwitchChk.Checked;
        }

        private void durationFld_Leave(object sender, EventArgs e)
        {
            try
            {
                if (_isChangingTime)
                    return;

                int newDuration = 0;
                if (!int.TryParse(durationFld.Text.Trim(), out newDuration))
                {
                    //revert
                    durationFld.Text = _duration.ToString();
                }
                else if (newDuration != _duration)
                {
                    _duration = newDuration;
                    UpdateEndTime(startPick.GetSelected().Value);
                    Timeline.Duration = newDuration;
                    Timeline.Redraw();
                    SetDurationDesc(newDuration);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to update the timeline", ex);
            }
        }

        private void SetDurationDesc(int newDuration)
        {
            if (newDuration > 0)
            {
                durationDescFld.Text = "(" + Utils.DurationToDisplayStr(newDuration) + ")";
            }
            else
            {
                durationDescFld.Text = "";
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
                    Timeline.UpdateOtherBookings();
                }

                UpdateConfirmationCall();

                if (_currentFu != null && _currentFu.Purpose.Equals(DashboardBL.EnquiryFollowupText))
                {
                    FinishFollowup(_currentFu, "Booked");
                    ConfigureFollowupControls();
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

            timePick.Value = DateTime.Parse("1/1/2000 12:00:00 AM")
                + Booking.GetBookingTime(time);

            startPick.SetValues(600, 1900, 15, time);

            endPick.SetValues(time > 0 ? time : 600, 1900, 15, 
                duration > 0 ? Utils.EndTime(time, duration) : 0);

            _duration = duration;
            durationFld.Text = duration.ToString();
            SetDurationDesc(duration);

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

                Cursor = Cursors.WaitCursor;

                _customer = DBBox.ReadItem<Customer>((string)searchLst.SelectedItems[0].Tag);

                var pastBookings = GetPastBookings();

                var accountId = pastBookings.OrderByDescending(x => x.BookingDate)
                    .FirstOrDefault(x => !string.IsNullOrEmpty(x.AccountId))?.AccountId ?? null;
                string copyFromId = null;
                if (!string.IsNullOrEmpty(accountId))
                {
                    _corporateAccount = DBBox.ReadItem<CorporateAccount>(accountId);
                    if (!string.IsNullOrEmpty(_corporateAccount.DefaultBookingId))
                        copyFromId = _corporateAccount.DefaultBookingId;
                }
                else
                {
                    _corporateAccount = null;
                }

                if (string.IsNullOrEmpty(copyFromId) && _customer.BookingIds.Any())
                {
                    copyFromId = _customer.BookingIds.Last();
                }

                if (!string.IsNullOrEmpty(copyFromId))
                {
                    Cursor = Cursors.Default;
                    if (MessageBox.Show(this, "Would you like to copy the details from "
                        + (_corporateAccount?.TradingName ?? _customer.FullName()) + "'s last booking #"
                        + copyFromId + "?", "Returning Customer", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Cursor = Cursors.WaitCursor;
                        CopyBookingDetails(BookingBL.GetBookingFull(copyFromId));
                    }
                    else
                    {
                        Cursor = Cursors.WaitCursor;
                    }
                }

                LoadAccount();
                LoadCustomer();
                SetCustomerEmblem(pastBookings);

                //this flag is kind of dangerous as it determines different types to put on LVI tags. Planning to make booking search a separate window
                //_isSearchModeForBookings = true;


                searchPnl.Visible = false;
                contactSearchChk.Checked = false;

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                ErrorHandler.HandleError(this, "Failed to select customer", ex);
            }
        }

        private void CopyBookingDetails(Booking other)
        {
            contactNicknameFld.Text = other.BookingName;

            SetTime(other.BookingTime, other.Duration);
            Timeline.Time = other.BookingTime;
            Timeline.Duration = other.Duration;
            Timeline.Redraw();

            ComboBoxItem.ManuallySelectItem<LocationRegions>
                (addressRegionBox, other.LocationRegion);
            addressFld.Text = other.Address;
            addressVenuFld.Text = other.VenueName;

            ComboBoxItem.ManuallySelectItem<ServiceTypes>
                (serviceBox, other.Service.ServiceType);
            LoadService(other.Service);

            priceItemsLst.Items.Clear();
            other.Service.PriceItems.ForEach(x => AddPriceItem((PriceItem)x.Clone()));
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

        private void priceProductBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)    // (char)Keys.Return && AttemptToSelectPriceProduct())
                TryAddCustomPriceItem();
        }

        private void priceDescFld_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                TryAddCustomPriceItem();
        }

        private void priceAmtFld_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                TryAddCustomPriceItem();
        }

        private void priceQtyFld_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                TryAddCustomPriceItem();
        }

        private void priceAddBtn_Click(object sender, EventArgs e)
        {
            TryAddCustomPriceItem();
        }

        private void TryAddCustomPriceItem()
        {
            try
            {
                var selectedItem = ComboBoxItem.GetSelected<ProductIds>(priceProductBox);
                if (selectedItem == ProductIds.NotSet
                    || string.IsNullOrEmpty(priceDescFld.Text) || string.IsNullOrEmpty(priceQtyFld.Text))
                {
                    MessageBox.Show(this, "You must fill in all fields for the Price Item.",
                        "Add/Update Price Item", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }
                else if (priceQtyFld.Text.Equals("0"))
                {
                    MessageBox.Show(this, "Quantity cannot be zero.",
                        "Add/Update Price Item", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    priceQtyFld.Focus();
                    return;
                }

                AddPriceItem(new PriceItem(selectedItem,
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

        private void priceInvoiceBtn_Click(object sender, EventArgs e)
        {
            //just load QB URL for now
            //set invoiced to true and save
            //CREATING IN QUICKBOOKS MIGHT NOT BE BEST IDEA. Amount can need revising and this is a pain
            //more thought needed on if/when invoice is created in quickbooks
            //and consider immediate benefit of creating invoices in html
            MessageBox.Show("Later phase. For now, create manually in Quickbooks.");
        }

        private void priceMethodBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                SubmitPayment();
        }

        private void priceSubmitBtn_Click(object sender, EventArgs e)
        {
            SubmitPayment();
        }

        private void SubmitPayment()
        {
            if (!Booking.IsBooked(_newStatus, true))
            {
                MessageBox.Show(this, "You can only submit a payment once this becomes a 'Booking'",
                        "Add Payment", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            try
            {
                decimal paymentAmount;
                var selectedMethod = ComboBoxItem.GetSelected<PaymentMethods>(priceMethodBox);
                if (string.IsNullOrEmpty(priceSubmitFld.Text)
                    || !decimal.TryParse(priceSubmitFld.Text, out paymentAmount)
                    || paymentAmount == 0 || selectedMethod == PaymentMethods.NotSet)
                {
                    MessageBox.Show(this, "You missed a payment field (or two)",
                        "Add Payment", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }

                if (MessageBox.Show(this, $"Confirm the details are correct:{Environment.NewLine}Booking: {_booking.Id} {contactNicknameFld.Text}{Environment.NewLine}Amount: {paymentAmount.ToString("C")}{Environment.NewLine}Method: {selectedMethod.ToString()}{Environment.NewLine}Resulting Balance: {(_booking.Balance - paymentAmount).ToString("C")}",
                   "Add Payment", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    != DialogResult.Yes)
                    return;

                //SEE Settings.CreditCardMultiplier

                //quickbooks stuff!
                //use wait cursor
                if (paymentAmount < 0)
                {

                }
                var payment = new Payment(DateTime.Now, paymentAmount, selectedMethod);
                _booking.PaymentHistory.Add(payment);
                Save();   //this is only way that payment gets saved into history, and will need to do once quickbooks done anyway
                priceHistoryLst.Items.Add(payment.ToString());
                UpdatePaymentFields();

            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to save submit payment", ex);
            }
        }

        private void UpdatePaymentFields()
        {
            //the value of booking might have changed since last save, but payments made on the object is always correct
            var total = CalculateTotal();
            pricePaidLbl.Text = _booking.AmountPaid.ToString("C");
            priceBalanceFld.Text = (total - _booking.AmountPaid).ToString("C");
            pricePaidPic.Visible = total > 0 && total == _booking.AmountPaid && Booking.IsOpenStatus(_newStatus);
            priceIssuePic.Visible = total < _booking.AmountPaid
                || (_newStatus == BookingStates.Completed && total > _booking.AmountPaid);
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
            if (ComboBoxItem.GetSelected<LeadSources>(contactLeadBox) != _booking.Customer.LeadSource)
                unsavedChanges += "lead source, ";

            if (_corporateAccount != null && _booking.Account == null)
            {
                unsavedChanges += "linking of corporate account, ";
            }

            if (!datePick.Value.ToShortDateString().Equals(_booking.BookingDate.ToShortDateString()))
                unsavedChanges += "booking date, ";
            if (startPick.GetSelected().Value != _booking.BookingTime)
                unsavedChanges += "booking time, ";
            if (_duration != _booking.Duration)
                unsavedChanges += "duration, ";

            if (ComboBoxItem.GetSelected<LocationRegions>(addressRegionBox) != _booking.LocationRegion)
                unsavedChanges += "region, ";
            if (!addressFld.Text.Equals(_booking.Address))
                unsavedChanges += "e-mail, ";
            if (!addressVenuFld.Text.Equals(_booking.VenueName))
                unsavedChanges += "venue, ";

            if (//_booking.Service != null && at the least it should be NotSet, for an existing booking
                ComboBoxItem.GetSelected<ServiceTypes>(serviceBox) != _service.ServiceType)
            {
                unsavedChanges += "service type, ";
            }
            else if (_service.ServiceType == ServiceTypes.ReptileParty)
            {
                var party = _service.Party;
                if (SelectedPartyPackage() != party.Package)
                    unsavedChanges += "party package, ";
                if (!partyBirthdayNameFld.Text.Equals(party.BirthdayName))
                    unsavedChanges += "birthday name, ";
                if (!partyAgeFld.Text.Equals(party.BirthdayAge.ToString())
                    && !(string.IsNullOrEmpty(partyAgeFld.Text) && party.BirthdayAge == 0))
                    unsavedChanges += "birthday age, ";
            }

            if (!servicePaxFld.Text.Equals(_service.Pax.ToString())
                && !(string.IsNullOrEmpty(servicePaxFld.Text) && _service.Pax == 0))
                unsavedChanges += "pax, ";
            if (!serviceAnimalsToCome.Text.Equals(_service.SpecificAnimalsToCome))
                unsavedChanges += "animals to come, ";
            if (serviceAddCrocChk.Checked != _service.AddCrocodile)
                unsavedChanges += "add croc, ";


            int pricingItemCnt = priceItemsLst.Items.Count;
            if (pricingItemCnt != _service.PriceItems.Count)
                unsavedChanges += "number pricing items, ";
            else
            {
                for (int i = 0; i < pricingItemCnt; i++)
                {
                    var itm1 = (PriceItem)priceItemsLst.Items[i].Tag;
                    var itm2 = _service.PriceItems[i];
                    if (!itm1.Equals(itm2))
                        unsavedChanges += itm1.Description + ", ";
                }
            }
            
            if (priceOverrideChk.Checked && Convert.ToDecimal(priceOverrideFld.Text)
                != _service.TotalPrice)
                unsavedChanges += "overidden total price, ";

            if (pricingPayOnDayChk.Checked != _booking.IsPayOnDay)
                unsavedChanges += "pay on day, ";

            //followups
            SyncFollowupControlsToObjects();
            if (_nextFu != null)
                unsavedChanges += "next followup, ";
            if (_currentFu != null && !_currentFu.Equals(_booking.GetCurrentFollowup()))
                unsavedChanges += "current followup, ";
            if (_confirmationCall != null && !_confirmationCall.Equals(_booking.ConfirmationCall))
                unsavedChanges += "confirmation call, ";

            if (!notesBookingFld.Text.Equals(_booking.BookingNotes))
                unsavedChanges += "booking notes, ";
            if (!notesPastFld.Text.Equals(_booking.Customer.PastNotes))
                unsavedChanges += "past notes, ";

            return unsavedChanges.Trim().Trim(',');
        }

        private void SyncFollowupControlsToObjects()
        {
            if (_currentFu != null)
            {
                _currentFu.CompleteNote = fuCompleteFld.Text.Trim();
            }

            if (_nextFu != null)
            {
                _nextFu.FollowupDate = Utils.StartOfDay(fuDatePick.Value);
                _nextFu.Purpose = fuPurposeFld.Text;
            }

            if (_confirmationCall != null)
            {
                _confirmationCall.FollowupDate = Utils.StartOfDay(fuConfirmationPick.Value);
                _confirmationCall.CompleteNote = fuConfirmationNoteFld.Text.Trim();
            }
        }

        public bool Save()
        {
            Cursor = Cursors.WaitCursor; 

            try
            {
                //any form validations to be first
                if (_nextFu != null ^ fuScheduleChk.Checked)
                    //3 wrong cases, 2 right cases
                    throw new Exception("Next followup mismatch with object instance and checkbox.");

                _booking.Status = _newStatus;

                if (_customer == null)
                    _customer = new Customer();

                _customer.FirstName = contactFirstNameFld.Text;
                _customer.LastName = contactLastNameFld.Text;
                _customer.PrimaryNumber = contactPrimaryNumFld.Text;
                _customer.SecondaryNumber = contactSecondaryNumFld.Text;
                _customer.CompanyName = contactCompanyFld.Text;               
                _customer.EmailAddress = contactEmailFld.Text;
                _customer.LeadSource = ComboBoxItem.GetSelected<LeadSources>(contactLeadBox);

                _customer.CompanyId = _corporateAccount?.Id ?? "";
                _booking.AccountId = _corporateAccount?.Id ?? "";
                _booking.Account = _corporateAccount;

                //always set bookingNickname, even if empty (will copy from customer during save), and even if same as customer name
                _booking.BookingNickname = contactNicknameFld.Text;
                _booking.BookingDate = Utils.StartOfDay(datePick.Value);
                _booking.BookingTime = startPick.GetSelected().Value;
                _booking.Duration = _duration;

                _booking.LocationRegion = ComboBoxItem.GetSelected<LocationRegions>(addressRegionBox);
                _booking.Address = addressFld.Text;
                _booking.VenueName = addressVenuFld.Text;

                _booking.Service = _service;
                var serviceType = ComboBoxItem.GetSelected<ServiceTypes>(serviceBox);
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

                //followups (need to be done last because if we have to get here twice due to validation, they will be added to Booking twice)
                SyncFollowupControlsToObjects();

                if (_currentFu != null)
                {
                    var fu = _booking.GetCurrentFollowup();
                    if (fu != null)
                    {
                        if (!fu.Equals(_currentFu))
                        {
                            _booking.Followups.Remove(fu);
                            _booking.Followups.Add(_currentFu);
                        }
                        //do nothing if FU hasn't changed
                    }
                    else
                    {
                        _booking.Followups.Add(_currentFu);
                    }

                    //unassign if it has just been completed
                    if (!_currentFu.IsOutstanding())
                        _currentFu = null;
                }

                if (_nextFu != null && fuScheduleChk.Checked)
                {
                    _booking.Followups.Add(_nextFu);
                }

                // (even if null, ie cancelled job)
                _booking.ConfirmationCall = _confirmationCall;

                BookingBL.SaveBookingEtc(_booking);

                //once saved, the 'next' followup becomes the 'current' followup
                if (_nextFu != null && fuScheduleChk.Checked)
                {
                    _currentFu = _nextFu;
                    _nextFu = null;
                    fuScheduleChk.Checked = false;
                    ConfigureFollowupControls();
                }

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
                        UpdateConfirmationCall();
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
                        FinishFollowup(_currentFu, "Booking cancelled", true);
                        FinishFollowup(_confirmationCall, "Booking cancelled", true);
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
                        FinishFollowup(_currentFu, "Booking cancelled", true);
                        FinishFollowup(_confirmationCall, "Booking cancelled", true);
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

        private void FinishFollowup(Followup fu, string completeNote, bool isIgnoreIfNoFu = false)
        {
            if (fu == null)
            {
                if (!isIgnoreIfNoFu)
                    throw new Exception($"Tried to close the followup with note '{completeNote}', but no current followup was found");
                return;
            }
            if (!fu.IsOutstanding())
            {
                return;
            }

            fu.CompletedDate = Utils.StartOfDay();
            fu.CompleteNote = completeNote;

            if (!fu.IsConfirmationCall)
                BuildFollowupHistory(true);
        }

        private void BuildFollowupHistory(bool isAddCurrentFollowup)
        {
            fuHistoryLst.Items.Clear();

            if (isAddCurrentFollowup)
                fuHistoryLst.Items.Add(MakeLvi(_currentFu));

            foreach (var fu in _booking.Followups.Where(x => !x.IsOutstanding())
                .OrderByDescending(y => y.CompletedDate.Value))
                fuHistoryLst.Items.Add(MakeLvi(fu));

            ListViewItem MakeLvi(Followup fu)
            {
                var lvi = new ListViewItem(new string[] { fu.FollowupDate.ToString("d"),
                fu.Purpose, fu.CompletedDate.Value.ToString("d"), fu.CompleteNote});
                lvi.Tag = fu;
                return lvi;
            }
        }

        private void bookBtn_Click(object sender, EventArgs e)
        {
            //plan for Confirmation Calls
            //when Book It pressed, we create the confirmation call FU with appropriate date
            //as a SEPARATE followup on Booking, with IsConfirmationCall = true.
            //Booking.FollowupDate() is the min of Booking.ConfirmationCall (when exists and not completed)
            //and Booking.GetCurrentFollowup()
            //reason it needs to be separate is so that user is able to set another FU with closer date
            //(without needing to 'complete' the confirmation call)
            //Confirmation call tab looks just like 'Complete Current Followup' with different button name
            //and fixed text (have constant for 'Confirmation Call')
            //(would fit to allow reschedule, but is there a legit use case? Could be confusing?)
            //this tab only appears once a confirmation call exists, and remains visible but disabled once
            //confirmation call is made. This needs to be done on ConfigureFollowupControls()
            //we make Confirmation Call the current tab if theres no other outstanding FU, or conf call precedes it
            //if user wants to push out the confirmation call, they can use a custom followup for that
            //for dashboard, we need to read the entire Booking for anything with an outstanding followup
            //we then have all the info we need to construct dashboard
            //don't forget to refresh dashboard when calendar refreshed

            try
            {
                var backupStatus = _newStatus;
                _newStatus = BookingStates.Booked;

                UpdateConfirmationCall();
                
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

        private void completeBtn_Click(object sender, EventArgs e)
        {
            if (_newStatus != BookingStates.Booked && _newStatus != BookingStates.PaymentDue)
            {
                MessageBox.Show(this, "The booking should be in a 'Booked' or 'Payment Due' status before you can close it", "Close Booking",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            if (_booking.BookingDate > Utils.StartOfDay())
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
                //leaving this for now - it might be desirable to schedule an after service fu in some cases
                //FinishFollowup(_currentFu, "Booking is completed and paid", true);
                FinishFollowup(_confirmationCall != null && _confirmationCall.IsOutstanding()
                    ? _confirmationCall : null, "Expired", true);
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
                System.Diagnostics.Process.Start(BookingBL.GenerateBookingHtmlFile(_booking, false));
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
            switch (ComboBoxItem.GetSelected<ServiceTypes>(serviceBox))
            {
                case ServiceTypes.ReptileParty:
                    partyPnl.Visible = true;
                    break;
                case ServiceTypes.Display:
                    displayPnl.Visible = true;
                    TryAddBasePriceItem();
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

                var selectedRegion = ComboBoxItem.GetSelected<LocationRegions>(addressRegionBox);
                var selectedService = ComboBoxItem.GetSelected<ServiceTypes>(serviceBox);

                if (selectedRegion == LocationRegions.NotSet
                    || selectedService == ServiceTypes.NotSet
                    || (selectedService == ServiceTypes.ReptileParty
                    && SelectedPartyPackage() == PartyPackages.NotSet))
                    return;

                var item = PriceItemsBL.GetBaseItem(selectedRegion, selectedService, SelectedPartyPackage());
                if (item != null)
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
            UpdatePaymentFields();
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
            AttemptToSelectPriceProduct();
        }

        private bool AttemptToSelectPriceProduct()
        {
            try
            {
                if (_isLoading)
                    return false;

                var pi = PriceItemsBL.Get(ComboBoxItem.GetSelected<ProductIds>(priceProductBox));
                priceDescFld.Text = pi.Description;
                priceAmtFld.Text = pi.UnitPrice.ToString("C");
                priceQtyFld.Text = pi.Quantity.ToString();
                return pi.Quantity > 0;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Failed to select price item", ex);
            }

            return false;
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
            if (!shortDemosChk.Focused)
                return;

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
            if (!serviceAddCrocChk.Focused)
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

        private void ConfigureFollowupControls()
        {
            if ((_currentFu == null || !_currentFu.IsOutstanding()))
            {
                //there is no current followup, or user has already completed it
                if (_nextFu != null)
                {
                    fuScheduleChk.Checked = true;
                    fuDatePick.Value = _nextFu.FollowupDate;
                    fuPurposeFld.Text = _nextFu.Purpose;
                }

                if (fuTabs.TabPages.Contains(fuCurrentPage))
                    fuTabs.TabPages.Remove(fuCurrentPage);
                if (!fuTabs.TabPages.Contains(fuNextPage))
                {
                    fuTabs.TabPages.Add(fuNextPage);
                    if (_confirmationCall == null || !_confirmationCall.IsOutstanding())
                        fuTabs.SelectedTab = fuNextPage;
                }
            }

            else
            {
                //there is a current followup to be completed
                fuPurposeLbl.Text = _currentFu.Purpose;
                fuDateLbl.Text = _currentFu.FollowupDate.ToShortDateString();
                fuCompleteFld.Text = _currentFu.CompleteNote;

                if (fuTabs.TabPages.Contains(fuNextPage))
                    fuTabs.TabPages.Remove(fuNextPage);
                if (!fuTabs.TabPages.Contains(fuCurrentPage))
                {
                    fuTabs.TabPages.Add(fuCurrentPage);
                    fuTabs.SelectedTab = fuCurrentPage;
                }
            }

            if (_confirmationCall != null)
            {
                if (!fuTabs.TabPages.Contains(fuConfirmationPage))
                {
                    fuConfirmationPick.Value = _confirmationCall.FollowupDate;
                    fuConfirmationNoteFld.Text = _confirmationCall.CompleteNote;
                    fuTabs.TabPages.Add(fuConfirmationPage);
                }
                if (_confirmationCall.IsOutstanding() && (_currentFu == null || 
                    !_currentFu.IsOutstanding() || _confirmationCall.FollowupDate < _currentFu.FollowupDate))
                {
                    fuTabs.SelectedTab = fuConfirmationPage;
                }
                fuConfirmationCompleteBtn.Enabled = fuConfirmationPick.Enabled = _confirmationCall.IsOutstanding();
            }
            else if (fuTabs.TabPages.Contains(fuConfirmationPage))
            {
                fuTabs.TabPages.Remove(fuConfirmationPage);
            }
        }

        private void fuSetBtn_Click(object sender, EventArgs e)
        {
            //currently does nothing

            //TRY WITHOUT updating current followup, ie can only create a new one if none in progress.
            //if we do decide to allow updating current fu (push out date), warn before overriding current
            //ConfigureFollowupControls()
        }

        private void fuHistoryLst_ItemActivate(object sender, EventArgs e)
        {
            try
            {
                //display messagebox with all details of selected followup
                var fu = (Followup)fuHistoryLst.SelectedItems[0].Tag;
                MessageBox.Show(this, $"Followup Scheduled: {fu.FollowupDate.ToShortDateString()}" +
                    $"{Environment.NewLine}For: {fu.Purpose}{Environment.NewLine}" +
                    $"Completed on: {fu.CompletedDate.Value.ToString("G")}" +
                    $"{Environment.NewLine}Completion Note: {fu.CompleteNote}",
                    "Followup History", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "View Followup", ex, true);
            }
        }

        private void fuCompleteBtn_Click(object sender, EventArgs e)
        {
            CompleteFollowupUI(_currentFu);
        }

        private void fuScheduleChk_CheckedChanged(object sender, EventArgs e)
        {
            if (!fuScheduleChk.Focused)
            {
                return;
            }

            fuDatePick.Enabled = fuScheduleChk.Checked;
            fuPurposeFld.Enabled = fuScheduleChk.Checked;
            fuSetBtn.Enabled = fuScheduleChk.Checked;

            //whether setting or unsetting, we want the fields reset
            fuDatePick.Value = Utils.StartOfDay();
            fuPurposeFld.Text = "";

            if (fuScheduleChk.Checked && _nextFu == null)
            {
                //checked because user has just checked it to set a custom followup
                _nextFu = new Followup();
            }
            else if (!fuScheduleChk.Checked)
            {
                // for some reason I thought it was bad to set this to null. Add comment if know why,
                // but if not setting it to null then there's at least one problem on save (xor check)
                //// don't want to set it to null, just look for checked status on save
                _nextFu = null;
            }
        }

        private void fuCompleteFld_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                CompleteFollowupUI(_currentFu);
        }

        private void CompleteFollowupUI(Followup fu)
        {
            try
            {
                FinishFollowup(fu, fu.IsConfirmationCall ? fuConfirmationNoteFld.Text.Trim() : fuCompleteFld.Text.Trim());
                ConfigureFollowupControls();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Complete Followup", ex, true);
            }
        }


        private void fuConfirmationNoteFld_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                CompleteFollowupUI(_confirmationCall);
        }

        private void fuConfirmationCompleteBtn_Click(object sender, EventArgs e)
        {
            CompleteFollowupUI(_confirmationCall);
        }


        private void UpdateConfirmationCall()
        {
            if (_newStatus != BookingStates.Booked)
                return;

            if (_confirmationCall == null || !_confirmationCall.IsOutstanding())
            {
                var newConfirmationCall = DashboardBL.CreateConfirmationCall(Utils.StartOfDay(datePick.Value));
                if (_confirmationCall != null && _confirmationCall.FollowupDate != newConfirmationCall.FollowupDate)
                    _confirmationCall.FollowupDate = _confirmationCall.FollowupDate;
                else
                    _confirmationCall = newConfirmationCall;
            }
            if (_confirmationCall.FollowupDate == Utils.StartOfDay())
            {
                MessageBox.Show(this,
                    "There will be no confirmation call setup, because the service is today.",
                    "Confirmation Call", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _confirmationCall = null;
            }
            else if (_confirmationCall.FollowupDate < Utils.StartOfDay())
            {
                MessageBox.Show(this,
                    "There will be no confirmation call setup, because it appears the service has already happened.",
                    "Confirmation Call", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _confirmationCall = null;
            }
            else if (_confirmationCall.FollowupDate.DayOfWeek != Settings.Inst().ConfirmationCallDay)
            {
                if (MessageBox.Show(this,
                    $"The service is just around the corner, would you still like to create a confirmation call for {_confirmationCall.FollowupDate.ToShortDateString()}?",
                    "Confirmation Call", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    _confirmationCall = null;
            }

            if (_confirmationCall != null)
            {
                fuConfirmationPick.Value = _confirmationCall.FollowupDate;
                fuConfirmationNoteFld.Text = _confirmationCall.CompleteNote;
            }
        }

        public void ConfigureMoveButtons(bool isLeftToRightMode)
        {
            moveLeftBtn.Visible = !isLeftToRightMode;
            moveRightBtn.Visible = isLeftToRightMode;
        }

        private void moveLeftBtn_Click(object sender, EventArgs e)
        {
            _owner.SwitchTabGroup(_booking, false);
        }

        private void moveRightBtn_Click(object sender, EventArgs e)
        {
            _owner.SwitchTabGroup(_booking, true);
            moveLeftBtn.Visible = true;
        }

        private void contactCompanyBtn_Click(object sender, EventArgs e)
        {
            if (_booking.IsNewBooking && _corporateAccount == null)
            {
                MessageBox.Show(this,
                    "Please save the booking before setting up a Corporate Account screen.",
                    "Corporate Account", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_corporateAccount == null)
            {
                _corporateAccount = new CorporateAccount();
                _corporateAccount.BookingIds.Add(_booking.Id);
                _corporateAccount.DefaultBookingId = _booking.Id;
            }

            try
            {
                var frm = new CorporateAccountFrm(_booking, _owner, _corporateAccount);
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    if (string.IsNullOrEmpty(contactCompanyFld.Text))
                        contactCompanyFld.Text = _corporateAccount.SmartName();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "Manage Corporate Account", ex, false);
            }

        }
    }
}
