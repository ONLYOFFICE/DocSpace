
using System;

using ASC.Api.Core.Auth;
using ASC.Api.Core.Core;
using ASC.Api.Core.Middleware;
using ASC.Common.DependencyInjection;
using ASC.Common.Logging;
using ASC.Common.Threading.Workers;
using ASC.Web.Files.Services.WCFService;

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
using Microsoft.Extensions.Options;

using static ASC.Web.Files.Utils.FileMarker;

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
            services.AddHttpContextAccessor();

            services.AddControllers()
                .AddNewtonsoftJson()
                .AddXmlSerializerFormatters();

            services.AddTransient<IConfigureOptions<MvcNewtonsoftJsonOptions>, CustomJsonOptionsWrapper>();

            services.AddMemoryCache();

            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddAuthentication("cookie")
                .AddScheme<AuthenticationSchemeOptions, CookieAuthHandler>("cookie", a => { });

            var builder = services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                config.Filters.Add(new AuthorizeFilter(policy));
                config.Filters.Add(new TypeFilterAttribute(typeof(TenantStatusFilter)));
                config.Filters.Add(new TypeFilterAttribute(typeof(PaymentFilter)));
                config.Filters.Add(new TypeFilterAttribute(typeof(IpSecurityFilter)));
                config.Filters.Add(new TypeFilterAttribute(typeof(ProductSecurityFilter)));
                config.Filters.Add(new CustomResponseFilterAttribute());
                config.Filters.Add(new CustomExceptionFilterAttribute());
                config.Filters.Add(new TypeFilterAttribute(typeof(FormatFilter)));

                config.OutputFormatters.RemoveType<XmlSerializerOutputFormatter>();
                config.OutputFormatters.Add(new XmlOutputFormatter());
            });

            services
                .AddCookieAuthHandler()
                .AddCultureMiddleware()
                .AddIpSecurityFilter()
                .AddPaymentFilter()
                .AddProductSecurityFilter()
                .AddTenantStatusFilter();

            services.Configure<WorkerQueue<AsyncTaskData>>(r =>
            {
                r.workerCount = 1;
                r.waitInterval = (int)TimeSpan.FromSeconds(60).TotalMilliseconds;
                r.errorCount = 1;
                r.stopAfterFinsih = false;
            });

            services.AddNLogManager("ASC.Files");

            services.AddFileStorageServiceController();

            services.AddAutofac(Configuration, HostEnvironment.ContentRootPath);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseCors(builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCultureMiddleware();

            app.UseDisposeMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapCustom();
            });

            app.UseStaticFiles();
        }
    }
}
