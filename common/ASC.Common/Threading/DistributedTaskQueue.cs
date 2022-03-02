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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common.Caching;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Common.Threading
{
    [Singletone]
    public class DistributedTaskCacheNotify
    {
        public ConcurrentDictionary<string, CancellationTokenSource> Cancelations { get; }
        public ICache Cache { get; }
        private readonly ICacheNotify<DistributedTaskCancelation> notify;
        private readonly ICacheNotify<DistributedTaskCache> notifyCache;

        public DistributedTaskCacheNotify(
            ICacheNotify<DistributedTaskCancelation> notify,
            ICacheNotify<DistributedTaskCache> notifyCache,
            ICache cache)
        {
            Cancelations = new ConcurrentDictionary<string, CancellationTokenSource>();

            Cache = cache;

            this.notify = notify;

            notify.Subscribe((c) =>
            {
                if (Cancelations.TryGetValue(c.Id, out var s))
                {
                    s.Cancel();
                }
            }, CacheNotifyAction.Remove);

            this.notifyCache = notifyCache;

            notifyCache.Subscribe((c) =>
            {
                Cache.HashSet(c.Key, c.Id, (DistributedTaskCache)null);
            }, CacheNotifyAction.Remove);

            notifyCache.Subscribe((c) =>
            {
                Cache.HashSet(c.Key, c.Id, c);
            }, CacheNotifyAction.InsertOrUpdate);
        }

        public void CancelTask(string id)
        {
            notify.Publish(new DistributedTaskCancelation() { Id = id }, CacheNotifyAction.Remove);
        }

        public async Task CancelTaskAsync(string id)
        {
            await notify.PublishAsync(new DistributedTaskCancelation() { Id = id }, CacheNotifyAction.Remove);
        }

        public void SetTask(DistributedTask task)
        {
            notifyCache.Publish(task.DistributedTaskCache, CacheNotifyAction.InsertOrUpdate);
        }

        public async Task SetTaskAsync(DistributedTask task)
        {
            await notifyCache.PublishAsync(task.DistributedTaskCache, CacheNotifyAction.InsertOrUpdate);
        }

        public void RemoveTask(string id, string key)
        {
            notifyCache.Publish(new DistributedTaskCache() { Id = id, Key = key }, CacheNotifyAction.Remove);
        }

        public async Task RemoveTaskAsync(string id, string key)
        {
            await notifyCache.PublishAsync(new DistributedTaskCache() { Id = id, Key = key }, CacheNotifyAction.Remove);
        }
    }

    [Singletone(typeof(ConfigureDistributedTaskQueue))]
    public class DistributedTaskQueueOptionsManager : OptionsManager<DistributedTaskQueue>
    {
        public DistributedTaskQueueOptionsManager(IOptionsFactory<DistributedTaskQueue> factory) : base(factory)
        {
        }

        public DistributedTaskQueue Get<T>() where T : DistributedTask
        {
            return Get(typeof(T).FullName);
        }
    }

    [Scope]
    public class ConfigureDistributedTaskQueue : IConfigureNamedOptions<DistributedTaskQueue>
    {
        private DistributedTaskCacheNotify DistributedTaskCacheNotify { get; }
        public IServiceProvider ServiceProvider { get; }

        public ConfigureDistributedTaskQueue(DistributedTaskCacheNotify distributedTaskCacheNotify, IServiceProvider serviceProvider)
        {
            DistributedTaskCacheNotify = distributedTaskCacheNotify;
            ServiceProvider = serviceProvider;
        }

        public void Configure(DistributedTaskQueue queue)
        {
            queue.DistributedTaskCacheNotify = DistributedTaskCacheNotify;
            queue.ServiceProvider = ServiceProvider;
        }

        public void Configure(string name, DistributedTaskQueue options)
        {
            Configure(options);
            options.Name = name;
        }
    }

    public class DistributedTaskQueue
    {
        public static readonly int InstanceId = Process.GetCurrentProcess().Id;

        private string key;
        private TaskScheduler Scheduler { get; set; } = TaskScheduler.Default;

        public IServiceProvider ServiceProvider { get; set; }
        public string Name { get { return key; } set { key = value + GetType().Name; } }
        private ICache Cache { get => DistributedTaskCacheNotify.Cache; }
        private ConcurrentDictionary<string, CancellationTokenSource> Cancelations { get => DistributedTaskCacheNotify.Cancelations; }

        public int MaxThreadsCount
        {
            set
            {
                Scheduler = value <= 0
                    ? TaskScheduler.Default
                : new ConcurrentExclusiveSchedulerPair(TaskScheduler.Default, value).ConcurrentScheduler;
            }
        }

        public DistributedTaskCacheNotify DistributedTaskCacheNotify { get; set; }

        public void QueueTask(DistributedTaskProgress taskProgress)
        {
            QueueTask((a, b) => taskProgress.RunJob(), taskProgress);
        }

        public void QueueTask(Action<DistributedTask, CancellationToken> action, DistributedTask distributedTask = null)
        {
            if (distributedTask == null)
            {
                distributedTask = new DistributedTask();
            }

            distributedTask.InstanceId = InstanceId;

            var cancelation = new CancellationTokenSource();
            var token = cancelation.Token;
            Cancelations[distributedTask.Id] = cancelation;

            var task = new Task(() => { action(distributedTask, token); }, token, TaskCreationOptions.LongRunning);
            task
                .ConfigureAwait(false)
                .GetAwaiter()
                .OnCompleted(() => OnCompleted(task, distributedTask.Id));

            distributedTask.Status = DistributedTaskStatus.Running;

            if (distributedTask.Publication == null)
            {
                distributedTask.Publication = GetPublication();
            }
            distributedTask.PublishChanges();

            task.Start(Scheduler);
        }

        public void QueueTask(Func<DistributedTask, CancellationToken, Task> action, DistributedTask distributedTask = null)
        {
            if (distributedTask == null)
            {
                distributedTask = new DistributedTask();
            }

            distributedTask.InstanceId = InstanceId;

            var cancelation = new CancellationTokenSource();
            var token = cancelation.Token;
            Cancelations[distributedTask.Id] = cancelation;

            var task = new Task(async () =>
            {
                var t = action(distributedTask, token);
                t.ConfigureAwait(false)
                .GetAwaiter()
                .OnCompleted(() => OnCompleted(t, distributedTask.Id));
                await t;
            }, token, TaskCreationOptions.LongRunning);

            distributedTask.Status = DistributedTaskStatus.Running;

            if (distributedTask.Publication == null)
            {
                distributedTask.Publication = GetPublication();
            }
            distributedTask.PublishChanges();

            task.Start(Scheduler);
        }

        public IEnumerable<DistributedTask> GetTasks()
        {
            var tasks = Cache.HashGetAll<DistributedTaskCache>(key).Values.Select(r => new DistributedTask(r)).ToList();
            tasks.ForEach(t =>
            {
                if (t.Publication == null)
                {
                    t.Publication = GetPublication();
                }
            });
            return tasks;
        }

        public IEnumerable<T> GetTasks<T>() where T : DistributedTask
        {
            var tasks = Cache.HashGetAll<DistributedTaskCache>(key).Values.Select(r =>
            {
                var result = ServiceProvider.GetService<T>();
                result.DistributedTaskCache = r;
                return result;
            }).ToList();

            tasks.ForEach(t =>
            {
                if (t.Publication == null)
                {
                    t.Publication = GetPublication();
                }
            });
            return tasks;
        }

        public T GetTask<T>(string id) where T : DistributedTask
        {
            var cache = Cache.HashGet<DistributedTaskCache>(key, id);
            if (cache != null)
            {
                using var scope = ServiceProvider.CreateScope();
                var task = scope.ServiceProvider.GetService<T>();
                task.DistributedTaskCache = cache;
                if (task.Publication == null)
                {
                    task.Publication = GetPublication();
                }
                return task;
            }

            return null;
        }

        public DistributedTask GetTask(string id)
        {
            var cache = Cache.HashGet<DistributedTaskCache>(key, id);
            if (cache != null)
            {
                var task = new DistributedTask();
                task.DistributedTaskCache = cache;
                if (task.Publication == null)
                {
                    task.Publication = GetPublication();
                }
                return task;
            }

            return null;
        }

        public void SetTask(DistributedTask task)
        {
            DistributedTaskCacheNotify.SetTask(task);
        }

        public void RemoveTask(string id)
        {
            DistributedTaskCacheNotify.RemoveTask(id, key);
        }

        public void CancelTask(string id)
        {
            DistributedTaskCacheNotify.CancelTask(id);
        }

        private void OnCompleted(Task task, string id)
        {
            var distributedTask = GetTask(id);
            if (distributedTask != null)
            {
                distributedTask.Status = DistributedTaskStatus.Completed;
                distributedTask.Exception = task.Exception;
                if (task.IsFaulted)
                {
                    distributedTask.Status = DistributedTaskStatus.Failted;
                }
                if (task.IsCanceled)
                {
                    distributedTask.Status = DistributedTaskStatus.Canceled;
                }

                Cancelations.TryRemove(id, out _);

                distributedTask.PublishChanges();
            }
        }

        private Action<DistributedTask> GetPublication()
        {
            return (t) =>
            {
                if (t.DistributedTaskCache != null)
                {
                    t.DistributedTaskCache.Key = key;
                }
                DistributedTaskCacheNotify.SetTask(t);
            };
        }
    }

    public static class DistributedTaskQueueExtention
    {
        public static DIHelper AddDistributedTaskQueueService<T>(this DIHelper services, int maxThreadsCount) where T : DistributedTask
        {
            services.TryAdd<DistributedTaskCacheNotify>();
            services.TryAdd<DistributedTaskQueueOptionsManager>();
            services.TryAdd<DistributedTaskQueue>();

            var type = typeof(T);
            if (!type.IsAbstract)
            {
                services.TryAdd<T>();
            }

            services.TryAddSingleton<IConfigureOptions<DistributedTaskQueue>, ConfigureDistributedTaskQueue>();

            _ = services.Configure<DistributedTaskQueue>(type.Name, r =>
            {
                r.MaxThreadsCount = maxThreadsCount;
                //r.errorCount = 1;
            });

            return services;
        }
    }
}
