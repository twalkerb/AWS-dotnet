using System;
using System.Threading.Tasks;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Newtonsoft.Json;
using Amazon.Lambda.APIGatewayEvents;
using System.Collections.Generic;
using Amazon.APIGateway;
using Amazon.APIGateway.Model;

namespace InvokeLambdaApp
{
    public class ConsoleApp
    {
        private readonly AmazonLambdaClient lambdaClient;
        private readonly AmazonLambdaConfig lambdaConfig;

        private readonly AmazonAPIGatewayClient apiGatewayClient;
        private readonly AmazonAPIGatewayConfig apiGatewayConfig;

        public ConsoleApp()
        {
            // Lambda Config
            lambdaConfig = new AmazonLambdaConfig()
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast2
            };
            lambdaClient = new AmazonLambdaClient(lambdaConfig);
            // AP Gateway Config
            apiGatewayConfig = new AmazonAPIGatewayConfig()
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast2
            };
            apiGatewayClient = new AmazonAPIGatewayClient(apiGatewayConfig);
        }

        public async Task Run()
        {
            try
            {
                // await invokeLambdaFunction();
                await invokeLambdaServerRestAPI();
                // await executeRestApi();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
            await Task.CompletedTask;
        }

        private async Task invokeLambdaFunction()
        {
            var payload = JsonConvert.SerializeObject("Hey");
            var request = new InvokeRequest() { FunctionName = "LambdaMigration", Payload = payload };
            var res = await lambdaClient.InvokeAsync(request);
            var responseText = System.Text.Encoding.UTF8.GetString(res.Payload.ToArray());
        }

        private async Task invokeLambdaServerRestAPI()
        {
            Console.WriteLine("Invoking Lamda Server by Lambda-Invoke");
            var apiRequestBody = new APIGatewayProxyRequest()
            {
                Body = "",
                Headers = new Dictionary<string, string>(),
                HttpMethod = "POST",
                IsBase64Encoded = new Boolean(),
                MultiValueQueryStringParameters = new Dictionary<string, IList<string>>(),
                MultiValueHeaders = new Dictionary<string, IList<string>>(),
                Path = "hit/WhatSay?",
                PathParameters = new Dictionary<string, string>(),
                QueryStringParameters = new Dictionary<string, string>(),
                RequestContext = new APIGatewayProxyRequest.ProxyRequestContext(),
                Resource = "/",
                StageVariables = new Dictionary<string, string>()
            };
            var payload = JsonConvert.SerializeObject(apiRequestBody);
            var request = new InvokeRequest() { FunctionName = "SampleMigration", Payload = payload };
            var res = await lambdaClient.InvokeAsync(request);
            var responseText = System.Text.Encoding.UTF8.GetString(res.Payload.ToArray());
            var apiProxyResponse = JsonConvert.DeserializeObject<APIGatewayProxyResponse>(responseText);
        }

        private async Task executeRestApi()
        {
            Console.WriteLine("Invoking Lamda Server by API Gateway");
            var request = new TestInvokeMethodRequest()
            {
                Body = "",
                ClientCertificateId = "",
                Headers = new Dictionary<string, string>(),
                HttpMethod = "GET",
                MultiValueHeaders = new Dictionary<string, List<string>>(),
                PathWithQueryString = "/",
                ResourceId = "ldvw2z",
                RestApiId = "f6128msoog",
                StageVariables = new Dictionary<string, string>()
            };
            var response = await apiGatewayClient.TestInvokeMethodAsync(request);
        }
    }
}