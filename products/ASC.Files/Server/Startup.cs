
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
                .AddEncryptionControllerService()
                .AddFileHandlerService()
                .AddChunkedUploaderHandlerService()
                .AddThirdPartyAppHandlerService()
                .AddDocuSignHandlerService();

            base.ConfigureServices(services);
        }
        public void Migrations(IApplicationBuilder app)
        {
            using (var Service = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
               
                using (var Base = Service.ServiceProvider.GetService<DbContextManager<AccountLinkContext>>())
                {
                    Base.Value.Database.Migrate();
                }
                using (var Base = Service.ServiceProvider.GetService<DbContextManager<FilesDbContext>>())
                 {
                     Base.Value.Database.Migrate();
                 }
                 using (var Base = Service.ServiceProvider.GetService<DbContextManager<CoreDbContext>>())
                 {
                     Base.Value.Database.Migrate();
                 }
                 using (var Base = Service.ServiceProvider.GetService<DbContextManager<TenantDbContext>>())
                 {
                     Base.Value.Database.Migrate();
                 }
                 using (var Base = Service.ServiceProvider.GetService<DbContextManager<UserDbContext>>())
                 {
                     Base.Value.Database.Migrate();
                 }
                 using (var Base = Service.ServiceProvider.GetService<DbContextManager<WebstudioDbContext>>())
                 {
                     Base.Value.Database.Migrate();
                 }
                 using (var Base = Service.ServiceProvider.GetService<DbContextManager<FeedDbContext>>())
                 {
                     Base.Value.Database.Migrate();
                 }
                 using (var Base = Service.ServiceProvider.GetService<DbContextManager<NotifyDbContext>>())
                 {
                     Base.Value.Database.Migrate();
                 }
                 using (var Base = Service.ServiceProvider.GetService<DbContextManager<ASC.Core.Common.EF.Context.DbContext>>())
                 {
                     Base.Value.Database.Migrate();
                 }
                 using (var Base = Service.ServiceProvider.GetService<DbContextManager<ResourceDbContext>>())
                 {
                     Base.Value.Database.Migrate();
                 }
                 using (var Base = Service.ServiceProvider.GetService<DbContextManager<VoipDbContext>>())
                 {
                     Base.Value.Database.Migrate();
                 }
                 using (var Base = Service.ServiceProvider.GetService<DbContextManager<MessagesContext>>())
                 {
                     Base.Value.Database.Migrate();
                 }
                 using (var Base = Service.ServiceProvider.GetService<DbContextManager<ASC.Files.Core.EF.FilesDbContext>>())
                 {
                     Base.Value.Database.Migrate();
                 }
            }
        }
        
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            base.Configure(app, env);

            Migrations(app);

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
