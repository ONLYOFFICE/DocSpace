using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Common.DependencyInjection
{
    public static class CommonServiceProvider
    {
        public static T GetService<T>() => ServiceProvider.GetService<T>();

        private static IServiceProvider ServiceProvider { get; set; }

        public static IApplicationBuilder InitCommonServiceProvider(this IApplicationBuilder app)
        {
            ServiceProvider = app.ApplicationServices;
            return app;
        }


    }
}
