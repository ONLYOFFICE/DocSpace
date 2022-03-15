using NotifySourceBase = ASC.Core.Notify.NotifySource;
using SubscriptionManager = ASC.Core.SubscriptionManager;

namespace ASC.Files.Core.Services.NotifyService;

[Scope]
public class NotifySource : NotifySourceBase
{
    public NotifySource(UserManager userManager, IRecipientProvider recipientsProvider, SubscriptionManager subscriptionManager)
        : base(new Guid("6FE286A4-479E-4c25-A8D9-0156E332B0C0"), userManager, recipientsProvider, subscriptionManager)
    {
    }

    protected override IActionProvider CreateActionProvider()
    {
        return new ConstActionProvider(
            NotifyConstants.EventShareFolder,
            NotifyConstants.EventShareDocument);
    }

    protected override IPatternProvider CreatePatternsProvider()
    {
        return new XmlPatternProvider2(FilesPatternResource.patterns);
    }
}
