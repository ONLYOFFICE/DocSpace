namespace ASC.Notify.Sinks;

public interface ISink
{
    ISink NextSink { get; set; }
    SendResponse ProcessMessage(INoticeMessage message);
    void ProcessMessageAsync(INoticeMessage message);
}
