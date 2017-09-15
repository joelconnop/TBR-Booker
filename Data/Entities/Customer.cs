using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;

namespace TBRBooker.Model.Entities
{
    public class Customer : BaseItem
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string OtherNumbers { get; set; }
        public string EmailAddress { get; set; }
        public string CompanyId { get; set; }

        public Customer() : base("customer")
        {
        }

        public override Dictionary<string, AttributeValue> WriteAttributes()
        {
            var atts = new Dictionary<string, AttributeValue>();
            SetIdIfNeeded();
            AddAttribute(atts, "id", new AttributeValue { S = Id }, Id);
            AddAttribute(atts, "firstName", new AttributeValue { S = FirstName }, FirstName);
            AddAttribute(atts, "lastName", new AttributeValue { S = LastName }, LastName);
            AddAttribute(atts, "mobileNumber", new AttributeValue { S = MobileNumber },MobileNumber);
            AddAttribute(atts, "otherNumbers", new AttributeValue { S = OtherNumbers }, OtherNumbers);
            AddAttribute(atts, "emailAddress", new AttributeValue { S = EmailAddress }, EmailAddress);
            return atts;
        }
    }
}
