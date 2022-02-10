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
        public async Task<String> FunctionHandler(string input, ILambdaContext context)
        {
            if(!string.IsNullOrEmpty(input))
            {
                
            }
            return "Failed to execute!";
        }
    }
}
