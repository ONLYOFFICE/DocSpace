using ASC.Api.Core.Auth;
using ASC.Common.DependencyInjection;
using ASC.Common.Utils;
using ASC.Data.Storage;
using ASC.Data.Storage.Configuration;
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

            services.AddAutofac(Configuration, HostEnvironment.ContentRootPath);

            services.AddAuthentication("cookie").AddScheme<AuthenticationSchemeOptions, CookieAuthHandler>("cookie", a => { });

            services.AddHttpContextAccessor()
                .AddStorage(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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

            app.UseCm();

            app.UseEndpoints(endpoints =>
            {
                endpoints.InitializeHttpHandlers();
            });
        }
    }
}
