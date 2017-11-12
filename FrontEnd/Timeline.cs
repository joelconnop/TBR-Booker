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
using Base;
using TBRBooker.Business;
using System.Drawing.Text;

namespace TBRBooker.FrontEnd
{
    public partial class Timeline : UserControl
    {

        private Bitmap _pic;

        private Booking _booking;
        private List<Booking> _others;

        private const int _inset = 5;
        private const int _insetGraphY = 20;
        private Point _lastClick;
        private Booking _clickedBooking;
        private Font _font = new Font("Microsoft Sans Serif", 8);

        public Timeline(Booking booking, List<Booking> others)
        {
            InitializeComponent();

            _booking = booking;
            _others = others;
            //_others = DBBox.GetItemCache()[Booking.TABLE_NAME]
            //    .Where(x as Booking => 

            _pic = new Bitmap(Box.Size.Width, Box.Size.Height);
            Box.Image = _pic;
        }

        private void Timeline_Load(object sender, EventArgs e)
        {
            Redraw(_booking, _others);
        }



        public void Redraw(Booking booking, List<Booking> others)
        {
            _booking = booking;
            _others = others;

            var g = Graphics.FromImage(_pic);

            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            //_others.ForEach(x => DrawBooking(g, x));
            //DrawBooking(g, booking);


            DrawTimeline(g);

            g.Dispose();
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
                hourScale += hourScale * parsed.Hour;
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

        private void DrawTimeline(Graphics g)
        {
            int lastx = 0;
            var height = Box.Height - 2 * _insetGraphY;
            
            for (int i = 1; i <= 2399; i++)
            {
                //the loop isn't great; need to skip numbers that aren't legit minutes
                if (i - (i / 100 * 100) >= 60)
                    continue;

                var x = GetTimeX(i);
                if (x == lastx)
                    continue;   //ie out of hours can be the same number again
                g.FillRectangle(GetBrush(i), 
                    new Rectangle(new Point(lastx, _insetGraphY), new Size(x - lastx, height)));
                lastx = x;

                //15 min notch
                var parsed = Utils.ParseTime(i);
                if (parsed.Minute % 15 == 0 && parsed.Hour >= 9 && parsed.Hour <= 17
                    && (parsed.Hour < 17 || parsed.Minute == 0))
                {
                    g.DrawLine(Pens.Black, 
                        new Point(x, Box.Height - _insetGraphY), 
                        new Point(x, Box.Height - _insetGraphY / 2));
                    if (parsed.Minute == 0)
                    {
                        string hourMark = parsed.Hour + ":00";
                        var drawStr = g.MeasureString(hourMark, _font, 50);
                        g.DrawString(hourMark, _font, Brushes.Black,
                            new PointF(x - drawStr.Width / 2, _inset));
                    }
                }
            }
        }

        private Brush GetBrush(int time)
        {
            var bookings = BookingBL.GetClashBookings(_others, time, time);

            var isThisBooking = BookingBL.GetClashBookings(
                new List<Booking> { _booking }, time, time).Count == 1;

            if (isThisBooking && bookings.Count > 0)
            {
                return bookings.Any(x => x.IsBooked()) ? Brushes.Red : Brushes.IndianRed;
            }

            if (isThisBooking)
                return Brushes.Green;

            if (bookings.Count > 0)
            {
                if (bookings.Count > 1)
                {
                    return bookings.All(x => x.IsBooked()) ? Brushes.Red : Brushes.IndianRed;
                }
                return bookings[0].IsBooked() ? Brushes.MediumBlue : Brushes.DimGray;
            }

            return Brushes.White;
        }

        private void Box_Click(object sender, EventArgs e)
        {
            _lastClick = MousePosition;
            //detect booking click!
            _clickedBooking = null;
            if (_clickedBooking == _booking)
                _clickedBooking = null; //meaningless to click the booking already on this form
        }

        private void contextMenuStrip1_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            _clickedBooking = null;
        }

        private void setTimeMnu_Click(object sender, EventArgs e)
        {

        }

        private void openBookingMnu_Click(object sender, EventArgs e)
        {
            //more like, this frm should have a reference to the master booking frm,
            //and add/show a booking tab to it
            var bookingFrm = new BookingFrm(_clickedBooking);
            bookingFrm.Show();
        }


    }
}
