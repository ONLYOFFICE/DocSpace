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

    public async Task<DateTime> GetLastTimeAggregateAsync(string key)
    {
        await using var feedDbContext = _dbContextFactory.CreateDbContext();
        var value = await Queries.LastTimeAsync(feedDbContext, key);

        return value != default ? value.AddSeconds(1) : value;
    }

    public async Task SaveFeedsAsync(IEnumerable<FeedRow> feeds, string key, DateTime value, int portionSize)
    {
        var feedLast = new FeedLast
        {
            LastKey = key,
            LastDate = value
        };

        await using var feedDbContext = _dbContextFactory.CreateDbContext();
        await feedDbContext.AddOrUpdateAsync(q => q.FeedLast, feedLast);
        await feedDbContext.SaveChangesAsync();

        var aggregatedDate = DateTime.UtcNow;

        var feedsPortion = new List<FeedRow>();
        foreach (var feed in feeds)
        {
            feedsPortion.Add(feed);
            if (feedsPortion.Sum(f => f.Users.Count) <= portionSize)
            {
                continue;
            }

            await SaveFeedsPortionAsync(feedsPortion, aggregatedDate);
            feedsPortion.Clear();
        }

        if (feedsPortion.Count > 0)
        {
            await SaveFeedsPortionAsync(feedsPortion, aggregatedDate);
        }
    }

    private async Task SaveFeedsPortionAsync(IEnumerable<FeedRow> feeds, DateTime aggregatedDate)
    {
        await using var feedDbContext = _dbContextFactory.CreateDbContext();

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
                var fu = await Queries.FeedUsersAsync(feedDbContext, f.Id);
                if (fu != null)
                {
                    feedDbContext.FeedUsers.Remove(fu);
                }
            }

            await feedDbContext.AddOrUpdateAsync(q => q.FeedAggregates, feedAggregate);

            foreach (var u in f.Users)
            {
                var feedUser = new FeedUsers
                {
                    FeedId = f.Id,
                    UserId = u
                };

                await feedDbContext.AddOrUpdateAsync(q => q.FeedUsers, feedUser);
            }
        }

        await feedDbContext.SaveChangesAsync();
    }

    public async Task RemoveFeedAggregateAsync(DateTime fromTime)
    {
        await using var feedDbContext = _dbContextFactory.CreateDbContext();

        var aggregates = await Queries.FeedAggregatesByFromTimeAsync(feedDbContext, fromTime).ToListAsync();

        feedDbContext.FeedAggregates.RemoveRange(aggregates);

        await feedDbContext.SaveChangesAsync();
    }

    public async Task<List<FeedResultItem>> GetFeedsAsync(FeedApiFilter filter)
    {
        var filterOffset = filter.Offset;
        var filterLimit = filter.Max > 0 && filter.Max < 1000 ? filter.Max : 1000;

        var feeds = new Dictionary<string, List<FeedResultItem>>();

        var tryCount = 0;
        List<FeedResultItem> feedsIteration;
        do
        {
            feedsIteration = await GetFeedsInternalAsync(filter);
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

    private async Task<List<FeedResultItem>> GetFeedsInternalAsync(FeedApiFilter filter)
    {
        await using var feedDbContext = _dbContextFactory.CreateDbContext();
        var tenant = await _tenantManager.GetCurrentTenantAsync();
        var q = feedDbContext.FeedAggregates.AsNoTracking()
            .Where(r => r.TenantId == tenant.Id);

        var feeds = filter.History ? GetFeedsAsHistoryQuery(q, filter) : GetFeedsDefaultQuery(feedDbContext, q, filter);

        return _mapper.Map<IEnumerable<FeedAggregate>, List<FeedResultItem>>(feeds);
    }

    private static IQueryable<FeedAggregate> GetFeedsAsHistoryQuery(IQueryable<FeedAggregate> query, FeedApiFilter filter)
    {
        Expression<Func<FeedAggregate, bool>> exp = null;

        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(filter.Id);
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(filter.Module);

        switch (filter.Module)
        {
            case Constants.RoomsModule:
                {
                    var roomId = $"{Constants.RoomItem}_{filter.Id}";

                    exp = f => f.Id == roomId || (f.Id.StartsWith(Constants.SharedRoomItem) && f.ContextId == roomId);

                    if (filter.History)
                    {
                        exp = f => f.Id == roomId || f.ContextId == roomId;
                    }

                    break;
                }
            case Constants.FilesModule:
                exp = f => f.Id.StartsWith($"{Constants.FileItem}_{filter.Id}") || f.Id.StartsWith($"{Constants.SharedFileItem}_{filter.Id}");
                break;
            case Constants.FoldersModule:
                exp = f => f.Id == $"{Constants.FolderItem}_{filter.Id}" || f.Id.StartsWith($"{Constants.SharedFolderItem}_{filter.Id}");
                break;
        }

        if (exp == null)
        {
            throw new InvalidOperationException();
        }

        return query.Where(exp);
    }

    private IQueryable<FeedAggregate> GetFeedsDefaultQuery(FeedDbContext feedDbContext, IQueryable<FeedAggregate> query, FeedApiFilter filter)
    {
        var q1 = query.Join(feedDbContext.FeedUsers, a => a.Id, b => b.FeedId, (aggregates, users) => new { aggregates, users })
            .OrderByDescending(r => r.aggregates.ModifiedDate)
            .Skip(filter.Offset)
            .Take(filter.Max);

        q1 = q1.Where(r => r.aggregates.ModifiedBy != _authContext.CurrentAccount.ID).
            Where(r => r.users.UserId == _authContext.CurrentAccount.ID);

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

        return q1.Select(r => r.aggregates).Distinct();
    }

    public async Task<int> GetNewFeedsCountAsync(DateTime lastReadedTime)
    {
        await using var feedDbContext = _dbContextFactory.CreateDbContext();
        var tenant = await _tenantManager.GetCurrentTenantAsync();

        return await Queries.CountFeedAggregatesAsync(feedDbContext, tenant.Id, _authContext.CurrentAccount.ID, lastReadedTime);
    }

    public async Task<IEnumerable<int>> GetTenantsAsync(TimeInterval interval)
    {
        await using var feedDbContext = _dbContextFactory.CreateDbContext();
        return await Queries.KeysAsync(feedDbContext, interval.From, interval.To).ToListAsync();
    }

    public async Task<FeedResultItem> GetFeedItemAsync(string id)
    {
        await using var feedDbContext = _dbContextFactory.CreateDbContext();
        var news = await Queries.FeedAggregateAsync(feedDbContext, id);

        return _mapper.Map<FeedAggregate, FeedResultItem>(news);
    }

    public async Task RemoveFeedItemAsync(string id)
    {
        await using var feedDbContext = _dbContextFactory.CreateDbContext();
        var aggregates = await Queries.FeedAggregatesAsync(feedDbContext, id).ToListAsync();

        feedDbContext.FeedAggregates.RemoveRange(aggregates);
        await feedDbContext.SaveChangesAsync();
    }
}

public class FeedResultItem : IMapFrom<FeedAggregate>
{
    public string Json { get; set; }
    public string Module { get; set; }
    public Guid AuthorId { get; set; }
    public Guid ModifiedBy { get; set; }
    public object TargetId { get; set; }
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

static file class Queries
{
    public static readonly Func<FeedDbContext, string, Task<DateTime>> LastTimeAsync =
        EF.CompileAsyncQuery(
            (FeedDbContext ctx, string key) =>
                ctx.FeedLast
                    .Where(r => r.LastKey == key)
                    .Select(r => r.LastDate)
                    .FirstOrDefault());

    public static readonly Func<FeedDbContext, string, Task<FeedUsers>> FeedUsersAsync =
        EF.CompileAsyncQuery(
            (FeedDbContext ctx, string id) =>
                ctx.FeedUsers
                    .FirstOrDefault(r => r.FeedId == id));

    public static readonly Func<FeedDbContext, DateTime, IAsyncEnumerable<FeedAggregate>>
        FeedAggregatesByFromTimeAsync = EF.CompileAsyncQuery(
            (FeedDbContext ctx, DateTime fromTime) =>
                ctx.FeedAggregates
                    .Where(r => r.AggregateDate <= fromTime));

    public static readonly Func<FeedDbContext, DateTime, IAsyncEnumerable<FeedUsers>> FeedsUsersByFromTimeAsync =
        EF.CompileAsyncQuery(
            (FeedDbContext ctx, DateTime fromTime) =>
                ctx.FeedUsers
                    .Where(r => ctx.FeedAggregates.Where(f => f.AggregateDate <= fromTime).Any(a => a.Id == r.FeedId)));

    public static readonly Func<FeedDbContext, int, Guid, DateTime, Task<int>> CountFeedAggregatesAsync =
        EF.CompileAsyncQuery(
            (FeedDbContext ctx, int tenantId, Guid id, DateTime lastReadedTime) =>
                ctx.FeedAggregates
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => r.ModifiedBy != id)
                    .Join(ctx.FeedUsers, r => r.Id, u => u.FeedId, (agg, user) => new { agg, user })
                    .Where(r => r.user.UserId == id)
                    .Where(r => lastReadedTime.Year <= 1 || r.agg.AggregateDate >= lastReadedTime)
                    .Take(1001)
                    .Select(r => r.agg.Id)
                    .Count());

    public static readonly Func<FeedDbContext, DateTime, DateTime, IAsyncEnumerable<int>> KeysAsync =
        EF.CompileAsyncQuery(
            (FeedDbContext ctx, DateTime from, DateTime to) =>
                ctx.FeedAggregates
                    .Where(r => r.AggregateDate >= from && r.AggregateDate <= to)
                    .GroupBy(r => r.TenantId)
                    .Select(r => r.Key));

    public static readonly Func<FeedDbContext, string, Task<FeedAggregate>> FeedAggregateAsync =
        EF.CompileAsyncQuery(
            (FeedDbContext ctx, string id) =>
                ctx.FeedAggregates
                    .FirstOrDefault(r => r.Id == id));

    public static readonly Func<FeedDbContext, string, IAsyncEnumerable<FeedAggregate>> FeedAggregatesAsync =
        EF.CompileAsyncQuery(
            (FeedDbContext ctx, string id) =>
                ctx.FeedAggregates
                    .Where(r => r.Id == id));

    public static readonly Func<FeedDbContext, string, IAsyncEnumerable<FeedUsers>> FeedsUsersAsync =
        EF.CompileAsyncQuery(
            (FeedDbContext ctx, string id) =>
                ctx.FeedUsers
                    .Where(r => r.FeedId == id));
}