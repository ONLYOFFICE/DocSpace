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

    public AuditEventsRepository(
        AuditActionMapper auditActionMapper,
        TenantManager tenantManager,
        IDbContextFactory<MessagesContext> dbContextFactory,
        IMapper mapper)
    {
        _auditActionMapper = auditActionMapper;
        _tenantManager = tenantManager;
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    private IEnumerable<AuditEventDto> Get(int tenant, DateTime? fromDate, DateTime? to, int? limit)
    {
        using var auditTrailContext = _dbContextFactory.CreateDbContext();
        var query =
           (from q in auditTrailContext.AuditEvents
            from p in auditTrailContext.Users.Where(p => q.UserId == p.Id).DefaultIfEmpty()
            where q.TenantId == tenant
            orderby q.Date descending
            select new AuditEventQuery
            {
                Event = q,
                FirstName = p.FirstName,
                LastName = p.LastName,
                UserName = p.UserName
            });

        if (fromDate.HasValue && to.HasValue)
        {
            query = query.Where(q => q.Event.Date >= fromDate & q.Event.Date <= to);
        }

        if (limit.HasValue)
        {
            query = query.Take((int)limit);
        }

        return _mapper.Map<List<AuditEventQuery>, IEnumerable<AuditEventDto>>(query.ToList());
    }

    public IEnumerable<AuditEventDto> GetByFilter(
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
        int limit = 0)
    {
        var tenant = _tenantManager.GetCurrentTenant().Id;
        using var auditTrailContext = _dbContextFactory.CreateDbContext();
        var query =
          (from q in auditTrailContext.AuditEvents
           from p in auditTrailContext.Users.Where(p => q.UserId == p.Id).DefaultIfEmpty()
           where q.TenantId == tenant
           orderby q.Date descending
           select new AuditEventQuery
           {
               Event = q,
               FirstName = p.FirstName,
               LastName = p.LastName,
               UserName = p.UserName
           });

        if (startIndex > 0)
        {
            query = query.Skip(startIndex);
        }
        if (limit > 0)
        {
            query = query.Take(limit);
        }

        if (userId.HasValue && userId.Value != Guid.Empty)
        {
            query = query.Where(r => r.Event.UserId == userId.Value);
        }

        var isNeedFindEntry = entry.HasValue && entry.Value != EntryType.None && target != null;


        if (action.HasValue && action.Value != MessageAction.None)
        {
            query = query.Where(r => r.Event.Action == (int)action);
        }
        else
        {
            IEnumerable<KeyValuePair<MessageAction, MessageMaps>> actions = new List<KeyValuePair<MessageAction, MessageMaps>>();

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
                            actions = moduleMapper.Actions;
                        }
                    }
                    else
                    {
                        actions = productMapper.Mappers.SelectMany(r => r.Actions);
                    }
                }
            }
            else
            {
                actions = _auditActionMapper.Mappers
                        .SelectMany(r => r.Mappers)
                        .SelectMany(r => r.Actions);
            }

            if (isFindActionType || isNeedFindEntry)
            {
                actions = actions
                        .Where(a => (!isFindActionType || a.Value.ActionType == actionType.Value) && (!isNeedFindEntry || (entry.Value == a.Value.EntryType1) || entry.Value == a.Value.EntryType2))
                        .ToList();
            }

            if (isNeedFindEntry)
            {
                FindByEntry(query, entry.Value, target, actions);
            }
            else
            {
                var keys = actions.Select(x => (int)x.Key).ToList();
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

        return _mapper.Map<List<AuditEventQuery>, IEnumerable<AuditEventDto>>(query.ToList());
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

    public int GetCount(int tenant, DateTime? from = null, DateTime? to = null)
    {
        using var auditTrailContext = _dbContextFactory.CreateDbContext();

        var query = auditTrailContext.AuditEvents
            .Where(a => a.TenantId == tenant);

        if (from.HasValue && to.HasValue)
        {
            query = query.Where(a => a.Date >= from & a.Date <= to);
        }

        return query.Count();
    }
}

internal static class PredicateBuilder
{
    internal static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> a, Expression<Func<T, bool>> b)
    {
        var p = a.Parameters[0];

        var visitor = new SubstExpressionVisitor();
        visitor.Subst[b.Parameters[0]] = p;

        Expression body = Expression.OrElse(a.Body, visitor.Visit(b.Body));
        return Expression.Lambda<Func<T, bool>>(body, p);
    }
}

internal class SubstExpressionVisitor : System.Linq.Expressions.ExpressionVisitor
{
    internal Dictionary<Expression, Expression> Subst = new Dictionary<Expression, Expression>();

    protected override Expression VisitParameter(ParameterExpression node)
    {
        if (Subst.TryGetValue(node, out var newValue))
        {
            return newValue;
        }
        return node;
    }
}

public static class AuditEventsRepositoryExtensions
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<EventTypeConverter>();
    }
}