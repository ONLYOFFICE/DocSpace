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

namespace ASC.Data.Reassigns;

/// <summary>
/// </summary>
[Transient]
public class RemoveProgressItem : DistributedTaskProgress
{
    /// <summary>ID of the user whose data is deleted</summary>
    /// <type>System.Guid, System</type>
    public Guid FromUser { get; private set; }

    /// <summary>The user whose data is deleted</summary>
    /// <type>ASC.Core.Users.UserInfo, ASC.Core.Common</type>
    public UserInfo User { get; private set; }

    private readonly IDictionary<string, StringValues> _httpHeaders;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly int _tenantId;
    private readonly Guid _currentUserId;
    private readonly bool _notify;
    //private readonly IFileStorageService _docService;
    //private readonly MailGarbageEngine _mailEraser;

    public RemoveProgressItem(
        IServiceScopeFactory serviceScopeFactory,
            IDictionary<string, StringValues> httpHeaders,
            int tenantId, UserInfo user, Guid currentUserId, bool notify)
    {
        _httpHeaders = httpHeaders;
        _serviceScopeFactory = serviceScopeFactory;


        //_docService = Web.Files.Classes.Global.FileStorageService;
        //_mailEraser = new MailGarbageEngine();

        _tenantId = tenantId;
        User = user;
        FromUser = user.Id;
        _currentUserId = currentUserId;
        _notify = notify;

        Id = QueueWorkerRemove.GetProgressItemId(tenantId, FromUser);
        Status = DistributedTaskStatus.Created;
        Exception = null;
        Percentage = 0;
        IsCompleted = false;
    }

    protected override async Task DoJob()
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var scopeClass = scope.ServiceProvider.GetService<RemoveProgressItemScope>();
        var (tenantManager, coreBaseSettings, messageService, studioNotifyService, securityContext, userManager, messageTarget, webItemManagerSecurity, storageFactory, userFormatter, options) = scopeClass;
        var logger = options.CreateLogger("ASC.Web");
        await tenantManager.SetCurrentTenantAsync(_tenantId);
        var userName = userFormatter.GetUserName(User, DisplayUserNameFormat.Default);

        try
        {
            Percentage = 0;
            Status = DistributedTaskStatus.Running;

            await securityContext.AuthenticateMeWithoutCookieAsync(_currentUserId);

            long crmSpace;
            var wrapper = await GetUsageSpace(webItemManagerSecurity);

            logger.LogInformation("deleting user data for {fromUser} ", FromUser);

            logger.LogInformation("deleting of data from documents");

            //_docService.DeleteStorage(_userId);
            Percentage = 25;
            PublishChanges();

            if (!coreBaseSettings.CustomMode)
            {
                logger.LogInformation("deleting of data from crm");


                //using (var scope = DIHelper.Resolve(_tenantId))
                //{
                //    var crmDaoFactory = scope.Resolve<CrmDaoFactory>();
                crmSpace = 0;// crmDaoFactory.ReportDao.GetFiles(_userId).Sum(file => file.ContentLength);
                             //    crmDaoFactory.ReportDao.DeleteFiles(_userId);
                             //}
                Percentage = 50;
            }
            else
            {
                crmSpace = 0;
            }

            PublishChanges();

            logger.LogInformation("deleting of data from mail");

            //_mailEraser.ClearUserMail(_userId);
            Percentage = 75;
            PublishChanges();

            logger.LogInformation("deleting of data from talk");
            await DeleteTalkStorage(storageFactory);
            Percentage = 99;
            PublishChanges();

            await SendSuccessNotifyAsync(studioNotifyService, messageService, messageTarget, userName, wrapper.DocsSpace, crmSpace, wrapper.MailSpace, wrapper.TalkSpace);

            Percentage = 100;
            Status = DistributedTaskStatus.Completed;
        }
        catch (Exception ex)
        {
            logger.ErrorRemoveProgressItem(ex);
            Status = DistributedTaskStatus.Failted;
            Exception = ex;
            await SendErrorNotifyAsync(studioNotifyService, ex.Message, userName);
        }
        finally
        {
            logger.LogInformation("data deletion is complete");
            IsCompleted = true;
            PublishChanges();
        }
    }

    public object Clone()
    {
        return MemberwiseClone();
    }

    private async Task<UsageSpaceWrapper> GetUsageSpace(WebItemManagerSecurity webItemManagerSecurity)
    {
        var usageSpaceWrapper = new UsageSpaceWrapper();

        var webItems = webItemManagerSecurity.GetItems(Web.Core.WebZones.WebZoneType.All, ItemAvailableState.All);

        foreach (var item in webItems)
        {
            IUserSpaceUsage manager;

            if (item.ID == WebItemManager.DocumentsProductID)
            {
                manager = item.Context.SpaceUsageStatManager as IUserSpaceUsage;
                if (manager == null)
                {
                    continue;
                }

                usageSpaceWrapper.DocsSpace = await manager.GetUserSpaceUsageAsync(FromUser);
            }

            if (item.ID == WebItemManager.MailProductID)
            {
                manager = item.Context.SpaceUsageStatManager as IUserSpaceUsage;
                if (manager == null)
                {
                    continue;
                }

                usageSpaceWrapper.MailSpace = await manager.GetUserSpaceUsageAsync(FromUser);
            }

            if (item.ID == WebItemManager.TalkProductID)
            {
                manager = item.Context.SpaceUsageStatManager as IUserSpaceUsage;
                if (manager == null)
                {
                    continue;
                }

                usageSpaceWrapper.TalkSpace = await manager.GetUserSpaceUsageAsync(FromUser);
            }
        }
        return usageSpaceWrapper;
    }

    private async Task DeleteTalkStorage(StorageFactory storageFactory)
    {
        using var md5 = MD5.Create();
        var data = md5.ComputeHash(Encoding.Default.GetBytes(FromUser.ToString()));

        var sBuilder = new StringBuilder();

        for (int i = 0, n = data.Length; i < n; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        var md5Hash = sBuilder.ToString();

        var storage = await storageFactory.GetStorageAsync(_tenantId, "talk");

        if (storage != null && await storage.IsDirectoryAsync(md5Hash))
        {
            await storage.DeleteDirectoryAsync(md5Hash);
        }
    }

    private async Task SendSuccessNotifyAsync(StudioNotifyService studioNotifyService, MessageService messageService, MessageTarget messageTarget, string userName, long docsSpace, long crmSpace, long mailSpace, long talkSpace)
    {
        if (_notify)
        {
            await studioNotifyService.SendMsgRemoveUserDataCompletedAsync(_currentUserId, User, userName,
                                                                        docsSpace, crmSpace, mailSpace, talkSpace);
        }

        if (_httpHeaders != null)
        {
            await messageService.SendAsync(_httpHeaders, MessageAction.UserDataRemoving, messageTarget.Create(FromUser), new[] { userName });
        }
        else
        {
            await messageService.SendAsync(MessageAction.UserDataRemoving, messageTarget.Create(FromUser), userName);
        }
    }

    private async Task SendErrorNotifyAsync(StudioNotifyService studioNotifyService, string errorMessage, string userName)
    {
        if (!_notify)
        {
            return;
        }

        await studioNotifyService.SendMsgRemoveUserDataFailedAsync(_currentUserId, User, userName, errorMessage);
    }
}

[Scope]
public class RemoveProgressItemScope
{
    private readonly TenantManager _tenantManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly MessageService _messageService;
    private readonly StudioNotifyService _studioNotifyService;
    private readonly SecurityContext _securityContext;
    private readonly UserManager _userManager;
    private readonly MessageTarget _messageTarget;
    private readonly WebItemManagerSecurity _webItemManagerSecurity;
    private readonly StorageFactory _storageFactory;
    private readonly UserFormatter _userFormatter;
    private readonly ILoggerProvider _options;

    public RemoveProgressItemScope(TenantManager tenantManager,
        CoreBaseSettings coreBaseSettings,
        MessageService messageService,
        StudioNotifyService studioNotifyService,
        SecurityContext securityContext,
        UserManager userManager,
        MessageTarget messageTarget,
        WebItemManagerSecurity webItemManagerSecurity,
        StorageFactory storageFactory,
        UserFormatter userFormatter,
        ILoggerProvider options)
    {
        _tenantManager = tenantManager;
        _coreBaseSettings = coreBaseSettings;
        _messageService = messageService;
        _studioNotifyService = studioNotifyService;
        _securityContext = securityContext;
        _userManager = userManager;
        _messageTarget = messageTarget;
        _webItemManagerSecurity = webItemManagerSecurity;
        _storageFactory = storageFactory;
        _userFormatter = userFormatter;
        _options = options;
    }

    public void Deconstruct(out TenantManager tenantManager,
        out CoreBaseSettings coreBaseSettings,
        out MessageService messageService,
        out StudioNotifyService studioNotifyService,
        out SecurityContext securityContext,
        out UserManager userManager,
        out MessageTarget messageTarget,
        out WebItemManagerSecurity webItemManagerSecurity,
        out StorageFactory storageFactory,
        out UserFormatter userFormatter,
        out ILoggerProvider optionsMonitor)
    {
        tenantManager = _tenantManager;
        coreBaseSettings = _coreBaseSettings;
        messageService = _messageService;
        studioNotifyService = _studioNotifyService;
        securityContext = _securityContext;
        userManager = _userManager;
        messageTarget = _messageTarget;
        webItemManagerSecurity = _webItemManagerSecurity;
        storageFactory = _storageFactory;
        userFormatter = _userFormatter;
        optionsMonitor = _options;
    }
}

class UsageSpaceWrapper
{
    public long DocsSpace { get; set; }
    public long MailSpace { get; set; }
    public long TalkSpace { get; set; }
}

public static class RemoveProgressItemExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<RemoveProgressItemScope>();
    }
}
