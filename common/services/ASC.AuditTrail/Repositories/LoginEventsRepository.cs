namespace ASC.AuditTrail.Repositories;

[Scope]
public class LoginEventsRepository
{
    private MessagesContext MessagesContext => _lazyMessagesContext.Value;

    private readonly Lazy<MessagesContext> _lazyMessagesContext;
    private readonly IMapper _mapper;

    public LoginEventsRepository(
        DbContextManager<MessagesContext> dbMessagesContext,
        IMapper mapper)
    {
        _lazyMessagesContext = new Lazy<MessagesContext>(() => dbMessagesContext.Value);
        _mapper = mapper;
    }

    public IEnumerable<LoginEventDto> GetLast(int tenant, int chunk)
    {
        var query =
            (from b in MessagesContext.LoginEvents
             from p in MessagesContext.Users.Where(p => b.UserId == p.Id).DefaultIfEmpty()
             where b.TenantId == tenant
             orderby b.Date descending
             select new LoginEventQuery
             {
                 Event = b,
                 UserName = p.UserName,
                 FirstName = p.FirstName,
                 LastName = p.LastName
             })
             .Take(chunk);

        return _mapper.Map<List<LoginEventQuery>, IEnumerable<LoginEventDto>>(query.ToList());
    }

    public IEnumerable<LoginEventDto> Get(int tenant, DateTime fromDate, DateTime to)
    {
        var query =
            from q in MessagesContext.LoginEvents
            from p in MessagesContext.Users.Where(p => q.UserId == p.Id).DefaultIfEmpty()
            where q.TenantId == tenant
            where q.Date >= fromDate
            where q.Date <= to
            orderby q.Date descending
            select new LoginEventQuery
            {
                Event = q,
                UserName = p.UserName,
                FirstName = p.FirstName,
                LastName = p.LastName
            };

        return _mapper.Map<List<LoginEventQuery>, IEnumerable<LoginEventDto>>(query.ToList());
    }

    public int GetCount(int tenant, DateTime? from = null, DateTime? to = null)
    {
        var query = MessagesContext.LoginEvents
            .Where(l => l.TenantId == tenant);

        if (from.HasValue && to.HasValue)
        {
            query = query.Where(l => l.Date >= from & l.Date <= to);
        }

        return query.Count();
    }
}