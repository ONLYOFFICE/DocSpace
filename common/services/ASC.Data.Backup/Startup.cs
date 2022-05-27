/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


namespace ASC.Data.Backup;

public class Startup : BaseStartup
{
    public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        : base(configuration, hostEnvironment)
    {

    }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        DIHelper.AddDistributedTaskQueueService<BaseBackupProgressItem>(1);

        DIHelper.TryAdd<BackupProgressItem>();
        DIHelper.TryAdd<RestoreProgressItem>();
        DIHelper.TryAdd<TransferProgressItem>();

        DIHelper.TryAdd<Schedule>();

        DIHelper.TryAdd<BackupController>();

        DIHelper.TryAdd<BackupCleanerService>();
        DIHelper.TryAdd<BackupSchedulerService>();
        DIHelper.TryAdd<BackupListenerService>();
        DIHelper.TryAdd<BackupWorkerService>();

        NotifyConfigurationExtension.Register(DIHelper);

        services.AddHostedService<BackupCleanerService>();
        services.AddHostedService<BackupSchedulerService>();
        services.AddHostedService<BackupListenerService>();
        services.AddHostedService<BackupWorkerService>();
    }

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        base.Configure(app, env);

        app.MapWhen(
            context => context.Request.Path.ToString().EndsWith("backupFileUpload.ashx"),
            appBranch =>
            {
                appBranch.UseBackupFileUploadHandler();
            });
    }
}
