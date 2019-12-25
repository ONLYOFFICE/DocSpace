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


using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.EF.Model;
using ASC.Core.Tenants;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;

namespace ASC.Feed.Data
{
    public class FeedAggregateDataProvider
    {
        public AuthContext AuthContext { get; }
        public TenantManager TenantManager { get; }
        public TenantUtil TenantUtil { get; }
        public FeedDbContext FeedDbContext { get; }

        public FeedAggregateDataProvider(DbContextManager<FeedDbContext> dbContextManager)
        {
            FeedDbContext = dbContextManager.Get(Constants.FeedDbId);
        }

        public FeedAggregateDataProvider(AuthContext authContext, TenantManager tenantManager, TenantUtil tenantUtil)
        {
            AuthContext = authContext;
            TenantManager = tenantManager;
            TenantUtil = tenantUtil;
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
                if (feedsPortion.Sum(f => f.Users.Count) <= feedsPortionSize) continue;

                SaveFeedsPortion(feedsPortion, aggregatedDate);
                feedsPortion.Clear();
            }
            if (feedsPortion.Any())
            {
                SaveFeedsPortion(feedsPortion, aggregatedDate);
            }
        }

        private void SaveFeedsPortion(IEnumerable<FeedRow> feeds, DateTime aggregatedDate)
        {
            using var tx = FeedDbContext.Database.BeginTransaction();

            foreach (var f in feeds)
            {
                if (0 >= f.Users.Count) continue;

                var feedAggregate = new FeedAggregate
                {
                    Id = f.Id,
                    Tenant = f.Tenant,
                    Product = f.ProductId,
                    Module = f.ModuleId,
                    Author = f.AuthorId,
                    ModifiedBy = f.ModifiedById,
                    GroupId = f.GroupId,
                    CreatedDate = f.CreatedDate,
                    ModifiedDate = f.ModifiedDate,
                    Json = f.Json,
                    Keywords = f.Keywords,
                    AggregateDate = aggregatedDate
                };

                if (f.ClearRightsBeforeInsert)
                {
                    var fu = FeedDbContext.FeedUsers.Where(r => r.FeedId == f.Id).FirstOrDefault();
                    FeedDbContext.FeedUsers.Remove(fu);
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
        }

        public void RemoveFeedAggregate(DateTime fromTime)
        {
            using var tx = FeedDbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

            var aggregates = FeedDbContext.FeedAggregates.Where(r => r.AggregateDate <= fromTime);
            FeedDbContext.FeedAggregates.RemoveRange(aggregates);

            var users = FeedDbContext.FeedUsers.Where(r => FeedDbContext.FeedAggregates.Where(r => r.AggregateDate <= fromTime).Any(a => a.Id == r.FeedId));
            FeedDbContext.FeedUsers.RemoveRange(users);

            tx.Commit();
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
                    if (feeds.ContainsKey(feed.GroupId))
                    {
                        feeds[feed.GroupId].Add(feed);
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
                .Where(r => r.Tenant == TenantManager.GetCurrentTenant().TenantId)
                .Where(r => r.ModifiedBy != AuthContext.CurrentAccount.ID)
                .Join(FeedDbContext.FeedUsers, a => a.Id, b => b.FeedId, (aggregates, users) => new { aggregates, users })
                .Where(r => r.users.UserId == AuthContext.CurrentAccount.ID)
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

            var news = q.Select(r => r.aggregates).ToList();

            return news.Select(r => new FeedResultItem(
                r.Json,
                r.Module,
                r.Author,
                r.ModifiedBy,
                r.GroupId,
                TenantUtil.DateTimeFromUtc(r.CreatedDate),
                TenantUtil.DateTimeFromUtc(r.ModifiedDate),
                TenantUtil.DateTimeFromUtc(r.AggregateDate),
                TenantUtil))
                .ToList();
        }

        public int GetNewFeedsCount(DateTime lastReadedTime, AuthContext authContext, TenantManager tenantManager)
        {
            var count = FeedDbContext.FeedAggregates
                .Where(r => r.Tenant == tenantManager.GetCurrentTenant().TenantId)
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

        public FeedResultItem GetFeedItem(string id, TenantUtil tenantUtil)
        {
            var news =
                FeedDbContext.FeedAggregates
                .Where(r => r.Id == id)
                .FirstOrDefault();

            return new FeedResultItem(news.Json, news.Module, news.Author, news.ModifiedBy, news.GroupId, news.CreatedDate, news.ModifiedDate, news.AggregateDate, tenantUtil);
        }

        public void RemoveFeedItem(string id)
        {
            using var tx = FeedDbContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted);

            var aggregates = FeedDbContext.FeedAggregates.Where(r => r.Id == id);
            FeedDbContext.FeedAggregates.RemoveRange(aggregates);

            var users = FeedDbContext.FeedUsers.Where(r => r.FeedId == id);
            FeedDbContext.FeedUsers.RemoveRange(users);

            FeedDbContext.SaveChanges();

            tx.Commit();
        }
    }


    public class FeedResultItem
    {
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

            if (now.Year == createdDate.Year && now.Date == createdDate.Date)
            {
                IsToday = true;
            }
            else if (now.Year == createdDate.Year && now.Date == createdDate.Date.AddDays(1))
            {
                IsYesterday = true;
            }

            CreatedDate = createdDate;
            ModifiedDate = modifiedDate;
            AggregatedDate = aggregatedDate;
        }

        public string Json { get; private set; }

        public string Module { get; private set; }

        public Guid AuthorId { get; private set; }

        public Guid ModifiedById { get; private set; }

        public string GroupId { get; private set; }

        public bool IsToday { get; private set; }

        public bool IsYesterday { get; private set; }

        public DateTime CreatedDate { get; private set; }

        public DateTime ModifiedDate { get; private set; }

        public DateTime AggregatedDate { get; private set; }

        public FeedMin ToFeedMin(UserManager userManager)
        {
            var feedMin = JsonConvert.DeserializeObject<FeedMin>(Json);
            feedMin.Author = new FeedMinUser { UserInfo = userManager.GetUsers(feedMin.AuthorId) };
            feedMin.CreatedDate = CreatedDate;

            if (feedMin.Comments == null) return feedMin;

            foreach (var comment in feedMin.Comments)
            {
                comment.Author = new FeedMinUser { UserInfo = userManager.GetUsers(comment.AuthorId) };
            }
            return feedMin;
        }
    }

    public static class FeedAggregateDataProviderExtension
    {
        public static IServiceCollection AddFeedAggregateDataProvider(this IServiceCollection services)
        {
            return services
                .AddAuthContextService()
                .AddTenantManagerService()
                .AddTenantUtilService()
                .AddFeedDbService();
        }
    }
}