using ASC.Api.Core.Auth;
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

            services.AddMvc();
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddSession();

            /*services.AddMvc(options => options.EnableEndpointRouting = false)
                .AddNewtonsoftJson();*/

            services.AddAuthentication("cookie").AddScheme<AuthenticationSchemeOptions, CookieAuthHandler>("cookie", a => { });

            services.AddNLogManager("ASC.Api", "ASC.Web");

            services
                .AddCookieAuthHandler()
                .AddStorage()
                .AddPathUtilsService()
                .AddStorageHandlerService();

            services.AddAutofac(Configuration, HostEnvironment.ContentRootPath);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

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

            app.UseStaticFiles();
            app.UseSession();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.InitializeHttpHandlers();
            });
        }
    }
}
