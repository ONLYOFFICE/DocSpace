namespace ASC.Feed.Aggregator.Service;

[Singletone]
public class FeedAggregatorService : FeedBaseService
{
    protected override string LoggerName { get; set; } = "ASC.Feed.Aggregator";

    private readonly SignalrServiceClient _signalrServiceClient;

    public FeedAggregatorService(
        FeedSettings feedSettings,
        IServiceScopeFactory serviceScopeFactory,
        IOptionsMonitor<ILog> optionsMonitor,
        SignalrServiceClient signalrServiceClient)
        : base(feedSettings, serviceScopeFactory, optionsMonitor)
    {
        _signalrServiceClient = signalrServiceClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Logger.Info("Feed Aggregator service running.");

        var cfg = FeedSettings;

        while (!stoppingToken.IsCancellationRequested)
        {
            AggregateFeeds(cfg.AggregateInterval);

            await Task.Delay(cfg.AggregatePeriod, stoppingToken);
        }

        Logger.Info("Feed Aggregator Service stopping.");
    }

    private static T Attempt<T>(int count, Func<T> action)
    {
        var counter = 0;
        while (true)
        {
            try
            {
                return action();
            }
            catch
            {
                if (count < ++counter)
                {
                    throw;
                }
            }
        }
    }

    private static bool TryAuthenticate(SecurityContext securityContext, AuthManager authManager, int tenantId, Guid userid)
    {
        try
        {
            securityContext.AuthenticateMeWithoutCookie(authManager.GetAccountByID(tenantId, userid));
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void AggregateFeeds(object interval)
    {
        try
        {
            var cfg = FeedSettings;
            using var scope = ServiceScopeFactory.CreateScope();
            var cache = scope.ServiceProvider.GetService<ICache>();
            var baseCommonLinkUtility = scope.ServiceProvider.GetService<BaseCommonLinkUtility>();
            baseCommonLinkUtility.Initialize(cfg.ServerRoot);

            var start = DateTime.UtcNow;
            Logger.DebugFormat("Start of collecting feeds...");

            var unreadUsers = new Dictionary<int, Dictionary<Guid, int>>();
            var modules = scope.ServiceProvider.GetService<IEnumerable<IFeedModule>>();

            var feedAggregateDataProvider = scope.ServiceProvider.GetService<FeedAggregateDataProvider>();
            var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
            var userManager = scope.ServiceProvider.GetService<UserManager>();
            var authManager = scope.ServiceProvider.GetService<AuthManager>();
            var securityContext = scope.ServiceProvider.GetService<SecurityContext>();

            foreach (var module in modules)
            {
                var result = new List<FeedRow>();
                var fromTime = feedAggregateDataProvider.GetLastTimeAggregate(module.GetType().Name);
                if (fromTime == default) fromTime = DateTime.UtcNow.Subtract((TimeSpan)interval);
                var toTime = DateTime.UtcNow;

                var tenants = Attempt(10, () => module.GetTenantsWithFeeds(fromTime)).ToList();
                Logger.DebugFormat("Find {1} tenants for module {0}.", module.GetType().Name, tenants.Count);

                foreach (var tenant in tenants)
                {
                    // Warning! There is hack here!
                    // clearing the cache to get the correct acl
                    cache.Remove("acl" + tenant);
                    cache.Remove("/webitemsecurity/" + tenant);
                    //cache.Remove(string.Format("sub/{0}/{1}/{2}", tenant, "6045b68c-2c2e-42db-9e53-c272e814c4ad", NotifyConstants.Event_NewCommentForMessage.ID));

                    try
                    {
                        if (tenantManager.GetTenant(tenant) == null)
                        {
                            continue;
                        }

                        tenantManager.SetCurrentTenant(tenant);
                        var users = userManager.GetUsers();

                        var feeds = Attempt(10, () => module.GetFeeds(new FeedFilter(fromTime, toTime) { Tenant = tenant }).Where(r => r.Item1 != null).ToList());
                        Logger.DebugFormat("{0} feeds in {1} tenant.", feeds.Count, tenant);

                        var tenant1 = tenant;
                        var module1 = module;
                        var feedsRow = feeds
                            .Select(tuple => new Tuple<FeedRow, object>(new FeedRow(tuple.Item1)
                            {
                                Tenant = tenant1,
                                Product = module1.Product
                            }, tuple.Item2))
                            .ToList();

                        foreach (var u in users)
                        {
                            if (!TryAuthenticate(securityContext, authManager, tenant1, u.Id))
                            {
                                continue;
                            }

                            module.VisibleFor(feedsRow, u.Id);
                        }

                        result.AddRange(feedsRow.Select(r => r.Item1));
                    }
                    catch (Exception ex)
                    {
                        Logger.ErrorFormat("Tenant: {0}, {1}", tenant, ex);
                    }
                }

                feedAggregateDataProvider.SaveFeeds(result, module.GetType().Name, toTime);

                foreach (var res in result)
                {
                    foreach (var userGuid in res.Users.Where(userGuid => !userGuid.Equals(res.ModifiedBy)))
                    {
                        if (!unreadUsers.TryGetValue(res.Tenant, out var dictionary))
                        {
                            dictionary = new Dictionary<Guid, int>();
                        }
                        if (dictionary.ContainsKey(userGuid))
                        {
                            ++dictionary[userGuid];
                        }
                        else
                        {
                            dictionary.Add(userGuid, 1);
                        }

                        unreadUsers[res.Tenant] = dictionary;
                    }
                }
            }

            _signalrServiceClient.SendUnreadUsers(unreadUsers);

            Logger.DebugFormat("Time of collecting news: {0}", DateTime.UtcNow - start);
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }
}