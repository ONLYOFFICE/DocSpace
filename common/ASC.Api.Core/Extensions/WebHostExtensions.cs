namespace ASC.Api.Core.Extensions;

public static class WebHostExtensions
{
    public static IWebHostBuilder ConfigureDefaultKestrel(this IWebHostBuilder webHostBuilder, Action<WebHostBuilderContext, KestrelServerOptions> configureDelegate = null)
    {
        webHostBuilder.ConfigureKestrel((hostingContext, serverOptions) =>
        {
            var kestrelConfig = hostingContext.Configuration.GetSection("Kestrel");

            if (!kestrelConfig.Exists())
            {
                return;
            }

            var unixSocket = kestrelConfig.GetValue<string>("ListenUnixSocket");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (!string.IsNullOrWhiteSpace(unixSocket))
                {
                    unixSocket = string.Format(unixSocket, hostingContext.HostingEnvironment.ApplicationName.Replace("ASC.", "").Replace(".", ""));

                    serverOptions.ListenUnixSocket(unixSocket);
                }
            }

            configureDelegate?.Invoke(hostingContext, serverOptions);
        });

        return webHostBuilder;
    }
}
