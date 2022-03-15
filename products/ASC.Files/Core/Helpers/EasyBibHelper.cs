namespace ASC.Web.Files.Helpers;

[Scope]
public class EasyBibHelper : Consumer
{
    public ILog Logger { get; set; }

    static readonly string SearchBookUrl = "https://worldcat.citation-api.com/query?search=",
                    SearchJournalUrl = "https://crossref.citation-api.com/query?search=",
                    SearchWebSiteUrl = "https://web.citation-api.com/query?search=",
                    EasyBibStyles = "https://api.citation-api.com/2.1/rest/styles";

    public enum EasyBibSource
    {
        book = 0,
        journal = 1,
        website = 2
    }

    public string AppKey => this["easyBibappkey"];

    public EasyBibHelper() { }

    public EasyBibHelper(
        IOptionsMonitor<ILog> option,
        TenantManager tenantManager,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        IConfiguration configuration,
        ICacheNotify<ConsumerCacheItem> cache,
        ConsumerFactory factory,
        string name,
        int order,
        Dictionary<string, string> props,
        Dictionary<string, string> additional = null)
        : base(tenantManager, coreBaseSettings, coreSettings, configuration, cache, factory, name, order, props, additional)
    {
        Logger = option.CurrentValue;
    }

    public static string GetEasyBibCitationsList(int source, string data)
    {
        var uri = "";
        switch (source)
        {
            case 0:
                uri = SearchBookUrl;
                break;
            case 1:
                uri = SearchJournalUrl;
                break;
            case 2:
                uri = SearchWebSiteUrl;
                break;
            default:
                break;
        }
        uri += data;

        const string method = "GET";
        var headers = new Dictionary<string, string>() { };
        try
        {
            return RequestHelper.PerformRequest(uri, "", method, "", headers);
        }
        catch (Exception)
        {
            return "error";
        }

    }

    public static string GetEasyBibStyles()
    {

        const string method = "GET";
        var headers = new Dictionary<string, string>() { };
        try
        {
            return RequestHelper.PerformRequest(EasyBibStyles, "", method, "", headers);
        }
        catch (Exception)
        {
            return "error";
        }
    }

    public object GetEasyBibCitation(string data)
    {
        try
        {
            var easyBibappkey = ConsumerFactory.Get<EasyBibHelper>().AppKey;

            var jsonBlogInfo = JObject.Parse(data);
            jsonBlogInfo.Add("key", easyBibappkey);
            var citationData = jsonBlogInfo.ToString();

            const string uri = "https://api.citation-api.com/2.0/rest/cite";
            const string contentType = "application/json";
            const string method = "POST";
            var body = citationData;
            var headers = new Dictionary<string, string>() { };

            return RequestHelper.PerformRequest(uri, contentType, method, body, headers);

        }
        catch (Exception)
        {
            return null;
            throw;
        }

    }
}
