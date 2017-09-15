using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;

namespace TBRBooker.Model.Entities
{
    /// <summary>
    /// this is not finished until I've seen what is utilised by QB API
    /// </summary>
    public class CorporateAccount : BaseItem
    {
        public CorporateAccount(string tableName) : base("corporate_account")
        {
            Branches = new List<string>();
        }

        /// <summary>
        /// Typically only one branch, for example Browns Brisbane and Browns Gold Coast are probably separate accounts with different billing details
        /// </summary>
        public List<(string BranchName, string BookingNickname)> Branches { get; set; }

        public string CompanyName { get; set; }
        public string Abn { get; set; }
        public Address BillingAddress { get; set; }
        public string BillingContact { get; set; }
        public string BillingEmail { get; set; }
        public string PhoneNumber { get; set; }

        public override Dictionary<string, AttributeValue> WriteAttributes()
        {
            throw new NotImplementedException();
        }
    }
}
