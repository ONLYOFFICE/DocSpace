using System;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Common.DependencyInjection
{
    public static class CommonServiceProvider
    {
        public static IServiceProvider Current { get; set; }

        public static T GetService<T>() => Current.GetService<T>();
    }
}
