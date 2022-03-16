namespace ASC.Web.Api.Models
{
    public class DeepLinkWrapper
    {
        public DocumentsDeepLinkWrapper Documents { get; set; }
    }

    public class DocumentsDeepLinkWrapper
    {
        public string AndroidPackageName { get; set; }
        public string Url { get; set; }
        public string IosPackageId { get; set; }
    }
}
