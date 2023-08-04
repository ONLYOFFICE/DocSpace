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

using static ASC.Web.Core.Sms.SmsKeyStorage;

using SecurityAction = ASC.Common.Security.Authorizing.Action;

namespace ASC.Web.Core;

[Singletone]
public class WebItemSecurityCache
{
    private readonly ICache _cache;
    private readonly ICacheNotify<WebItemSecurityNotifier> _cacheNotify;

    public WebItemSecurityCache(ICacheNotify<WebItemSecurityNotifier> cacheNotify, ICache cache)
    {
        _cache = cache;
        _cacheNotify = cacheNotify;
        _cacheNotify.Subscribe((r) =>
        {
            ClearCache(r.Tenant);
        }, CacheNotifyAction.Any);
    }

    public void ClearCache(int tenantId)
    {
        _cache.Remove(GetCacheKey(tenantId));
    }

    public string GetCacheKey(int tenantId)
    {
        return $"{tenantId}:webitemsecurity";
    }

    public void Publish(int tenantId)
    {
        _cacheNotify.Publish(new WebItemSecurityNotifier { Tenant = tenantId }, CacheNotifyAction.Any);
    }

    public Dictionary<string, bool> Get(int tenantId)
    {
        return _cache.Get<Dictionary<string, bool>>(GetCacheKey(tenantId));
    }

    public Dictionary<string, bool> GetOrInsert(int tenantId)
    {

        var dic = Get(tenantId);
        if (dic == null)
        {
            dic = new Dictionary<string, bool>();
            _cache.Insert(GetCacheKey(tenantId), dic, DateTime.UtcNow.Add(TimeSpan.FromMinutes(1)));
        }

        return dic;
    }
}

[Scope]
public class WebItemSecurity
{
    private static readonly SecurityAction _read = new SecurityAction(new Guid("77777777-32ae-425f-99b5-83176061d1ae"), "ReadWebItem", false, true);

    private readonly UserManager _userManager;
    private readonly AuthContext _authContext;
    private readonly PermissionContext _permissionContext;
    private readonly AuthManager _authentication;
    private readonly WebItemManager _webItemManager;
    private readonly TenantManager _tenantManager;
    private readonly AuthorizationManager _authorizationManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly WebItemSecurityCache _webItemSecurityCache;
    private readonly SettingsManager _settingsManager;
    private readonly CountPaidUserChecker _countPaidUserChecker;

    public WebItemSecurity(
        UserManager userManager,
        AuthContext authContext,
        PermissionContext permissionContext,
        AuthManager authentication,
        WebItemManager webItemManager,
        TenantManager tenantManager,
        AuthorizationManager authorizationManager,
        CoreBaseSettings coreBaseSettings,
        WebItemSecurityCache webItemSecurityCache,
        SettingsManager settingsManager,
        CountPaidUserChecker countPaidUserChecker)
    {
        _userManager = userManager;
        _authContext = authContext;
        _permissionContext = permissionContext;
        _authentication = authentication;
        _webItemManager = webItemManager;
        _tenantManager = tenantManager;
        _authorizationManager = authorizationManager;
        _coreBaseSettings = coreBaseSettings;
        _webItemSecurityCache = webItemSecurityCache;
        _settingsManager = settingsManager;
        _countPaidUserChecker = countPaidUserChecker;
    }

    //
    public async Task<bool> IsAvailableForMeAsync(Guid id)
    {
        return await IsAvailableForUserAsync(id, _authContext.CurrentAccount.ID);
    }

    public async Task<bool> IsAvailableForUserAsync(Guid itemId, Guid @for)
    {
        var tenant = await _tenantManager.GetCurrentTenantAsync();

        var id = itemId.ToString();
        bool result;
        var dic = _webItemSecurityCache.GetOrInsert(tenant.Id);
        if (dic != null)
        {
            lock (dic)
            {
                if (dic.TryGetValue(id + @for, out var value))
                {
                    return value;
                }
            }
        }

        // can read or administrator
        var securityObj = WebItemSecurityObject.Create(id, _webItemManager);

        if (_coreBaseSettings.Personal
            && securityObj.WebItemId != WebItemManager.DocumentsProductID)
        {
            // only files visible in your-docs portal
            result = false;
        }
        else
        {
            var webitem = _webItemManager[securityObj.WebItemId];
            if (webitem != null)
            {
                if ((webitem.ID == WebItemManager.CRMProductID ||
                    webitem.ID == WebItemManager.PeopleProductID ||
                    webitem.ID == WebItemManager.BirthdaysProductID ||
                    webitem.ID == WebItemManager.MailProductID) &&
                    await _userManager.IsUserAsync(@for))
                {
                    // hack: crm, people, birtthday and mail products not visible for collaborators
                    result = false;
                }
                else if ((webitem.ID == WebItemManager.CalendarProductID ||
                          webitem.ID == WebItemManager.TalkProductID) &&
                         await _userManager.IsOutsiderAsync(@for))
                {
                    // hack: calendar and talk products not visible for outsider
                    result = false;
                }
                else if (webitem is IModule)
                {
                    result = await _permissionContext.PermissionResolver.CheckAsync(await _authentication.GetAccountByIDAsync(tenant.Id, @for), securityObj, null, _read) &&
                        await IsAvailableForUserAsync(_webItemManager.GetParentItemID(webitem.ID), @for);
                }
                else
                {
                    var hasUsers = (await _authorizationManager.GetAcesAsync(Guid.Empty, _read.ID, securityObj)).Any(a => a.Subject != ASC.Core.Users.Constants.GroupEveryone.ID);
                    result = await _permissionContext.PermissionResolver.CheckAsync(await _authentication.GetAccountByIDAsync(tenant.Id, @for), securityObj, null, _read) ||
                             (hasUsers && await IsProductAdministratorAsync(securityObj.WebItemId, @for));
                }
            }
            else
            {
                result = false;
            }
        }

        dic = _webItemSecurityCache.Get(tenant.Id);
        if (dic != null)
        {
            lock (dic)
            {
                dic[id + @for] = result;
            }
        }
        return result;
    }

    public async Task SetSecurityAsync(string id, bool enabled, params Guid[] subjects)
    {
        if ((await _settingsManager.LoadAsync<TenantAccessSettings>()).Anyone)
        {
            throw new SecurityException("Security settings are disabled for an open portal");
        }

        var securityObj = WebItemSecurityObject.Create(id, _webItemManager);

        // remove old aces
        await _authorizationManager.RemoveAllAcesAsync(securityObj);
        var allowToAll = new AzRecord(ASC.Core.Users.Constants.GroupEveryone.ID, _read.ID, AceType.Allow, securityObj.FullId);
        await _authorizationManager.RemoveAceAsync(allowToAll);

        // set new aces
        if (subjects == null || subjects.Length == 0 || subjects.Contains(ASC.Core.Users.Constants.GroupEveryone.ID))
        {
            if (!enabled && subjects != null && subjects.Length == 0)
            {
                // users from list with no users equals allow to all users
                enabled = true;
            }
            subjects = new[] { ASC.Core.Users.Constants.GroupEveryone.ID };
        }
        foreach (var s in subjects)
        {
            var a = new AzRecord(s, _read.ID, enabled ? AceType.Allow : AceType.Deny, securityObj.FullId);
            await _authorizationManager.AddAceAsync(a);
        }

        _webItemSecurityCache.Publish(await _tenantManager.GetCurrentTenantIdAsync());
    }

    public async Task<WebItemSecurityInfo> GetSecurityInfoAsync(string id)
    {
        var info = (await GetSecurityAsync(id)).ToList();
        var module = _webItemManager.GetParentItemID(new Guid(id)) != Guid.Empty;

        var infoAsync = info.ToAsyncEnumerable();
        return new WebItemSecurityInfo
        {
            WebItemId = id,

            Enabled = info.Count == 0 || (!module && info.Any(i => i.Item2)) || (module && info.All(i => i.Item2)),

            Users = await infoAsync
                           .SelectAwait(async i => await _userManager.GetUsersAsync(i.Item1))
                           .Where(u => u.Id != ASC.Core.Users.Constants.LostUser.Id).ToListAsync(),

            Groups = await infoAsync
                           .SelectAwait(async i => await _userManager.GetGroupInfoAsync(i.Item1))
                           .Where(g => g.ID != ASC.Core.Users.Constants.LostGroupInfo.ID && g.CategoryID != ASC.Core.Users.Constants.SysGroupCategoryId).ToListAsync()
        };
    }


    private async Task<IEnumerable<Tuple<Guid, bool>>> GetSecurityAsync(string id)
    {
        var securityObj = WebItemSecurityObject.Create(id, _webItemManager);
        var result = (await _authorizationManager
            .GetAcesWithInheritsAsync(Guid.Empty, _read.ID, securityObj, null))
            .GroupBy(a => a.Subject)
            .Select(a => Tuple.Create(a.Key, a.First().AceType == AceType.Allow))
            .ToList();
        if (result.Count == 0)
        {
            result.Add(Tuple.Create(ASC.Core.Users.Constants.GroupEveryone.ID, false));
        }
        return result;
    }

    public async Task SetProductAdministrator(Guid productid, Guid userid, bool administrator)
    {
        if (productid == Guid.Empty)
        {
            productid = ASC.Core.Users.Constants.GroupAdmin.ID;
        }

        if (administrator)
        {
            if (await _userManager.IsUserInGroupAsync(userid, ASC.Core.Users.Constants.GroupUser.ID))
            {
                await _countPaidUserChecker.CheckAppend();
                await _userManager.RemoveUserFromGroupAsync(userid, ASC.Core.Users.Constants.GroupUser.ID);
            }

            if (productid == WebItemManager.PeopleProductID)
            {
                foreach (var ace in GetPeopleModuleActions(userid))
                {
                    await _authorizationManager.AddAceAsync(ace);
                }
            }

            await _userManager.AddUserIntoGroupAsync(userid, productid);
        }
        else
        {
            if (productid == ASC.Core.Users.Constants.GroupAdmin.ID)
            {
                var groups = new List<Guid> { WebItemManager.MailProductID };
                groups.AddRange(_webItemManager.GetItemsAll().OfType<IProduct>().Select(p => p.ID));

                foreach (var id in groups)
                {
                    await _userManager.RemoveUserFromGroupAsync(userid, id);
                }
            }

            if (productid == ASC.Core.Users.Constants.GroupAdmin.ID || productid == WebItemManager.PeopleProductID)
            {
                foreach (var ace in GetPeopleModuleActions(userid))
                {
                    await _authorizationManager.RemoveAceAsync(ace);
                }
            }

            await _userManager.RemoveUserFromGroupAsync(userid, productid);
        }

        _webItemSecurityCache.Publish(await _tenantManager.GetCurrentTenantIdAsync());
    }

    public async Task<bool> IsProductAdministratorAsync(Guid productid, Guid userid)
    {
        return await _userManager.IsUserInGroupAsync(userid, ASC.Core.Users.Constants.GroupAdmin.ID) ||
               await _userManager.IsUserInGroupAsync(userid, productid);
    }

    public async Task<IEnumerable<UserInfo>> GetProductAdministratorsAsync(Guid productid)
    {
        var groups = new List<Guid>();
        if (productid == Guid.Empty)
        {
            groups.Add(ASC.Core.Users.Constants.GroupAdmin.ID);
            groups.AddRange(_webItemManager.GetItemsAll().OfType<IProduct>().Select(p => p.ID));
            groups.Add(WebItemManager.MailProductID);
        }
        else
        {
            groups.Add(productid);
        }

        var users = Enumerable.Empty<UserInfo>();
        foreach (var id in groups)
        {
            users = users.Union(await _userManager.GetUsersByGroupAsync(id));
        }
        return users.ToList();
    }

    private static IEnumerable<AzRecord> GetPeopleModuleActions(Guid userid)
    {
        return new List<Guid>
                {
                    ASC.Core.Users.Constants.Action_AddRemoveUser.ID,
                    ASC.Core.Users.Constants.Action_EditUser.ID,
                    ASC.Core.Users.Constants.Action_EditGroups.ID
                }.Select(action => new AzRecord(userid, action, AceType.Allow));
    }

    private class WebItemSecurityObject : ISecurityObject
    {
        public Guid WebItemId { get; private set; }
        private readonly WebItemManager _webItemManager;

        public Type ObjectType
        {
            get { return GetType(); }
        }

        public object SecurityId
        {
            get { return WebItemId.ToString("N"); }
        }

        public string FullId => AzObjectIdHelper.GetFullObjectId(this);

        public bool InheritSupported
        {
            get { return true; }
        }

        public bool ObjectRolesSupported
        {
            get { return false; }
        }

        public static WebItemSecurityObject Create(string id, WebItemManager webItemManager)
        {
            ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(id);

            var itemId = Guid.Empty;
            if (32 <= id.Length)
            {
                itemId = new Guid(id);
            }
            else
            {
                var w = webItemManager
                    .GetItemsAll()
                    .FirstOrDefault(i => id.Equals(i.GetSysName(), StringComparison.InvariantCultureIgnoreCase));
                if (w != null)
                {
                    itemId = w.ID;
                }
            }
            return new WebItemSecurityObject(itemId, webItemManager);
        }


        private WebItemSecurityObject(Guid itemId, WebItemManager webItemManager)
        {
            WebItemId = itemId;
            _webItemManager = webItemManager;
        }

        public ISecurityObjectId InheritFrom(ISecurityObjectId objectId)
        {
            if (objectId is WebItemSecurityObject s)
            {
                return Create(_webItemManager.GetParentItemID(s.WebItemId).ToString("N"), _webItemManager) is WebItemSecurityObject parent && parent.WebItemId != s.WebItemId && parent.WebItemId != Guid.Empty ? parent : null;
            }
            return null;
        }

        public IEnumerable<IRole> GetObjectRoles(ISubject account, ISecurityObjectId objectId, SecurityCallContext callContext)
        {
            throw new NotImplementedException();
        }
    }
}
