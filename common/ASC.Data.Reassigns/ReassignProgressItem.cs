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

[Transient]
public class ReassignProgressItem : DistributedTaskProgress
{
    public Guid FromUser { get; private set; }
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
        var (tenantManager, coreBaseSettings, messageService, studioNotifyService, securityContext, userManager, userPhotoManager, displayUserSettingsHelper, messageTarget, options) = scopeClass;
        var logger = options.CreateLogger("ASC.Web");
        tenantManager.SetCurrentTenant(_tenantId);

        try
        {
            Percentage = 0;
            Status = DistributedTaskStatus.Running;

            securityContext.AuthenticateMeWithoutCookie(_currentUserId);

            logger.LogInformation("reassignment of data from {fromUser} to {toUser}", FromUser, ToUser);

            logger.LogInformation("reassignment of data from documents");


            //_docService.ReassignStorage(_fromUserId, _toUserId);
            Percentage = 33;
            PublishChanges();

            logger.LogInformation("reassignment of data from projects");

            //_projectsReassign.Reassign(_fromUserId, _toUserId);
            Percentage = 66;
            PublishChanges();

            if (!coreBaseSettings.CustomMode)
            {
                logger.LogInformation("reassignment of data from crm");

                //using (var scope = DIHelper.Resolve(_tenantId))
                //{
                //    var crmDaoFactory = scope.Resolve<CrmDaoFactory>();
                //    crmDaoFactory.ContactDao.ReassignContactsResponsible(_fromUserId, _toUserId);
                //    crmDaoFactory.DealDao.ReassignDealsResponsible(_fromUserId, _toUserId);
                //    crmDaoFactory.TaskDao.ReassignTasksResponsible(_fromUserId, _toUserId);
                //    crmDaoFactory.CasesDao.ReassignCasesResponsible(_fromUserId, _toUserId);
                //}
                Percentage = 99;
                PublishChanges();
            }

            SendSuccessNotify(userManager, studioNotifyService, messageService, messageTarget, displayUserSettingsHelper);

            Percentage = 100;
            Status = DistributedTaskStatus.Completed;

            if (_deleteProfile)
            {
                await DeleteUserProfile(userManager, userPhotoManager, messageService, messageTarget, displayUserSettingsHelper, queueWorkerRemove);
            }
        }
        catch (Exception ex)
        {
            logger.ErrorReassignProgressItem(ex);
            Status = DistributedTaskStatus.Failted;
            Exception = ex;
            SendErrorNotify(userManager, studioNotifyService, ex.Message);
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

    private void SendSuccessNotify(UserManager userManager, StudioNotifyService studioNotifyService, MessageService messageService, MessageTarget messageTarget, DisplayUserSettingsHelper displayUserSettingsHelper)
    {
        var fromUser = userManager.GetUsers(FromUser);
        var toUser = userManager.GetUsers(ToUser);

        studioNotifyService.SendMsgReassignsCompleted(_currentUserId, fromUser, toUser);

        var fromUserName = fromUser.DisplayUserName(false, displayUserSettingsHelper);
        var toUserName = toUser.DisplayUserName(false, displayUserSettingsHelper);

        if (_httpHeaders != null)
        {
            messageService.Send(_httpHeaders, MessageAction.UserDataReassigns, messageTarget.Create(FromUser), new[] { fromUserName, toUserName });
        }
        else
        {
            messageService.Send(MessageAction.UserDataReassigns, messageTarget.Create(FromUser), fromUserName, toUserName);
        }
    }

    private void SendErrorNotify(UserManager userManager, StudioNotifyService studioNotifyService, string errorMessage)
    {
        var fromUser = userManager.GetUsers(FromUser);
        var toUser = userManager.GetUsers(ToUser);

        studioNotifyService.SendMsgReassignsFailed(_currentUserId, fromUser, toUser, errorMessage);
    }

    private async Task DeleteUserProfile(UserManager userManager, UserPhotoManager userPhotoManager, MessageService messageService, MessageTarget messageTarget, DisplayUserSettingsHelper displayUserSettingsHelper, QueueWorkerRemove queueWorkerRemove)
    {
        var user = userManager.GetUsers(FromUser);
        var userName = user.DisplayUserName(false, displayUserSettingsHelper);

        await userPhotoManager.RemovePhoto(user.Id);
        await userManager.DeleteUser(user.Id);
        queueWorkerRemove.Start(_tenantId, user, _currentUserId, false);

        if (_httpHeaders != null)
        {
            messageService.Send(_httpHeaders, MessageAction.UserDeleted, messageTarget.Create(FromUser), new[] { userName });
        }
        else
        {
            messageService.Send(MessageAction.UserDeleted, messageTarget.Create(FromUser), userName);
        }
    }
}

[Scope]
public class ReassignProgressItemScope
{
    private readonly TenantManager _tenantManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly MessageService _messageService;
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
