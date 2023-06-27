// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

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

    public async Task<string> GetRefreshedTokenAsync(TokenHelper tokenHelper, OAuth20TokenHelper oAuth20TokenHelper, ThirdPartySelector thirdPartySelector)
    {
        if (IsExpired)
        {
            var app = thirdPartySelector.GetApp(App);
            try
            {
                tokenHelper.Logger.DebugRefreshToken(App);

                var refreshUrl = app.GetRefreshUrl();

                var refreshed = oAuth20TokenHelper.RefreshToken(refreshUrl, this);

                if (refreshed != null)
                {
                    AccessToken = refreshed.AccessToken;
                    RefreshToken = refreshed.RefreshToken;
                    ExpiresIn = refreshed.ExpiresIn;
                    Timestamp = DateTime.UtcNow;

                    await tokenHelper.SaveTokenAsync(this);
                }
            }
            catch (Exception ex)
            {
                tokenHelper.Logger.ErrorRefreshToken(App, ex);
            }
        }

        return AccessToken;
    }
}

[Scope]
public class TokenHelper
{
    private readonly IDbContextFactory<FilesDbContext> _dbContextFactory;
    public ILogger<TokenHelper> Logger;
    private readonly InstanceCrypto _instanceCrypto;
    private readonly AuthContext _authContext;
    private readonly TenantManager _tenantManager;

    public TokenHelper(
        IDbContextFactory<FilesDbContext> dbContextFactory,
        ILogger<TokenHelper> logger,
        InstanceCrypto instanceCrypto,
        AuthContext authContext,
        TenantManager tenantManager)
    {
        _dbContextFactory = dbContextFactory;
        Logger = logger;
        _instanceCrypto = instanceCrypto;
        _authContext = authContext;
        _tenantManager = tenantManager;
    }

    public async Task SaveTokenAsync(Token token)
    {
        var dbFilesThirdpartyApp = new DbFilesThirdpartyApp
        {
            App = token.App,
            Token = EncryptToken(token),
            UserId = _authContext.CurrentAccount.ID,
            TenantId = await _tenantManager.GetCurrentTenantIdAsync(),
            ModifiedOn = DateTime.UtcNow
        };

        await using var filesDbContext = _dbContextFactory.CreateDbContext();
        await filesDbContext.AddOrUpdateAsync(q => q.ThirdpartyApp, dbFilesThirdpartyApp);
        await filesDbContext.SaveChangesAsync();
    }

    public async Task<Token> GetTokenAsync(string app)
    {
        return await GetTokenAsync(app, _authContext.CurrentAccount.ID);
    }

    public async Task<Token> GetTokenAsync(string app, Guid userId)
    {
        var tenant = await _tenantManager.GetCurrentTenantAsync();
        await using var filesDbContext = _dbContextFactory.CreateDbContext();
        var oAuth20Token = await Queries.TokenAsync(filesDbContext, tenant.Id, userId, app);

        if (oAuth20Token == null)
        {
            return null;
        }

        return new Token(DecryptToken(oAuth20Token), app);
    }

    public async Task DeleteTokenAsync(string app, Guid? userId = null)
    {
        var tenant = await _tenantManager.GetCurrentTenantAsync();
        await using var filesDbContext = _dbContextFactory.CreateDbContext();
        await Queries.DeleteTokenAsync(filesDbContext, tenant.Id, userId ?? _authContext.CurrentAccount.ID, app);
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

static file class Queries
{
    public static readonly Func<FilesDbContext, int, Guid, string, Task<string>> TokenAsync =
        EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, Guid userId, string app) =>
                ctx.ThirdpartyApp
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => r.UserId == userId)
                    .Where(r => r.App == app)
                    .Select(r => r.Token)
                    .FirstOrDefault());

    public static readonly Func<FilesDbContext, int, Guid, string, Task<int>> DeleteTokenAsync =
        EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, Guid userId, string app) =>
                ctx.ThirdpartyApp
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => r.UserId == userId)
                    .Where(r => r.App == app)
                    .ExecuteDelete());
}