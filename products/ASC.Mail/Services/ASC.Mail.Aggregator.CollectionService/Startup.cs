
using ASC.Api.Core;
using ASC.Common;
using ASC.Common.Logging;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.Mail.Aggregator.CollectionService
{
    public class Startup : BaseStartup
    {
        public Startup(
            IConfiguration configuration,
            IHostEnvironment hostEnvironment)
            : base(configuration, hostEnvironment)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            DIHelper.TryAdd<AggregatorServiceLauncher>();

            services.AddHostedService<AggregatorServiceLauncher>();

            LogNLogExtension.ConfigureLog(DIHelper, "ASC.Mail.Aggregator", "ASC.Mail.MainThread", "ASC.Mail.Stat", "ASC.Mail.MailboxEngine");
        }
    }
}
