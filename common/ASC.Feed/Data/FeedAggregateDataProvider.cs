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
    private readonly AuthContext _authContext;
    private readonly TenantManager _tenantManager;
    private readonly IMapper _mapper;
    private readonly IDbContextFactory<FeedDbContext> _dbContextFactory;

    public FeedAggregateDataProvider(
        AuthContext authContext,
        TenantManager tenantManager,
        IDbContextFactory<FeedDbContext> dbContextFactory,
        IMapper mapper)
        : this(authContext, tenantManager, mapper)
    {
        _dbContextFactory = dbContextFactory;
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
        using var feedDbContext = _dbContextFactory.CreateDbContext();
        var value = feedDbContext.FeedLast.Where(r => r.LastKey == key).Select(r => r.LastDate).FirstOrDefault();

        return value != default ? value.AddSeconds(1) : value;
    }

    public void SaveFeeds(IEnumerable<FeedRow> feeds, string key, DateTime value)
    {
        var feedLast = new FeedLast
        {
            LastKey = key,
            LastDate = value
        };

        using var feedDbContext = _dbContextFactory.CreateDbContext();
        feedDbContext.AddOrUpdate(r => r.FeedLast, feedLast);
        feedDbContext.SaveChanges();

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
        using var feedDbContext = _dbContextFactory.CreateDbContext();
        var strategy = feedDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var feedDbContext = _dbContextFactory.CreateDbContext();
            using var tx = feedDbContext.Database.BeginTransaction();

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
                    var fu = feedDbContext.FeedUsers.Where(r => r.FeedId == f.Id).FirstOrDefault();
                    if (fu != null)
                    {
                        feedDbContext.FeedUsers.Remove(fu);
                    }
                }

                feedDbContext.AddOrUpdate(r => r.FeedAggregates, feedAggregate);

                foreach (var u in f.Users)
                {
                    var feedUser = new FeedUsers
                    {
                        FeedId = f.Id,
                        UserId = u
                    };

                    feedDbContext.AddOrUpdate(r => r.FeedUsers, feedUser);
                }
            }

            feedDbContext.SaveChanges();

            tx.Commit();
        });
    }

    public void RemoveFeedAggregate(DateTime fromTime)
    {
        using var feedDbContext = _dbContextFactory.CreateDbContext();
        var strategy = feedDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var feedDbContext = _dbContextFactory.CreateDbContext();
            using var tx = feedDbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

            var aggregates = feedDbContext.FeedAggregates.Where(r => r.AggregateDate <= fromTime);
            feedDbContext.FeedAggregates.RemoveRange(aggregates);

            var users = feedDbContext.FeedUsers.Where(r => feedDbContext.FeedAggregates.Where(r => r.AggregateDate <= fromTime).Any(a => a.Id == r.FeedId));
            feedDbContext.FeedUsers.RemoveRange(users);

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
        using var feedDbContext = _dbContextFactory.CreateDbContext();
        var q = feedDbContext.FeedAggregates.AsNoTracking()
            .Where(r => r.Tenant == _tenantManager.GetCurrentTenant().Id);

        var exp = GetIdSearchExpression(filter.Id, filter.Module, filter.WithRelated);

        if (exp != null)
        {
            q = q.Where(exp);
        }

        var q1 = q.Join(feedDbContext.FeedUsers, a => a.Id, b => b.FeedId, (aggregates, users) => new { aggregates, users })
            .OrderByDescending(r => r.aggregates.ModifiedDate)
            .Skip(filter.Offset)
            .Take(filter.Max);

        if (exp == null)
        {
            q1 = q1.Where(r => r.aggregates.ModifiedBy != _authContext.CurrentAccount.ID).
                Where(r => r.users.UserId == _authContext.CurrentAccount.ID);
        }

        if (filter.OnlyNew)
        {
            q1 = q1.Where(r => r.aggregates.AggregateDate >= filter.From);
        }
        else
        {
            if (1 < filter.From.Year)
            {
                q1 = q1.Where(r => r.aggregates.ModifiedDate >= filter.From);
            }
            if (filter.To.Year < 9999)
            {
                q1 = q1.Where(r => r.aggregates.ModifiedDate <= filter.To);
            }
        }

        if (!string.IsNullOrEmpty(filter.Product))
        {
            q1 = q1.Where(r => r.aggregates.Product == filter.Product);
        }

        if (filter.Author != Guid.Empty)
        {
            q1 = q1.Where(r => r.aggregates.ModifiedBy == filter.Author);
        }

        if (filter.SearchKeys != null && filter.SearchKeys.Length > 0)
        {
            var keys = filter.SearchKeys
                            .Where(s => !string.IsNullOrEmpty(s))
                            .Select(s => s.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_"))
                            .ToList();

            q1 = q1.Where(r => keys.Any(k => r.aggregates.Keywords.StartsWith(k)));
        }

        var news = q1.Select(r => r.aggregates).AsEnumerable();

        return _mapper.Map<IEnumerable<FeedAggregate>, List<FeedResultItem>>(news);
    }

    public int GetNewFeedsCount(DateTime lastReadedTime)
    {
        using var feedDbContext = _dbContextFactory.CreateDbContext();
        var count = feedDbContext.FeedAggregates
            .Where(r => r.Tenant == _tenantManager.GetCurrentTenant().Id)
            .Where(r => r.ModifiedBy != _authContext.CurrentAccount.ID)
            .Join(feedDbContext.FeedUsers, r => r.Id, u => u.FeedId, (agg, user) => new { agg, user })
            .Where(r => r.user.UserId == _authContext.CurrentAccount.ID);

        if (1 < lastReadedTime.Year)
        {
            count = count.Where(r => r.agg.AggregateDate >= lastReadedTime);
        }

        return count.Take(1001).Select(r => r.agg.Id).Count();
    }

    public IEnumerable<int> GetTenants(TimeInterval interval)
    {
        using var feedDbContext = _dbContextFactory.CreateDbContext();
        return feedDbContext.FeedAggregates
            .Where(r => r.AggregateDate >= interval.From && r.AggregateDate <= interval.To)
            .GroupBy(r => r.Tenant)
            .Select(r => r.Key)
            .ToList();
    }

    public FeedResultItem GetFeedItem(string id)
    {
        using var feedDbContext = _dbContextFactory.CreateDbContext();
        var news =
            feedDbContext.FeedAggregates
            .Where(r => r.Id == id)
            .FirstOrDefault();

        return _mapper.Map<FeedAggregate, FeedResultItem>(news);
    }

    public void RemoveFeedItem(string id)
    {
        using var feedDbContext = _dbContextFactory.CreateDbContext();
        var strategy = feedDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var feedDbContext = _dbContextFactory.CreateDbContext();
            using var tx = feedDbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

            var aggregates = feedDbContext.FeedAggregates.Where(r => r.Id == id);
            feedDbContext.FeedAggregates.RemoveRange(aggregates);

            var users = feedDbContext.FeedUsers.Where(r => r.FeedId == id);
            feedDbContext.FeedUsers.RemoveRange(users);

            feedDbContext.SaveChanges();

            tx.Commit();
        });
    }

    private Expression<Func<FeedAggregate, bool>> GetIdSearchExpression(string id, string module, bool withRelated)
    {
        Expression<Func<FeedAggregate, bool>> exp = null;

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(module))
        {
            return exp;
        }

        if (module == Constants.RoomsModule)
        {
            var roomId = $"{Constants.RoomItem}_{id}";
            var sharedRoomId = $"{Constants.SharedRoomItem}_{id}";

            exp = f => f.Id == roomId || f.Id.StartsWith(sharedRoomId);

            if (withRelated)
            {
                exp = f => f.Id == roomId || f.Id.StartsWith(sharedRoomId) || f.ContextId == roomId;
            }
        }

        if (module == Constants.FilesModule)
        {
            exp = f => f.Id.StartsWith($"{Constants.FileItem}_{id}") || f.Id.StartsWith($"{Constants.SharedFileItem}_{id}");
        }

        if (module == Constants.FoldersModule)
        {
            exp = f => f.Id == $"{Constants.FolderItem}_{id}" || f.Id.StartsWith($"{Constants.SharedFolderItem}_{id}");
        }

        return exp;
    }
}

public class FeedResultItem : IMapFrom<FeedAggregate>
{
    public string Json { get; set; }
    public string Module { get; set; }
    public Guid AuthorId { get; set; }
    public Guid ModifiedById { get; set; }
    public string GroupId { get; set; }
    public bool IsToday { get; set; }
    public bool IsYesterday { get; set; }
    public bool IsTomorrow { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public DateTime AggregatedDate { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<FeedAggregate, FeedResultItem>()
            .AfterMap<FeedMappingAction>();
    }
}