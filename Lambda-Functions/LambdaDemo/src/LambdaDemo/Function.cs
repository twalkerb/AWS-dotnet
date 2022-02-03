using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaDemo
{
    public class Function
    {

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string name = "No Name";
            context.Logger.Log($"------Executing API Gateway------");
            if (request.QueryStringParameters != null && request.QueryStringParameters.ContainsKey("name"))
            {
                name = request.QueryStringParameters["name"];
            }

            if (request.HttpMethod == "POST")
                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = $"Received POST Method. User name passed: {name}"
                };

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = $"User name passed: {name}"
            };
        }
    }
}
