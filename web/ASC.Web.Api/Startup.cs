using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ASC.Common.Logging;
using ASC.Web.Api.Handlers;
using ASC.Api.Core.Middleware;
using ASC.Common.Utils;
using ASC.Common.DependencyInjection;
using ASC.Web.Core;
using ASC.Data.Storage.Configuration;
using ASC.MessagingSystem;
using ASC.Data.Reassigns;
using ASC.Core;
using System.Threading;
using ASC.Api.Core.Core;

namespace ASC.Web.Api
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
            services.AddControllers()
                .AddNewtonsoftJson(s=> s.UseCamelCasing(true))
                .AddXmlSerializerFormatters();

            services.AddMemoryCache();

            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddHttpContextAccessor();

            services.AddAuthentication("cookie").AddScheme<AuthenticationSchemeOptions, CookieAuthHandler>("cookie", a=> { });

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
            });

            var container = services.AddAutofac(Configuration, HostEnvironment.ContentRootPath);

            services.AddLogManager()
                    .AddStorage()
                    .AddWebItemManager()
                    .AddScoped<MessageService>()
                    .AddScoped<QueueWorkerReassign>()
                    .AddScoped<QueueWorkerRemove>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();

            app.Use(async (context, next) => {
                if (SecurityContext.IsAuthenticated)
                {
                    var user = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
                    var culture = user.GetCulture();
                    Thread.CurrentThread.CurrentCulture = user.GetCulture();
                    Thread.CurrentThread.CurrentCulture = user.GetCulture();
                }
                await next.Invoke();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapCustom();
            });

            app.UseCSP();
            app.UseCm();
            app.UseWebItemManager();
        }
    }
}
