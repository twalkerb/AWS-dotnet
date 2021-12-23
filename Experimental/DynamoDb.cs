using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;

namespace Experimental
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

                await GetTransactionEvents();

                // var t1 = new Thread(() => TransactEvents(events, 1));
                // var t2 = new Thread(() => TransactEvents(events, 2));
                // var t3 = new Thread(() => TransactEvents(events, 3));
                // var t4 = new Thread(() => TransactHarcodedEvents(events, 4));
                // var t5 = new Thread(() => TransactHarcodedEvents(events, 5));
                // var t6 = new Thread(() => TransactHarcodedEvents(events, 6));
                // t1.Start();
                // t2.Start();
                // t3.Start();
                // t4.Start();
                // t5.Start();
                // t6.Start();

                Console.WriteLine("Starting Threads...");
                Parallel.Invoke
                (
                    () =>
                    {
                        Console.WriteLine("Invoke T1...");
                        TransactEvents(events, 1);
                    },
                    () =>
                    {
                        Console.WriteLine("Invoke T2...");
                        TransactHarcodedEvents(events, 2);
                    },
                    () =>
                    {
                        Console.WriteLine("Invoke T3...");
                        TransactEvents(events, 3);
                    },
                    () =>
                    {
                        Console.WriteLine("Invoke T4...");
                        TransactHarcodedEvents(events, 4);
                    }
                );
                Console.WriteLine("Threads Executed...");
                // await TransactEvents(events);
                // await TransactHarcodedEvents(events);


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

        private async Task GetTransactionEvents()
        {
            Console.WriteLine("---Begin: Get Events Transaction---");
            var request = new TransactGetItemsRequest
            {
                TransactItems = new List<TransactGetItem>
                {
                    new TransactGetItem
                    {
                        Get = new Get
                        {
                            TableName = EventIndexTableName,
                            Key = new Dictionary<string, AttributeValue>()
                            {
                                {
                                    "eventTimestamp", new AttributeValue
                                    {
                                        N = (1645096949450).ToString()
                                    }
                                },
                            },
                            ExpressionAttributeNames = new Dictionary<string, string>()
                            {
                                {"#Timestamp", "eventTimestamp"},
                            }
                        }
                    }
                }
            };
            var response = await client.TransactGetItemsAsync(request);
            Console.WriteLine("---End: Get Events Transaction---");
        }


        private void TransactHarcodedEvents(List<Event> events, int additionalCounter)
        {
            Console.WriteLine($"---Begin: Thread-{additionalCounter} Harcoded Events Transaction---");
            try
            {
                var groupedEvents = events
                                        .GroupBy(x => new { x.eventTimestamp })
                                        .ToList();

                foreach (var data in groupedEvents)
                {
                    long ts = data.Key.eventTimestamp; // + 30000
                    int cnt = data.Count() + additionalCounter;
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
                                        "eventTimestamp", new AttributeValue { N = ts.ToString() }
                                    },
                                    {
                                        "counter", new AttributeValue { N = cnt.ToString() }
                                    }
                                }
                            }
                        }
                    }
                    };
                    var response = client.TransactWriteItemsAsync(request).GetAwaiter().GetResult();
                    // var response = client.TransactWriteItemsAsync(request).Result;
                    Console.WriteLine($"Thread-{additionalCounter}: Hardcoded Transaction sent with - Timestamp - {data.Key.eventTimestamp} PUTs - {response.HttpStatusCode}");
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
            Console.WriteLine($"---End: Thread-{additionalCounter} Hardcoded Events Transaction---");
        }
        private void TransactEvents(List<Event> events, int additionalCounter)
        {
            Console.WriteLine($"---Begin: Thread-{additionalCounter} Events Transaction---");
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
                                        "counter", new AttributeValue { N = (data.Count()+ additionalCounter).ToString() }
                                    }
                                },
                                ReturnValuesOnConditionCheckFailure = ReturnValuesOnConditionCheckFailure.ALL_OLD
                            }
                        }
                    },
                    };
                    var response = client.TransactWriteItemsAsync(request).GetAwaiter().GetResult();
                    // var response = client.TransactWriteItemsAsync(request).Result;
                    // var response = await client.TransactWriteItemsAsync(request);
                    // var response = WriteTransactions(request).Result;
                    Console.WriteLine($"Thread-{additionalCounter}: Transaction sent with - Timestamp - {data.Key.eventTimestamp} PUTs - {response.HttpStatusCode}");
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
            Console.WriteLine($"---End: Thread-{additionalCounter} Events Transaction---");
        }
    }
}