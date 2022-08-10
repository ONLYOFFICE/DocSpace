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

using ASC.Api.Core.Extensions;
using ASC.Common;
using ASC.Common.Mapping;
using ASC.Core.Data;
using ASC.Data.Backup.Tasks;
using ASC.Data.Backup.Tasks.Modules;
using ASC.Data.Storage;
using ASC.Data.Storage.DiscStorage;
using ASC.Data.Storage.S3;
using ASC.Migration.PersonalToDocspace.Creator;

Thread.Sleep(4000);
var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
};

var builder = WebApplication.CreateBuilder(options);

builder.Host.ConfigureDefault(args);

builder.WebHost.ConfigureServices((hostContext, services) =>
{
    var diHelper = new DIHelper();
    services.AddScoped<EFLoggerFactory>();
    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    services.AddHttpClient();
    services.AddBaseDbContextPool<AccountLinkContext>();
    services.AddBaseDbContextPool<BackupsContext>();
    services.AddBaseDbContextPool<FilesDbContext>();
    services.AddBaseDbContextPool<CoreDbContext>();
    services.AddBaseDbContextPool<TenantDbContext>();
    services.AddBaseDbContextPool<UserDbContext>();
    services.AddBaseDbContextPool<TelegramDbContext>();
    services.AddBaseDbContextPool<CustomDbContext>();
    services.AddBaseDbContextPool<WebstudioDbContext>();
    services.AddBaseDbContextPool<InstanceRegistrationContext>();
    services.AddBaseDbContextPool<IntegrationEventLogContext>();
    services.AddBaseDbContextPool<FeedDbContext>();
    services.AddBaseDbContextPool<MessagesContext>();
    services.AddBaseDbContextPool<WebhooksDbContext>();
    services.AddAutoMapper(typeof(MappingProfile));
    diHelper.Configure(services);

    diHelper.TryAdd<TempStream>();
    diHelper.TryAdd<DbFactory>();
    diHelper.TryAdd<ModuleProvider>();
    diHelper.TryAdd<DbTenantService>();
    diHelper.TryAdd<StorageFactoryConfig>();
    diHelper.TryAdd<StorageFactory>();
    diHelper.TryAdd<DiscDataStore>();
    diHelper.TryAdd<S3Storage>();
});

var app = builder.Build();
var config = builder.Configuration;

var migrationCreator = new MigrationCreator(app.Services, string.IsNullOrEmpty(config["pathToSave"]) ? "" : config["pathToSave"], Int32.Parse(config["tenant"]));
migrationCreator.Create();