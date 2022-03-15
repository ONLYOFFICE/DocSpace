namespace ASC.Web.Core
{
    public abstract class SpaceUsageStatManager
    {
        public class UsageSpaceStatItem
        {
            public string Name { get; set; }
            public string Url { get; set; }
            public string ImgUrl { get; set; }
            public bool Disabled { get; set; }
            public long SpaceUsage { get; set; }
        }

        public abstract ValueTask<System.Collections.Generic.List<UsageSpaceStatItem>> GetStatDataAsync();
    }

    public interface IUserSpaceUsage
    {
        Task<long> GetUserSpaceUsageAsync(Guid userId);
    }
}
