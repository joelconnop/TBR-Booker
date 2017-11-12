using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
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

    }
}
