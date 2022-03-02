/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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

namespace ASC.Data.Backup.Services;

[Singletone(Additional = typeof(FactoryProgressItemExtension))]
public class FactoryProgressItem
{
    public IServiceProvider ServiceProvider { get; }

    public FactoryProgressItem(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public BackupProgressItem CreateBackupProgressItem(
        StartBackupRequest request,
        bool isScheduled,
        string tempFolder,
        int limit,
        string currentRegion,
        Dictionary<string, string> configPaths)
    {
        var item = ServiceProvider.GetService<BackupProgressItem>();
        item.Init(request, isScheduled, tempFolder, limit, currentRegion, configPaths);

        return item;
    }

    public BackupProgressItem CreateBackupProgressItem(
        BackupSchedule schedule,
        bool isScheduled,
        string tempFolder,
        int limit,
        string currentRegion,
        Dictionary<string, string> configPaths
        )
    {
        var item = ServiceProvider.GetService<BackupProgressItem>();
        item.Init(schedule, isScheduled, tempFolder, limit, currentRegion, configPaths);

        return item;
    }

    public RestoreProgressItem CreateRestoreProgressItem(
        StartRestoreRequest request,
        string tempFolder,
        string upgradesPath,
        string currentRegion,
        Dictionary<string, string> configPaths
        )
    {
        var item = ServiceProvider.GetService<RestoreProgressItem>();
        item.Init(request, tempFolder, upgradesPath, currentRegion, configPaths);

        return item;
    }

    public TransferProgressItem CreateTransferProgressItem(
        string targetRegion,
        bool transferMail,
        int tenantId,
        string tempFolder,
        int limit,
        bool notify,
        string currentRegion,
        Dictionary<string, string> configPaths
        )
    {
        var item = ServiceProvider.GetService<TransferProgressItem>();
        item.Init(targetRegion, transferMail, tenantId, tempFolder, limit, notify, currentRegion, configPaths);

        return item;
    }
}
public static class FactoryProgressItemExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<BackupProgressItem>();
        services.TryAdd<RestoreProgressItem>();
        services.TryAdd<TransferProgressItem>();
    }
}
