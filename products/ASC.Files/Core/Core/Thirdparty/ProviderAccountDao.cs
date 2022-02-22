/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.Linq;

using AppLimit.CloudComputing.SharpBox;
using AppLimit.CloudComputing.SharpBox.StorageProvider.DropBox;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Configuration;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;
using ASC.FederatedLogin;
using ASC.FederatedLogin.Helpers;
using ASC.FederatedLogin.LoginProviders;
using ASC.Files.Core;
using ASC.Files.Core.EF;
using ASC.Files.Core.Resources;
using ASC.Files.Thirdparty.Box;
using ASC.Files.Thirdparty.Dropbox;
using ASC.Files.Thirdparty.GoogleDrive;
using ASC.Files.Thirdparty.OneDrive;
using ASC.Files.Thirdparty.SharePoint;
using ASC.Files.Thirdparty.Sharpbox;
using ASC.Security.Cryptography;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Helpers;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using static ASC.Files.Core.Data.AbstractDao;

namespace ASC.Files.Thirdparty
{
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
        private int tenantID;
        protected int TenantID 
        { 
            get 
            {
                if (tenantID == 0) tenantID = TenantManager.GetCurrentTenant().TenantId;
                return tenantID; 
            } 
        }
        private Lazy<FilesDbContext> LazyFilesDbContext { get; }
        private FilesDbContext FilesDbContext { get => LazyFilesDbContext.Value; }
        public ILog Logger { get; }
        private IServiceProvider ServiceProvider { get; }
        private TenantUtil TenantUtil { get; }
        private TenantManager TenantManager { get; }
        private InstanceCrypto InstanceCrypto { get; }
        private SecurityContext SecurityContext { get; }
        private ConsumerFactory ConsumerFactory { get; }
        private ThirdpartyConfiguration ThirdpartyConfiguration { get; }

        public ProviderAccountDao(
            IServiceProvider serviceProvider,
            TenantUtil tenantUtil,
            TenantManager tenantManager,
            InstanceCrypto instanceCrypto,
            SecurityContext securityContext,
            ConsumerFactory consumerFactory,
            ThirdpartyConfiguration thirdpartyConfiguration,
            DbContextManager<FilesDbContext> dbContextManager,
            IOptionsMonitor<ILog> options)
        {
            LazyFilesDbContext = new Lazy<FilesDbContext>(() => dbContextManager.Get(FileConstant.DatabaseId));
            Logger = options.Get("ASC.Files");
            ServiceProvider = serviceProvider;
            TenantUtil = tenantUtil;
            TenantManager = tenantManager;
            InstanceCrypto = instanceCrypto;
            SecurityContext = securityContext;
            ConsumerFactory = consumerFactory;
            ThirdpartyConfiguration = thirdpartyConfiguration;
        }

        public virtual IProviderInfo GetProviderInfo(int linkId)
        {
            return GetProvidersInfoInternal(linkId).Single();
        }

        public virtual List<IProviderInfo> GetProvidersInfo()
        {
            return GetProvidersInfoInternal();
        }

        public virtual List<IProviderInfo> GetProvidersInfo(FolderType folderType, string searchText = null)
        {
            return GetProvidersInfoInternal(folderType: folderType, searchText: searchText);
        }

        public virtual List<IProviderInfo> GetProvidersInfo(Guid userId)
        {
            try
            {
                return FilesDbContext.ThirdpartyAccount
                    .Where(r => r.TenantId == TenantID)
                    .Where(r => r.UserId == userId)
                    .AsEnumerable()
                    .Select(ToProviderInfo)
                    .ToList();
            }
            catch (Exception e)
            {
                Logger.Error(string.Format("GetProvidersInfoInternal: user = {0}", userId), e);
                return new List<IProviderInfo>();
            }
        }

        static Func<FilesDbContext, int, int, FolderType, Guid, string, IEnumerable<DbFilesThirdpartyAccount>> getProvidersInfoQuery =
    EF.CompileQuery((FilesDbContext ctx, int tenantId, int linkId, FolderType folderType, Guid userId, string searchText) =>
    ctx.ThirdpartyAccount
    .AsNoTracking()
    .Where(r => r.TenantId == tenantId)
    .Where(r => !(folderType == FolderType.USER || folderType == FolderType.DEFAULT && linkId == -1) || r.UserId == userId)
    .Where(r => linkId == -1 || r.Id == linkId)
    .Where(r => folderType == FolderType.DEFAULT || r.FolderType == folderType)
    .Where(r => searchText == "" || r.Title.ToLower().Contains(searchText))
    );

        private List<IProviderInfo> GetProvidersInfoInternal(int linkId = -1, FolderType folderType = FolderType.DEFAULT, string searchText = null)
        {
            try
            {
                return getProvidersInfoQuery(FilesDbContext, TenantID, linkId, folderType, SecurityContext.CurrentAccount.ID, GetSearchText(searchText))
                    .AsEnumerable()
                    .Select(ToProviderInfo)
                    .ToList();
            }
            catch (Exception e)
            {
                Logger.Error(string.Format("GetProvidersInfoInternal: linkId = {0} , folderType = {1} , user = {2}",
                                                  linkId, folderType, SecurityContext.CurrentAccount.ID), e);
                return new List<IProviderInfo>();
            }
        }

        public virtual int SaveProviderInfo(string providerKey, string customerTitle, AuthData authData, FolderType folderType)
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

            if (!CheckProviderInfo(ToProviderInfo(0, prKey, customerTitle, authData, SecurityContext.CurrentAccount.ID, folderType, TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow()))))
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
                CreateOn = TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow()),
                UserId = SecurityContext.CurrentAccount.ID,
                Token = EncryptPassword(authData.Token ?? ""),
                Url = authData.Url ?? ""
            };

            var res = FilesDbContext.AddOrUpdate(r => r.ThirdpartyAccount, dbFilesThirdpartyAccount);
            FilesDbContext.SaveChanges();
            return res.Id;
        }

        public bool CheckProviderInfo(IProviderInfo providerInfo)
        {
            return providerInfo != null && providerInfo.CheckAccess();
        }

        public virtual int UpdateProviderInfo(int linkId, AuthData authData)
        {
            var forUpdate = FilesDbContext.ThirdpartyAccount
                .Where(r => r.Id == linkId)
                .Where(r => r.TenantId == TenantID)
                .ToList();

            foreach (var f in forUpdate)
            {
                f.UserName = authData.Login ?? "";
                f.Password = EncryptPassword(authData.Password);
                f.Token = EncryptPassword(authData.Token ?? "");
                f.Url = authData.Url ?? "";
            }

            FilesDbContext.SaveChanges();

            return forUpdate.Count == 1 ? linkId : default;
        }

        public virtual int UpdateProviderInfo(int linkId, string customerTitle, AuthData newAuthData, FolderType folderType, Guid? userId = null)
        {
            var authData = new AuthData();
            if (newAuthData != null && !newAuthData.IsEmpty())
            {
                var querySelect =
                    FilesDbContext.ThirdpartyAccount
                    .Where(r => r.TenantId == TenantID)
                    .Where(r => r.Id == linkId);

                DbFilesThirdpartyAccount input;
                try
                {
                    input = querySelect.Single();
                }
                catch (Exception e)
                {
                    Logger.Error(string.Format("UpdateProviderInfo: linkId = {0} , user = {1}", linkId, SecurityContext.CurrentAccount.ID), e);
                    throw;
                }

                if (!Enum.TryParse(input.Provider, true, out ProviderTypes key))
                {
                    throw new ArgumentException("Unrecognize ProviderType");
                }

                authData = new AuthData(
                    !string.IsNullOrEmpty(newAuthData.Url) ? newAuthData.Url : input.Url,
                    input.UserName,
                    !string.IsNullOrEmpty(newAuthData.Password) ? newAuthData.Password : DecryptPassword(input.Password),
                    newAuthData.Token);

                if (!string.IsNullOrEmpty(newAuthData.Token))
                {
                    authData = GetEncodedAccesToken(authData, key);
                }

                if (!CheckProviderInfo(ToProviderInfo(0, key, customerTitle, authData, SecurityContext.CurrentAccount.ID, folderType, TenantUtil.DateTimeToUtc(TenantUtil.DateTimeNow()))))
                    throw new UnauthorizedAccessException(string.Format(FilesCommonResource.ErrorMassage_SecurityException_Auth, key));
            }

            var toUpdate = FilesDbContext.ThirdpartyAccount
                .Where(r => r.Id == linkId)
                .Where(r => r.TenantId == TenantID)
                .ToList();

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

            FilesDbContext.SaveChanges();
            return toUpdate.Count == 1 ? linkId : default;
        }

        public virtual void RemoveProviderInfo(int linkId)
        {
            using var tx = FilesDbContext.Database.BeginTransaction();
            var folderId = GetProviderInfo(linkId).RootFolderId;

            var entryIDs = FilesDbContext.ThirdpartyIdMapping
                .Where(r => r.TenantId == TenantID)
                .Where(r => r.Id.StartsWith(folderId))
                .Select(r => r.HashId)
                .ToList();

            var forDelete = FilesDbContext.Security
                .Where(r => r.TenantId == TenantID)
                .Where(r => entryIDs.Any(a => a == r.EntryId))
                .ToList();

            FilesDbContext.Security.RemoveRange(forDelete);
            FilesDbContext.SaveChanges();

            var linksForDelete = FilesDbContext.TagLink
                .Where(r => r.TenantId == TenantID)
                .Where(r => entryIDs.Any(e => e == r.EntryId))
                .ToList();

            FilesDbContext.TagLink.RemoveRange(linksForDelete);
            FilesDbContext.SaveChanges();

            var accountsForDelete = FilesDbContext.ThirdpartyAccount
                .Where(r => r.Id == linkId)
                .Where(r => r.TenantId == TenantID)
                .ToList();

            FilesDbContext.ThirdpartyAccount.RemoveRange(accountsForDelete);
            FilesDbContext.SaveChanges();

            tx.Commit();
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
            if (!Enum.TryParse(input.Provider, true, out ProviderTypes key)) return null;

            var id = input.Id;
            var providerTitle = input.Title ?? string.Empty;
            var token = DecryptToken(input.Token);
            var owner = input.UserId;
            var folderType = input.FolderType;
            var createOn = TenantUtil.DateTimeFromUtc(input.CreateOn);
            var authData = new AuthData(input.Url, input.UserName, DecryptPassword(input.Password), token);

            if (key == ProviderTypes.Box)
            {
                if (string.IsNullOrEmpty(token))
                    throw new ArgumentException("Token can't be null");

                var box = ServiceProvider.GetService<BoxProviderInfo>();
                box.ID = id;
                box.CustomerTitle = providerTitle;
                box.Owner = owner == Guid.Empty ? SecurityContext.CurrentAccount.ID : owner;
                box.ProviderKey = input.Provider;
                box.RootFolderType = folderType;
                box.CreateOn = createOn;
                box.Token = OAuth20Token.FromJson(token);
                return box;
            }

            if (key == ProviderTypes.DropboxV2)
            {
                if (string.IsNullOrEmpty(token))
                    throw new ArgumentException("Token can't be null");

                var drop = ServiceProvider.GetService<DropboxProviderInfo>();
                drop.ID = id;
                drop.CustomerTitle = providerTitle;
                drop.Owner = owner == Guid.Empty ? SecurityContext.CurrentAccount.ID : owner;
                drop.ProviderKey = input.Provider;
                drop.RootFolderType = folderType;
                drop.CreateOn = createOn;
                drop.Token = OAuth20Token.FromJson(token);

                return drop;
            }

            if (key == ProviderTypes.SharePoint)
            {
                if (!string.IsNullOrEmpty(authData.Login) && string.IsNullOrEmpty(authData.Password))
                    throw new ArgumentNullException("password", "Password can't be null");

                var sh = ServiceProvider.GetService<SharePointProviderInfo>();
                sh.ID = id;
                sh.CustomerTitle = providerTitle;
                sh.Owner = owner == Guid.Empty ? SecurityContext.CurrentAccount.ID : owner;
                sh.ProviderKey = input.Provider;
                sh.RootFolderType = folderType;
                sh.CreateOn = createOn;
                sh.InitClientContext(authData);
                return sh;
            }

            if (key == ProviderTypes.GoogleDrive)
            {
                if (string.IsNullOrEmpty(token))
                    throw new ArgumentException("Token can't be null");

                var gd = ServiceProvider.GetService<GoogleDriveProviderInfo>();
                gd.ID = id;
                gd.CustomerTitle = providerTitle;
                gd.Owner = owner == Guid.Empty ? SecurityContext.CurrentAccount.ID : owner;
                gd.ProviderKey = input.Provider;
                gd.RootFolderType = folderType;
                gd.CreateOn = createOn;
                gd.Token = OAuth20Token.FromJson(token);

                return gd;
            }

            if (key == ProviderTypes.OneDrive)
            {
                if (string.IsNullOrEmpty(token))
                    throw new ArgumentException("Token can't be null");

                var od = ServiceProvider.GetService<OneDriveProviderInfo>();
                od.ID = id;
                od.CustomerTitle = providerTitle;
                od.Owner = owner == Guid.Empty ? SecurityContext.CurrentAccount.ID : owner;
                od.ProviderKey = input.Provider;
                od.RootFolderType = folderType;
                od.CreateOn = createOn;
                od.Token = OAuth20Token.FromJson(token);

                return od;
            }

            if (string.IsNullOrEmpty(input.Provider))
                throw new ArgumentNullException("providerKey");
            if (string.IsNullOrEmpty(authData.Token) && string.IsNullOrEmpty(authData.Password))
                throw new ArgumentNullException("token", "Both token and password can't be null");
            if (!string.IsNullOrEmpty(authData.Login) && string.IsNullOrEmpty(authData.Password) && string.IsNullOrEmpty(authData.Token))
                throw new ArgumentNullException("password", "Password can't be null");

            var sharpBoxProviderInfo = ServiceProvider.GetService<SharpBoxProviderInfo>();
            sharpBoxProviderInfo.ID = id;
            sharpBoxProviderInfo.CustomerTitle = providerTitle;
            sharpBoxProviderInfo.Owner = owner == Guid.Empty ? SecurityContext.CurrentAccount.ID : owner;
            sharpBoxProviderInfo.ProviderKey = input.Provider;
            sharpBoxProviderInfo.RootFolderType = folderType;
            sharpBoxProviderInfo.CreateOn = createOn;
            sharpBoxProviderInfo.AuthData = authData;

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
                    token = OAuth20TokenHelper.GetAccessToken<GoogleLoginProvider>(ConsumerFactory, code);

                    if (token == null) throw new UnauthorizedAccessException(string.Format(FilesCommonResource.ErrorMassage_SecurityException_Auth, provider));

                    return new AuthData(token: token.ToJson());

                case ProviderTypes.Box:
                    code = authData.Token;
                    token = OAuth20TokenHelper.GetAccessToken<BoxLoginProvider>(ConsumerFactory, code);

                    if (token == null) throw new UnauthorizedAccessException(string.Format(FilesCommonResource.ErrorMassage_SecurityException_Auth, provider));

                    return new AuthData(token: token.ToJson());

                case ProviderTypes.DropboxV2:
                    code = authData.Token;
                    token = OAuth20TokenHelper.GetAccessToken<DropboxLoginProvider>(ConsumerFactory, code);

                    if (token == null) throw new UnauthorizedAccessException(string.Format(FilesCommonResource.ErrorMassage_SecurityException_Auth, provider));

                    return new AuthData(token: token.ToJson());

                case ProviderTypes.DropBox:

                    var dropBoxRequestToken = DropBoxRequestToken.Parse(authData.Token);

                    var config = CloudStorage.GetCloudConfigurationEasy(nSupportedCloudConfigurations.DropBox);
                    var accessToken = DropBoxStorageProviderTools.ExchangeDropBoxRequestTokenIntoAccessToken(config as DropBoxConfiguration,
                                                                                                             ThirdpartyConfiguration.DropboxAppKey,
                                                                                                             ThirdpartyConfiguration.DropboxAppSecret,
                                                                                                             dropBoxRequestToken);

                    var base64Token = new CloudStorage().SerializeSecurityTokenToBase64Ex(accessToken, config.GetType(), new Dictionary<string, string>());

                    return new AuthData(token: base64Token);

                case ProviderTypes.OneDrive:
                    code = authData.Token;
                    token = OAuth20TokenHelper.GetAccessToken<OneDriveLoginProvider>(ConsumerFactory, code);

                    if (token == null) throw new UnauthorizedAccessException(string.Format(FilesCommonResource.ErrorMassage_SecurityException_Auth, provider));

                    return new AuthData(token: token.ToJson());

                case ProviderTypes.SkyDrive:

                    code = authData.Token;

                    token = OAuth20TokenHelper.GetAccessToken<OneDriveLoginProvider>(ConsumerFactory, code);

                    if (token == null) throw new UnauthorizedAccessException(string.Format(FilesCommonResource.ErrorMassage_SecurityException_Auth, provider));

                    accessToken = AppLimit.CloudComputing.SharpBox.Common.Net.oAuth20.OAuth20Token.FromJson(token.ToJson());

                    if (accessToken == null) throw new UnauthorizedAccessException(string.Format(FilesCommonResource.ErrorMassage_SecurityException_Auth, provider));

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
            return string.IsNullOrEmpty(password) ? string.Empty : InstanceCrypto.Encrypt(password);
        }

        private string DecryptPassword(string password)
        {
            return string.IsNullOrEmpty(password) ? string.Empty : InstanceCrypto.Decrypt(password);
        }

        private string DecryptToken(string token)
        {
            try
            {
                return DecryptPassword(token);
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
}