namespace ASC.ElasticSearch;

public enum Analyzer
{
    standard,
    whitespace,
    uax_url_email
}

[Flags]
public enum CharFilter
{
    io,
    html
}

[Flags]
public enum Filter
{
    lowercase
}
