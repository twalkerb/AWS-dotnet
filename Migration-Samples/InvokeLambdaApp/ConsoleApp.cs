using System;
using System.Threading.Tasks;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Newtonsoft.Json;

namespace InvokeLambdaApp
{
    public class ConsoleApp
    {
        private readonly AmazonLambdaClient client;
        private readonly AmazonLambdaConfig config;
        public ConsoleApp()
        {
            config = new AmazonLambdaConfig()
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast2
            };
            client = new AmazonLambdaClient(config);
        }

        public async Task Run()
        {
            try
            {
                var payload = JsonConvert.SerializeObject("Hey");
                var request = new InvokeRequest() { FunctionName = "LambdaMigration", Payload = payload };
                var res = await client.InvokeAsync(request);
                var responseText = System.Text.Encoding.UTF8.GetString(res.Payload.ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadLine();
            await Task.CompletedTask;
        }
    }
}