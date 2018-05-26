using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBRBooker.Base
{
    public class Utils
    {

        public static (int Hour, int Minute) ParseTime(int time)
        {
            int hour = 0;
            int min = 0;
            if (time > 2399)
                throw new Exception("Unsupported Booking Time (max 2399): " + time);
            else if (time < 0)
                throw new Exception("Unsupported Booking Time (min 0): " + time);
            if (time >= 100)
            {
                hour = time / 100;
                min = time - (time / 100 * 100);
            }
            else
            {
                min = time;
            }

            return (hour, min);
        }

        public static int TimeInt(DateTime time)
        {
            string min = time.Minute.ToString();
            if (time.Minute < 10)
                min = "0" + min;
            return int.Parse($"{time.Hour}{min}");
        }

        public static string DisplayHour(int hour)
        {
            if (Settings.Inst().Is24HourTime || hour <= 12)
                return hour.ToString();
            else
                return (hour - 12).ToString();
        }

        public static string DisplayTime(int time)
        {
            return DisplayTime(ParseTime(time));
        }

        public static string DisplayTime((int Hour, int Minute) parsed)
        {
            var amPm = Settings.Inst().Is24HourTime ? "" : (parsed.Hour >= 12 ? " PM" : " AM");
            return $"{DisplayHour(parsed.Hour)}:{(parsed.Minute == 0 ? "00" : ((parsed.Minute < 10 ? "0" : "") + parsed.Minute.ToString()))}{amPm}";
        }

        public static string DurationToDisplayStr(int minutes)
        {
            string display = "";
            int hours = minutes / 60;
            int min = minutes % 60;
            if (hours > 0)
                display = hours.ToString() + " " + (hours > 1 ? "hours" : "hour");
            if (min > 0)
                display += " " + min + " min";
            return display.Trim();
        }

        public static int MinuteDifference(int time1, int time2)
        {
            var time1Parsed = ParseTime(time1);
            var time2Parsed = ParseTime(time2);

            int diff = (time2Parsed.Hour - time1Parsed.Hour) * 60;
            diff += time2Parsed.Minute - time1Parsed.Minute;
            return diff;
        }

        public static int MinuteDifference(DateTime d1, DateTime d2)
        {
            var ts = new TimeSpan(Math.Max(d1.Ticks, d2.Ticks)
                - Math.Min(d1.Ticks, d2.Ticks));
            return ts.Hours * 60 + ts.Minutes;
        }

        public static int EndTime(int startTime, int duration)
        {
            var parsed = Utils.ParseTime(startTime);
            var ts = new TimeSpan(parsed.Hour, parsed.Minute, 0);
            return int.Parse(ts.Add(new TimeSpan(0, duration, 0)).ToString("hhmm"));
        }

        public static DateTime StartOfDay(DateTime? dayIfNotForToday = null)
        {
            DateTime date = dayIfNotForToday.HasValue ? dayIfNotForToday.Value : DateTime.Now;
            return new DateTime(date.Year, date.Month, date.Day);
        }
    }
}
