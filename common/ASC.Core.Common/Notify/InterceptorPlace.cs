namespace ASC.Notify;

[Flags]
public enum InterceptorPlace
{
    Prepare = 1,
    GroupSend = 2,
    DirectSend = 4,
    MessageSend = 8
}
