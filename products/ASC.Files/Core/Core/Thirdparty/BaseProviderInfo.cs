namespace ASC.Files.Core.Thirdparty;

public class BaseProviderInfo<T> where T : IProviderInfo
{
    public T ProviderInfo { get; set; }
    public string Path { get; set; }
    public string PathPrefix { get; set; }
}
