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

using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Api.Controllers.Settings;

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

    internal readonly ApiContext _apiContext;
    internal readonly IMemoryCache _memoryCache;
    internal readonly WebItemManager _webItemManager;

    public BaseSettingsController(ApiContext apiContext, IMemoryCache memoryCache, WebItemManager webItemManager)
    {
        _apiContext = apiContext;
        _memoryCache = memoryCache;
        _webItemManager = webItemManager;
    }

    


    private readonly int maxCount = 10;
    private readonly int expirationMinutes = 2;
    internal void CheckCache(string basekey)
    {
        var key = _apiContext.HttpContextAccessor.HttpContext.Request.GetUserHostAddress() + basekey;
        if (_memoryCache.TryGetValue<int>(key, out var count))
        {
            if (count > maxCount)
                throw new Exception(Resource.ErrorRequestLimitExceeded);
        }

        _memoryCache.Set(key, count + 1, TimeSpan.FromMinutes(expirationMinutes));
    }

    internal string GetProductName(Guid productId)
    {
        var product = _webItemManager[productId];
        return productId == Guid.Empty ? "All" : product != null ? product.Name : productId.ToString();
    }
}