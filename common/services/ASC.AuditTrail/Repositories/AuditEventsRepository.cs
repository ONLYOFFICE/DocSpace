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

namespace ASC.AuditTrail.Repositories;

[Scope(Additional = typeof(AuditEventsRepositoryExtensions))]
public class AuditEventsRepository
{
    private readonly AuditActionMapper _auditActionMapper;
    private readonly TenantManager _tenantManager;
    private readonly IDbContextFactory<MessagesContext> _dbContextFactory;
    private readonly IMapper _mapper;
    private readonly GeolocationHelper _geolocationHelper;

    public AuditEventsRepository(
        AuditActionMapper auditActionMapper,
        TenantManager tenantManager,
        IDbContextFactory<MessagesContext> dbContextFactory,
        IMapper mapper,
        GeolocationHelper geolocationHelper)
    {
        _auditActionMapper = auditActionMapper;
        _tenantManager = tenantManager;
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
        _geolocationHelper = geolocationHelper;
    }

    public async Task<IEnumerable<AuditEvent>> GetByFilterAsync(
        Guid? userId = null,
        ProductType? productType = null,
        ModuleType? moduleType = null,
        ActionType? actionType = null,
        MessageAction? action = null,
        EntryType? entry = null,
        string target = null,
        DateTime? from = null,
        DateTime? to = null,
        int startIndex = 0,
        int limit = 0,
        Guid? withoutUserId = null)
    {
        return await GetByFilterWithActionsAsync(
            userId,
            productType,
            moduleType,
            actionType,
            new List<MessageAction?> { action },
            entry,
            target,
            from,
            to,
            startIndex,
            limit,
            withoutUserId);
    }

    public async Task<IEnumerable<AuditEvent>> GetByFilterWithActionsAsync(
        Guid? userId = null,
        ProductType? productType = null,
        ModuleType? moduleType = null,
        ActionType? actionType = null,
        List<MessageAction?> actions = null,
        EntryType? entry = null,
        string target = null,
        DateTime? from = null,
        DateTime? to = null,
        int startIndex = 0,
        int limit = 0,
        Guid? withoutUserId = null)
    {
        var tenant = await _tenantManager.GetCurrentTenantIdAsync();
        await using var auditTrailContext = _dbContextFactory.CreateDbContext();

        var query =
           from q in auditTrailContext.AuditEvents
           from p in auditTrailContext.Users.Where(p => q.UserId == p.Id).DefaultIfEmpty()
           where q.TenantId == tenant
           orderby q.Date descending
           select new AuditEventQuery
           {
               Event = q,
               FirstName = p.FirstName,
               LastName = p.LastName,
               UserName = p.UserName
           };

        if (userId.HasValue && userId.Value != Guid.Empty)
        {
            query = query.Where(r => r.Event.UserId == userId.Value);
        }
        else if (withoutUserId.HasValue && withoutUserId.Value != Guid.Empty)
        {
            query = query.Where(r => r.Event.UserId != withoutUserId.Value);
        }

        var isNeedFindEntry = entry.HasValue && entry.Value != EntryType.None && target != null;


        if (actions != null && actions.Any() && actions[0] != null && actions[0] != MessageAction.None)
        {
            query = query.Where(r => actions.Contains(r.Event.Action != null ? (MessageAction)r.Event.Action : MessageAction.None));
        }
        else
        {
            IEnumerable<KeyValuePair<MessageAction, MessageMaps>> actionsList = new List<KeyValuePair<MessageAction, MessageMaps>>();

            var isFindActionType = actionType.HasValue && actionType.Value != ActionType.None;

            if (productType.HasValue && productType.Value != ProductType.None)
            {
                var productMapper = _auditActionMapper.Mappers.FirstOrDefault(m => m.Product == productType.Value);

                if (productMapper != null)
                {
                    if (moduleType.HasValue && moduleType.Value != ModuleType.None)
                    {
                        var moduleMapper = productMapper.Mappers.FirstOrDefault(m => m.Module == moduleType.Value);
                        if (moduleMapper != null)
                        {
                            actionsList = moduleMapper.Actions;
                        }
                    }
                    else
                    {
                        actionsList = productMapper.Mappers.SelectMany(r => r.Actions);
                    }
                }
            }
            else
            {
                actionsList = _auditActionMapper.Mappers
                        .SelectMany(r => r.Mappers)
                        .SelectMany(r => r.Actions);
            }

            if (isFindActionType || isNeedFindEntry)
            {
                actionsList = actionsList
                        .Where(a => (!isFindActionType || a.Value.ActionType == actionType.Value) && (!isNeedFindEntry || (entry.Value == a.Value.EntryType1) || entry.Value == a.Value.EntryType2))
                        .ToList();
            }

            if (isNeedFindEntry)
            {
                FindByEntry(query, entry.Value, target, actionsList);
            }
            else
            {
                var keys = actionsList.Select(x => (int)x.Key).ToList();
                query = query.Where(r => keys.Contains(r.Event.Action ?? 0));
            }
        }

        var hasFromFilter = (from.HasValue && from.Value != DateTime.MinValue);
        var hasToFilter = (to.HasValue && to.Value != DateTime.MinValue);

        if (hasFromFilter || hasToFilter)
        {
            if (hasFromFilter)
            {
                if (hasToFilter)
                {
                    query = query.Where(q => q.Event.Date >= from.Value & q.Event.Date <= to.Value);
                }
                else
                {
                    query = query.Where(q => q.Event.Date >= from.Value);
                }
            }
            else if (hasToFilter)
            {
                query = query.Where(q => q.Event.Date <= to.Value);
            }
        }

        if (startIndex > 0)
        {
            query = query.Skip(startIndex);
        }
        if (limit > 0)
        {
            query = query.Take(limit);
        }
        var events = _mapper.Map<List<AuditEventQuery>, IEnumerable<AuditEvent>>(await query.ToListAsync());

        foreach(var e in events)
        {
            await _geolocationHelper.AddGeolocationAsync(e);
        }
        return events;
    }

    private static void FindByEntry(IQueryable<AuditEventQuery> q, EntryType entry, string target, IEnumerable<KeyValuePair<MessageAction, MessageMaps>> actions)
    {
        Expression<Func<AuditEventQuery, bool>> a = r => false;

        foreach (var action in actions)
        {
            if (action.Value.EntryType1 == entry || action.Value.EntryType2 == entry)
            {
                a = a.Or(r => r.Event.Action == (int)action.Key && r.Event.Target.Split(',', StringSplitOptions.TrimEntries).Contains(target));
            }
        }

        q = q.Where(a);
    }

    public async Task<IEnumerable<int>> GetTenantsAsync(DateTime? from = null, DateTime? to = null)
    {
        await using var feedDbContext = _dbContextFactory.CreateDbContext();

        return await Queries.TenantsAsync(feedDbContext, from, to).ToListAsync();
    }
}

public static class AuditEventsRepositoryExtensions
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<EventTypeConverter>();
    }
}

static file class Queries
{
    public static readonly Func<MessagesContext, DateTime?, DateTime?, IAsyncEnumerable<int>> TenantsAsync =
        EF.CompileAsyncQuery(
            (MessagesContext ctx, DateTime? from, DateTime? to) =>
                ctx.AuditEvents
                    .Where(r => r.Date >= from && r.Date <= to)
                    .Select(r => r.TenantId)
                    .Distinct());
}