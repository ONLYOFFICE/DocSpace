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

namespace ASC.Data.Reassigns;

public class QueueWorker<T> where T : DistributedTaskProgress
{
    private readonly object _synchRoot = new object();

    protected readonly IServiceProvider _serviceProvider;
    protected readonly DistributedTaskQueue _queue;
    protected readonly IDictionary<string, StringValues> _httpHeaders;

    public QueueWorker(
        IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider,
            IDistributedTaskQueueFactory queueFactory,
            string queueName)
    {
        _serviceProvider = serviceProvider;
        _queue = queueFactory.CreateQueue(queueName);
        _httpHeaders = httpContextAccessor.HttpContext?.Request?.Headers;
    }

    public static string GetProgressItemId(int tenantId, Guid userId)
    {
        return string.Format("{0}_{1}_{2}", tenantId, userId, typeof(T).Name);
    }

    public T GetProgressItemStatus(int tenantId, Guid userId)
    {
        var id = GetProgressItemId(tenantId, userId);

        return _queue.PeekTask<T>(id);
    }

    public void Terminate(int tenantId, Guid userId)
    {
        var item = GetProgressItemStatus(tenantId, userId);

        if (item != null)
        {
            _queue.DequeueTask(item.Id);
        }
    }

    protected T Start(int tenantId, Guid userId, T newTask)
    {
        lock (_synchRoot)
        {
            var task = GetProgressItemStatus(tenantId, userId);

            if (task != null && task.IsCompleted)
            {
                _queue.DequeueTask(task.Id);
                task = null;
            }

            if (task == null)
            {
                task = newTask;
                _queue.EnqueueTask(task);
            }

            return task;
        }
    }
}

[Scope(Additional = typeof(ReassignProgressItemExtension))]
public class QueueWorkerReassign : QueueWorker<ReassignProgressItem>
{
    public const string CUSTOM_DISTRIBUTED_TASK_QUEUE_NAME = "user_data_reassign";

    public QueueWorkerReassign(
        IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider,
            IDistributedTaskQueueFactory queueFactory) :
            base(httpContextAccessor, serviceProvider, queueFactory, CUSTOM_DISTRIBUTED_TASK_QUEUE_NAME)
    {
    }

    public ReassignProgressItem Start(int tenantId, Guid fromUserId, Guid toUserId, Guid currentUserId, bool notify, bool deleteProfile)
    {
        var result = _serviceProvider.GetService<ReassignProgressItem>();

        result.Init(_httpHeaders, tenantId, fromUserId, toUserId, currentUserId, notify, deleteProfile);

        return Start(tenantId, fromUserId, result);
    }
}

[Scope(Additional = typeof(RemoveProgressItemExtension))]
public class QueueWorkerRemove : QueueWorker<RemoveProgressItem>
{
    public const string CUSTOM_DISTRIBUTED_TASK_QUEUE_NAME = "user_data_remove";

    public QueueWorkerRemove(
        IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider,
            IDistributedTaskQueueFactory queueFactory) :
            base(httpContextAccessor, serviceProvider, queueFactory, CUSTOM_DISTRIBUTED_TASK_QUEUE_NAME)
    {
    }

    public RemoveProgressItem Start(int tenantId, UserInfo user, Guid currentUserId, bool notify, bool deleteProfile)
    {
        var result = _serviceProvider.GetService<RemoveProgressItem>();

        result.Init(_httpHeaders, tenantId, user, currentUserId, notify, deleteProfile);

        return Start(tenantId, user.Id, result);
    }
}
