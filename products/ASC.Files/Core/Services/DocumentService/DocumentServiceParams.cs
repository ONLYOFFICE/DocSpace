namespace ASC.Web.Files.Services.DocumentService;

public class DocumentServiceParams
{
    public string DisplayName { get; set; }
    public string DocKeyForTrack { get; set; }
    public bool EditByUrl { get; set; }
    public string Email { get; set; }
    public string FileId { get; set; }
    public string FileProviderKey { get; set; }
    public int FileVersion { get; set; }
    public string LinkToEdit { get; set; }
    public bool OpenHistory { get; set; }
    public string OpeninigDate { get; set; }
    public string ServerErrorMessage { get; set; }
    public string ShareLinkParam { get; set; }
    public string TabId { get; set; }
    public bool ThirdPartyApp { get; set; }
    public bool CanGetUsers { get; set; }
    public string PageTitlePostfix { get; set; }

    public static string Serialize(DocumentServiceParams docServiceParams)
    {
        return System.Text.Json.JsonSerializer.Serialize(docServiceParams);
    }
}
