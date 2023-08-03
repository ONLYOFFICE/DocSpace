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

namespace ASC.Web.Api.Controllers.Settings;

///<summary>
/// Portal settings API.
///</summary>
///<name>settings</name>
[Scope]
[DefaultRoute]
[ApiController]
[ControllerName("settings")]
public partial class BaseSettingsController : ControllerBase
{
    //private const int ONE_THREAD = 1;

    //private static readonly DistributedTaskQueue quotaTasks = new DistributedTaskQueue("quotaOperations", ONE_THREAD);
    //private static DistributedTaskQueue LDAPTasks { get; } = new DistributedTaskQueue("ldapOperations");
    //private static DistributedTaskQueue SMTPTasks { get; } = new DistributedTaskQueue("smtpOperations");

    internal readonly ApiContext ApiContext;
    internal readonly IMemoryCache MemoryCache;
    internal readonly WebItemManager WebItemManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly int _maxCount = 10;
    private readonly int _expirationMinutes = 2;

    public BaseSettingsController(ApiContext apiContext, IMemoryCache memoryCache, WebItemManager webItemManager, IHttpContextAccessor httpContextAccessor)
    {
        ApiContext = apiContext;
        MemoryCache = memoryCache;
        WebItemManager = webItemManager;
        _httpContextAccessor = httpContextAccessor;
    }

    internal void CheckCache(string basekey)
    {
        var key = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString() + basekey;
        if (MemoryCache.TryGetValue<int>(key, out var count))
        {
            if (count > _maxCount)
            {
                throw new Exception(Resource.ErrorRequestLimitExceeded);
            }
        }

        MemoryCache.Set(key, count + 1, TimeSpan.FromMinutes(_expirationMinutes));
    }

    internal string GetProductName(Guid productId)
    {
        var product = WebItemManager[productId];
        return productId == Guid.Empty ? "All" : product != null ? product.Name : productId.ToString();
    }
}