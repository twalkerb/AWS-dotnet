using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;


namespace SampleMigration
{
    class Program
    {
        public static void Main_Deprecated(string[] args)
        {
            try
            {
                int localPort = 8989;
                GetWebHost(localPort).Run();
            }
            catch (Exception e)
            {
                System.Console.Error.WriteLine(e.ToString());
            }
        }

        public static string GetServerUrl(string defaultHost, int defaultPort)
        {
            return "http://" + defaultHost + ":" + defaultPort;
        }

        static IWebHost GetWebHost(int defaultPort)
        {
            var builder = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>();

            builder.UseUrls(GetServerUrl("*", defaultPort));
            return builder.Build();
        }
        
    }
}
