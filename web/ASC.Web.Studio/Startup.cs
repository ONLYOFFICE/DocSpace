
using ASC.Api.Core;
using ASC.Data.Storage;
using ASC.Data.Storage.DiscStorage;
using ASC.FederatedLogin;
using ASC.FederatedLogin.LoginProviders;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;

namespace ASC.Web.Studio
{
    public class Startup : BaseStartup
    {
        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment) : base(configuration, hostEnvironment)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            base.ConfigureServices(services);

            services.AddMemoryCache();
            DIHelper.TryAdd<Login>();
            DIHelper.TryAdd<PathUtils>();
            DIHelper.TryAdd<StorageHandlerScope>();
            DIHelper.TryAdd<GoogleLoginProvider>();
            DIHelper.TryAdd<FacebookLoginProvider>();
            DIHelper.TryAdd<LinkedInLoginProvider>();

            services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(Configuration.GetSection("Redis").Get<RedisConfiguration>());
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseRouting();

            app.UseCors(builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.InitializeHttpHandlers();
            });

            app.MapWhen(
               context => context.Request.Path.ToString().EndsWith("login.ashx"),
               appBranch =>
               {
                   appBranch.UseLoginHandler();
               });
        }
    }
}
