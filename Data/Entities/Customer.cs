using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;

namespace TBRBooker.Model.Entities
{
    public class Customer : BaseItem
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        /// <summary>
        /// Good for 'one off' companies that don't need a corporate account
        /// </summary>
        public string CompanyName { get; set; }

        public string MobileNumber { get; set; }
        public string OtherNumbers { get; set; }
        public string EmailAddress { get; set; }
        public string CompanyId { get; set; }
        public DateTime CreatedDate { get; set; }

        [JsonIgnore]
        public CorporateAccount Company { get; set; }

        public Customer()
        {
            TableName = "customer";
        }

        //public Customer() : base("customer")    //, "smartName")
        //{
        //}

        public override Dictionary<string, AttributeValue> WriteAttributes()
        {
            //Filter = SmartName();
            var atts = base.WriteAttributes();
            AddAttribute(atts, "smartName", new AttributeValue { S = SmartName() }, SmartName());
            return atts;
        }

        public string SmartName()
        {
            string companyName = "";
            if (Company != null)
            {
                companyName = Company.SmartName();
                //if (companyName.Contains(CorporateAccount.SmartNameJoiner))
                //    return companyName;
            }
            else
            {
                companyName = CompanyName;
            }

            string name = FirstName;
            if (!string.IsNullOrEmpty(LastName))
            {
                if (!string.IsNullOrEmpty(FirstName))
                {
                    name += " ";
                }
                name += LastName;
            }

            if (!string.IsNullOrEmpty(companyName))
            {
                name += " from " + companyName;
            }

            return name;
        }
    }
}
