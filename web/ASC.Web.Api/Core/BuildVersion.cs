namespace ASC.Api.Settings;

[Scope]
public class BuildVersion
{
    public string CommunityServer { get; set; }

    public string DocumentServer { get; set; }

    public string MailServer { get; set; }

    public string XmppServer { get; set; }

    [JsonIgnore]
    private readonly IConfiguration _configuration;

    [JsonIgnore]
    private readonly FilesLinkUtility _filesLinkUtility;

    [JsonIgnore]
    private readonly DocumentServiceConnector _documentServiceConnector;

    public BuildVersion(IConfiguration configuration, FilesLinkUtility filesLinkUtility, DocumentServiceConnector documentServiceConnector)
    {
        _configuration = configuration;
        _filesLinkUtility = filesLinkUtility;
        _documentServiceConnector = documentServiceConnector;
    }

    public async Task<BuildVersion> GetCurrentBuildVersionAsync()
    {
        CommunityServer = GetCommunityVersion();
        DocumentServer = await GetDocumentVersionAsync();
        MailServer = GetMailServerVersion();
        XmppServer = GetXmppServerVersion();

        return this;
    }

    private string GetCommunityVersion()
    {
        return _configuration["version:number"] ?? "8.5.0";
    }

    private Task<string> GetDocumentVersionAsync()
    {
        if (string.IsNullOrEmpty(_filesLinkUtility.DocServiceApiUrl))
            return null;

        return _documentServiceConnector.GetVersionAsync();
    }

    private static string GetMailServerVersion()
    {
        //TODO
        return "";
        /*
        try
        {
               
            var engineFactory = new EngineFactory(
                CoreContext.TenantManager.GetCurrentTenant().Id,
                SecurityContext.CurrentAccount.ID.ToString());

            var version = engineFactory.ServerEngine.GetServerVersion();
            return version;
            }
        catch (Exception e)
        {
            LogManager.GetLogger("ASC").Warn(e.Message, e);
        }

        return null;*/
    }

    private static string GetXmppServerVersion()
    {
        //try
        //{
        //    if (ConfigurationManagerExtension.AppSettings["web.talk"] != "true")
        //        return null;

        //    return new JabberServiceClient().GetVersion();
        //}
        //catch (Exception e)
        //{
        //    LogManager.GetLogger("ASC").Warn(e.Message, e);
        //}

        return null;
    }
}
