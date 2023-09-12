
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

namespace ASC.Web.Files.Utils;

[Scope]
public class FileSharingAceHelper
{
    private readonly FileSecurity _fileSecurity;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly FileUtility _fileUtility;
    private readonly UserManager _userManager;
    private readonly AuthContext _authContext;
    private readonly DocumentServiceHelper _documentServiceHelper;
    private readonly FileMarker _fileMarker;
    private readonly NotifyClient _notifyClient;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly FileSharingHelper _fileSharingHelper;
    private readonly FileTrackerHelper _fileTracker;
    private readonly FilesSettingsHelper _filesSettingsHelper;
    private readonly InvitationLinkService _invitationLinkService;
    private readonly StudioNotifyService _studioNotifyService;
    private readonly UsersInRoomChecker _usersInRoomChecker;
    private readonly UserManagerWrapper _userManagerWrapper;
    private readonly CountPaidUserChecker _countPaidUserChecker;
    private readonly ILogger _logger;

    public FileSharingAceHelper(
        FileSecurity fileSecurity,
        CoreBaseSettings coreBaseSettings,
        FileUtility fileUtility,
        UserManager userManager,
        AuthContext authContext,
        DocumentServiceHelper documentServiceHelper,
        FileMarker fileMarker,
        NotifyClient notifyClient,
        GlobalFolderHelper globalFolderHelper,
        FileSharingHelper fileSharingHelper,
        FileTrackerHelper fileTracker,
        FilesSettingsHelper filesSettingsHelper,
        InvitationLinkService invitationLinkService,
        StudioNotifyService studioNotifyService,
        ILoggerProvider loggerProvider,
        UsersInRoomChecker usersInRoomChecker,
        UserManagerWrapper userManagerWrapper,
        CountPaidUserChecker countPaidUserChecker)
    {
        _fileSecurity = fileSecurity;
        _coreBaseSettings = coreBaseSettings;
        _fileUtility = fileUtility;
        _userManager = userManager;
        _authContext = authContext;
        _documentServiceHelper = documentServiceHelper;
        _fileMarker = fileMarker;
        _notifyClient = notifyClient;
        _globalFolderHelper = globalFolderHelper;
        _fileSharingHelper = fileSharingHelper;
        _fileTracker = fileTracker;
        _filesSettingsHelper = filesSettingsHelper;
        _invitationLinkService = invitationLinkService;
        _studioNotifyService = studioNotifyService;
        _usersInRoomChecker = usersInRoomChecker;
        _logger = loggerProvider.CreateLogger("ASC.Files");
        _userManagerWrapper = userManagerWrapper;
        _countPaidUserChecker = countPaidUserChecker;
    }

    public async Task<(bool, string)> SetAceObjectAsync<T>(List<AceWrapper> aceWrappers, FileEntry<T> entry, bool notify, string message, AceAdvancedSettingsWrapper advancedSettings)
    {
        if (entry == null)
        {
            throw new ArgumentNullException(FilesCommonResource.ErrorMassage_BadRequest);
        }

        if (!aceWrappers.All(r => r.Id == _authContext.CurrentAccount.ID && r.Access == FileShare.None) && !await _fileSharingHelper.CanSetAccessAsync(entry) && advancedSettings is not { InvitationLink: true })
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);
        }

        if (entry is Folder<T> { Private: true } && advancedSettings is not { AllowSharingPrivateRoom: true })
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);
        }

        var ownerId = entry.RootFolderType == FolderType.USER ? entry.RootCreateBy : entry.CreateBy;
        var room = entry is Folder<T> folder && DocSpaceHelper.IsRoom(folder.FolderType) ? folder : null;
        var entryType = entry.FileEntryType;
        var recipients = new Dictionary<Guid, FileShare>();
        var usersWithoutRight = new List<Guid>();
        var changed = false;
        string warning = null;
        var shares = (await _fileSecurity.GetSharesAsync(entry)).ToList();

        foreach (var w in aceWrappers.OrderByDescending(ace => ace.SubjectGroup))
        {
            var emailInvite = !string.IsNullOrEmpty(w.Email);
            var currentUserType = await _userManager.GetUserTypeAsync(w.Id);
            var userType = EmployeeType.User;
            var existedShare = shares.FirstOrDefault(r => r.Subject == w.Id);
            var rightIsAvailable = FileSecurity.AvailableUserRights.TryGetValue(currentUserType, out var userAccesses)
                                  && userAccesses.Contains(w.Access);

            if (room != null)
            {
                if (FileSecurity.AvailableRoomRights.TryGetValue(room.FolderType, out var roomAccesses) && !roomAccesses.Contains(w.Access))
                {
                    continue;
                }

                if (room.FolderType is not (FolderType.PublicRoom or FolderType.CustomRoom) && w.SubjectType == SubjectType.ExternalLink)
                {
                    continue;
                }

                if (room.FolderType == FolderType.PublicRoom && w.Access == FileShare.Read && w.SubjectType != SubjectType.ExternalLink)
                {
                    continue;
                }
            }

            if (room != null && existedShare is not { IsLink: true } && !w.IsLink)
            {
                if (currentUserType == EmployeeType.DocSpaceAdmin && !rightIsAvailable)
                {
                    continue;
                }

                if (existedShare != null && !rightIsAvailable)
                {
                    throw new InvalidOperationException(FilesCommonResource.ErrorMessage_RoleNotAvailable);
                }

                try
                {
                    if (!rightIsAvailable && currentUserType == EmployeeType.User)
                    {
                        await _countPaidUserChecker.CheckAppend();
                    }

                    userType = FileSecurity.GetTypeByShare(w.Access);

                    if (!emailInvite && currentUserType != EmployeeType.DocSpaceAdmin)
                    {
                        var user = await _userManager.GetUsersAsync(w.Id);
                        await _userManagerWrapper.UpdateUserTypeAsync(user, userType);
                    }
                }
                catch (TenantQuotaException e)
                {
                    warning ??= e.Message;
                    w.Access = FileSecurity.GetHighFreeRole(room.FolderType);

                    if (w.Access == FileShare.None)
                    {
                        continue;
                    }
                }
                catch (Exception e)
                {
                    warning ??= e.Message;
                    continue;
                }

                if (emailInvite)
                {
                    try
                    {
                        var user = await _userManagerWrapper.AddInvitedUserAsync(w.Email, userType);
                        w.Id = user.Id;
                    }
                    catch (Exception e)
                    {
                        warning ??= e.Message;
                        continue;
                    }
                }
            }

            var subjects = await _fileSecurity.GetUserSubjectsAsync(w.Id);

            if (entry.RootFolderType == FolderType.COMMON && subjects.Contains(Constants.GroupAdmin.ID)
                || ownerId == w.Id)
            {
                continue;
            }

            var share = w.Access;

            if (w.Id == FileConstant.ShareLinkId)
            {
                if (w.Access == FileShare.ReadWrite && await _userManager.IsUserAsync(_authContext.CurrentAccount.ID))
                {
                    throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);
                }

                if (_coreBaseSettings.Personal && !_fileUtility.CanWebView(entry.Title) && w.Access != FileShare.Restrict)
                {
                    throw new SecurityException(FilesCommonResource.ErrorMassage_BadRequest);
                }

                share = w.Access == FileShare.Restrict || !_filesSettingsHelper.ExternalShare
                    ? FileShare.None
                    : w.Access;
            }

            await _fileSecurity.ShareAsync(entry.Id, entryType, w.Id, share, w.SubjectType, w.FileShareOptions);
            changed = true;

            if (emailInvite)
            {
                var link = await _invitationLinkService.GetInvitationLinkAsync(w.Email, share, _authContext.CurrentAccount.ID);
                await _studioNotifyService.SendEmailRoomInviteAsync(w.Email, entry.Title, link);
                _logger.Debug(link);
            }

            if (w.Id == FileConstant.ShareLinkId)
            {
                continue;
            }

            entry.Access = share;

            var listUsersId = new List<Guid>();

            if (w.SubjectGroup)
            {
                listUsersId = (await _userManager.GetUsersByGroupAsync(w.Id)).Select(ui => ui.Id).ToList();
            }
            else
            {
                listUsersId.Add(w.Id);
            }

            listUsersId.Remove(_authContext.CurrentAccount.ID);

            if (entryType == FileEntryType.File)
            {
                listUsersId.ForEach(uid => _fileTracker.ChangeRight(entry.Id, uid, true));
            }

            var addRecipient = share == FileShare.Read
                               || share == FileShare.CustomFilter
                               || share == FileShare.ReadWrite
                               || share == FileShare.Review
                               || share == FileShare.FillForms
                               || share == FileShare.Comment
                               || share == FileShare.RoomAdmin
                               || share == FileShare.Editing
                               || share == FileShare.Collaborator
                               || (share == FileShare.None && entry.RootFolderType == FolderType.COMMON);

            var removeNew = share == FileShare.Restrict || (share == FileShare.None
                && (entry.RootFolderType == FolderType.USER ||
                    entry.RootFolderType == FolderType.VirtualRooms ||
                    entry.RootFolderType == FolderType.Archive));

            listUsersId.ForEach(id =>
            {
                recipients.Remove(id);
                if (addRecipient)
                {
                    recipients.Add(id, share);
                }
                else if (removeNew)
                {
                    usersWithoutRight.Add(id);
                }
            });
        }

        if (entryType == FileEntryType.File)
        {
            await _documentServiceHelper.CheckUsersForDropAsync((File<T>)entry);
        }

        if (recipients.Count > 0)
        {
            if (entryType == FileEntryType.File
                || ((Folder<T>)entry).FoldersCount + ((Folder<T>)entry).FilesCount > 0
                || entry.ProviderEntry)
            {
                await _fileMarker.MarkAsNewAsync(entry, recipients.Keys.ToList());
            }

            if ((entry.RootFolderType == FolderType.USER
               || entry.RootFolderType == FolderType.Privacy)
               && notify)
            {
                await _notifyClient.SendShareNoticeAsync(entry, recipients, message);
            }
        }

        if (advancedSettings != null && entryType == FileEntryType.File && ownerId == _authContext.CurrentAccount.ID && _fileUtility.CanWebView(entry.Title) && !entry.ProviderEntry)
        {
            await _fileSecurity.ShareAsync(entry.Id, entryType, FileConstant.DenyDownloadId, advancedSettings.DenyDownload ? FileShare.Restrict : FileShare.None);
            await _fileSecurity.ShareAsync(entry.Id, entryType, FileConstant.DenySharingId, advancedSettings.DenySharing ? FileShare.Restrict : FileShare.None);
        }

        foreach (var userId in usersWithoutRight)
        {
            await _fileMarker.RemoveMarkAsNewAsync(entry, userId);
        }

        return (changed, warning);
    }

    public async Task RemoveAceAsync<T>(FileEntry<T> entry)
    {
        if (entry.RootFolderType != FolderType.USER && entry.RootFolderType != FolderType.Privacy
                || Equals(entry.RootId, await _globalFolderHelper.FolderMyAsync)
                || Equals(entry.RootId, await _globalFolderHelper.FolderPrivacyAsync))
        {
            return;
        }

        var entryType = entry.FileEntryType;
        await _fileSecurity.ShareAsync(entry.Id, entryType, _authContext.CurrentAccount.ID,
                entry.RootFolderType == FolderType.USER
                ? _fileSecurity.DefaultMyShare
                : _fileSecurity.DefaultPrivacyShare);

        if (entryType == FileEntryType.File)
        {
            await _documentServiceHelper.CheckUsersForDropAsync((File<T>)entry);
        }

        await _fileMarker.RemoveMarkAsNewAsync(entry);
    }
}

[Scope]
public class FileSharingHelper
{
    public FileSharingHelper(
        Global global,
        GlobalFolderHelper globalFolderHelper,
        FileSecurity fileSecurity,
        AuthContext authContext,
        UserManager userManager,
        CoreBaseSettings coreBaseSettings)
    {
        _global = global;
        _globalFolderHelper = globalFolderHelper;
        _fileSecurity = fileSecurity;
        _authContext = authContext;
        _userManager = userManager;
        _coreBaseSettings = coreBaseSettings;
    }

    private readonly Global _global;
    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly FileSecurity _fileSecurity;
    private readonly AuthContext _authContext;
    private readonly UserManager _userManager;
    private readonly CoreBaseSettings _coreBaseSettings;

    public async Task<bool> CanSetAccessAsync<T>(FileEntry<T> entry)
    {
        var folder = entry as Folder<T>;

        if (entry == null)
        {
            return false;
        }

        if (entry.RootFolderType == FolderType.COMMON && await _global.IsDocSpaceAdministratorAsync)
        {
            return true;
        }

        if (await _fileSecurity.CanEditAccessAsync(entry))
        {
            return true;
        }

        if (await _userManager.IsUserAsync(_authContext.CurrentAccount.ID))
        {
            return false;
        }

        if (_coreBaseSettings.DisableDocSpace)
        {
            if (entry.RootFolderType == FolderType.USER && Equals(entry.RootId, await _globalFolderHelper.FolderMyAsync) || await _fileSecurity.CanShareAsync(entry))
            {
                return true;
            }
        }
        else
        {
            if (entry.RootFolderType == FolderType.USER && Equals(entry.RootId, await _globalFolderHelper.FolderMyAsync))
            {
                return false;
            }
        }


        return entry.RootFolderType == FolderType.Privacy
                && entry is File<T>
                && (Equals(entry.RootId, await _globalFolderHelper.FolderPrivacyAsync) || await _fileSecurity.CanShareAsync(entry));
    }
}

[Scope]
public class FileSharing
{
    private readonly ILogger<FileSharing> _logger;
    private readonly Global _global;
    private readonly FileSecurity _fileSecurity;
    private readonly AuthContext _authContext;
    private readonly UserManager _userManager;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly FileShareLink _fileShareLink;
    private readonly IDaoFactory _daoFactory;
    private readonly FileSharingHelper _fileSharingHelper;
    private readonly FilesSettingsHelper _filesSettingsHelper;
    private readonly InvitationLinkService _invitationLinkService;
    private readonly ExternalShare _externalShare;

    public FileSharing(
        Global global,
        FileSecurity fileSecurity,
        AuthContext authContext,
        UserManager userManager,
        ILogger<FileSharing> logger,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        FileShareLink fileShareLink,
        IDaoFactory daoFactory,
        FileSharingHelper fileSharingHelper,
        FilesSettingsHelper filesSettingsHelper,
        InvitationLinkService invitationLinkService,
        ExternalShare externalShare)
    {
        _global = global;
        _fileSecurity = fileSecurity;
        _authContext = authContext;
        _userManager = userManager;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _fileShareLink = fileShareLink;
        _daoFactory = daoFactory;
        _fileSharingHelper = fileSharingHelper;
        _filesSettingsHelper = filesSettingsHelper;
        _logger = logger;
        _invitationLinkService = invitationLinkService;
        _externalShare = externalShare;
    }

    public async Task<bool> CanSetAccessAsync<T>(FileEntry<T> entry)
    {
        return await _fileSharingHelper.CanSetAccessAsync(entry);
    }

    public async Task<List<AceWrapper>> GetSharedInfoAsync<T>(FileEntry<T> entry, IEnumerable<SubjectType> subjectsTypes = null, bool withoutTemplates = false)
    {
        if (entry == null)
        {
            throw new ArgumentNullException(FilesCommonResource.ErrorMassage_BadRequest);
        }

        if (!await _fileSecurity.CanReadAsync(entry))
        {
            _logger.ErrorUserCanTGetSharedInfo(_authContext.CurrentAccount.ID, entry.FileEntryType, entry.Id.ToString());

            return new List<AceWrapper>();
            //throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException);
        }

        var linkAccess = FileShare.Restrict;
        var result = new List<AceWrapper>();
        var shares = await _fileSecurity.GetSharesAsync(entry);
        var isRoom = entry is Folder<T> { Private: false } room && DocSpaceHelper.IsRoom(room.FolderType);
        var canEditAccess = await _fileSecurity.CanEditAccessAsync(entry);

        var records = shares
            .GroupBy(r => r.Subject)
            .Select(g => g.OrderBy(r => r.Level)
                          .ThenBy(r => r.Level)
                          .ThenByDescending(r => r.Share, new FileShareRecord.ShareComparer()).FirstOrDefault());

        foreach (var r in records)
        {
            if (subjectsTypes != null && !subjectsTypes.Contains(r.SubjectType))
            {
                continue;
            }

            if (r.Subject == FileConstant.ShareLinkId)
            {
                linkAccess = r.Share;
                continue;
            }

            if (r.Subject == FileConstant.DenyDownloadId || r.Subject == FileConstant.DenySharingId)
            {
                continue;
            }

            var u = await _userManager.GetUsersAsync(r.Subject);
            var isgroup = false;
            var title = u.DisplayUserName(false, _displayUserSettingsHelper);
            var share = r.Share;

            if (u.Id == Constants.LostUser.Id && !r.IsLink)
            {
                var g = await _userManager.GetGroupInfoAsync(r.Subject);
                isgroup = true;
                title = g.Name;

                if (g.ID == Constants.GroupAdmin.ID)
                {
                    title = FilesCommonResource.Admin;
                }

                if (g.ID == Constants.GroupEveryone.ID)
                {
                    title = FilesCommonResource.Everyone;
                }

                if (g.ID == Constants.LostGroupInfo.ID)
                {
                    await _fileSecurity.RemoveSubjectAsync<T>(r.Subject);

                    continue;
                }
            }

            var w = new AceWrapper
            {
                Id = r.Subject,
                SubjectGroup = isgroup,
                Access = share,
                FileShareOptions = r.Options,
            };

            w.CanEditAccess = _authContext.CurrentAccount.ID != w.Id && w.SubjectType is SubjectType.User or SubjectType.Group && canEditAccess;

            if (isRoom && r.IsLink)
            {
                if (!canEditAccess)
                {
                    continue;
                }
                
                w.Link = r.SubjectType == SubjectType.InvitationLink ?
                    _invitationLinkService.GetInvitationLink(r.Subject, _authContext.CurrentAccount.ID) :
                    await _externalShare.GetLinkAsync(r.Subject);
                w.SubjectGroup = true;
                w.CanEditAccess = false;
                w.FileShareOptions.Password = await _externalShare.GetPasswordAsync(w.FileShareOptions.Password);
                w.SubjectType = r.SubjectType;
            }
            else
            {
                w.SubjectName = title;
                w.Owner = entry.RootFolderType == FolderType.USER
                            ? entry.RootCreateBy == r.Subject
                            : entry.CreateBy == r.Subject;
                w.LockedRights = r.Subject == _authContext.CurrentAccount.ID;
            }

            result.Add(w);
        }

        if (isRoom && canEditAccess&& !withoutTemplates)
        {
            var invitationId = Guid.NewGuid();

            var invitationAceTemplate = new AceWrapper
            {
                Id = invitationId,
                Link = _invitationLinkService.GetInvitationLink(invitationId, _authContext.CurrentAccount.ID),
                SubjectGroup = true,
                Access = ((Folder<T>)entry).FolderType == FolderType.PublicRoom ? FileShare.RoomAdmin : FileShare.Read,
                Owner = false,
                IsTemplate = true,
                SubjectType = SubjectType.InvitationLink
            };

            var externalId = Guid.NewGuid();

            var externalAceTemplate = new AceWrapper
            {
                Id = externalId,
                Link = await _externalShare.GetLinkAsync(externalId),
                SubjectGroup = true,
                Access = FileShare.Read,
                Owner = false,
                IsTemplate = true,
                SubjectType = SubjectType.ExternalLink
            };

            result.Add(invitationAceTemplate);
            result.Add(externalAceTemplate);
        }

        if (entry.FileEntryType == FileEntryType.File && result.All(w => w.Id != FileConstant.ShareLinkId)
            && entry.FileEntryType == FileEntryType.File
            && !((File<T>)entry).Encrypted)
        {
            var w = new AceWrapper
            {
                Id = FileConstant.ShareLinkId,
                Link = _filesSettingsHelper.ExternalShare ? await _fileShareLink.GetLinkAsync((File<T>)entry) : string.Empty,
                SubjectGroup = true,
                Access = linkAccess,
                Owner = false
            };

            result.Add(w);
        }

        if (!result.Any(w => w.Owner) && (subjectsTypes == null || subjectsTypes.Contains(SubjectType.User) || subjectsTypes.Contains(SubjectType.Group)))
        {
            var ownerId = entry.RootFolderType == FolderType.USER ? entry.RootCreateBy : entry.CreateBy;
            var w = new AceWrapper
            {
                Id = ownerId,
                SubjectName = await _global.GetUserNameAsync(ownerId),
                SubjectGroup = false,
                Access = FileShare.ReadWrite,
                Owner = true,
                CanEditAccess = false,
            };

            result.Add(w);
        }

        if (result.Any(w => w.Id == _authContext.CurrentAccount.ID))
        {
            result.Single(w => w.Id == _authContext.CurrentAccount.ID).LockedRights = true;
        }

        if (entry.RootFolderType == FolderType.COMMON)
        {
            if (result.All(w => w.Id != Constants.GroupAdmin.ID))
            {
                var w = new AceWrapper
                {
                    Id = Constants.GroupAdmin.ID,
                    SubjectName = FilesCommonResource.Admin,
                    SubjectGroup = true,
                    Access = FileShare.ReadWrite,
                    Owner = false,
                    LockedRights = true,
                };

                result.Add(w);
            }

            var index = result.FindIndex(w => w.Id == Constants.GroupEveryone.ID);
            if (index == -1)
            {
                var w = new AceWrapper
                {
                    Id = Constants.GroupEveryone.ID,
                    SubjectName = FilesCommonResource.Everyone,
                    SubjectGroup = true,
                    Access = _fileSecurity.DefaultCommonShare,
                    Owner = false,
                    DisableRemove = true
                };

                result.Add(w);
            }
            else
            {
                result[index].DisableRemove = true;
            }
        }

        return result;
    }

    public async Task<List<AceWrapper>> GetSharedInfoAsync<T>(IEnumerable<T> fileIds, IEnumerable<T> folderIds, IEnumerable<SubjectType> subjectTypes = null,
        bool withoutTemplates = false)
    {
        if (!_authContext.IsAuthenticated)
        {
            throw new InvalidOperationException(FilesCommonResource.ErrorMassage_SecurityException);
        }

        var result = new List<AceWrapper>();

        var fileDao = _daoFactory.GetFileDao<T>();
        var files = await fileDao.GetFilesAsync(fileIds).ToListAsync();

        var folderDao = _daoFactory.GetFolderDao<T>();
        var folders = await folderDao.GetFoldersAsync(folderIds).ToListAsync();

        var entries = files.Cast<FileEntry<T>>().Concat(folders.Cast<FileEntry<T>>());

        foreach (var entry in entries)
        {
            IEnumerable<AceWrapper> acesForObject;
            try
            {
                acesForObject = await GetSharedInfoAsync(entry, subjectTypes, withoutTemplates);
            }
            catch (Exception e)
            {
                _logger.ErrorGetSharedInfo(e);

                throw new InvalidOperationException(e.Message, e);
            }

            foreach (var aceForObject in acesForObject)
            {
                var duplicate = result.FirstOrDefault(ace => ace.Id == aceForObject.Id);
                if (duplicate == null)
                {
                    if (result.Count > 0)
                    {
                        aceForObject.Owner = false;
                        aceForObject.Access = FileShare.Varies;
                    }

                    continue;
                }

                if (duplicate.Access != aceForObject.Access)
                {
                    aceForObject.Access = FileShare.Varies;
                }
                if (duplicate.Owner != aceForObject.Owner)
                {
                    aceForObject.Owner = false;
                    aceForObject.Access = FileShare.Varies;
                }

                result.Remove(duplicate);
            }

            var withoutAce = result.Where(ace =>
                                            acesForObject.FirstOrDefault(aceForObject =>
                                                                        aceForObject.Id == ace.Id) == null);
            foreach (var ace in withoutAce)
            {
                ace.Access = FileShare.Varies;
            }

            var notOwner = result.Where(ace =>
                                        ace.Owner &&
                                        acesForObject.FirstOrDefault(aceForObject =>
                                                                        aceForObject.Owner
                                                                        && aceForObject.Id == ace.Id) == null);
            foreach (var ace in notOwner)
            {
                ace.Owner = false;
                ace.Access = FileShare.Varies;
            }

            result.AddRange(acesForObject);
        }


        var ownerAce = result.FirstOrDefault(ace => ace.Owner);
        result.Remove(ownerAce);

        var meAce = result.FirstOrDefault(ace => ace.Id == _authContext.CurrentAccount.ID);
        result.Remove(meAce);

        AceWrapper linkAce = null;
        if (entries.Count() > 1)
        {
            result.RemoveAll(ace => ace.Id == FileConstant.ShareLinkId);
        }
        else
        {
            linkAce = result.FirstOrDefault(ace => ace.Id == FileConstant.ShareLinkId);
        }

        result.Sort((x, y) => string.Compare(x.SubjectName, y.SubjectName));

        if (ownerAce != null)
        {
            result = new List<AceWrapper> { ownerAce }.Concat(result).ToList();
        }
        if (meAce != null)
        {
            result = new List<AceWrapper> { meAce }.Concat(result).ToList();
        }
        if (linkAce != null)
        {
            result.Remove(linkAce);
            result = new List<AceWrapper> { linkAce }.Concat(result).ToList();
        }

        return new List<AceWrapper>(result);
    }

    public async Task<List<AceShortWrapper>> GetSharedInfoShortFileAsync<T>(T fileID)
    {
        var aces = await GetSharedInfoAsync(new List<T> { fileID }, new List<T>());

        return GetAceShortWrappers(aces);
    }

    public async Task<List<AceShortWrapper>> GetSharedInfoShortFolderAsync<T>(T folderId)
    {
        var aces = await GetSharedInfoAsync(new List<T>(), new List<T> { folderId });

        return GetAceShortWrappers(aces);
    }

    private List<AceShortWrapper> GetAceShortWrappers(List<AceWrapper> aces)
    {
        return new List<AceShortWrapper>(aces
            .Where(aceWrapper => !aceWrapper.Id.Equals(FileConstant.ShareLinkId) || aceWrapper.Access != FileShare.Restrict)
            .Select(aceWrapper => new AceShortWrapper(aceWrapper)));
    }
}