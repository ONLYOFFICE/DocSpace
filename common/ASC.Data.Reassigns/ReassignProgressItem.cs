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
public class ReassignProgressItem : DistributedTaskProgress
{
    /// <summary>The user whose data is reassigned</summary>
    /// <type>System.Guid, System</type>
    public Guid FromUser { get; private set; }

    /// <summary>The user to whom this data is reassigned</summary>
    /// <type>System.Guid, System</type>
    public Guid ToUser { get; private set; }

    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IDictionary<string, StringValues> _httpHeaders;
    private readonly int _tenantId;
    private readonly Guid _currentUserId;
    private readonly bool _deleteProfile;

    //private readonly IFileStorageService _docService;
    //private readonly ProjectsReassign _projectsReassign;

    public ReassignProgressItem(
        IServiceScopeFactory serviceScopeFactory,
            IDictionary<string, StringValues> httpHeaders,
            int tenantId, Guid fromUserId, Guid toUserId, Guid currentUserId, bool deleteProfile)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _httpHeaders = httpHeaders;

        //_docService = Web.Files.Classes.Global.FileStorageService;
        //_projectsReassign = new ProjectsReassign();

        _tenantId = tenantId;
        FromUser = fromUserId;
        ToUser = toUserId;
        _currentUserId = currentUserId;
        _deleteProfile = deleteProfile;

        //_docService = Web.Files.Classes.Global.FileStorageService;
        //_projectsReassign = new ProjectsReassign();

        Id = QueueWorkerReassign.GetProgressItemId(tenantId, fromUserId);
        Status = DistributedTaskStatus.Created;
        Exception = null;
        Percentage = 0;
        IsCompleted = false;
    }

    protected override async Task DoJob()
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var scopeClass = scope.ServiceProvider.GetService<ReassignProgressItemScope>();
        var queueWorkerRemove = scope.ServiceProvider.GetService<QueueWorkerRemove>();
        var (tenantManager, coreBaseSettings, messageService, fileStorageService, studioNotifyService, securityContext, userManager, userPhotoManager, displayUserSettingsHelper, messageTarget, options) = scopeClass;
        var logger = options.CreateLogger("ASC.Web");
        await tenantManager.SetCurrentTenantAsync(_tenantId);

        try
        {
            Percentage = 0;
            Status = DistributedTaskStatus.Running;

            await securityContext.AuthenticateMeWithoutCookieAsync(_currentUserId);

            Percentage = 33;
            PublishChanges();

            await fileStorageService.ReassignStorageAsync<int>(FromUser, ToUser);

            Percentage = 66;
            PublishChanges();

            await SendSuccessNotifyAsync(userManager, studioNotifyService, messageService, messageTarget, displayUserSettingsHelper);

            Percentage = 99;
            PublishChanges();

            if (_deleteProfile)
            {
                await DeleteUserProfile(userManager, userPhotoManager, messageService, messageTarget, displayUserSettingsHelper, queueWorkerRemove);
            }

            Percentage = 100;
            Status = DistributedTaskStatus.Completed;
        }
        catch (Exception ex)
        {
            logger.ErrorReassignProgressItem(ex);
            Status = DistributedTaskStatus.Failted;
            Exception = ex;
            await SendErrorNotifyAsync(userManager, studioNotifyService, ex.Message);
        }
        finally
        {
            logger.LogInformation("data reassignment is complete");
            IsCompleted = true;
        }

        PublishChanges();
    }

    public object Clone()
    {
        return MemberwiseClone();
    }

    private async Task SendSuccessNotifyAsync(UserManager userManager, StudioNotifyService studioNotifyService, MessageService messageService, MessageTarget messageTarget, DisplayUserSettingsHelper displayUserSettingsHelper)
    {
        var fromUser = await userManager.GetUsersAsync(FromUser);
        var toUser = await userManager.GetUsersAsync(ToUser);

        await studioNotifyService.SendMsgReassignsCompletedAsync(_currentUserId, fromUser, toUser);

        var fromUserName = fromUser.DisplayUserName(false, displayUserSettingsHelper);
        var toUserName = toUser.DisplayUserName(false, displayUserSettingsHelper);

        if (_httpHeaders != null)
        {
            await messageService.SendAsync(_httpHeaders, MessageAction.UserDataReassigns, messageTarget.Create(FromUser), new[] { fromUserName, toUserName });
        }
        else
        {
           await messageService.SendAsync(MessageAction.UserDataReassigns, messageTarget.Create(FromUser), fromUserName, toUserName);
        }
    }

    private async Task SendErrorNotifyAsync(UserManager userManager, StudioNotifyService studioNotifyService, string errorMessage)
    {
        var fromUser = await userManager.GetUsersAsync(FromUser);
        var toUser = await userManager.GetUsersAsync(ToUser);

        await studioNotifyService.SendMsgReassignsFailedAsync(_currentUserId, fromUser, toUser, errorMessage);
    }

    private async Task DeleteUserProfile(UserManager userManager, UserPhotoManager userPhotoManager, MessageService messageService, MessageTarget messageTarget, DisplayUserSettingsHelper displayUserSettingsHelper, QueueWorkerRemove queueWorkerRemove)
    {
        var user = await userManager.GetUsersAsync(FromUser);
        var userName = user.DisplayUserName(false, displayUserSettingsHelper);

        await userPhotoManager.RemovePhotoAsync(user.Id);
        await userManager.DeleteUserAsync(user.Id);
        queueWorkerRemove.Start(_tenantId, user, _currentUserId, false);

        if (_httpHeaders != null)
        {
            await messageService.SendAsync(_httpHeaders, MessageAction.UserDeleted, messageTarget.Create(FromUser), new[] { userName });
        }
        else
        {
            await messageService.SendAsync(MessageAction.UserDeleted, messageTarget.Create(FromUser), userName);
        }
    }
}

[Scope]
public class ReassignProgressItemScope
{
    private readonly TenantManager _tenantManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly MessageService _messageService;
    private readonly FileStorageService _fileStorageService;
    private readonly StudioNotifyService _studioNotifyService;
    private readonly SecurityContext _securityContext;
    private readonly UserManager _userManager;
    private readonly UserPhotoManager _userPhotoManager;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly MessageTarget _messageTarget;
    private readonly ILoggerProvider _options;

    public ReassignProgressItemScope(TenantManager tenantManager,
        CoreBaseSettings coreBaseSettings,
        MessageService messageService,
        FileStorageService fileStorageService,
        StudioNotifyService studioNotifyService,
        SecurityContext securityContext,
        UserManager userManager,
        UserPhotoManager userPhotoManager,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        MessageTarget messageTarget,
        ILoggerProvider options)
    {
        _tenantManager = tenantManager;
        _coreBaseSettings = coreBaseSettings;
        _messageService = messageService;
        _fileStorageService = fileStorageService;
        _studioNotifyService = studioNotifyService;
        _securityContext = securityContext;
        _userManager = userManager;
        _userPhotoManager = userPhotoManager;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _messageTarget = messageTarget;
        _options = options;
    }

    public void Deconstruct(out TenantManager tenantManager,
        out CoreBaseSettings coreBaseSettings,
        out MessageService messageService,
        out FileStorageService fileStorageService,
        out StudioNotifyService studioNotifyService,
        out SecurityContext securityContext,
        out UserManager userManager,
        out UserPhotoManager userPhotoManager,
        out DisplayUserSettingsHelper displayUserSettingsHelper,
        out MessageTarget messageTarget,
        out ILoggerProvider optionsMonitor)
    {
        tenantManager = _tenantManager;
        coreBaseSettings = _coreBaseSettings;
        messageService = _messageService;
        fileStorageService = _fileStorageService;
        studioNotifyService = _studioNotifyService;
        securityContext = _securityContext;
        userManager = _userManager;
        userPhotoManager = _userPhotoManager;
        displayUserSettingsHelper = _displayUserSettingsHelper;
        messageTarget = _messageTarget;
        optionsMonitor = _options;
    }
}

public static class ReassignProgressItemExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<ReassignProgressItemScope>();
    }
}
