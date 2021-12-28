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

namespace SnsProcessor
{
    public class DynamoDb
    {
        private readonly AmazonDynamoDBClient client;
        private readonly DynamoDBContext context;
        private const string logTable = "message-processor";

        public DynamoDb()
        {
            client = new AmazonDynamoDBClient
            (
                Amazon.RegionEndpoint.USEast2
            );
            context = new DynamoDBContext(client);
        }

        public async Task InsertTable(EventMessage message)
        {
            await client.UpdateItemAsync(new UpdateItemRequest
            {
                TableName = logTable,
                Key = new Dictionary<string, AttributeValue>
                {
                    { 
                        "messageId", new AttributeValue($"SnsProcessor|{message.TestId}") 
                    }
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { 
                        ":incr", new AttributeValue { N = "1" }
                    },
                    { 
                        ":end", new AttributeValue(DateTimeOffset.UtcNow.ToString("o"))
                    }
                },
                UpdateExpression = "SET MessageCount = MessageCount + :incr, EndTime = :end"
            });
        }
    }
}