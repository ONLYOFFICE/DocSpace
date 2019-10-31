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
using ASC.Common.Data;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Expressions;
using ASC.Common.Utils;
using ASC.FederatedLogin.Profile;
using ASC.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

    public class AccountLinker
    {

        private readonly string dbid;

        public Signature Signature { get; }
        public InstanceCrypto InstanceCrypto { get; }
        public DbOptionsManager DbOptions { get; }
        public AccountLinkerStorage AccountLinkerStorage { get; }

        public AccountLinker(string dbid, Signature signature, InstanceCrypto instanceCrypto, DbOptionsManager dbOptions, AccountLinkerStorage accountLinkerStorage)
        {
            this.dbid = dbid;
            Signature = signature;
            InstanceCrypto = instanceCrypto;
            DbOptions = dbOptions;
            AccountLinkerStorage = accountLinkerStorage;
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
            var db = DbOptions.Get(dbid);
            var query = new SqlQuery("account_links")
.Select("id").Where("uid", hashid).Where(!Exp.Eq("provider", string.Empty));
            return db.ExecuteList(query).ConvertAll(x => (string)x[0]);
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
            var db = DbOptions.Get(dbid);
            var query = new SqlQuery("account_links")
.Select("profile").Where("id", obj);
            return db.ExecuteList(query).ConvertAll(x => LoginProfile.CreateFromSerializedString(Signature, InstanceCrypto, (string)x[0]));
        }

        public void AddLink(string obj, LoginProfile profile)
        {
            var db = DbOptions.Get(dbid);
            db.ExecuteScalar<int>(
                new SqlInsert("account_links", true)
                    .InColumnValue("id", obj)
                    .InColumnValue("uid", profile.HashId)
                    .InColumnValue("provider", profile.Provider)
                    .InColumnValue("profile", profile.ToSerializedString())
                    .InColumnValue("linked", DateTime.UtcNow)
                );
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
            var sql = new SqlDelete("account_links").Where("id", obj);

            if (!string.IsNullOrEmpty(provider)) sql.Where("provider", provider);
            if (!string.IsNullOrEmpty(hashId)) sql.Where("uid", hashId);

            var db = DbOptions.Get(dbid);
            db.ExecuteScalar<int>(sql);

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
}