namespace ASC.Common.Web;

public class RouteCallInfo
{
    public int? Tid { get; set; }
    public string Url { get; set; }
    public string AttachmentUrl { get; set; }
    public bool IsNewRequest { get; set; }
    public string Method { get; set; }
    public Dictionary<string, object> Params { get; set; }
    public bool CleanupHtml { get; set; }

    public RouteCallInfo()
    {
        CleanupHtml = true; //Default
    }

    public override string ToString()
    {
        return string.Format("{0} {1} T:{2},{3}", Method.ToUpper(), Url, Tid,
            string.Join(",",
            Params.Select(x => string.Format("{0}={1}", x.Key, x.Value)).ToArray()));
    }
}
