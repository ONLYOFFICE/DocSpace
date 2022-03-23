// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implie d warranty
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

namespace ASC.Api.Core.Extensions;

public static class WebApp
{
    public static WebApplicationBuilder CreateWebApplicationBuilder(string[] args,
        Action<WebHostBuilderContext, KestrelServerOptions> configureKestrel = null,
        Action<HostBuilderContext, IConfigurationBuilder, IHostEnvironment, string> configureApp = null)
    {
        var options = new WebApplicationOptions
        {
            Args = args,
            ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
        };

        var builder = WebApplication.CreateBuilder(options);

        builder.Host.UseSystemd();
        builder.Host.UseWindowsService();
        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.Host.ConfigureNLogLogging();
        builder.Host.ConfigureDefaultAppConfiguration(args, configureApp);
        builder.WebHost.ConfigureDefaultKestrel(configureKestrel);

        return builder;
    }

    public static WebApplicationBuilder CreateWebApplicationBuilder(string[] args,
        Action<WebHostBuilderContext, KestrelServerOptions> configureKestrel = null,
        Action<HostBuilderContext, IConfigurationBuilder, IHostEnvironment, string> configureApp = null,
        Action<HostBuilderContext, IServiceCollection, DIHelper> configureServices = null)
    {
        var builder = CreateWebApplicationBuilder(args, configureKestrel, configureApp);

        builder.Host.ConfigureDefaultServices(configureServices);

        return builder;
    }
}

public static class WebApplicationExtensions
{
    public static WebApplication Build<T>(this WebApplicationBuilder builder) where T : Core.IStartup
    {
        var startup = CreateStartup<T>(builder);

        builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
        {
            startup.ConfigureContainer(containerBuilder);
        });

        return UseStartupAndBuild(builder, startup);
    }

    public static WebApplication Build<T>(this WebApplicationBuilder builder,
        Action<HostBuilderContext, ContainerBuilder> configureContainer) where T : Core.IStartup
    {
        var startup = CreateStartup<T>(builder);

        builder.Host.ConfigureContainer(configureContainer);

        return UseStartupAndBuild(builder, startup);
    }

    private static Core.IStartup CreateStartup<T>(WebApplicationBuilder builder) where T : Core.IStartup
    {
        var startup = (T)Activator.CreateInstance(typeof(T), new object[] { builder.Configuration, builder.Environment });

        return startup;
    }

    private static WebApplication UseStartupAndBuild(WebApplicationBuilder builder, Core.IStartup startup)
    {
        startup.ConfigureServices(builder.Services);

        var app = builder.Build();

        startup.Configure(app, app.Environment);

        return app;
    }
}