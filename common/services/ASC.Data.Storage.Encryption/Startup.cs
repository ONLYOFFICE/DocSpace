
using ASC.Api.Core;
using ASC.Common;

using Autofac.Extensions.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ASC.Common.DependencyInjection;

namespace ASC.Data.Storage.Encryption
{
    public class Startup : BaseStartup
    {
        public override string[] LogParams { get => new string[] { "ASC.Data.Storage.Encryption" }; }

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment) : base(configuration, hostEnvironment)
        {

        }

        public override void ConfigureServices(IServiceCollection services)
        {
            var diHelper = new DIHelper(services);

            diHelper.AddEncryptionServiceLauncher();

            services.AddHostedService<EncryptionServiceLauncher>();

            base.ConfigureServices(services);

            services.AddAutofac(Configuration, HostEnvironment.ContentRootPath);
        }
    }
}
