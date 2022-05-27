using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;

using ASC.Api.Core;
using ASC.Common;
using ASC.CRM.Api;
using ASC.CRM.ApiModels;
using ASC.CRM.HttpHandlers;
using ASC.CRM.Mapping;
using ASC.Web.CRM.Core.Search;
using ASC.Web.CRM.HttpHandlers;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            base.ConfigureServices(services);

            services.AddHttpClient("DownloadCurrencyPage").ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler()
                {
                    AllowAutoRedirect = true,
                    MaxAutomaticRedirections = 2,
                    UseDefaultCredentials = true
                };
            });

            DIHelper.TryAdd<EntryPointApiController>();
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
            DIHelper.TryAdd<ReportsController>();
            DIHelper.TryAdd<TagsController>();
            DIHelper.TryAdd<TasksController>();
            DIHelper.TryAdd<TaskTemplateController>();
            DIHelper.TryAdd<UtilsController>();
            DIHelper.TryAdd<VoIPController>();

            DIHelper.TryAdd<CasesDtoTypeConverter>();
            DIHelper.TryAdd<ContactDtoTypeConverter>();
            DIHelper.TryAdd<InvoiceBaseDtoTypeConverter>();
            DIHelper.TryAdd<InvoiceDtoTypeConverter>();
            DIHelper.TryAdd<InvoiceItemDtoTypeConverter>();
            DIHelper.TryAdd<InvoiceTaxDtoTypeConverter>();
            DIHelper.TryAdd<OpportunityDtoTypeConverter>();
            DIHelper.TryAdd<RelationshipEventDtoTypeConverter>();
            DIHelper.TryAdd<ListItemDtoTypeConverter>();
            DIHelper.TryAdd<TaskDtoTypeConverter>();
            DIHelper.TryAdd<CustomFieldDtoTypeConverter>();
            DIHelper.TryAdd<DealMilestoneDtoTypeConverter>();
            DIHelper.TryAdd<VoipCallDtoTypeConverter>();

            DIHelper.TryAdd<FactoryIndexerCase>();
            DIHelper.TryAdd<FactoryIndexerContact>();
            DIHelper.TryAdd<FactoryIndexerContactInfo>();
            DIHelper.TryAdd<FactoryIndexerDeal>();
            DIHelper.TryAdd<FactoryIndexerEvents>();
            DIHelper.TryAdd<FactoryIndexerFieldValue>();
            DIHelper.TryAdd<FactoryIndexerInvoice>();
            DIHelper.TryAdd<FactoryIndexerTask>();
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);

            app.UseMiddleware<TenantConfigureMiddleware>();

            app.MapWhen(
                context => context.Request.Path.ToString().EndsWith("httphandlers/contactphotohandler.ashx"),
                appBranch =>
                {
                    appBranch.UseContactPhotoHandler();
                });

            app.MapWhen(
              context => context.Request.Path.ToString().EndsWith("httphandlers/filehandler.ashx"),
              appBranch =>
              {
                  appBranch.UseFileHandler();
              });

            app.MapWhen(
              context => context.Request.Path.ToString().EndsWith("httphandlers/fileuploaderhandler.ashx"),
              appBranch =>
              {
                  appBranch.UseFileUploaderHandler();
              });

            app.MapWhen(
              context => context.Request.Path.ToString().EndsWith("httphandlers/importfilehandler.ashx"),
              appBranch =>
              {
                  appBranch.UseImportFileHandlerHandler();
              });

            app.MapWhen(
              context => context.Request.Path.ToString().EndsWith("httphandlers/organisationlogohandler.ashx"),
              appBranch =>
              {
                  appBranch.UseOrganisationLogoHandler();
              });


            app.MapWhen(
              context => context.Request.Path.ToString().EndsWith("httphandlers/webtoleadfromhandler.ashx"),
              appBranch =>
              {
                  appBranch.UseWebToLeadFromHandlerHandler();
              });

        }

        public override JsonConverter[] Converters
        {
            get
            {
                var jsonConverters = new List<JsonConverter>
                {
                    new ContactDtoJsonConverter()
                };

                return jsonConverters.ToArray();
            }
        }

    }
}


