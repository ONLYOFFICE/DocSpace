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

using ASC.Common;
using ASC.Common.Threading;
using ASC.Core.Users;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace ASC.Data.Reassigns
{
    public class QueueWorker
    {
        public static IDictionary<string, StringValues> GetHttpHeaders(HttpRequest httpRequest)
        {
            return httpRequest?.Headers;
        }
    }

    public class QueueWorker<T> where T : DistributedTaskProgress
    {
        protected readonly DistributedTaskQueue Queue;

        protected IHttpContextAccessor HttpContextAccessor { get; }
        protected IServiceProvider ServiceProvider { get; }

        public object SynchRoot = new object();

        public QueueWorker(
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider,
            DistributedTaskQueueOptionsManager options)
        {
            HttpContextAccessor = httpContextAccessor;
            ServiceProvider = serviceProvider;
            Queue = options.Get<T>();
        }

        public static string GetProgressItemId(int tenantId, Guid userId)
        {
            return string.Format("{0}_{1}_{2}", tenantId, userId, typeof(T).Name);
        }

        public T GetProgressItemStatus(int tenantId, Guid userId)
        {
            var id = GetProgressItemId(tenantId, userId);
            return Queue.GetTask<T>(id);
        }

        public void Terminate(int tenantId, Guid userId)
        {
            var item = GetProgressItemStatus(tenantId, userId);

            if (item != null)
            {
                Queue.CancelTask(item.Id);
            }
        }

        protected DistributedTaskProgress Start(int tenantId, Guid userId, Func<T> constructor)
        {
            lock (SynchRoot)
            {
                var task = GetProgressItemStatus(tenantId, userId);

                if (task != null && task.IsCompleted)
                {
                    Queue.RemoveTask(task.Id);
                    task = null;
                }

                if (task == null)
                {
                    task = constructor();
                    Queue.QueueTask(task);
                }

                return task;
            }
        }
    }

    [Scope(Additional = typeof(ReassignProgressItemExtension))]
    public class QueueWorkerReassign : QueueWorker<ReassignProgressItem>
    {
        public QueueWorkerReassign(
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider,
            DistributedTaskQueueOptionsManager options) :
            base(httpContextAccessor, serviceProvider, options)
        {
        }

        public ReassignProgressItem Start(int tenantId, Guid fromUserId, Guid toUserId, Guid currentUserId, bool deleteProfile)
        {
            return Start(tenantId, fromUserId, () =>
            {
                var result = ServiceProvider.GetService<ReassignProgressItem>();
                result.Init(tenantId, fromUserId, toUserId, currentUserId, deleteProfile);
                return result;
            }) as ReassignProgressItem;
        }
    }

    [Scope(Additional = typeof(RemoveProgressItemExtension))]
    public class QueueWorkerRemove : QueueWorker<RemoveProgressItem>
    {
        public QueueWorkerRemove(
            IHttpContextAccessor httpContextAccessor,
            IServiceProvider serviceProvider,
            DistributedTaskQueueOptionsManager options) :
            base(httpContextAccessor, serviceProvider, options)
        {
        }

        public RemoveProgressItem Start(int tenantId, UserInfo user, Guid currentUserId, bool notify)
        {
            return Start(tenantId, user.ID, () =>
            {
                var result = ServiceProvider.GetService<RemoveProgressItem>();
                result.Init(tenantId, user, currentUserId, notify);
                return result;
            }) as RemoveProgressItem;
        }
    }
}
