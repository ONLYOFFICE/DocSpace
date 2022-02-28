namespace ASC.People;

public class Startup : BaseStartup
{
    public override bool ConfirmAddScheme => true;

    public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment) : base(configuration, hostEnvironment)
    {
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        DIHelper.TryAdd<UserController>();
        DIHelper.TryAdd<ContactsController>();
        DIHelper.TryAdd<NotificationController>();
        DIHelper.TryAdd<PhotoController>();
        DIHelper.TryAdd<ReassignController>();
        DIHelper.TryAdd<RemoveUserDataController>();
        DIHelper.TryAdd<ThirdpartyController>();
        DIHelper.TryAdd<GroupController>();
    }
}