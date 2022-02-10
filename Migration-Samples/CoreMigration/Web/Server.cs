using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;
using System.Web;
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace SampleMigration.Web
{
    public class Server
    {
        private readonly MongoClient client;

        public Server()
        {
            client = new MongoClient("mongodb+srv://admin:5oswlcROstAwOtu@cluster0.6hckj.mongodb.net/learning-MongoDB?retryWrites=true&w=majority");
        }

        public static RequestDelegate Route(RequestDelegate action)
            => action;

        RequestDelegate Route(Func<HttpContext, string, Task> action, string name) =>
            ctx => action(ctx, UrlParam(ctx, name));

        string UrlParam(HttpContext ctx, string name) =>
            ctx.GetRouteData().Values[name].ToString();

        public (int, string, string, RequestDelegate)[] Routes =>
            new[]
            {
                (0, "GET", "/", Route(AcknowledgeRequest)),
                (0, "GET", "/status", Route(GetStatus)),
                (0, "POST", "/hit/{comment}", Route(InsertRecord, "comment")),
            };

        private Task AcknowledgeRequest(HttpContext context)
        {
            var res = context.Response;
            res.ContentType = "text/plain";
            var body = System.Text.Encoding.UTF8.GetBytes("Hi, can you see me?");
            return res.Body.WriteAsync(body, 0, body.Length);
        }

        private Task GetStatus(HttpContext context)
        {
            var res = context.Response;
            res.ContentType = "text/plain";
            var body = System.Text.Encoding.UTF8.GetBytes("Status is all good!");
            return res.Body.WriteAsync(body, 0, body.Length);
        }

        private async Task InsertRecord(HttpContext context, string comment)
        {
            IMongoDatabase db = client.GetDatabase("learning-MongoDB");
            var countLoggerCollection = db.GetCollection<CountLogger>("CountLogger");
            var doc = new CountLogger()
            {
                EventDate = DateTime.Now.ToString(),
                Comment = comment
            };
            var res = string.Empty;
            try
            {
                await countLoggerCollection.InsertOneAsync(doc);
                await Reply(context, doc.id);                
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }            
        }

        Task Reply(HttpContext context, object value)
        {
            var json = JsonConvert.SerializeObject(value);
            context.Response.ContentType = "application/json";
            using (var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                return jsonStream.CopyToAsync(context.Response.Body);
        }
    }
}