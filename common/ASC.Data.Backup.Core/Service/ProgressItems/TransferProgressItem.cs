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


using ConfigurationProvider = ASC.Data.Backup.Utils.ConfigurationProvider;

namespace ASC.Data.Backup.Services;

[Transient]
public class TransferProgressItem : BaseBackupProgressItem
{
    private readonly TenantManager _tenantManager;
    private readonly NotifyHelper _notifyHelper;
    private readonly TransferPortalTask _transferPortalTask;

    public TransferProgressItem(
        IOptionsMonitor<ILog> options,
        TenantManager tenantManager,
        NotifyHelper notifyHelper,
        TransferPortalTask transferPortalTask) : 
        base(options)
    {
        _tenantManager = tenantManager;
        _notifyHelper = notifyHelper;
        _transferPortalTask = transferPortalTask;
    }

    public override BackupProgressItemEnum BackupProgressItemEnum { get => BackupProgressItemEnum.Transfer; }
    public string TargetRegion { get; set; }
    public bool TransferMail { get; set; }
    public bool Notify { get; set; }
    public string Link { get; set; }
    public string TempFolder { get; set; }
    public Dictionary<string, string> ConfigPaths { get; set; }
    public string CurrentRegion { get; set; }
    public int Limit { get; set; }


    public void Init(
        string targetRegion,
        bool transferMail,
        int tenantId,
        string tempFolder,
        int limit,
        bool notify,
        string currentRegion,
        Dictionary<string, string> configPaths)
    {
        TenantId = tenantId;
        TargetRegion = targetRegion;
        TransferMail = transferMail;
        Notify = notify;
        TempFolder = tempFolder;
        ConfigPaths = configPaths;
        CurrentRegion = currentRegion;
        Limit = limit;

    }

    protected override void DoJob()
    {
        var tempFile = PathHelper.GetTempFileName(TempFolder);
        var tenant = _tenantManager.GetTenant(TenantId);
        var alias = tenant.Alias;

        try
        {
            _notifyHelper.SendAboutTransferStart(tenant, TargetRegion, Notify);
            var transferProgressItem = _transferPortalTask;
            transferProgressItem.Init(TenantId, ConfigPaths[CurrentRegion], ConfigPaths[TargetRegion], Limit, TempFolder);
            transferProgressItem.ProgressChanged += (sender, args) =>
            {
                Percentage = args.Progress;
                PublishChanges();
            };
            if (!TransferMail)
            {
                transferProgressItem.IgnoreModule(ModuleName.Mail);
            }
            transferProgressItem.RunJob();

            Link = GetLink(alias, false);
            _notifyHelper.SendAboutTransferComplete(tenant, TargetRegion, Link, !Notify, transferProgressItem.ToTenantId);

            PublishChanges();
        }
        catch (Exception error)
        {
            Logger.Error(error);
            Exception = error;

            Link = GetLink(alias, true);
            _notifyHelper.SendAboutTransferError(tenant, TargetRegion, Link, !Notify);
        }
        finally
        {
            try
            {
                PublishChanges();
            }
            catch (Exception error)
            {
                Logger.Error("publish", error);
            }

            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    private string GetLink(string alias, bool isErrorLink)
    {
        return "https://" + alias + "." + ConfigurationProvider.Open(ConfigPaths[isErrorLink ? CurrentRegion : TargetRegion]).AppSettings.Settings["core:base-domain"].Value;
    }

    public override object Clone()
    {
        return MemberwiseClone();
    }
}
