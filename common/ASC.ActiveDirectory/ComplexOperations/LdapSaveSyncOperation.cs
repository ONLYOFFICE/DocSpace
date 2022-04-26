/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

// ReSharper disable RedundantToStringCall

using System.Diagnostics;

namespace ASC.ActiveDirectory.ComplexOperations;
[Singletone(Additional = typeof(LdapOperationExtension))]
public class LdapSaveSyncOperation
{
    private DistributedTaskQueue _progressQueue;
    private IServiceProvider _serviceProvider;

    public LdapSaveSyncOperation(
        IServiceProvider serviceProvider,
        DistributedTaskQueueOptionsManager progressQueue)
    {
        _serviceProvider = serviceProvider;
        _progressQueue = progressQueue.Get<LdapOperationJob>();
    }

    public void RunJob(LdapSettings settings, Tenant tenant, LdapOperationType operationType, LdapLocalization resource = null, string userId = null)
    {
        var item = _progressQueue.GetTasks<LdapOperationJob>().FirstOrDefault(t => t.TenantId == tenant.TenantId);
        if (item != null && item.IsCompleted)
        {
            _progressQueue.RemoveTask(item.Id);
            item = null;
        }
        if (item == null)
        {
            using var scope = _serviceProvider.CreateScope();
            item = scope.ServiceProvider.GetService<LdapOperationJob>();
            item.InitJob(settings, tenant, operationType, resource, userId);
            _progressQueue.QueueTask(item);
        }

        item.PublishChanges();
    }

    public LdapOperationStatus ToLdapOperationStatus(int tenantId)
    {
        var operations = _progressQueue.GetTasks().ToList();

        foreach (var o in operations)
        {
            if (Process.GetProcesses().Any(p => p.Id == o.InstanceId))
                continue;

            o.SetProperty(LdapTaskProperty.PROGRESS, 100);
            _progressQueue.RemoveTask(o.Id);
        }

        var operation =
            operations
                .FirstOrDefault(t => t.GetProperty<int>(LdapTaskProperty.OWNER) == tenantId);

        if (operation == null)
        {
            return null;
        }

        if (DistributedTaskStatus.Running < operation.Status)
        {
            operation.SetProperty(LdapTaskProperty.PROGRESS, 100);
            _progressQueue.RemoveTask(operation.Id);
        }

        var certificateConfirmRequest = operation.GetProperty<LdapCertificateConfirmRequest>(LdapTaskProperty.CERT_REQUEST);

        var result = new LdapOperationStatus
        {
            Id = operation.Id,
            Completed = operation.GetProperty<bool>(LdapTaskProperty.FINISHED),
            Percents = operation.GetProperty<int>(LdapTaskProperty.PROGRESS),
            Status = operation.GetProperty<string>(LdapTaskProperty.RESULT),
            Error = operation.GetProperty<string>(LdapTaskProperty.ERROR),
            CertificateConfirmRequest = certificateConfirmRequest,
            Source = operation.GetProperty<string>(LdapTaskProperty.SOURCE),
            OperationType = Enum.GetName(typeof(LdapOperationType),
                (LdapOperationType)operation.GetProperty<int>(LdapTaskProperty.OPERATION_TYPE)),
            Warning = operation.GetProperty<string>(LdapTaskProperty.WARNING)
        };

        if (!(string.IsNullOrEmpty(result.Warning)))
        {
            operation.SetProperty(LdapTaskProperty.WARNING, ""); // "mark" as read
        }

        return result;
    }

    public static class LdapOperationExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<LdapOperationJob>();
        }
    }
}