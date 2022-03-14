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


using System.Collections.Generic;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Common.Settings;
using ASC.Core.Users;
using ASC.Files.Core.Model;
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
    [Scope]
    [DefaultRoute]
    [ApiController]
    public class PrivacyRoomController : ControllerBase
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

        public PrivacyRoomController(
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
        public object SetKeysFromBody([FromBody] PrivacyRoomModel model)
        {
            return SetKeys(model);
        }

        [Update("keys")]
        [Consumes("application/x-www-form-urlencoded")]
        public object SetKeysFromForm([FromForm] PrivacyRoomModel model)
        {
            return SetKeys(model);
        }

        private object SetKeys(PrivacyRoomModel model)
        {
            PermissionContext.DemandPermissions(new UserSecurityProvider(AuthContext.CurrentAccount.ID), Constants.Action_EditUser);

            if (!PrivacyRoomSettings.GetEnabled(SettingsManager)) throw new System.Security.SecurityException();

            var keyPair = EncryptionKeyPairHelper.GetKeyPair();
            if (keyPair != null)
            {
                if (!string.IsNullOrEmpty(keyPair.PublicKey) && !model.Update)
                {
                    return new { isset = true };
                }

                Log.InfoFormat("User {0} updates address", AuthContext.CurrentAccount.ID);
            }

            EncryptionKeyPairHelper.SetKeyPair(model.PublicKey, model.PrivateKeyEnc);

            return new
            {
                isset = true
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <visible>false</visible>
        [Read("keys")]
        public EncryptionKeyPair GetKeys()
        {
            PermissionContext.DemandPermissions(new UserSecurityProvider(AuthContext.CurrentAccount.ID), Constants.Action_EditUser);

            if (!PrivacyRoomSettings.GetEnabled(SettingsManager)) throw new System.Security.SecurityException();

            return EncryptionKeyPairHelper.GetKeyPair();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <visible>false</visible>
        [Read("access/{fileId}")]
        public Task<IEnumerable<EncryptionKeyPair>> GetPublicKeysWithAccess(string fileId)
        {
            if (!PrivacyRoomSettings.GetEnabled(SettingsManager)) throw new System.Security.SecurityException();

            return EncryptionKeyPairHelper.GetKeyPairAsync(fileId, FileStorageService);
        }


        [Read("access/{fileId:int}")]
        public Task<IEnumerable<EncryptionKeyPair>> GetPublicKeysWithAccess(int fileId)
        {
            if (!PrivacyRoomSettings.GetEnabled(SettingsManager)) throw new System.Security.SecurityException();

            return EncryptionKeyPairHelper.GetKeyPairAsync(fileId, FileStorageServiceInt);
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
        public bool SetPrivacyRoomFromBody([FromBody] PrivacyRoomModel model)
        {
            return SetPrivacyRoom(model);
        }

        [Update("")]
        [Consumes("application/x-www-form-urlencoded")]
        public bool SetPrivacyRoomFromForm([FromForm] PrivacyRoomModel model)
        {
            return SetPrivacyRoom(model);
        }

        private bool SetPrivacyRoom(PrivacyRoomModel model)
        {
            PermissionContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            if (model.Enable)
            {
                if (!PrivacyRoomSettings.IsAvailable(TenantManager))
                {
                    throw new BillingException(Resource.ErrorNotAllowedOption, "PrivacyRoom");
                }
            }

            PrivacyRoomSettings.SetEnabled(TenantManager, SettingsManager, model.Enable);

            MessageService.Send(model.Enable ? MessageAction.PrivacyRoomEnable : MessageAction.PrivacyRoomDisable);

            return model.Enable;
        }
    }
}