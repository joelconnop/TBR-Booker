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
using Shouldly;
using TBRBooker.Model.DTO;
using TBRBooker.Model.Enums;

namespace TBRBooker.Business
{
    public class DBBox
    {

        private static Dictionary<string, Dictionary<string, BaseItem>> ItemCache;

        public static Dictionary<string, Dictionary<string, BaseItem>> GetFullItemCache()
        {
            if (ItemCache == null)
                ItemCache = new Dictionary<string, Dictionary<string, BaseItem>>();

            return ItemCache;
        }

        public static Dictionary<string, BaseItem> GetCachedItems(string tableName)
        {
            var cache = GetFullItemCache();
            if (!cache.ContainsKey(tableName))
                cache.Add(tableName, new Dictionary<string, BaseItem>());
            return cache[tableName];
        }


        private static AmazonDynamoDBClient DbClient;

        /// <summary>
        /// As this application is single thread, basically single user, not too database intensive,
        /// we can get away with making an ID from date/time and a 3 digit int. 
        /// Its easy enough to use a GUID, but we can get meaning for querying out of a date 
        /// and also want to keep IDs short for storage and performance at DynamoDB
        /// </summary>
        private static int ItemIdSalt;
        private static string GetUniqueId(BaseItem itm)
        {
            if (itm is Booking)
                return BookingBL.GetNextBookingNum();

            ItemIdSalt++;
            if (ItemIdSalt == 1 || ItemIdSalt >= 1000)
                //start the salt at 100; we will create 3 digit salts
                ItemIdSalt = 101;

            return $"{DateTime.Now.Ticks}-{DateTime.Now.ToShortDateString()}-{ItemIdSalt}";
        }



        //horrible practise but could be just the ticket for regression testing.
        //and real data can be copied to test data in AWS console, probably.
        public static bool IsTestEnvironment { get; private set; }
        private static bool IsCheckedTestEnvironment { get; set; }
        public void SetTestEnvironment(bool isTestEnv)
        {
            IsTestEnvironment = isTestEnv;
        }



        /// <summary>
        /// Not neccessary to call but sometimes want to
        /// </summary>
        public static void InitDynamoDbClient()
        {
            var client = GetDynamoDBClient();
        }

        private static AmazonDynamoDBClient GetDynamoDBClient()
        {
            if (!IsCheckedTestEnvironment && !IsTestEnvironment)
            {
                IsTestEnvironment = true;
                //string testAssemblyName = "Microsoft.VisualStudio.TestPlatform.TestFramework";
                //IsTestEnvironment = AppDomain.CurrentDomain.GetAssemblies()
                //    .Any(a => a.FullName.StartsWith(testAssemblyName));
                IsCheckedTestEnvironment = true;
            }

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

        private static string CheckTablenameForTest(string tableName)
        {
            string test = "test_";
            bool isTestTable = tableName.StartsWith(test);
            if (isTestTable && !IsTestEnvironment)
                //return tableName.Replace(test, "");   //use this if we decide we do need to hit here
                throw new Exception("Should not have encountered " + tableName + " outside of testing.");
            else if (!isTestTable && IsTestEnvironment)
                return test + tableName;
            else
                return tableName;
        }

        public static void AddOrUpdate(BaseItem itm)
        {
            AmazonDynamoDBClient client = GetDynamoDBClient();
            Table table = Table.LoadTable(client, CheckTablenameForTest(itm.TableName));

            if (string.IsNullOrEmpty(itm.Id))
            {
                itm.Id = GetUniqueId(itm);
                var doc = itm.GetAddUpdateDoc();
                var config = new PutItemOperationConfig()
                {
                    ConditionalExpression = new Expression()
                    {
                        ExpressionStatement = "attribute_not_exists(id)"
                    }
                };
                table.PutItem(doc, config);

                //now that the item is in database, it can go into cache as well
                if (itm.IsCacheItems())
                {
                    GetCachedItems(itm.TableName).Add(itm.Id, itm);
                }
            }
            else
            {
                var doc = itm.GetAddUpdateDoc();
                table.UpdateItem(doc);
            }

            if (itm is Booking)
            {
                var calendar = GetCalendarItems();
                var calendarItem = calendar.FirstOrDefault(x => x.BookingNum == int.Parse(itm.Id));
                if (calendarItem != null)
                    calendar.Remove(calendarItem);
                calendar.Add((itm as Booking).ToCalendarItem());
            }
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
            var temp = Activator.CreateInstance<T>();

            Dictionary<string, BaseItem> cache = null;
            if (temp.IsCacheItems())
            {
                cache = GetCachedItems(temp.TableName);
                if (cache.ContainsKey(id))
                    return (T)cache[id];
            }

            AmazonDynamoDBClient client = GetDynamoDBClient();          
            Table table = Table.LoadTable(client, CheckTablenameForTest(temp.TableName));
            GetItemOperationConfig config = new GetItemOperationConfig()
            {
                AttributesToGet = temp.GetReadAttributes(),
            };
            Document doc = table.GetItem(id, config);
            var item = DocToItem<T>(doc);

            if (temp.IsCacheItems() && item != null)
                cache.Add(id, item);

            return item;
        }

        private static List<T> SearchToItems<T>(Search search) where T: BaseItem
        {
            var items = new List<T>();

            List<Document> documentList = new List<Document>();
            do
            {
                foreach (var doc in search.GetNextSet())
                {
                    var itm = DocToItem<T>(doc);
                    if (itm != null)
                        items.Add(itm);
                }
            } while (!search.IsDone);

            return items;
        }

        private static T DocToItem<T>(Document doc) where T: BaseItem
        {
            if (doc == null)
                return null;

            var item = JsonConvert.DeserializeObject<T>(doc["json"]);
            item.LoadAttributes(doc);
            return item;
        }

        private static List<CalendarItemDTO> Calendar { get; set; }

        public static List<CalendarItemDTO> GetCalendarItems()
        {
            if (Calendar != null)
            {
                return Calendar;
            }

            Calendar = new List<CalendarItemDTO>();

            AmazonDynamoDBClient client = GetDynamoDBClient();
            var config = new QueryOperationConfig()
            {
                Select = SelectValues.SpecificAttributes,
                AttributesToGet = new List<string> { "id", "bookingName", "bookingDate", "bookingTime", "status" },
                Filter = new QueryFilter()
            };
            config.Filter.AddCondition("isOpen", QueryOperator.Equal, Booking.IS_OPEN_STR);
            config.IndexName = "isOpenIdx";
            Table table = Table.LoadTable(client, CheckTablenameForTest(Booking.TABLE_NAME));
            
            var search = table.Query(config);
            List<Document> documentList = new List<Document>();
            do
            {
                foreach (var doc in search.GetNextSet())
                {
                    long ticks = Convert.ToInt64(doc["bookingDate"]);
                    Calendar.Add(new CalendarItemDTO()
                    {
                        BookingNum = int.Parse(doc["id"]),
                        BookingName = doc["bookingName"],
                        BookingDate = ticks > 0 ? new DateTime(ticks) : default(DateTime),
                        BookingTime = int.Parse(doc["bookingTime"]),
                        //TimeSlot = (TimeSlots)Enum.Parse(typeof(TimeSlots), doc["timeSlot"]),
                        BookingStatus = (BookingStates)Enum.Parse(typeof(BookingStates), doc["status"])                        
                    });
                }
            } while (!search.IsDone);

            return Calendar;
        }

        private static List<ExistingCustomerDTO> CustomerDirectory { get; set; }

        public static List<ExistingCustomerDTO> GetCustomerDirectory()
        {
            if (CustomerDirectory != null)
            {
                return CustomerDirectory;
            }

            CustomerDirectory = new List<ExistingCustomerDTO>();
            AmazonDynamoDBClient client = GetDynamoDBClient();

            var config = new ScanOperationConfig()
            {
                Select = SelectValues.SpecificAttributes,
                AttributesToGet = new List<string> { "id", "directoryName" }
            };

            Table table = Table.LoadTable(client, CheckTablenameForTest(Customer.TABLE_NAME));
            var search = table.Scan(config);

            List<Document> documentList = new List<Document>();
            do
            {
                foreach (var doc in search.GetNextSet())
                {
                    CustomerDirectory.Add(new ExistingCustomerDTO()
                    {
                        CustomerId = doc["id"],
                        DirectoryName = doc["directoryName"],
                    });
                }
            } while (!search.IsDone);

            return CustomerDirectory;
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

        //private class BaseItemFactory<T> where T : new()
        //{
        //    public T CreateItem()
        //    {
        //        return new T();
        //    }
        //}

    }
}
