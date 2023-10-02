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

using Actions = ASC.Web.Studio.Core.Notify.Actions;

namespace ASC.Files.Core.Security;

[Scope]
public class FileSecurityCommon
{
    private readonly UserManager _userManager;
    private readonly WebItemSecurity _webItemSecurity;

    public FileSecurityCommon(UserManager userManager, WebItemSecurity webItemSecurity)
    {
        _userManager = userManager;
        _webItemSecurity = webItemSecurity;
    }

    public async Task<bool> IsDocSpaceAdministratorAsync(Guid userId)
    {
        return await _userManager.IsUserInGroupAsync(userId, Constants.GroupAdmin.ID) ||
               await _webItemSecurity.IsProductAdministratorAsync(ProductEntryPoint.ID, userId);
    }
}

[Scope]
public class FileSecurity : IFileSecurity
{
    private readonly IDaoFactory _daoFactory;
    public readonly FileShare DefaultMyShare = FileShare.Restrict;
    public readonly FileShare DefaultCommonShare = FileShare.Read;
    public readonly FileShare DefaultPrivacyShare = FileShare.Restrict;
    public readonly FileShare DefaultArchiveShare = FileShare.Restrict;
    public readonly FileShare DefaultVirtualRoomsShare = FileShare.Restrict;

    public static readonly Dictionary<FolderType, Dictionary<SubjectType, HashSet<FileShare>>> AvailableRoomAccesses = new()
    {
        {
            FolderType.CustomRoom, new Dictionary<SubjectType, HashSet<FileShare>>
            {
                {
                    SubjectType.User, new HashSet<FileShare>
                    {
                        FileShare.RoomAdmin, FileShare.Collaborator, FileShare.Editing, FileShare.Review, FileShare.Comment, FileShare.FillForms, FileShare.Read, FileShare.None
                    }
                },
                {
                    SubjectType.InvitationLink, new HashSet<FileShare>
                    {
                        FileShare.RoomAdmin, FileShare.Collaborator, FileShare.Editing, FileShare.Review, FileShare.Comment, FileShare.FillForms, FileShare.Read, FileShare.None
                    }
                },
                {
                    SubjectType.ExternalLink, new HashSet<FileShare>
                    {
                        FileShare.Read, FileShare.None
                    }
                },
                {
                    SubjectType.PrimaryExternalLink, new HashSet<FileShare>
                    {
                        FileShare.Read, FileShare.None
                    }
                }
            }
        },
        {
            FolderType.PublicRoom, new Dictionary<SubjectType, HashSet<FileShare>>
            {
                {
                    SubjectType.User, new HashSet<FileShare>
                    {
                        FileShare.RoomAdmin, FileShare.Collaborator, FileShare.None
                    }
                },
                {
                    SubjectType.InvitationLink, new HashSet<FileShare>
                    {
                        FileShare.RoomAdmin, FileShare.Collaborator, FileShare.Read, FileShare.None
                    }
                },
                {
                    SubjectType.ExternalLink, new HashSet<FileShare>
                    {
                        FileShare.Read, FileShare.None
                    }
                },
                {
                    SubjectType.PrimaryExternalLink, new HashSet<FileShare>
                    {
                        FileShare.Read, FileShare.None
                    }
                }
            }
        },
        {
            FolderType.FillingFormsRoom, new Dictionary<SubjectType, HashSet<FileShare>>
            {
                {
                    SubjectType.User, new HashSet<FileShare>
                    {
                        FileShare.RoomAdmin, FileShare.Collaborator, FileShare.FillForms, FileShare.Read, FileShare.None
                    }
                },
                {
                    SubjectType.InvitationLink, new HashSet<FileShare>
                    {
                        FileShare.RoomAdmin, FileShare.Collaborator, FileShare.FillForms, FileShare.Read, FileShare.None
                    }
                }
            }
        },
        {
            FolderType.EditingRoom, new Dictionary<SubjectType, HashSet<FileShare>>
            {
                {
                    SubjectType.User, new HashSet<FileShare>
                    {
                        FileShare.RoomAdmin, FileShare.Collaborator, FileShare.Editing, FileShare.Read, FileShare.None
                    }
                },
                {
                    SubjectType.InvitationLink, new HashSet<FileShare>
                    {
                        FileShare.RoomAdmin, FileShare.Collaborator, FileShare.Editing, FileShare.Read, FileShare.None
                    }
                }
            }
        },
        {
            FolderType.ReviewRoom, new Dictionary<SubjectType, HashSet<FileShare>>
            {
                {
                    SubjectType.User, new HashSet<FileShare>
                    {
                        FileShare.RoomAdmin, FileShare.Collaborator, FileShare.Review, FileShare.Comment, FileShare.Read, FileShare.None
                    }
                },
                {
                    SubjectType.InvitationLink, new HashSet<FileShare>
                    {
                        FileShare.RoomAdmin, FileShare.Collaborator, FileShare.Review, FileShare.Comment, FileShare.Read, FileShare.None
                    }
                }
            }
        },
        {
            FolderType.ReadOnlyRoom, new Dictionary<SubjectType, HashSet<FileShare>>
            {
                {
                    SubjectType.User, new HashSet<FileShare>
                    {
                        FileShare.RoomAdmin, FileShare.Collaborator, FileShare.Read, FileShare.None
                    }
                },
                {
                    SubjectType.InvitationLink, new HashSet<FileShare>
                    {
                        FileShare.RoomAdmin, FileShare.Collaborator, FileShare.Read, FileShare.None
                    }
                }
            }
        }
    };

    public static readonly Dictionary<EmployeeType, HashSet<FileShare>> AvailableUserAccesses = new()
    {
        {
            EmployeeType.DocSpaceAdmin, new HashSet<FileShare>
            {
                FileShare.RoomAdmin, FileShare.None
            }
        },
        {
            EmployeeType.RoomAdmin, new HashSet<FileShare>
            {
                FileShare.RoomAdmin, FileShare.Collaborator, FileShare.Editing, FileShare.Review, FileShare.Comment, FileShare.FillForms, FileShare.Read, FileShare.None
            }
        },
        {
            EmployeeType.Collaborator, new HashSet<FileShare>
            {
                FileShare.Collaborator, FileShare.Editing, FileShare.Review, FileShare.Comment, FileShare.FillForms, FileShare.Read, FileShare.None
            }
        },
        {
            EmployeeType.User, new HashSet<FileShare>
            {
                FileShare.Editing, FileShare.Review, FileShare.Comment, FileShare.FillForms, FileShare.Read, FileShare.None
            }
        }
    };

    private static readonly IDictionary<FileEntryType, IEnumerable<FilesSecurityActions>> _securityEntries =
    new Dictionary<FileEntryType, IEnumerable<FilesSecurityActions>>()
    {
            {
                FileEntryType.File, new List<FilesSecurityActions>()
                {
                    FilesSecurityActions.Read,
                    FilesSecurityActions.Comment,
                    FilesSecurityActions.FillForms,
                    FilesSecurityActions.Review,
                    FilesSecurityActions.Edit,
                    FilesSecurityActions.Delete,
                    FilesSecurityActions.CustomFilter,
                    FilesSecurityActions.Rename,
                    FilesSecurityActions.ReadHistory,
                    FilesSecurityActions.Lock,
                    FilesSecurityActions.EditHistory,
                    FilesSecurityActions.Copy,
                    FilesSecurityActions.Move,
                    FilesSecurityActions.Duplicate,
                    FilesSecurityActions.SubmitToFormGallery,
                    FilesSecurityActions.Download,
                    FilesSecurityActions.Convert
                }
            },
            {
                FileEntryType.Folder, new List<FilesSecurityActions>()
                {
                    FilesSecurityActions.Read,
                    FilesSecurityActions.Create,
                    FilesSecurityActions.Delete,
                    FilesSecurityActions.EditRoom,
                    FilesSecurityActions.Rename,
                    FilesSecurityActions.CopyTo,
                    FilesSecurityActions.MoveTo,
                    FilesSecurityActions.Copy,
                    FilesSecurityActions.Move,
                    FilesSecurityActions.Pin,
                    FilesSecurityActions.Mute,
                    FilesSecurityActions.EditAccess,
                    FilesSecurityActions.Duplicate,
                    FilesSecurityActions.Download,
                    FilesSecurityActions.CopySharedLink
                }
            }
    };

    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;
    private readonly AuthContext _authContext;
    private readonly AuthManager _authManager;
    private readonly GlobalFolder _globalFolder;
    private readonly FileSecurityCommon _fileSecurityCommon;
    private readonly FileUtility _fileUtility;
    private readonly StudioNotifyHelper _studioNotifyHelper;
    private readonly BadgesSettingsHelper _badgesSettingsHelper;
    private readonly ExternalShare _externalShare;
    private readonly ConcurrentDictionary<string, FileShareRecord> _cachedRecords = new();
    private readonly ConcurrentDictionary<string, Guid> _cachedRoomOwner = new();

    public FileSecurity(
        IDaoFactory daoFactory,
        UserManager userManager,
        TenantManager tenantManager,
        AuthContext authContext,
        AuthManager authManager,
        GlobalFolder globalFolder,
        FileSecurityCommon fileSecurityCommon,
        FileUtility fileUtility,
        StudioNotifyHelper studioNotifyHelper,
        BadgesSettingsHelper badgesSettingsHelper, 
        ExternalShare externalShare)
    {
        _daoFactory = daoFactory;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _authContext = authContext;
        _authManager = authManager;
        _globalFolder = globalFolder;
        _fileSecurityCommon = fileSecurityCommon;
        _fileUtility = fileUtility;
        _studioNotifyHelper = studioNotifyHelper;
        _badgesSettingsHelper = badgesSettingsHelper;
        _externalShare = externalShare;
    }

    public IAsyncEnumerable<Tuple<FileEntry<T>, bool>> CanReadAsync<T>(IAsyncEnumerable<FileEntry<T>> entries, Guid userId)
    {
        return CanAsync(entries, userId, FilesSecurityActions.Read);
    }

    public IAsyncEnumerable<Tuple<FileEntry<T>, bool>> CanReadAsync<T>(IAsyncEnumerable<FileEntry<T>> entries)
    {
        return CanReadAsync(entries, _authContext.CurrentAccount.ID);
    }

    public async Task<bool> CanReadAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return await CanAsync(entry, userId, FilesSecurityActions.Read);
    }

    public async Task<bool> CanReadHistoryAsync<T>(FileEntry<T> entry)
    {
        return await CanReadHistoryAsync(entry, _authContext.CurrentAccount.ID);
    }

    public async Task<bool> CanReadHistoryAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return await CanAsync(entry, userId, FilesSecurityActions.ReadHistory);
    }

    public async Task<bool> CanCommentAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return await CanAsync(entry, userId, FilesSecurityActions.Comment);
    }

    public async Task<bool> CanFillFormsAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return await CanAsync(entry, userId, FilesSecurityActions.FillForms);
    }

    public async Task<bool> CanReviewAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return await CanAsync(entry, userId, FilesSecurityActions.Review);
    }

    public async Task<bool> CanCustomFilterEditAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return await CanAsync(entry, userId, FilesSecurityActions.CustomFilter);
    }

    public async Task<bool> CanCreateAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return await CanAsync(entry, userId, FilesSecurityActions.Create);
    }

    public async Task<bool> CanEditAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return await CanAsync(entry, userId, FilesSecurityActions.Edit);
    }

    public async Task<bool> CanDeleteAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return await CanAsync(entry, userId, FilesSecurityActions.Delete);
    }

    public async Task<bool> CanEditRoomAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return await CanAsync(entry, userId, FilesSecurityActions.EditRoom);
    }

    public async Task<bool> CanRenameAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return await CanAsync(entry, userId, FilesSecurityActions.Rename);
    }

    public async Task<bool> CanDownloadAsync<T>(FileEntry<T> entry)
    {
        return await CanDownloadAsync(entry, _authContext.CurrentAccount.ID);
    }

    public async Task<bool> CanDownloadAsync<T>(FileEntry<T> entry, Guid userId)
    {
        if (!await CanAsync(entry, userId, FilesSecurityActions.Download))
        {
            return false;
        }

        return CheckDenyDownload(entry);
    }

    public async Task<bool> CanShareAsync<T>(FileEntry<T> entry, Guid userId)
    {
        if (!await CanEditAsync(entry, userId))
        {
            return false;
        }

        return CheckDenySharing(entry);
    }

    public Task<bool> CanConvert<T>(FileEntry<T> entry, Guid userId)
    {
        return CanAsync(entry, userId, FilesSecurityActions.Convert);
    }

    public async Task<bool> CanReadAsync<T>(FileEntry<T> entry)
    {
        return await CanReadAsync(entry, _authContext.CurrentAccount.ID);
    }

    public async Task<bool> CanCommentAsync<T>(FileEntry<T> entry)
    {
        return await CanCommentAsync(entry, _authContext.CurrentAccount.ID);
    }

    public async Task<bool> CanCustomFilterEditAsync<T>(FileEntry<T> entry)
    {
        return await CanCustomFilterEditAsync(entry, _authContext.CurrentAccount.ID);
    }

    public async Task<bool> CanFillFormsAsync<T>(FileEntry<T> entry)
    {
        return await CanFillFormsAsync(entry, _authContext.CurrentAccount.ID);
    }

    public async Task<bool> CanReviewAsync<T>(FileEntry<T> entry)
    {
        return await CanReviewAsync(entry, _authContext.CurrentAccount.ID);
    }

    public async Task<bool> CanCreateAsync<T>(FileEntry<T> entry)
    {
        return await CanCreateAsync(entry, _authContext.CurrentAccount.ID);
    }

    public async Task<bool> CanEditAsync<T>(FileEntry<T> entry)
    {
        return await CanEditAsync(entry, _authContext.CurrentAccount.ID);
    }

    public async Task<bool> CanRenameAsync<T>(FileEntry<T> entry)
    {
        return await CanRenameAsync(entry, _authContext.CurrentAccount.ID);
    }

    public async Task<bool> CanDeleteAsync<T>(FileEntry<T> entry)
    {
        return await CanDeleteAsync(entry, _authContext.CurrentAccount.ID);
    }
    public async Task<bool> CanDownload<T>(FileEntry<T> entry)
    {
        return await CanDownloadAsync(entry, _authContext.CurrentAccount.ID);
    }

    public async Task<bool> CanEditRoomAsync<T>(FileEntry<T> entry)
    {
        return await CanEditRoomAsync(entry, _authContext.CurrentAccount.ID);
    }

    public async Task<bool> CanShareAsync<T>(FileEntry<T> entry)
    {
        return await CanShareAsync(entry, _authContext.CurrentAccount.ID);
    }

    public async Task<bool> CanLockAsync<T>(FileEntry<T> entry)
    {
        return await CanLockAsync(entry, _authContext.CurrentAccount.ID);
    }

    public async Task<bool> CanLockAsync<T>(FileEntry<T> entry, Guid userId)
    {
        return await CanAsync(entry, userId, FilesSecurityActions.Lock);
    }

    public async Task<bool> CanCopyToAsync<T>(FileEntry<T> entry)
    {
        return await CanAsync(entry, _authContext.CurrentAccount.ID, FilesSecurityActions.CopyTo);
    }

    public async Task<bool> CanCopyAsync<T>(FileEntry<T> entry)
    {
        return await CanAsync(entry, _authContext.CurrentAccount.ID, FilesSecurityActions.Copy);
    }

    public async Task<bool> CanMoveToAsync<T>(FileEntry<T> entry)
    {
        return await CanAsync(entry, _authContext.CurrentAccount.ID, FilesSecurityActions.MoveTo);
    }

    public async Task<bool> CanMoveAsync<T>(FileEntry<T> entry)
    {
        return await CanAsync(entry, _authContext.CurrentAccount.ID, FilesSecurityActions.Move);
    }

    public async Task<bool> CanPinAsync<T>(FileEntry<T> entry)
    {
        return await CanAsync(entry, _authContext.CurrentAccount.ID, FilesSecurityActions.Pin);
    }

    public async Task<bool> CanEditAccessAsync<T>(FileEntry<T> entry)
    {
        return await CanAsync(entry, _authContext.CurrentAccount.ID, FilesSecurityActions.EditAccess);
    }

    public async Task<bool> CanEditHistoryAsync<T>(FileEntry<T> entry)
    {
        return await CanAsync(entry, _authContext.CurrentAccount.ID, FilesSecurityActions.EditHistory);
    }

    public Task<bool> CanConvert<T>(FileEntry<T> entry)
    {
        return CanAsync(entry, _authContext.CurrentAccount.ID, FilesSecurityActions.Convert);
    }
    
    public async Task<IEnumerable<Guid>> WhoCanReadAsync<T>(FileEntry<T> entry)
    {
        return await WhoCanAsync(entry, FilesSecurityActions.Read);
    }

    private async Task<IEnumerable<Guid>> WhoCanAsync<T>(FileEntry<T> entry, FilesSecurityActions action)
    {
        var shares = await GetSharesAsync(entry);
        var copyShares = shares.GroupBy(k => k.Subject).ToDictionary(k => k.Key);

        FileShareRecord[] defaultRecords;

        switch (entry.RootFolderType)
        {
            case FolderType.COMMON:
                defaultRecords = new[]
                {
                    new FileShareRecord
                    {
                        Level = int.MaxValue,
                        EntryId = entry.Id,
                        EntryType = entry.FileEntryType,
                        Share = DefaultCommonShare,
                        Subject = Constants.GroupEveryone.ID,
                        TenantId = await _tenantManager.GetCurrentTenantIdAsync(),
                        Owner = _authContext.CurrentAccount.ID
                    }
                };

                if (!shares.Any())
                {
                    var defaultShareRecord = defaultRecords.FirstOrDefault();

                    if ((defaultShareRecord.Share == FileShare.Read && action == FilesSecurityActions.Read) ||
                        (defaultShareRecord.Share == FileShare.ReadWrite))
                    {
                        return (await _userManager.GetUsersByGroupAsync(defaultShareRecord.Subject))
                                          .Where(x => x.Status == EmployeeStatus.Active).Select(y => y.Id).Distinct();
                    }

                    return Enumerable.Empty<Guid>();
                }

                break;

            case FolderType.USER:
                defaultRecords = new[]
                {
                    new FileShareRecord
                    {
                        Level = int.MaxValue,
                        EntryId = entry.Id,
                        EntryType = entry.FileEntryType,
                        Share = DefaultMyShare,
                        Subject = entry.RootCreateBy,
                        TenantId = await _tenantManager.GetCurrentTenantIdAsync(),
                        Owner = entry.RootCreateBy
                    }
                };

                if (!shares.Any())
                {
                    return new List<Guid>
                    {
                        entry.RootCreateBy
                    };
                }
                break;

            case FolderType.Privacy:
                defaultRecords = new[]
                {
                    new FileShareRecord
                    {
                        Level = int.MaxValue,
                        EntryId = entry.Id,
                        EntryType = entry.FileEntryType,
                        Share = DefaultPrivacyShare,
                        Subject = entry.RootCreateBy,
                        TenantId = await _tenantManager.GetCurrentTenantIdAsync(),
                        Owner = entry.RootCreateBy
                    }
                };

                if (!shares.Any())
                {
                    return new List<Guid>
                    {
                        entry.RootCreateBy
                    };
                }
                break;

            case FolderType.BUNCH:
                if (action == FilesSecurityActions.Read)
                {
                    var folderDao = _daoFactory.GetFolderDao<T>();
                    var root = await folderDao.GetFolderAsync(entry.RootId);
                    if (root != null)
                    {
                        var path = await folderDao.GetBunchObjectIDAsync(root.Id);

                        var adapter = FilesIntegration.GetFileSecurity(path);

                        if (adapter != null)
                        {
                            return await adapter.WhoCanReadAsync(entry);
                        }
                    }
                }

                // TODO: For Projects and other
                defaultRecords = null;
                break;

            case FolderType.VirtualRooms:
                defaultRecords = null;

                if (entry is not Folder<T> || entry is Folder<T> folder && folder.FolderType != FolderType.VirtualRooms)
                {
                    break;
                }

                defaultRecords = new[]
                {
                    new FileShareRecord
                    {
                        Level = int.MaxValue,
                        EntryId = entry.Id,
                        EntryType = entry.FileEntryType,
                        Share = FileShare.Read,
                        Subject = Constants.GroupEveryone.ID,
                        TenantId = await _tenantManager.GetCurrentTenantIdAsync(),
                        Owner = entry.RootCreateBy
                    }
                };

                if (!shares.Any())
                {
                    var users = new List<Guid>();

                    foreach (var defaultRecord in defaultRecords)
                    {
                        users.AddRange((await _userManager.GetUsersByGroupAsync(defaultRecord.Subject)).Where(x => x.Status == EmployeeStatus.Active).Select(y => y.Id));
                    }

                    return users.Distinct();
                }

                break;

            default:
                defaultRecords = null;
                break;
        }

        if (defaultRecords != null)
        {
            shares = shares.Concat(defaultRecords);
        }

        var manyShares = shares.ToAsyncEnumerable().SelectManyAwait(async x => await ToGuidAsync(x)).Distinct();

        var result = new List<Guid>();

        await foreach (var userId in manyShares)
        {
            var userSubjects = await GetUserSubjectsAsync(userId);
            var userShares = new List<FileShareRecord>();

            foreach (var subject in userSubjects)
            {
                if (copyShares.TryGetValue(subject, out var value))
                {
                    userShares.AddRange(value);
                }
            }
            
            if (await CanAsync(entry, userId, action, userShares))
            {
                result.Add(userId);
            }
        }

        return result;
    }

    private async ValueTask<IAsyncEnumerable<Guid>> ToGuidAsync(FileShareRecord x)
    {
        if (x.SubjectType != SubjectType.Group)
        {
            return new[] { x.Subject }.ToAsyncEnumerable();
        }

        var groupInfo = await _userManager.GetGroupInfoAsync(x.Subject);

        if (groupInfo.ID != Constants.LostGroupInfo.ID)
        {
            return (await _userManager.GetUsersByGroupAsync(groupInfo.ID))
                .Where(p => p.Status == EmployeeStatus.Active)
                .Select(y => y.Id).ToAsyncEnumerable();
        }

        return new[] { x.Subject }.ToAsyncEnumerable();
    }

    public async IAsyncEnumerable<FileEntry<T>> FilterReadAsync<T>(IAsyncEnumerable<FileEntry<T>> entries)
    {
        await foreach (var e in CanReadAsync(entries.Where(f => f != null)))
        {
            if (e.Item2)
            {
                yield return e.Item1;
            }
        }
    }

    public IAsyncEnumerable<FileEntry<T>> SetSecurity<T>(IAsyncEnumerable<FileEntry<T>> entries)
    {
        return SetSecurity(entries, _authContext.CurrentAccount.ID);
    }

    public async IAsyncEnumerable<FileEntry<T>> SetSecurity<T>(IAsyncEnumerable<FileEntry<T>> entries, Guid userId)
    {
        var user = await _userManager.GetUsersAsync(userId);
        var isOutsider = await _userManager.IsOutsiderAsync(user);
        var isUser = await _userManager.IsUserAsync(user);
        var isAuthenticated = (await _authManager.GetAccountByIDAsync(await _tenantManager.GetCurrentTenantIdAsync(), userId)).IsAuthenticated;
        var isDocSpaceAdmin = await _fileSecurityCommon.IsDocSpaceAdministratorAsync(userId);
        var isCollaborator = await _userManager.IsCollaboratorAsync(user);

        await foreach (var entry in entries)
        {
            if (entry.Security != null)
            {
                yield return entry;
            }

            var tasks = Enum.GetValues<FilesSecurityActions>()
                .Where(r => _securityEntries[entry.FileEntryType].Contains(r))
                .Select(async e =>
                {
                    var t = await FilterEntry(entry, e, userId, null, isOutsider, isUser, isAuthenticated, isDocSpaceAdmin, isCollaborator);
                    return new KeyValuePair<FilesSecurityActions, bool>(e, t);
                });

            entry.Security = (await Task.WhenAll(tasks)).ToDictionary(a => a.Key, b => b.Value);

            yield return entry;
        }
    }

    private async Task<bool> CanAsync<T>(FileEntry<T> entry, Guid userId, FilesSecurityActions action, IEnumerable<FileShareRecord> shares = null)
    {
        if (entry.Security != null && entry.Security.TryGetValue(action, out var result))
        {
            return result;
        }

        var user = await _userManager.GetUsersAsync(userId);
        var isOutsider = await _userManager.IsOutsiderAsync(user);

        if (isOutsider && action != FilesSecurityActions.Read)
        {
            return false;
        }

        var isUser = await _userManager.IsUserAsync(user);
        var isAuthenticated = (await _authManager.GetAccountByIDAsync(await _tenantManager.GetCurrentTenantIdAsync(), userId)).IsAuthenticated;
        var isDocSpaceAdmin = await _fileSecurityCommon.IsDocSpaceAdministratorAsync(userId);
        var isCollaborator = await _userManager.IsCollaboratorAsync(user);

        return await FilterEntry(entry, action, userId, shares, isOutsider, isUser, isAuthenticated, isDocSpaceAdmin, isCollaborator);
    }

    public IAsyncEnumerable<FileEntry<T>> FilterDownloadAsync<T>(IAsyncEnumerable<FileEntry<T>> entries)
    {
        return FilterReadAsync(entries).Where(CheckDenyDownload);
    }

    public static FileShare GetHighFreeRole(FolderType folderType)
    {
        return folderType switch
        {
            FolderType.CustomRoom => FileShare.Editing,
            FolderType.FillingFormsRoom => FileShare.FillForms,
            FolderType.EditingRoom => FileShare.Editing,
            FolderType.ReviewRoom => FileShare.Review,
            FolderType.ReadOnlyRoom => FileShare.Read,
            _ => FileShare.None
        };
    }

    public static EmployeeType GetTypeByShare(FileShare share)
    {
        return share switch
        {
            FileShare.RoomAdmin => EmployeeType.RoomAdmin,
            FileShare.Collaborator => EmployeeType.Collaborator,
            _ => EmployeeType.User,
        };
    }

    private bool CheckDenyDownload<T>(FileEntry<T> entry)
    {
        return entry.DenyDownload
            ? entry.Access != FileShare.Read && entry.Access != FileShare.Comment
            : true;
    }

    private bool CheckDenySharing<T>(FileEntry<T> entry)
    {
        return entry.DenySharing
            ? entry.Access != FileShare.ReadWrite
            : true;
    }

    private async IAsyncEnumerable<Tuple<FileEntry<T>, bool>> CanAsync<T>(IAsyncEnumerable<FileEntry<T>> entry, Guid userId, FilesSecurityActions action)
    {
        await foreach (var r in SetSecurity(entry, userId))
        {
            if (r.Security != null && r.Security.ContainsKey(action))
            {
                yield return new Tuple<FileEntry<T>, bool>(r, r.Security[action]);
            }
            else
            {
                yield return new Tuple<FileEntry<T>, bool>(r, await CanAsync(r, userId, action));
            }
        }
    }

    private async Task<bool> FilterEntry<T>(FileEntry<T> e, FilesSecurityActions action, Guid userId, IEnumerable<FileShareRecord> shares, bool isOutsider, bool isUser, bool isAuthenticated, bool isDocSpaceAdmin, bool isCollaborator)
    {
        if (!isAuthenticated && action is not (FilesSecurityActions.Read or FilesSecurityActions.Download))
        {
            return false;
        }

        var file = e as File<T>;
        var folder = e as Folder<T>;
        var isRoom = folder != null && DocSpaceHelper.IsRoom(folder.FolderType);

        if (file != null &&
            action == FilesSecurityActions.Edit &&
            _fileUtility.CanWebRestrictedEditing(file.Title))
        {
            return false;
        }

        if ((action == FilesSecurityActions.ReadHistory ||
             action == FilesSecurityActions.EditHistory) &&
             e.ProviderEntry)
        {
            return false;
        }

        if (action == FilesSecurityActions.SubmitToFormGallery &&
            file is not { FilterType: FilterType.OFormTemplateOnly })
        {
            return false;
        }

        if (e.FileEntryType == FileEntryType.Folder)
        {
            if (folder == null)
            {
                return false;
            }

            if (action != FilesSecurityActions.Read)
            {
                if ((action == FilesSecurityActions.Pin ||
                     action == FilesSecurityActions.EditAccess ||
                     action == FilesSecurityActions.Mute) &&
                    !isRoom)
                {
                    return false;
                }

                if (action == FilesSecurityActions.Mute && isRoom && await IsAllGeneralNotificationSettingsOffAsync())
                {
                    return false;
                }

                if (action == FilesSecurityActions.CopySharedLink && folder.FolderType is not (FolderType.CustomRoom or FolderType.PublicRoom))
                {
                    return false;
                }

                if (action is FilesSecurityActions.Copy or FilesSecurityActions.Duplicate && isRoom)
                {
                    return false;
                }

                if (!isUser)
                {
                    if (folder.FolderType == FolderType.USER)
                    {
                        return folder.CreateBy == userId && (
                            action == FilesSecurityActions.Create ||
                            action == FilesSecurityActions.CopyTo ||
                            action == FilesSecurityActions.MoveTo ||
                            action == FilesSecurityActions.FillForms);
                    }

                    if (folder.FolderType == FolderType.Archive)
                    {
                        return action == FilesSecurityActions.MoveTo;
                    }

                    if (folder.FolderType == FolderType.VirtualRooms && !isCollaborator)
                    {
                        return action == FilesSecurityActions.Create ||
                            action == FilesSecurityActions.MoveTo;
                    }

                    if (folder.FolderType == FolderType.TRASH)
                    {
                        return action == FilesSecurityActions.MoveTo;
                    }
                }
            }
            else if (isAuthenticated)
            {
                if (folder.FolderType == FolderType.VirtualRooms)
                {
                    return true;
                }

                if (folder.FolderType == FolderType.Archive)
                {
                    return true;
                }
            }
        }

        switch (e.RootFolderType)
        {
            case FolderType.DEFAULT:
                if (isDocSpaceAdmin)
                {
                    // administrator can work with crashed entries (crash in files_folder_tree)
                    return true;
                }
                break;
            case FolderType.TRASH:
                if (action != FilesSecurityActions.Read && action != FilesSecurityActions.Delete && action != FilesSecurityActions.Move)
                {
                    return false;
                }

                var mytrashId = await _globalFolder.GetFolderTrashAsync(_daoFactory);
                if (!Equals(mytrashId, 0))
                {
                    return Equals(e.RootId, mytrashId) && (folder == null || action != FilesSecurityActions.Delete || !Equals(e.Id, mytrashId));
                }
                break;
            case FolderType.USER:
                if (isOutsider || isUser || action == FilesSecurityActions.Lock)
                {
                    return false;
                }
                if (e.RootCreateBy == userId)
                {
                    // user has all right in his folder
                    return true;
                }
                break;
            case FolderType.VirtualRooms:
                if (action == FilesSecurityActions.Delete && isRoom)
                {
                    return false;
                }

                if (await HasFullAccessAsync(e, userId, isUser, isDocSpaceAdmin, isRoom, isCollaborator))
                {
                    return true;
                }
                break;
            case FolderType.Archive:
                if (action != FilesSecurityActions.Read &&
                    action != FilesSecurityActions.Delete &&
                    action != FilesSecurityActions.ReadHistory &&
                    action != FilesSecurityActions.Copy &&
                    action != FilesSecurityActions.Move
                    )
                {
                    return false;
                }

                if ((action == FilesSecurityActions.Delete ||
                    action == FilesSecurityActions.Move) &&
                    !isRoom)
                {
                    return false;
                }

                if (await HasFullAccessAsync(e, userId, isUser, isDocSpaceAdmin, isRoom, isCollaborator))
                {
                    return true;
                }
                break;
            case FolderType.ThirdpartyBackup:
                if (isDocSpaceAdmin)
                {
                    return true;
                }
                break;
        }

        FileShareRecord ace;

        if (e.RootFolderType is FolderType.VirtualRooms or FolderType.Archive && 
            (_cachedRecords.TryGetValue(GetCacheKey(e.ParentId, userId, isRoom), out var value)) || 
            _cachedRecords.TryGetValue(GetCacheKey(e.ParentId, await _externalShare.GetLinkIdAsync(), isRoom), out value))
        {
            ace = value.Clone();
            ace.EntryId = e.Id;
        }
        else
        {
            var subjects = new List<Guid>();
            if (shares == null)
            {
                subjects = await GetUserSubjectsAsync(userId);
                shares = await GetSharesAsync(e, subjects);
            }

            if (e.FileEntryType == FileEntryType.File)
            {
                ace = shares
                    .OrderBy(r => r, new SubjectComparer(subjects))
                    .ThenByDescending(r => r.Share, new FileShareRecord.ShareComparer())
                    .FirstOrDefault(r => Equals(r.EntryId, e.Id) && r.EntryType == FileEntryType.File);

                if (ace == null)
                {
                    // share on parent folders
                    ace = shares.Where(r => Equals(r.EntryId, file.ParentId) && r.EntryType == FileEntryType.Folder)
                        .OrderBy(r => r, new SubjectComparer(subjects))
                        .ThenBy(r => r.Level)
                        .FirstOrDefault();
                }
            }
            else
            {
                ace = shares.Where(r => Equals(r.EntryId, e.Id) && r.EntryType == FileEntryType.Folder)
                    .OrderBy(r => r, new SubjectComparer(subjects))
                    .ThenBy(r => r.Level)
                    .ThenByDescending(r => r.Share, new FileShareRecord.ShareComparer())
                    .FirstOrDefault();
            }
            
            if (e.RootFolderType is FolderType.VirtualRooms or FolderType.Archive && 
                ace is { SubjectType: SubjectType.User or SubjectType.ExternalLink or SubjectType.PrimaryExternalLink })
            {
                var id = ace.SubjectType is SubjectType.ExternalLink or SubjectType.PrimaryExternalLink ? ace.Subject : userId;

                _cachedRecords.TryAdd(GetCacheKey(e.ParentId, id, isRoom), ace);
            }
        }

        if (ace is { SubjectType: SubjectType.ExternalLink or SubjectType.PrimaryExternalLink } && ace.Subject != userId && 
            await _externalShare.ValidateRecordAsync(ace, null) != Status.Ok)
        {
            return false;
        }

        var defaultShare =
            userId == FileConstant.ShareLinkId ? FileShare.Restrict :
            e.RootFolderType == FolderType.VirtualRooms ? DefaultVirtualRoomsShare :
            e.RootFolderType == FolderType.USER ? DefaultMyShare :
            e.RootFolderType == FolderType.Privacy ? DefaultPrivacyShare :
            e.RootFolderType == FolderType.Archive ? DefaultArchiveShare :
            DefaultCommonShare;

        e.Access = ace?.Share ?? defaultShare;
        e.Access = e.RootFolderType is FolderType.ThirdpartyBackup ? FileShare.Restrict : e.Access;

        switch (action)
        {
            case FilesSecurityActions.Read:
            case FilesSecurityActions.Pin:
            case FilesSecurityActions.Mute:
                return e.Access != FileShare.Restrict;
            case FilesSecurityActions.Comment:
                if (e.Access is FileShare.Comment || 
                    e.Access == FileShare.Review ||
                    e.Access == FileShare.CustomFilter ||
                    e.Access == FileShare.ReadWrite ||
                    e.Access == FileShare.RoomAdmin ||
                    e.Access == FileShare.Editing ||
                    e.Access == FileShare.FillForms ||
                    e.Access == FileShare.Collaborator)
                {
                    return true;
                }
                break;
            case FilesSecurityActions.FillForms:
                if (e.Access == FileShare.FillForms ||
                    e.Access == FileShare.ReadWrite ||
                    e.Access == FileShare.RoomAdmin ||
                    e.Access == FileShare.Editing ||
                    e.Access == FileShare.Collaborator)
                {
                    return true;
                }
                break;
            case FilesSecurityActions.Review:
                if (e.Access == FileShare.Review ||
                    e.Access == FileShare.ReadWrite ||
                    e.Access == FileShare.RoomAdmin ||
                    e.Access == FileShare.Editing ||
                    e.Access == FileShare.Collaborator)
                {
                    return true;
                }
                break;
            case FilesSecurityActions.Convert:
            case FilesSecurityActions.Create:
                if (e.Access == FileShare.ReadWrite ||
                    e.Access == FileShare.RoomAdmin ||
                    e.Access == FileShare.Collaborator)
                {
                    return true;
                }
                break;
            case FilesSecurityActions.Edit:
                if (e.Access == FileShare.ReadWrite ||
                    e.Access == FileShare.RoomAdmin ||
                    e.Access == FileShare.Editing ||
                    e.Access == FileShare.Collaborator)
                {
                    return true;
                }
                break;
            case FilesSecurityActions.Delete:
                if (e.Access == FileShare.RoomAdmin ||
                    (e.Access == FileShare.Collaborator && e.CreateBy == _authContext.CurrentAccount.ID))
                {
                    if (file is { RootFolderType: FolderType.VirtualRooms })
                    {
                        return true;
                    }

                    if (folder is { RootFolderType: FolderType.VirtualRooms, FolderType: FolderType.DEFAULT })
                    {
                        return true;
                    }
                }
                break;
            case FilesSecurityActions.CustomFilter:
                if (e.Access == FileShare.CustomFilter ||
                    e.Access == FileShare.ReadWrite ||
                    e.Access == FileShare.RoomAdmin ||
                    e.Access == FileShare.Editing ||
                    e.Access == FileShare.Collaborator)
                {
                    return true;
                }
                break;
            case FilesSecurityActions.EditRoom:
                if (e.Access == FileShare.RoomAdmin)
                {
                    return true;
                }
                break;
            case FilesSecurityActions.Rename:
                if (e.Access == FileShare.ReadWrite ||
                    e.Access == FileShare.RoomAdmin ||
                    (e.Access == FileShare.Collaborator && e.CreateBy == _authContext.CurrentAccount.ID))
                {
                    return true;
                }
                break;
            case FilesSecurityActions.ReadHistory:
                if (e.Access == FileShare.RoomAdmin ||
                    e.Access == FileShare.Editing ||
                    e.Access == FileShare.Collaborator)
                {
                    return true;
                }
                break;
            case FilesSecurityActions.Lock:
                if (e.Access == FileShare.RoomAdmin ||
                    e.Access == FileShare.Collaborator)
                {
                    return true;
                }
                break;
            case FilesSecurityActions.EditHistory:
                if (e.Access == FileShare.ReadWrite ||
                    e.Access == FileShare.RoomAdmin ||
                    e.Access == FileShare.Collaborator)
                {
                    return file != null && !file.Encrypted;
                }
                break;
            case FilesSecurityActions.CopyTo:
            case FilesSecurityActions.MoveTo:
                if (e.Access == FileShare.RoomAdmin ||
                    e.Access == FileShare.Collaborator)
                {
                    return true;
                }
                break;
            case FilesSecurityActions.Copy:
            case FilesSecurityActions.Duplicate:
                if (e.Access == FileShare.RoomAdmin ||
                    (e.Access == FileShare.Collaborator && e.CreateBy == _authContext.CurrentAccount.ID))
                {
                    return true;
                }
                break;
            case FilesSecurityActions.EditAccess:
                if (e.Access == FileShare.RoomAdmin)
                {
                    return true;
                }
                break;
            case FilesSecurityActions.Move:
                if ((e.Access == FileShare.RoomAdmin ||
                     e.Access == FileShare.Collaborator && e.CreateBy == _authContext.CurrentAccount.ID)
                    && !isRoom)
                {
                    return true;
                } 
                break;
            case FilesSecurityActions.SubmitToFormGallery:
                if ((e.Access == FileShare.RoomAdmin ||
                     e.Access == FileShare.Collaborator) && 
                    file is { FilterType: FilterType.OFormTemplateOnly })
                {
                    return true;
                }
                break;
            case FilesSecurityActions.Download:
                if (e.Access != FileShare.Restrict && ace?.Options is not { DenyDownload: true })
                {
                    return true;
                }
                break;
            case FilesSecurityActions.CopySharedLink:
                if (e.Access == FileShare.RoomAdmin || (e.Access != FileShare.Restrict && e.Shared))
                {
                    return true;
                }
                break;
        }

        if (e.Access != FileShare.Restrict &&
            e.CreateBy == userId &&
            (e.FileEntryType == FileEntryType.File || folder.FolderType != FolderType.COMMON) &&
            e.RootFolderType != FolderType.Archive && e.RootFolderType != FolderType.VirtualRooms)
        {
            return true;
        }

        if (e.CreateBy == userId)
        {
            e.Access = FileShare.None; //HACK: for client
        }


        return false;
    }

    public async Task ShareAsync<T>(T entryId, FileEntryType entryType, Guid @for, FileShare share, SubjectType subjectType = default, FileShareOptions options = null)
    {
        var securityDao = _daoFactory.GetSecurityDao<T>();
        var r = new FileShareRecord
        {
            TenantId = await _tenantManager.GetCurrentTenantIdAsync(),
            EntryId = entryId,
            EntryType = entryType,
            Subject = @for,
            Owner = _authContext.CurrentAccount.ID,
            Share = share,
            SubjectType = subjectType,
            Options = options,
        };

        await securityDao.SetShareAsync(r);
    }

    public async Task<IEnumerable<FileShareRecord>> GetSharesAsync<T>(FileEntry<T> entry, IEnumerable<Guid> subjects = null)
    {
        return await _daoFactory.GetSecurityDao<T>().GetSharesAsync(entry, subjects);
    }

    public IAsyncEnumerable<FileShareRecord> GetPureSharesAsync<T>(FileEntry<T> entry, IEnumerable<Guid> subjects)
    {
        return _daoFactory.GetSecurityDao<T>().GetPureSharesAsync(entry, subjects);
    }

    public IAsyncEnumerable<FileShareRecord> GetPureSharesAsync<T>(FileEntry<T> entry, ShareFilterType filterType, EmployeeActivationStatus? status, int offset = 0, int count = -1)
    {
        return _daoFactory.GetSecurityDao<T>().GetPureSharesAsync(entry, filterType, status, offset, count);
    }

    public Task<int> GetPureSharesCountAsync<T>(FileEntry<T> entry, ShareFilterType filterType, EmployeeActivationStatus? status)
    {
        return _daoFactory.GetSecurityDao<T>().GetPureSharesCountAsync(entry, filterType, status);
    }

    public IAsyncEnumerable<UserInfoWithShared> GetUsersWithSharedAsync<T>(FileEntry<T> entry, string text, EmployeeStatus? employeeStatus, EmployeeActivationStatus? activationStatus, 
        bool excludeShared, int offset, int count)
    {
        return _daoFactory.GetSecurityDao<T>().GetUsersWithSharedAsync(entry, text, employeeStatus, activationStatus, excludeShared, offset, count);
    }

    public async Task<int> GetUsersWithSharedCountAsync<T>(FileEntry<T> entry, string text, EmployeeStatus? employeeStatus, EmployeeActivationStatus? activationStatus, 
        bool excludeShared)
    {
        return await _daoFactory.GetSecurityDao<T>().GetUsersWithSharedCountAsync(entry, text, employeeStatus, activationStatus, excludeShared);
    }

    public async IAsyncEnumerable<FileEntry> GetSharesForMeAsync(FilterType filterType, bool subjectGroup, Guid subjectID, string searchText = "", bool searchInContent = false, bool withSubfolders = false)
    {
        var securityDao = _daoFactory.GetSecurityDao<int>();
        var subjects = await GetUserSubjectsAsync(_authContext.CurrentAccount.ID);
        var records = await securityDao.GetSharesAsync(subjects).ToListAsync();

        var firstTask = GetSharesForMeAsync<int>(records.Where(r => r.EntryId is int), subjects, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders).ToListAsync();
        var secondTask = GetSharesForMeAsync<string>(records.Where(r => r.EntryId is string), subjects, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders).ToListAsync();

        foreach (var items in await Task.WhenAll(firstTask.AsTask(), secondTask.AsTask()))
        {
            foreach (var item in items)
            {
                yield return item;
            }
        }
    }

    public async Task<List<FileEntry>> GetVirtualRoomsAsync(FilterType filterType, Guid subjectId, string searchText, bool searchInContent, bool withSubfolders,
        SearchArea searchArea, bool withoutTags, IEnumerable<string> tagNames, bool excludeSubject, ProviderFilter provider, SubjectFilter subjectFilter)
    {
        var securityDao = _daoFactory.GetSecurityDao<int>();

        var subjectEntries = subjectFilter is SubjectFilter.Member
                ? await securityDao.GetSharesAsync(new[] { subjectId }).Where(r => r.EntryType == FileEntryType.Folder).Select(r => r.EntryId.ToString()).ToListAsync()
                : null;

        if (await _fileSecurityCommon.IsDocSpaceAdministratorAsync(_authContext.CurrentAccount.ID))
        {
            return await GetAllVirtualRoomsAsync(filterType, subjectId, searchText, searchInContent, withSubfolders, searchArea, withoutTags, tagNames, excludeSubject, provider, subjectFilter, subjectEntries);
        }

        return await GetVirtualRoomsForMeAsync(filterType, subjectId, searchText, searchInContent, withSubfolders, searchArea, withoutTags, tagNames, excludeSubject, provider, subjectFilter, subjectEntries);
    }

    private async Task<List<FileEntry>> GetAllVirtualRoomsAsync(FilterType filterType, Guid subjectId, string search, bool searchInContent, bool withSubfolders,
        SearchArea searchArea, bool withoutTags, IEnumerable<string> tagNames, bool excludeSubject, ProviderFilter provider, SubjectFilter subjectFilter,
        IEnumerable<string> subjectEntries)
    {
        var folderDao = _daoFactory.GetFolderDao<int>();
        var folderThirdPartyDao = _daoFactory.GetFolderDao<string>();
        var fileDao = _daoFactory.GetFileDao<int>();
        var fileThirdPartyDao = _daoFactory.GetFileDao<string>();
        var entries = new List<FileEntry>();

        var rootFoldersIds = searchArea switch
        {
            SearchArea.Active => new[] { await _globalFolder.GetFolderVirtualRoomsAsync(_daoFactory) },
            SearchArea.Archive => new[] { await _globalFolder.GetFolderArchiveAsync(_daoFactory) },
            _ => new[] { await _globalFolder.GetFolderVirtualRoomsAsync(_daoFactory), await _globalFolder.GetFolderArchiveAsync(_daoFactory) }
        };

        var roomsEntries = await folderDao.GetRoomsAsync(rootFoldersIds, filterType, tagNames, subjectId, search, withSubfolders, withoutTags, excludeSubject,
            provider, subjectFilter, subjectEntries).ToListAsync();
        var thirdPartyRoomsEntries = await folderThirdPartyDao.GetFakeRoomsAsync(rootFoldersIds.Select(id => id.ToString()), filterType, tagNames, subjectId, search,
            withSubfolders, withoutTags, excludeSubject, provider, subjectFilter, subjectEntries).ToListAsync();

        entries.AddRange(roomsEntries);
        entries.AddRange(thirdPartyRoomsEntries);

        if (withSubfolders && filterType != FilterType.FoldersOnly)
        {
            List<File<int>> files;
            List<File<string>> thirdPartyFiles;

            if (!string.IsNullOrEmpty(search))
            {
                files = await fileDao.GetFilesAsync(roomsEntries.Select(r => r.Id), FilterType.None, false, Guid.Empty, search, searchInContent).ToListAsync();
                thirdPartyFiles = await fileThirdPartyDao.GetFilesAsync(thirdPartyRoomsEntries.Select(f => f.Id), FilterType.None, false, Guid.Empty, search, searchInContent).ToListAsync();
            }
            else
            {
                files = await fileDao.GetFilesAsync(roomsEntries.Where(r => DocSpaceHelper.IsRoom(r.FolderType)).Select(r => r.Id), FilterType.None, false, Guid.Empty, search, searchInContent).ToListAsync();
                thirdPartyFiles = await fileThirdPartyDao.GetFilesAsync(thirdPartyRoomsEntries.Select(r => r.Id), FilterType.None, false, Guid.Empty, search, searchInContent).ToListAsync();
            }

            entries.AddRange(files);
            entries.AddRange(thirdPartyFiles);
        }

        await SetTagsAsync(roomsEntries);
        await SetTagsAsync(thirdPartyRoomsEntries);
        await SetPinAsync(roomsEntries);
        await SetPinAsync(thirdPartyRoomsEntries);

        return entries;
    }

    private async Task<List<FileEntry>> GetVirtualRoomsForMeAsync(FilterType filterType, Guid subjectId, string search, bool searchInContent, bool withSubfolders,
        SearchArea searchArea, bool withoutTags, IEnumerable<string> tagNames, bool excludeSubject, ProviderFilter provider, SubjectFilter subjectFilter, IEnumerable<string> subjectEntries)
    {
        var folderDao = _daoFactory.GetFolderDao<int>();
        var folderThirdPartyDao = _daoFactory.GetFolderDao<string>();
        var fileDao = _daoFactory.GetFileDao<int>();
        var thirdPartyFileDao = _daoFactory.GetFileDao<string>();
        var securityDao = _daoFactory.GetSecurityDao<int>();

        var entries = new List<FileEntry>();

        var currentUserSubjects = await GetUserSubjectsAsync(_authContext.CurrentAccount.ID);
        var currentUsersRecords = await securityDao.GetSharesAsync(currentUserSubjects).ToListAsync();

        var roomsIds = new Dictionary<int, FileShare>();
        var thirdPartyRoomsIds = new Dictionary<string, FileShare>();

        var recordGroup = currentUsersRecords.GroupBy(r => new { r.EntryId, r.EntryType }, (key, group) => new
        {
            firstRecord = group.OrderBy(r => r, new SubjectComparer(currentUserSubjects))
            .ThenByDescending(r => r.Share, new FileShareRecord.ShareComparer())
            .First()
        });

        foreach (var record in recordGroup.Where(r => r.firstRecord.Share != FileShare.Restrict))
        {
            if (record.firstRecord.EntryType != FileEntryType.Folder)
            {
                continue;
            }

            switch (record.firstRecord.EntryId)
            {
                case int roomId when !roomsIds.ContainsKey(roomId):
                    roomsIds.Add(roomId, record.firstRecord.Share);
                    break;
                case string thirdPartyRoomId when !thirdPartyRoomsIds.ContainsKey(thirdPartyRoomId):
                    thirdPartyRoomsIds.Add(thirdPartyRoomId, record.firstRecord.Share);
                    break;
            }
        }

        var rootFoldersIds = searchArea switch
        {
            SearchArea.Active => new[] { await _globalFolder.GetFolderVirtualRoomsAsync(_daoFactory) },
            SearchArea.Archive => new[] { await _globalFolder.GetFolderArchiveAsync(_daoFactory) },
            _ => new[] { await _globalFolder.GetFolderVirtualRoomsAsync(_daoFactory), await _globalFolder.GetFolderArchiveAsync(_daoFactory) }
        };

        var rooms = await folderDao.GetRoomsAsync(roomsIds.Keys, filterType, tagNames, subjectId, search, withSubfolders, withoutTags, excludeSubject, provider, subjectFilter, subjectEntries, rootFoldersIds)
             .Where(r => Filter(r, roomsIds)).ToListAsync();
        var thirdPartyRooms = await folderThirdPartyDao.GetFakeRoomsAsync(rootFoldersIds.Select(id => id.ToString()), thirdPartyRoomsIds.Keys, filterType,
                tagNames, subjectId, search, withSubfolders, withoutTags, excludeSubject, provider, subjectFilter, subjectEntries)
            .Where(r => Filter(r, thirdPartyRoomsIds)).ToListAsync();

        if (withSubfolders && filterType != FilterType.FoldersOnly)
        {
            List<File<int>> files;
            List<File<string>> thirdPartyFiles;

            if (!string.IsNullOrEmpty(search))
            {
                files = await fileDao.GetFilesAsync(rooms.Select(r => r.Id), FilterType.None, false, Guid.Empty, search, searchInContent).ToListAsync();
                thirdPartyFiles = await thirdPartyFileDao.GetFilesAsync(thirdPartyRooms.Select(r => r.Id), FilterType.None, false, Guid.Empty, search, searchInContent).ToListAsync();
            }
            else
            {
                files = await fileDao.GetFilesAsync(rooms.Select(r => r.Id), FilterType.None, false, Guid.Empty, search, searchInContent).ToListAsync();
                thirdPartyFiles = await thirdPartyFileDao.GetFilesAsync(thirdPartyRooms.Select(r => r.Id), FilterType.None, false, Guid.Empty, search, searchInContent).ToListAsync();
            }

            entries.AddRange(files.Where(f => Filter(f, roomsIds)));
            entries.AddRange(thirdPartyFiles.Where(f => Filter(f, thirdPartyRoomsIds)));
        }

        await SetTagsAsync(rooms);
        await SetTagsAsync(thirdPartyRooms);
        await SetPinAsync(rooms);
        await SetPinAsync(thirdPartyRooms);

        entries.AddRange(rooms);
        entries.AddRange(thirdPartyRooms);

        bool Filter<T>(FileEntry<T> entry, IReadOnlyDictionary<T, FileShare> accesses)
        {
            var id = entry.FileEntryType == FileEntryType.Folder ? entry.Id : entry.ParentId;

            switch (searchArea)
            {
                case SearchArea.Archive when entry.RootFolderType == FolderType.Archive:
                    {
                        entry.Access = accesses.TryGetValue(id, out var share) ? share : FileShare.None;
                        return true;
                    }
                case SearchArea.Active when entry.RootFolderType == FolderType.VirtualRooms:
                    {
                        entry.Access = accesses.TryGetValue(id, out var share) ? share : FileShare.None;
                        return true;
                    }
                case SearchArea.Any when (entry.RootFolderType == FolderType.VirtualRooms || entry.RootFolderType == FolderType.Archive):
                    {
                        entry.Access = accesses.TryGetValue(id, out var share) ? share : FileShare.None;
                        return true;
                    }
                default:
                    return false;
            }
        }

        return entries;
    }

    private async Task SetTagsAsync<T>(IEnumerable<FileEntry<T>> entries)
    {
        if (!entries.Any())
        {
            return;
        }

        var tagDao = _daoFactory.GetTagDao<T>();

        var tags = await tagDao.GetTagsAsync(TagType.Custom, entries).ToLookupAsync(f => (T)f.EntryId);

        foreach (var room in entries)
        {
            room.Tags = tags[room.Id];
        }
    }

    private async Task SetPinAsync<T>(IEnumerable<FileEntry<T>> entries)
    {
        if (!entries.Any())
        {
            return;
        }

        var tagDao = _daoFactory.GetTagDao<T>();

        var tags = await tagDao.GetTagsAsync(_authContext.CurrentAccount.ID, TagType.Pin, entries).ToDictionaryAsync(t => (T)t.EntryId);

        foreach (var fileEntry in entries.Where(e => e.FileEntryType == FileEntryType.Folder))
        {
            var room = (Folder<T>)fileEntry;
            if (tags.ContainsKey(room.Id))
            {
                room.Pinned = true;
            }
        }
    }

    private async IAsyncEnumerable<FileEntry> GetSharesForMeAsync<T>(IEnumerable<FileShareRecord> records, List<Guid> subjects, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText = "", bool searchInContent = false, bool withSubfolders = false)
    {
        var folderDao = _daoFactory.GetFolderDao<T>();
        var fileDao = _daoFactory.GetFileDao<T>();
        var securityDao = _daoFactory.GetSecurityDao<T>();

        var fileIds = new Dictionary<T, FileShare>();
        var folderIds = new Dictionary<T, FileShare>();

        var recordGroup = records.GroupBy(r => new { r.EntryId, r.EntryType }, (key, group) => new
        {
            firstRecord = group.OrderBy(r => r, new SubjectComparer(subjects))
               .ThenByDescending(r => r.Share, new FileShareRecord.ShareComparer())
               .First()
        });

        foreach (var r in recordGroup.Where(r => r.firstRecord.Share != FileShare.Restrict))
        {
            if (r.firstRecord.EntryType == FileEntryType.Folder)
            {
                if (!folderIds.ContainsKey((T)r.firstRecord.EntryId))
                {
                    folderIds.Add((T)r.firstRecord.EntryId, r.firstRecord.Share);
                }
            }
            else
            {
                if (!fileIds.ContainsKey((T)r.firstRecord.EntryId))
                {
                    fileIds.Add((T)r.firstRecord.EntryId, r.firstRecord.Share);
                }
            }
        }

        var entries = new List<FileEntry<T>>();

        if (filterType != FilterType.FoldersOnly)
        {
            var files = fileDao.GetFilesFilteredAsync(fileIds.Keys.ToArray(), filterType, subjectGroup, subjectID, searchText, searchInContent);
            var share = await _globalFolder.GetFolderShareAsync<T>(_daoFactory);

            await foreach (var x in files)
            {
                if (fileIds.TryGetValue(x.Id, out var access))
                {
                    x.Access = fileIds[x.Id];
                    x.FolderIdDisplay = share;
                }

                entries.Add(x);
            }
        }

        if (filterType == FilterType.None || filterType == FilterType.FoldersOnly)
        {
            IAsyncEnumerable<FileEntry<T>> folders = folderDao.GetFoldersAsync(folderIds.Keys, filterType, subjectGroup, subjectID, searchText, withSubfolders, false);

            if (withSubfolders)
            {
                folders = FilterReadAsync(folders);
            }

            var share = await _globalFolder.GetFolderShareAsync<T>(_daoFactory);

            await foreach (var folder in folders)
            {
                if (folderIds.TryGetValue(folder.Id, out var access))
                {
                    folder.Access = folderIds[folder.Id];
                    folder.FolderIdDisplay = share;
                }

                entries.Add(folder);
            }
        }

        if (filterType != FilterType.FoldersOnly && withSubfolders)
        {
            IAsyncEnumerable<FileEntry<T>> filesInSharedFolders = fileDao.GetFilesAsync(folderIds.Keys, filterType, subjectGroup, subjectID, searchText, searchInContent);
            filesInSharedFolders = FilterReadAsync(filesInSharedFolders);
            entries.AddRange(await filesInSharedFolders.Distinct().ToListAsync());
        }

        var data = entries.Where(f =>
                                f.RootFolderType == FolderType.USER // show users files
                                && f.RootCreateBy != _authContext.CurrentAccount.ID // don't show my files
            );

        if (await _userManager.IsUserAsync(_authContext.CurrentAccount.ID))
        {
            data = data.Where(r => !r.ProviderEntry);
        }

        var failedEntries = entries.Where(x => !string.IsNullOrEmpty(x.Error));
        var failedRecords = new List<FileShareRecord>();

        foreach (var failedEntry in failedEntries)
        {
            var entryType = failedEntry.FileEntryType;

            var failedRecord = records.First(x => x.EntryId.Equals(failedEntry.Id) && x.EntryType == entryType);

            failedRecord.Share = FileShare.None;

            failedRecords.Add(failedRecord);
        }

        if (failedRecords.Count > 0)
        {
            await securityDao.DeleteShareRecordsAsync(failedRecords);
        }

        data = data.Where(x => string.IsNullOrEmpty(x.Error));

        foreach (var e in data)
        {
            yield return e;
        };
    }

    public async IAsyncEnumerable<FileEntry> GetPrivacyForMeAsync(FilterType filterType, bool subjectGroup, Guid subjectID, string searchText = "", bool searchInContent = false, bool withSubfolders = false)
    {
        var securityDao = _daoFactory.GetSecurityDao<int>();
        var subjects = await GetUserSubjectsAsync(_authContext.CurrentAccount.ID);
        var records = await securityDao.GetSharesAsync(subjects).ToListAsync();

        await foreach (var e in GetPrivacyForMeAsync<int>(records.Where(r => r.EntryId is int), subjects, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders))
        {
            yield return e;
        }

        await foreach (var e in GetPrivacyForMeAsync<string>(records.Where(r => r.EntryId is string), subjects, filterType, subjectGroup, subjectID, searchText, searchInContent, withSubfolders))
        {
            yield return e;
        }
    }

    private async IAsyncEnumerable<FileEntry<T>> GetPrivacyForMeAsync<T>(IEnumerable<FileShareRecord> records, List<Guid> subjects, FilterType filterType, bool subjectGroup, Guid subjectID, string searchText = "", bool searchInContent = false, bool withSubfolders = false)
    {
        var folderDao = _daoFactory.GetFolderDao<T>();
        var fileDao = _daoFactory.GetFileDao<T>();

        var fileIds = new Dictionary<T, FileShare>();
        var folderIds = new Dictionary<T, FileShare>();

        var recordGroup = records.GroupBy(r => new { r.EntryId, r.EntryType }, (key, group) => new
        {
            firstRecord = group.OrderBy(r => r, new SubjectComparer(subjects))
                .ThenByDescending(r => r.Share, new FileShareRecord.ShareComparer())
                .First()
        });

        foreach (var r in recordGroup.Where(r => r.firstRecord.Share != FileShare.Restrict))
        {
            if (r.firstRecord.EntryType == FileEntryType.Folder)
            {
                if (!folderIds.ContainsKey((T)r.firstRecord.EntryId))
                {
                    folderIds.Add((T)r.firstRecord.EntryId, r.firstRecord.Share);
                }
            }
            else
            {
                if (!fileIds.ContainsKey((T)r.firstRecord.EntryId))
                {
                    fileIds.Add((T)r.firstRecord.EntryId, r.firstRecord.Share);
                }
            }
        }

        var entries = new List<FileEntry<T>>();

        if (filterType != FilterType.FoldersOnly)
        {
            var files = fileDao.GetFilesFilteredAsync(fileIds.Keys.ToArray(), filterType, subjectGroup, subjectID, searchText, searchInContent);
            var privateFolder = await _globalFolder.GetFolderPrivacyAsync<T>(_daoFactory);

            await foreach (var x in files)
            {
                if (fileIds.TryGetValue(x.Id, out var access))
                {
                    x.Access = access;
                    x.FolderIdDisplay = privateFolder;
                }

                entries.Add(x);
            }
        }

        if (filterType == FilterType.None || filterType == FilterType.FoldersOnly)
        {
            IAsyncEnumerable<FileEntry<T>> folders = folderDao.GetFoldersAsync(folderIds.Keys, filterType, subjectGroup, subjectID, searchText, withSubfolders, false);

            if (withSubfolders)
            {
                folders = FilterReadAsync(folders);
            }

            var privacyFolder = await _globalFolder.GetFolderPrivacyAsync<T>(_daoFactory);

            await foreach (var folder in folders)
            {
                if (folderIds.TryGetValue(folder.Id, out var access))
                {
                    folder.Access = access;
                    folder.FolderIdDisplay = privacyFolder;
                }

                entries.Add(folder);
            }
        }

        if (filterType != FilterType.FoldersOnly && withSubfolders)
        {
            IAsyncEnumerable<FileEntry<T>> filesInSharedFolders = fileDao.GetFilesAsync(folderIds.Keys, filterType, subjectGroup, subjectID, searchText, searchInContent);
            filesInSharedFolders = FilterReadAsync(filesInSharedFolders);
            entries.AddRange(await filesInSharedFolders.Distinct().ToListAsync());
        }

        var data = entries.Where(f =>
                                f.RootFolderType == FolderType.Privacy // show users files
                                && f.RootCreateBy != _authContext.CurrentAccount.ID // don't show my files
            );

        foreach (var e in data)
        {
            yield return e;
        }
    }


    public async Task RemoveSubjectAsync<T>(Guid subject)
    {
        await _daoFactory.GetSecurityDao<T>().RemoveSubjectAsync(subject);
    }

    public async Task<List<Guid>> GetUserSubjectsAsync(Guid userId)
    {
        // priority order
        // User, Departments, admin, everyone

        var result = new List<Guid> { userId };

        var linkId = await _externalShare.GetLinkIdAsync();

        if (linkId != default)
        {
            result.Add(linkId);
        }

        if (userId == FileConstant.ShareLinkId)
        {
            return result;
        }

        result.AddRange((await _userManager.GetUserGroupsAsync(userId)).Select(g => g.ID));
        if (await _fileSecurityCommon.IsDocSpaceAdministratorAsync(userId))
        {
            result.Add(Constants.GroupAdmin.ID);
        }

        result.Add(Constants.GroupEveryone.ID);

        return result;
    }

    public static void CorrectSecurityByLockedStatus<T>(FileEntry<T> entry)
    {
        if (entry is not File<T> file || file.Security == null || file.LockedBy == null)
        {
            return;
        }

        foreach (var action in _securityEntries[FileEntryType.File])
        {
            if (action != FilesSecurityActions.Read &&
                action != FilesSecurityActions.ReadHistory &&
                action != FilesSecurityActions.Copy &&
                action != FilesSecurityActions.Duplicate &&
                action != FilesSecurityActions.Lock)
            {
                file.Security[action] = false;
            }
        }
    }

    private async Task<bool> HasFullAccessAsync<T>(FileEntry<T> entry, Guid userId, bool isUser, bool isDocSpaceAdmin, bool isRoom, bool isCollaborator)
    {
        if (isUser || isCollaborator)
        {
            return false;
        }

        if (isDocSpaceAdmin)
        {
            return true;
        }

        if (isRoom)
        {
            return entry.CreateBy == userId;
        }

        if (_cachedRoomOwner.TryGetValue(GetCacheKey(entry.ParentId), out var roomOwner))
        {
            return roomOwner == userId;
        }

        var room = await _daoFactory.GetFolderDao<T>().GetParentFoldersAsync(entry.ParentId)
            .Where(f => DocSpaceHelper.IsRoom(f.FolderType))
            .FirstOrDefaultAsync();

        if (room == null)
        {
            return false;
        }

        _cachedRoomOwner.TryAdd(GetCacheKey(entry.ParentId), room.CreateBy);

        return room.CreateBy == userId;
    }

    private async Task<bool> IsAllGeneralNotificationSettingsOffAsync()
    {
        var userId = _authContext.CurrentAccount.ID;

        if (!await _badgesSettingsHelper.GetEnabledForCurrentUserAsync()
            && !await _studioNotifyHelper.IsSubscribedToNotifyAsync(userId, Actions.RoomsActivity)
            && !await _studioNotifyHelper.IsSubscribedToNotifyAsync(userId, Actions.SendWhatsNew))
        {
            return true;
        }

        return false;
    }

    private string GetCacheKey<T>(T entryId, Guid userId, bool isRoom)
    {
        var key = $"{_tenantManager.GetCurrentTenant().Id}-{userId}-{entryId}";

        if (isRoom)
        {
            key += $"-room";
        }

        return key;
    }

    private string GetCacheKey<T>(T parentId)
    {
        return $"{_tenantManager.GetCurrentTenant().Id}-{parentId}";
    }

    private sealed class SubjectComparer : IComparer<FileShareRecord>
    {
        private readonly List<Guid> _subjects;

        public SubjectComparer(List<Guid> subjects)
        {
            _subjects = subjects;
        }

        public int Compare(FileShareRecord x, FileShareRecord y)
        {
            if (x.Subject == y.Subject)
            {
                return 0;
            }

            var index1 = _subjects.IndexOf(x.Subject);
            var index2 = _subjects.IndexOf(y.Subject);
            if (index1 == 0 || index2 == 0 // UserId
                || Constants.BuildinGroups.Any(g => g.ID == x.Subject) || Constants.BuildinGroups.Any(g => g.ID == y.Subject)) // System Groups
            {
                return index1.CompareTo(index2);
            }

            // Departments are equal.
            return 0;
        }
    }

    public enum FilesSecurityActions
    {
        Read,
        Comment,
        FillForms,
        Review,
        Create,
        Edit,
        Delete,
        CustomFilter,
        EditRoom,
        Rename,
        ReadHistory,
        Lock,
        EditHistory,
        CopyTo,
        Copy,
        MoveTo,
        Move,
        Pin,
        Mute,
        EditAccess,
        Duplicate,
        SubmitToFormGallery,
        Download,
        Convert,
        CopySharedLink
    }
}
