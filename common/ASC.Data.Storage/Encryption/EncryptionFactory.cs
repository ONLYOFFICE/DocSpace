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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ASC.Data.Storage.Encryption
{
    public class EncryptionFactory
    {
        private IServiceProvider ServiceProvider { get; }

        public EncryptionFactory(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public ICrypt GetCrypt(string storageName, EncryptionSettings encryptionSettings)
        {
            var crypt = ServiceProvider.GetService<Crypt>();
            crypt.Init(storageName, encryptionSettings);
            return crypt;
        }

        public IMetadata GetMetadata()
        {
            return ServiceProvider.GetService<Metadata>();
        }
    }

    public static class EncryptionFactoryExtension
    {
        public static DIHelper AddEncryptionFactoryService(this DIHelper services)
        {
            services.TryAddSingleton<EncryptionFactory>();
            services.TryAddTransient<Crypt>();
            services.TryAddTransient<Metadata>();
            return services;
        }
    }
}
