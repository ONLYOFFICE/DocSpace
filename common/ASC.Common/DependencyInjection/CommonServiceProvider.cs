using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Common.DependencyInjection
{
    public static class CommonServiceProvider
    {
        public static IServiceProvider Current { get; set; }

        public static T GetService<T>() => Current.GetService<T>();

        public static IHostingEnvironment HostingEnvironment { get => Current.GetService<IHostingEnvironment>(); }
    }
}
