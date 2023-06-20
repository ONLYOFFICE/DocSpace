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

namespace ASC.Web.Core.RemovePortal;

[Singletone(Additional = typeof(RemovePortalWorkerExtension))]
public class RemovePortalWorker
{
    private readonly object _locker;
    private readonly DistributedTaskQueue _queue;
    private readonly IServiceProvider _serviceProvider;

    public const string CUSTOM_DISTRIBUTED_TASK_QUEUE_NAME = "removePortal";

    public RemovePortalWorker(IDistributedTaskQueueFactory queueFactory,
                            IServiceProvider serviceProvider)
    {
        _locker = new object();
        _serviceProvider = serviceProvider;
        _queue = queueFactory.CreateQueue(CUSTOM_DISTRIBUTED_TASK_QUEUE_NAME);
    }

    public void Start(int tenantId)
    {
        lock (_locker)
        {
            var item = _queue.GetAllTasks<RemovePortalOperation>().FirstOrDefault(t => t.TenantId == tenantId);

            if (item != null && item.IsCompleted)
            {
                _queue.DequeueTask(item.Id);
                item = null;
            }
            if (item == null)
            {

                item = _serviceProvider.GetService<RemovePortalOperation>();

                item.Init(tenantId);

                _queue.EnqueueTask(item);
            }

            item.PublishChanges();
        }
    }

    public void Stop()
    {
        var tasks = _queue.GetAllTasks(DistributedTaskQueue.INSTANCE_ID);

        foreach (var t in tasks)
        {
            _queue.DequeueTask(t.Id);
        }
    }
}

public static class RemovePortalWorkerExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<RemovePortalOperation>();
    }
}