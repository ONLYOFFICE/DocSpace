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

using CommandLine;

var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
};

var param = Parser.Default.ParseArguments<Options>(args).Value;
/*var param = new Options()
{
    FromRegion = "personal",
    ToRegion = "personal",
    FromAlias = "localhost"
};*/

var builder = WebApplication.CreateBuilder(options);

builder.Configuration.AddJsonFile($"appsettings.personalToDocspace.json", true)
                     .AddCommandLine(args);

var config = builder.Configuration;

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

RegionSettings.SetCurrent(param.FromRegion);

builder.Services.RegisterFeature()
    .AddScoped<EFLoggerFactory>()
    .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
    .AddHttpClient()
    .AddBaseDbContextPool<AccountLinkContext>()
    .AddBaseDbContextPool<BackupsContext>()
    .AddBaseDbContextPool<FilesDbContext>()
    .AddBaseDbContextPool<CoreDbContext>()
    .AddBaseDbContextPool<TenantDbContext>()
    .AddBaseDbContextPool<UserDbContext>()
    .AddBaseDbContextPool<TelegramDbContext>()
    .AddBaseDbContextPool<CustomDbContext>()
    .AddBaseDbContextPool<WebstudioDbContext>()
    .AddBaseDbContextPool<InstanceRegistrationContext>()
    .AddBaseDbContextPool<IntegrationEventLogContext>()
    .AddBaseDbContextPool<FeedDbContext>()
    .AddBaseDbContextPool<MessagesContext>()
    .AddBaseDbContextPool<WebhooksDbContext>()
    .AddAutoMapper(BaseStartup.GetAutoMapperProfileAssemblies())
    .AddMemoryCache()
    .AddSingleton<IEventBus, MockEventBusRabbitMQ>()
    .AddCacheNotify(config);


var diHelper = new DIHelper();
diHelper.Configure(builder.Services);

diHelper.TryAdd<MigrationCreator>();
diHelper.TryAdd<MigrationRunner>();



if (string.IsNullOrEmpty(param.UserName) && string.IsNullOrEmpty(param.Mail))
{
    throw new Exception("username or email must be entered");
}

var app = builder.Build();
Console.WriteLine("backup start");
var migrationCreator = app.Services.GetService<MigrationCreator>();
var fileName = await migrationCreator.Create(param.FromAlias, param.UserName, param.Mail, param.ToRegion, param.ToAlias);
Console.WriteLine("backup was success");

Console.WriteLine("restore start");
var migrationRunner = app.Services.GetService<MigrationRunner>();
await migrationRunner.Run(fileName, param.ToRegion, param.FromAlias, param.ToAlias);
Console.WriteLine("restore was success");

Directory.GetFiles(AppContext.BaseDirectory).Where(f => f.Equals(fileName)).ToList().ForEach(File.Delete);

if (Directory.Exists(AppContext.BaseDirectory + "\\temp"))
{
    Directory.Delete(AppContext.BaseDirectory + "\\temp");
}

Console.WriteLine("migration was success");
if (!string.IsNullOrEmpty(migrationCreator.NewAlias))
{
    Console.WriteLine($"new alias is - {migrationCreator.NewAlias}");
}
public sealed class Options
{
    [Option('a', "fromAlias", Required = true)]
    public string FromAlias { get; set; }

    [Option('u', "username", Required = false, HelpText = "enter username or mail for find user")]
    public string UserName { get; set; }

    [Option('m', "mail", Required = false, HelpText = "enter username or mail for find user")]
    public string Mail { get; set; }

    [Option('t', "toRegion", Required = true)]
    public string ToRegion { get; set; }

    [Option('f', "fromRegion", Required = false, Default = "personal")]
    public string FromRegion { get; set; }

    [Option("toAlias", Required = false, HelpText = "if you wish migration to already exist portal, enter the alias")]
    public string ToAlias { get; set; }
}