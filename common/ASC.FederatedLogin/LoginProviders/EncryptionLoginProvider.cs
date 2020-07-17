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
using System.Linq;
using System.Security;

using ASC.Common;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Users;
using ASC.FederatedLogin;
using ASC.FederatedLogin.Profile;
using ASC.Security.Cryptography;

using Microsoft.Extensions.Options;

using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Web.Studio.Core
{
    public class EncryptionLoginProvider
    {
        public UserManager UserManager { get; }
        public TenantManager TenantManager { get; }
        public SecurityContext SecurityContext { get; }
        public Signature Signature { get; }
        public InstanceCrypto InstanceCrypto { get; }
        public IOptionsSnapshot<AccountLinker> Snapshot { get; }

        public EncryptionLoginProvider(
            UserManager userManager,
            TenantManager tenantManager,
            SecurityContext securityContext,
            Signature signature,
            InstanceCrypto instanceCrypto,
            IOptionsSnapshot<AccountLinker> snapshot)
        {
            UserManager = userManager;
            TenantManager = tenantManager;
            SecurityContext = securityContext;
            Signature = signature;
            InstanceCrypto = instanceCrypto;
            Snapshot = snapshot;
        }

        public void UpdateAddress(string account)
        {
            var tenant = TenantManager.GetCurrentTenant();
            var user = UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
            if (!SecurityContext.IsAuthenticated || user.IsVisitor(UserManager)) throw new SecurityException();

            var loginProfile = new LoginProfile(Signature, InstanceCrypto)
            {
                Provider = ProviderConstants.Encryption,
            };

            var linker = Snapshot.Get("webstudio");
            if (string.IsNullOrEmpty(account))
            {
                linker.RemoveLink(user.ID.ToString(), loginProfile);
            }
            else
            {
                loginProfile.Name = Crypto.GetV(account, 1, true);
                linker.AddLink(user.ID.ToString(), loginProfile);
            }
        }


        public string GetAddress()
        {
            return GetAddress(SecurityContext.CurrentAccount.ID);
        }

        public string GetAddress(Guid userId)
        {
            var linker = Snapshot.Get("webstudio");
            var profile = linker.GetLinkedProfiles(userId.ToString(), ProviderConstants.Encryption).FirstOrDefault();
            if (profile == null) return null;

            var account = Crypto.GetV(profile.Name, 1, false);
            return account;
        }
    }
    public static class EncryptionLoginProviderExtension
    {
        public static DIHelper AddEncryptionLoginProviderService(this DIHelper services)
        {
            if (services.TryAddScoped<EncryptionLoginProvider>())
            {
                return services
                    .AddUserManagerService()
                    .AddTenantManagerService()
                    .AddSecurityContextService()
                    .AddSignatureService()
                    .AddInstanceCryptoService()
                    .AddAccountLinker();
            }

            return services;
        }
    }
}