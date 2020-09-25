/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


using System.Collections.Generic;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Common.Settings;
using ASC.Core.Users;
using ASC.MessagingSystem;
using ASC.Web.Api.Routing;
using ASC.Web.Core.PublicResources;
using ASC.Web.Files.Core.Entries;
using ASC.Web.Files.Services.WCFService;
using ASC.Web.Studio.Core;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ASC.Api.Documents
{
    [DefaultRoute]
    [ApiController]
    public class PrivacyRoomApi : ControllerBase
    {
        private AuthContext AuthContext { get; }
        private PermissionContext PermissionContext { get; }
        private SettingsManager SettingsManager { get; }
        private TenantManager TenantManager { get; }
        private EncryptionKeyPairHelper EncryptionKeyPairHelper { get; }
        private FileStorageService<int> FileStorageServiceInt { get; }
        private FileStorageService<string> FileStorageService { get; }
        private MessageService MessageService { get; }
        private ILog Log { get; }

        public PrivacyRoomApi(
            AuthContext authContext,
            PermissionContext permissionContext,
            SettingsManager settingsManager,
            TenantManager tenantManager,
            EncryptionKeyPairHelper encryptionKeyPairHelper,
            FileStorageService<int> fileStorageServiceInt,
            FileStorageService<string> fileStorageService,
            MessageService messageService,
            IOptionsMonitor<ILog> option)
        {
            AuthContext = authContext;
            PermissionContext = permissionContext;
            SettingsManager = settingsManager;
            TenantManager = tenantManager;
            EncryptionKeyPairHelper = encryptionKeyPairHelper;
            FileStorageServiceInt = fileStorageServiceInt;
            FileStorageService = fileStorageService;
            MessageService = messageService;
            Log = option.Get("ASC.Api.Documents");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <visible>false</visible>
        [Update("keys")]
        public object SetKeys(string publicKey, string privateKeyEnc)
        {
            PermissionContext.DemandPermissions(new UserSecurityProvider(AuthContext.CurrentAccount.ID), Constants.Action_EditUser);

            if (!PrivacyRoomSettings.GetEnabled(SettingsManager)) throw new System.Security.SecurityException();

            var keyPair = EncryptionKeyPairHelper.GetKeyPair();
            if (keyPair != null)
            {
                if (!string.IsNullOrEmpty(keyPair.PublicKey))
                {
                    return new { isset = true };
                }

                Log.InfoFormat("User {0} updates address", AuthContext.CurrentAccount.ID);
            }

            EncryptionKeyPairHelper.SetKeyPair(publicKey, privateKeyEnc);

            return new
            {
                isset = true
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <visible>false</visible>
        [Read("access/{fileId}")]
        public IEnumerable<EncryptionKeyPair> GetPublicKeysWithAccess(string fileId)
        {
            if (!PrivacyRoomSettings.GetEnabled(SettingsManager)) throw new System.Security.SecurityException();

            return EncryptionKeyPairHelper.GetKeyPair(fileId, FileStorageService);
        }

        [Read("access/{fileId:int}")]
        public IEnumerable<EncryptionKeyPair> GetPublicKeysWithAccess(int fileId)
        {
            if (!PrivacyRoomSettings.GetEnabled(SettingsManager)) throw new System.Security.SecurityException();

            return EncryptionKeyPairHelper.GetKeyPair(fileId, FileStorageServiceInt);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <visible>false</visible>
        [Read("")]
        public bool PrivacyRoom()
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            return PrivacyRoomSettings.GetEnabled(SettingsManager);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enable"></param>
        /// <returns></returns>
        /// <visible>false</visible>
        [Update("")]
        public bool SetPrivacyRoom(bool enable)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            if (enable)
            {
                if (!PrivacyRoomSettings.IsAvailable(TenantManager))
                {
                    throw new BillingException(Resource.ErrorNotAllowedOption, "PrivacyRoom");
                }
            }

            PrivacyRoomSettings.SetEnabled(TenantManager, SettingsManager, enable);

            MessageService.Send(enable ? MessageAction.PrivacyRoomEnable : MessageAction.PrivacyRoomDisable);

            return enable;
        }
    }

    public static class PrivacyRoomApiExtention
    {
        public static DIHelper AddPrivacyRoomApiService(this DIHelper services)
        {
            if (services.TryAddScoped<PrivacyRoomApi>())
            {
                services
                    .AddAuthContextService()
                    .AddPermissionContextService()
                    .AddSettingsManagerService()
                    .AddTenantManagerService()
                    .AddMessageServiceService()
                    .AddEncryptionKeyPairHelperService();
            }

            return services;
        }
    }
}