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
                _instance = new Settings(1, 0, false, "D:\\Google Drive\\Bookings");
            }
            return _instance;
        }
        
        public readonly int MainScreenDefaultId;
        public readonly int BookingsScreenId;
        public readonly bool Is24HourTime;
        public readonly string SaveFilesPath;
        
        public Settings(int mainScreenId, int bookingsScreenId, bool is24HourTime, string saveFilesPath)
        {
            MainScreenDefaultId = mainScreenId;
            BookingsScreenId = bookingsScreenId;
            Is24HourTime = is24HourTime;
            SaveFilesPath = saveFilesPath;
        }

    }
}
