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

using ASC.Common.Caching;
using ASC.Common.Utils;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.EF.Model;
using ASC.FederatedLogin.Profile;
using ASC.Security.Cryptography;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ASC.FederatedLogin
{
    public class AccountLinkerStorage
    {
        private readonly ICache cache;
        private readonly ICacheNotify<LinkerCacheItem> notify;

        public AccountLinkerStorage(ICacheNotify<LinkerCacheItem> notify)
        {
            cache = AscCache.Memory;
            this.notify = notify;
            notify.Subscribe((c) => cache.Remove(c.Obj), CacheNotifyAction.Remove);
        }

        public void RemoveFromCache(string obj)
        {
            notify.Publish(new LinkerCacheItem { Obj = obj }, CacheNotifyAction.Remove);
        }
        public List<LoginProfile> GetFromCache(string obj, Func<string, List<LoginProfile>> fromDb)
        {
            var profiles = cache.Get<List<LoginProfile>>(obj);
            if (profiles == null)
            {
                cache.Insert(obj, profiles = fromDb(obj), DateTime.UtcNow + TimeSpan.FromMinutes(10));
            }
            return profiles;
        }
    }
    public class ConfigureAccountLinker : IConfigureNamedOptions<AccountLinker>
    {
        public Signature Signature { get; }
        public InstanceCrypto InstanceCrypto { get; }
        public AccountLinkerStorage AccountLinkerStorage { get; }
        public DbContextManager<AccountLinkContext> DbContextManager { get; }

        public ConfigureAccountLinker(
            Signature signature,
            InstanceCrypto instanceCrypto,
            AccountLinkerStorage accountLinkerStorage,
            DbContextManager<AccountLinkContext> dbContextManager)
        {
            Signature = signature;
            InstanceCrypto = instanceCrypto;
            AccountLinkerStorage = accountLinkerStorage;
            DbContextManager = dbContextManager;
        }

        public void Configure(string name, AccountLinker options)
        {
            options.DbId = name;
            options.AccountLinkerStorage = AccountLinkerStorage;
            options.InstanceCrypto = InstanceCrypto;
            options.Signature = Signature;
            options.AccountLinkContextManager = DbContextManager;
        }

        public void Configure(AccountLinker options)
        {
            Configure("default", options);
        }
    }

    public class AccountLinker
    {
        public string DbId { get; set; }
        public Signature Signature { get; set; }
        public InstanceCrypto InstanceCrypto { get; set; }
        public AccountLinkerStorage AccountLinkerStorage { get; set; }
        public DbContextManager<AccountLinkContext> AccountLinkContextManager { get; set; }

        public AccountLinkContext AccountLinkContext
        {
            get
            {
                return AccountLinkContextManager.Get(DbId);
            }
        }

        public DbSet<AccountLinks> AccountLinks
        {
            get
            {
                return AccountLinkContext.AccountLinks;
            }
        }

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

        private List<LoginProfile> GetLinkedProfilesFromDB(string obj)
        {
            //Retrieve by uinque id
            return AccountLinks
                    .Where(r => r.Id == obj)
                    .Select(r => r.Profile)
                    .ToList()
                    .ConvertAll(x => LoginProfile.CreateFromSerializedString(Signature, InstanceCrypto, x));
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
            using var tr = AccountLinkContext.Database.BeginTransaction();

            var accountLinkQuery = AccountLinks
                .Where(r => r.Id == obj);

            if (!string.IsNullOrEmpty(provider)) accountLinkQuery = accountLinkQuery.Where(r => r.Provider == provider);
            if (!string.IsNullOrEmpty(hashId)) accountLinkQuery = accountLinkQuery.Where(r => r.UId == hashId);

            var accountLink = accountLinkQuery.FirstOrDefault();
            AccountLinks.Remove(accountLink);

            tr.Commit();
            AccountLinkerStorage.RemoveFromCache(obj);
        }
    }

    public static class AccountLinkerStorageExtension
    {
        public static IServiceCollection AddAccountLinkerStorageService(this IServiceCollection services)
        {
            services.TryAddSingleton<AccountLinkerStorage>();
            services.TryAddSingleton(typeof(ICacheNotify<>), typeof(KafkaCache<>));

            return services;
        }
    }

    public static class AccountLinkerExtension
    {
        public static IServiceCollection AddAccountLinker(this IServiceCollection services)
        {
            services.TryAddScoped<AccountLinker>();
            services.TryAddScoped<IConfigureOptions<AccountLinker>, ConfigureAccountLinker>();

            return services
                .AddAccountLinkContextService()
                .AddSignatureService()
                .AddInstanceCryptoService()
                .AddAccountLinkerStorageService();
        }
    }
}