using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBRBooker.Base;
using TBRBooker.Model.Enums;

namespace TBRBooker.Model.Entities
{
    public class Penalty : BaseItem
    {
        public const string TABLE_NAME = "penalty";

        public Penalty()
        {
            TableName = TABLE_NAME;
        }

        public override bool IsCacheItems()
        {
            return true;
        }

        [JsonIgnore]
        private bool IsPendingInDatabase { get; set; }

        public Penalties PenaltyType { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime RelevantDate { get; set; }

        public DateTime DateAbsolved { get; set; }

        public int Value { get; set; }

        public int AbsolvedValue { get; set; }

        /// <summary>
        /// eg booking number
        /// </summary>
        public string Description { get; set; }

        public override Document GetAddUpdateDoc()
        {
            var doc = base.GetAddUpdateDoc();

            //for CalendarItemDTO
            doc["isPending"] = DateAbsolved == DateTime.MinValue ? Constants.DbBoolFilter : "";

            return doc;
        }

        public override List<string> GetReadAttributes()
        {
            var atts = base.GetReadAttributes();
            atts.Add("isPending");
            //other attributes are redundantly stored in json
            return atts;
        }

        public override void LoadAttributes(Document doc)
        {
            if (doc.ContainsKey("isPending"))
                IsPendingInDatabase = doc["isPending"].AsString().Equals(Constants.DbBoolFilter);
            else
                IsPendingInDatabase = false;
        }
    }
}
