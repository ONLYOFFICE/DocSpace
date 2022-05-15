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

using ASC.MessagingSystem.Models;

using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ASC.Api.Core.Security;

[Scope]
public class RoomLinksService
{
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly MessageTarget _messageTarget;
    private readonly UserManager _userManager;
    private readonly MessageFactory _messageFactory;
    private readonly Lazy<MessagesContext> _messagesContext;
    private readonly IMapper _mapper;

    public RoomLinksService(
        CommonLinkUtility commonLinkUtility,
        IHttpContextAccessor httpContextAccessor,
        MessageTarget messageTarget,
        UserManager userManager,
        MessageFactory messageFactory,
        DbContextManager<MessagesContext> dbContextManager,
        IMapper mapper)
    {
        _commonLinkUtility = commonLinkUtility;
        _httpContextAccessor = httpContextAccessor;
        _messageTarget = messageTarget;
        _userManager = userManager;
        _messageFactory = messageFactory;
        _messagesContext = new Lazy<MessagesContext>(() => dbContextManager.Value);
        _mapper = mapper;
    }

    public string GenerateLink<T>(T id, string email, int fileShare, EmployeeType employeeType, Guid guid)
    {
        var user = _userManager.GetUserByEmail(email);

        if (user != ASC.Core.Users.Constants.LostUser)
        {
            throw new Exception();
        }

        var postifx = ((int)employeeType + fileShare) + id.ToString();

        var link = _commonLinkUtility.GetConfirmationUrl(email, ConfirmType.RoomInvite, postifx, guid)
            + $"&emplType={employeeType:d}&roomId={id}&access={fileShare}";

        return link;
    }

    public bool VisitProcess(string id, string email, string key, TimeSpan interval)
    {
        var user = _userManager.GetUserByEmail(email);

        if (user != ASC.Core.Users.Constants.LostUser)
        {
            return false;
        }

        var message = GetLinkInfo(id, email, key);

        if (message == null)
        {
            SaveVisitLinkInfo(id, email, key, interval);

            return true;
        }

        return message.Date <= DateTime.Now ? false : true;
    }

    private void SaveVisitLinkInfo(string id, string email, string key, TimeSpan interval)
    {
        var headers = _httpContextAccessor?.HttpContext?.Request?.Headers;
        var target = _messageTarget.Create(new[] {id, email});

        var message = _messageFactory.Create(null, headers, MessageAction.RoomInviteLinkUsed, target);
        var audit = _mapper.Map<EventMessage, AuditEvent>(message);

        audit.Date = DateTime.Now + interval;
        audit.DescriptionRaw = Serialize(new[] { key });

        var context = _messagesContext.Value;
        context.AuditEvents.Add(audit);
        context.SaveChanges();
    }

    private AuditEvent GetLinkInfo(string id, string email, string key)
    {
        var context = _messagesContext.Value;
        var target = _messageTarget.Create(new[] { id, email });

        var message = context.AuditEvents.Where(a => a.Target == target.ToString() && 
            a.DescriptionRaw == Serialize(new[] { key })).FirstOrDefault();

        return message;
    }

    private string Serialize(IEnumerable<string> description)
    {
        return JsonConvert.SerializeObject(description,
            new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            });
    }
}