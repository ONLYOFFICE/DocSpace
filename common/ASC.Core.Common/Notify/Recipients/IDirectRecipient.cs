namespace ASC.Notify.Recipients;

public interface IDirectRecipient : IRecipient
{
    string[] Addresses { get; }
    bool CheckActivation { get; }
}
