
using System.Text;

using ASC.Api.Core;
using ASC.Api.Core.Auth;
using ASC.Api.Core.Core;
using ASC.Api.Core.Middleware;
using ASC.Api.Documents;
using ASC.Common;
using ASC.Common.DependencyInjection;
using ASC.Common.Logging;
using ASC.Web.Files;
using ASC.Web.Files.HttpHandlers;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.Files
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostEnvironment HostEnvironment { get; }

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


            services.AddControllers()
                .AddXmlSerializerFormatters()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.WriteIndented = false;
                    options.JsonSerializerOptions.IgnoreNullValues = true;
                    options.JsonSerializerOptions.Converters.Add(new ApiDateTimeConverter());
                    options.JsonSerializerOptions.Converters.Add(new FileEntryWrapperConverter());
                });

            services.AddMemoryCache();

            var diHelper = new DIHelper(services);
            diHelper
                .AddCookieAuthHandler()
                .AddCultureMiddleware()
                .AddIpSecurityFilter()
                .AddPaymentFilter()
                .AddProductSecurityFilter()
                .AddTenantStatusFilter();

            diHelper.AddNLogManager("ASC.Files");

            diHelper
                .AddDocumentsControllerService()
                .AddEncryptionControllerService()
                .AddFileHandlerService()
                .AddChunkedUploaderHandlerService()
                .AddThirdPartyAppHandlerService()
                .AddDocuSignHandlerService();

            GeneralStartup.ConfigureServices(services, false, false);
            services.AddAutofac(Configuration, HostEnvironment.ContentRootPath);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            GeneralStartup.Configure(app);

            app.MapWhen(
                context => context.Request.Path.ToString().EndsWith("httphandlers/filehandler.ashx"),
                appBranch =>
                {
                    appBranch.UseFileHandler();
                });

            app.MapWhen(
                context => context.Request.Path.ToString().EndsWith("ChunkedUploader.ashx"),
                appBranch =>
                {
                    appBranch.UseChunkedUploaderHandler();
                });

            app.MapWhen(
                context => context.Request.Path.ToString().EndsWith("ThirdPartyAppHandler.ashx"),
                appBranch =>
                {
                    appBranch.UseThirdPartyAppHandler();
                });

            app.MapWhen(
                context => context.Request.Path.ToString().EndsWith("DocuSignHandler.ashx"),
                appBranch =>
                {
                    appBranch.UseDocuSignHandler();
                });

            app.UseStaticFiles();
        }
    }
}
