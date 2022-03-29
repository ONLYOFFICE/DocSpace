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

using Microsoft.Extensions.Caching.Distributed;

namespace ASC.Common.Threading;

[Transient]
public class DistributedTaskQueue
{
    const string QUEUE_DEFAULT_PREFIX = "asc_distributed_task_queue_";

    private readonly ConcurrentDictionary<string, CancellationTokenSource> _cancelations;
    private readonly IServiceProvider _serviceProvider;
    private readonly ICacheNotify<DistributedTaskCancelation> _cancellationCacheNotify;
    private readonly IDistributedCache _distributedCache;
    private readonly ILog _logger;

    /// <summary>
    /// setup -1 for infinity thread counts
    /// </summary>
    private int _maxThreadsCount = 1; 
    private string _name;
    private TaskScheduler Scheduler { get; set; } = TaskScheduler.Default;

    public DistributedTaskQueue(IServiceProvider serviceProvider,
                                ICacheNotify<DistributedTaskCancelation> cancelTaskNotify,
                                IDistributedCache distributedCache,
                                IOptionsMonitor<ILog> options)

    {
        _distributedCache = distributedCache;
        _serviceProvider = serviceProvider;
        _cancellationCacheNotify = cancelTaskNotify;
        _cancelations = new ConcurrentDictionary<string, CancellationTokenSource>();
        _logger = options.CurrentValue;

        _cancellationCacheNotify.Subscribe((c) =>
        {
            if (_cancelations.TryGetValue(c.Id, out var s))
            {
                s.Cancel();
            }
        }, CacheNotifyAction.Remove);
    }

    public string Name
    {
        get => _name;
        set => _name = QUEUE_DEFAULT_PREFIX + value;
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

    public void EnqueueTask(DistributedTaskProgress taskProgress)
    {
        EnqueueTask((a, b) => taskProgress.RunJob(), taskProgress);
    }

    public void EnqueueTask(Action<DistributedTask, CancellationToken> action, DistributedTask distributedTask = null)
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

        _logger.TraceFormat("EnqueueTask '{DistributedTaskId}' by instanse id '{InstanceId}'", distributedTask.Id, InstanceId);

    }

    public void EnqueueTask(Func<DistributedTask, CancellationToken, Task> action, DistributedTask distributedTask = null)
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

        _logger.TraceFormat("EnqueueTask '{DistributedTaskId}' by instanse id '{InstanceId}'", distributedTask.Id, InstanceId);

    }

    public IEnumerable<DistributedTask> GetAllTasks()
    {
        var serializedObject = _distributedCache.Get(_name);

        if (serializedObject == null) return new List<DistributedTask>();

        using var ms = new MemoryStream(serializedObject);

        var queueTasks = Serializer.Deserialize<List<DistributedTask>>(ms);

        queueTasks.ForEach(t =>
        {
            if (t.Publication == null)
            {
                t.Publication = GetPublication();
            }
        });

        return queueTasks;
    }


    public IEnumerable<T> GetAllTasks<T>() where T : DistributedTask
    {
       return GetAllTasks().Select(x => Map(x, _serviceProvider.GetService<T>()));                       
    }

    public T PeekTask<T>(string id) where T : DistributedTask
    {      
        var taskById = GetAllTasks().FirstOrDefault(x => x.Id == id);

        if (taskById == null) return null;
               
        return Map(taskById, _serviceProvider.GetService<T>());
    }

    public void DequeueTask(string id)
    {
        var queueTasks = GetAllTasks().ToList();

        if (!queueTasks.Exists(x => x.Id == id)) return;

        queueTasks = queueTasks.FindAll(x => x.Id != id);

        using var ms = new MemoryStream();

        Serializer.Serialize(ms, queueTasks);

        _distributedCache.Set(_name, ms.ToArray());

        _logger.TraceFormat("DequeueTask '{DistributedTaskId}' by instanse id '{InstanceId}'", id, InstanceId);

    }

    public void CancelTask(string id)
    {
        _cancellationCacheNotify.Publish(new DistributedTaskCancelation() { Id = id }, CacheNotifyAction.Remove);
    }

    private void OnCompleted(Task task, string id)
    {
        var distributedTask = GetAllTasks().FirstOrDefault(x => x.Id == id);
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
        return (task) =>
        {
            var queueTasks = GetAllTasks().ToList().FindAll(x => x.Id != task.Id);

            queueTasks.Add(task);

            using var ms = new MemoryStream();

            Serializer.Serialize(ms, queueTasks);

            _distributedCache.Set(_name, ms.ToArray());

            _logger.TraceFormat("Publication DistributedTask '{DistributedTaskId}' by instanse id '{InstanceId}' ", task.Id, task.InstanceId);
        };
    }

    /// <summary>
    /// Maps the source object to destination object.
    /// </summary>
    /// <typeparam name="T">Type of destination object.</typeparam>
    /// <typeparam name="TU">Type of source object.</typeparam>
    /// <param name="destination">Destination object.</param>
    /// <param name="source">Source object.</param>
    /// <returns>Updated destination object.</returns>
    private T Map<T, TU>(TU source, T destination)
    {
        destination.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .ToList()
                    .ForEach(field =>
                    {
                        var sf = source.GetType().GetField(field.Name, BindingFlags.NonPublic | BindingFlags.Instance);

                        if (sf != null)
                        {
                            var value = sf.GetValue(source);
                            destination.GetType().GetField(field.Name, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(destination, value);
                        }
                    });

        destination.GetType().GetProperties().Where(p => p.CanWrite == true && !p.GetIndexParameters().Any())
                    .ToList()
                    .ForEach(prop =>
                    {
                        var sp = source.GetType().GetProperty(prop.Name);
                        if (sp != null)
                        {
                            var value = sp.GetValue(source, null);
                            destination.GetType().GetProperty(prop.Name).SetValue(destination, value, null);
                        }
                    });



        return destination;
    }

}