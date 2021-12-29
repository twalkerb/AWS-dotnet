using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;

namespace DynamoDB
{
    public class DynamoDb
    {
        private readonly AmazonDynamoDBClient client;
        private readonly DynamoDBContext context;
        private const string EventTableName = "events";
        private const string EventIndexTableName = "events-index";
        public DynamoDb()
        {
            client = new AmazonDynamoDBClient
            (
                Amazon.RegionEndpoint.USEast2
            );
            context = new DynamoDBContext(client);
        }

        public async Task Run()
        {
            try
            {
                var events = await GetAllEvents();
                await TransactEvents(events);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
            await Task.CompletedTask;
        }

        private async Task<List<Event>> GetAllEvents()
        {
            Console.WriteLine("---Begin: Fetch All Events---");
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
            Console.WriteLine("---End: Fetch All Events---");
            return events;
        }

        private async Task TransactEvents(List<Event> events)
        {
            Console.WriteLine("---Begin: Events Transaction---");
            try
            {
                var groupedEvents = events
                        .GroupBy(x => new { x.eventTimestamp })
                        .ToList();

                foreach (var data in groupedEvents)
                {
                    var request = new TransactWriteItemsRequest
                    {
                        TransactItems = new List<TransactWriteItem>
                        {
                            new TransactWriteItem
                            {
                                Put = new Put
                                {
                                    TableName = EventIndexTableName,
                                    Item = new Dictionary<string, AttributeValue>()
                                    {
                                        {
                                            "eventTimestamp", new AttributeValue { N = data.Key.eventTimestamp.ToString() }
                                        },
                                        {
                                            "counter", new AttributeValue { N = (data.Count()).ToString() }
                                        }
                                    },
                                    ReturnValuesOnConditionCheckFailure = ReturnValuesOnConditionCheckFailure.ALL_OLD
                                }
                            }
                        }
                    };
                    var response = await client.TransactWriteItemsAsync(request);
                    Console.WriteLine($"Transaction sent with - Timestamp - {data.Key.eventTimestamp} PUTs - {response.HttpStatusCode}");
                }
            }
            catch (TransactionCanceledException e)
            {
                Console.WriteLine(e.CancellationReasons[0].Message);
            }
            catch (AmazonDynamoDBException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (AmazonServiceException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine($"---End: Events Transaction---");
        }
    }
}