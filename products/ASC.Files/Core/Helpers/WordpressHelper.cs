namespace ASC.Web.Files.Helpers;

[Scope]
public class WordpressToken
{
    public ILog Logger { get; set; }
    private readonly TokenHelper _tokenHelper;
    public ConsumerFactory ConsumerFactory { get; }

    public const string AppAttr = "wordpress";

    public WordpressToken(IOptionsMonitor<ILog> optionsMonitor, TokenHelper tokenHelper, ConsumerFactory consumerFactory)
    {
        Logger = optionsMonitor.CurrentValue;
        _tokenHelper = tokenHelper;
        ConsumerFactory = consumerFactory;
    }

    public OAuth20Token GetToken()
    {
        return _tokenHelper.GetToken(AppAttr);
    }

    public void SaveToken(OAuth20Token token)
    {
        ArgumentNullException.ThrowIfNull(token);

        _tokenHelper.SaveToken(new Token(token, AppAttr));
    }

    public OAuth20Token SaveTokenFromCode(string code)
    {
        var token = OAuth20TokenHelper.GetAccessToken<WordpressLoginProvider>(ConsumerFactory, code);
        ArgumentNullException.ThrowIfNull(token);

        _tokenHelper.SaveToken(new Token(token, AppAttr));

        return token;
    }

    public void DeleteToken(OAuth20Token token)
    {
        ArgumentNullException.ThrowIfNull(token);

        _tokenHelper.DeleteToken(AppAttr);

    }
}

[Singletone]
public class WordpressHelper
{
    public ILog Logger { get; set; }

    public enum WordpressStatus
    {
        draft = 0,
        publish = 1
    }

    public WordpressHelper(IOptionsMonitor<ILog> optionsMonitor)
    {
        Logger = optionsMonitor.CurrentValue;
    }

    public string GetWordpressMeInfo(string token)
    {
        try
        {
            return WordpressLoginProvider.GetWordpressMeInfo(token);
        }
        catch (Exception ex)
        {
            Logger.Error("Get Wordpress info about me ", ex);

            return string.Empty;
        }

    }

    public bool CreateWordpressPost(string title, string content, int status, string blogId, OAuth20Token token)
    {
        try
        {
            var wpStatus = ((WordpressStatus)status).ToString();
            WordpressLoginProvider.CreateWordpressPost(title, content, wpStatus, blogId, token);

            return true;
        }
        catch (Exception ex)
        {
            Logger.Error("Create Wordpress post ", ex);

            return false;
        }
    }
}
