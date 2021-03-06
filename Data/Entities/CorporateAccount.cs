﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;

namespace TBRBooker.Model.Entities
{
    /// <summary>
    /// this is not finished until I've seen what is utilised by QB API
    /// Might need another table that is CorporateAccountId + bookingId with partitionkey on CorporateAccountId?
    /// </summary>
    public class CorporateAccount : BaseItem
    {

        public const string TABLE_NAME = "corporate_account";

        public CorporateAccount()
        {
            TableName = TABLE_NAME;
            BookingIds = new List<string>();
          //  Branches = new List<string>();
        }

        /// <summary>
        /// Typically only one branch, for example Browns Brisbane and Browns Gold Coast are probably separate accounts with different billing details
        /// </summary>
       // public List<(string BranchName, string BookingNickname)> Branches { get; set; }

        public string TradingName { get; set; }
        public string BusinessName { get; set; }
        public string Abn { get; set; }
        public string BillingAddress { get; set; }
        public string BillingContact { get; set; }
        public string BillingEmail { get; set; }
        public string PhoneNumber { get; set; }
        public string OtherNumbers { get; set; }
        public string SpecialArrangements { get; set; }
        public string Notes { get; set; }

        /// <summary>
        /// Any new booking is automatically based off this booking
        /// </summary>
        public string DefaultBookingId { get; set; }

        /// <summary>
        /// Needed because so db can be queried to extract each booking, as opposed to full table scan
        /// </summary>
        public List<string> BookingIds { get; set; }

        public override bool IsCacheItems()
        {
            return true;
        }

        public override Document GetAddUpdateDoc()
        {
            var doc = base.GetAddUpdateDoc();

            doc["tradingName"] = TradingName;

            return doc;
        }

            //public const string SmartNameJoiner = " from ";

            public string SmartName()
        {
            string name = BusinessName;
            //if (!string.IsNullOrEmpty(BillingContact))
            //{
            //    name = BillingContact + SmartNameJoiner;
            //}
            return name;
        }
    }
}
