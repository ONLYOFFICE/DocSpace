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
    private MessagesContext AuditTrailContext => _lazyAuditTrailContext.Value;

    private readonly Lazy<MessagesContext> _lazyAuditTrailContext;
    private readonly IMapper _mapper;

    public AuditEventsRepository(
        DbContextManager<MessagesContext> dbContextManager,
        IMapper mapper)
    {
        _lazyAuditTrailContext = new Lazy<MessagesContext>(() => dbContextManager.Value);
        _mapper = mapper;
    }

    public IEnumerable<AuditEventDto> GetLast(int tenant, int chunk)
    {
        return Get(tenant, null, null, chunk);
    }

    public IEnumerable<AuditEventDto> Get(int tenant, DateTime from, DateTime to)
    {
        return Get(tenant, from, to, null);
    }

    public int GetCount(int tenant, DateTime? from = null, DateTime? to = null)
    {
        IQueryable<AuditEvent> query = AuditTrailContext.AuditEvents
            .Where(a => a.TenantId == tenant)
            .OrderByDescending(a => a.Date);

        if (from.HasValue && to.HasValue)
        {
            query = query.Where(a => a.Date >= from & a.Date <= to);
        }

        return query.Count();
    }

    private IEnumerable<AuditEventDto> Get(int tenant, DateTime? fromDate, DateTime? to, int? limit)
    {
        var query =
           from q in AuditTrailContext.AuditEvents
           from p in AuditTrailContext.Users.Where(p => q.UserId == p.Id).DefaultIfEmpty()
           where q.TenantId == tenant
           orderby q.Date descending
           select new AuditEventQuery
           {
               Event = q,
               FirstName = p.FirstName,
               LastName = p.LastName,
               UserName = p.UserName
           };

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
}

public static class AuditEventsRepositoryExtensions
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<EventTypeConverter>();
    }
}