using System.Text;
using System.Text.Json.Serialization;

using ASC.Api.Core;
using ASC.Api.Documents;
using ASC.Web.Files;
using ASC.Web.Files.HttpHandlers;
using ASC.Web.Studio.Core.Notify;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace ASC.Files
{
    public class Startup : BaseStartup
    { 
        public override JsonConverter[] Converters { get => new JsonConverter[] { new FileEntryWrapperConverter() }; }

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

            NotifyConfigurationExtension.Register(DIHelper);

            services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(Configuration.GetSection("Redis").Get<RedisConfiguration>());
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
