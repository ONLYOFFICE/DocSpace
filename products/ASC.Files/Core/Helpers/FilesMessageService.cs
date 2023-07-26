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

namespace ASC.Web.Files.Helpers;

[Scope]
public class FilesMessageService
{
    private readonly ILogger _logger;
    private readonly MessageTarget _messageTarget;
    private readonly MessageService _messageService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDaoFactory _daoFactory;
    private readonly NotifyClient _notifyClient;
    private readonly AuthContext _authContext;

    public FilesMessageService(
        ILoggerProvider options,
        MessageTarget messageTarget,
        MessageService messageService,
        IDaoFactory daoFactory,
        NotifyClient notifyClient,
        AuthContext authContext)
    {
        _logger = options.CreateLogger("ASC.Messaging");
        _messageTarget = messageTarget;
        _messageService = messageService;
        _daoFactory = daoFactory;
        _notifyClient = notifyClient;
        _authContext = authContext;
    }

    public FilesMessageService(
        ILoggerProvider options,
        MessageTarget messageTarget,
        MessageService messageService,
        IHttpContextAccessor httpContextAccessor,
        IDaoFactory daoFactory,
        NotifyClient notifyClient,
        AuthContext authContext)
        : this(options, messageTarget, messageService, daoFactory, notifyClient, authContext)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task SendAsync(IDictionary<string, StringValues> headers, MessageAction action, params string[] description)
    {
        await SendHeadersMessageAsync(headers, action, null, description);
    }

    public async Task SendAsync<T>(FileEntry<T> entry, IDictionary<string, StringValues> headers, MessageAction action, params string[] description)
    {
        await SendAsync(entry, headers, action, null, FileShare.None, Guid.Empty, description);
    }

    public async Task SendAsync<T>(FileEntry<T> entry, IDictionary<string, StringValues> headers, List<AceWrapper> aces, MessageAction action, params string[] description)
    {
        if (action == MessageAction.RoomDeleted)
        {
            var userId = _authContext.CurrentAccount.ID;
            await _notifyClient.SendRoomRemovedAsync(entry, aces, userId);
        }

        await SendAsync(entry, headers, action, null, FileShare.None, Guid.Empty, description);
    }

    public async Task SendAsync<T>(FileEntry<T> entry, IDictionary<string, StringValues> headers, string oldTitle, MessageAction action, params string[] description)
    {
        await SendAsync(entry, headers, action, oldTitle, FileShare.None, Guid.Empty, description);
    }

    public async Task SendAsync<T>(FileEntry<T> entry, IDictionary<string, StringValues> headers, MessageAction action, Guid userId, params string[] description)
    {
        await SendAsync(entry, headers, action, null, FileShare.None, userId, description);
    }

    public async Task SendAsync<T>(FileEntry<T> entry, IDictionary<string, StringValues> headers, MessageAction action, FileShare userRole, Guid userId, params string[] description)
    {
        description = description.Append(FileStorageService.GetAccessString(userRole)).ToArray();
        await SendAsync(entry, headers, action, null, userRole, userId, description);
    }

    private async Task SendAsync<T>(FileEntry<T> entry, IDictionary<string, StringValues> headers, MessageAction action, string oldTitle, FileShare userRole, Guid userId, params string[] description)
    {
        if (entry == null)
        {
            return;
        }

        var additionalParam = await GetAdditionalNotificationParamAsync(entry, action, oldTitle, userId, userRole);

        if (additionalParam != "")
        {
            description = description.Append(additionalParam).ToArray();
        }

        await SendHeadersMessageAsync(headers, action, _messageTarget.Create(entry.Id), description);
    }

    public async Task SendAsync<T1, T2>(FileEntry<T1> entry1, FileEntry<T2> entry2, IDictionary<string, StringValues> headers, MessageAction action, params string[] description)
    {
        if (entry1 == null || entry2 == null)
        {
            return;
        }

        var additionalParams = await GetAdditionalNotificationParamAsync(entry1, action);

        if (!string.IsNullOrEmpty(additionalParams))
        {
            description = description.Append(additionalParams).ToArray();
        }

        await SendHeadersMessageAsync(headers, action, _messageTarget.CreateFromGroupValues(new[] { entry1.Id.ToString(), entry2.Id.ToString() }), description);
    }

    private async Task SendHeadersMessageAsync(IDictionary<string, StringValues> headers, MessageAction action, MessageTarget target, params string[] description)
    {
        if (headers == null)//todo check need if
        {
            _logger.DebugEmptyRequestHeaders(action);

            return;
        }

        await _messageService.SendAsync(headers, action, target, description);
    }

    public async Task SendAsync<T>(FileEntry<T> entry, MessageAction action, string description)
    {
        if (entry == null)
        {
            return;
        }

        if (_httpContextAccessor == null)
        {
            _logger.DebugEmptyHttpRequest(action);

            return;
        }

        var additionalParam = await GetAdditionalNotificationParamAsync(entry, action);

        if (additionalParam != "")
        {
            await _messageService.SendAsync(action, _messageTarget.Create(entry.Id), description, additionalParam);
        }
        else
        {
            await _messageService.SendAsync(action, _messageTarget.Create(entry.Id), description);
        }
    }

    public async Task SendAsync<T>(FileEntry<T> entry, MessageAction action, string d1, string d2)
    {
        if (entry == null)
        {
            return;
        }

        if (_httpContextAccessor == null)
        {
            _logger.DebugEmptyHttpRequest(action);
            return;
        }

        await _messageService.SendAsync(action, _messageTarget.Create(entry.Id), d1, d2);
    }

    public async Task SendAsync<T>(FileEntry<T> entry, MessageInitiator initiator, MessageAction action, params string[] description)
    {
        if (entry == null)
        {
            return;
        }

        var additionalParam = await GetAdditionalNotificationParamAsync(entry, action);

        if (additionalParam != "")
        {
            description = description.Append(additionalParam).ToArray();
        }

        await _messageService.SendAsync(initiator, action, _messageTarget.Create(entry.Id), description);
    }

    private async Task<string> GetAdditionalNotificationParamAsync<T>(FileEntry<T> entry, MessageAction action, string oldTitle = null, Guid userid = default(Guid), FileShare userRole = FileShare.None)
    {
        var folderDao = _daoFactory.GetFolderDao<int>();
        var roomInfo = await folderDao.GetParentRoomInfoFromFileEntryAsync(entry);

        var info = new AdditionalNotificationInfo
        {
            RoomId = roomInfo.RoomId,
            RoomTitle = roomInfo.RoomTitle
        };

        if (action == MessageAction.RoomRenamed && !string.IsNullOrEmpty(oldTitle))
        {
            info.RoomOldTitle = oldTitle;
        }

        if ((action == MessageAction.RoomCreateUser || action == MessageAction.RoomRemoveUser)
            && userid != Guid.Empty)
        {
            info.UserIds = new List<Guid> { userid };
        }

        if (action == MessageAction.RoomUpdateAccessForUser
            && (userRole != FileShare.None)
            && userid != Guid.Empty)
        {
            info.UserIds = new List<Guid> { userid };
            info.UserRole = (int)userRole;
        }

        info.RootFolderTitle = entry.RootFolderType switch
        {
            FolderType.USER => FilesUCResource.MyFiles,
            FolderType.TRASH => FilesUCResource.Trash,
            _ => string.Empty
        };

        var serializedParam = JsonSerializer.Serialize(info);

        return serializedParam;
    }
}
