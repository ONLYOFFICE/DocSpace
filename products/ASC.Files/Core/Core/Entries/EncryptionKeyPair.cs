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
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

using ASC.Common;
using ASC.Core;
using ASC.Core.Users;
using ASC.Files.Core;
using ASC.Files.Core.Data;
using ASC.Files.Core.Resources;
using ASC.Files.Core.Security;
using ASC.Web.Files.Services.WCFService;
using ASC.Web.Studio.Core;

namespace ASC.Web.Files.Core.Entries
{
    public class EncryptionKeyPair
    {
        public string PrivateKeyEnc { get; set; }
        public string PublicKey { get; set; }
        public Guid UserId { get; set; }
    }

    public class EncryptionKeyPairHelper<T>
    {
        private UserManager UserManager { get; }
        private AuthContext AuthContext { get; }
        private EncryptionLoginProvider EncryptionLoginProvider { get; }
        private FileSecurity FileSecurity { get; }
        private IDaoFactory DaoFactory { get; }
        private FileStorageService<T> FileStorageService { get; }

        public EncryptionKeyPairHelper(
            UserManager userManager,
            AuthContext authContext,
            EncryptionLoginProvider encryptionLoginProvider,
            FileSecurity fileSecurity,
            IDaoFactory daoFactory,
            FileStorageService<T> fileStorageService)
        {
            UserManager = userManager;
            AuthContext = authContext;
            EncryptionLoginProvider = encryptionLoginProvider;
            FileSecurity = fileSecurity;
            DaoFactory = daoFactory;
            FileStorageService = fileStorageService;
        }

        public void SetKeyPair(string publicKey, string privateKeyEnc)
        {
            if (string.IsNullOrEmpty(publicKey)) throw new ArgumentNullException("publicKey");
            if (string.IsNullOrEmpty(privateKeyEnc)) throw new ArgumentNullException("privateKeyEnc");

            var user = UserManager.GetUsers(AuthContext.CurrentAccount.ID);
            if (!AuthContext.IsAuthenticated || user.IsVisitor(UserManager)) throw new System.Security.SecurityException();

            var keyPair = new EncryptionKeyPair
            {
                PrivateKeyEnc = privateKeyEnc,
                PublicKey = publicKey,
                UserId = user.ID,
            };

            var keyPairString = JsonSerializer.Serialize(keyPair);
            EncryptionLoginProvider.SetKeys(user.ID, keyPairString);
        }

        public EncryptionKeyPair GetKeyPair()
        {
            var currentAddressString = EncryptionLoginProvider.GetKeys();
            if (string.IsNullOrEmpty(currentAddressString)) return null;

            var keyPair = JsonSerializer.Deserialize<EncryptionKeyPair>(currentAddressString);
            if (keyPair.UserId != AuthContext.CurrentAccount.ID) return null;
            return keyPair;
        }

        public IEnumerable<EncryptionKeyPair> GetKeyPair(T fileId)
        {
            var fileDao = DaoFactory.GetFileDao<T>();

            fileDao.InvalidateCache(fileId);

            var file = fileDao.GetFile(fileId);
            if (file == null) throw new System.IO.FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
            if (!FileSecurity.CanEdit(file)) throw new System.Security.SecurityException(FilesCommonResource.ErrorMassage_SecurityException_EditFile);
            if (file.RootFolderType != FolderType.Privacy) throw new NotSupportedException();

            var fileShares = FileStorageService.GetSharedInfo(new ItemList<string> { string.Format("file_{0}", fileId) }).ToList();
            fileShares = fileShares.Where(share => !share.SubjectGroup
                                            && !share.SubjectId.Equals(FileConstant.ShareLinkId)
                                            && share.Share == FileShare.ReadWrite).ToList();

            var fileKeysPair = fileShares.Select(share =>
            {
                var fileKeyPairString = EncryptionLoginProvider.GetKeys(share.SubjectId);
                if (string.IsNullOrEmpty(fileKeyPairString)) return null;

                var fileKeyPair = JsonSerializer.Deserialize<EncryptionKeyPair>(fileKeyPairString);
                if (fileKeyPair.UserId != share.SubjectId) return null;

                fileKeyPair.PrivateKeyEnc = null;

                return fileKeyPair;
            })
                .Where(keyPair => keyPair != null);

            return fileKeysPair;
        }
    }

    public static class EncryptionKeyPairHelperExtention
    {
        public static DIHelper AddEncryptionKeyPairHelperService(this DIHelper services)
        {
            if (services.TryAddScoped<EncryptionKeyPairHelper<string>>())
            {
                services.TryAddScoped<EncryptionKeyPairHelper<int>>();
                services
                    .AddAuthContextService()
                    .AddUserManagerService()
                    .AddEncryptionLoginProviderService()
                    .AddFileSecurityService()
                    .AddDaoFactoryService()
                    .AddFileStorageService();
            }

            return services;
        }
    }
}