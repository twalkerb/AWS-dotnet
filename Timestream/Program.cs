using System;
using Microsoft.Extensions.DependencyInjection;

namespace Timestream
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
                .AddSingleton<Timestream>()
                .AddSingleton<TimestreamDatabase>()
                .AddSingleton<TimestreamTable>()
                .AddSingleton<TimestreamInsert>()
                .AddSingleton<SampleTimestream>()
                .AddSingleton<Compression>()
            ;
        }
    }
}
