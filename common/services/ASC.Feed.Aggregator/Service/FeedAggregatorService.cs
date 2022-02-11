namespace ASC.Feed.Aggregator.Service
{
    public class FeedAggregatorService : FeedBaseService
    {
        private readonly SignalrServiceClient _signalrServiceClient;

        public FeedAggregatorService(
            FeedSettings feedSettings,
            IServiceProvider serviceProvider,
            IOptionsMonitor<ILog> optionsMonitor,
            SignalrServiceClient signalrServiceClient)
            : base(feedSettings, serviceProvider, optionsMonitor)
        {
            _signalrServiceClient = signalrServiceClient;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.Info("Feed Aggregator service running.");

            var cfg = FeedSettings;
            IsStopped = false;

            Timer = new Timer(AggregateFeeds, cfg.AggregateInterval, TimeSpan.Zero, cfg.AggregatePeriod);

            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.Info("Feed Aggregator service stopping.");

            IsStopped = true;

            Timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private void AggregateFeeds(object interval)
        {
            if (!Monitor.TryEnter(LockObj))
            {
                return;
            }

            try
            {
                var cfg = FeedSettings;
                using var scope = ServiceProvider.CreateScope();
                var scopeClass = scope.ServiceProvider.GetService<FeedAggregatorServiceScope>();
                var cache = scope.ServiceProvider.GetService<ICache>();
                var (baseCommonLinkUtility, tenantManager, feedAggregateDataProvider, userManager, securityContext, authManager) = scopeClass;
                baseCommonLinkUtility.Initialize(cfg.ServerRoot);

                var start = DateTime.UtcNow;
                Logger.DebugFormat("Start of collecting feeds...");

                var unreadUsers = new Dictionary<int, Dictionary<Guid, int>>();
                var modules = scope.ServiceProvider.GetService<IEnumerable<IFeedModule>>();

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
                                    ProductId = module1.Product
                                }, tuple.Item2))
                                .ToList();

                            foreach (var u in users)
                            {
                                if (IsStopped)
                                {
                                    return;
                                }
                                if (!TryAuthenticate(securityContext, authManager, tenant1, u.ID))
                                {
                                    continue;
                                }

                                module.VisibleFor(feedsRow, u.ID);
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
                        foreach (var userGuid in res.Users.Where(userGuid => !userGuid.Equals(res.ModifiedById)))
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
            finally
            {
                Monitor.Exit(LockObj);
            }
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
    }

    [Scope]
    public class FeedAggregatorServiceScope
    {
        private readonly BaseCommonLinkUtility _baseCommonLinkUtility;
        private readonly TenantManager _tenantManager;
        private readonly FeedAggregateDataProvider _feedAggregateDataProvider;
        private readonly UserManager _userManager;
        private readonly SecurityContext _securityContext;
        private readonly AuthManager _authManager;

        public FeedAggregatorServiceScope(BaseCommonLinkUtility baseCommonLinkUtility,
            TenantManager tenantManager,
            FeedAggregateDataProvider feedAggregateDataProvider,
            UserManager userManager,
            SecurityContext securityContext,
            AuthManager authManager)
        {
            _baseCommonLinkUtility = baseCommonLinkUtility;
            _tenantManager = tenantManager;
            _feedAggregateDataProvider = feedAggregateDataProvider;
            _userManager = userManager;
            _securityContext = securityContext;
            _authManager = authManager;
        }

        public void Deconstruct(out BaseCommonLinkUtility baseCommonLinkUtility,
            out TenantManager tenantManager,
            out FeedAggregateDataProvider feedAggregateDataProvider,
            out UserManager userManager,
            out SecurityContext securityContext,
            out AuthManager authManager)
        {
            baseCommonLinkUtility = _baseCommonLinkUtility;
            tenantManager = _tenantManager;
            feedAggregateDataProvider = _feedAggregateDataProvider;
            userManager = _userManager;
            securityContext = _securityContext;
            authManager = _authManager;
        }
    }

    public static class FeedAggregatorServiceExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<FeedAggregatorServiceScope>();
        }
    }
}