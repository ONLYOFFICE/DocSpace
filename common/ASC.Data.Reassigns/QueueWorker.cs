namespace ASC.Data.Reassigns
{
    public static class QueueWorker
    {
        public static IDictionary<string, StringValues> GetHttpHeaders(HttpRequest httpRequest)
        {
            return httpRequest?.Headers;
        }
    }

    public class QueueWorker<T> where T : DistributedTaskProgress
    {
        protected IHttpContextAccessor HttpContextAccessor { get; }
        protected IServiceProvider ServiceProvider { get; }

        private readonly object _synchRoot = new object();
        protected readonly DistributedTaskQueue Queue;

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
            lock (_synchRoot)
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
            return Start(tenantId, user.Id, () =>
            {
                var result = ServiceProvider.GetService<RemoveProgressItem>();
                result.Init(tenantId, user, currentUserId, notify);

                return result;
            }) as RemoveProgressItem;
        }
    }
}
