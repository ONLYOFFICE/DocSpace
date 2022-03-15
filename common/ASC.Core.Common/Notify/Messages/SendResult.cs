namespace ASC.Notify.Messages;

[Flags]
public enum SendResult
{
    OK = 1,
    IncorrectRecipient = 2,
    Impossible = 4,
    Inprogress = 8,
    Prevented = 16
}
