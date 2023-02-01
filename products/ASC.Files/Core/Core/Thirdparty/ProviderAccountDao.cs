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
    private int _tenantID;
    protected int TenantID
    {
        get
        {
            if (_tenantID == 0)
            {
                _tenantID = _tenantManager.GetCurrentTenant().Id;
            }

            return _tenantID;
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
            var thirdpartyAccounts = filesDbContext.ThirdpartyAccount
                .Where(r => r.TenantId == TenantID)
                .Where(r => r.UserId == userId)
                .AsAsyncEnumerable();

            return thirdpartyAccounts.Select(ToProviderInfo);
        }
        catch (Exception e)
        {
            _logger.ErrorGetProvidersInfoInternalUser(userId, e);

            return new List<IProviderInfo>().ToAsyncEnumerable();
        }
    }

    static readonly Func<FilesDbContext, int, int, FolderType, Guid, string, IAsyncEnumerable<DbFilesThirdpartyAccount>> _getProvidersInfoQuery =
        EF.CompileAsyncQuery((FilesDbContext ctx, int tenantId, int linkId, FolderType folderType, Guid userId, string searchText) =>
        ctx.ThirdpartyAccount
        .AsNoTracking()
        .Where(r => r.TenantId == tenantId)
        .Where(r => !(folderType == FolderType.USER || folderType == FolderType.DEFAULT && linkId == -1) || r.UserId == userId || r.FolderType == FolderType.ThirdpartyBackup)
        .Where(r => linkId == -1 || r.Id == linkId)
        .Where(r => folderType == FolderType.DEFAULT && !(r.FolderType == FolderType.ThirdpartyBackup && linkId == -1) || r.FolderType == folderType)
        .Where(r => searchText == "" || r.Title.ToLower().Contains(searchText)));

    private IAsyncEnumerable<IProviderInfo> GetProvidersInfoInternalAsync(int linkId = -1, FolderType folderType = FolderType.DEFAULT, string searchText = null)
    {
        try
        {
            var filesDbContext = _dbContextFactory.CreateDbContext();
            return _getProvidersInfoQuery(filesDbContext, TenantID, linkId, folderType, _securityContext.CurrentAccount.ID, GetSearchText(searchText))
                .Select(ToProviderInfo);
        }
        catch (Exception e)
        {
            _logger.ErrorGetProvidersInfoInternal(linkId, folderType, _securityContext.CurrentAccount.ID, e);
            return new List<IProviderInfo>().ToAsyncEnumerable();
        }
    }

    public virtual Task<int> SaveProviderInfoAsync(string providerKey, string customerTitle, AuthData authData, FolderType folderType)
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

        return InternalSaveProviderInfoAsync(providerKey, customerTitle, authData, folderType, prKey);
    }

    private async Task<int> InternalSaveProviderInfoAsync(string providerKey, string customerTitle, AuthData authData, FolderType folderType, ProviderTypes prKey)
    {
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

        using var filesDbContext = _dbContextFactory.CreateDbContext();
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
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        var forUpdate = await filesDbContext.ThirdpartyAccount
            .Where(r => r.Id == linkId)
            .Where(r => r.TenantId == TenantID)
            .FirstOrDefaultAsync();

        if (forUpdate == null)
        {
            return false;
        }

        forUpdate.FolderType = rootFolderType;

        await filesDbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateProviderInfoAsync(int linkId, bool hasLogo)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var forUpdate = await filesDbContext.ThirdpartyAccount
            .Where(r => r.Id == linkId)
            .Where(r => r.TenantId == TenantID)
            .FirstOrDefaultAsync();

        if (forUpdate == null)
        {
            return false;
        }

        forUpdate.HasLogo = hasLogo;

        await filesDbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateProviderInfoAsync(int linkId, string title, string folderId, FolderType roomType, bool @private)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var forUpdate = await filesDbContext.ThirdpartyAccount
            .Where(r => r.Id == linkId)
            .Where(r => r.TenantId == TenantID)
            .FirstOrDefaultAsync();

        if (forUpdate == null)
        {
            return false;
        }

        forUpdate.RoomType = roomType;
        forUpdate.FolderId = folderId;
        forUpdate.FolderType = FolderType.VirtualRooms;
        forUpdate.Private = @private;
        forUpdate.Title = title;

        await filesDbContext.SaveChangesAsync();

        return true;
    }

    public virtual async Task<int> UpdateProviderInfoAsync(int linkId, AuthData authData)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        var forUpdate = await filesDbContext.ThirdpartyAccount
            .Where(r => r.Id == linkId)
            .Where(r => r.TenantId == TenantID)
            .ToListAsync();

        foreach (var f in forUpdate)
        {
            f.UserName = authData.Login ?? "";
            f.Password = EncryptPassword(authData.Password);
            f.Token = EncryptPassword(authData.Token ?? "");
            f.Url = authData.Url ?? "";
        }

        await filesDbContext.SaveChangesAsync();

        return forUpdate.Count == 1 ? linkId : default;
    }

    public virtual async Task<int> UpdateProviderInfoAsync(int linkId, string customerTitle, AuthData newAuthData, FolderType folderType, Guid? userId = null)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();

        var authData = new AuthData();
        if (newAuthData != null && !newAuthData.IsEmpty())
        {
            var querySelect =
                filesDbContext.ThirdpartyAccount
                .Where(r => r.TenantId == TenantID)
                .Where(r => r.Id == linkId);

            DbFilesThirdpartyAccount input;
            try
            {
                input = await querySelect.SingleAsync();
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

        var toUpdate = await filesDbContext.ThirdpartyAccount
            .Where(r => r.Id == linkId)
            .Where(r => r.TenantId == TenantID)
            .ToListAsync();

        foreach (var t in toUpdate)
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
        }

        await filesDbContext.SaveChangesAsync();

        return toUpdate.Count == 1 ? linkId : default;
    }

    public virtual async Task<int> UpdateBackupProviderInfoAsync(string providerKey, string customerTitle, AuthData newAuthData)
    {
        using var filesDbContext = await _dbContextFactory.CreateDbContextAsync();

        var querySelect =
                filesDbContext.ThirdpartyAccount
                .AsQueryable()
                .Where(r => r.TenantId == TenantID)
                .Where(r => r.FolderType == FolderType.ThirdpartyBackup);

        DbFilesThirdpartyAccount thirdparty;
        try
        {
            thirdparty = await querySelect.SingleAsync().ConfigureAwait(false);
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

        await filesDbContext.SaveChangesAsync().ConfigureAwait(false);

        return thirdparty.Id;
    }

    public virtual async Task RemoveProviderInfoAsync(int linkId)
    {
        using var filesDbContext = _dbContextFactory.CreateDbContext();
        var strategy = filesDbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            using var filesDbContext = _dbContextFactory.CreateDbContext();
            using var tx = await filesDbContext.Database.BeginTransactionAsync();
            var folderId = (await GetProviderInfoAsync(linkId)).RootFolderId;

            var entryIDs = await filesDbContext.ThirdpartyIdMapping
            .Where(r => r.TenantId == TenantID)
            .Where(r => r.Id.StartsWith(folderId))
            .Select(r => r.HashId)
            .ToListAsync();

            var forDelete = await filesDbContext.Security
            .Where(r => r.TenantId == TenantID)
            .Where(r => entryIDs.Any(a => a == r.EntryId))
            .ToListAsync();

            filesDbContext.Security.RemoveRange(forDelete);
            await filesDbContext.SaveChangesAsync();

            var linksForDelete = await filesDbContext.TagLink
            .Where(r => r.TenantId == TenantID)
            .Where(r => entryIDs.Any(e => e == r.EntryId))
            .ToListAsync();

            filesDbContext.TagLink.RemoveRange(linksForDelete);
            await filesDbContext.SaveChangesAsync();

            var accountsForDelete = await filesDbContext.ThirdpartyAccount
            .Where(r => r.Id == linkId)
            .Where(r => r.TenantId == TenantID)
            .ToListAsync();

            filesDbContext.ThirdpartyAccount.RemoveRange(accountsForDelete);
            await filesDbContext.SaveChangesAsync();

            await tx.CommitAsync();
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
            box.ID = id;
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
            drop.ID = id;
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
            sh.ID = id;
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
            gd.ID = id;
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
            od.ID = id;
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
        sharpBoxProviderInfo.ID = id;
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
