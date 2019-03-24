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
            if (IsForcedToTestMode())
                _instance = new Settings(true, 1, 0, false, "C:\\Programming\\TBR Booker Instance",
                    3, 0.98M, DayOfWeek.Wednesday, DayOfWeek.Monday, 7,
                    "sarahjane@truebluereptiles.com.au",
                    "#23A8EF", "#FFE0C0", "");
            else
                _instance = new Settings(false, 1, 0, false, "G:\\My Drive\\Bookings\\TBR Booker",
                    3, 0.98M, DayOfWeek.Wednesday, DayOfWeek.Monday, 7,
                    "sarahjane@truebluereptiles.com.au",
                     "#23A8EF", "#FFE0C0", "");
            return _instance;
        }

        public static void SetInst(Settings inst)
        {
            _instance = inst;
        }

        // ADDING A NEW SETTING:
        // -add to Clone function
        // -default in full constructor
        // -have a nice day

        [Category("Important")]
        public string Username { get; set; }

        [Category("Important")]
        [DisplayName("Test Mode (separate 'muck-around' database)")]
        public bool IsTestMode { get; set; }

        [Category("Important")]
        [DisplayName("Root Directory for Config and Save Files")]
        public string WorkingDir { get; set; }
        [Category("Important")]
        [DisplayName("Google API Key")]
        public string GoogleAPIKey { get; set; }

        [Category("Interface")]
        [DisplayName("Screen Id for Calendar (0 or 1)")]
        public int MainScreenDefaultId { get; set; }
        [Category("Interface")]
        [DisplayName("Screen Id for Booking Forms (0 or 1)")]
        public int BookingsScreenId { get; set; }
        [Category("Interface")]
        [DisplayName("Use 24 Hour Time")]
        public bool Is24HourTime { get; set; }
        [Category("Interface")]
        [DisplayName("Main UI Colour (google 'hex colour picker')")]
        public string MainColour { get; set; }
        [Category("Interface")]
        [DisplayName("Contrast UI Colour")]
        public string ContrastColour { get; set; }

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
        [DisplayName("Confirmation Call Day")]
        public DayOfWeek FollowupDay { get; set; }

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
            DayOfWeek confirmationCallDay, DayOfWeek followupDay,
            int daysBeforeOverdue, string calendarAccount,
            string mainColour, string contrastColour, string apiKey)
        {
            IsTestMode = isTestMode;
            MainScreenDefaultId = mainScreenId;
            BookingsScreenId = bookingsScreenId;
            Is24HourTime = is24HourTime;
            WorkingDir = saveFilesPath;
            MonthsForBookingHistory = monthsForBookingHistory;
            CreditCardMultiplier = creditCardMultiplier;
            ConfirmationCallDay = confirmationCallDay;
            FollowupDay = followupDay;
            DaysBeforeOverdue = daysBeforeOverdue;
            GmailCalendarAccount = calendarAccount;
            MainColour = mainColour;
            ContrastColour = contrastColour;
            GoogleAPIKey = apiKey;
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
                FollowupDay = FollowupDay,
                DaysBeforeOverdue = DaysBeforeOverdue,
                GmailCalendarAccount = GmailCalendarAccount,
                CreditCardMultiplier = CreditCardMultiplier,
                MainColour = MainColour,
                ContrastColour = ContrastColour,
                GoogleAPIKey = GoogleAPIKey
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
