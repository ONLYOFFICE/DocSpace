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
    public const string QUEUE_DEFAULT_PREFIX = "asc_distributed_task_queue_";
    public static readonly int INSTANCE_ID = Process.GetCurrentProcess().Id;

    private readonly ConcurrentDictionary<string, CancellationTokenSource> _cancelations;
    private readonly IServiceProvider _serviceProvider;
    private readonly ICacheNotify<DistributedTaskCancelation> _cancellationCacheNotify;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<DistributedTaskQueue> _logger;
    private bool _subscribed;

    /// <summary>
    /// setup -1 for infinity thread counts
    /// </summary>
    private int _maxThreadsCount = 1;
    private string _name;
    private readonly int _timeUntilUnregisterInSeconds = 60;
    private TaskScheduler Scheduler { get; set; } = TaskScheduler.Default;

    public DistributedTaskQueue(
        IServiceProvider serviceProvider,
        ICacheNotify<DistributedTaskCancelation> cancelTaskNotify,
        IDistributedCache distributedCache,
        ILogger<DistributedTaskQueue> logger)

    {
        _distributedCache = distributedCache;
        _serviceProvider = serviceProvider;
        _cancellationCacheNotify = cancelTaskNotify;
        _cancelations = new ConcurrentDictionary<string, CancellationTokenSource>();
        _logger = logger;
        _subscribed = false;
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

    public void EnqueueTask(DistributedTaskProgress taskProgress)
    {
        EnqueueTask(taskProgress.RunJob, taskProgress);
    }

    public void EnqueueTask(Func<DistributedTask, CancellationToken, Task> action, DistributedTask distributedTask = null)
    {
        if (distributedTask == null)
        {
            distributedTask = new DistributedTask();
        }

        distributedTask.InstanceId = INSTANCE_ID;

        if (distributedTask.LastModifiedOn.Equals(DateTime.MinValue))
        {
            distributedTask.LastModifiedOn = DateTime.UtcNow;
        }

        var cancelation = new CancellationTokenSource();
        var token = cancelation.Token;
        _cancelations[distributedTask.Id] = cancelation;

        if (!_subscribed)
        {
            _cancellationCacheNotify.Subscribe((c) =>
            {
                if (_cancelations.TryGetValue(c.Id, out var s))
                {
                    s.Cancel();
                }
            }, CacheNotifyAction.Remove);

            _subscribed = true;
        }

        var task = new Task(() =>
    {
        var t = action(distributedTask, token);
        t.ConfigureAwait(false)
        .GetAwaiter()
        .OnCompleted(() => OnCompleted(t, distributedTask.Id));
    }, token, TaskCreationOptions.LongRunning);

        task.ConfigureAwait(false);

        distributedTask.Status = DistributedTaskStatus.Running;

        if (distributedTask.Publication == null)
        {
            distributedTask.Publication = GetPublication();
        }
        distributedTask.PublishChanges();

        task.Start(Scheduler);

        _logger.TraceEnqueueTask(distributedTask.Id, INSTANCE_ID);

    }

    public IEnumerable<DistributedTask> GetAllTasks(int? instanceId = null)
    {
        var queueTasks = LoadFromCache();

        queueTasks = DeleteOrphanCacheItem(queueTasks);

        if (instanceId.HasValue)
        {
            queueTasks = queueTasks.Where(x => x.InstanceId == instanceId.Value);
        }

        foreach (var task in queueTasks)
        {
            if (task.Publication == null)
            {
                task.Publication = GetPublication();
            }
        }

        return queueTasks;
    }

    public IEnumerable<T> GetAllTasks<T>() where T : DistributedTask
    {
        return GetAllTasks().Select(x => Map(x, _serviceProvider.GetService<T>()));
    }

    public T PeekTask<T>(string id) where T : DistributedTask
    {
        var taskById = GetAllTasks().FirstOrDefault(x => x.Id == id);

        if (taskById == null)
        {
            return null;
        }

        return Map(taskById, _serviceProvider.GetService<T>());
    }

    public void DequeueTask(string id)
    {
        var queueTasks = GetAllTasks().ToList();

        if (!queueTasks.Exists(x => x.Id == id))
        {
            return;
        }

        _cancellationCacheNotify.Publish(new DistributedTaskCancelation() { Id = id }, CacheNotifyAction.Remove);

        queueTasks = queueTasks.FindAll(x => x.Id != id);

        if (queueTasks.Count == 0)
        {
            _distributedCache.Remove(_name);
        }
        else
        {
            SaveToCache(queueTasks);
        }

        _logger.TraceEnqueueTask(id, INSTANCE_ID);

    }

    private void OnCompleted(Task task, string id)
    {
        var distributedTask = GetAllTasks().FirstOrDefault(x => x.Id == id);
        if (distributedTask != null)
        {
            distributedTask.Status = DistributedTaskStatus.Completed;
            if (task.Exception != null)
            {
                distributedTask.Exception = task.Exception;
            }
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

            task.LastModifiedOn = DateTime.UtcNow;

            queueTasks.Add(task);

            SaveToCache(queueTasks);

            _logger.TracePublicationDistributedTask(task.Id, task.InstanceId);
        };
    }


    private void SaveToCache(IEnumerable<DistributedTask> queueTasks)
    {
        if (!queueTasks.Any())
        {
            _distributedCache.Remove(_name);

            return;
        }

        using var ms = new MemoryStream();

        Serializer.Serialize(ms, queueTasks);

        _distributedCache.Set(_name, ms.ToArray(), new DistributedCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(15)
        });

    }

    private IEnumerable<DistributedTask> LoadFromCache()
    {
        var serializedObject = _distributedCache.Get(_name);

        if (serializedObject == null)
        {
            return new List<DistributedTask>();
        }

        using var ms = new MemoryStream(serializedObject);

        return Serializer.Deserialize<List<DistributedTask>>(ms);
    }

    private IEnumerable<DistributedTask> DeleteOrphanCacheItem(IEnumerable<DistributedTask> queueTasks)
    {
        var listTasks = queueTasks.ToList();

        if (listTasks.RemoveAll(IsOrphanCacheItem) > 0)
        {
            SaveToCache(listTasks);
        }

        return listTasks;
    }

    private bool IsOrphanCacheItem(DistributedTask obj)
    {
        return obj.LastModifiedOn.AddSeconds(_timeUntilUnregisterInSeconds) < DateTime.UtcNow;
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