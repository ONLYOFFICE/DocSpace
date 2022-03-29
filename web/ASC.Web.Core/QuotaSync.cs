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

using ASC.Common.Threading;
using ASC.Core;
using ASC.Data.Storage;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.Web.Studio.Core.Quota
{
    public class QuotaSync
    {
        public const string TenantIdKey = "tenantID";
        protected DistributedTask TaskInfo { get; private set; }
        private int TenantId { get; set; }
        private IServiceProvider ServiceProvider { get; }

        public QuotaSync(int tenantId, IServiceProvider serviceProvider)
        {
            TenantId = tenantId;
            TaskInfo = new DistributedTask();
            ServiceProvider = serviceProvider;
        }

        public void RunJob()//DistributedTask distributedTask, CancellationToken cancellationToken)
        {
            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<QuotaSyncScope>();
            var (tenantManager, storageFactoryConfig, storageFactory) = scopeClass;
            tenantManager.SetCurrentTenant(TenantId);

            var storageModules = storageFactoryConfig.GetModuleList(string.Empty);

            foreach (var module in storageModules)
            {
                var storage = storageFactory.GetStorage(TenantId.ToString(), module);
                storage.ResetQuotaAsync("").Wait();

                var domains = storageFactoryConfig.GetDomainList(string.Empty, module);
                foreach (var domain in domains)
                {
                    storage.ResetQuotaAsync(domain).Wait();
                }

            }
        }

        public virtual DistributedTask GetDistributedTask()
        {
            TaskInfo.SetProperty(TenantIdKey, TenantId);
            return TaskInfo;
        }
    }

    class QuotaSyncScope
    {
        private TenantManager TenantManager { get; }
        private StorageFactoryConfig StorageFactoryConfig { get; }
        private StorageFactory StorageFactory { get; }

        public QuotaSyncScope(TenantManager tenantManager, StorageFactoryConfig storageFactoryConfig, StorageFactory storageFactory)
        {
            TenantManager = tenantManager;
            StorageFactoryConfig = storageFactoryConfig;
            StorageFactory = storageFactory;
        }

        public void Deconstruct(out TenantManager tenantManager, out StorageFactoryConfig storageFactoryConfig, out StorageFactory storageFactory)
        {
            tenantManager = TenantManager;
            storageFactoryConfig = StorageFactoryConfig;
            storageFactory = StorageFactory;
        }
    }
}