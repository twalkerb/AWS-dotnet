using System;
using Microsoft.Extensions.DependencyInjection;

namespace DynamoDB
{
    class Program
    {
         public static void Main(string[] args)
        {
            try
            {
                IServiceCollection services = new ServiceCollection();
                ConfigureServices(services);
                IServiceProvider serviceProvider = services.BuildServiceProvider();
                var service = serviceProvider.GetService<ConsoleApp>();
                service.Run().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services                
                .AddSingleton<ConsoleApp>()
                .AddSingleton<DynamoDb>()
            ;
        }
    }
}
