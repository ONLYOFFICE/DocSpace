namespace ASC.Notify;

[Singletone(Additional = typeof(DbWorkerExtension))]
public class DbWorker
{
    private readonly string _dbid;
    private readonly object _syncRoot = new object();
    private readonly IMapper _mapper;

    private readonly IServiceScopeFactory _serviceScopeFactory;
    public NotifyServiceCfg NotifyServiceCfg { get; }

    public DbWorker(IServiceScopeFactory serviceScopeFactory, IOptions<NotifyServiceCfg> notifyServiceCfg, IMapper mapper)
    {
        _serviceScopeFactory = serviceScopeFactory;
        NotifyServiceCfg = notifyServiceCfg.Value;
        _dbid = NotifyServiceCfg.ConnectionStringName;
        _mapper = mapper;
    }

    public int SaveMessage(NotifyMessage m)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetService<DbContextManager<NotifyDbContext>>().Get(_dbid);
        using var tx = dbContext.Database.BeginTransaction(IsolationLevel.ReadCommitted);

        var notifyQueue = _mapper.Map<NotifyMessage, NotifyQueue>(m);

        notifyQueue = dbContext.NotifyQueue.Add(notifyQueue).Entity;
        dbContext.SaveChanges();

        var id = notifyQueue.NotifyId;

        var info = new NotifyInfo
        {
            NotifyId = id,
            State = 0,
            Attempts = 0,
            ModifyDate = DateTime.UtcNow,
            Priority = m.Priority
        };

        dbContext.NotifyInfo.Add(info);
        dbContext.SaveChanges();

        tx.Commit();

        return 1;
    }

    public IDictionary<int, NotifyMessage> GetMessages(int count)
    {
        lock (_syncRoot)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            using var dbContext = scope.ServiceProvider.GetService<DbContextManager<NotifyDbContext>>().Get(_dbid);
            using var tx = dbContext.Database.BeginTransaction();

            var q = dbContext.NotifyQueue
                .Join(dbContext.NotifyInfo, r => r.NotifyId, r => r.NotifyId, (queue, info) => new { queue, info })
                .Where(r => r.info.State == (int)MailSendingState.NotSended || r.info.State == (int)MailSendingState.Error && r.info.ModifyDate < DateTime.UtcNow - TimeSpan.Parse(NotifyServiceCfg.Process.AttemptsInterval))
                .OrderBy(i => i.info.Priority)
                .ThenBy(i => i.info.NotifyId)
                .Take(count);


            var messages = q
                .ToDictionary(
                    r => r.queue.NotifyId,
                    r =>
                    {
                        var res = _mapper.Map<NotifyQueue, NotifyMessage>(r.queue);

                        try
                        {
                            res.Attachments.AddRange(JsonConvert.DeserializeObject<RepeatedField<NotifyMessageAttachment>>(r.queue.Attachments));
                        }
                        catch (Exception)
                        {

                        }

                        return res;
                    });

            var info = dbContext.NotifyInfo.Where(r => messages.Keys.Any(a => a == r.NotifyId)).ToList();

            foreach (var i in info)
            {
                i.State = (int)MailSendingState.Sending;
            }

            dbContext.SaveChanges();
            tx.Commit();

            return messages;
        }
    }

    public void ResetStates()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetService<DbContextManager<NotifyDbContext>>().Get(_dbid);

        var tr = dbContext.Database.BeginTransaction();
        var info = dbContext.NotifyInfo.Where(r => r.State == 1).ToList();

        foreach (var i in info)
        {
            i.State = 0;
        }

        dbContext.SaveChanges();
        tr.Commit();
    }

    public void SetState(int id, MailSendingState result)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetService<DbContextManager<NotifyDbContext>>().Get(_dbid);
        using var tx = dbContext.Database.BeginTransaction();

        if (result == MailSendingState.Sended)
        {
            var d = dbContext.NotifyInfo.Where(r => r.NotifyId == id).FirstOrDefault();
            dbContext.NotifyInfo.Remove(d);
            dbContext.SaveChanges();
        }
        else
        {
            if (result == MailSendingState.Error)
            {
                var attempts = dbContext.NotifyInfo.Where(r => r.NotifyId == id).Select(r => r.Attempts).FirstOrDefault();
                if (NotifyServiceCfg.Process.MaxAttempts <= attempts + 1)
                {
                    result = MailSendingState.FatalError;
                }
            }

            var info = dbContext.NotifyInfo
                .Where(r => r.NotifyId == id)
                .ToList();

            foreach (var i in info)
            {
                i.State = (int)result;
                i.Attempts += 1;
                i.ModifyDate = DateTime.UtcNow;
            }

            dbContext.SaveChanges();
        }

        tx.Commit();
    }
}

public static class DbWorkerExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<DbContextManager<NotifyDbContext>>();
    }
}
