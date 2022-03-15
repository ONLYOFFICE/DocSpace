namespace ASC.Notify.Patterns;

public class Pattern : IPattern
{
    public const string HtmlContentType = "html";
    public const string TextContentType = "text";
    public const string RtfContentType = "rtf";


    public string ID { get; private set; }
    public string Subject { get; private set; }
    public string Body { get; private set; }
    public string ContentType { get; internal set; }
    public string Styler { get; internal set; }


    public Pattern(string id, string subject, string body, string contentType)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("id");
        }

        ID = id;
        Subject = subject ?? throw new ArgumentNullException(nameof(subject));
        Body = body ?? throw new ArgumentNullException(nameof(body));
        ContentType = string.IsNullOrEmpty(contentType) ? HtmlContentType : contentType;
    }


    public override bool Equals(object obj)
    {
        return obj is IPattern p && p.ID == ID;
    }

    public override int GetHashCode()
    {
        return ID.GetHashCode();
    }

    public override string ToString()
    {
        return ID;
    }
}
