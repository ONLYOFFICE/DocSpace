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
public class RoomInvitationLinksService
{
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly MessageTarget _messageTarget;
    private readonly UserManager _userManager;
    private readonly MessageService _messageService;
    private readonly IDbContextFactory<MessagesContext> _dbContextFactory;

    public RoomInvitationLinksService(
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

    public string GenerateLink<T>(T id, int fileShare, EmployeeType employeeType, Guid guid)
    {
        return GenerateLink(id, string.Empty, fileShare, employeeType, guid);
    }

    public string GenerateLink<T>(T id, string email, int fileShare, EmployeeType employeeType, Guid guid)
    {
        var user = _userManager.GetUserByEmail(email);

        if (user != ASC.Core.Users.Constants.LostUser)
        {
            throw new Exception("The user with this email already exists");
        }

        var postifx = (int)employeeType + fileShare + id.ToString();

        var link = _commonLinkUtility.GetConfirmationUrl(email, ConfirmType.LinkInvite, postifx, guid)
            + $"&emplType={employeeType:d}&roomId={id}&roomAccess={fileShare}";

        return link;
    }

    public bool VisitProcess(string id, string email, string key, TimeSpan interval)
    {
        if (!string.IsNullOrEmpty(email))
        {
            var user = _userManager.GetUserByEmail(email);

            if (user != ASC.Core.Users.Constants.LostUser)
            {
                return false;
            }
        }

        var message = GetLinkInfo(id, email, key);

        if (message == null)
        {
            SaveVisitLinkInfo(id, email, key);

            return true;
        }

        return message.Date + interval > DateTime.UtcNow;
    }

    private void SaveVisitLinkInfo(string id, string email, string key)
    {
        var headers = _httpContextAccessor?.HttpContext?.Request?.Headers;
        var target = _messageTarget.CreateFromGroupValues(email != null ? new[] { id, email } : new[] { id });

        _messageService.Send(headers, MessageAction.RoomInviteLinkUsed, target, key);
    }

    private AuditEvent GetLinkInfo(string id, string email, string key)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var target = _messageTarget.CreateFromGroupValues(email != null ? new[] { id, email } : new[] { id });
        var description = JsonConvert.SerializeObject(new[] { key },
            new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            });

        var message = context.AuditEvents.Where(a => a.Target == target.ToString() &&
            a.DescriptionRaw == description).FirstOrDefault();

        return message;
    }
}