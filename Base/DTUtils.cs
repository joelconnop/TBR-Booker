﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBRBooker.Base
{
    public class DTUtils
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

        public static int AddTimeInts(int startTime, int duration)
        {
            var parsed = DTUtils.ParseTime(startTime);
            var ts = new TimeSpan(parsed.Hour, parsed.Minute, 0);
            return int.Parse(ts.Add(new TimeSpan(0, duration, 0)).ToString("hhmm"));
        }

        public static DateTime DateTimeFromInt(DateTime day, int time, int duration = 0)
        {
            if (duration > 0)
                time = AddTimeInts(time, duration);

            var parsed = DTUtils.ParseTime(time);

            return StartOfDay(day).AddHours(parsed.Hour).AddMinutes(parsed.Minute);
        }

        public static DateTime StartOfDay(DateTime? dayIfNotForToday = null)
        {
            DateTime date = dayIfNotForToday.HasValue ? dayIfNotForToday.Value : DateTime.Now;
            return new DateTime(date.Year, date.Month, date.Day);
        }

        public static DateTime EndOfDay(DateTime? dayIfNotForToday = null)
        {
            DateTime date = StartOfDay(dayIfNotForToday);
            return new DateTime(date.Year, date.Month, date.Day).AddDays(1).AddHours(-1);
        }

        public static DateTime StartOfMonth(DateTime? dayIfNotForToday = null)
        {
            DateTime date = StartOfDay(dayIfNotForToday);
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime EndOfMonth(DateTime? dayIfNotForToday = null)
        {
            var date = StartOfMonth(dayIfNotForToday);
            return date.AddMonths(1).AddHours(-1);
        }

        public static DateTime Min(DateTime dt1, DateTime dt2)
        {
            if (dt1.Ticks < dt2.Ticks)
                return dt1;
            else
                return dt2;
        }

        public static DateTime Max(DateTime dt1, DateTime dt2)
        {
            if (dt1.Ticks > dt2.Ticks)
                return dt1;
            else
                return dt2;
        }

        public static int WeekOfMonthForDay(DateTime dt)
        {
            return Convert.ToInt32(Math.Ceiling(dt.Day / 7.0));
        }

        public static bool SameDay(DateTime dt1, DateTime dt2)
        {
            return StartOfDay(dt1) == StartOfDay(dt2);
        }

    }
}