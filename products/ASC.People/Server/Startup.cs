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
        DIHelper.TryAdd<UserControllerEngine>();

        DIHelper.TryAdd<ContactsController>();
        DIHelper.TryAdd<ContactsControllerEngine>();

        DIHelper.TryAdd<NotificationController>();
        DIHelper.TryAdd<NotificationControllerEngine>();

        DIHelper.TryAdd<PhotoController>();
        DIHelper.TryAdd<PhotoControllerEngine>();

        DIHelper.TryAdd<ReassignController>();
        DIHelper.TryAdd<ReassignControllerEngine>();

        DIHelper.TryAdd<RemoveUserDataController>();
        DIHelper.TryAdd<RemoveUserDataController>();

        DIHelper.TryAdd<ThirdpartyController>();
        DIHelper.TryAdd<ThirdpartyControllerEngine>();

        DIHelper.TryAdd<GroupController>();
        DIHelper.TryAdd<GroupControllerEngine>();
    }
}