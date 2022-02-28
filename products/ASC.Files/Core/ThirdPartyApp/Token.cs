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
using System.Diagnostics;
using System.Linq;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.FederatedLogin;
using ASC.FederatedLogin.Helpers;
using ASC.Files.Core;
using ASC.Files.Core.EF;
using ASC.Security.Cryptography;

using Microsoft.Extensions.Options;

namespace ASC.Web.Files.ThirdPartyApp
{
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
        private Lazy<FilesDbContext> LazyFilesDbContext { get; }
        private FilesDbContext FilesDbContext { get => LazyFilesDbContext.Value; }
        private InstanceCrypto InstanceCrypto { get; }
        private AuthContext AuthContext { get; }
        private TenantManager TenantManager { get; }

        public TokenHelper(
            DbContextManager<FilesDbContext> dbContextManager,
            IOptionsMonitor<ILog> option,
            InstanceCrypto instanceCrypto,
            AuthContext authContext,
            TenantManager tenantManager)
        {
            Logger = option.CurrentValue;
            LazyFilesDbContext = new Lazy<FilesDbContext>(() => dbContextManager.Get(FileConstant.DatabaseId));
            InstanceCrypto = instanceCrypto;
            AuthContext = authContext;
            TenantManager = tenantManager;
        }

        public void SaveToken(Token token)
        {
            var dbFilesThirdpartyApp = new DbFilesThirdpartyApp
            {
                App = token.App,
                Token = EncryptToken(token),
                UserId = AuthContext.CurrentAccount.ID,
                TenantId = TenantManager.GetCurrentTenant().TenantId
            };

            FilesDbContext.AddOrUpdate(r => r.ThirdpartyApp, dbFilesThirdpartyApp);
            FilesDbContext.SaveChanges();
        }

        public Token GetToken(string app)
        {
            return GetToken(app, AuthContext.CurrentAccount.ID);
        }

        public Token GetToken(string app, Guid userId)
        {
            var oAuth20Token = FilesDbContext.ThirdpartyApp
                .AsQueryable()
                .Where(r => r.TenantId == TenantManager.GetCurrentTenant().TenantId)
                .Where(r => r.UserId == userId)
                .Where(r => r.App == app)
                .Select(r => r.Token)
                .FirstOrDefault();

            if (oAuth20Token == null) return null;

            return new Token(DecryptToken(oAuth20Token), app);
        }

        public void DeleteToken(string app, Guid? userId = null)
        {
            var apps = FilesDbContext.ThirdpartyApp
                .AsQueryable()
                .Where(r => r.TenantId == TenantManager.GetCurrentTenant().TenantId)
                .Where(r => r.UserId == (userId ?? AuthContext.CurrentAccount.ID))
                .Where(r => r.App == app);

            FilesDbContext.RemoveRange(apps);
            FilesDbContext.SaveChanges();
        }

        private string EncryptToken(OAuth20Token token)
        {
            var t = token.ToJson();
            return string.IsNullOrEmpty(t) ? string.Empty : InstanceCrypto.Encrypt(t);
        }

        private OAuth20Token DecryptToken(string token)
        {
            return string.IsNullOrEmpty(token) ? null : OAuth20Token.FromJson(InstanceCrypto.Decrypt(token));
        }
    }
}