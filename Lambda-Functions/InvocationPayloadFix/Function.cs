using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace InvocationPayloadFix
{
    public class Function
    {
        public AwsS3 s3 = new AwsS3();
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(string input, ILambdaContext context)
        {
            var fileData = s3.ReadJson<DummyData>().GetAwaiter().GetResult();
            var s3fileUrl = s3.GeneratePreSignedURL(fileData).GetAwaiter().GetResult();
            context.Logger.Log($"Received input: {input}");
            return s3fileUrl;
        }

    }
}
