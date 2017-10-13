﻿using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Amazon.DynamoDBv2.DocumentModel;

namespace TBRBooker.Model.Entities
{
    public abstract class BaseItem
    {
        public string TableName { get; set; }
        //private readonly Type _filterType;

        public string Id { get; set; }
        public Object Filter { get; set; }
        public string FilterName { get; set; }


        public virtual Dictionary<string, AttributeValue> WriteAttributes()
        {
            var atts = new Dictionary<string, AttributeValue>();

            SetIdIfNeeded();
            AddAttribute(atts, "id", new AttributeValue { S = Id }, Id);
            if (!string.IsNullOrEmpty(FilterName))
            {
                AddAttribute(atts, FilterName, new AttributeValue { S = Filter.ToString() }, Filter.ToString());
                throw new Exception("this should be INSTEAD of an Id, right? see https://aws.amazon.com/blogs/database/choosing-the-right-dynamodb-partition-key/");
            }
            var json = JsonConvert.SerializeObject(this);
            AddAttribute(atts, "json", new AttributeValue { S = json }, json);

            return atts;
        }

        public virtual UpdateItemRequest GetUpdateRequest()
        {
            var request = new UpdateItemRequest
            {
                TableName = this.TableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    {"id", new AttributeValue{S = Id}}
                },
                UpdateExpression = "SET #j = :jsonval",
                ExpressionAttributeNames = new Dictionary<string, string>
                {
                    {"#j", "json"},
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":jsonval", new AttributeValue{S = JsonConvert.SerializeObject(this)}},
                }
            };

            if (!string.IsNullOrEmpty(FilterName))
            {
                request.UpdateExpression += ", #f = :filter";
                request.ExpressionAttributeNames.Add("#f", FilterName);
                request.ExpressionAttributeValues.Add("filter", new AttributeValue { S = Filter.ToString() });
            }

            return request;
        }

        public void SetIdIfNeeded()
        {
            if (string.IsNullOrEmpty(Id))
                Id = DateTime.Now.ToShortDateString() + "-" + Guid.NewGuid().ToString();
        }

        public void AddAttribute(Dictionary<string, AttributeValue> atts, string key, AttributeValue value, string strValue)
        {
            if (!string.IsNullOrEmpty(strValue))
            {
                atts.Add(key, value);
            }
        }

        public virtual List<string> GetReadAttributes()
        {
            return new List<string>() { "json" };
        }

        public virtual void LoadAttributes(Document doc)
        {
            //json is loaded from DBBox, and pretty much all attributes are loaded from json. Only override if something is missing or out of date in json.
        }

        public BaseItem()
        {
            //for deserialisation
        }

        //public BaseItem(string tableName, string filterName = null) //, Type filterType = null)
        //{
        //    TableName = tableName;
        //    FilterName = filterName;
        // //   _filterType = filterType;
        //}

    }
}
