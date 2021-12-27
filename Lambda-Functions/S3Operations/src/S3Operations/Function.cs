using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace S3Operations
{
    public class Function
    {
        // DynamoDBOperations DynamoDbOperations;
        // S3FileOperations S3FileOperations;
        // public Function
        // (
        //     DynamoDBOperations dynamodbOperations,
        //     S3FileOperations s3FileOperations
        // )
        // {
        //     DynamoDbOperations = dynamodbOperations;
        //     S3FileOperations = s3FileOperations;
        // }
        public DynamoDBOperations DynamoDBOperations = new DynamoDBOperations();
        public S3FileOperations S3FileOperations = new S3FileOperations();

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<String> FunctionHandler(string input, ILambdaContext context)
        {
            if(!string.IsNullOrEmpty(input))
            {
                var events = await DynamoDBOperations.GetAllEvents();
                context.Logger.Log($"----Fetched records from DynamoDB: {events.Count()}----");
                await S3FileOperations.WriteJson(events);
                context.Logger.Log("----Created File on S3 Bucket----");
                return "Executed!";
            }
            return "Failed to execute!";
        }
    }
}
