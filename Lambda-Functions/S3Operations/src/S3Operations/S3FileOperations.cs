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
        
        public S3FileOperations()
        {
            client = new AmazonS3Client
            (
                Amazon.RegionEndpoint.USEast2
            );
        }

        public async Task WriteObject(object obj) 
        {
            string key = String.Format("{0}.json", DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss"));
            var request = new PutObjectRequest()
            {
                BucketName = bucket,
                Key = key,
                ContentBody = JsonConvert.SerializeObject(obj),
                ContentType = "application/json",
            };            
            await client.PutObjectAsync(request);
        }        
    }
}