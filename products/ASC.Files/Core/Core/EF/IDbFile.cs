namespace ASC.Files.Core.EF;

public interface IDbFile
{
    public int TenantId { get; set; }
}

public interface IDbSearch
{
    public string Title { get; set; }
}
