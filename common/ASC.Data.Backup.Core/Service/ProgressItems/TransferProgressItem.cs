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
    private TenantManager _tenantManager;
    private readonly ILogger<TransferProgressItem> _logger;
    private readonly NotifyHelper _notifyHelper;
    private TransferPortalTask _transferPortalTask;

    public TransferProgressItem(
        ILogger<TransferProgressItem> logger,
        IServiceScopeFactory serviceScopeFactory,
        NotifyHelper notifyHelper) :
        base(logger, serviceScopeFactory)
    {
        _logger = logger;
        _notifyHelper = notifyHelper;
        BackupProgressItemEnum = BackupProgressItemEnum.Transfer;
    }

    public string TargetRegion { get; set; }
    public bool TransferMail { get; set; }
    public bool Notify { get; set; }
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
            using var scope = _serviceScopeProvider.CreateScope();
            _tenantManager = scope.ServiceProvider.GetService<TenantManager>();
            _transferPortalTask = scope.ServiceProvider.GetService<TransferPortalTask>();


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
            _logger.ErrorTransferProgressItem(error);
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
                _logger.ErrorPublish(error);
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
