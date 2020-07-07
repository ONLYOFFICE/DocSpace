using ASC.Api.Core.Core;
using ASC.Api.Core.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using ASC.Api.Core.Auth;
using ASC.Common.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using ASC.Common;
using ASC.Common.Logging;

namespace ASC.Api.Core
{
    public abstract class BaseStartup
    {
        public IConfiguration Configuration { get; }
        public IHostEnvironment HostEnvironment { get; }
        public string[] LogParams { get; set; }
        public bool addcontrollers = false;
        public bool confirmAddScheme = false;

        public BaseStartup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }  
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            if (addcontrollers)
            {
                services.AddControllers()
                    .AddXmlSerializerFormatters()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.WriteIndented = false;
                        options.JsonSerializerOptions.IgnoreNullValues = true;
                        options.JsonSerializerOptions.Converters.Add(new ApiDateTimeConverter());
                    });
            }
            var builder = services.AddMvcCore(config =>
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

            if (confirmAddScheme)
            {
                services.AddAuthentication("cookie")
                       .AddScheme<AuthenticationSchemeOptions, CookieAuthHandler>("cookie", a => { })
                       .AddScheme<AuthenticationSchemeOptions, ConfirmAuthHandler>("confirm", a => { });
            }
            else
            {
                services.AddAuthentication("cookie")
                       .AddScheme<AuthenticationSchemeOptions, CookieAuthHandler>("cookie", a => { });
            }

            var diHelper = new DIHelper(services);

            if (LogParams != null) {
                diHelper.AddNLogManager(LogParams);
            }

            services.AddAutofac(Configuration, HostEnvironment.ContentRootPath);
        }
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCultureMiddleware();

            app.UseDisposeMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapCustom();
            });
        }
    }
}