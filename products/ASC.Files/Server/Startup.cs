
using System.Text;
using System.Text.Json.Serialization;

using ASC.Api.Core;
using ASC.Api.Documents;
using ASC.Common;
using ASC.Common.Security.Authentication;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Web.Files;
using ASC.Web.Files.HttpHandlers;
using ASC.Web.Studio.Core.Notify;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.Files
{
    public class Startup : BaseStartup
    {
        public override string[] LogParams { get => new string[] { "ASC.Files" }; }
        public override JsonConverter[] Converters { get => new JsonConverter[] { new FileEntryWrapperConverter() }; }

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
            : base(configuration, hostEnvironment)
        {

        }

        public override void ConfigureServices(IServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            services.AddMemoryCache();

            var diHelper = new DIHelper(services);

            diHelper
                .AddApiProductEntryPointService()
                .AddDocumentsControllerService()
                .AddPrivacyRoomApiService()
                .AddFileHandlerService()
                .AddChunkedUploaderHandlerService()
                .AddThirdPartyAppHandlerService()
                .AddDocuSignHandlerService()
                .AddNotifyConfiguration();

            base.ConfigureServices(services);
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);

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
        }
    }
}
