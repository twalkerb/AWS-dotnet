using System;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.SNSEvents;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SnsProcessor
{
    public class Function
    {
        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        
        public Function()
        {            
        }

        public DynamoDb DynamoDb = new DynamoDb();

        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an SNS event object and can be used 
        /// to respond to SNS messages.
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(SNSEvent evnt, ILambdaContext context)
        {
            await ProcessRecordAsync(evnt.Records[0], context);
        }

        private async Task ProcessRecordAsync(SNSEvent.SNSRecord record, ILambdaContext context)
        {
            context.Logger.LogLine($"Processed record {record.Sns.Message}");
            
            // TODO: record.Sns.MessageAttributes
            try
            {
                var message = new EventMessage()
                {
                    eventId = $"SNS_{Guid.NewGuid()}",
                    eventMessageId = record.Sns.MessageId,
                    eventSubject = record.Sns.Subject,
                    eventMessageText = record.Sns.Message,
                    eventTimestamp = new DateTimeOffset(record.Sns.Timestamp).ToUnixTimeMilliseconds(),
                    eventTopicArn = record.Sns.TopicArn
                };
                await DynamoDb.InsertTable(message, context);
            }
            catch(Exception ex)
            {
                context.Logger.Log($"---Exception: {ex.Message}---");
            }
            await Task.CompletedTask;
        }
    }
}
