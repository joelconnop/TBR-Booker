using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon;
using TBRBooker.Model.Entities;

namespace TBRBooker.Business
{
    public class DBBox
    {

        private static AmazonDynamoDBClient GetDynamoDBClient()
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

        public static void WriteItem(BaseItem itm)
        {
            var client = GetDynamoDBClient();
            client.PutItem(
                tableName: itm.TableName,
                item: itm.WriteAttributes()
            );
        }

        public static void SaveBookingEtc(Booking booking)
        {
            if (booking.Customer == null)
        }

    }
}
