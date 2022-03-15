namespace ASC.Notify.Patterns;

public interface IPattern
{
    string ID { get; }
    string Subject { get; }
    string Body { get; }
    string ContentType { get; }
    string Styler { get; }
}
