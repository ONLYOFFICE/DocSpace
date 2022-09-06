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

namespace ASC.FederatedLogin;

[Singletone]
public class AccountLinkerStorage
{
    private readonly ICache _cache;
    private readonly ICacheNotify<LinkerCacheItem> _notify;

    public AccountLinkerStorage(ICacheNotify<LinkerCacheItem> notify, ICache cache)
    {
        _cache = cache;
        _notify = notify;
        notify.Subscribe((c) => cache.Remove(c.Obj), CacheNotifyAction.Remove);
    }

    public void RemoveFromCache(string obj)
    {
        _notify.Publish(new LinkerCacheItem { Obj = obj }, CacheNotifyAction.Remove);
    }

    public List<LoginProfile> GetFromCache(string obj, Func<string, List<LoginProfile>> fromDb)
    {
        var profiles = _cache.Get<List<LoginProfile>>(obj);
        if (profiles == null)
        {
            profiles = fromDb(obj);
            _cache.Insert(obj, profiles, DateTime.UtcNow + TimeSpan.FromMinutes(10));
        }

        return profiles;
    }
}

[Scope]
public class AccountLinker
{
    private readonly Signature _signature;
    private readonly InstanceCrypto _instanceCrypto;
    private readonly AccountLinkerStorage _accountLinkerStorage;
    private readonly IDbContextFactory<AccountLinkContext> _accountLinkContextManager;

    public AccountLinker(Signature signature, InstanceCrypto instanceCrypto, AccountLinkerStorage accountLinkerStorage, IDbContextFactory<AccountLinkContext> accountLinkContextManager)
    {
        _signature = signature;
        _instanceCrypto = instanceCrypto;
        _accountLinkerStorage = accountLinkerStorage;
        _accountLinkContextManager = accountLinkContextManager;
    }

    public IEnumerable<string> GetLinkedObjects(string id, string provider)
    {
        return GetLinkedObjects(new LoginProfile(_signature, _instanceCrypto) { Id = id, Provider = provider });
    }

    public IEnumerable<string> GetLinkedObjects(LoginProfile profile)
    {
        return GetLinkedObjectsByHashId(profile.HashId);
    }

    public IEnumerable<string> GetLinkedObjectsByHashId(string hashid)
    {
        using var accountLinkContext = _accountLinkContextManager.CreateDbContext();
        return accountLinkContext.AccountLinks
            .Where(r => r.UId == hashid)
            .Where(r => r.Provider != string.Empty)
            .Select(r => r.Id)
            .ToList();
    }

    public IEnumerable<LoginProfile> GetLinkedProfiles(string obj, string provider)
    {
        return GetLinkedProfiles(obj).Where(profile => profile.Provider.Equals(provider));
    }

    public IDictionary<string, LoginProfile> GetLinkedProfiles(IEnumerable<string> objects, string provider)
    {
        return GetLinkedProfiles(objects).Where(o => o.Value.Provider.Equals(provider)).ToDictionary(k => k.Key, v => v.Value);
    }

    public IEnumerable<LoginProfile> GetLinkedProfiles(string obj)
    {
        return _accountLinkerStorage.GetFromCache(obj, GetLinkedProfilesFromDB);
    }

    public void AddLink(string obj, LoginProfile profile)
    {
        var accountLink = new AccountLinks
        {
            Id = obj,
            UId = profile.HashId,
            Provider = profile.Provider,
            Profile = profile.ToSerializedString(),
            Linked = DateTime.UtcNow
        };

        using var accountLinkContext = _accountLinkContextManager.CreateDbContext();
        accountLinkContext.AddOrUpdate(r => r.AccountLinks, accountLink);
        accountLinkContext.SaveChanges();

        _accountLinkerStorage.RemoveFromCache(obj);
    }

    public void AddLink(string obj, string id, string provider)
    {
        AddLink(obj, new LoginProfile(_signature, _instanceCrypto) { Id = id, Provider = provider });
    }

    public void RemoveLink(string obj, string id, string provider)
    {
        RemoveLink(obj, new LoginProfile(_signature, _instanceCrypto) { Id = id, Provider = provider });
    }

    public void RemoveLink(string obj, LoginProfile profile)
    {
        RemoveProvider(obj, hashId: profile.HashId);
    }

    public void RemoveProvider(string obj, string provider = null, string hashId = null)
    {
        using var accountLinkContext = _accountLinkContextManager.CreateDbContext();
        var strategy = accountLinkContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var accountLinkContext = _accountLinkContextManager.CreateDbContext();
            using var tr = accountLinkContext.Database.BeginTransaction();

            var accountLinkQuery = accountLinkContext.AccountLinks
                .Where(r => r.Id == obj);

            if (!string.IsNullOrEmpty(provider))
            {
                accountLinkQuery = accountLinkQuery.Where(r => r.Provider == provider);
            }

            if (!string.IsNullOrEmpty(hashId))
            {
                accountLinkQuery = accountLinkQuery.Where(r => r.UId == hashId);
            }

            var accountLink = accountLinkQuery.FirstOrDefault();
            accountLinkContext.AccountLinks.Remove(accountLink);
            accountLinkContext.SaveChanges();

            tr.Commit();
        });



        _accountLinkerStorage.RemoveFromCache(obj);
    }

    private List<LoginProfile> GetLinkedProfilesFromDB(string obj)
    {
        using var accountLinkContext = _accountLinkContextManager.CreateDbContext();

        //Retrieve by uinque id
        return accountLinkContext.AccountLinks
                .Where(r => r.Id == obj)
                .Select(r => r.Profile)
                .ToList()
                .ConvertAll(x => LoginProfile.CreateFromSerializedString(_signature, _instanceCrypto, x));
    }

    private IDictionary<string, LoginProfile> GetLinkedProfiles(IEnumerable<string> objects)
    {
        using var accountLinkContext = _accountLinkContextManager.CreateDbContext();

        return accountLinkContext.AccountLinks.Where(r => objects.Contains(r.Id))
            .Select(r => new { r.Id, r.Profile })
            .ToDictionary(k => k.Id, v => LoginProfile.CreateFromSerializedString(_signature, _instanceCrypto, v.Profile));
    }
}
