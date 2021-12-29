using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;

namespace SnsProcessor
{
    public class DynamoDb
    {
        private readonly AmazonDynamoDBClient client;
        private readonly DynamoDBContext context;
        private const string logTable = "events-trigger";

        public DynamoDb()
        {
            client = new AmazonDynamoDBClient
            (
                Amazon.RegionEndpoint.USEast2
            );
            context = new DynamoDBContext(client);
        }

        public async Task InsertTable(EventMessage eventRecord, ILambdaContext context)
        {
            try
            {
                var request = new PutItemRequest
                {
                    TableName = logTable,
                    Item = new Dictionary<string, AttributeValue>()
                    {
                        {
                            "eventMessageId", new AttributeValue { S = eventRecord.eventMessageId }
                        },
                        {
                            "eventTimestamp", new AttributeValue { N = eventRecord.eventTimestamp.ToString() }
                        },
                        {
                            "eventId", new AttributeValue { S =  eventRecord.eventId }
                        },
                        {
                            "eventSubject", new AttributeValue { S = eventRecord.eventSubject }
                        },
                        {
                            "eventMessageText", new AttributeValue { S = eventRecord.eventMessageText }
                        },
                        {
                            "eventTopicArn", new AttributeValue { S = eventRecord.eventTopicArn }
                        }
                    }
                };
                await client.PutItemAsync(request);
                context.Logger.Log("Record inserted successfully!");
            }
            catch(Exception ex)
            {
                context.Logger.Log($"---Exception: {ex.Message}---");
            }
        }
    }
}