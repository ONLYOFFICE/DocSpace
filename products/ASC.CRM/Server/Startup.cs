using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Api.Core.Auth;
using ASC.Api.Core.Core;
using ASC.Api.Core.Middleware;
using ASC.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ASC.Common.Logging;
using Autofac.Extensions.DependencyInjection;
using ASC.Common.DependencyInjection;
using ASC.Api.CRM;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text;

namespace ASC.CRM
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }

        public IHostEnvironment HostEnvironment { get; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            services.AddHttpContextAccessor();

            services.AddControllers()
                .AddNewtonsoftJson()
                .AddXmlSerializerFormatters();

            services.AddTransient<IConfigureOptions<MvcNewtonsoftJsonOptions>, CustomJsonOptionsWrapper>();

            services.AddMemoryCache();

            services.AddAuthentication("cookie")
                .AddScheme<AuthenticationSchemeOptions, CookieAuthHandler>("cookie", a => { });

            var builder = services.AddMvcCore(config =>
            {
                //var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                //config.Filters.Add(new AuthorizeFilter(policy));
                //config.Filters.Add(new TypeFilterAttribute(typeof(TenantStatusFilter)));
                //config.Filters.Add(new TypeFilterAttribute(typeof(PaymentFilter)));
                //config.Filters.Add(new TypeFilterAttribute(typeof(IpSecurityFilter)));
                //config.Filters.Add(new TypeFilterAttribute(typeof(ProductSecurityFilter)));
                //config.Filters.Add(new CustomResponseFilterAttribute());
                //config.Filters.Add(new CustomExceptionFilterAttribute());
                //config.Filters.Add(new TypeFilterAttribute(typeof(FormatFilter)));

                config.OutputFormatters.RemoveType<XmlSerializerOutputFormatter>();
                config.OutputFormatters.Add(new XmlOutputFormatter());
            });

            var diHelper = new DIHelper(services);
            diHelper
                .AddCookieAuthHandler()
                .AddCultureMiddleware()
                .AddIpSecurityFilter()
                .AddPaymentFilter()
                .AddProductSecurityFilter()
                .AddTenantStatusFilter();


            diHelper.AddNLogManager("ASC.CRM");

            diHelper.AddCRMControllerService();

            services.AddAutofac(Configuration, HostEnvironment.ContentRootPath);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
