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


using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Cors;
using System.Web.Http.Cors;

namespace ASC.ApiSystem.Classes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class EnableCorsAppSettingsAttribute: Attribute, ICorsPolicyProvider
    {
        private readonly CorsPolicy policy;

        public EnableCorsAppSettingsAttribute(string appSettingOriginKey, string appSettingHeadersKey)
        {
            policy = new CorsPolicy
            {
                AllowAnyMethod = true
            };

            // loads the origins from AppSettings
            string originsString = ConfigurationManager.AppSettings[appSettingOriginKey];

            if (string.IsNullOrEmpty(originsString))
            {
                policy.AllowAnyOrigin = false;
            }
            else if (string.Compare(originsString, "*") == 0)
            {
                policy.AllowAnyOrigin = true;
            }
            else
            {
                foreach (var origin in originsString.Split(','))
                {
                    policy.Origins.Add(origin);
                }
            }


            string headersString = ConfigurationManager.AppSettings[appSettingHeadersKey];
            if (string.IsNullOrEmpty(headersString))
            {
                policy.AllowAnyHeader = false;
            }
            else if (string.Compare(headersString, "*") == 0)
            {
                policy.AllowAnyHeader = true;
            }
            else
            {
                foreach (var header in headersString.Split(','))
                {
                    policy.Headers.Add(header);
                }
            }
        }

        public Task<CorsPolicy> GetPolicyAsync(HttpContext context, string policyName)
        {
            return Task.FromResult(policy);
        }
    }
}