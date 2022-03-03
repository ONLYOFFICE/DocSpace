namespace ASC.People;

public class Startup : BaseStartup
{
    public override bool ConfirmAddScheme => true;

    public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment) : base(configuration, hostEnvironment)
    {
    }

}