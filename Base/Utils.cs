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
            var parsed = ParseTime(time);
            var amPm = Settings.Inst().Is24HourTime ? "" : (parsed.Hour >= 12 ? " PM" : " AM");
            return $"{DisplayHour(parsed.Hour)}:{(parsed.Minute == 0 ? "00" : parsed.Minute.ToString())}{amPm}"; 
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

    }
}
