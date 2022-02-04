using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace SampleMigration.Web
{
    public class Server
    {

        public static RequestDelegate Route(RequestDelegate action)
            => action;
        public (int, string, string, RequestDelegate)[] Routes =>
            new[]
            {
                (0, "GET", "/status", Route(GetStatus)),
            };
        
        private Task GetStatus(HttpContext context)
        {
            var res = context.Response;
            res.ContentType = "text/plain";
            var body = System.Text.Encoding.UTF8.GetBytes("Let's start!");
            return res.Body.WriteAsync(body, 0, body.Length);
        }
    }
}