namespace ASC.Notify.Model;

public interface INotifySource
{
    string Id { get; }
    IActionProvider GetActionProvider();
    IPatternProvider GetPatternProvider();
    IRecipientProvider GetRecipientsProvider();
    ISubscriptionProvider GetSubscriptionProvider();
}
