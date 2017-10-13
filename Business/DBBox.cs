using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon;
using TBRBooker.Model.Entities;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;

namespace TBRBooker.Business
{
    public class DBBox
    {

        private static AmazonDynamoDBClient DbClient;

        private static AmazonDynamoDBClient GetDynamoDBClient()
        {
            if (DbClient != null)
            {
                return DbClient;
            }
            else
            {
                var accessKey = Config.Lookup("aws-access-key");
                var secretKey = Config.Lookup("aws-secret-key");
                var credentials = new BasicAWSCredentials(
                accessKey: accessKey,
                secretKey: secretKey);
                var dbConfig = new AmazonDynamoDBConfig
                {
                    RegionEndpoint = RegionEndpoint.APSoutheast2
                };
                return new AmazonDynamoDBClient(credentials, dbConfig);
            }
        }

        public static void EndSession()
        {
            if (DbClient != null)
            {
                DbClient.Dispose();
                DbClient = null;
            }
        }

        public static void WriteItem(BaseItem itm)
        {
            var client = GetDynamoDBClient();
            client.PutItem(
                tableName: itm.TableName,
                item: itm.WriteAttributes()
            );
        }

        //public static List<T> GetItemsLike<T>(string filterText) where T : BaseItem
        //{
        //    AmazonDynamoDBClient client = GetDynamoDBClient();

        //    var sampleObj = Activator.CreateInstance<T>();

        //    var request = new QueryRequest
        //    {
        //        TableName = sampleObj.TableName,
        //        F
        //    };

        //    var response = client.Scan(request);
        //    var result = response.ScanResult;

        //    foreach (Dictionary<string, AttributeValue> item in response.ScanResult.Items)
        //    {
        //        // Process the result.
        //        PrintItem(item);
        //    }
        //}

        public static T ReadItem<T>(string id) where T: BaseItem
        {
            AmazonDynamoDBClient client = GetDynamoDBClient();

            var item = Activator.CreateInstance<T>();
            Table table = Table.LoadTable(client, item.TableName);
            GetItemOperationConfig config = new GetItemOperationConfig()
            {
                AttributesToGet = item.GetReadAttributes(),
            };
            Document doc = table.GetItem(id, config);
            return DocToItem<T>(doc);
        }

        private static List<T> SearchToItems<T>(Search search) where T: BaseItem
        {
            var items = new List<T>();

            List<Document> documentList = new List<Document>();
            do
            {
                foreach (var doc in search.GetNextSet())
                {
                    items.Add(DocToItem<T>(doc));
                }
            } while (!search.IsDone);

            return items;
        }

        private static T DocToItem<T>(Document doc) where T: BaseItem
        {
            var item = JsonConvert.DeserializeObject<T>(doc["json"]);
            item.LoadAttributes(doc);
            return item;
        }

        public static List<Booking> GetBookingsForCalendar()
        {
            AmazonDynamoDBClient client = GetDynamoDBClient();
            
            Table table = Table.LoadTable(client, Booking.TABLE_NAME);
            var filter = new ScanFilter();
            filter.AddCondition("isOpen", ScanOperator.Equal, Booking.IS_OPEN_STR);
            var search = table.Scan(filter);

            return SearchToItems<Booking>(search);
        }

        //public static void CleanupBookings()
        //{
        //    var client = GetDynamoDBClient();
        //    var atts = new Dictionary<string, AttributeValueUpdate>();
        //    AddAttribute(atts, "id", new AttributeValueUpdate { AttributeAction.DELETE}, Id);
        //    client.PutItem(
        //        tableName: itm.TableName,
        //        item: itm.WriteAttributes()
        //    );
        //}

        private class BaseItemFactory<T> where T : new()
        {
            protected T CreateItem()
            {
                return new T();
            }
        }

    }
}
