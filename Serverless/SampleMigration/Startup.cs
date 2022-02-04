using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace SampleMigration
{
    using SampleMigration.DependencyInjection;
    using SampleMigration.Server;

    class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddRouting()
                .AddResponseCompression()
                .ConfigureServices(Web.Startup.ConfigureServices);
        }

        public void Configure(IApplicationBuilder appbuilder, Web.Server webServer)
        {
            appbuilder
                .UseRouter(new[]
                {
                    ("", webServer.Routes)
                });
        }
    }
}