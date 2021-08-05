using System.Text;

using ASC.Api.Core;
using ASC.Api.Projects;
using ASC.Common;
using ASC.Web.Studio.Core.Notify;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.Projects
{

    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
             : base(configuration, hostEnvironment)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            base.ConfigureServices(services);

            DIHelper.TryAdd<BaseProjectController>();
            DIHelper.TryAdd<CommentsController>();
            DIHelper.TryAdd<MessageController>();
            DIHelper.TryAdd<MilestoneController>();
            DIHelper.TryAdd<ProjectController>();
            DIHelper.TryAdd<ReportsController>();
            DIHelper.TryAdd<TagsController>();
            DIHelper.TryAdd<SettingsController>();
            DIHelper.TryAdd<TasksController>();
            DIHelper.TryAdd<TimeSpendController>();
            NotifyConfigurationExtension.Register(DIHelper);

        }

    }
}