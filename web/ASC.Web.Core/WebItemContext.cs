namespace ASC.Web.Core
{
    public class WebItemContext
    {
        public SpaceUsageStatManager SpaceUsageStatManager { get; set; }

        public Func<string> GetCreateContentPageAbsoluteUrl { get; set; }

        public ISubscriptionManager SubscriptionManager { get; set; }

        public Func<List<string>> UserOpportunities { get; set; }

        public Func<List<string>> AdminOpportunities { get; set; }

        public string SmallIconFileName { get; set; }

        public string DisabledIconFileName { get; set; }

        public string IconFileName { get; set; }

        public string LargeIconFileName { get; set; }

        public int DefaultSortOrder { get; set; }

        public bool HasComplexHierarchyOfAccessRights { get; set; }

        public bool CanNotBeDisabled { get; set; }

        public WebItemContext()
        {
            GetCreateContentPageAbsoluteUrl = () => string.Empty;
        }
    }
}
