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
using System.Collections.Generic;
using System.Linq;
using System.Security;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Users;
using ASC.Web.Core.Utility.Settings;

using SecurityAction = ASC.Common.Security.Authorizing.Action;

namespace ASC.Web.Core
{
    [Singletone]
    public class WebItemSecurityCache
    {
        private ICache Cache { get; }
        private ICacheNotify<WebItemSecurityNotifier> CacheNotify { get; }

        public WebItemSecurityCache(ICacheNotify<WebItemSecurityNotifier> cacheNotify, ICache cache)
        {
            Cache = cache;
            CacheNotify = cacheNotify;
            CacheNotify.Subscribe((r) =>
            {
                ClearCache(r.Tenant);
            }, CacheNotifyAction.Any);
        }

        public void ClearCache(int tenantId)
        {
            Cache.Remove(GetCacheKey(tenantId));
        }

        public string GetCacheKey(int tenantId)
        {
            return $"{tenantId}:webitemsecurity";
        }

        public void Publish(int tenantId)
        {
            CacheNotify.Publish(new WebItemSecurityNotifier { Tenant = tenantId }, CacheNotifyAction.Any);
        }

        public Dictionary<string, bool> Get(int tenantId)
        {
            return Cache.Get<Dictionary<string, bool>>(GetCacheKey(tenantId));
        }

        public Dictionary<string, bool> GetOrInsert(int tenantId)
        {

            var dic = Get(tenantId);
            if (dic == null)
            {
                dic = new Dictionary<string, bool>();
                Cache.Insert(GetCacheKey(tenantId), dic, DateTime.UtcNow.Add(TimeSpan.FromMinutes(1)));
            }

            return dic;
        }
    }

    [Scope]
    public class WebItemSecurity
    {
        private static readonly SecurityAction Read = new SecurityAction(new Guid("77777777-32ae-425f-99b5-83176061d1ae"), "ReadWebItem", false, true);

        private UserManager UserManager { get; }
        private AuthContext AuthContext { get; }
        private PermissionContext PermissionContext { get; }
        private AuthManager Authentication { get; }
        public WebItemManager WebItemManager { get; }
        private TenantManager TenantManager { get; }
        private AuthorizationManager AuthorizationManager { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private WebItemSecurityCache WebItemSecurityCache { get; }
        private SettingsManager SettingsManager { get; }

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
            SettingsManager settingsManager)
        {
            UserManager = userManager;
            AuthContext = authContext;
            PermissionContext = permissionContext;
            Authentication = authentication;
            WebItemManager = webItemManager;
            TenantManager = tenantManager;
            AuthorizationManager = authorizationManager;
            CoreBaseSettings = coreBaseSettings;
            WebItemSecurityCache = webItemSecurityCache;
            SettingsManager = settingsManager;
        }

        //
        public bool IsAvailableForMe(Guid id)
        {
            return IsAvailableForUser(id, AuthContext.CurrentAccount.ID);
        }

        public bool IsAvailableForUser(Guid itemId, Guid @for)
        {
            var id = itemId.ToString();
            bool result;

            var tenant = TenantManager.GetCurrentTenant();
            var dic = WebItemSecurityCache.GetOrInsert(tenant.TenantId);
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
            var securityObj = WebItemSecurityObject.Create(id, WebItemManager);

            if (CoreBaseSettings.Personal
                && securityObj.WebItemId != WebItemManager.DocumentsProductID)
            {
                // only files visible in your-docs portal
                result = false;
            }
            else
            {
                var webitem = WebItemManager[securityObj.WebItemId];
                if (webitem != null)
                {
                    if ((webitem.ID == WebItemManager.CRMProductID ||
                        webitem.ID == WebItemManager.PeopleProductID ||
                        webitem.ID == WebItemManager.BirthdaysProductID ||
                        webitem.ID == WebItemManager.MailProductID) &&
                        UserManager.GetUsers(@for).IsVisitor(UserManager))
                    {
                        // hack: crm, people, birtthday and mail products not visible for collaborators
                        result = false;
                    }
                    else if ((webitem.ID == WebItemManager.CalendarProductID ||
                              webitem.ID == WebItemManager.TalkProductID) &&
                             UserManager.GetUsers(@for).IsOutsider(UserManager))
                    {
                        // hack: calendar and talk products not visible for outsider
                        result = false;
                    }
                    else if (webitem is IModule)
                    {
                        result = PermissionContext.PermissionResolver.Check(Authentication.GetAccountByID(tenant.TenantId, @for), securityObj, null, Read) &&
                            IsAvailableForUser(WebItemManager.GetParentItemID(webitem.ID), @for);
                    }
                    else
                    {
                        var hasUsers = AuthorizationManager.GetAces(Guid.Empty, Read.ID, securityObj).Any(a => a.SubjectId != ASC.Core.Users.Constants.GroupEveryone.ID);
                        result = PermissionContext.PermissionResolver.Check(Authentication.GetAccountByID(tenant.TenantId, @for), securityObj, null, Read) ||
                                 (hasUsers && IsProductAdministrator(securityObj.WebItemId, @for));
                    }
                }
                else
                {
                    result = false;
                }
            }

            dic = WebItemSecurityCache.Get(tenant.TenantId);
            if (dic != null)
            {
                lock (dic)
                {
                    dic[id + @for] = result;
                }
            }
            return result;
        }

        public void SetSecurity(string id, bool enabled, params Guid[] subjects)
        {
            if (SettingsManager.Load<TenantAccessSettings>().Anyone)
                throw new SecurityException("Security settings are disabled for an open portal");

            var securityObj = WebItemSecurityObject.Create(id, WebItemManager);

            // remove old aces
            AuthorizationManager.RemoveAllAces(securityObj);
            var allowToAll = new AzRecord(ASC.Core.Users.Constants.GroupEveryone.ID, Read.ID, AceType.Allow, securityObj);
            AuthorizationManager.RemoveAce(allowToAll);

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
                var a = new AzRecord(s, Read.ID, enabled ? AceType.Allow : AceType.Deny, securityObj);
                AuthorizationManager.AddAce(a);
            }

            WebItemSecurityCache.Publish(TenantManager.GetCurrentTenant().TenantId);
        }

        public WebItemSecurityInfo GetSecurityInfo(string id)
        {
            var info = GetSecurity(id).ToList();
            var module = WebItemManager.GetParentItemID(new Guid(id)) != Guid.Empty;
            return new WebItemSecurityInfo
            {
                WebItemId = id,

                Enabled = info.Count == 0 || (!module && info.Any(i => i.Item2)) || (module && info.All(i => i.Item2)),

                Users = info
                               .Select(i => UserManager.GetUsers(i.Item1))
                               .Where(u => u.ID != ASC.Core.Users.Constants.LostUser.ID),

                Groups = info
                               .Select(i => UserManager.GetGroupInfo(i.Item1))
                               .Where(g => g.ID != ASC.Core.Users.Constants.LostGroupInfo.ID && g.CategoryID != ASC.Core.Users.Constants.SysGroupCategoryId)
            };
        }

        private IEnumerable<Tuple<Guid, bool>> GetSecurity(string id)
        {
            var securityObj = WebItemSecurityObject.Create(id, WebItemManager);
            var result = AuthorizationManager
                .GetAcesWithInherits(Guid.Empty, Read.ID, securityObj, null)
                .GroupBy(a => a.SubjectId)
                .Select(a => Tuple.Create(a.Key, a.First().Reaction == AceType.Allow))
                .ToList();
            if (result.Count == 0)
            {
                result.Add(Tuple.Create(ASC.Core.Users.Constants.GroupEveryone.ID, false));
            }
            return result;
        }

        public void SetProductAdministrator(Guid productid, Guid userid, bool administrator)
        {
            if (productid == Guid.Empty)
            {
                productid = ASC.Core.Users.Constants.GroupAdmin.ID;
            }
            if (administrator)
            {
                if (UserManager.IsUserInGroup(userid, ASC.Core.Users.Constants.GroupVisitor.ID))
                {
                    throw new SecurityException("Collaborator can not be an administrator");
                }

                if (productid == WebItemManager.PeopleProductID)
                {
                    foreach (var ace in GetPeopleModuleActions(userid))
                    {
                        AuthorizationManager.AddAce(ace);
                    }
                }

                UserManager.AddUserIntoGroup(userid, productid);
            }
            else
            {
                if (productid == ASC.Core.Users.Constants.GroupAdmin.ID)
                {
                    var groups = new List<Guid> { WebItemManager.MailProductID };
                    groups.AddRange(WebItemManager.GetItemsAll().OfType<IProduct>().Select(p => p.ID));

                    foreach (var id in groups)
                    {
                        UserManager.RemoveUserFromGroup(userid, id);
                    }
                }

                if (productid == ASC.Core.Users.Constants.GroupAdmin.ID || productid == WebItemManager.PeopleProductID)
                {
                    foreach (var ace in GetPeopleModuleActions(userid))
                    {
                        AuthorizationManager.RemoveAce(ace);
                    }
                }

                UserManager.RemoveUserFromGroup(userid, productid);
            }

            WebItemSecurityCache.Publish(TenantManager.GetCurrentTenant().TenantId);
        }

        public bool IsProductAdministrator(Guid productid, Guid userid)
        {
            return UserManager.IsUserInGroup(userid, ASC.Core.Users.Constants.GroupAdmin.ID) ||
                   UserManager.IsUserInGroup(userid, productid);
        }

        public IEnumerable<UserInfo> GetProductAdministrators(Guid productid)
        {
            var groups = new List<Guid>();
            if (productid == Guid.Empty)
            {
                groups.Add(ASC.Core.Users.Constants.GroupAdmin.ID);
                groups.AddRange(WebItemManager.GetItemsAll().OfType<IProduct>().Select(p => p.ID));
                groups.Add(WebItemManager.MailProductID);
            }
            else
            {
                groups.Add(productid);
            }

            var users = Enumerable.Empty<UserInfo>();
            foreach (var id in groups)
            {
                users = users.Union(UserManager.GetUsersByGroup(id));
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
            private WebItemManager WebItemManager { get; }

            public Type ObjectType
            {
                get { return GetType(); }
            }

            public object SecurityId
            {
                get { return WebItemId.ToString("N"); }
            }

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
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentNullException(nameof(id));
                }

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
                    if (w != null) itemId = w.ID;
                }
                return new WebItemSecurityObject(itemId, webItemManager);
            }


            private WebItemSecurityObject(Guid itemId, WebItemManager webItemManager)
            {
                WebItemId = itemId;
                WebItemManager = webItemManager;
            }

            public ISecurityObjectId InheritFrom(ISecurityObjectId objectId)
            {
                if (objectId is WebItemSecurityObject s)
                {
                    return Create(WebItemManager.GetParentItemID(s.WebItemId).ToString("N"), WebItemManager) is WebItemSecurityObject parent && parent.WebItemId != s.WebItemId && parent.WebItemId != Guid.Empty ? parent : null;
                }
                return null;
            }

            public IEnumerable<IRole> GetObjectRoles(ISubject account, ISecurityObjectId objectId, SecurityCallContext callContext)
            {
                throw new NotImplementedException();
            }
        }
    }
}