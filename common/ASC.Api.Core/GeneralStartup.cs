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

namespace ASC.Api.Core
{
    public static class GeneralStartup
    {
        public static void ConfigureServices(IServiceCollection services, bool confirmAddScheme, bool addcontrollers)
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
        }
        public static void Configure(IApplicationBuilder app)
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