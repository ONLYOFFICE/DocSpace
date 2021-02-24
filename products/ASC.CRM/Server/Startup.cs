using System.Text;

using ASC.Api.Core;
using ASC.Api.CRM;
using ASC.Common;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.CRM
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
             : base(configuration, hostEnvironment)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            DIHelper.TryAdd<CRMController>();

        }   
    }
}
