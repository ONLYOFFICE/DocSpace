
using System;
using ASC.Api.Core;
using ASC.Api.Core.Auth;
using ASC.Api.Core.Core;
using ASC.Api.Core.Middleware;
using ASC.Common.Caching;
using ASC.Common.Data;
using ASC.Common.DependencyInjection;
using ASC.Common.Logging;
using ASC.Common.Security.Authorizing;
using ASC.Common.Threading.Progress;
using ASC.Common.Threading.Workers;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Caching;
using ASC.Core.Common;
using ASC.Core.Common.Settings;
using ASC.Core.Data;
using ASC.Core.Notify;
using ASC.Core.Security.Authentication;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Data.Reassigns;
using ASC.Data.Storage;
using ASC.Data.Storage.Configuration;
using ASC.FederatedLogin;
using ASC.IPSecurity;
using ASC.MessagingSystem;
using ASC.MessagingSystem.DbSender;
using ASC.Notify.Recipients;
using ASC.Security.Cryptography;
using ASC.Web.Core;
using ASC.Web.Core.Files;
using ASC.Web.Core.Helpers;
using ASC.Web.Core.Notify;
using ASC.Web.Core.Sms;
using ASC.Web.Core.Users;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Core.WhiteLabel;
using ASC.Web.Studio.Core;
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

            services.Configure<LogNLog>(r => r.Name = "ASC");
            services.Configure<LogNLog>("ASC", r => r.Name = "ASC");
            services.Configure<LogNLog>("ASC.Api", r => r.Name = "ASC.Api");

            services.Configure<DbManager>(r => { });
            services.Configure<DbManager>("default", r => { });
            services.Configure<DbManager>("messages", r => { r.CommandTimeout = 180000; });

            //= new ProgressQueue<T>(1, TimeSpan.FromMinutes(5), true)
            services.Configure<WorkerQueue<ResizeWorkerItem>>(r =>
            {
                r.workerCount = 2;
                r.waitInterval = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
                r.errorCount = 1;
                r.stopAfterFinsih = true;
            });

            services.Configure<ProgressQueue<ReassignProgressItem>>(r =>
            {
                r.workerCount = 1;
                r.waitInterval = (int)TimeSpan.FromMinutes(5).TotalMilliseconds;
                r.removeAfterCompleted = true;
                r.stopAfterFinsih = false;
                r.errorCount = 0;
            });

            services.Configure<ProgressQueue<RemoveProgressItem>>(r =>
            {
                r.workerCount = 1;
                r.waitInterval = (int)TimeSpan.FromMinutes(5).TotalMilliseconds;
                r.removeAfterCompleted = true;
                r.stopAfterFinsih = false;
                r.errorCount = 0;
            });

            services
                    .AddSubscriptionService()
                    .AddTenantManagerService()
                    .AddUserManagerService()
                    .AddSingleton(typeof(ILog), typeof(LogNLog))
                    .AddStorage(Configuration)
                    .AddWebItemManager()
                    .AddScoped<SecurityContext>()
                    .AddScoped<ApiContext>()
                    .AddScoped<StudioNotifyService>()
                    .AddScoped<UserManagerWrapper>()
                    .AddScoped<MessageService>()
                    .AddScoped<QueueWorkerReassign>()
                    .AddScoped<QueueWorkerRemove>()
                    .AddScoped<StudioNotifyHelper>()
                    .AddScoped<StudioNotifySource>()
                    .AddScoped<StudioNotifyServiceHelper>()
                    .AddScoped<AuthManager>()
                    .AddScoped<TenantExtra>()
                    .AddScoped<TenantStatisticsProvider>()
                    .AddScoped<AzManager>()
                    .AddScoped<WebItemSecurity>()
                    .AddScoped<UserPhotoManager>()
                    .AddScoped<CookiesManager>()
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
                    .AddScoped<DbOptionsManager>()

                    .AddSingleton<WorkerQueueOptionsManager<ResizeWorkerItem>>()
                    .AddSingleton<WorkerQueue<ResizeWorkerItem>>()
                    .AddSingleton<IConfigureOptions<WorkerQueue<ResizeWorkerItem>>, ConfigureWorkerQueue<ResizeWorkerItem>>()

                    .AddSingleton<ProgressQueueOptionsManager<ReassignProgressItem>>()
                    .AddSingleton<ProgressQueue<ReassignProgressItem>>()
                    .AddSingleton<IConfigureOptions<ProgressQueue<ReassignProgressItem>>, ConfigureProgressQueue<ReassignProgressItem>>()

                    .AddSingleton<ProgressQueueOptionsManager<RemoveProgressItem>>()
                    .AddSingleton<ProgressQueue<RemoveProgressItem>>()
                    .AddSingleton<IConfigureOptions<ProgressQueue<RemoveProgressItem>>, ConfigureProgressQueue<RemoveProgressItem>>()

                    .AddScoped<DbManager>()
                    .AddScoped<IConfigureOptions<DbManager>, ConfigureDbManager>()
                    .AddSingleton<StorageFactoryListener>()
                    .AddSingleton<StorageFactoryConfig>()
                    .AddScoped<StorageSettings>()
                    .AddScoped<IPRestrictionsSettings>()
                    .AddScoped<CustomNamingPeople>()
                    .AddScoped<PeopleNamesSettings>()
                    .AddScoped<EmailValidationKeyProvider>()
                    .AddScoped<TenantUtil>()
                    .AddScoped<PaymentManager>()
                    .AddScoped<BaseCommonLinkUtility>()
                    .AddScoped<CommonLinkUtility>()
                    .AddScoped<FilesLinkUtility>()
                    .AddScoped<FileUtility>()
                    .AddScoped<LicenseReader>()
                    .AddScoped<ApiSystemHelper>()
                    .AddSingleton<WebPathSettings>()
                    .AddSingleton<BaseStorageSettingsListener>()
                    .AddSingleton<SetupInfo>()
                    .AddScoped<FileSizeComment>()
                    .AddScoped<SubscriptionManager>()
                    .AddScoped<IPSecurity.IPSecurity>()
                    .AddSingleton<PathUtils>()
                    .AddSingleton<DbMessageSender>()
                    .AddSingleton<UrlShortener>()
                    .AddSingleton<MessagePolicy>()
                    .AddScoped<DisplayUserSettings>()
                    .AddScoped<SmsSender>()
                    .AddScoped<CookieStorage>()
                    .AddSingleton<UserFormatter>()
                    .AddSingleton<MachinePseudoKeys>()
                    .AddSingleton<Signature>()
                    .AddSingleton<InstanceCrypto>()
                    .AddSingleton<DbRegistry>()
                    .AddSingleton<MessagesRepository>()
                    .AddSingleton<IPRestrictionsServiceCache>()
                    .AddScoped<IPRestrictionsService>()
                    .AddScoped<IPRestrictionsRepository>()
                    .AddSingleton<DbSettingsManagerCache>()
                    .AddSingleton<AccountLinkerStorage>()
                    .AddSingleton<SmsKeyStorageCache>()
                    .AddSingleton<WebItemSecurityCache>()
                    .AddSingleton<UserPhotoManagerCache>()
                    .AddSingleton<AscCacheNotify>()
                    .AddSingleton<MessageTarget>()
                    .AddScoped(typeof(IRecipientProvider), typeof(RecipientProviderImpl))
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

            app.UseStaticFiles();
        }
    }
}
