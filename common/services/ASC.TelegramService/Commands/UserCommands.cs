namespace ASC.TelegramService.Commands;

[Scope]
public class UserCommands : CommandContext
{
    private readonly CachedTelegramDao _cachedTelegramDao;

    public UserCommands(IOptionsSnapshot<CachedTelegramDao> cachedTelegramDao)
    {
        _cachedTelegramDao = cachedTelegramDao.Value;
    }

    [Command("start")]
    public Task StartCommand(string token)
    {
        if (string.IsNullOrEmpty(token)) return Task.CompletedTask;

        return InternalStartCommand(token);
    }

    private async Task InternalStartCommand(string token)
    {
        var user = MemoryCache.Default.Get(token);
        if (user != null)
        {
            MemoryCache.Default.Remove(token);
            MemoryCache.Default.Remove((string)user);
            var split = ((string)user).Split(':');

            var guid = Guid.Parse(split[0]);
            var tenant = int.Parse(split[1]);

            if (tenant == TenantId)
            {
                _cachedTelegramDao.RegisterUser(guid, tenant, Context.User.Id);
                await ReplyAsync("Ok!");
                return;
            }
        }

        await ReplyAsync("Error");
    }
}