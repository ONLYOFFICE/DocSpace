namespace ASC.Core.Notify.Senders;

public enum NoticeSendResult
{
    OK,
    TryOnceAgain,
    MessageIncorrect,
    SendingImpossible,
}
