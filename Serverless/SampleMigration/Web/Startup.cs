using Microsoft.Extensions.DependencyInjection;

namespace SampleMigration.Web
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services) =>
            services
                .AddSingleton<Server>();
    }
}