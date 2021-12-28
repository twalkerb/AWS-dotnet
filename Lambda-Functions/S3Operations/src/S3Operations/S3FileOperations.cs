using System;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;

namespace S3Operations
{
    public class S3FileOperations
    {
        private readonly AmazonS3Client client;
        private const string bucket = "learners-lambda-bucket";
        public SNSOperations SNSOperations = new SNSOperations();        
        public S3FileOperations()
        {
            client = new AmazonS3Client
            (
                Amazon.RegionEndpoint.USEast2
            );
        }

        public async Task WriteObject(string key, byte[] bytes, string contentType)
        {
            using (var memStream = new MemoryStream(bytes))
            {
                var request = new PutObjectRequest
                {
                    BucketName = bucket,
                    Key = key,
                    InputStream = memStream,
                    ContentType = contentType,
                };
                await client.PutObjectAsync(request);
                await SNSOperations.PublishMessage($"Created File on S3 Bucket: {key}", "S3 File Operation");
            }
        }

        public Task WriteJson(object obj)
        {
            return WriteObject(
                    key: String.Format("{0}.json", DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss")),
                    bytes: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj, Formatting.Indented)),
                    contentType: "application/json"
                );
        }
    }
}