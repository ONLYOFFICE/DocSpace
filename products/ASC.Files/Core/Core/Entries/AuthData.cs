namespace ASC.Files.Core;

[DebuggerDisplay("{Login} {Password} {Token} {Url}")]
public class AuthData
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string Token { get; set; }
    public string Url { get; set; }

    public AuthData(string url = null, string login = null, string password = null, string token = null)
    {
        Url = url ?? string.Empty;
        Login = login ?? string.Empty;
        Password = password ?? string.Empty;
        Token = token ?? string.Empty;
    }

    public bool IsEmpty()
    {
        return string.IsNullOrEmpty((Url ?? string.Empty) + (Login ?? string.Empty) + (Password ?? string.Empty) + (Token ?? string.Empty));
    }
}
