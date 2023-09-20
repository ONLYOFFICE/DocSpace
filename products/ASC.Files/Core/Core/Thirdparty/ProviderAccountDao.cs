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

namespace ASC.Files.Thirdparty;

[EnumExtensions]
public enum ProviderTypes
{
    Box,
    BoxNet,
    DropBox,
    DropboxV2,
    Google,
    GoogleDrive,
    OneDrive,
    SharePoint,
    SkyDrive,
    WebDav,
    kDrive,
    Yandex,
}

[Scope]
internal class ProviderAccountDao : IProviderDao
{
    protected int TenantID
    {
        get
        {
            return _tenantManager.GetCurrentTenant().Id;
        }
    }

    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TenantUtil _tenantUtil;
    private readonly TenantManager _tenantManager;
    private readonly InstanceCrypto _instanceCrypto;
    private readonly SecurityContext _securityContext;
    private readonly ConsumerFactory _consumerFactory;
    private readonly ThirdpartyConfiguration _thirdpartyConfiguration;
    private readonly IDbContextFactory<FilesDbContext> _dbContextFactory;
    private readonly OAuth20TokenHelper _oAuth20TokenHelper;

    public ProviderAccountDao(
        IServiceProvider serviceProvider,
        TenantUtil tenantUtil,
        TenantManager tenantManager,
        InstanceCrypto instanceCrypto,
        SecurityContext securityContext,
        ConsumerFactory consumerFactory,
        ThirdpartyConfiguration thirdpartyConfiguration,
        IDbContextFactory<FilesDbContext> dbContextFactory,
            OAuth20TokenHelper oAuth20TokenHelper,
        ILoggerProvider options)
    {
        _logger = options.CreateLogger("ASC.Files");
        _serviceProvider = serviceProvider;
        _tenantUtil = tenantUtil;
        _tenantManager = tenantManager;
        _instanceCrypto = instanceCrypto;
        _securityContext = securityContext;
        _consumerFactory = consumerFactory;
        _thirdpartyConfiguration = thirdpartyConfiguration;
        _dbContextFactory = dbContextFactory;
        _oAuth20TokenHelper = oAuth20TokenHelper;
    }

    public virtual Task<IProviderInfo> GetProviderInfoAsync(int linkId)
    {
        var providersInfo = GetProvidersInfoInternalAsync(linkId);

        return providersInfo.SingleAsync().AsTask();
    }

    public async Task<IProviderInfo> GetProviderInfoByEntryIdAsync(string entryId)
    {
        try
        {
            var id = Selectors.Pattern.Match(entryId).Groups["id"].Value;
            return await GetProviderInfoAsync(int.Parse(id));
        }
        catch
        {
            return null;
        }
    }

    public virtual IAsyncEnumerable<IProviderInfo> GetProvidersInfoAsync()
    {
        return GetProvidersInfoInternalAsync();
    }

    public virtual IAsyncEnumerable<IProviderInfo> GetProvidersInfoAsync(FolderType folderType, string searchText = null)
    {
        return GetProvidersInfoInternalAsync(folderType: folderType, searchText: searchText);
    }

    public virtual IAsyncEnumerable<IProviderInfo> GetProvidersInfoAsync(Guid userId)
    {
        try
        {
            var filesDbContext = _dbContextFactory.CreateDbContext();
            var thirdpartyAccounts = Queries.ThirdpartyAccountsAsync(filesDbContext, TenantID, userId);

            return thirdpartyAccounts.Select(ToProviderInfo);
        }
        catch (Exception e)
        {
            _logger.ErrorGetProvidersInfoInternalUser(userId, e);

            return new List<IProviderInfo>().ToAsyncEnumerable();
        }
    }

    private IAsyncEnumerable<IProviderInfo> GetProvidersInfoInternalAsync(int linkId = -1, FolderType folderType = FolderType.DEFAULT, string searchText = null)
    {
        try
        {
            var filesDbContext = _dbContextFactory.CreateDbContext();
            return Queries.ThirdpartyAccountsByFilterAsync(filesDbContext, TenantID, linkId, folderType, _securityContext.CurrentAccount.ID, GetSearchText(searchText))
                .Select(ToProviderInfo);
        }
        catch (Exception e)
        {
            _logger.ErrorGetProvidersInfoInternal(linkId, folderType, _securityContext.CurrentAccount.ID, e);
            return new List<IProviderInfo>().ToAsyncEnumerable();
        }
    }

    public virtual async Task<int> SaveProviderInfoAsync(string providerKey, string customerTitle, AuthData authData, FolderType folderType)
    {
        ProviderTypes prKey;
        try
        {
            prKey = (ProviderTypes)Enum.Parse(typeof(ProviderTypes), providerKey, true);
        }
        catch (Exception)
        {
            throw new ArgumentException("Unrecognize ProviderType");
        }

        authData = GetEncodedAccesToken(authData, prKey);

        if (!await CheckProviderInfoAsync(ToProviderInfo(0, prKey, customerTitle, authData, _securityContext.CurrentAccount.ID, folderType, _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow()))))
        {
            throw new UnauthorizedAccessException(string.Format(FilesCommonResource.ErrorMassage_SecurityException_Auth, providerKey));
        }

        var dbFilesThirdpartyAccount = new DbFilesThirdpartyAccount
        {
            Id = 0,
            TenantId = TenantID,
            Provider = providerKey,
            Title = Global.ReplaceInvalidCharsAndTruncate(customerTitle),
            UserName = authData.Login ?? "",
            Password = EncryptPassword(authData.Password),
            FolderType = folderType,
            CreateOn = _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow()),
            UserId = _securityContext.CurrentAccount.ID,
            Token = EncryptPassword(authData.Token ?? ""),
            Url = authData.Url ?? ""
        };

        await using var filesDbContext = _dbContextFactory.CreateDbContext();
        var res = await filesDbContext.AddOrUpdateAsync(r => r.ThirdpartyAccount, dbFilesThirdpartyAccount);
        await filesDbContext.SaveChangesAsync();

        return res.Id;
    }

    public async Task<bool> CheckProviderInfoAsync(IProviderInfo providerInfo)
    {
        return providerInfo != null && await providerInfo.CheckAccessAsync();
    }

    public async Task<bool> UpdateProviderInfoAsync(int linkId, FolderType rootFolderType)
    {
        await using var filesDbContext = _dbContextFactory.CreateDbContext();

        var forUpdate = await Queries.ThirdpartyAccountAsync(filesDbContext, TenantID, linkId);

        if (forUpdate == null)
        {
            return false;
        }

        forUpdate.FolderType = rootFolderType;
        filesDbContext.Update(forUpdate);

        await filesDbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateProviderInfoAsync(int linkId, bool hasLogo)
    {
        await using var filesDbContext = _dbContextFactory.CreateDbContext();
        var forUpdate = await Queries.ThirdpartyAccountAsync(filesDbContext, TenantID, linkId);

        if (forUpdate == null)
        {
            return false;
        }

        forUpdate.HasLogo = hasLogo;
        filesDbContext.Update(forUpdate);

        await filesDbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateProviderInfoAsync(int linkId, string title, string folderId, FolderType roomType, bool @private)
    {
        await using var filesDbContext = _dbContextFactory.CreateDbContext();
        var forUpdate = await Queries.ThirdpartyAccountAsync(filesDbContext, TenantID, linkId);

        if (forUpdate == null)
        {
            return false;
        }

        forUpdate.RoomType = roomType;
        forUpdate.FolderId = folderId;
        forUpdate.FolderType = FolderType.VirtualRooms;
        forUpdate.Private = @private;
        forUpdate.Title = title;
        filesDbContext.Update(forUpdate);

        await filesDbContext.SaveChangesAsync();

        return true;
    }

    public virtual async Task<int> UpdateProviderInfoAsync(int linkId, AuthData authData)
    {
        await using var filesDbContext = _dbContextFactory.CreateDbContext();
        var tenantId = TenantID;
        var login = authData.Login ?? "";
        var password = EncryptPassword(authData.Password);
        var token = EncryptPassword(authData.Token ?? "");
        var url = authData.Url ?? "";

        var forUpdateCount = await Queries.UpdateThirdpartyAccountsAsync(filesDbContext, tenantId, linkId, login, password, token, url);

        return forUpdateCount == 1 ? linkId : default;
    }

    public virtual async Task<int> UpdateProviderInfoAsync(int linkId, string customerTitle, AuthData newAuthData, FolderType folderType, Guid? userId = null)
    {
        await using var filesDbContext = _dbContextFactory.CreateDbContext();

        var authData = new AuthData();
        if (newAuthData != null && !newAuthData.IsEmpty())
        {
            DbFilesThirdpartyAccount input;
            try
            {
                input = await Queries.ThirdpartyAccountByLinkIdAsync(filesDbContext, TenantID, linkId);
            }
            catch (Exception e)
            {
                _logger.ErrorUpdateProviderInfo(linkId, _securityContext.CurrentAccount.ID, e);
                throw;
            }

            if (!ProviderTypesExtensions.TryParse(input.Provider, true, out var key))
            {
                throw new ArgumentException("Unrecognize ProviderType");
            }

            authData = new AuthData(
                !string.IsNullOrEmpty(newAuthData.Url) ? newAuthData.Url : input.Url,
                input.UserName,
                !string.IsNullOrEmpty(newAuthData.Password) ? newAuthData.Password : DecryptPassword(input.Password, linkId),
                newAuthData.Token);

            if (!string.IsNullOrEmpty(newAuthData.Token))
            {
                authData = GetEncodedAccesToken(authData, key);
            }

            if (!await CheckProviderInfoAsync(ToProviderInfo(0, key, customerTitle, authData, _securityContext.CurrentAccount.ID, folderType, _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow()))))
            {
                throw new UnauthorizedAccessException(string.Format(FilesCommonResource.ErrorMassage_SecurityException_Auth, key));
            }
        }

        var toUpdate = Queries.ThirdpartyAccountsByLinkIdAsync(filesDbContext, TenantID, linkId);
        var toUpdateCount = 0;

        await foreach (var t in toUpdate)
        {
            if (!string.IsNullOrEmpty(customerTitle))
            {
                t.Title = customerTitle;
            }

            if (folderType != FolderType.DEFAULT)
            {
                t.FolderType = folderType;
            }

            if (userId.HasValue)
            {
                t.UserId = userId.Value;
            }

            if (!authData.IsEmpty())
            {
                t.UserName = authData.Login ?? "";
                t.Password = EncryptPassword(authData.Password);
                t.Token = EncryptPassword(authData.Token ?? "");
                t.Url = authData.Url ?? "";
            }

            toUpdateCount++;
        }

        await filesDbContext.SaveChangesAsync();

        return toUpdateCount == 1 ? linkId : default;
    }

    public virtual async Task<int> UpdateBackupProviderInfoAsync(string providerKey, string customerTitle, AuthData newAuthData)
    {
        await using var filesDbContext = _dbContextFactory.CreateDbContext();

        DbFilesThirdpartyAccount thirdparty;
        try
        {
            thirdparty = await Queries.ThirdpartyBackupAccountAsync(filesDbContext, TenantID);
        }
        catch (Exception e)
        {
            _logger.ErrorUpdateBackupProviderInfo(_securityContext.CurrentAccount.ID, e);
            throw;
        }

        if (!ProviderTypesExtensions.TryParse(providerKey, true, out var key))
        {
            throw new ArgumentException("Unrecognize ProviderType");
        }

        if (newAuthData != null && !newAuthData.IsEmpty())
        {
            if (!string.IsNullOrEmpty(newAuthData.Token))
            {
                newAuthData = GetEncodedAccesToken(newAuthData, key);
            }

            if (!await CheckProviderInfoAsync(ToProviderInfo(0, key, customerTitle, newAuthData, _securityContext.CurrentAccount.ID, FolderType.ThirdpartyBackup, _tenantUtil.DateTimeToUtc(_tenantUtil.DateTimeNow()))).ConfigureAwait(false))
            {
                throw new UnauthorizedAccessException(string.Format(FilesCommonResource.ErrorMassage_SecurityException_Auth, key));
            }
        }

        if (!string.IsNullOrEmpty(customerTitle))
        {
            thirdparty.Title = customerTitle;
        }

        thirdparty.UserId = _securityContext.CurrentAccount.ID;
        thirdparty.Provider = providerKey;

        if (!newAuthData.IsEmpty())
        {
            thirdparty.UserName = newAuthData.Login ?? "";
            thirdparty.Password = EncryptPassword(newAuthData.Password);
            thirdparty.Token = EncryptPassword(newAuthData.Token ?? "");
            thirdparty.Url = newAuthData.Url ?? "";
        }

        filesDbContext.Update(thirdparty);
        await filesDbContext.SaveChangesAsync();

        return thirdparty.Id;
    }

    public virtual async Task RemoveProviderInfoAsync(int linkId)
    {
        await using var filesDbContext = _dbContextFactory.CreateDbContext();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            using var tr = await filesDbContext.Database.BeginTransactionAsync();

            var folderId = (await GetProviderInfoAsync(linkId)).RootFolderId;
            var entryIDs = await Queries.HashIdsAsync(filesDbContext, TenantID, folderId).ToListAsync();

            await Queries.DeleteDbFilesSecuritiesAsync(filesDbContext, TenantID, entryIDs);
            await Queries.DeleteDbFilesTagLinksAsync(filesDbContext, TenantID, entryIDs);
            await Queries.DeleteThirdpartyAccountsByLinkIdAsync(filesDbContext, TenantID, linkId);

            await tr.CommitAsync();
        });
    }

    private IProviderInfo ToProviderInfo(int id, ProviderTypes providerKey, string customerTitle, AuthData authData, Guid owner, FolderType type, DateTime createOn)
    {
        var dbFilesThirdpartyAccount = new DbFilesThirdpartyAccount
        {
            Id = id,
            Title = customerTitle,
            Token = EncryptPassword(authData.Token),
            Url = authData.Url,
            UserName = authData.Login,
            Password = EncryptPassword(authData.Password),
            UserId = owner,
            FolderType = type,
            CreateOn = createOn,
            Provider = providerKey.ToString()
        };

        return ToProviderInfo(dbFilesThirdpartyAccount);
    }

    private IProviderInfo ToProviderInfo(DbFilesThirdpartyAccount input)
    {
        if (!ProviderTypesExtensions.TryParse(input.Provider, true, out var key))
        {
            return null;
        }

        var id = input.Id;
        var providerTitle = input.Title ?? string.Empty;
        var token = DecryptToken(input.Token, id);
        var owner = input.UserId;
        var rootFolderType = input.FolderType;
        var folderType = input.RoomType;
        var privateRoom = input.Private;
        var folderId = input.FolderId;
        var createOn = _tenantUtil.DateTimeFromUtc(input.CreateOn);
        var authData = new AuthData(input.Url, input.UserName, DecryptPassword(input.Password, id), token);
        var hasLogo = input.HasLogo;

        if (key == ProviderTypes.Box)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Token can't be null");
            }

            var box = _serviceProvider.GetService<BoxProviderInfo>();
            box.ProviderId = id;
            box.CustomerTitle = providerTitle;
            box.Owner = owner == Guid.Empty ? _securityContext.CurrentAccount.ID : owner;
            box.ProviderKey = input.Provider;
            box.RootFolderType = rootFolderType;
            box.CreateOn = createOn;
            box.Token = OAuth20Token.FromJson(token);
            box.FolderType = folderType;
            box.FolderId = folderId;
            box.Private = privateRoom;
            box.HasLogo = hasLogo;

            return box;
        }

        if (key == ProviderTypes.DropboxV2)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Token can't be null");
            }

            var drop = _serviceProvider.GetService<DropboxProviderInfo>();
            drop.ProviderId = id;
            drop.CustomerTitle = providerTitle;
            drop.Owner = owner == Guid.Empty ? _securityContext.CurrentAccount.ID : owner;
            drop.ProviderKey = input.Provider;
            drop.RootFolderType = rootFolderType;
            drop.CreateOn = createOn;
            drop.Token = OAuth20Token.FromJson(token);
            drop.FolderType = folderType;
            drop.FolderId = folderId;
            drop.Private = privateRoom;
            drop.HasLogo = hasLogo;

            return drop;
        }

        if (key == ProviderTypes.SharePoint)
        {
            if (!string.IsNullOrEmpty(authData.Login) && string.IsNullOrEmpty(authData.Password))
            {
                throw new ArgumentNullException("password", "Password can't be null");
            }

            var sh = _serviceProvider.GetService<SharePointProviderInfo>();
            sh.ProviderId = id;
            sh.CustomerTitle = providerTitle;
            sh.Owner = owner == Guid.Empty ? _securityContext.CurrentAccount.ID : owner;
            sh.ProviderKey = input.Provider;
            sh.RootFolderType = rootFolderType;
            sh.CreateOn = createOn;
            sh.InitClientContext(authData);
            sh.FolderType = folderType;
            sh.FolderId = folderId;
            sh.Private = privateRoom;
            sh.HasLogo = hasLogo;

            return sh;
        }

        if (key == ProviderTypes.GoogleDrive)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Token can't be null");
            }

            var gd = _serviceProvider.GetService<GoogleDriveProviderInfo>();
            gd.ProviderId = id;
            gd.CustomerTitle = providerTitle;
            gd.Owner = owner == Guid.Empty ? _securityContext.CurrentAccount.ID : owner;
            gd.ProviderKey = input.Provider;
            gd.RootFolderType = rootFolderType;
            gd.CreateOn = createOn;
            gd.Token = OAuth20Token.FromJson(token);
            gd.FolderType = folderType;
            gd.FolderId = folderId;
            gd.Private = privateRoom;
            gd.HasLogo = hasLogo;

            return gd;
        }

        if (key == ProviderTypes.OneDrive)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("Token can't be null");
            }

            var od = _serviceProvider.GetService<OneDriveProviderInfo>();
            od.ProviderId = id;
            od.CustomerTitle = providerTitle;
            od.Owner = owner == Guid.Empty ? _securityContext.CurrentAccount.ID : owner;
            od.ProviderKey = input.Provider;
            od.RootFolderType = rootFolderType;
            od.CreateOn = createOn;
            od.Token = OAuth20Token.FromJson(token);
            od.FolderType = folderType;
            od.FolderId = folderId;
            od.Private = privateRoom;
            od.HasLogo = hasLogo;

            return od;
        }

        if (string.IsNullOrEmpty(input.Provider))
        {
            throw new ArgumentNullException("providerKey");
        }

        if (string.IsNullOrEmpty(authData.Token) && string.IsNullOrEmpty(authData.Password))
        {
            throw new ArgumentNullException("token", "Both token and password can't be null");
        }

        if (!string.IsNullOrEmpty(authData.Login) && string.IsNullOrEmpty(authData.Password) && string.IsNullOrEmpty(authData.Token))
        {
            throw new ArgumentNullException("password", "Password can't be null");
        }

        var sharpBoxProviderInfo = _serviceProvider.GetService<SharpBoxProviderInfo>();
        sharpBoxProviderInfo.ProviderId = id;
        sharpBoxProviderInfo.CustomerTitle = providerTitle;
        sharpBoxProviderInfo.Owner = owner == Guid.Empty ? _securityContext.CurrentAccount.ID : owner;
        sharpBoxProviderInfo.ProviderKey = input.Provider;
        sharpBoxProviderInfo.RootFolderType = rootFolderType;
        sharpBoxProviderInfo.CreateOn = createOn;
        sharpBoxProviderInfo.AuthData = authData;
        sharpBoxProviderInfo.FolderType = folderType;
        sharpBoxProviderInfo.FolderId = folderId;
        sharpBoxProviderInfo.Private = privateRoom;
        sharpBoxProviderInfo.HasLogo = hasLogo;

        return sharpBoxProviderInfo;
    }

    private AuthData GetEncodedAccesToken(AuthData authData, ProviderTypes provider)
    {
        string code;
        OAuth20Token token;

        switch (provider)
        {
            case ProviderTypes.GoogleDrive:
                code = authData.Token;
                token = _oAuth20TokenHelper.GetAccessToken<GoogleLoginProvider>(_consumerFactory, code);

                if (token == null)
                {
                    throw new UnauthorizedAccessException(string.Format(FilesCommonResource.ErrorMassage_SecurityException_Auth, provider));
                }

                return new AuthData(token: token.ToJson());

            case ProviderTypes.Box:
                code = authData.Token;
                token = _oAuth20TokenHelper.GetAccessToken<BoxLoginProvider>(_consumerFactory, code);

                if (token == null)
                {
                    throw new UnauthorizedAccessException(string.Format(FilesCommonResource.ErrorMassage_SecurityException_Auth, provider));
                }

                return new AuthData(token: token.ToJson());

            case ProviderTypes.DropboxV2:
                code = authData.Token;
                token = _oAuth20TokenHelper.GetAccessToken<DropboxLoginProvider>(_consumerFactory, code);

                if (token == null)
                {
                    throw new UnauthorizedAccessException(string.Format(FilesCommonResource.ErrorMassage_SecurityException_Auth, provider));
                }

                return new AuthData(token: token.ToJson());

            case ProviderTypes.DropBox:

                var dropBoxRequestToken = DropBoxRequestToken.Parse(authData.Token);

                var config = CloudStorage.GetCloudConfigurationEasy(nSupportedCloudConfigurations.DropBox);
                var accessToken = DropBoxStorageProviderTools.ExchangeDropBoxRequestTokenIntoAccessToken(config as DropBoxConfiguration,
                                                                                                         _thirdpartyConfiguration.DropboxAppKey,
                                                                                                         _thirdpartyConfiguration.DropboxAppSecret,
                                                                                                         dropBoxRequestToken);

                var base64Token = new CloudStorage().SerializeSecurityTokenToBase64Ex(accessToken, config.GetType(), new Dictionary<string, string>());

                return new AuthData(token: base64Token);

            case ProviderTypes.OneDrive:
                code = authData.Token;
                token = _oAuth20TokenHelper.GetAccessToken<OneDriveLoginProvider>(_consumerFactory, code);

                if (token == null)
                {
                    throw new UnauthorizedAccessException(string.Format(FilesCommonResource.ErrorMassage_SecurityException_Auth, provider));
                }

                return new AuthData(token: token.ToJson());

            case ProviderTypes.SkyDrive:

                code = authData.Token;

                token = _oAuth20TokenHelper.GetAccessToken<OneDriveLoginProvider>(_consumerFactory, code);

                if (token == null)
                {
                    throw new UnauthorizedAccessException(string.Format(FilesCommonResource.ErrorMassage_SecurityException_Auth, provider));
                }

                accessToken = AppLimit.CloudComputing.SharpBox.Common.Net.oAuth20.OAuth20Token.FromJson(token.ToJson());

                if (accessToken == null)
                {
                    throw new UnauthorizedAccessException(string.Format(FilesCommonResource.ErrorMassage_SecurityException_Auth, provider));
                }

                config = CloudStorage.GetCloudConfigurationEasy(nSupportedCloudConfigurations.SkyDrive);
                var storage = new CloudStorage();
                base64Token = storage.SerializeSecurityTokenToBase64Ex(accessToken, config.GetType(), new Dictionary<string, string>());

                return new AuthData(token: base64Token);

            case ProviderTypes.SharePoint:
            case ProviderTypes.WebDav:
                break;

            default:
                authData.Url = null;
                break;
        }

        return authData;
    }

    private string EncryptPassword(string password)
    {
        return string.IsNullOrEmpty(password) ? string.Empty : _instanceCrypto.Encrypt(password);
    }

    private string DecryptPassword(string password, int id)
    {
        try
        {
            return string.IsNullOrEmpty(password) ? string.Empty : _instanceCrypto.Decrypt(password);
        }
        catch (Exception e)
        {
            _logger.ErrorDecryptPassword(id, _securityContext.CurrentAccount.ID, e);
            return null;
        }
    }

    private string DecryptToken(string token, int id)
    {
        try
        {
            return DecryptPassword(token, id);
        }
        catch
        {
            //old token in base64 without encrypt
            return token ?? "";
        }
    }
}

public static class ProviderAccountDaoExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<BoxProviderInfo>();
        services.TryAdd<DropboxProviderInfo>();
        services.TryAdd<SharePointProviderInfo>();
        services.TryAdd<GoogleDriveProviderInfo>();
        services.TryAdd<OneDriveProviderInfo>();
        services.TryAdd<SharpBoxProviderInfo>();
    }
}


static file class Queries
{
    public static readonly Func<FilesDbContext, int, Guid, IAsyncEnumerable<DbFilesThirdpartyAccount>>
        ThirdpartyAccountsAsync = EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, Guid userId) =>
                ctx.ThirdpartyAccount
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => r.UserId == userId));

    public static readonly
        Func<FilesDbContext, int, int, FolderType, Guid, string, IAsyncEnumerable<DbFilesThirdpartyAccount>>
        ThirdpartyAccountsByFilterAsync = EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, int linkId, FolderType folderType, Guid userId, string searchText) =>
                ctx.ThirdpartyAccount
                    
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => !(folderType == FolderType.USER || folderType == FolderType.DEFAULT && linkId == -1) ||
                                r.UserId == userId || r.FolderType == FolderType.ThirdpartyBackup)
                    .Where(r => linkId == -1 || r.Id == linkId)
                    .Where(r => folderType == FolderType.DEFAULT &&
                        !(r.FolderType == FolderType.ThirdpartyBackup && linkId == -1) || r.FolderType == folderType)
                    .Where(r => searchText == "" || r.Title.ToLower().Contains(searchText)));

    public static readonly Func<FilesDbContext, int, int, Task<DbFilesThirdpartyAccount>> ThirdpartyAccountAsync =
        EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, int linkId) =>
                ctx.ThirdpartyAccount
                    .Where(r => r.Id == linkId)
                    .Where(r => r.TenantId == tenantId)
                    .FirstOrDefault());

    public static readonly Func<FilesDbContext, int, int, string, string, string, string, Task<int>>
        UpdateThirdpartyAccountsAsync = EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, int linkId, string login, string password, string token, string url) =>
                ctx.ThirdpartyAccount
                    .Where(r => r.Id == linkId)
                    .Where(r => r.TenantId == tenantId)
                    .ExecuteUpdate(f => f
                        .SetProperty(p => p.UserName, login)
                        .SetProperty(p => p.Password, password)
                        .SetProperty(p => p.Token, token)
                        .SetProperty(p => p.Url, url)));

    public static readonly Func<FilesDbContext, int, int, Task<DbFilesThirdpartyAccount>>
        ThirdpartyAccountByLinkIdAsync = EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, int linkId) =>
                ctx.ThirdpartyAccount
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => r.Id == linkId)
                    .Single());

    public static readonly Func<FilesDbContext, int, int, IAsyncEnumerable<DbFilesThirdpartyAccount>>
        ThirdpartyAccountsByLinkIdAsync = EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, int linkId) =>
                ctx.ThirdpartyAccount
                    .AsTracking()
                    .Where(r => r.Id == linkId)
                    .Where(r => r.TenantId == tenantId));

    public static readonly Func<FilesDbContext, int, int, Task<int>>
        DeleteThirdpartyAccountsByLinkIdAsync = EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, int linkId) =>
                ctx.ThirdpartyAccount
                    .AsTracking()
                    .Where(r => r.Id == linkId)
                    .Where(r => r.TenantId == tenantId)
                    .ExecuteDelete());

    public static readonly Func<FilesDbContext, int, Task<DbFilesThirdpartyAccount>> ThirdpartyBackupAccountAsync =
        EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId) =>
                ctx.ThirdpartyAccount
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => r.FolderType == FolderType.ThirdpartyBackup)
                    .Single());

    public static readonly Func<FilesDbContext, int, string, IAsyncEnumerable<string>> HashIdsAsync =
        EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, string folderId) =>
                ctx.ThirdpartyIdMapping
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => r.Id.StartsWith(folderId))
                    .Select(r => r.HashId));

    public static readonly Func<FilesDbContext, int, IEnumerable<string>, Task<int>>
        DeleteDbFilesSecuritiesAsync = EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, IEnumerable<string> entryIDs) =>
                ctx.Security
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => entryIDs.Any(a => a == r.EntryId))
                    .ExecuteDelete());

    public static readonly Func<FilesDbContext, int, IEnumerable<string>, Task<int>>
        DeleteDbFilesTagLinksAsync = EF.CompileAsyncQuery(
            (FilesDbContext ctx, int tenantId, IEnumerable<string> entryIDs) =>
                ctx.TagLink
                    .Where(r => r.TenantId == tenantId)
                    .Where(r => entryIDs.Any(e => e == r.EntryId))
                    .ExecuteDelete());
}