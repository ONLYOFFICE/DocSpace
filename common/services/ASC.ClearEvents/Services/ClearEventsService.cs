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

namespace ASC.ClearEvents.Services;

[Scope]
public class ClearEventsService : IHostedService, IDisposable
{
    private readonly ILogger<ClearEventsService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private Timer _timer;

    public ClearEventsService(ILogger<ClearEventsService> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.InformationTimerRunnig();

        _timer = new Timer(async state => await DeleteOldEventsAsync(state), null, TimeSpan.Zero,
            TimeSpan.FromDays(1));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.InformationTimerStopping();

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (_timer == null)
        {
            return;
        }

        var handle = new AutoResetEvent(false);

        if (!_timer.Dispose(handle))
        {
            throw new Exception("Timer already disposed");
        }

        handle.WaitOne();
    }

    private async Task DeleteOldEventsAsync(object state)
    {
        try
        {
            await GetOldEventsAsync(r => r.LoginEvents, "LoginHistoryLifeTime");
            await GetOldEventsAsync(r => r.AuditEvents, "AuditTrailLifeTime");
        }
        catch (Exception ex)
        {
            _logger.ErrorWithException(ex);
        }
    }

    private async Task GetOldEventsAsync<T>(Expression<Func<MessagesContext, DbSet<T>>> func, string settings) where T : MessageEvent
    {
        List<T> ids;
        var compile = func.Compile();
        do
        {
            using var scope = _serviceScopeFactory.CreateScope();
            using var ef = scope.ServiceProvider.GetService<IDbContextFactory<MessagesContext>>().CreateDbContext();
            var table = compile.Invoke(ef);

            var ae = table
                .Join(ef.Tenants, r => r.TenantId, r => r.Id, (audit, tenant) => audit)
                .Select(r => new
                {
                    r.Id,
                    r.Date,
                    r.TenantId,
                    ef = r
                })
                .Where(r => r.Date < DateTime.UtcNow.AddDays(-Convert.ToDouble(
                    ef.WebstudioSettings
                    .Where(a => a.TenantId == r.TenantId && a.Id == TenantAuditSettings.Guid)
                    .Select(r => JsonExtensions.JsonValue(nameof(r.Data).ToLower(), settings))
                    .FirstOrDefault() ?? TenantAuditSettings.MaxLifeTime.ToString())))
                .Take(1000);

            ids = await ae.Select(r => r.ef).ToListAsync();

            if (!ids.Any())
            {
                return;
            }

            table.RemoveRange(ids);
            await ef.SaveChangesAsync();

        } while (ids.Any());
    }
}