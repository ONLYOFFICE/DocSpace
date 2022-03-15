namespace ASC.Web.Studio.Core.Statistic
{
    [Scope]
    public class StatisticManager
    {
        private static DateTime lastSave = DateTime.UtcNow;
        private static readonly TimeSpan cacheTime = TimeSpan.FromMinutes(2);
        private static readonly IDictionary<string, UserVisit> cache = new Dictionary<string, UserVisit>();

        private Lazy<WebstudioDbContext> LazyWebstudioDbContext { get; }
        private WebstudioDbContext WebstudioDbContext { get => LazyWebstudioDbContext.Value; }

        public StatisticManager(DbContextManager<WebstudioDbContext> dbContextManager)
        {
            LazyWebstudioDbContext = new Lazy<WebstudioDbContext>(() => dbContextManager.Value);
        }

        public void SaveUserVisit(int tenantID, Guid userID, Guid productID)
        {
            var now = DateTime.UtcNow;
            var key = string.Format("{0}|{1}|{2}|{3}", tenantID, userID, productID, now.Date);

            lock (cache)
            {
                var visit = cache.ContainsKey(key) ?
                                cache[key] :
                                new UserVisit
                                {
                                    TenantID = tenantID,
                                    UserID = userID,
                                    ProductID = productID,
                                    VisitDate = now
                                };

                visit.VisitCount++;
                visit.LastVisitTime = now;
                cache[key] = visit;
            }

            if (cacheTime < DateTime.UtcNow - lastSave)
            {
                FlushCache();
            }
        }

        public List<Guid> GetVisitorsToday(int tenantID, Guid productID)
        {
            var users = WebstudioDbContext.WebstudioUserVisit
                .Where(r => r.VisitDate == DateTime.UtcNow.Date)
                .Where(r => r.TenantId == tenantID)
                .Where(r => r.ProductId == productID)
                .OrderBy(r => r.FirstVisitTime)
                .GroupBy(r => r.UserId)
                .Select(r => r.Key)
                .ToList();

            lock (cache)
            {
                foreach (var visit in cache.Values)
                {
                    if (!users.Contains(visit.UserID) && visit.VisitDate.Date == DateTime.UtcNow.Date)
                    {
                        users.Add(visit.UserID);
                    }
                }
            }
            return users;
        }

        public List<UserVisit> GetHitsByPeriod(int tenantID, DateTime startDate, DateTime endPeriod)
        {
            return WebstudioDbContext.WebstudioUserVisit
                .Where(r => r.TenantId == tenantID)
                .Where(r => r.VisitDate >= startDate && r.VisitDate <= endPeriod)
                .OrderBy(r => r.VisitDate)
                .GroupBy(r => r.VisitDate)
                .Select(r => new UserVisit { VisitDate = r.Key, VisitCount = r.Sum(a => a.VisitCount) })
                .ToList();
        }

        public List<UserVisit> GetHostsByPeriod(int tenantID, DateTime startDate, DateTime endPeriod)
        {
            return
                WebstudioDbContext.WebstudioUserVisit
                .Where(r => r.TenantId == tenantID)
                .Where(r => r.VisitDate >= startDate && r.VisitDate <= endPeriod)
                .OrderBy(r => r.VisitDate)
                .GroupBy(r => new { r.UserId, r.VisitDate })
                .Select(r => new UserVisit { VisitDate = r.Key.VisitDate, UserID = r.Key.UserId })
                .ToList();
        }

        private void FlushCache()
        {
            if (cache.Count == 0) return;

            List<UserVisit> visits;
            lock (cache)
            {
                visits = new List<UserVisit>(cache.Values);
                cache.Clear();
                lastSave = DateTime.UtcNow;
            }

            using var tx = WebstudioDbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
            foreach (var v in visits)
            {
                var w = new DbWebstudioUserVisit
                {
                    TenantId = v.TenantID,
                    ProductId = v.ProductID,
                    UserId = v.UserID,
                    VisitDate = v.VisitDate.Date,
                    FirstVisitTime = v.VisitDate,
                    VisitCount = v.VisitCount
                };

                if (v.LastVisitTime.HasValue)
                {
                    w.LastVisitTime = v.LastVisitTime.Value;
                }

                WebstudioDbContext.WebstudioUserVisit.Add(w);
                WebstudioDbContext.SaveChanges();
            }
            tx.Commit();
        }
    }
}