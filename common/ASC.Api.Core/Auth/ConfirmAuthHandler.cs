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

using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Api.Core.Auth;

[Transient]
public class ConfirmAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly SecurityContext _securityContext;
    private readonly UserManager _userManager;
    private readonly EmailValidationKeyModelHelper _emailValidationKeyModelHelper;

    public ConfirmAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) :
        base(options, logger, encoder, clock)
    { }

    public ConfirmAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        SecurityContext securityContext,
        UserManager userManager,
        EmailValidationKeyModelHelper emailValidationKeyModelHelper) :
        base(options, logger, encoder, clock)
    {
        _securityContext = securityContext;
        _userManager = userManager;
        _emailValidationKeyModelHelper = emailValidationKeyModelHelper;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var emailValidationKeyModel = _emailValidationKeyModelHelper.GetModel();

        if (!emailValidationKeyModel.Type.HasValue)
        {
            return _securityContext.IsAuthenticated
                ? AuthenticateResult.Success(new AuthenticationTicket(Context.User, new AuthenticationProperties(), Scheme.Name))
                    : AuthenticateResult.Fail(new AuthenticationException(nameof(HttpStatusCode.Unauthorized)));
        }

        EmailValidationKeyProvider.ValidationResult checkKeyResult;
        try
        {
            checkKeyResult = await _emailValidationKeyModelHelper.ValidateAsync(emailValidationKeyModel);
        }
        catch (ArgumentNullException)
        {
            checkKeyResult = EmailValidationKeyProvider.ValidationResult.Invalid;
        }

        var claims = new List<Claim>()
        {
                new Claim(ClaimTypes.Role, emailValidationKeyModel.Type.ToString())
        };

        if (checkKeyResult == EmailValidationKeyProvider.ValidationResult.Ok)
        {
            Guid userId;
            if (!_securityContext.IsAuthenticated)
            {
                if (emailValidationKeyModel.UiD.HasValue && !emailValidationKeyModel.UiD.Equals(Guid.Empty))
                {
                    userId = emailValidationKeyModel.UiD.Value;
                }
                else
                {
                    if (emailValidationKeyModel.Type == ConfirmType.EmailActivation
                        || emailValidationKeyModel.Type == ConfirmType.EmpInvite
                        || emailValidationKeyModel.Type == ConfirmType.LinkInvite)
                    {
                        userId = ASC.Core.Configuration.Constants.CoreSystem.ID;
                    }
                    else
                    {
                        userId = (await _userManager.GetUserByEmailAsync(emailValidationKeyModel.Email)).Id;
                    }
                }
            }
            else
            {
                userId = _securityContext.CurrentAccount.ID;
            }

            await _securityContext.AuthenticateMeWithoutCookieAsync(userId, claims);
        }

        var result = checkKeyResult switch
        {
            EmailValidationKeyProvider.ValidationResult.Ok => AuthenticateResult.Success(new AuthenticationTicket(Context.User, new AuthenticationProperties(), Scheme.Name)),
            _ => AuthenticateResult.Fail(new AuthenticationException(nameof(HttpStatusCode.Unauthorized)))
        };

        return result;
    }
}