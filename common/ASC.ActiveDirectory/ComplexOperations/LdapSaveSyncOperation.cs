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

namespace ASC.ActiveDirectory.ComplexOperations;

[Singletone(Additional = typeof(LdapOperationExtension))]
public class LdapSaveSyncOperation
{
    public const string CUSTOM_DISTRIBUTED_TASK_QUEUE_NAME = "ldapOperation";

    private readonly DistributedTaskQueue _progressQueue;
    private readonly IServiceProvider _serviceProvider;

    public LdapSaveSyncOperation(
        IServiceProvider serviceProvider,
         IDistributedTaskQueueFactory queueFactory)
    {
        _serviceProvider = serviceProvider;
        _progressQueue = queueFactory.CreateQueue(CUSTOM_DISTRIBUTED_TASK_QUEUE_NAME);
    }

    public async Task RunJobAsync(LdapSettings settings, Tenant tenant, LdapOperationType operationType, LdapLocalization resource = null, string userId = null)
    {
        var item = _progressQueue.GetAllTasks<LdapOperationJob>().FirstOrDefault(t => t.TenantId == tenant.Id);
        if (item != null && item.IsCompleted)
        {
            _progressQueue.DequeueTask(item.Id);
            item = null;
        }
        if (item == null)
        {
            using var scope = _serviceProvider.CreateScope();
            item = scope.ServiceProvider.GetRequiredService<LdapOperationJob>();
            await item.InitJobAsync(settings, tenant, operationType, resource, userId);
            _progressQueue.EnqueueTask(item);
        }

        item.PublishChanges();
    }

    public async Task<LdapOperationStatus> TestLdapSaveAsync(LdapSettings ldapSettings, Tenant tenant, string userId)
    {
        var (hasStarted, operations) = HasStarterdForTenant(tenant.Id, LdapOperationType.SyncTest, LdapOperationType.SaveTest);

        if (hasStarted)
        {
            return ToLdapOperationStatus(tenant.Id);
        }

        if (operations.Any(o => o.Status <= DistributedTaskStatus.Running))
        {
            return GetStartProcessError();
        }

        var ldapLocalization = new LdapLocalization();
        ldapLocalization.Init(Resource.ResourceManager);

        await RunJobAsync(ldapSettings, tenant, LdapOperationType.SaveTest, ldapLocalization, userId);
        return ToLdapOperationStatus(tenant.Id);
    }

    public async Task<LdapOperationStatus> SaveLdapSettingsAsync(LdapSettings ldapSettings, Tenant tenant, string userId)
    {
        var operations = GetOperationsForTenant(tenant.Id);

        if (operations.Any(o => o.Status <= DistributedTaskStatus.Running))
        {
            return GetStartProcessError();
        }

        //ToDo
        ldapSettings.AccessRights.Clear();

        if (!ldapSettings.LdapMapping.ContainsKey(LdapSettings.MappingFields.MailAttribute) || string.IsNullOrEmpty(ldapSettings.LdapMapping[LdapSettings.MappingFields.MailAttribute]))
        {
            ldapSettings.SendWelcomeEmail = false;
        }

        var ldapLocalization = new LdapLocalization();
        ldapLocalization.Init(Resource.ResourceManager, WebstudioNotifyPatternResource.ResourceManager);

        await RunJobAsync(ldapSettings, tenant, LdapOperationType.Save, ldapLocalization, userId);
        return ToLdapOperationStatus(tenant.Id);
    }

    public async Task<LdapOperationStatus> SyncLdapAsync(LdapSettings ldapSettings, Tenant tenant, string userId)
    {
        var (hasStarted, operations) = HasStarterdForTenant(tenant.Id, LdapOperationType.Sync, LdapOperationType.Save);

        if (hasStarted)
        {
            return ToLdapOperationStatus(tenant.Id);
        }

        if (operations.Any(o => o.Status <= DistributedTaskStatus.Running))
        {
            return GetStartProcessError();
        }

        var ldapLocalization = new LdapLocalization();
        ldapLocalization.Init(Resource.ResourceManager);

        await RunJobAsync(ldapSettings, tenant, LdapOperationType.Sync, ldapLocalization, userId);
        return ToLdapOperationStatus(tenant.Id);
    }

    public async Task<LdapOperationStatus> TestLdapSyncAsync(LdapSettings ldapSettings, Tenant tenant)
    {
        var (hasStarted, operations) = HasStarterdForTenant(tenant.Id, LdapOperationType.SyncTest, LdapOperationType.SaveTest);

        if (hasStarted)
        {
            return ToLdapOperationStatus(tenant.Id);
        }

        if (operations.Any(o => o.Status <= DistributedTaskStatus.Running))
        {
            return GetStartProcessError();
        }

        var ldapLocalization = new LdapLocalization();
        ldapLocalization.Init(Resource.ResourceManager);

        await RunJobAsync(ldapSettings, tenant, LdapOperationType.SyncTest, ldapLocalization);
        return ToLdapOperationStatus(tenant.Id);
    }

    public LdapOperationStatus ToLdapOperationStatus(int tenantId)
    {
        var operations = _progressQueue.GetAllTasks<LdapOperationJob>().ToList();

        foreach (var o in operations)
        {
            if (Process.GetProcesses().Any(p => p.Id == o.InstanceId))
            {
                continue;
            }

            o[LdapTaskProperty.PROGRESS] = 100;
            _progressQueue.DequeueTask(o.Id);
        }

        var operation =
            operations
                .FirstOrDefault(t => t[LdapTaskProperty.OWNER] == tenantId);

        if (operation == null)
        {
            return null;
        }

        if (DistributedTaskStatus.Running < operation.Status)
        {
            operation[LdapTaskProperty.PROGRESS] = 100;
            _progressQueue.DequeueTask(operation.Id);
        }

        var result = new LdapOperationStatus
        {
            Id = operation.Id,
            Completed = operation[LdapTaskProperty.FINISHED],
            Percents = operation[LdapTaskProperty.PROGRESS],
            Status = operation[LdapTaskProperty.RESULT],
            Error = operation[LdapTaskProperty.ERROR],
            CertificateConfirmRequest = operation[LdapTaskProperty.CERT_REQUEST] != "" ? operation[LdapTaskProperty.CERT_REQUEST] : null,
            Source = operation[LdapTaskProperty.SOURCE],
            OperationType = operation[LdapTaskProperty.OPERATION_TYPE],
            Warning = operation[LdapTaskProperty.WARNING]
        };

        if (!(string.IsNullOrEmpty(result.Warning)))
        {
            operation[LdapTaskProperty.WARNING] = ""; // "mark" as read
        }

        return result;
    }

    private static LdapOperationStatus GetStartProcessError()
    {
        var result = new LdapOperationStatus
        {
            Id = null,
            Completed = true,
            Percents = 0,
            Status = "",
            Error = Resource.LdapSettingsTooManyOperations,
            CertificateConfirmRequest = null,
            Source = ""
        };

        return result;
    }

    private (bool hasStarted, List<LdapOperationJob> operations) HasStarterdForTenant(int tenantId, LdapOperationType arg1, LdapOperationType arg2)
    {
        var operations = GetOperationsForTenant(tenantId);

        var hasStarted = operations.Any(o =>
        {
            var opType = o[LdapTaskProperty.OPERATION_TYPE];

            return o.Status <= DistributedTaskStatus.Running &&
                   (opType == arg1.ToString() || opType == arg2.ToString());
        });

        return (hasStarted, operations);
    }

    private List<LdapOperationJob> GetOperationsForTenant(int tenantId)
    {
        return _progressQueue.GetAllTasks<LdapOperationJob>()
            .Where(t => t[LdapTaskProperty.OWNER] == tenantId)
            .ToList();
    }

    public static class LdapOperationExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<LdapOperationJob>();
        }
    }
}