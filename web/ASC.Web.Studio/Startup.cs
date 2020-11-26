
using ASC.Api.Core;
using ASC.Common.DependencyInjection;
using ASC.Data.Storage;
using ASC.Data.Storage.DiscStorage;
using ASC.FederatedLogin;

using Autofac;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.Web.Studio
{
    public class Startup : BaseStartup
    {
        public override string[] LogParams { get => new string[] { "ASC.Web" }; }

        public override bool AddControllers { get => false; }

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment) : base(configuration, hostEnvironment)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            base.ConfigureServices(services);

            services.AddMemoryCache();
            DIHelper.TryAdd<Login>();
            DIHelper.TryAdd<PathUtils>();
            DIHelper.TryAdd<StorageHandlerScope>();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.Register(Configuration, HostEnvironment.ContentRootPath);
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseRouting();

            app.UseCors(builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.InitializeHttpHandlers();
            });

            app.MapWhen(
               context => context.Request.Path.ToString().EndsWith("login.ashx"),
               appBranch =>
               {
                   appBranch.UseLoginHandler();
               });
        }
    }
}
