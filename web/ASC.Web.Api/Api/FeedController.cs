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

namespace ASC.Web.Api.Controllers;

[Scope]
[DefaultRoute]
[ApiController]
public class FeedController : ControllerBase
{
    private readonly FeedReadedDataProvider _feedReadedDataProvider;
    private readonly ApiContext _apiContext;
    private readonly ICache _newFeedsCountCache;
    private readonly FeedAggregateDataProvider _feedAggregateDataProvider;
    private readonly TenantUtil _tenantUtil;
    private readonly SecurityContext _securityContext;
    private readonly IMapper _mapper;

    public FeedController(
        FeedReadedDataProvider feedReadedDataProvider,
        ApiContext apiContext,
        ICache newFeedsCountCache,
        FeedAggregateDataProvider feedAggregateDataProvider,
        TenantUtil tenantUtil,
        SecurityContext securityContext,
        IMapper mapper)
    {
        _feedReadedDataProvider = feedReadedDataProvider;
        _apiContext = apiContext;
        _newFeedsCountCache = newFeedsCountCache;
        _feedAggregateDataProvider = feedAggregateDataProvider;
        _tenantUtil = tenantUtil;
        _securityContext = securityContext;
        _mapper = mapper;
    }

    private string Key => $"newfeedscount/{_securityContext.CurrentAccount.ID}";

    ///<summary>
    ///Opens feeds for reading.
    ///</summary>
    ///<short>
    ///Read feeds
    ///</short>
    [HttpPut("read")]
    public async Task Read()
    {
        await _feedReadedDataProvider.SetTimeReadedAsync();
    }

    ///<summary>
    ///Returns a list of feeds that are filtered by the parameters specified in the request.
    ///</summary>
    ///<short>
    ///Get feeds
    ///</short>
    /// <param name="id">Entity ID</param>
    /// <param name="product">Module that will be searched for by entity ID</param>
    /// <param name="module"></param>
    /// <param name="from">Time from which the feeds should be displayed</param>
    /// <param name="to">Time until which the feeds should be displayed</param>
    /// <param name="author">Author whose feeds you want to read</param>
    /// <param name="onlyNew">Displays only fresh feeds</param>
    /// <param name="withRelated">Include the associated feed related to the entity with the given id</param>
    /// <param name="timeReaded">Time when the feeds were read</param>
    ///<returns>List of filtered feeds</returns>
    [HttpGet("filter")]
    public async Task<object> GetFeedAsync(
        string id,
        string product,
        string module,
        ApiDateTime from,
        ApiDateTime to,
        Guid? author,
        bool? onlyNew,
        bool? withRelated,
        ApiDateTime timeReaded)
    {
        var filter = new FeedApiFilter
        {
            Id = id,
            Product = product,
            Module = module,
            Offset = Convert.ToInt32(_apiContext.StartIndex),
            Max = Convert.ToInt32(_apiContext.Count) - 1,
            Author = author ?? Guid.Empty,
            SearchKeys = _apiContext.FilterValues,
            OnlyNew = onlyNew.HasValue && onlyNew.Value,
            History = withRelated.HasValue && withRelated.Value,
        };

        if (from != null && to != null)
        {
            var f = _tenantUtil.DateTimeFromUtc(from.UtcTime);
            filter.From = new DateTime(f.Year, f.Month, f.Day, 0, 0, 0);

            var t = _tenantUtil.DateTimeFromUtc(to.UtcTime);
            filter.To = new DateTime(t.Year, t.Month, t.Day, 23, 59, 59);
        }
        else
        {
            filter.From = from != null ? from.UtcTime : DateTime.MinValue;
            filter.To = to != null ? to.UtcTime : DateTime.MaxValue;
        }

        var lastTimeReaded = DateTime.UtcNow;
        var readedDate = lastTimeReaded;

        if (string.IsNullOrEmpty(id))
        {
            lastTimeReaded = await _feedReadedDataProvider.GetTimeReadedAsync();
            readedDate = timeReaded != null ? timeReaded.UtcTime : lastTimeReaded;
        }

        if (filter.OnlyNew)
        {
            filter.From = lastTimeReaded;
            filter.Max = 100;
        }
        else if (timeReaded == null)
        {
            await _feedReadedDataProvider.SetTimeReadedAsync();
            _newFeedsCountCache.Remove(Key);
        }

        var feeds = (await _feedAggregateDataProvider
            .GetFeedsAsync(filter))
            .GroupBy(n => n.GroupId,
                     n => _mapper.Map<FeedResultItem, FeedDto>(n),
                     (n, group) =>
                     {
                         var firstFeed = group.First();
                         firstFeed.GroupedFeeds = group.Skip(1);
                         return firstFeed;
                     })
            .OrderByDescending(f => f.ModifiedDate)
            .ToList();

        return new { feeds, readedDate };
    }

    ///<summary>
    ///Returns a number of fresh feeds.
    ///</summary>
    ///<short>
    ///Count fresh feeds
    ///</short>
    ///<returns>Number of fresh feeds</returns>
    [HttpGet("newfeedscount")]
    public async Task<object> GetFreshNewsCountAsync()
    {
        var cacheKey = Key;
        var resultfromCache = _newFeedsCountCache.Get<string>(cacheKey);

        if (!int.TryParse(resultfromCache, out var result))
        {
            var lastTimeReaded = await _feedReadedDataProvider.GetTimeReadedAsync();
            result = await _feedAggregateDataProvider.GetNewFeedsCountAsync(lastTimeReaded);
            _newFeedsCountCache.Insert(cacheKey, result.ToString(), DateTime.UtcNow.AddMinutes(3));
        }

        return result;
    }
}