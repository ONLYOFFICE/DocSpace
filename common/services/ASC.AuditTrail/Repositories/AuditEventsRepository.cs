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
 * Pursuant to Section 7 � 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 � 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

namespace ASC.AuditTrail.Repositories;

[Scope]
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