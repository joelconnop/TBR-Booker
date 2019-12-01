using System;
using Microsoft.Win32;
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
                    "#23A8EF", "#FFE0C0", "", new DateTime(2019, 7, 1).Ticks);
            else
                _instance = new Settings(false, 1, 0, false, "G:\\My Drive\\Bookings\\TBR Booker",
                    3, 0.98M, DayOfWeek.Wednesday, DayOfWeek.Monday, 7,
                    "sarahjane@truebluereptiles.com.au",
                     "#23A8EF", "#FFE0C0", "", new DateTime(2019, 7, 1).Ticks);
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

        // note - the penalties do NOT include the last scan date (it is 'start of day')
        [Category("Hardcoded Settings")]
        [ReadOnly(true)]
        [Browsable(false)]
        public long LastScan;




        public Settings()
        {
            //default constructor
        }

        public Settings(bool isTestMode, int mainScreenId, int bookingsScreenId, bool is24HourTime, string saveFilesPath,
            int monthsForBookingHistory, decimal creditCardMultiplier, 
            DayOfWeek confirmationCallDay, DayOfWeek followupDay,
            int daysBeforeOverdue, string calendarAccount,
            string mainColour, string contrastColour, string apiKey, long lastScan)
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
            LastScan = lastScan;
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
                GoogleAPIKey = GoogleAPIKey,
                LastScan = LastScan,
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

        public static void PersistSettings(Settings newSettings, Settings oldSettings, bool skipRegistry)
        {
            if (string.IsNullOrEmpty(newSettings.Username))
            {
                throw new Exception("A Username is required to go any further.");
            }

            if (!Directory.Exists(newSettings.WorkingDir))
            {
                throw new Exception("A valid working directory is needed (preferrably a Google Drive Stream location) is needed. Example: G:\\My Drive\\Bookings\\TBR Booker");
            }

            if (!skipRegistry && oldSettings == null)
                throw new Exception("Cannot update registry, the old settings were mysteriously absent.");

            // don't touch registry entries in test or we will mess with production config
            if (!Settings.IsForcedToTestMode())
            {
                if (newSettings.Username != null && (oldSettings.Username == null
                || !oldSettings.Username.Equals(newSettings.Username)))
                    Registry.SetValue(Settings.EnvironmentVarsRoot, Settings.UserKey, newSettings.Username,
                        RegistryValueKind.String);
                if (!oldSettings.WorkingDir.Equals(newSettings.WorkingDir))
                    Registry.SetValue(Settings.EnvironmentVarsRoot, Settings.WorkingDirKey, newSettings.WorkingDir,
                        RegistryValueKind.String);
            }

            var settingsFilename = newSettings.WorkingDir + "\\config\\" + newSettings.Username + "_settings.json";
            try
            {
                using (StreamWriter file = File.CreateText(settingsFilename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, newSettings);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to write the settings to " + settingsFilename + " because: " + ex.Message, ex);
            }
        }

    }
}
