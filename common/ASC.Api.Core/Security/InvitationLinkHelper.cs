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

using Constants = ASC.Core.Users.Constants;
using ValidationResult = ASC.Security.Cryptography.EmailValidationKeyProvider.ValidationResult;

namespace ASC.Api.Core.Security;

[Scope]
public class InvitationLinkHelper
{
    private readonly IDbContextFactory<MessagesContext> _dbContextFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly MessageService _messageService;
    private readonly MessageTarget _messageTarget;
    private readonly Signature _signature;
    private readonly EmailValidationKeyProvider _emailValidationKeyProvider;
    private readonly UserManager _userManager;
    private readonly AuthManager _authManager;

    public TimeSpan IndividualLinkExpirationInterval => _emailValidationKeyProvider.ValidEmailKeyInterval;

    public InvitationLinkHelper(
        IHttpContextAccessor httpContextAccessor,
        MessageTarget messageTarget,
        MessageService messageService,
        Signature signature,
        IDbContextFactory<MessagesContext> dbContextFactory,
        EmailValidationKeyProvider emailValidationKeyProvider,
        UserManager userManager,
        AuthManager authManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _messageTarget = messageTarget;
        _messageService = messageService;
        _dbContextFactory = dbContextFactory;
        _signature = signature;
        _emailValidationKeyProvider = emailValidationKeyProvider;
        _userManager = userManager;
        _authManager = authManager;
    }

    public string MakeIndividualLinkKey(Guid linkId)
    {
        return _signature.Create(linkId);
    }

    public async Task<LinkValidationResult> ValidateAsync(string key, string email, EmployeeType employeeType)
    {
        var validationResult = new LinkValidationResult { Result = ValidationResult.Invalid };

        var (commonWithRoomLinkResult, linkId) = ValidateCommonWithRoomLink(key);

        if (commonWithRoomLinkResult != ValidationResult.Invalid)
        {
            validationResult.Result = commonWithRoomLinkResult;
            validationResult.LinkType = InvitationLinkType.CommonWithRoom;
            validationResult.LinkId = linkId;

            return validationResult;
        }

        var commonLinkResult = await _emailValidationKeyProvider.ValidateEmailKeyAsync(ConfirmType.LinkInvite.ToStringFast() + (int)employeeType,
            key, _emailValidationKeyProvider.ValidEmailKeyInterval);

        if (commonLinkResult == ValidationResult.Invalid)
        {
            commonLinkResult = await _emailValidationKeyProvider.ValidateEmailKeyAsync(email + ConfirmType.EmpInvite.ToStringFast() + (int)employeeType,
                key, _emailValidationKeyProvider.ValidEmailKeyInterval);
        }

        if (commonLinkResult != ValidationResult.Invalid)
        {
            validationResult.Result = commonLinkResult;
            validationResult.LinkType = InvitationLinkType.Common;

            return validationResult;
        }

        if (string.IsNullOrEmpty(email))
        {
            return validationResult;
        }

        var individualLinkResult = await ValidateIndividualLinkAsync(email, key, employeeType);

        validationResult.Result = individualLinkResult;
        validationResult.LinkType = InvitationLinkType.Individual;

        return validationResult;
    }

    private async Task<ValidationResult> ValidateIndividualLinkAsync(string email, string key, EmployeeType employeeType)
    {
        var result = await _emailValidationKeyProvider.ValidateEmailKeyAsync(email + ConfirmType.LinkInvite.ToStringFast() + employeeType.ToStringFast(),
            key, IndividualLinkExpirationInterval);

        if (result != ValidationResult.Ok)
        {
            return result;
        }

        var user = await _userManager.GetUserByEmailAsync(email);

        if (user.Equals(Constants.LostUser) || await _authManager.GetUserPasswordStampAsync(user.Id) != DateTime.MinValue)
        {
            return ValidationResult.Invalid;
        }

        var visitMessage = await GetLinkVisitMessageAsync(email, key);

        if (visitMessage == null)
        {
            await SaveLinkVisitMessageAsync(email, key);
        }
        else if (visitMessage.Date + _emailValidationKeyProvider.ValidVisitLinkInterval < DateTime.UtcNow)
        {
            return ValidationResult.Expired;
        }

        return result;
    }

    private (ValidationResult, Guid) ValidateCommonWithRoomLink(string key)
    {
        var linkId = _signature.Read<Guid>(key);

        return linkId == default ? (ValidationResult.Invalid, default) : (ValidationResult.Ok, linkId);
    }

    private async Task<AuditEvent> GetLinkVisitMessageAsync(string email, string key)
    {
        await using var context = _dbContextFactory.CreateDbContext();

        var target = _messageTarget.Create(email);
        var description = JsonConvert.SerializeObject(new[] { key });

        var message = await Queries.AuditEventsAsync(context, target.ToString(), description);

        return message;
    }

    private async Task SaveLinkVisitMessageAsync(string email, string key)
    {
        var headers = _httpContextAccessor?.HttpContext?.Request.Headers;
        var target = _messageTarget.Create(email);

        await _messageService.SendAsync(headers, MessageAction.RoomInviteLinkUsed, target, key);
    }
}

public enum InvitationLinkType
{
    Common,
    CommonWithRoom,
    Individual
}

public class LinkValidationResult
{
    public ValidationResult Result { get; set; }
    public InvitationLinkType LinkType { get; set; }
    public Guid LinkId { get; set; }
}

static file class Queries
{
    public static readonly Func<MessagesContext, string, string, Task<AuditEvent>> AuditEventsAsync =
        EF.CompileAsyncQuery(
            (MessagesContext ctx, string target, string description) =>
                ctx.AuditEvents.FirstOrDefault(a => a.Target == target && a.DescriptionRaw == description));
}