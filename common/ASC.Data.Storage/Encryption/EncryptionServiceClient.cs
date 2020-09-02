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
using ASC.Common.Caching;
using ASC.Common.Module;

namespace ASC.Data.Storage.Encryption
{
    public class EncryptionServiceClient :  IEncryptionService
    {

        private ICacheNotify<EncryptionSettingsProto> NotifySetting { get; }
        private ICacheNotify<EncryptionStop> NotifyStop { get; }
        private ICacheNotify<ProgressEncryption> ProgressEncryption { get; }

        public EncryptionServiceClient(
            ICacheNotify<EncryptionSettingsProto> notifySetting, ICacheNotify<ProgressEncryption> progressEncryption, ICacheNotify<EncryptionStop> notifyStop)
        {
            NotifySetting = notifySetting;
            NotifyStop = notifyStop;
            ProgressEncryption = progressEncryption;
        }

        public void Start(EncryptionSettingsProto encryptionSettingsProto)
        {
            NotifySetting.Publish(encryptionSettingsProto, CacheNotifyAction.Insert);
        }

        public void Stop()
        {
            NotifyStop.Publish(new EncryptionStop(), CacheNotifyAction.Insert);
        }

        public void Get()
        {
            ProgressEncryption.Subscribe((n) => { 
                var k = n.Proggress;
            },CacheNotifyAction.Insert);
        }
    }

    public static class EncryptionServiceClientExtension
    {
        public static DIHelper AddEEncryptionServiceClient(this DIHelper services)
        {
            services.TryAddScoped<EncryptionServiceClient>();
            return services;
        }
    }
}
