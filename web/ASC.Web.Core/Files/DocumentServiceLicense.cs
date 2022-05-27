/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
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
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Core;
using ASC.Core.Billing;

using static ASC.Web.Core.Files.DocumentService;

namespace ASC.Web.Core.Files
{
    [Scope]
    public class DocumentServiceLicense
    {
        private static readonly TimeSpan CACHE_EXPIRATION = TimeSpan.FromMinutes(15);

        private ICache Cache { get; }
        public CoreBaseSettings CoreBaseSettings { get; }
        private FilesLinkUtility FilesLinkUtility { get; }
        private FileUtility FileUtility { get; }
        private IHttpClientFactory ClientFactory { get; }


        public DocumentServiceLicense(
            ICache cache,
            CoreBaseSettings coreBaseSettings,
            FilesLinkUtility filesLinkUtility,
            FileUtility fileUtility,
            IHttpClientFactory clientFactory)
        {
            Cache = cache;
            CoreBaseSettings = coreBaseSettings;
            FilesLinkUtility = filesLinkUtility;
            FileUtility = fileUtility;
            ClientFactory = clientFactory;
        }

        private Task<CommandResponse> GetDocumentServiceLicenseAsync()
        {
            if (!CoreBaseSettings.Standalone) return Task.FromResult<CommandResponse>(null);
            if (string.IsNullOrEmpty(FilesLinkUtility.DocServiceCommandUrl)) return Task.FromResult<CommandResponse>(null);

            return InternalGetDocumentServiceLicenseAsync();
        }

        private async Task<CommandResponse> InternalGetDocumentServiceLicenseAsync()
        {
            var cacheKey = "DocumentServiceLicense";
            var commandResponse = Cache.Get<CommandResponse>(cacheKey);
            if (commandResponse == null)
            {
                commandResponse = await DocumentService.CommandRequestAsync(
                       FileUtility,
                       FilesLinkUtility.DocServiceCommandUrl,
                       DocumentService.CommandMethod.License,
                       null,
                       null,
                       null,
                       null,
                       FileUtility.SignatureSecret,
                       ClientFactory
                       );
                Cache.Insert(cacheKey, commandResponse, DateTime.UtcNow.Add(CACHE_EXPIRATION));
            }

            return commandResponse;
        }

        public async Task<Dictionary<string, DateTime>> GetLicenseQuotaAsync()
        {
            var commandResponse = await GetDocumentServiceLicenseAsync();
            if (commandResponse == null
                || commandResponse.Quota == null
                || commandResponse.Quota.Users == null)
                return null;

            var result = new Dictionary<string, DateTime>();
            commandResponse.Quota.Users.ForEach(user => result.Add(user.UserId, user.Expire));
            return result;
        }

        public async Task<License> GetLicenseAsync()
        {
            var commandResponse = await GetDocumentServiceLicenseAsync();
            if (commandResponse == null)
                return null;

            return commandResponse.License;
        }
    }
}