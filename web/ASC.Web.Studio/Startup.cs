// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Web.Studio;
public class Startup : BaseStartup
{
    public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment) : base(configuration, hostEnvironment)
    {
    }

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        base.Configure(app, env);

        app.UseRouting();

        app.UseAuthentication();

        app.UseEndpoints(endpoints =>
        {
            endpoints.InitializeHttpHandlers();
        });

        app.MapWhen(
              context => context.Request.Path.ToString().EndsWith("ssologin.ashx"),
              appBranch =>
              {
                  appBranch.UseSsoHandler();
              });

        app.MapWhen(
            context => context.Request.Path.ToString().EndsWith("login.ashx"),
            appBranch =>
            {
                appBranch.UseLoginHandler();
            });
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        services.AddMemoryCache();
        DIHelper.TryAdd<Login>();
        DIHelper.TryAdd<PathUtils>();
        DIHelper.TryAdd<StorageFactory>();
        DIHelper.TryAdd<GoogleLoginProvider>();
        DIHelper.TryAdd<FacebookLoginProvider>();
        DIHelper.TryAdd<LinkedInLoginProvider>();
        DIHelper.TryAdd<SsoHandlerService>();


        services.AddHttpClient();

        DIHelper.TryAdd<DbWorker>();

        services.AddHostedService<WorkerService>();
        DIHelper.TryAdd<WorkerService>();

        services.AddHttpClient("webhook")
        .SetHandlerLifetime(TimeSpan.FromMinutes(5))
        .AddPolicyHandler((s, request) =>
        {
            var settings = s.GetRequiredService<Settings>();

            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<SslException>()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(settings.RepeatCount.HasValue ? settings.RepeatCount.Value : 5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        })
        .ConfigurePrimaryHttpMessageHandler((s) =>
        {
            return new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
                {
                    var helper = s.GetRequiredService<SslHelper>();
                    return helper.ValidateCertificate(sslPolicyErrors);
                }
            };
        });
    }
}
