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