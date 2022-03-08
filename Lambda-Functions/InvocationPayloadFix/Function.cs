using System;
using System.Text;
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
        public Task<DummyData> FunctionHandler(string input, ILambdaContext context)
        {
            var fileData = s3.ReadJson<DummyData>();
            context.Logger.Log($"Received input: {input}");
            return fileData;
        }

        public class DummyData
        {
            public List<CtRoot> ctRoot {get; set;}
        }

        public class CtRoot
        {
            public string _id {get; set;}
            public string name {get; set;}
            public string dob {get; set;}
            public string telephone {get; set;}
            public string[] pets {get; set;}
            public float score {get; set;}
            public string email {get; set;}
            public string url {get; set;}
            public string verified {get; set;}
            public string description {get; set;}
            public float salary {get; set;}
            public address address {get; set;}
        }

        public class address
        {
            public string street {get; set;}
            public string town {get; set;}
            public string postcode {get; set;}
        }

    }
}
