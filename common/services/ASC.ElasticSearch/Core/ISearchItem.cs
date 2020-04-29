using Nest;

using Newtonsoft.Json;

namespace ASC.ElasticSearch
{
    public interface ISearchItem
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string IndexName { get; }
    }

    public interface ISearchItemDocument : ISearchItem
    {
        public Document Document { get; set; }

        [Ignore, JsonIgnore]
        public abstract string SettingsTitle { get; }
    }
}
