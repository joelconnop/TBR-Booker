using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;
using Amazon.DynamoDBv2.DocumentModel;

namespace TBRBooker.Model.Entities
{
    public class Customer : BaseItem
    {
        public const string TABLE_NAME = "customer";

        public Customer()
        {
            TableName = "customer";
            BookingIds = new List<string>();
        }

        public override bool IsCacheItems()
        {
            return true;
        }


        public string FirstName { get; set; }
        public string LastName { get; set; }



        public string PrimaryNumber { get; set; }
        public string SecondaryNumber { get; set; }
        public string EmailAddress { get; set; }
        public string CompanyId { get; set; }
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// A little bit redundant with Booking.Account, but might need to later change Customer's account - but Booking's account is a historic value
        /// </summary>
        //[JsonIgnore]
        //public CorporateAccount Company { get; set; }

        /// <summary>
        /// Good for 'one off' companies that don't need a corporate account
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Needed because so db can be queried to extract each booking, as opposed to full table scan
        /// Also a (redundant) subset of CorporateAccount.BookingIds, where applicable
        public List<string> BookingIds { get; set; }


        //public Customer() : base("customer")    //, "directoryName")
        //{
        //}

        public override Document GetAddUpdateDoc()
        {
            var doc = base.GetAddUpdateDoc();

            doc["directoryName"] = DirectoryName();

            return doc;
        }

        public string DirectoryName()
        {
            //string companyName = "";
            //if (Company != null)
            //{
            //    companyName = Company.SmartName();
            //    //if (companyName.Contains(CorporateAccount.SmartNameJoiner))
            //    //    return companyName;
            //}
            //else
            //{
            //    companyName = CompanyName;
            //}

            string name = "";

            if (string.IsNullOrEmpty(LastName))
                name = FirstName;
            else if (!string.IsNullOrEmpty(FirstName))
                name = FirstName.Substring(0, 1) + ".";

            if (!string.IsNullOrEmpty(LastName))
            {
                if (!string.IsNullOrEmpty(FirstName))
                {
                    name += " ";
                }
                name += LastName;
            }

            if (!string.IsNullOrEmpty(CompanyName))
            {
                name += " from " + CompanyName;
            }

            if (!string.IsNullOrEmpty(EmailAddress))
            {
                name += "; " + EmailAddress;
            }
            if (!string.IsNullOrEmpty(PrimaryNumber))
            {
                name += "; " + PrimaryNumber;
            }
            if (!string.IsNullOrEmpty(SecondaryNumber))
            {
                name += "; " + SecondaryNumber;
            }


            return name;
        }
    }
}
