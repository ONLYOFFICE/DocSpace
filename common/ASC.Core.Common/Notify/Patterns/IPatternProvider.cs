namespace ASC.Notify.Patterns;

public interface IPatternProvider
{
    Func<INotifyAction, string, NotifyRequest, IPattern> GetPatternMethod { get; set; }
    IPattern GetPattern(INotifyAction action, string senderName);
    IPatternFormatter GetFormatter(IPattern pattern);
}
