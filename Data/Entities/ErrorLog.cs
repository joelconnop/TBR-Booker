using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;

namespace TBRBooker.Model.Entities
{
    //Can't use DBBox.ReadItem on this class because Id is not the partition key 
    //(what good is reading one old error anyway)
    public class ErrorLog : BaseItem
    {
        public override bool IsCacheItems()
        {
            return false;
        }

        public const string TABLE_NAME = "error_log";

        public ErrorLog()
        {
            TableName = TABLE_NAME;
        }

        /// <summary>
        /// Action is actually the partition key for this table
        /// it means what we were doing when we got the error
        /// </summary>
        public string Action { get; set; }

        public DateTime CreatedDate { get; set; }
        public string ExceptionMsg { get; set; }
        public string StackTrace { get; set; }

        public override Document GetAddUpdateDoc()
        {
            var doc = base.GetAddUpdateDoc();
            doc["action"] = Action; //needed because its the partition key
            return doc;
        }
    }
}
