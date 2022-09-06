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


namespace ASC.ApiSystem.Controllers;

[Scope]
[ApiController]
[Route("[controller]")]
public class PeopleController : ControllerBase
{
    private ILogger<PeopleController> Log { get; }
    private HostedSolution HostedSolution { get; }
    private UserFormatter UserFormatter { get; }
    private ICache Cache { get; }
    private CoreSettings CoreSettings { get; }
    private CommonLinkUtility CommonLinkUtility { get; }
    public IHttpContextAccessor HttpContextAccessor { get; }

    public PeopleController(
        ILogger<PeopleController> option,
        IOptionsSnapshot<HostedSolution> hostedSolutionOptions,
        UserFormatter userFormatter,
        ICache cache,
        CoreSettings coreSettings,
        CommonLinkUtility commonLinkUtility,
        IHttpContextAccessor httpContextAccessor)
    {
        Log = option;
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
            id = user.Id,
            name = UserFormatter.GetUserName(user),
            email = user.Email,

            link = GetUserProfileLink(user)
        });

        Log.LogDebug("People find {0} / {1}; Elapsed {2} ms", result.Count(), userIds.Count(), sw.ElapsedMilliseconds);
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
