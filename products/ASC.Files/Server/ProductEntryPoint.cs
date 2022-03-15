namespace ASC.Files;

[Scope]
public class ApiProductEntryPoint : ProductEntryPoint
{
    public override string ApiURL
    {
        get => "api/2.0/files/info.json";
    }

    public ApiProductEntryPoint(
       //FilesSpaceUsageStatManager filesSpaceUsageStatManager,
       CoreBaseSettings coreBaseSettings,
       AuthContext authContext,
       UserManager userManager
       //SubscriptionManager subscriptionManager
       ) : base(coreBaseSettings, authContext, userManager, null)
    {

    }
}
