namespace ASC.Files.Core.Security;

public interface IFileSecurityProvider
{
    IFileSecurity GetFileSecurity(string data);
    Dictionary<object, IFileSecurity> GetFileSecurity(Dictionary<string, string> data);
}
