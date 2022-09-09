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

namespace ASC.Api.Core.Security;

[Scope]
public class DocSpaceLinksHelper
{
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly IDbContextFactory<MessagesContext> _dbContextFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly MessageService _messageService;
    private readonly MessageTarget _messageTarget;
    private readonly UserManager _userManager;

    public DocSpaceLinksHelper(
        CommonLinkUtility commonLinkUtility,
        IHttpContextAccessor httpContextAccessor,
        MessageTarget messageTarget,
        UserManager userManager,
        MessageService messageService,
        IDbContextFactory<MessagesContext> dbContextFactory)
    {
        _commonLinkUtility = commonLinkUtility;
        _httpContextAccessor = httpContextAccessor;
        _messageTarget = messageTarget;
        _userManager = userManager;
        _messageService = messageService;
        _dbContextFactory = dbContextFactory;
    }

    public static string MakePayload(string email, EmployeeType employeeType, Guid target)
    {
        return email + ConfirmType.LinkInvite.ToStringFast() + employeeType.ToStringFast() + target.ToString();
    }

    public string GenerateInvitationRoomLink(string email, EmployeeType employeeType, Guid createdBy, Guid target)
    {
        var link = _commonLinkUtility.GetConfirmationEmailUrl(email, ConfirmType.LinkInvite, employeeType + target.ToString(), createdBy)
            + $"&emplType={employeeType:d}";

        if (target != default)
        {
            link += $"&target={target}";
        }

        return link;
    }

    public bool ProcessLinkVisit(string email, string key, TimeSpan interval)
    {
        var message = GetLinkInfo(email, key);

        if (message == null)
        {
            SaveVisitLinkInfo(email, key);

            return true;
        }

        return message.Date + interval > DateTime.UtcNow;
    }

    private AuditEvent GetLinkInfo(string email, string key)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var target = _messageTarget.Create(email);
        var description = JsonConvert.SerializeObject(new[] { key },
            new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            });

        var message = context.AuditEvents.Where(a => a.Target == target.ToString() &&
            a.DescriptionRaw == description).FirstOrDefault();

        return message;
    }

    private void SaveVisitLinkInfo(string email, string key)
    {
        var headers = _httpContextAccessor?.HttpContext?.Request?.Headers;
        var target = _messageTarget.Create(email);

        _messageService.Send(headers, MessageAction.RoomInviteLinkUsed, target, key);
    }
}