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

namespace ASC.Web.Core.Helpers
{
    [Scope]
    public class AuthorizationHelper
    {
        private IHttpContextAccessor HttpContextAccessor { get; }
        private UserManager UserManager { get; }
        private SecurityContext SecurityContext { get; }
        private PasswordHasher PasswordHasher { get; }

        public AuthorizationHelper(
            IHttpContextAccessor httpContextAccessor,
            UserManager userManager,
            SecurityContext securityContext,
            PasswordHasher passwordHasher)
        {
            HttpContextAccessor = httpContextAccessor;
            UserManager = userManager;
            SecurityContext = securityContext;
            PasswordHasher = passwordHasher;
        }

        public bool ProcessBasicAuthorization(out string authCookie)
        {
            authCookie = null;
            try
            {
                //Try basic
                var authorization = HttpContextAccessor.HttpContext.Request.Cookies["asc_auth_key"] ?? HttpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authorization))
                {
                    return false;
                }

                authorization = authorization.Trim();
                if (0 <= authorization.IndexOf("Basic", 0))
                {
                    var arr = Encoding.ASCII.GetString(Convert.FromBase64String(authorization.Substring(6))).Split(new[] { ':' });
                    var username = arr[0];
                    var password = arr[1];
                    var u = UserManager.GetUserByEmail(username);
                    if (u != null && u.Id != ASC.Core.Users.Constants.LostUser.Id)
                    {
                        var passwordHash = PasswordHasher.GetClientPassword(password);
                        authCookie = SecurityContext.AuthenticateMe(u.Email, passwordHash);
                    }
                }
                else if (0 <= authorization.IndexOf("Bearer", 0))
                {
                    authorization = authorization.Substring("Bearer ".Length);
                    if (SecurityContext.AuthenticateMe(authorization))
                    {
                        authCookie = authorization;
                    }
                }
                else
                {
                    if (SecurityContext.AuthenticateMe(authorization))
                    {
                        authCookie = authorization;
                    }
                }
            }
            catch (Exception) { }
            return SecurityContext.IsAuthenticated;
        }
    }
}