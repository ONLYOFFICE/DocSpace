
using System;

using ASC.Api.Core.Auth;
using ASC.Api.Core.Core;
using ASC.Api.Core.Middleware;
using ASC.Common.DependencyInjection;
using ASC.Common.Logging;
using ASC.Common.Threading.Progress;
using ASC.Common.Threading.Workers;
using ASC.Data.Reassigns;
using ASC.Calendar.Controllers;
using ASC.Web.Core.Users;

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
using ASC.Common;
using ASC.Api.Core;
using ASC.Calendar.BusinessObjects;
using ASC.Calendar.Core;
using ASC.Calendar.Models;

namespace ASC.Calendar
{
    public class Startup : BaseStartup
    {
        
        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
            : base(configuration, hostEnvironment)
        {

        }

        public override void ConfigureServices(IServiceCollection services)
        {          
            services.AddMemoryCache();

            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddAuthentication("cookie")
                .AddScheme<AuthenticationSchemeOptions, CookieAuthHandler>("cookie", a => { })
                .AddScheme<AuthenticationSchemeOptions, ConfirmAuthHandler>("confirm", a => { });

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

            DIHelper.TryAdd<CalendarController>();
            DIHelper.TryAdd<DataProvider>();
            DIHelper.TryAdd<EventHistory>();
            DIHelper.TryAdd<ExportDataCache>();
            DIHelper.TryAdd<CalendarWrapper>();
            DIHelper.TryAdd<EventWrapper>();
            DIHelper.TryAdd<EventWrapper>();

            var diHelper = new DIHelper(services);
            diHelper
                .AddCookieAuthHandler()
                .AddCultureMiddleware()
                .AddIpSecurityFilter()
                .AddPaymentFilter()
                .AddProductSecurityFilter()
                .AddTenantStatusFilter();


            diHelper.AddNLogManager("ASC.Api", "ASC.Web");

            diHelper
                .AddCalendarController();

            services.AddAutofac(Configuration, HostEnvironment.ContentRootPath);
        }

        public override  void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
