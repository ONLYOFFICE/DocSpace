using System;

using Microsoft.Extensions.Hosting;

namespace ASC.Api.Core
{
    public static class HostBuilderExtention
    {
        public static IHostBuilder TryUseWindowsService(this IHostBuilder hostBuilder) =>
           OperatingSystem.IsWindows() ? hostBuilder.UseWindowsService() : hostBuilder;
    }
}
