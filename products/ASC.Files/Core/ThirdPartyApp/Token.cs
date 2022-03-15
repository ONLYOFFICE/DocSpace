namespace ASC.Web.Files.ThirdPartyApp;

[DebuggerDisplay("{App} - {AccessToken}")]
public class Token : OAuth20Token
{
    public string App { get; private set; }

    public Token(OAuth20Token oAuth20Token, string app)
        : base(oAuth20Token)
    {
        App = app;
    }

    public string GetRefreshedToken(TokenHelper tokenHelper)
    {
        if (IsExpired)
        {
            var app = ThirdPartySelector.GetApp(App);
            try
            {
                tokenHelper.Logger.Debug("Refresh token for app: " + App);

                var refreshUrl = app.GetRefreshUrl();

                var refreshed = OAuth20TokenHelper.RefreshToken(refreshUrl, this);

                if (refreshed != null)
                {
                    AccessToken = refreshed.AccessToken;
                    RefreshToken = refreshed.RefreshToken;
                    ExpiresIn = refreshed.ExpiresIn;
                    Timestamp = DateTime.UtcNow;

                    tokenHelper.SaveToken(this);
                }
            }
            catch (Exception ex)
            {
                tokenHelper.Logger.Error("Refresh token for app: " + app, ex);
            }
        }

        return AccessToken;
    }
}

[Scope]
public class TokenHelper
{
    public ILog Logger { get; }
    private readonly Lazy<FilesDbContext> _lazyFilesDbContext;
    private FilesDbContext FilesDbContext => _lazyFilesDbContext.Value;
    private readonly InstanceCrypto _instanceCrypto;
    private readonly AuthContext _authContext;
    private readonly TenantManager _tenantManager;

    public TokenHelper(
        DbContextManager<FilesDbContext> dbContextManager,
        IOptionsMonitor<ILog> option,
        InstanceCrypto instanceCrypto,
        AuthContext authContext,
        TenantManager tenantManager)
    {
        Logger = option.CurrentValue;
        _lazyFilesDbContext = new Lazy<FilesDbContext>(() => dbContextManager.Get(FileConstant.DatabaseId));
        _instanceCrypto = instanceCrypto;
        _authContext = authContext;
        _tenantManager = tenantManager;
    }

    public void SaveToken(Token token)
    {
        var dbFilesThirdpartyApp = new DbFilesThirdpartyApp
        {
            App = token.App,
            Token = EncryptToken(token),
            UserId = _authContext.CurrentAccount.ID,
            TenantId = _tenantManager.GetCurrentTenant().Id
        };

        FilesDbContext.AddOrUpdate(r => r.ThirdpartyApp, dbFilesThirdpartyApp);
        FilesDbContext.SaveChanges();
    }

    public Token GetToken(string app)
    {
        return GetToken(app, _authContext.CurrentAccount.ID);
    }

    public Token GetToken(string app, Guid userId)
    {
        var oAuth20Token = FilesDbContext.ThirdpartyApp
            .AsQueryable()
            .Where(r => r.TenantId == _tenantManager.GetCurrentTenant().Id)
            .Where(r => r.UserId == userId)
            .Where(r => r.App == app)
            .Select(r => r.Token)
            .FirstOrDefault();

        if (oAuth20Token == null)
        {
            return null;
        }

        return new Token(DecryptToken(oAuth20Token), app);
    }

    public void DeleteToken(string app, Guid? userId = null)
    {
        var apps = FilesDbContext.ThirdpartyApp
            .AsQueryable()
            .Where(r => r.TenantId == _tenantManager.GetCurrentTenant().Id)
            .Where(r => r.UserId == (userId ?? _authContext.CurrentAccount.ID))
            .Where(r => r.App == app);

        FilesDbContext.RemoveRange(apps);
        FilesDbContext.SaveChanges();
    }

    private string EncryptToken(OAuth20Token token)
    {
        var t = token.ToJson();

        return string.IsNullOrEmpty(t) ? string.Empty : _instanceCrypto.Encrypt(t);
    }

    private OAuth20Token DecryptToken(string token)
    {
        return string.IsNullOrEmpty(token) ? null : OAuth20Token.FromJson(_instanceCrypto.Decrypt(token));
    }
}
