/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/




using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ASC.ApiSystem.Classes;
using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ASC.ApiSystem.Controllers
{
    [Scope]
    [ApiController]
    [Route("[controller]")]
    public class PeopleController : ControllerBase
    {
        private ILog Log { get; }
        private HostedSolution HostedSolution { get; }
        private UserFormatter UserFormatter { get; }
        private ICache Cache { get; }
        private CoreSettings CoreSettings { get; }
        private CommonLinkUtility CommonLinkUtility { get; }
        public IHttpContextAccessor HttpContextAccessor { get; }

        public PeopleController(
            IOptionsMonitor<ILog> option,
            IOptionsSnapshot<HostedSolution> hostedSolutionOptions,
            UserFormatter userFormatter,
            ICache cache,
            CoreSettings coreSettings,
            CommonLinkUtility commonLinkUtility,
            IHttpContextAccessor httpContextAccessor)
        {
            Log = option.Get("ASC.ApiSystem.People");
            HostedSolution = hostedSolutionOptions.Value;
            UserFormatter = userFormatter;
            Cache = cache;
            CoreSettings = coreSettings;
            CommonLinkUtility = commonLinkUtility;
            HttpContextAccessor = httpContextAccessor;
        }

        #region For TEST api

        [HttpGet("test")]
        public IActionResult Check()
        {
            return Ok(new
            {
                value = "Portal api works"
            });
        }

        #endregion

        #region API methods

        [HttpPost("find")]
        [AllowCrossSiteJson]
        public IActionResult Find(IEnumerable<Guid> userIds)
        {
            var sw = Stopwatch.StartNew();
            userIds = userIds ?? new List<Guid>();

            var users = HostedSolution.FindUsers(userIds);

            var result = users.Select(user => new
            {
                id = user.ID,
                name = UserFormatter.GetUserName(user),
                email = user.Email,

                link = GetUserProfileLink(user)
            });

            Log.DebugFormat("People find {0} / {1}; Elapsed {2} ms", result.Count(), userIds.Count(), sw.ElapsedMilliseconds);
            sw.Stop();

            return Ok(new
            {
                result
            });
        }

        #endregion

        #region private methods

        private string GetTenantDomain(int tenantId)
        {
            var domain = Cache.Get<string>(tenantId.ToString());
            if (string.IsNullOrEmpty(domain))
            {
                var tenant = HostedSolution.GetTenant(tenantId);
                domain = tenant.GetTenantDomain(CoreSettings);
                Cache.Insert(tenantId.ToString(), domain, TimeSpan.FromMinutes(10));
            }
            return domain;
        }

        private string GetUserProfileLink(UserInfo user)
        {
            var tenantDomain = GetTenantDomain(user.Tenant);
            return string.Format("{0}{1}{2}/{3}",
                                 HttpContextAccessor.HttpContext.Request.Scheme,
                                 Uri.SchemeDelimiter,
                                 tenantDomain,
                                 "Products/People/Profile.aspx?" + CommonLinkUtility.GetUserParamsPair(user));
        }

        #endregion
    }
}