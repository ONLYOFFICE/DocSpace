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
    public const string RoomTargetMark = "room:";

    private readonly ILogger _logger;
    private readonly MessageTarget _messageTarget;
    private readonly MessageService _messageService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDaoFactory _daoFactory;

    public FilesMessageService(
        ILoggerProvider options,
        MessageTarget messageTarget,
        MessageService messageService,
        IDaoFactory daoFactory)
    {
        _logger = options.CreateLogger("ASC.Messaging");
        _messageTarget = messageTarget;
        _messageService = messageService;
        _daoFactory = daoFactory;
    }

    public FilesMessageService(
        ILoggerProvider options,
        MessageTarget messageTarget,
        MessageService messageService,
        IHttpContextAccessor httpContextAccessor,
        IDaoFactory daoFactory)
        : this(options, messageTarget, messageService, daoFactory)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Send(IDictionary<string, StringValues> headers, MessageAction action, params string[] description)
    {
        SendHeadersMessage(headers, action, null, description);
    }

    public async Task Send<T>(FileEntry<T> entry, IDictionary<string, StringValues> headers, MessageAction action, params string[] description)
    {
        if (entry == null)
        {
            return;
        }

        SendHeadersMessage(headers, action, await CreateMessageTargetAsync(entry), description);
    }

    public async Task Send<T1, T2>(FileEntry<T1> entry1, FileEntry<T2> entry2, IDictionary<string, StringValues> headers, MessageAction action, params string[] description)
    {
        if (entry1 == null || entry2 == null)
        {
            return;
        }

        SendHeadersMessage(headers, action, await CreateMessageTargetAsync(entry1, entry2), description);
    }

    private void SendHeadersMessage(IDictionary<string, StringValues> headers, MessageAction action, MessageTarget target, params string[] description)
    {
        if (headers == null)//todo check need if
        {
            _logger.DebugEmptyRequestHeaders(action);

            return;
        }

        _messageService.Send(headers, action, target, description);
    }

    public async Task Send<T>(FileEntry<T> entry, MessageAction action, string description)
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

        _messageService.Send(action, await CreateMessageTargetAsync(entry), description);
    }

    public async Task Send<T>(FileEntry<T> entry, MessageAction action, string d1, string d2)
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

        _messageService.Send(action, await CreateMessageTargetAsync(entry), d1, d2);
    }

    public async Task Send<T>(FileEntry<T> entry, MessageInitiator initiator, MessageAction action, params string[] description)
    {
        if (entry == null)
        {
            return;
        }

        _messageService.Send(initiator, action, await CreateMessageTargetAsync(entry), description);
    }

    private async Task<MessageTarget> CreateMessageTargetAsync<T>(FileEntry<T> entry)
    {
        var messageEntries = await CreateMessageEntries(entry);

        return _messageTarget.CreateFromGroupValues(messageEntries);
    }

    private async Task<MessageTarget> CreateMessageTargetAsync<T1, T2>(FileEntry<T1> entry1, FileEntry<T2> entry2)
    {
        var messageEntries1 = await CreateMessageEntries(entry1);
        var messageEntries2 = await CreateMessageEntries(entry2);

        var entries = messageEntries1.Concat(messageEntries2);

        return _messageTarget.CreateFromGroupValues(entries);
    }

    private async Task<IEnumerable<string>> CreateMessageEntries<T>(FileEntry<T> entry)
    {
        if (entry.RootFolderType != FolderType.VirtualRooms && entry.RootFolderType != FolderType.Archive)
        {
            return new[] { entry.Id.ToString() };
        }

        if (entry is Folder<T> folder && DocSpaceHelper.IsRoom(folder.FolderType))
        {
            return new[] { $"{RoomTargetMark}{folder.Id}" };
        }

        var folderDao = _daoFactory.GetFolderDao<T>();
        var parentRoom = await folderDao.GetParentFoldersAsync(entry.ParentId).FirstOrDefaultAsync(f => DocSpaceHelper.IsRoom(f.FolderType));

        return new[] { $"{RoomTargetMark}{parentRoom.Id}", entry.Id.ToString() };
    }
}
