using System.Text;

using ASC.Api.Core;
using ASC.CRM.Api;
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

            DIHelper.TryAdd<CasesController>();
            DIHelper.TryAdd<ContactInfosController>();
            DIHelper.TryAdd<ContactsController>();
            DIHelper.TryAdd<CurrencyRatesController>();
            DIHelper.TryAdd<CustomFieldsController>();
            DIHelper.TryAdd<TasksController>();
            DIHelper.TryAdd<DealsController>();
            DIHelper.TryAdd<InvoicesController>();
            DIHelper.TryAdd<ListItemsController>();
            DIHelper.TryAdd<RelationshipEventsController>();
            DIHelper.TryAdd<TagsController>();
            DIHelper.TryAdd<TasksController>();
            DIHelper.TryAdd<TaskTemplateController>();
            DIHelper.TryAdd<UtilsController>();
        }   
    }
}
