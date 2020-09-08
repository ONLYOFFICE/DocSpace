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
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.Data.Storage.Encryption
{
    public class EncryptionWorker
    {
        private CancellationTokenSource TokenSource { get; set; }
        private ICache Cache { get; }
        private object Locker { get; }
        private FactoryOperation FactoryOperation { get; }

        public EncryptionWorker(FactoryOperation factoryOperation)
        {
            Cache = AscCache.Memory;
            Locker = new object();
            FactoryOperation = factoryOperation;
        }

        public void Start(EncryptionSettingsProto encryptionSettings)
        {
            EncryptionOperation encryptionOperation;
            lock (Locker)
            {
                if (Cache.Get<EncryptionOperation>(GetCacheKey()) != null) return;
                TokenSource = new CancellationTokenSource();
                encryptionOperation = FactoryOperation.CreateOperation(encryptionSettings);
                Cache.Insert(GetCacheKey(), encryptionOperation, DateTime.MaxValue);
            }

            var task = new Task(encryptionOperation.RunJob, TokenSource.Token, TaskCreationOptions.LongRunning);

            task.ConfigureAwait(false)
                .GetAwaiter()
                .OnCompleted(() =>
                {
                    lock (Locker)
                    {
                        Cache.Remove(GetCacheKey());
                    }
                });

            task.Start();
        }

        public void Stop()
        {
            TokenSource.Cancel();
        }

        public string GetCacheKey()
        {
            return typeof(EncryptionOperation).FullName;
        }
    }

    public class FactoryOperation
    {
        private IServiceProvider ServiceProvider { get; set; }

        public FactoryOperation(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public EncryptionOperation CreateOperation(EncryptionSettingsProto encryptionSettings)
        {
            var item = ServiceProvider.GetService<EncryptionOperation>();
            item.Init(encryptionSettings);
            return item;
        }
    }

    public static class EncryptionWorkerExtension
    {
        public static DIHelper AddEncryptionWorkerService(this DIHelper services)
        {
            services.TryAddSingleton<EncryptionWorker>();
            services.TryAddSingleton<FactoryOperation>();
            return services.AddEncryptionOperationService();
        }
    }
}
