using System;
using System.Linq.Expressions;

namespace ASC.ElasticSearch
{
    public interface ISearchItem
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string IndexName { get; }

        public Expression<Func<ISearchItem, object[]>> SearchContentFields { get; }
    }

    public interface ISearchItemDocument : ISearchItem
    {
        public Document Document { get; set; }
    }
}
