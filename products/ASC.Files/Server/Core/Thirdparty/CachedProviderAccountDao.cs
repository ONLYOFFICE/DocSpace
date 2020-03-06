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
using System.Collections.Concurrent;
using System.Globalization;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Configuration;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.Files.Core;
using ASC.Files.Core.EF;
using ASC.Security.Cryptography;

using Microsoft.Extensions.Options;

namespace ASC.Files.Thirdparty
{
    internal class CachedProviderAccountDaoNotify
    {
        public ConcurrentDictionary<string, IProviderInfo> Cache { get; private set; }
        public ICacheNotify<ProviderAccountCacheItem> CacheNotify { get; private set; }

        public CachedProviderAccountDaoNotify(ICacheNotify<ProviderAccountCacheItem> cacheNotify)
        {
            Cache = new ConcurrentDictionary<string, IProviderInfo>();
            CacheNotify = cacheNotify;
            cacheNotify.Subscribe((i) => RemoveFromCache(i.Key), CacheNotifyAction.Any);
        }
        private void RemoveFromCache(string key)
        {
            Cache.TryRemove(key, out _);
        }
    }

    internal class CachedProviderAccountDao : ProviderAccountDao
    {
        private readonly ConcurrentDictionary<string, IProviderInfo> cache;
        private readonly ICacheNotify<ProviderAccountCacheItem> cacheNotify;

        private readonly string _rootKey;

        public CachedProviderAccountDao(
            IServiceProvider serviceProvider,
            TenantUtil tenantUtil,
            TenantManager tenantManager,
            InstanceCrypto instanceCrypto,
            SecurityContext securityContext,
            ConsumerFactory consumerFactory,
            DbContextManager<FilesDbContext> dbContextManager,
            IOptionsMonitor<ILog> options,
            CachedProviderAccountDaoNotify cachedProviderAccountDaoNotify)
            : base(serviceProvider, tenantUtil, tenantManager, instanceCrypto, securityContext, consumerFactory, dbContextManager, options)
        {
            cache = cachedProviderAccountDaoNotify.Cache;
            cacheNotify = cachedProviderAccountDaoNotify.CacheNotify;
            _rootKey = tenantManager.GetCurrentTenant().TenantId.ToString(CultureInfo.InvariantCulture);
        }

        public override IProviderInfo GetProviderInfo(int linkId)
        {
            var key = _rootKey + linkId.ToString(CultureInfo.InvariantCulture);
            if (!cache.TryGetValue(key, out var value))
            {
                value = base.GetProviderInfo(linkId);
                cache.TryAdd(key, value);
            }
            return value;
        }

        public override void RemoveProviderInfo(int linkId)
        {
            base.RemoveProviderInfo(linkId);

            var key = _rootKey + linkId.ToString(CultureInfo.InvariantCulture);
            cacheNotify.Publish(new ProviderAccountCacheItem { Key = key }, CacheNotifyAction.Any);
        }

        public override int UpdateProviderInfo(int linkId, string customerTitle, AuthData authData, FolderType folderType, Guid? userId = null)
        {
            var result = base.UpdateProviderInfo(linkId, customerTitle, authData, folderType, userId);

            var key = _rootKey + linkId.ToString(CultureInfo.InvariantCulture);
            cacheNotify.Publish(new ProviderAccountCacheItem { Key = key }, CacheNotifyAction.Any);
            return result;
        }

        public override int UpdateProviderInfo(int linkId, AuthData authData)
        {
            var result = base.UpdateProviderInfo(linkId, authData);

            var key = _rootKey + linkId.ToString(CultureInfo.InvariantCulture);
            cacheNotify.Publish(new ProviderAccountCacheItem { Key = key }, CacheNotifyAction.Any);
            return result;
        }
    }
    public static class CachedProviderAccountDaoExtention
    {
        public static DIHelper AddCachedProviderAccountDaoService(this DIHelper services)
        {
            services.TryAddScoped<IProviderDao, CachedProviderAccountDao>();

            services.TryAddSingleton<CachedProviderAccountDaoNotify>();

            return services
                .AddProviderAccountDaoService();
        }
    }
}