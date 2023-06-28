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

    public async Task<List<LoginProfile>> GetFromCacheAsync(string obj, Func<string, Task<List<LoginProfile>>> fromDb)
    {
        var profiles = _cache.Get<List<LoginProfile>>(obj);
        if (profiles == null)
        {
            profiles = await fromDb(obj);
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

    public async Task<IEnumerable<string>> GetLinkedObjectsAsync(string id, string provider)
    {
        return await GetLinkedObjectsAsync(new LoginProfile(_signature, _instanceCrypto) { Id = id, Provider = provider });
    }

    public async Task<IEnumerable<string>> GetLinkedObjectsAsync(LoginProfile profile)
    {
        return await GetLinkedObjectsByHashIdAsync(profile.HashId);
    }

    public async Task<IEnumerable<string>> GetLinkedObjectsByHashIdAsync(string hashid)
    {
        await using var accountLinkContext = _accountLinkContextManager.CreateDbContext();
        return await Queries.LinkedObjectsByHashIdAsync(accountLinkContext, hashid).ToListAsync();
    }

    public async Task<IEnumerable<LoginProfile>> GetLinkedProfilesAsync(string obj, string provider)
    {
        return (await GetLinkedProfilesAsync(obj)).Where(profile => profile.Provider.Equals(provider));
    }

    public async Task<IDictionary<string, LoginProfile>> GetLinkedProfilesAsync(IEnumerable<string> objects, string provider)
    {
        return (await GetLinkedProfilesAsync(objects)).Where(o => o.Value.Provider.Equals(provider)).ToDictionary(k => k.Key, v => v.Value);
    }

    public async Task<IEnumerable<LoginProfile>> GetLinkedProfilesAsync(string obj)
    {
        return await _accountLinkerStorage.GetFromCacheAsync(obj, GetLinkedProfilesFromDBAsync);
    }

    public async Task AddLinkAsync(string obj, LoginProfile profile)
    {
        var accountLink = new AccountLinks
        {
            Id = obj,
            UId = profile.HashId,
            Provider = profile.Provider,
            Profile = profile.ToSerializedString(),
            Linked = DateTime.UtcNow
        };

        await using var accountLinkContext = _accountLinkContextManager.CreateDbContext();
        await accountLinkContext.AddOrUpdateAsync(a => a.AccountLinks, accountLink);
        await accountLinkContext.SaveChangesAsync();

        _accountLinkerStorage.RemoveFromCache(obj);
    }

    public async Task AddLinkAsync(string obj, string id, string provider)
    {
        await AddLinkAsync(obj, new LoginProfile(_signature, _instanceCrypto) { Id = id, Provider = provider });
    }

    public async Task RemoveLinkAsync(string obj, string id, string provider)
    {
        await RemoveLinkAsync(obj, new LoginProfile(_signature, _instanceCrypto) { Id = id, Provider = provider });
    }

    public async Task RemoveLinkAsync(string obj, LoginProfile profile)
    {
        await RemoveProviderAsync(obj, hashId: profile.HashId);
    }

    public async Task RemoveProviderAsync(string obj, string provider = null, string hashId = null)
    {
        await using var accountLinkContext = _accountLinkContextManager.CreateDbContext();

        var accountLink = await Queries.AccountLinkAsync(accountLinkContext, obj, provider, hashId);

        accountLinkContext.AccountLinks.Remove(accountLink);
        await accountLinkContext.SaveChangesAsync();

        _accountLinkerStorage.RemoveFromCache(obj);
    }

    private async Task<List<LoginProfile>> GetLinkedProfilesFromDBAsync(string obj)
    {
        await using var accountLinkContext = _accountLinkContextManager.CreateDbContext();
        //Retrieve by uinque id
        return (await Queries.LinkedProfilesFromDbAsync(accountLinkContext, obj).ToListAsync())
                .ConvertAll(x => LoginProfile.CreateFromSerializedString(_signature, _instanceCrypto, x));
    }

    private async Task<IDictionary<string, LoginProfile>> GetLinkedProfilesAsync(IEnumerable<string> objects)
    {
        await using var accountLinkContext = _accountLinkContextManager.CreateDbContext();

        return await accountLinkContext.AccountLinks.Where(r => objects.Contains(r.Id))
            .Select(r => new { r.Id, r.Profile })
            .ToDictionaryAsync(k => k.Id, v => LoginProfile.CreateFromSerializedString(_signature, _instanceCrypto, v.Profile));
    }
}

static file class Queries
{
    public static readonly Func<AccountLinkContext, string, IAsyncEnumerable<string>> LinkedObjectsByHashIdAsync =
        EF.CompileAsyncQuery(
            (AccountLinkContext ctx, string hashId) =>
                ctx.AccountLinks
                    .Where(r => r.UId == hashId)
                    .Where(r => r.Provider != string.Empty)
                    .Select(r => r.Id));

    public static readonly Func<AccountLinkContext, string, string, string, Task<AccountLinks>> AccountLinkAsync =
        EF.CompileAsyncQuery(
            (AccountLinkContext ctx, string id, string provider, string hashId) =>
                ctx.AccountLinks
                    .Where(r => r.Id == id)
                    .Where(r => string.IsNullOrEmpty(provider) || r.Provider == provider)
                    .Where(r => string.IsNullOrEmpty(hashId) || r.UId == hashId)
                    .FirstOrDefault());

    public static readonly Func<AccountLinkContext, string, IAsyncEnumerable<string>> LinkedProfilesFromDbAsync =
        EF.CompileAsyncQuery(
            (AccountLinkContext ctx, string id) =>
                ctx.AccountLinks
                    .Where(r => r.Id == id)
                    .Select(r => r.Profile));
}
