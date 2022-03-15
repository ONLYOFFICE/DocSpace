namespace ASC.Web.Core
{
    [Serializable]
    public class WebItemSecurityInfo
    {
        public string WebItemId { get; set; }

        public IEnumerable<UserInfo> Users { get; set; }

        public IEnumerable<GroupInfo> Groups { get; set; }

        public bool Enabled { get; set; }
    }
}
