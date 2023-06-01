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

using AutoMapper;

namespace ASC.Webhooks.Core;

[Scope]
public class DbWorker
{
    public static readonly IReadOnlyList<string> MethodList = new List<string>
    {
        "POST",
        "PUT",
        "DELETE"
    };

    private readonly IDbContextFactory<WebhooksDbContext> _dbContextFactory;
    private readonly TenantManager _tenantManager;
    private readonly AuthContext _authContext;
    private readonly IMapper _mapper;

    private int Tenant
    {
        get
        {
            return _tenantManager.GetCurrentTenant().Id;
        }
    }

    public DbWorker(
        IDbContextFactory<WebhooksDbContext> dbContextFactory,
        TenantManager tenantManager,
        AuthContext authContext,
        IMapper mapper)
    {
        _dbContextFactory = dbContextFactory;
        _tenantManager = tenantManager;
        _authContext = authContext;
        _mapper = mapper;
    }

    public async Task<WebhooksConfig> AddWebhookConfig(string uri, string secretKey)
    {
        await using var webhooksDbContext = _dbContextFactory.CreateDbContext();

        var objForCreate = await Queries.WebhooksConfigByUriAsync(webhooksDbContext, Tenant, uri);

        if (objForCreate != null)
        {
            return objForCreate;
        }

        var toAdd = new WebhooksConfig
        {
            TenantId = Tenant,
            Uri = uri,
            SecretKey = secretKey
        };

        toAdd = await webhooksDbContext.AddOrUpdateAsync(r => r.WebhooksConfigs, toAdd);
        await webhooksDbContext.SaveChangesAsync();

        return toAdd;
    }

    public IAsyncEnumerable<WebhooksConfig> GetTenantWebhooks()
    {
        using var webhooksDbContext = _dbContextFactory.CreateDbContext();

        return Queries.WebhooksConfigsAsync(webhooksDbContext, Tenant);
    }

    public async Task<WebhooksConfig> UpdateWebhookConfig(int id, string uri, string key, bool? enabled)
    {
        await using var webhooksDbContext = _dbContextFactory.CreateDbContext();

        var updateObj = await Queries.WebhooksConfigAsync(webhooksDbContext, Tenant, id);

        if (updateObj != null)
        {
            if (!string.IsNullOrEmpty(uri))
            {
                updateObj.Uri = uri;
            }

            if (!string.IsNullOrEmpty(key))
            {
                updateObj.SecretKey = key;
            }

            if (enabled.HasValue)
            {
                updateObj.Enabled = enabled.Value;
            }

            webhooksDbContext.WebhooksConfigs.Update(updateObj);
            await webhooksDbContext.SaveChangesAsync();
        }

        return updateObj;
    }

    public async Task<WebhooksConfig> RemoveWebhookConfigAsync(int id)
    {
        await using var webhooksDbContext = _dbContextFactory.CreateDbContext();

        var removeObj = await Queries.WebhooksConfigAsync(webhooksDbContext, Tenant, id);

        if (removeObj != null)
        {
            webhooksDbContext.WebhooksConfigs.Remove(removeObj);
            await webhooksDbContext.SaveChangesAsync();
        }

        return removeObj;
    }

    public IAsyncEnumerable<WebhooksLog> ReadJournal(int startIndex, int limit, DateTime? delivery, string hookUri, int hookId)
    {
        using var webhooksDbContext = _dbContextFactory.CreateDbContext();

        var q = webhooksDbContext.WebhooksLogs
            .AsNoTracking()
            .Where(r => r.TenantId == Tenant);

        if (delivery.HasValue)
        {
            var date = delivery.Value;
            q = q.Where(r => r.Delivery == date);
        }

        if (!string.IsNullOrEmpty(hookUri))
        {
            q = q.Where(r => r.Config.Uri == hookUri);
        }

        if (hookId != 0)
        {
            q = q.Where(r => r.WebhookId == hookId);
        }

        if (startIndex != 0)
        {
            q = q.Skip(startIndex);
        }

        if (limit != 0)
        {
            q = q.Take(limit);
        }

        return q.OrderByDescending(t => t.Id).AsAsyncEnumerable();
    }

    public async Task<WebhooksLog> ReadJournal(int id)
    {
        await using var webhooksDbContext = _dbContextFactory.CreateDbContext();

        return await Queries.WebhooksLogAsync(webhooksDbContext, id);
    }

    public async Task<WebhooksLog> WriteToJournal(WebhooksLog webhook)
    {
        webhook.TenantId = await _tenantManager.GetCurrentTenantIdAsync();
        webhook.Uid = _authContext.CurrentAccount.ID;

        await using var webhooksDbContext = _dbContextFactory.CreateDbContext();

        var entity = await webhooksDbContext.WebhooksLogs.AddAsync(webhook);
        await webhooksDbContext.SaveChangesAsync();

        return entity.Entity;
    }

    public async Task<WebhooksLog> UpdateWebhookJournal(int id, int status, DateTime delivery, string requestHeaders, string responsePayload, string responseHeaders)
    {
        await using var webhooksDbContext = _dbContextFactory.CreateDbContext();

        var webhook = await Queries.WebhooksLogAsync(webhooksDbContext, id);
        webhook.Status = status;
        webhook.RequestHeaders = requestHeaders;
        webhook.ResponsePayload = responsePayload;
        webhook.ResponseHeaders = responseHeaders;
        webhook.Delivery = delivery;

        webhooksDbContext.WebhooksLogs.Update(webhook);
        await webhooksDbContext.SaveChangesAsync();

        return webhook;
    }

    public async Task Register(List<Webhook> webhooks)
    {
        await using var webhooksDbContext = _dbContextFactory.CreateDbContext();

        var dbWebhooks = Queries.DbWebhooksAsync(webhooksDbContext);

        foreach (var webhook in webhooks)
        {
            if (!await dbWebhooks.AnyAsync(r => r.Route == webhook.Route && r.Method == webhook.Method))
            {
                try
                {
                    await webhooksDbContext.Webhooks.AddAsync(_mapper.Map<DbWebhook>(webhook));
                    await webhooksDbContext.SaveChangesAsync();
                }
                catch (Exception)
                {

                }
            }
        }
    }

    public async Task<List<Webhook>> GetWebhooksAsync()
    {
        await using var webhooksDbContext = _dbContextFactory.CreateDbContext();
        var webHooks = await Queries.DbWebhooksAsync(webhooksDbContext).ToListAsync();
        return _mapper.Map<List<DbWebhook>, List<Webhook>>(webHooks);
    }

    public async Task<Webhook> GetWebhookAsync(int id)
    {
        await using var webhooksDbContext = _dbContextFactory.CreateDbContext();
        var webHook = await Queries.DbWebhookAsync(webhooksDbContext, id);
        return _mapper.Map<DbWebhook, Webhook>(webHook);
    }

    public async Task<Webhook> GetWebhookAsync(string method, string routePattern)
    {
        await using var webhooksDbContext = _dbContextFactory.CreateDbContext();

        var webHook = await Queries.DbWebhookByMethodAsync(webhooksDbContext, method, routePattern);

        return _mapper.Map<DbWebhook, Webhook>(webHook);
    }
}

static file class Queries
{
    public static readonly Func<WebhooksDbContext, int, string, Task<WebhooksConfig>> WebhooksConfigByUriAsync =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (WebhooksDbContext ctx, int tenantId, string uri) =>
                ctx.WebhooksConfigs
                    .FirstOrDefault(r => r.TenantId == tenantId && r.Uri == uri));

    public static readonly Func<WebhooksDbContext, int, IAsyncEnumerable<WebhooksConfig>> WebhooksConfigsAsync =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (WebhooksDbContext ctx, int tenantId) =>
                ctx.WebhooksConfigs
                    .AsNoTracking()
                    .Where(it => it.TenantId == tenantId));

    public static readonly Func<WebhooksDbContext, int, int, Task<WebhooksConfig>> WebhooksConfigAsync =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (WebhooksDbContext ctx, int tenantId, int id) =>
                ctx.WebhooksConfigs
                    .FirstOrDefault(it => it.TenantId == tenantId && it.Id == id));

    public static readonly Func<WebhooksDbContext, int, Task<WebhooksLog>> WebhooksLogAsync =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (WebhooksDbContext ctx, int id) =>
                ctx.WebhooksLogs
                    .AsNoTracking()
                    .FirstOrDefault(it => it.Id == id));

    public static readonly Func<WebhooksDbContext, IAsyncEnumerable<DbWebhook>> DbWebhooksAsync =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (WebhooksDbContext ctx) =>
                ctx.Webhooks.AsNoTracking());

    public static readonly Func<WebhooksDbContext, int, Task<DbWebhook>> DbWebhookAsync =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (WebhooksDbContext ctx, int id) =>
                ctx.Webhooks
                    .AsNoTracking()
                    .FirstOrDefault(r => r.Id == id));

    public static readonly Func<WebhooksDbContext, string, string, Task<DbWebhook>> DbWebhookByMethodAsync =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (WebhooksDbContext ctx, string method, string routePattern) =>
                ctx.Webhooks
                    .AsNoTracking()
                    .FirstOrDefault(r => r.Method == method && r.Route == routePattern));
}