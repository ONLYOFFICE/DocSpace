namespace ASC.TelegramService;

public class Startup : BaseStartup
{
    public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment) : base(configuration, hostEnvironment)
    {
        LoadProducts = false;
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        DIHelper.TryAdd<TelegramListenerService>();

        services.AddHostedService<TelegramListenerService>();
    }
}
