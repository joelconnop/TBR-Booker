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

namespace TBRBooker.FrontEnd
{
    public partial class Timeline : UserControl
    {

        private Bitmap _pic;

        private BookingsFrm _bookingsFrm;
        private BookingPnl _owner;
        private string _bookingId;
        private List<Booking> _others;

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
        
        public DateTime BookingDate { get; set; }
        public int Time { get; set; }
        public int Duration { get; set; }


        public Timeline(Booking booking, BookingsFrm bookingsFrm, BookingPnl owner)
        {
            InitializeComponent();

            _bookingsFrm = bookingsFrm;
            _owner = owner;
            _bookingId = booking.Id;

            BookingDate = booking.BookingDate;
            Time = booking.BookingTime;
            Duration = booking.Duration;
            _isSettingTime = false;
            _isSetTimeMode = booking.Duration == 0;
            _timeScale = new Dictionary<int, (int Time, int Mins)>();

        }

        private void Timeline_Load(object sender, EventArgs e)
        {
            UpdateOtherBookings();
        }

        public void UpdateOtherBookings()
        {
            _others = new List<Booking>();
            foreach (var summary in DBBox.GetCalendarItems(false)
                .Where(x => x.BookingDate.ToShortDateString().Equals(BookingDate.ToShortDateString())
                && !x.BookingNum.ToString().Equals(_bookingId)))
            {
                _others.Add(DBBox.ReadItem<Booking>((summary.BookingNum.ToString())));
            }
            Redraw();
        }


        public void Redraw()
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

            var parsed = Utils.ParseTime(time);

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
                if (x == lastx)
                    continue;   //ie out of hours can be the same number again    

                var parsed = Utils.ParseTime(i);

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
                        string hourMark = Utils.DisplayHour(parsed.Hour) + ":00";
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
            var bookings = BookingBL.GetClashBookings(_others, time, time);

            //show summary for the last booking, once we have finished drawing its graph
            if (_lastBooking != null && (bookings.Count == 0 || bookings[0] != _lastBooking))
            {
                g.DrawString($"{_lastBooking.BookingNickname} - {_lastBooking.LocationRegion}",
                    _font, Brushes.Black, new PointF(timeX, _insetGraphY + 5));
                _lastBooking = null;
            }
            if (_lastBooking == null && bookings.Count > 0)
            {
                _lastBooking = bookings[0];
                _lastBookingStartX = timeX + 10;
            }

            var isThisBooking = Duration > 0 && BookingBL.GetClashBookings(new List<Booking> {
                new Booking() { BookingTime = Time, Duration = Duration } }, time, time).Count == 1;

            if (isThisBooking && bookings.Count > 0)
            {
                return bookings.Any(x => x.IsBooked()) ? Brushes.Red : Brushes.IndianRed;
            }

            if (isThisBooking)
                return Brushes.LimeGreen;

            if (bookings.Count > 0)
            {
                if (bookings.Count > 1)
                {
                    return bookings.All(x => x.IsBooked()) ? Brushes.Red : Brushes.IndianRed;
                }
                return bookings[0].IsBooked() ? new SolidBrush(Color.FromArgb(35, 168, 239)) : Brushes.DimGray;
            }

            return Brushes.White;
        }

        private void Box_Click(object sender, EventArgs e)
        {
            if (_isSettingTime)
                return;

            var clickedBooking = DetectClickedBooking();
            if (clickedBooking != null)
            {
                selectedLbl.Text = clickedBooking.Summary();
                selectedLbl.Left = MousePosition.X;
                selectedLbl.Visible = true;
            }
            else
            {
                selectedLbl.Visible = false;
            }
        }

        private Booking DetectClickedBooking()
        {
            var time = _timeScale.OrderBy(x => Math.Abs(x.Key - MousePosition.X)).First().Value.Time;
            return BookingBL.GetClashBookings(_others, time, time).FirstOrDefault();
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
            var clickedBooking = DetectClickedBooking();
            if (clickedBooking != null)
                _bookingsFrm.ShowBooking(BookingBL.GetBookingFull(clickedBooking.Id), _bookingId);
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
                Redraw();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(ParentForm, "Failed to capture time slot", ex, true);
                _isSetTimeMode = _isSettingTime = false;
            }
        }

        private void Box_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //open other booking
            MessageBox.Show("double clicked.");
        }

        private void Box_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isSettingTime || e.X <= _setTimeStartX)
                return;

            var currentX = _timeScale.OrderBy(x => Math.Abs(x.Key - e.X)).First().Key;
            if (currentX != _lastX)
            {
                Duration = _timeScale[currentX].Mins - _timeScale[_setTimeStartX].Mins;
                Redraw();
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
            }
        }

        private void timeTmr_Tick(object sender, EventArgs e)
        {
            Debug.WriteLine("Drawing!");
            Redraw();
            timeTmr.Stop();
            timeTmr.Enabled = false;
        }
    }
}
