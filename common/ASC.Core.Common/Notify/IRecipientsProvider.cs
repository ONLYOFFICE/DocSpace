namespace ASC.Notify.Recipients;

[Scope(typeof(RecipientProviderImpl))]
public interface IRecipientProvider
{
    IDirectRecipient FilterRecipientAddresses(IDirectRecipient recipient);
    IRecipient GetRecipient(string id);
    IRecipient[] GetGroupEntries(IRecipientsGroup group);
    IRecipientsGroup[] GetGroups(IRecipient recipient);
    string[] GetRecipientAddresses(IDirectRecipient recipient, string senderName);
}
