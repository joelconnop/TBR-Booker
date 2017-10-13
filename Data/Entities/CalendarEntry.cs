//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Amazon.DynamoDBv2.Model;
//using TBRBooker.Model.Enums;

//namespace TBRBooker.Model.Entities
//{

//    //sounds a lot like a DTO. Don't really need in database if isOpen filter works

//    public class CalendarEntry : BaseItem
//    {
//        public string BookingId { get; set; }
//        public DateTime BookingDate { get; set; }
//        public int BookingTime { get; set; }

//        /// <summary>
//        /// in case BookingTime is not set
//        /// </summary>
//        public TimeSlots TimeSlot { get; set; }

//        public string CustomerName { get; set; }


//        public CalendarEntry(string tableName) : base("calendar_entry")
//        {
//        }

//        //public override Dictionary<string, AttributeValue> WriteAttributes()
//        //{
//        //    throw new NotImplementedException();
//        //}
//    }
//}
