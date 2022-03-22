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

namespace ASC.Common.Threading;

[Transient]
public class DistributedTaskQueue
{
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _cancelations;
    private readonly IServiceProvider _serviceProvider;
    private readonly ICacheNotify<DistributedTaskCancelation> _cancellationCacheNotify;
    private readonly ICacheNotify<DistributedTaskCache> _taskCacheNotify;
    private readonly ICache _localCache;

    private int _maxThreadsCount = -1;
    private string _name;
    private TaskScheduler Scheduler { get; set; } = TaskScheduler.Default;

    public DistributedTaskQueue(IServiceProvider serviceProvider,
                                ICacheNotify<DistributedTaskCancelation> cancelTaskNotify,
                                ICacheNotify<DistributedTaskCache> taskCacheNotify,
                                ICache cache)
    {
        _serviceProvider = serviceProvider;
        _cancellationCacheNotify = cancelTaskNotify;
        _cancelations = new ConcurrentDictionary<string, CancellationTokenSource>();
        _localCache = cache;
        _cancellationCacheNotify.Subscribe((c) =>
        {
            if (_cancelations.TryGetValue(c.Id, out var s))
            {
                s.Cancel();
            }
        }, CacheNotifyAction.Remove);

        _taskCacheNotify = taskCacheNotify;

        _taskCacheNotify.Subscribe((c) =>
        {
            _localCache.HashSet(c.Key, c.Id, (DistributedTaskCache)null);
        }, CacheNotifyAction.Remove);

        _taskCacheNotify.Subscribe((c) =>
        {
            _localCache.HashSet(c.Key, c.Id, c);
        }, CacheNotifyAction.InsertOrUpdate);

    }

    public string Name
    {
        get => _name;
        set => _name = value + GetType().Name;
    }

    public int MaxThreadsCount
    {
        get
        {
            return _maxThreadsCount;
        }

        set
        {
            Scheduler = value <= 0
                ? TaskScheduler.Default
                : new ConcurrentExclusiveSchedulerPair(TaskScheduler.Default, value).ConcurrentScheduler;

            if (value > 0)
            {
                _maxThreadsCount = value;
            }
        }
    }

    public static readonly int InstanceId = Process.GetCurrentProcess().Id;
    
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
        _cancelations[distributedTask.Id] = cancelation;

        var task = new Task(() => { action(distributedTask, token); }, token, TaskCreationOptions.LongRunning);

        task.ConfigureAwait(false)
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
        _cancelations[distributedTask.Id] = cancelation;

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
        var tasks = _localCache.HashGetAll<DistributedTaskCache>(_name).Values.Select(r => new DistributedTask(r)).ToList();

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
        var tasks = _localCache.HashGetAll<DistributedTaskCache>(_name).Values.Select(r =>
        {
            var result = _serviceProvider.GetService<T>();
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
        var cache = _localCache.HashGet<DistributedTaskCache>(_name, id);
        if (cache != null)
        {
            using var scope = _serviceProvider.CreateScope();
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
        var cache = _localCache.HashGet<DistributedTaskCache>(_name, id);
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
        _taskCacheNotify.Publish(task.DistributedTaskCache, CacheNotifyAction.InsertOrUpdate);
    }

    public void RemoveTask(string id)
    {
        _taskCacheNotify.Publish(new DistributedTaskCache() { Id = id, Key = id }, CacheNotifyAction.Remove);
    }

    public void CancelTask(string id)
    {
        _cancellationCacheNotify.Publish(new DistributedTaskCancelation() { Id = id }, CacheNotifyAction.Remove);
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

            _cancelations.TryRemove(id, out _);

            distributedTask.PublishChanges();
        }
    }

    private Action<DistributedTask> GetPublication()
    {
        return (t) =>
        {
            if (t.DistributedTaskCache != null)
            {
                t.DistributedTaskCache.Key = _name;
            }

            SetTask(t);
        };
    }
}