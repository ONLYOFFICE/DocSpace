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
    }
}
