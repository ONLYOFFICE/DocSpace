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

namespace ASC.Web.Studio.Core.Quota;

[Singletone(Additional = typeof(QuotaSyncOperationExtension))]
public class QuotaSyncOperation
{

    public const string CUSTOM_DISTRIBUTED_TASK_QUEUE_NAME = "ldapOperation";

    private readonly DistributedTaskQueue _progressQueue;

    private readonly IServiceProvider _serviceProvider;

    public QuotaSyncOperation(IServiceProvider serviceProvider, IDistributedTaskQueueFactory queueFactory)
    {
;
        _serviceProvider = serviceProvider;

        _progressQueue = queueFactory.CreateQueue(CUSTOM_DISTRIBUTED_TASK_QUEUE_NAME);
    }
    public void RunJob(Tenant tenant)
    {
        var item = _progressQueue.GetAllTasks<QuotaSyncJob>().FirstOrDefault(t => t.TenantId == tenant.Id);
        if (item != null && item.IsCompleted)
        {
            _progressQueue.DequeueTask(item.Id);
            item = null;
        }
        
        if (item == null)
        {
            item = _serviceProvider.GetRequiredService<QuotaSyncJob>();
            item.InitJob(tenant);
            _progressQueue.EnqueueTask(item);
        }

        item.PublishChanges();
    }


    public static class QuotaSyncOperationExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<QuotaSyncJob>();
        }
    }

}

public class QuotaSyncJob : DistributedTaskProgress
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private int? _tenantId;
    public int TenantId
    {
        get
        {
            return _tenantId ?? this[nameof(_tenantId)];
        }
        private set
        {
            _tenantId = value;
            this[nameof(_tenantId)] = value;
        }
    }

    public QuotaSyncJob(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
    public void InitJob(Tenant tenant)
    {
        TenantId = tenant.Id;
    }
    protected override void DoJob()
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var _tenantManager = scope.ServiceProvider.GetRequiredService<TenantManager>();
            var _storageFactoryConfig = scope.ServiceProvider.GetRequiredService<StorageFactoryConfig>();
            var _storageFactory = scope.ServiceProvider.GetRequiredService<StorageFactory>();

            _tenantManager.SetCurrentTenant(TenantId);
            var storageModules = _storageFactoryConfig.GetModuleList(string.Empty);

            foreach (var module in storageModules)
            {
                var storage = _storageFactory.GetStorage(TenantId.ToString(), module);
                storage.ResetQuotaAsync("").Wait();

                var domains = _storageFactoryConfig.GetDomainList(string.Empty, module);
                foreach (var domain in domains)
                {
                    storage.ResetQuotaAsync(domain).Wait();
                }
            }
        }
        catch (Exception ex)
        {
            Status = DistributedTaskStatus.Failted;
            Exception = ex;
        }
        finally
        {
            IsCompleted = true;
        }
        PublishChanges();
    }
}
