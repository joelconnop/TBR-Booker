using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBRBooker.Model.Entities
{
    public class RepeatSchedule : BaseItem
    {
        public const string TABLE_NAME = "repeat_schedule";

        // The ID is the Repeat Code, create list of them here
        public const string BeardedDragonId = "BeardedDragon";
        public const string LittleNatureLovers = "LittleNatureLovers";

        public RepeatSchedule()
        {
            TableName = TABLE_NAME;
        }

        public string CustomerId { get; set; }

        public DateTime StartDate { get; set; }

        public List<(DateTime Date, string Reason)> Cancellations { get; set; }
 
        public DayOfWeek RepeatDay { get; set; }
        public bool IsByDayOfWeek { get; set; }
        
        /// <summary>
        /// use 1 for "every week", 2 for "every fortnight" etc
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        /// 0 = not applicable. 2 = "2nd Friday of very month"
        /// (and set Frequency = 1, or 2 if its 'every 2nd month' etc)
        /// </summary>
        public int WeekNumOfEveryMonth { get; set; }

        public override bool IsCacheItems()
        {
            return true;
        }

        public override List<string> GetReadAttributes()
        {
            return new List<string>() { "id", "json" };
        }
    }
}
