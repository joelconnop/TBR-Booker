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
using TBRBooker.Base;
using TBRBooker.Business;
using System.Drawing.Text;
using System.Diagnostics;
using TBRBooker.Model.DTO;

namespace TBRBooker.FrontEnd
{
    public partial class Timeline : UserControl
    {

        private Bitmap _pic;

        private BookingsFrm _bookingsFrm;
        private BookingPnl2K _owner;
        private string _bookingId;
        public List<Booking> Others { get; private set; }
        private List<GoogleCalendarItemDTO> _events;

        private const int _inset = 5;
        private const int _insetGraphY = 20;
        private int _setTimeStartX;
        private Font _font = new Font("Microsoft Sans Serif", 8);

        private bool _isSetTimeMode;
        private bool _isSettingTime;
        private Dictionary<int, (int Time, int Mins)> _timeScale;
        private int _lastX;
        private Booking _lastBooking;
        private int _lastBookingStartX;
        private int _contextX;
        private int _travelTime;
        private bool _disableTravel;
        
        public DateTime BookingDate { get; set; }
        public int Time { get; set; }
        public int Duration { get; set; }
        public string Address { get; set; }


        public Timeline(Booking booking, BookingsFrm bookingsFrm, BookingPnl2K owner)
        {
            InitializeComponent();

            _bookingsFrm = bookingsFrm;
            _owner = owner;
            _bookingId = booking.Id;

            BookingDate = booking.BookingDate;
            Time = booking.BookingTime;
            Duration = booking.Duration;
            Address = booking.Address;  //Booking.CombineAddress(booking.Address, booking.VenueName);
            _isSettingTime = false;
            _isSetTimeMode = booking.Duration == 0;
            _timeScale = new Dictionary<int, (int Time, int Mins)>();
        }

        private void Timeline_Load(object sender, EventArgs e)
        {
            UpdateOtherBookingsAndTravelTimesAndRedraw();
        }

        public void UpdateOtherBookingsAndTravelTimesAndRedraw()
        {
            Others = new List<Booking>();

            var bookingItems = DBBox.GetCalendarItems(false, false)
                .Where(x => x.Date.ToShortDateString().Equals(BookingDate.ToShortDateString())
                && !x.BookingNum.ToString().Equals(_bookingId))
                .ToList();
            foreach (var summary in bookingItems)
            {
                Others.Add(DBBox.ReadItem<Booking>((summary.BookingNum.ToString())));
            }

            var rangeStart = DTUtils.StartOfDay(BookingDate);
            foreach (var repeatMarkers in RepeatScheduleBL.GetMarkersInRange
                (rangeStart, rangeStart.AddHours(23.9), false))
            {
                Others.Add(repeatMarkers.HypotheticalBooking);
            }

            try
            {
                _events = new List<GoogleCalendarItemDTO>();

                _events.AddRange(TheGoogle.GetGoogleCalendar(
                    rangeStart, rangeStart.AddHours(23.9), false));
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(_owner, "Read Google Events", ex, true);
            }

            SetTravelTimesAndRedraw();
        }

        public void SetTravelTimesAndRedraw()
        {
            _isSetTimeMode = false; // this line... before timer or during timer? hopefully makes no diff
            // wait just a tick, in case another change is imminent
            if (!travelTmr.Enabled && !travelAndRedrawTmr.Enabled)
            {
                travelTmr.Start();
                // this executes and redraws without any delay
                SetTravelTimes();
                DoRedraw();
            }
            else
            {
                // the last request was less than a second ago, try again in 3 seconds
                Console.WriteLine("Tried to spam google maps but was caught by the timer.");
                // this is a slower timer and is intended to give user chance to finish inputting
                // before doing the next (and hopefully final) google api call
                if (!travelAndRedrawTmr.Enabled)
                    travelAndRedrawTmr.Start();
                // else case: getting too spammy, ignore (but the change will probably display right
                // when the 3 second timer expires and it redraws
            }
        }

        private void SetTravelTimes()
        {
            if (_disableTravel)
                return;

            try
            {
                var addresses = new List<string>();
                var finalArrivalTime = DTUtils.DateTimeFromInt(BookingDate, Time);
                bool thisBookingAdded = false;
                var sortedOthers = Others.Where(x => Booking.IsOpenStatus(x.Status))
                    .OrderBy(x => x.BookingTime).ToList();
                int thisAddressIdx = 0;

                foreach (var other in sortedOthers)
                {
                    if (!thisBookingAdded && !string.IsNullOrEmpty(Address)
                        && Time < other.BookingTime && Time > 0)
                    {
                        addresses.Add(Address);
                        thisBookingAdded = true;
                    }
                    else if (!thisBookingAdded)
                        thisAddressIdx++;
                    if (string.IsNullOrEmpty(other.Address))
                        addresses.Add($"UNKNOWN ADDRESS for {other.BookingNickname}, Qld");
                    else
                        addresses.Add(other.Address);
                }
                if (!thisBookingAdded)
                {
                    if (!string.IsNullOrEmpty(Address) && Time > 0)
                        addresses.Add(Address);
                    else
                        thisAddressIdx = -1;
                }
                else if (sortedOthers.Any())
                {
                    var final = sortedOthers[sortedOthers.Count - 1];
                    finalArrivalTime = DTUtils.DateTimeFromInt(final.BookingDate, final.BookingTime);
                }

                if (addresses.Any())
                {
                    var route = TheGoogle.TravelInfo(addresses, finalArrivalTime);

                    // if they don't match then an address probably couldn't be recognised but don't throw exception
                    if (route.Durations.Length == addresses.Count)
                    {
                        for (int i = 0; i < addresses.Count; i++)
                        {
                            // its a bit cheeky setting state on objects that might not get saved,
                            // but it will work if this is the only significant usage of these fields
                            Booking b = i == thisAddressIdx ? _owner.GetBooking() 
                                : sortedOthers[thisAddressIdx >= 0 && thisAddressIdx < i ? i-1 : i];
                            b.TravelTime = route.Durations[i];
                            b.TravelDistance = route.Distances[i];
                            if (i == thisAddressIdx)
                            {
                                _travelTime = route.Durations[i];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _disableTravel = true;
                ErrorHandler.HandleError(_owner, "Crunch Travel Times", ex, true);
            }

        }

        private void DoRedraw()
        {
            if (!Enabled)
                return;

            _pic = new Bitmap(Box.Size.Width, Box.Size.Height);
            Box.Image = _pic;
            var g = Graphics.FromImage(_pic);

            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            //_others.ForEach(x => DrawBooking(g, x));
            //DrawBooking(g, booking);

            try
            {
                DrawTimeline(g);
            }
            catch (Exception ex)
            {
                Enabled = false;
                ErrorHandler.HandleError(ParentForm, "Unexpected error displaying time line", ex);
            }
            finally
            {
                g.Dispose();
            }
        }

        private int GetTimeX(int time)
        {
            //10 is for 8 hour day 9-5 with a bit of 'before 9' and 'after 5'
            double hourScale = (Box.Width - _inset * 2) / 10;
            int x = _inset;

            var parsed = DTUtils.ParseTime(time);

            if (parsed.Hour < 9)
            {
                hourScale /= 9; //midnight to 9am
                x += Convert.ToInt32(hourScale * parsed.Hour);
            }
            else if (parsed.Hour > 17)
            {
                x += Convert.ToInt32(hourScale * 9); //start at 9 of the 10 hour notches
                hourScale /= 7; //5pm till midnight
                x += Convert.ToInt32(hourScale * (parsed.Hour - 17));
            }
            else
            {
                x += Convert.ToInt32(hourScale * (parsed.Hour - 8)); // - 9am + 1(before 9) = -9
            }

            x += Convert.ToInt32(hourScale / 60 * parsed.Minute);

            return x;
        }

        private int GetBarHeight()
        {
            return Box.Height - 2 * _insetGraphY;
        }

        private void DrawTimeline(Graphics g)
        {
            int lastx = 0;
            var height = GetBarHeight();

            bool isNeedToSetScale = _timeScale.Count == 0;
            var hourPen = new Pen(Brushes.Black);
            hourPen.Width = 3;
            
            for (int i = 0; i <= 2399; i++)
            {
                //the loop isn't great; need to skip numbers that aren't legit minutes
                if (i - (i / 100 * 100) >= 60)
                    continue;

                var x = GetTimeX(i);
                if (x == lastx && i != 900)
                    continue;   //ie out of hours can be the same number again    

                var parsed = DTUtils.ParseTime(i);

                g.FillRectangle(FindBookingsHereAndGetBrush(g, i, x),
                    new Rectangle(new Point(lastx, _insetGraphY), new Size(x - lastx, height)));
                lastx = x;

                //first iteration
                if (_timeScale.Count == 0)
                {
                    _timeScale.Add(0, (i, AllTheMinutesForAllTheHours(parsed)));
                }
                //15 min notch
                else if (parsed.Minute % 15 == 0 && parsed.Hour >= 9 && parsed.Hour <= 17
                    && (parsed.Hour < 17 || parsed.Minute == 0))
                {
                    if (isNeedToSetScale)
                        _timeScale.Add(x, (i, AllTheMinutesForAllTheHours(parsed)));

                    g.DrawLine(parsed.Minute == 0 ? hourPen : Pens.Black, 
                        new Point(x, Box.Height - _insetGraphY), 
                        new Point(x, Box.Height - _insetGraphY / 2));
                    if (parsed.Minute == 0)
                    {
                        string hourMark = DTUtils.DisplayHour(parsed.Hour) + ":00";
                        var drawStr = g.MeasureString(hourMark, _font, 50);
                        g.DrawString(hourMark, _font, Brushes.Black,
                            new PointF(x - drawStr.Width / 2, _inset));
                    }
                }

            }

            hourPen.Dispose();

            Box.Refresh();
        }

        private int AllTheMinutesForAllTheHours((int Hour, int Minute) parsedTime)
        {
            return parsedTime.Hour * 60 + parsedTime.Minute;
        }

        private Brush FindBookingsHereAndGetBrush(Graphics g, int time, int timeX)
        {
            var clashBs = BookingBL.GetClashBookings(Others, time, time);

            // show summary for the last booking, once we have finished drawing its graph
            if (_lastBooking != null && (clashBs.Count == 0 || clashBs[0] != _lastBooking))
            {
                g.DrawString($"{_lastBooking.BookingNickname} - {_lastBooking.LocationRegion} - {_lastBooking.TravelDistance}km",
                    _font, Brushes.Black, new PointF(timeX, _insetGraphY + 5));
                _lastBooking = null;
            }
            if (_lastBooking == null && clashBs.Count > 0)
            {
                _lastBooking = clashBs[0];
                _lastBookingStartX = timeX + 10;
            }

            var isThisBooking = Time > 0 && Duration > 0 && BookingBL.GetClashBookings(new List<Booking> {
                new Booking() {
                    BookingTime = _travelTime > 0 
                    ? DTUtils.AddTimeInts(Time, _travelTime * -1) 
                    : Time,
                    Duration = Duration + _travelTime } }, time, time).Count == 1;

            var blockouts = _events.Where(x => (x.Time >= time && x.EndTime <= time)
                || (time >= x.Time && time <= x.EndTime))
                .ToList();

            if (isThisBooking && (clashBs.Count > 0 || blockouts.Count > 0))
            {
                return clashBs.Any(x => x.IsBooked()) ? Brushes.Red : Brushes.IndianRed;
            }

            if (isThisBooking)
            {
                if (time < Time)
                    return Brushes.PaleGoldenrod;   // travel
                return Brushes.LimeGreen;           // this booking
            }

            if (clashBs.Count > 0)
            {
                if (clashBs.Count > 1 || blockouts.Count > 0)
                {
                    return clashBs.All(x => x.IsBooked()) ? Brushes.Red : Brushes.IndianRed;
                }
                if (time < clashBs[0].BookingTime)
                    return Brushes.PaleGoldenrod;   // travel 
                return new SolidBrush(BookingSlotPnl.BackColourForBooking(clashBs[0].Status));
            }

            if (blockouts.Count > 0)
            {
                return Brushes.Magenta;
            }

            return Brushes.White;
        }

        private void Box_Click(object sender, EventArgs e)
        {
            if (_isSettingTime)
                return;

            var clickedBooking = DetectClickedBooking(Box.PointToClient(MousePosition).X);
            if (clickedBooking != null)
            {
                selectedLbl.Text = clickedBooking.Summary();
                selectedLbl.Left = Box.PointToClient(MousePosition).X;
                selectedLbl.Visible = true;
            }
            else
            {
                selectedLbl.Visible = false;
            }
        }

        private Booking DetectClickedBooking(int mouseX)
        {
            var time = _timeScale.OrderBy(x => Math.Abs(x.Key - mouseX)).First().Value.Time;
            return BookingBL.GetClashBookings(Others, time, time).FirstOrDefault();
        }

        private void contextMenuStrip1_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            
        }

        private void setTimeMnu_Click(object sender, EventArgs e)
        {
            _isSetTimeMode = true;
            _setTimeStartX = -1;
        }

        private void openBookingMnu_Click(object sender, EventArgs e)
        {
            OpenOtherBooking(_contextX);
        }

        private void OpenOtherBooking(int mouseX)
        {
            var clickedBooking = DetectClickedBooking(_contextX);
            if (clickedBooking != null)
            {
                if (clickedBooking.Id == Booking.RepeatingBookingId)
                    MessageBox.Show($"Enquiry is a Repeat Marker only. You need to confirm the marker from the calendar before you can open it.");
                else
                    _bookingsFrm.ShowBooking(BookingBL.GetBookingFull(clickedBooking.Id), _bookingId);
            }
        }

        private void Box_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right || !_isSetTimeMode || e.X < _timeScale.First().Key || e.X > _timeScale.Last().Key)
                return;

            if (_isSettingTime)
            {
                MessageBox.Show("Unexpected state during setting time");
                return;
            }

            try
            {
                _setTimeStartX = _timeScale.OrderBy(x => Math.Abs(x.Key - e.X)).First().Key;
                _lastX = _setTimeStartX;
                Time = _timeScale[_setTimeStartX].Time;
                _isSettingTime = true;
                Duration = 15;
                DoRedraw();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ParentForm, "Failed to capture time slot", ex, true);
                _isSetTimeMode = _isSettingTime = false;
            }
        }

        private void Box_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OpenOtherBooking(Box.PointToClient(MousePosition).X);
        }

        private void Box_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isSettingTime || e.X <= _setTimeStartX)
                return;

            var currentX = _timeScale.OrderBy(x => Math.Abs(x.Key - e.X)).First().Key;
            if (currentX != _lastX)
            {
                Duration = _timeScale[currentX].Mins - _timeScale[_setTimeStartX].Mins;
                DoRedraw();
            }
            _lastX = currentX;

            //if (!timeTmr.Enabled)
            //{
            //    timeTmr.Enabled = true;
            //    timeTmr.Start();
            //}
        }

        private void Box_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isSettingTime)
            {
                _isSetTimeMode = _isSettingTime = false;
                _owner.SetTime(Time, Duration);
                SetTravelTimesAndRedraw();
            }
        }

        private void travelTmr_Tick(object sender, EventArgs e)
        {
            travelTmr.Stop();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            _contextX = Box.PointToClient(MousePosition).X;
        }

        private void travelAndRedrawTmr_Tick(object sender, EventArgs e)
        {
            travelTmr.Start();
            DoRedraw();
            travelAndRedrawTmr.Stop();
        }
    }
}
