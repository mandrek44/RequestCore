using System;
using Microsoft.AspNetCore.Builder;

namespace RequestCore
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseRequestHandler(this IApplicationBuilder appBuilder, IRequestHandler service)
        {
            appBuilder.Use(async (context, next) =>
            {
                var handled = await service.HandleAsync(context);
                if (!handled)
                    await next();
            });
        }

        public static void LogAllRequests(this IApplicationBuilder appBuilder)
        {
            appBuilder.Use(async (context, next) =>
            {
                Console.WriteLine($"-> {context.Request.Method} {context.Request.Path.Value}");

                try
                {
                    await next();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"!! {e.Message}");
                    throw;
                }

                Console.WriteLine($"<- {context.Response.StatusCode}");
            });
        }
    }
}