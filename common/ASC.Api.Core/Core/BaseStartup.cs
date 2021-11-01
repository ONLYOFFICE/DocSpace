﻿using System;
using System.Reflection;
using System.Text.Json.Serialization;

using ASC.Api.Core.Auth;
using ASC.Api.Core.Convention;
using ASC.Api.Core.Core;
using ASC.Api.Core.Middleware;
using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.DependencyInjection;
using ASC.Common.Logging;
using ASC.Common.Mapping;
using ASC.Common.Utils;
using ASC.Webhooks.Core;

using Autofac;

using HealthChecks.UI.Client;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NLog;
using NLog.Extensions.Logging;

namespace ASC.Api.Core
{
    public abstract class BaseStartup
    {
        public IConfiguration Configuration { get; }
        public IHostEnvironment HostEnvironment { get; }
        public virtual JsonConverter[] Converters { get; }
        public virtual bool AddControllersAsServices { get; } = false;
        public virtual bool ConfirmAddScheme { get; } = false;
        public virtual bool AddAndUseSession { get; } = false;
        protected DIHelper DIHelper { get; }
        protected bool LoadProducts { get; set; } = true;
        protected bool LoadConsumers { get; } = true;

        public BaseStartup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
            DIHelper = new DIHelper();
            if (bool.TryParse(Configuration["core:products"], out var loadProducts))
            {
                LoadProducts = loadProducts;
            }
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomHealthCheck(Configuration);
            services.AddHttpContextAccessor();
            services.AddMemoryCache();

            if (AddAndUseSession)
                services.AddSession();

            DIHelper.Configure(services);

            Action<JsonOptions> jsonOptions = options =>
                               {
                                   options.JsonSerializerOptions.WriteIndented = false;
                                   options.JsonSerializerOptions.IgnoreNullValues = true;
                                   options.JsonSerializerOptions.Converters.Add(new ApiDateTimeConverter());

                                   if (Converters != null)
                                   {
                                       foreach (var c in Converters)
                                       {
                                           options.JsonSerializerOptions.Converters.Add(c);
                                       }
                                   }
                               };

            services.AddControllers()
                .AddXmlSerializerFormatters()
                .AddJsonOptions(jsonOptions);

            services.AddSingleton(jsonOptions);

            DIHelper.TryAdd<DisposeMiddleware>();
            DIHelper.TryAdd<CultureMiddleware>();
            DIHelper.TryAdd<IpSecurityFilter>();
            DIHelper.TryAdd<PaymentFilter>();
            DIHelper.TryAdd<ProductSecurityFilter>();
            DIHelper.TryAdd<TenantStatusFilter>();
            DIHelper.TryAdd<ConfirmAuthHandler>();
            DIHelper.TryAdd<CookieAuthHandler>();
            DIHelper.TryAdd<WebhooksGlobalFilterAttribute>();

            DIHelper.TryAdd(typeof(ICacheNotify<>), typeof(KafkaCache<>));
            DIHelper.TryAdd(typeof(IWebhookPublisher), typeof(WebhookPublisher));

            if (LoadProducts)
            {
                DIHelper.RegisterProducts(Configuration, HostEnvironment.ContentRootPath);
            }

            var builder = services.AddMvcCore(config =>
            {
                config.Conventions.Add(new ControllerNameAttributeConvention());

                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

                config.Filters.Add(new AuthorizeFilter(policy));
                config.Filters.Add(new TypeFilterAttribute(typeof(TenantStatusFilter)));
                config.Filters.Add(new TypeFilterAttribute(typeof(PaymentFilter)));
                config.Filters.Add(new TypeFilterAttribute(typeof(IpSecurityFilter)));
                config.Filters.Add(new TypeFilterAttribute(typeof(ProductSecurityFilter)));
                config.Filters.Add(new CustomResponseFilterAttribute());
                config.Filters.Add(new CustomExceptionFilterAttribute());
                config.Filters.Add(new TypeFilterAttribute(typeof(FormatFilter)));
                config.Filters.Add(new TypeFilterAttribute(typeof(WebhooksGlobalFilterAttribute)));

                config.OutputFormatters.RemoveType<XmlSerializerOutputFormatter>();
                config.OutputFormatters.Add(new XmlOutputFormatter());
            });


            var authBuilder = services.AddAuthentication("cookie")
                .AddScheme<AuthenticationSchemeOptions, CookieAuthHandler>("cookie", a => { });

            if (ConfirmAddScheme)
            {
                authBuilder.AddScheme<AuthenticationSchemeOptions, ConfirmAuthHandler>("confirm", a => { });
            }

            services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseRouting();

            if (AddAndUseSession)
                app.UseSession();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCultureMiddleware();

            app.UseDisposeMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapCustom();

                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.Register(Configuration, LoadProducts, LoadConsumers);
        }
    }

    public static class LogNLogConfigureExtenstion
    {
        public static IHostBuilder ConfigureNLogLogging(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureLogging((hostBuildexContext, r) =>
            {
                _ = new ConfigureLogNLog(hostBuildexContext.Configuration, new ConfigurationExtension(hostBuildexContext.Configuration), hostBuildexContext.HostingEnvironment);
                r.AddNLog(LogManager.Configuration);
            });
        }
    }
}