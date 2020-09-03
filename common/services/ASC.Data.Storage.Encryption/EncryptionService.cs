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
using ASC.Common.Threading.Progress;

namespace ASC.Data.Storage.Encryption
{
    class EncryptionService : IEncryptionService
    {
        private EncryptionWorker EncryptionWorker { get; }
        private ICacheNotify<EncryptionStop> NotifyStop { get; }

        public EncryptionService(EncryptionWorker encryptionWorker, ICacheNotify<EncryptionStop> notifyStop)
        {
            EncryptionWorker = encryptionWorker;
            NotifyStop = notifyStop;
        }

        public void Start(EncryptionSettingsProto encryptionSettingsProto)
        {
            NotifyStop.Subscribe((n) => Stop(), CacheNotifyAction.Insert);
            EncryptionWorker.Start(encryptionSettingsProto);
        }

        public void Stop()
        {
            NotifyStop.Unsubscribe(CacheNotifyAction.Insert);
            EncryptionWorker.Stop();
        }
    }

    public static class EncryptionServiceExtension
    {
        public static DIHelper AddEncryptionService(this DIHelper services)
        {
            services.TryAddScoped<EncryptionService>();
            return services.AddEncryptionWorkerService();
        }
    }
}
