using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Common.DependencyInjection
{
    public static class CommonServiceProvider
    {
        public static T GetService<T>() => ServiceProvider.GetService<T>();

        private static IServiceProvider ServiceProvider { get; set; }

        public static void Init(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
        public static void UseCSP(this IApplicationBuilder applicationBuilder)
        {
            Init(applicationBuilder.ApplicationServices);
        }
    }
}
