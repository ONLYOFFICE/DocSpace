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

using static ASC.Security.Cryptography.EmailValidationKeyProvider;

using Constants = ASC.Core.Users.Constants;

namespace ASC.Api.Core.Security;

[Transient]
public class EmailValidationKeyModelHelper
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly EmailValidationKeyProvider _provider;
    private readonly AuthContext _authContext;
    private readonly UserManager _userManager;
    private readonly AuthManager _authentication;
    private readonly RoomInvitationLinksService _roomLinksService;
    private readonly AuditEventsRepository _auditEventsRepository;
    private readonly TenantUtil _tenantUtil;
    private readonly MessageTarget _messageTarget;

    public EmailValidationKeyModelHelper(
        IHttpContextAccessor httpContextAccessor,
        EmailValidationKeyProvider provider,
        AuthContext authContext,
        UserManager userManager,
        AuthManager authentication,
        RoomInvitationLinksService roomLinksService,
        AuditEventsRepository auditEventsRepository,
        TenantUtil tenantUtil,
        MessageTarget messageTarget)
    {
        _httpContextAccessor = httpContextAccessor;
        _provider = provider;
        _authContext = authContext;
        _userManager = userManager;
        _authentication = authentication;
        _roomLinksService = roomLinksService;
        _auditEventsRepository = auditEventsRepository;
        _tenantUtil = tenantUtil;
        _messageTarget = messageTarget;
    }

    public EmailValidationKeyModel GetModel()
    {
        var request = QueryHelpers.ParseQuery(_httpContextAccessor.HttpContext.Request.Headers["confirm"]);

        var type = request.ContainsKey("type") ? request["type"].FirstOrDefault() : null;

        ConfirmType? cType = null;
        if (ConfirmTypeExtensions.TryParse(type, out var confirmType))
        {
            cType = confirmType;
        }

        request.TryGetValue("key", out var key);

        request.TryGetValue("emplType", out var emplType);
        EmployeeTypeExtensions.TryParse(emplType, out var employeeType);

        request.TryGetValue("email", out var _email);

        request.TryGetValue("uid", out var userIdKey);
        Guid.TryParse(userIdKey, out var userId);

        request.TryGetValue("access", out var fileShareRaw);
        int.TryParse(fileShareRaw, out var fileShare);

        request.TryGetValue("roomId", out var roomId);

        return new EmailValidationKeyModel
        {
            Email = _email,
            EmplType = employeeType,
            Key = key,
            Type = cType,
            UiD = userId,
            RoomAccess = fileShare,
            RoomId = roomId
        };
    }

    public ValidationResult Validate(EmailValidationKeyModel inDto)
    {
        var (key, emplType, email, uiD, type, fileShare, roomId) = inDto;

        ValidationResult checkKeyResult;

        switch (type)
        {
            case ConfirmType.EmpInvite:
                checkKeyResult = _provider.ValidateEmailKey(email + type + (int)emplType, key, _provider.ValidEmailKeyInterval);
                break;

            case ConfirmType.LinkInvite:
                if (fileShare != default && !string.IsNullOrEmpty(roomId))
                {
                    checkKeyResult = _provider.ValidateEmailKey(email + type + ((int)emplType + (int)fileShare + roomId), key, _provider.ValidEmailKeyInterval);
                    if (checkKeyResult == ValidationResult.Ok &&
                        !_roomLinksService.VisitProcess(roomId, email, key, _provider.ValidVisitLinkInterval))
                    {
                        checkKeyResult = ValidationResult.Expired;
                    }
                    break;
                }

                checkKeyResult = _provider.ValidateEmailKey(type.ToString() + (int)emplType, key, _provider.ValidEmailKeyInterval);
                break;

            case ConfirmType.PortalOwnerChange:
                checkKeyResult = _provider.ValidateEmailKey(email + type + uiD.GetValueOrDefault(), key, _provider.ValidEmailKeyInterval);
                break;

            case ConfirmType.EmailChange:
                checkKeyResult = _provider.ValidateEmailKey(email + type + _authContext.CurrentAccount.ID, key, _provider.ValidEmailKeyInterval);
                break;
            case ConfirmType.PasswordChange:
                var userInfo = _userManager.GetUserByEmail(email);
                var auditEvent = _auditEventsRepository.GetByFilter(action: MessageAction.UserSentPasswordChangeInstructions, entry: EntryType.User, target: _messageTarget.Create(userInfo.Id).ToString(), limit: 1).FirstOrDefault();
                var passwordStamp = _authentication.GetUserPasswordStamp(_userManager.GetUserByEmail(email).Id);

                string hash;

                if (auditEvent != null)
                {
                    var auditEventDate = _tenantUtil.DateTimeToUtc(auditEvent.Date);

                    hash = (auditEventDate.CompareTo(passwordStamp) > 0 ? auditEventDate : passwordStamp).ToString("s");
                }
                else
                {
                    hash = passwordStamp.ToString("s");
                }

                checkKeyResult = _provider.ValidateEmailKey(email + type + hash, key, _provider.ValidEmailKeyInterval);
                break;

            case ConfirmType.Activation:
                checkKeyResult = _provider.ValidateEmailKey(email + type + uiD, key, _provider.ValidEmailKeyInterval);
                break;

            case ConfirmType.ProfileRemove:
                // validate UiD
                var user = _userManager.GetUsers(uiD.GetValueOrDefault());
                if (user == null || user == Constants.LostUser || user.Status == EmployeeStatus.Terminated || _authContext.IsAuthenticated && _authContext.CurrentAccount.ID != uiD)
                {
                    return ValidationResult.Invalid;
                }

                checkKeyResult = _provider.ValidateEmailKey(email + type + uiD, key, _provider.ValidEmailKeyInterval);
                break;

            case ConfirmType.Wizard:
                checkKeyResult = _provider.ValidateEmailKey("" + type, key, _provider.ValidEmailKeyInterval);
                break;

            case ConfirmType.PhoneActivation:
            case ConfirmType.PhoneAuth:
            case ConfirmType.TfaActivation:
            case ConfirmType.TfaAuth:
            case ConfirmType.Auth:
                checkKeyResult = _provider.ValidateEmailKey(email + type, key, _provider.ValidAuthKeyInterval);
                break;

            case ConfirmType.PortalContinue:
                checkKeyResult = _provider.ValidateEmailKey(email + type, key);
                break;

            default:
                checkKeyResult = _provider.ValidateEmailKey(email + type, key, _provider.ValidEmailKeyInterval);
                break;
        }

        return checkKeyResult;
    }
}