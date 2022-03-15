namespace ASC.ElasticSearch;

public interface IIndexer
{
    string IndexName { get; }

    void IndexAll();

    Task ReIndex();
}
