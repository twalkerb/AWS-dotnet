using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace S3Operations
{
    public class SNSOperations
    {
        private readonly AmazonSimpleNotificationServiceClient client;
        private const string snsTopic = "Learners-Dynamo-S3";
        public SNSOperations()
        {
            client = new AmazonSimpleNotificationServiceClient
            (
                Amazon.RegionEndpoint.USEast2
            );
        }

        public async Task<int> PublishMessage(string messageBody, string subject)
        {
            var snsTopicArn = await getTopicDetails();
            var request = new PublishRequest()
            {
                TopicArn = snsTopicArn,
                Message = messageBody,
                Subject = subject
            };

            var response = await client.PublishAsync(request);
            return (int)response.HttpStatusCode;
        }

        private async Task<string> getTopicDetails()
        {
            var result = await client.FindTopicAsync(snsTopic);
            return result.TopicArn;
        }

    }
}