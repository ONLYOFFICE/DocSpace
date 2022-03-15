namespace ASC.Core.Common.Notify.Telegram;

public class ConfigureTelegramDaoService : IConfigureNamedOptions<TelegramDao>
{
    private readonly DbContextManager<TelegramDbContext> _dbContextManager;

    public ConfigureTelegramDaoService(DbContextManager<TelegramDbContext> dbContextManager)
    {
        _dbContextManager = dbContextManager;
    }

    public void Configure(string name, TelegramDao options)
    {
        Configure(options);
        options.TelegramDbContext = _dbContextManager.Get(name);
    }

    public void Configure(TelegramDao options)
    {
        options.TelegramDbContext = _dbContextManager.Value;
    }
}

[Scope(typeof(ConfigureTelegramDaoService))]
public class TelegramDao
{
    public TelegramDbContext TelegramDbContext { get; set; }
    public TelegramDao() { }

    public TelegramDao(DbContextManager<TelegramDbContext> dbContextManager)
    {
        TelegramDbContext = dbContextManager.Value;
    }

    public void RegisterUser(Guid userId, int tenantId, int telegramId)
    {
        var user = new TelegramUser
        {
            PortalUserId = userId,
            TenantId = tenantId,
            TelegramUserId = telegramId
        };

        TelegramDbContext.AddOrUpdate(r => r.Users, user);
        TelegramDbContext.SaveChanges();
    }

    public TelegramUser GetUser(Guid userId, int tenantId)
    {
        return TelegramDbContext.Users
            .AsNoTracking()
            .Where(r => r.PortalUserId == userId)
            .Where(r => r.TenantId == tenantId)
            .FirstOrDefault();
    }

    public List<TelegramUser> GetUser(int telegramId)
    {
        return TelegramDbContext.Users
            .AsNoTracking()
            .Where(r => r.TelegramUserId == telegramId)
            .ToList();
    }

    public void Delete(Guid userId, int tenantId)
    {
        var toRemove = TelegramDbContext.Users
            .Where(r => r.PortalUserId == userId)
            .Where(r => r.TenantId == tenantId)
            .ToList();

        TelegramDbContext.Users.RemoveRange(toRemove);
        TelegramDbContext.SaveChanges();
    }

    public void Delete(int telegramId)
    {
        var toRemove = TelegramDbContext.Users
            .Where(r => r.TelegramUserId == telegramId)
            .ToList();

        TelegramDbContext.Users.RemoveRange(toRemove);
        TelegramDbContext.SaveChanges();
    }
}
