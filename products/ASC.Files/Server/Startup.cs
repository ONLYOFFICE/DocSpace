using System;
using System.Text;
using System.Text.Json.Serialization;

using ASC.Api.Core;
using ASC.Api.Documents;
using ASC.Files.Core.Security;
using ASC.Files.Core.Services.OFormService;
using ASC.Web.Files;
using ASC.Web.Files.HttpHandlers;
using ASC.Web.Studio.Core.Notify;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.Files
{
    public class Startup : BaseStartup
    {
        public override JsonConverter[] Converters { get => new JsonConverter[] { new FileEntryWrapperConverter(), new FileShareConverter() }; }

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
            : base(configuration, hostEnvironment)
        {

        }

        public override void ConfigureServices(IServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            services.AddMemoryCache();

            base.ConfigureServices(services);

            DIHelper.TryAdd<FilesController>();
            DIHelper.TryAdd<PrivacyRoomController>();
            DIHelper.TryAdd<FileHandlerService>();
            DIHelper.TryAdd<ChunkedUploaderHandlerService>();
            DIHelper.TryAdd<DocuSignHandlerService>();
            DIHelper.TryAdd<ThirdPartyAppHandlerService>();
            DIHelper.TryAdd<OFormService>();

            services.AddHostedService<OFormService>();
            NotifyConfigurationExtension.Register(DIHelper);
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());

            base.Configure(app, env);

            app.MapWhen(
                context => context.Request.Path.ToString().EndsWith("httphandlers/filehandler.ashx", StringComparison.OrdinalIgnoreCase),
                appBranch =>
                {
                    appBranch.UseFileHandler();
                });

            app.MapWhen(
                context => context.Request.Path.ToString().EndsWith("ChunkedUploader.ashx", StringComparison.OrdinalIgnoreCase),
                appBranch =>
                {
                    appBranch.UseChunkedUploaderHandler();
                });

            app.MapWhen(
                context => context.Request.Path.ToString().EndsWith("ThirdPartyApp", StringComparison.OrdinalIgnoreCase),
                appBranch =>
                {
                    appBranch.UseThirdPartyAppHandler();
                });

            app.MapWhen(
                context => context.Request.Path.ToString().EndsWith("httphandlers/DocuSignHandler.ashx", StringComparison.OrdinalIgnoreCase),
                appBranch =>
                {
                    appBranch.UseDocuSignHandler();
                });
        }
    }
}
