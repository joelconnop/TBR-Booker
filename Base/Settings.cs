using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBRBooker.Base
{
    public class Settings
    {
        private static Settings _instance;
        public static Settings Inst()
        {
            if (_instance == null)
            {
                //read from config instead!
                //could also make config into a property grid class, and read/save as xml file
                if (IsShouldBeTestMode())
                    _instance = new Settings(true, 1, 0, false, "C:\\Programming\\TBR Booker Instance", 
                        3, 0.98M, DayOfWeek.Wednesday, 7);
                else
                    _instance = new Settings(false, 1, 0, false, "G:\\My Drive\\Bookings\\TBR Booker", 
                        3, 0.98M, DayOfWeek.Wednesday, 7);
            }
            return _instance;
        }

        public readonly bool IsTestMode;
        public readonly int MainScreenDefaultId;
        public readonly int BookingsScreenId;
        public readonly bool Is24HourTime;
        public readonly string WorkingDir;
        public readonly int MonthsForBookingHistory;
        public readonly DayOfWeek ConfirmationCallDay;
        public readonly int DaysBeforeOverdue;

        //Idea: multiply CC payments by this for how much is submitted, then do another calc for determining if fully paid
        public readonly decimal CreditCardMultiplier;
        
        public Settings(bool isTestMode, int mainScreenId, int bookingsScreenId, bool is24HourTime, string saveFilesPath,
            int monthsForBookingHistory, decimal creditCardMultiplier, DayOfWeek confirmationCallDay,
            int daysBeforeOverdue)
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
        }

        private static bool IsShouldBeTestMode()
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
