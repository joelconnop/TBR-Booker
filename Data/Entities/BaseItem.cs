using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBRBooker.Model.Entities
{
    public abstract class BaseItem
    {
        public readonly string TableName;

        public string Id;

        public abstract Dictionary<string, AttributeValue> WriteAttributes();

        public void SetIdIfNeeded()
        {
            if (string.IsNullOrEmpty(Id))
                Id = Guid.NewGuid().ToString();
        }

        public void AddAttribute(Dictionary<string, AttributeValue> atts, string key, AttributeValue value, string strValue)
        {
            if (!string.IsNullOrEmpty(strValue))
            {
                atts.Add(key, value);
            }
        }

        public BaseItem(string tableName)
        {
            TableName = tableName;
        }
    }
}
