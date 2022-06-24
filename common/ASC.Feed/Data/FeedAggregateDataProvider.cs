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



namespace ASC.Feed.Data;

[Scope]
public class FeedAggregateDataProvider
{
    private FeedDbContext FeedDbContext => _lazyFeedDbContext.Value;

    private readonly AuthContext _authContext;
    private readonly TenantManager _tenantManager;
    private readonly Lazy<FeedDbContext> _lazyFeedDbContext;
    private readonly IMapper _mapper;

    public FeedAggregateDataProvider(
        AuthContext authContext,
        TenantManager tenantManager,
        DbContextManager<FeedDbContext> dbContextManager,
        IMapper mapper)
        : this(authContext, tenantManager, mapper)
    {
        _lazyFeedDbContext = new Lazy<FeedDbContext>(() => dbContextManager.Get(Constants.FeedDbId));
    }

    public FeedAggregateDataProvider(
        AuthContext authContext,
        TenantManager tenantManager,
        IMapper mapper)
    {
        _authContext = authContext;
        _tenantManager = tenantManager;
        _mapper = mapper;
    }

    public DateTime GetLastTimeAggregate(string key)
    {
        var value = FeedDbContext.FeedLast.Where(r => r.LastKey == key).Select(r => r.LastDate).FirstOrDefault();

        return value != default ? value.AddSeconds(1) : value;
    }

    public void SaveFeeds(IEnumerable<FeedRow> feeds, string key, DateTime value)
    {
        var feedLast = new FeedLast
        {
            LastKey = key,
            LastDate = value
        };

        FeedDbContext.AddOrUpdate(r => r.FeedLast, feedLast);
        FeedDbContext.SaveChanges();

        const int feedsPortionSize = 1000;
        var aggregatedDate = DateTime.UtcNow;

        var feedsPortion = new List<FeedRow>();
        foreach (var feed in feeds)
        {
            feedsPortion.Add(feed);
            if (feedsPortion.Sum(f => f.Users.Count) <= feedsPortionSize)
            {
                continue;
            }

            SaveFeedsPortion(feedsPortion, aggregatedDate);
            feedsPortion.Clear();
        }

        if (feedsPortion.Count > 0)
        {
            SaveFeedsPortion(feedsPortion, aggregatedDate);
        }
    }

    private void SaveFeedsPortion(IEnumerable<FeedRow> feeds, DateTime aggregatedDate)
    {
        var strategy = FeedDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var tx = FeedDbContext.Database.BeginTransaction();

            foreach (var f in feeds)
            {
                if (0 >= f.Users.Count)
                {
                    continue;
                }

                var feedAggregate = _mapper.Map<FeedRow, FeedAggregate>(f);
                feedAggregate.AggregateDate = aggregatedDate;

                if (f.ClearRightsBeforeInsert)
                {
                    var fu = FeedDbContext.FeedUsers.Where(r => r.FeedId == f.Id).FirstOrDefault();
                    if (fu != null)
                    {
                        FeedDbContext.FeedUsers.Remove(fu);
                    }
                }

                FeedDbContext.AddOrUpdate(r => r.FeedAggregates, feedAggregate);

                foreach (var u in f.Users)
                {
                    var feedUser = new FeedUsers
                    {
                        FeedId = f.Id,
                        UserId = u
                    };

                    FeedDbContext.AddOrUpdate(r => r.FeedUsers, feedUser);
                }
            }

            FeedDbContext.SaveChanges();

            tx.Commit();
        });
    }

    public void RemoveFeedAggregate(DateTime fromTime)
    {
        var strategy = FeedDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var tx = FeedDbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

            var aggregates = FeedDbContext.FeedAggregates.Where(r => r.AggregateDate <= fromTime);
            FeedDbContext.FeedAggregates.RemoveRange(aggregates);

            var users = FeedDbContext.FeedUsers.Where(r => FeedDbContext.FeedAggregates.Where(r => r.AggregateDate <= fromTime).Any(a => a.Id == r.FeedId));
            FeedDbContext.FeedUsers.RemoveRange(users);

            tx.Commit();
        });
    }

    public List<FeedResultItem> GetFeeds(FeedApiFilter filter)
    {
        var filterOffset = filter.Offset;
        var filterLimit = filter.Max > 0 && filter.Max < 1000 ? filter.Max : 1000;

        var feeds = new Dictionary<string, List<FeedResultItem>>();

        var tryCount = 0;
        List<FeedResultItem> feedsIteration;
        do
        {
            feedsIteration = GetFeedsInternal(filter);
            foreach (var feed in feedsIteration)
            {
                if (feeds.TryGetValue(feed.GroupId, out var value))
                {
                    value.Add(feed);
                }
                else
                {
                    feeds[feed.GroupId] = new List<FeedResultItem> { feed };
                }
            }
            filter.Offset += feedsIteration.Count;
        } while (feeds.Count < filterLimit
                 && feedsIteration.Count == filterLimit
                 && tryCount++ < 5);

        filter.Offset = filterOffset;

        return feeds.Take(filterLimit).SelectMany(group => group.Value).ToList();
    }

    private List<FeedResultItem> GetFeedsInternal(FeedApiFilter filter)
    {
        var q = FeedDbContext.FeedAggregates
            .Where(r => r.Tenant == _tenantManager.GetCurrentTenant().Id)
            .Where(r => r.ModifiedBy != _authContext.CurrentAccount.ID)
            .Join(FeedDbContext.FeedUsers, a => a.Id, b => b.FeedId, (aggregates, users) => new { aggregates, users })
            .Where(r => r.users.UserId == _authContext.CurrentAccount.ID)
            .OrderByDescending(r => r.aggregates.ModifiedDate)
            .Skip(filter.Offset)
            .Take(filter.Max);

        if (filter.OnlyNew)
        {
            q = q.Where(r => r.aggregates.AggregateDate >= filter.From);
        }
        else
        {
            if (1 < filter.From.Year)
            {
                q = q.Where(r => r.aggregates.ModifiedDate >= filter.From);
            }
            if (filter.To.Year < 9999)
            {
                q = q.Where(r => r.aggregates.ModifiedDate <= filter.To);
            }
        }

        if (!string.IsNullOrEmpty(filter.Product))
        {
            q = q.Where(r => r.aggregates.Product == filter.Product);
        }

        if (filter.Author != Guid.Empty)
        {
            q = q.Where(r => r.aggregates.ModifiedBy == filter.Author);
        }

        if (filter.SearchKeys != null && filter.SearchKeys.Length > 0)
        {
            var keys = filter.SearchKeys
                            .Where(s => !string.IsNullOrEmpty(s))
                            .Select(s => s.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_"))
                            .ToList();

            q = q.Where(r => keys.Any(k => r.aggregates.Keywords.StartsWith(k)));
        }

        var news = q.Select(r => r.aggregates).AsEnumerable();

        return _mapper.Map<IEnumerable<FeedAggregate>, List<FeedResultItem>>(news);
    }

    public int GetNewFeedsCount(DateTime lastReadedTime, AuthContext authContext, TenantManager tenantManager)
    {
        var count = FeedDbContext.FeedAggregates
            .Where(r => r.Tenant == tenantManager.GetCurrentTenant().Id)
            .Where(r => r.ModifiedBy != authContext.CurrentAccount.ID)
            .Join(FeedDbContext.FeedUsers, r => r.Id, u => u.FeedId, (agg, user) => new { agg, user })
            .Where(r => r.user.UserId == authContext.CurrentAccount.ID);

        if (1 < lastReadedTime.Year)
        {
            count = count.Where(r => r.agg.AggregateDate >= lastReadedTime);
        }

        return count.Take(1001).Select(r => r.agg.Id).Count();
    }

    public IEnumerable<int> GetTenants(TimeInterval interval)
    {
        return FeedDbContext.FeedAggregates
            .Where(r => r.AggregateDate >= interval.From && r.AggregateDate <= interval.To)
            .GroupBy(r => r.Tenant)
            .Select(r => r.Key)
            .ToList();
    }

    public FeedResultItem GetFeedItem(string id)
    {
        var news =
            FeedDbContext.FeedAggregates
            .Where(r => r.Id == id)
            .FirstOrDefault();

        return _mapper.Map<FeedAggregate, FeedResultItem>(news);
    }

    public void RemoveFeedItem(string id)
    {
        var strategy = FeedDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var tx = FeedDbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

            var aggregates = FeedDbContext.FeedAggregates.Where(r => r.Id == id);
            FeedDbContext.FeedAggregates.RemoveRange(aggregates);

            var users = FeedDbContext.FeedUsers.Where(r => r.FeedId == id);
            FeedDbContext.FeedUsers.RemoveRange(users);

            FeedDbContext.SaveChanges();

            tx.Commit();
        });
    }
}

public class FeedResultItem : IMapFrom<FeedAggregate>
{
    public string Json { get; private set; }
    public string Module { get; private set; }
    public Guid AuthorId { get; private set; }
    public Guid ModifiedById { get; private set; }
    public string GroupId { get; private set; }
    public bool IsToday { get; private set; }
    public bool IsYesterday { get; private set; }
    public bool IsTomorrow { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime ModifiedDate { get; private set; }
    public DateTime AggregatedDate { get; private set; }

    public FeedResultItem() { }

    public FeedResultItem(
        string json,
        string module,
        Guid authorId,
        Guid modifiedById,
        string groupId,
        DateTime createdDate,
        DateTime modifiedDate,
        DateTime aggregatedDate,
        TenantUtil tenantUtil)
    {
        var now = tenantUtil.DateTimeFromUtc(DateTime.UtcNow);

        Json = json;
        Module = module;

        AuthorId = authorId;
        ModifiedById = modifiedById;

        GroupId = groupId;

        var compareDate = JsonNode.Parse(Json)["IsAllDayEvent"].GetValue<bool>()
                ? tenantUtil.DateTimeToUtc(createdDate).Date
                : createdDate.Date;

        if (now.Date == compareDate.AddDays(-1))
        {
            IsTomorrow = true;
        }
        else if (now.Date == compareDate)
        {
            IsToday = true;
        }
        else if (now.Date == compareDate.AddDays(1))
        {
            IsYesterday = true;
        }

        CreatedDate = createdDate;
        ModifiedDate = modifiedDate;
        AggregatedDate = aggregatedDate;
    }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<FeedAggregate, FeedResultItem>()
            .ConvertUsing<FeedTypeConverter>();
    }
}
