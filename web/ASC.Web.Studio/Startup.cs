using ASC.Api.Core;
using ASC.Api.Core.Auth;
using ASC.Common;
using ASC.Common.DependencyInjection;
using ASC.Common.Logging;
using ASC.Data.Storage;
using ASC.Data.Storage.Configuration;
using ASC.Data.Storage.DiscStorage;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASC.Web.Studio
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

            services.AddCors();
            
            var diHelper = new DIHelper(services);

            diHelper.AddNLogManager("ASC.Api", "ASC.Web");

            diHelper
                .AddCookieAuthHandler()
                .AddStorage()
                .AddPathUtilsService()
                .AddStorageHandlerService();
            GeneralStartup.ConfigureServices(services, false, false);
            services.AddAutofac(Configuration, HostEnvironment.ContentRootPath);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
        }
    }
}
