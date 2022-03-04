/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
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

namespace ASC.Web.Files.Helpers;

[Scope]
public class FilesMessageService
{
    private readonly ILog _logger;
    private readonly MessageTarget _messageTarget;
    private readonly MessageService _messageService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FilesMessageService(
        IOptionsMonitor<ILog> options,
        MessageTarget messageTarget,
        MessageService messageService)
    {
        _logger = options.Get("ASC.Messaging");
        _messageTarget = messageTarget;
        _messageService = messageService;
    }

    public FilesMessageService(
        IOptionsMonitor<ILog> options,
        MessageTarget messageTarget,
        MessageService messageService,
        IHttpContextAccessor httpContextAccessor)
        : this(options, messageTarget, messageService)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Send(IDictionary<string, StringValues> headers, MessageAction action)
    {
        SendHeadersMessage(headers, action, null);
    }

    public void Send<T>(FileEntry<T> entry, IDictionary<string, StringValues> headers, MessageAction action, params string[] description)
    {
        // do not log actions in users folder
        if (entry == null || entry.RootFolderType == FolderType.USER)
        {
            return;
        }

        SendHeadersMessage(headers, action, _messageTarget.Create(entry.Id), description);
    }

    public void Send<T1, T2>(FileEntry<T1> entry1, FileEntry<T2> entry2, IDictionary<string, StringValues> headers, MessageAction action, params string[] description)
    {
        // do not log actions in users folder
        if (entry1 == null || entry2 == null || entry1.RootFolderType == FolderType.USER || entry2.RootFolderType == FolderType.USER)
        {
            return;
        }

        SendHeadersMessage(headers, action, _messageTarget.Create(new[] { entry1.Id.ToString(), entry2.Id.ToString() }), description);
    }

    private void SendHeadersMessage(IDictionary<string, StringValues> headers, MessageAction action, MessageTarget target, params string[] description)
    {
        if (headers == null)
        {
            _logger.Debug(string.Format("Empty Request Headers for \"{0}\" type of event", action));

            return;
        }

        _messageService.Send(headers, action, target, description);
    }

    public void Send<T>(FileEntry<T> entry, MessageAction action, params string[] description)
    {
        // do not log actions in users folder
        if (entry == null || entry.RootFolderType == FolderType.USER)
        {
            return;
        }

        if (_httpContextAccessor == null)
        {
            _logger.Debug(string.Format("Empty Http Request for \"{0}\" type of event", action));

            return;
        }

        _messageService.Send(action, _messageTarget.Create(entry.Id), description);
    }

    public void Send<T>(FileEntry<T> entry, MessageInitiator initiator, MessageAction action, params string[] description)
    {
        if (entry == null || entry.RootFolderType == FolderType.USER)
        {
            return;
        }

        _messageService.Send(initiator, action, _messageTarget.Create(entry.Id), description);
    }
}
