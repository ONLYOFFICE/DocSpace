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
public class ConfigureAccountLinker : IConfigureNamedOptions<AccountLinker>
{
    private readonly Signature _signature;
    private readonly InstanceCrypto _instanceCrypto;
    private readonly AccountLinkerStorage _accountLinkerStorage;
    private readonly DbContextManager<AccountLinkContext> _dbContextManager;

    public ConfigureAccountLinker(
        Signature signature,
        InstanceCrypto instanceCrypto,
        AccountLinkerStorage accountLinkerStorage,
        DbContextManager<AccountLinkContext> dbContextManager)
    {
        _signature = signature;
        _instanceCrypto = instanceCrypto;
        _accountLinkerStorage = accountLinkerStorage;
        _dbContextManager = dbContextManager;
    }

    public void Configure(string name, AccountLinker options)
    {
        options.DbId = name;
        options.AccountLinkerStorage = _accountLinkerStorage;
        options.InstanceCrypto = _instanceCrypto;
        options.Signature = _signature;
        options.AccountLinkContextManager = _dbContextManager;
    }

    public void Configure(AccountLinker options)
    {
        Configure("default", options);
    }
}

[Scope(typeof(ConfigureAccountLinker))]
public class AccountLinker
{
    public string DbId { get; set; }
    public AccountLinkContext AccountLinkContext => AccountLinkContextManager.Get(DbId);
    public DbSet<AccountLinks> AccountLinks => AccountLinkContext.AccountLinks;
    internal Signature Signature { get; set; }
    internal InstanceCrypto InstanceCrypto { get; set; }
    internal AccountLinkerStorage AccountLinkerStorage { get; set; }
    internal DbContextManager<AccountLinkContext> AccountLinkContextManager { get; set; }

    public IEnumerable<string> GetLinkedObjects(string id, string provider)
    {
        return GetLinkedObjects(new LoginProfile(Signature, InstanceCrypto) { Id = id, Provider = provider });
    }

    public IEnumerable<string> GetLinkedObjects(LoginProfile profile)
    {
        return GetLinkedObjectsByHashId(profile.HashId);
    }

    public IEnumerable<string> GetLinkedObjectsByHashId(string hashid)
    {
        return AccountLinks
            .Where(r => r.UId == hashid)
            .Where(r => r.Provider != string.Empty)
            .Select(r => r.Id)
            .ToList();
    }

    public IEnumerable<LoginProfile> GetLinkedProfiles(string obj, string provider)
    {
        return GetLinkedProfiles(obj).Where(profile => profile.Provider.Equals(provider));
    }

    public IEnumerable<LoginProfile> GetLinkedProfiles(string obj)
    {
        return AccountLinkerStorage.GetFromCache(obj, GetLinkedProfilesFromDB);
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

        AccountLinkContext.AddOrUpdate(r => r.AccountLinks, accountLink);
        AccountLinkContext.SaveChanges();

        AccountLinkerStorage.RemoveFromCache(obj);
    }

    public void AddLink(string obj, string id, string provider)
    {
        AddLink(obj, new LoginProfile(Signature, InstanceCrypto) { Id = id, Provider = provider });
    }

    public void RemoveLink(string obj, string id, string provider)
    {
        RemoveLink(obj, new LoginProfile(Signature, InstanceCrypto) { Id = id, Provider = provider });
    }

    public void RemoveLink(string obj, LoginProfile profile)
    {
        RemoveProvider(obj, hashId: profile.HashId);
    }

    public void RemoveProvider(string obj, string provider = null, string hashId = null)
    {
        var strategy = AccountLinkContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var tr = AccountLinkContext.Database.BeginTransaction();

            var accountLinkQuery = AccountLinks
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
            AccountLinks.Remove(accountLink);
            AccountLinkContext.SaveChanges();

            tr.Commit();
        });



        AccountLinkerStorage.RemoveFromCache(obj);
    }

    private List<LoginProfile> GetLinkedProfilesFromDB(string obj)
    {
        //Retrieve by uinque id
        return AccountLinks
                .Where(r => r.Id == obj)
                .Select(r => r.Profile)
                .ToList()
                .ConvertAll(x => LoginProfile.CreateFromSerializedString(Signature, InstanceCrypto, x));
    }
}
