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
using TBRBooker.Base;

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
        public static bool IsTestEnvironment()
        {
            return Settings.Inst().IsTestMode;
        }
        private static bool IsCheckedTestEnvironment { get; set; }


        /// <summary>
        /// Not neccessary to call but sometimes want to
        /// </summary>
        public static void InitDynamoDbClient()
        {
            var client = GetDynamoDBClient();
        }

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

        private static string CheckTablenameForTest(string tableName)
        {
            string test = "test_";
            bool isTestTable = tableName.StartsWith(test);
            if (isTestTable && !IsTestEnvironment())
                //return tableName.Replace(test, "");   //use this if we decide we do need to hit here
                throw new Exception("Should not have encountered " + tableName + " outside of testing.");
            else if (!isTestTable && IsTestEnvironment())
                return test + tableName;
            else
                return tableName;
        }

        public static void AddOrUpdate(BaseItem itm)
        {
            if (!string.IsNullOrEmpty(itm.Id) && itm.Id.Equals(Booking.RepeatingBookingId))
            {
                throw new Exception("Entity still has the TBD status. Cannot save.");
            }

            AmazonDynamoDBClient client = GetDynamoDBClient();
            Table table = Table.LoadTable(client, CheckTablenameForTest(itm.TableName));

            if (itm.IsNew())
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

        public static T ReadItem<T>(string id) where T : BaseItem
        {
            if (id.Equals(Booking.RepeatingBookingId))
            {
                throw new Exception("item has the TBD status. Cannot read.");
            }

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

        private static List<BookingCalendarItemDTO> Calendar { get; set; }

        public static List<BookingCalendarItemDTO> GetCalendarItems(
            bool isForceReadAll, bool leaveCachedCalendarAlone)
        {
            if (Calendar != null && !isForceReadAll)
            {
                return Calendar;
            }

            var dbCalendar = new List<BookingCalendarItemDTO>();

            AmazonDynamoDBClient client = GetDynamoDBClient();
            Table table = Table.LoadTable(client, CheckTablenameForTest(Booking.TABLE_NAME));
            var attsToGet = new List<string> { "id", "bookingName", "bookingDate",
                "bookingTime", "status", "followupDate", "confirmationDate" };
            QueryOperationConfig qryConfig = null;
            ScanOperationConfig scanConfig = null;

            if (isForceReadAll)
            {
                //all bookings from all time
                scanConfig = new ScanOperationConfig()
                {
                    Select = SelectValues.SpecificAttributes,
                    AttributesToGet = attsToGet
                };
            }
            else
            {
                //typical case
                qryConfig = new QueryOperationConfig()
                {
                    Select = SelectValues.SpecificAttributes,
                    AttributesToGet = attsToGet,
                    Filter = new QueryFilter()
                };
                qryConfig.Filter.AddCondition("isOpen", QueryOperator.Equal, Constants.DbBoolFilter);
                qryConfig.IndexName = "isOpenIdx";
            }          
            
            var search = isForceReadAll ? table.Scan(scanConfig) : table.Query(qryConfig);
            List<Document> documentList = new List<Document>();
            do
            {
                foreach (var doc in search.GetNextSet())
                {
                    long bookingTicks = Convert.ToInt64(doc["bookingDate"]);

                    DateTime? followupDate = null;
                    if (doc.ContainsKey("followupDate"))    //(didn't have this field from beginning)
                    {
                        long followupTicks = long.Parse(doc["followupDate"]);
                        if (followupTicks > 0)
                            followupDate = new DateTime(followupTicks);
                    }
                    DateTime? confirmationDate = null;
                    if (doc.ContainsKey("confirmationDate"))    //(didn't have this field from beginning)
                    {
                        long confirmationTicks = long.Parse(doc["confirmationDate"]);
                        if (confirmationTicks > 0)
                            confirmationDate = new DateTime(confirmationTicks);
                    }

                    dbCalendar.Add(new BookingCalendarItemDTO(int.Parse(doc["id"]), doc["bookingName"],
                        bookingTicks > 0 ? new DateTime(bookingTicks) : default(DateTime),
                        int.Parse(doc["bookingTime"]),
                        (BookingStates)Enum.Parse(typeof(BookingStates), doc["status"]),
                        followupDate, confirmationDate));
                        //TimeSlot = (TimeSlots)Enum.Parse(typeof(TimeSlots), doc["timeSlot"]),
                }
            } while (!search.IsDone);

            var calendar = CalendarBL.BuildCalendarFromDbRead(dbCalendar, isForceReadAll);

            if (!leaveCachedCalendarAlone)
                Calendar = calendar;

            return calendar;
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
                    CustomerDirectory.Add(new ExistingCustomerDTO(doc["id"], doc["directoryName"]));
                }
            } while (!search.IsDone);

            return CustomerDirectory;
        }

        public static List<ErrorLog> SearchErrorLog(string action)
        {
            //a bit like GetCustomerDirectory, except there must be an easier way when
            //we are just grabbing all records for a partitionkey (action)
            throw new NotImplementedException();
        }

        /// <summary>
        /// could easily be converted into a ReadAll(T) function
        /// </summary>
        /// <returns></returns>
        public static List<RepeatSchedule> GetRepeatSchedules()
        {
            // this table only ever changes programatically so happy to only read once per instance
            var cache = GetCachedItems(RepeatSchedule.TABLE_NAME);
            if (cache.Count > 0)
                return cache.Values.Select(x => (RepeatSchedule)x).ToList();

            AmazonDynamoDBClient client = GetDynamoDBClient();

            var temp = new RepeatSchedule();
            var config = new ScanOperationConfig()
            {
                Select = SelectValues.SpecificAttributes,
                AttributesToGet = temp.GetReadAttributes()
            };

            Table table = Table.LoadTable(client, CheckTablenameForTest(temp.TableName));
            var search = table.Scan(config);

            List<Document> documentList = new List<Document>();
            do
            {
                foreach (var doc in search.GetNextSet())
                {
                    var item = DocToItem<RepeatSchedule>(doc);

                    if (temp.IsCacheItems() && item != null)
                        cache.Add(item.Id, item);
                }
            } while (!search.IsDone);

            return cache.Values.Select(x => (RepeatSchedule)x).ToList();
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

        public static List<Penalty> GetUnpaidPenalties()
        {
            var cache = GetCachedItems(Penalty.TABLE_NAME);
            cache.Clear();

            AmazonDynamoDBClient client = GetDynamoDBClient();
            Table table = Table.LoadTable(client, CheckTablenameForTest(Penalty.TABLE_NAME));
            QueryOperationConfig qryConfig = null;

            //typical case
            qryConfig = new QueryOperationConfig()
            {
                Select = SelectValues.AllAttributes,
                Filter = new QueryFilter()
            };
            qryConfig.Filter.AddCondition("isPending", QueryOperator.Equal, Constants.DbBoolFilter);
            qryConfig.IndexName = "isPendingIdx";

            var search = table.Query(qryConfig);
            var penalties = new List<Penalty>();
            do
            {
                foreach (var doc in search.GetNextSet())
                {
                    var item = DocToItem<Penalty>(doc);
                    if (item.IsCacheItems() && item != null)
                        cache.Add(item.Id, item);
                    penalties.Add(item);
                }
            } while (!search.IsDone);

            return penalties;
        }

    }
}
