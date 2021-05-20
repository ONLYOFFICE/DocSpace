/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core.Notify;
using ASC.Notify.Messages;

using Microsoft.Extensions.Options;

namespace ASC.Data.Storage.Encryption
{
    [Scope]
    public class NotifyHelper
    {
        private const string NotifyService = "ASC.Web.Studio.Core.Notify.StudioNotifyService, ASC.Web.Core";

        private string ServerRootPath { get; set; }
        private NotifyServiceClient NotifyServiceClient { get; set; }
        private ILog Log { get; set; }

        public NotifyHelper(IOptionsMonitor<ILog> option, NotifyServiceClient notifyServiceClient)
        {
            NotifyServiceClient = notifyServiceClient;
            Log = option.CurrentValue;
        }

        public void Init(string serverRootPath)
        {
            ServerRootPath = serverRootPath;
        }

        public void SendStorageEncryptionStart(int tenantId)
        {
            SendStorageEncryptionNotification("SendStorageEncryptionStart", tenantId);
        }

        public void SendStorageEncryptionSuccess(int tenantId)
        {
            SendStorageEncryptionNotification("SendStorageEncryptionSuccess", tenantId);
        }

        public void SendStorageEncryptionError(int tenantId)
        {
            SendStorageEncryptionNotification("SendStorageEncryptionError", tenantId);
        }

        public void SendStorageDecryptionStart(int tenantId)
        {
            SendStorageEncryptionNotification("SendStorageDecryptionStart", tenantId);
        }

        public void SendStorageDecryptionSuccess(int tenantId)
        {
            SendStorageEncryptionNotification("SendStorageDecryptionSuccess", tenantId);
        }

        public void SendStorageDecryptionError(int tenantId)
        {
            SendStorageEncryptionNotification("SendStorageDecryptionError", tenantId);
        }

        private void SendStorageEncryptionNotification(string method, int tenantId)
        {
            var notifyInvoke = new NotifyInvoke()
            {
                Service = NotifyService,
                Method = method,
                Tenant = tenantId
            };
            notifyInvoke.Parameters.Add(ServerRootPath);
            try
            {
                NotifyServiceClient.InvokeSendMethod(notifyInvoke);
            }
            catch (Exception error)
            {
                Log.Warn("Error while sending notification", error);
            }
        }
    }
}
