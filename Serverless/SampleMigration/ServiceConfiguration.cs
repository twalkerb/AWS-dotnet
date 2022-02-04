using System;
using Microsoft.Extensions.DependencyInjection;

namespace SampleMigration.DependencyInjection
{
    public static class ServiceConfiguration
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection sc, Action<IServiceCollection> configure)
        {
            configure(sc);
            return sc;
        }

        public static T GetService<T>(this IServiceCollection sc) =>
            sc.BuildServiceProvider().GetService<T>();
    }
}