using System.Configuration;

using ASC.Api.Core;
using ASC.Api.Core.Auth;
using ASC.Api.Core.Core;
using ASC.Api.Core.Middleware;
using ASC.Common.Data;
using ASC.Common.DependencyInjection;
using ASC.Common.Logging;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Caching;
using ASC.Core.Common.Settings;
using ASC.Core.Data;
using ASC.Core.Notify;
using ASC.Core.Security.Authorizing;
using ASC.Core.Tenants;
using ASC.Data.Reassigns;
using ASC.Data.Storage;
using ASC.Data.Storage.Configuration;
using ASC.IPSecurity;
using ASC.MessagingSystem;
using ASC.Notify.Recipients;
using ASC.Security.Cryptography;
using ASC.Web.Core;
using ASC.Web.Core.Notify;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Core.WhiteLabel;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ASC.People
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

            services.AddControllers().AddControllersAsServices()
                .AddNewtonsoftJson()
                .AddXmlSerializerFormatters();

            services.AddTransient<IConfigureOptions<MvcNewtonsoftJsonOptions>, CustomJsonOptionsWrapper>();

            services.AddMemoryCache();

            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddAuthentication("cookie")
                .AddScheme<AuthenticationSchemeOptions, CookieAuthHandler>("cookie", a => { })
                .AddScheme<AuthenticationSchemeOptions, ConfirmAuthHandler>("confirm", a => { });

            var builder = services.AddMvc(config =>
            {
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

            var container = services.AddAutofac(Configuration, HostEnvironment.ContentRootPath);


            services.AddLogManager()
                    .AddStorage()
                    .AddWebItemManager()
                    .AddSingleton((r) =>
                    {
                        var cs = DbRegistry.GetConnectionString("core");
                        if (cs == null)
                        {
                            throw new ConfigurationErrorsException("Can not configure CoreContext: connection string with name core not found.");
                        }
                        return (ISubscriptionService)new CachedSubscriptionService(new DbSubscriptionService(cs));
                    })
                    .AddSingleton((r) =>
                    {
                        var cs = DbRegistry.GetConnectionString("core");
                        if (cs == null)
                        {
                            throw new ConfigurationErrorsException("Can not configure CoreContext: connection string with name core not found.");
                        }
                        return (IAzService)new CachedAzService(new DbAzService(cs));
                    })
                    .AddSingleton((r) =>
                    {
                        var cs = DbRegistry.GetConnectionString("core");
                        if (cs == null)
                        {
                            throw new ConfigurationErrorsException("Can not configure CoreContext: connection string with name core not found.");
                        }
                        return (IUserService)new CachedUserService(new DbUserService(cs), r.GetService<CoreBaseSettings>());
                    })
                    .AddSingleton((r) =>
                    {
                        var cs = DbRegistry.GetConnectionString("core");
                        if (cs == null)
                        {
                            throw new ConfigurationErrorsException("Can not configure CoreContext: connection string with name core not found.");
                        }
                        return (ITenantService)new CachedTenantService(new DbTenantService(cs), r.GetService<CoreBaseSettings>());
                    })
                    .AddSingleton((r) =>
                    {
                        var quotaCacheEnabled = false;
                        if (Common.Utils.ConfigurationManager.AppSettings["core:enable-quota-cache"] == null)
                        {
                            quotaCacheEnabled = true;
                        }
                        else
                        {
                            quotaCacheEnabled = !bool.TryParse(Common.Utils.ConfigurationManager.AppSettings["core:enable-quota-cache"], out var enabled) || enabled;
                        }

                        var cs = DbRegistry.GetConnectionString("core");
                        if (cs == null)
                        {
                            throw new ConfigurationErrorsException("Can not configure CoreContext: connection string with name core not found.");
                        }
                        return quotaCacheEnabled ? (IQuotaService)new CachedQuotaService(new DbQuotaService(cs)) : new DbQuotaService(cs); ;
                    })
                    .AddSingleton((r) =>
                    {
                        var cs = DbRegistry.GetConnectionString("core");
                        if (cs == null)
                        {
                            throw new ConfigurationErrorsException("Can not configure CoreContext: connection string with name core not found.");
                        }
                        return (ITariffService)new TariffService(cs, r.GetService<IQuotaService>(), r.GetService<ITenantService>(), r.GetService<CoreBaseSettings>(), r.GetService<CoreSettings>());
                    })
                    .AddScoped<ApiContext>()
                    .AddScoped<StudioNotifyService>()
                    .AddScoped<UserManagerWrapper>()
                    .AddScoped<MessageService>()
                    .AddScoped<QueueWorkerReassign>()
                    .AddScoped<QueueWorkerRemove>()
                    .AddScoped<TenantManager>()
                    .AddScoped<UserManager>()
                    .AddScoped<StudioNotifyHelper>()
                    .AddScoped<StudioNotifySource>()
                    .AddScoped<StudioNotifyServiceHelper>()
                    .AddScoped<AuthManager>()
                    .AddScoped<TenantExtra>()
                    .AddScoped<TenantStatisticsProvider>()
                    .AddScoped<SecurityContext>()
                    .AddScoped<AzManager>()
                    .AddScoped<WebItemSecurity>()
                    .AddScoped<UserPhotoManager>()
                    .AddScoped<CookiesManager>()
                    .AddScoped<PermissionContext>()
                    .AddScoped<AuthContext>()
                    .AddScoped<MessageFactory>()
                    .AddScoped<WebImageSupplier>()
                    .AddScoped<UserPhotoThumbnailSettings>()
                    .AddScoped<TenantCookieSettings>()
                    .AddScoped<WebItemManagerSecurity>()
                    .AddScoped<DbSettingsManager>()
                    .AddScoped<SettingsManager>()
                    .AddScoped<WebPath>()
                    .AddScoped<AdditionalWhiteLabelSettings>()
                    .AddScoped<TenantAccessSettings>()
                    .AddScoped<MailWhiteLabelSettings>()
                    .AddScoped<PasswordSettings>()
                    .AddScoped<StaticUploader>()
                    .AddScoped<CdnStorageSettings>()
                    .AddScoped<StorageFactory>()
                    .AddSingleton<StorageFactoryListener>()
                    .AddSingleton<StorageFactoryConfig>()
                    .AddScoped<StorageSettings>()
                    .AddScoped<IPRestrictionsSettings>()
                    .AddScoped<CustomNamingPeople>()
                    .AddScoped<PeopleNamesSettings>()
                    .AddScoped<EmailValidationKeyProvider>()
                    .AddScoped<TenantUtil>()
                    .AddScoped<PaymentManager>()
                    .AddScoped<AuthorizationManager>()
                    .AddScoped<CoreConfiguration>()
                    .AddSingleton<CoreSettings>()
                    .AddSingleton<WebPathSettings>()
                    .AddSingleton<BaseStorageSettingsListener>()
                    .AddSingleton<CoreBaseSettings>()
                    .AddSingleton<SubscriptionManager>()
                    .AddScoped(typeof(IRecipientProvider), typeof(RecipientProviderImpl))
                    .AddSingleton(typeof(IRoleProvider), typeof(RoleProvider))
                    .AddScoped(typeof(IPermissionResolver), typeof(PermissionResolver))
                    .AddScoped(typeof(IPermissionProvider), typeof(PermissionProvider))
                    ;
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

            app.UseCSP();
            app.UseCm();
            app.UseStaticFiles();
        }
    }
}
