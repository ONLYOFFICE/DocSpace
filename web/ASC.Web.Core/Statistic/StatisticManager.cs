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

namespace ASC.Web.Studio.Core.Statistic;

[Scope]
public class StatisticManager
{
    private static DateTime _lastSave = DateTime.UtcNow;
    private static readonly TimeSpan _cacheTime = TimeSpan.FromMinutes(2);
    private static readonly IDictionary<string, UserVisit> _cache = new Dictionary<string, UserVisit>();
    private readonly IDbContextFactory<WebstudioDbContext> _dbContextFactory;

    public StatisticManager(IDbContextFactory<WebstudioDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async ValueTask SaveUserVisitAsync(int tenantId, Guid userId, Guid productId)
    {
        var now = DateTime.UtcNow;
        var key = string.Format("{0}|{1}|{2}|{3}", tenantId, userId, productId, now.Date);

        lock (_cache)
        {
            var visit = _cache.ContainsKey(key) ?
                            _cache[key] :
                            new UserVisit
                            {
                                TenantID = tenantId,
                                UserID = userId,
                                ProductID = productId,
                                VisitDate = now
                            };

            visit.VisitCount++;
            visit.LastVisitTime = now;
            _cache[key] = visit;
        }

        if (_cacheTime < DateTime.UtcNow - _lastSave)
        {
            await FlushCacheAsync();
        }
    }

    public async Task<List<UserVisit>> GetHitsByPeriodAsync(int tenantID, DateTime startPeriod, DateTime endPeriod)
    {
        await using var webstudioDbContext = _dbContextFactory.CreateDbContext();
        return await Queries.UserVisitsAsync(webstudioDbContext, tenantID, startPeriod, endPeriod).ToListAsync();
    }

    public async Task<List<UserVisit>> GetHostsByPeriodAsync(int tenantID, DateTime startPeriod, DateTime endPeriod)
    {
        await using var webstudioDbContext = _dbContextFactory.CreateDbContext();
        return await Queries.UserVisitsGroupByUserIdAsync(webstudioDbContext, tenantID, startPeriod, endPeriod).ToListAsync();
    }

    private async Task FlushCacheAsync()
    {
        if (_cache.Count == 0)
        {
            return;
        }

        List<UserVisit> visits;
        lock (_cache)
        {
            visits = new List<UserVisit>(_cache.Values);
            _cache.Clear();
            _lastSave = DateTime.UtcNow;
        }

        await using var webstudioDbContext = _dbContextFactory.CreateDbContext();

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

            await webstudioDbContext.WebstudioUserVisit.AddAsync(w);
        }
        await webstudioDbContext.SaveChangesAsync();
    }
}

static file class Queries
{
    public static readonly Func<WebstudioDbContext, int, DateTime, DateTime, IAsyncEnumerable<UserVisit>>
        UserVisitsAsync = EF.CompileAsyncQuery(
            (WebstudioDbContext ctx, int tenantId, DateTime startDate, DateTime endPeriod) =>
                ctx.WebstudioUserVisit
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => r.VisitDate >= startDate && r.VisitDate <= endPeriod)
                    .OrderBy(r => r.VisitDate)
                    .GroupBy(r => r.VisitDate)
                    .Select(r => new UserVisit { VisitDate = r.Key, VisitCount = r.Sum(a => a.VisitCount) }));

    public static readonly Func<WebstudioDbContext, int, DateTime, DateTime, IAsyncEnumerable<UserVisit>>
        UserVisitsGroupByUserIdAsync = EF.CompileAsyncQuery(
            (WebstudioDbContext ctx, int tenantId, DateTime startDate, DateTime endPeriod) =>
                ctx.WebstudioUserVisit
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => r.VisitDate >= startDate && r.VisitDate <= endPeriod)
                    .OrderBy(r => r.VisitDate)
                    .GroupBy(r => new { r.UserId, r.VisitDate })
                    .Select(r => new UserVisit { VisitDate = r.Key.VisitDate, UserID = r.Key.UserId }));
}
