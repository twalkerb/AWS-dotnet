using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace SampleMigration.Server
{
    public static class Router
    {
        public static IApplicationBuilder UseRouter(
                this IApplicationBuilder appBuilder,
                (string mountPoint, (int level, string verb, string path, RequestDelegate handler)[] routes)[] routeSet
            )
        {
            RequestDelegate fixHandler(string mountPoint, int level, RequestDelegate handler)
            {
                if (!mountPoint.StartsWith("/"))
                    mountPoint = "/" + mountPoint;

                return async (context) =>
                {
                    if (mountPoint != "/")
                    {
                        context.Request.PathBase += mountPoint;
                        context.Request.Path = context.Request.Path.Value.Substring(mountPoint.Length);
                    }
                    if (level <= 0 || (context.Items["AccessLevel"] is int grant) && grant >= level)
                    {
                        await handler(context);
                    }
                    else
                    {
                        context.Response.StatusCode = 403;
                    }
                };
            }

            void buildRoutes(IRouteBuilder routeBuilder)
            {
                foreach (var (mountPoint, routes) in routeSet)
                    foreach (var (level, verb, path, handler) in routes)
                        routeBuilder.MapVerb(verb, mountPoint + path, fixHandler(mountPoint, level, handler));
            }

            return RoutingBuilderExtensions.UseRouter(appBuilder, buildRoutes);
        }
    }
}