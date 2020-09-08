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


using System;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core.Notify;

using Microsoft.Extensions.Options;

namespace ASC.Data.Storage.Encryption
{
    public class NotifyHelper
    {
        private const string NotifyService = "ASC.Web.Studio.Core.Notify.StudioNotifyService, ASC.Web.Studio";

        private string ServerRootPath { get; set; }
        private NotifyServiceClient NotifyServiceClient { get; set; }
        private ILog Log { get; set; }

        public NotifyHelper( IOptionsMonitor<ILog> option, NotifyServiceClient notifyServiceClient)
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
            try
            {
                NotifyServiceClient.InvokeSendMethod(NotifyService, method, tenantId, ServerRootPath);
            }
            catch (Exception error)
            {
                Log.Warn("Error while sending notification", error);
            }
        }
    }

    public static class NotifyHelperExtension
    {
        public static DIHelper AddNotifyHelperService(this DIHelper services)
        {
            services.TryAddSingleton<NotifyHelper>();
            return services;

        }
    }
}
