using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;

namespace TBRBooker.Base
{
    public class Settings : ICloneable
    {

        public const string EnvironmentVarsRoot = "HKEY_CURRENT_USER\\Software\\TBR Booker";
        public const string UserKey = "Username";
        public const string WorkingDirKey = "WorkingDir";

        private static Settings _instance;

        public static Settings Inst()
        {
            return _instance;
        }

        public static Settings CreateDefaultInst()
        {
                //read from config instead!
                //could also make config into a property grid class, and read/save as xml file
                if (IsForcedToTestMode())
                    _instance = new Settings(true, 1, 0, false, "C:\\Programming\\TBR Booker Instance", 
                        3, 0.98M, DayOfWeek.Wednesday, 7, "sarahjane@truebluereptiles.com.au");
                else
                    _instance = new Settings(false, 1, 0, false, "G:\\My Drive\\Bookings\\TBR Booker", 
                        3, 0.98M, DayOfWeek.Wednesday, 7, "sarahjane@truebluereptiles.com.au");
            return _instance;
        }

        public static void SetInst(Settings inst)
        {
            _instance = inst;
        }

        public string Username { get; set; }

        [DisplayName("Test Mode (separate 'muck-around' database)")]
        public bool IsTestMode { get; set; }

        [DisplayName("Root Directory for Config and Save Files")]
        public string WorkingDir { get; set; }

        [DisplayName("Screen Id for Calendar (0 or 1)")]
        public int MainScreenDefaultId { get; set; }

        [DisplayName("Screen Id for Booking Forms (0 or 1)")]
        public int BookingsScreenId { get; set; }

        [DisplayName("Use 24 Hour Time")]
        public bool Is24HourTime { get; set; }

        [Category("Hardcoded Settings")]
        [ReadOnly(true)]
        [DisplayName("Months for Booking History")]
        public int MonthsForBookingHistory { get; set; }

        [Category("Hardcoded Settings")]
        [ReadOnly(true)]
        [DisplayName("Confirmation Call Day")]
        public DayOfWeek ConfirmationCallDay { get; set; }

        [Category("Hardcoded Settings")]
        [ReadOnly(true)]
        [DisplayName("Days Before Overdue")]
        public int DaysBeforeOverdue { get; set; }

        [Category("Hardcoded Settings")]
        [ReadOnly(true)]
        [DisplayName("Gmail Calendar Account")]
        public string GmailCalendarAccount { get; set; }

        //Idea: multiply CC payments by this for how much is submitted, then do another calc for determining if fully paid
        [Category("Hardcoded Settings")]
        [ReadOnly(true)]
        //[DisplayName("Credit Card Multiplier (not in use)")]
        [Browsable(false)]
        public decimal CreditCardMultiplier;
        
        public Settings()
        {
            //default constructor
        }

        public Settings(bool isTestMode, int mainScreenId, int bookingsScreenId, bool is24HourTime, string saveFilesPath,
            int monthsForBookingHistory, decimal creditCardMultiplier, 
            DayOfWeek confirmationCallDay, int daysBeforeOverdue, string calendarAccount)
        {
            IsTestMode = isTestMode;
            MainScreenDefaultId = mainScreenId;
            BookingsScreenId = bookingsScreenId;
            Is24HourTime = is24HourTime;
            WorkingDir = saveFilesPath;
            MonthsForBookingHistory = monthsForBookingHistory;
            CreditCardMultiplier = creditCardMultiplier;
            ConfirmationCallDay = confirmationCallDay;
            DaysBeforeOverdue = daysBeforeOverdue;
            GmailCalendarAccount = calendarAccount;
        }

        public Object Clone()
        {
            return new Settings()
            {
                Username = Username,
                IsTestMode = IsTestMode,
                WorkingDir = WorkingDir,
                MainScreenDefaultId = MainScreenDefaultId,
                BookingsScreenId = BookingsScreenId,
                Is24HourTime = Is24HourTime,
                MonthsForBookingHistory = MonthsForBookingHistory,
                ConfirmationCallDay = ConfirmationCallDay,
                DaysBeforeOverdue = DaysBeforeOverdue,
                GmailCalendarAccount = GmailCalendarAccount,
                CreditCardMultiplier = CreditCardMultiplier
            };
        }

        public static bool IsForcedToTestMode()
        {
#if DEBUG
            return true;
#else
                string testAssemblyName = "Microsoft.VisualStudio.TestPlatform.TestFramework";
           return AppDomain.CurrentDomain.GetAssemblies()
                .Any(a => a.FullName.StartsWith(testAssemblyName));
#endif

        }

    }
}
