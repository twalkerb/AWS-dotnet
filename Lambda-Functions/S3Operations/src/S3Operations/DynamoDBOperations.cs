using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.DataModel;

namespace S3Operations
{
    public class DynamoDBOperations
    {
        private readonly AmazonDynamoDBClient client;
        private readonly DynamoDBContext context;
        private const string EventTableName = "events";
        public DynamoDBOperations()
        {
            client = new AmazonDynamoDBClient
            (
                Amazon.RegionEndpoint.USEast2
            );
            context = new DynamoDBContext(client);
        }

        public async Task<List<Event>> GetAllEvents()
        {
            var request = new ScanRequest
            {
                TableName = EventTableName
            };
            var response = await client.ScanAsync(request);

            var items = response.Items;

            var events = new List<Event>();
            foreach (var item in items)
            {
                var docItem = Document.FromAttributeMap(item);
                events.Add
                (
                    context.FromDocument<Event>(docItem)
                );
            }
            return events;
        }
    }
}