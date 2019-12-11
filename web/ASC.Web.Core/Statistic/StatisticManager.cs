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

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ASC.Web.Studio.Core.Statistic
{
    public class StatisticManager
    {
        private static DateTime lastSave = DateTime.UtcNow;
        private static readonly TimeSpan cacheTime = TimeSpan.FromMinutes(2);
        private static readonly IDictionary<string, UserVisit> cache = new Dictionary<string, UserVisit>();

        public WebstudioDbContext WebstudioDbContext { get; }

        public StatisticManager(DbContextManager<WebstudioDbContext> dbContextManager)
        {
            WebstudioDbContext = dbContextManager.Value;
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

    public static class StatisticManagerExtension
    {
        public static IServiceCollection AddStatisticManagerService(this IServiceCollection services)
        {
            services.TryAddScoped<StatisticManager>();

            return services.AddWebstudioDbContextService();
        }
    }
}