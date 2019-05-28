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
using ASC.Web.Api.Middleware;

using Autofac;
using Autofac.Configuration;


namespace ASC.Web.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(s=> s.UseCamelCasing(true))
                .AddXmlSerializerFormatters();

            services.AddHttpContextAccessor();

            services.AddAuthentication("cookie").AddScheme<AuthenticationSchemeOptions, CookieAuthHandler>("cookie", a=> { });

            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                config.Filters.Add(new TypeFilterAttribute(typeof(FormatFilter)));
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            var module = new ConfigurationModule(Configuration);
            var builder = new ContainerBuilder();
            builder.RegisterModule(module);

            var container = builder.Build();

            services.AddSingleton(container);
            services.AddSingleton<LogManager>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder => builder.AllowAnyOrigin());

            app.UseRouting();

            app.UseAuthentication();

            app.UseResponseWrapper();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
