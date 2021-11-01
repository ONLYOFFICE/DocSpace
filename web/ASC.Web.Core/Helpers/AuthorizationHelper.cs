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


using System;
using System.Text;

using ASC.Common;
using ASC.Core;
using ASC.Security.Cryptography;

using Microsoft.AspNetCore.Http;

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
                    if (u != null && u.ID != ASC.Core.Users.Constants.LostUser.ID)
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