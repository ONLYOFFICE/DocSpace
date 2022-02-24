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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Core;
using ASC.Core.Users;
using ASC.Files.Core;
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

    [Scope]
    public class EncryptionKeyPairHelper
    {
        private UserManager UserManager { get; }
        private AuthContext AuthContext { get; }
        private EncryptionLoginProvider EncryptionLoginProvider { get; }
        private FileSecurity FileSecurity { get; }
        private IDaoFactory DaoFactory { get; }

        public EncryptionKeyPairHelper(
            UserManager userManager,
            AuthContext authContext,
            EncryptionLoginProvider encryptionLoginProvider,
            FileSecurity fileSecurity,
            IDaoFactory daoFactory)
        {
            UserManager = userManager;
            AuthContext = authContext;
            EncryptionLoginProvider = encryptionLoginProvider;
            FileSecurity = fileSecurity;
            DaoFactory = daoFactory;
        }

        public void SetKeyPair(string publicKey, string privateKeyEnc)
        {
            if (string.IsNullOrEmpty(publicKey)) throw new ArgumentNullException(nameof(publicKey));
            if (string.IsNullOrEmpty(privateKeyEnc)) throw new ArgumentNullException(nameof(privateKeyEnc));

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

            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };
            var keyPair = JsonSerializer.Deserialize<EncryptionKeyPair>(currentAddressString, options);
            if (keyPair.UserId != AuthContext.CurrentAccount.ID) return null;
            return keyPair;
        }

        public async Task<IEnumerable<EncryptionKeyPair>> GetKeyPairAsync<T>(T fileId, FileStorageService<T> FileStorageService)
        {
            var fileDao = DaoFactory.GetFileDao<T>();

            await fileDao.InvalidateCacheAsync(fileId);

            var file = await fileDao.GetFileAsync(fileId);
            if (file == null) throw new System.IO.FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
            if (!await FileSecurity.CanEditAsync(file)) throw new System.Security.SecurityException(FilesCommonResource.ErrorMassage_SecurityException_EditFile);
            if (file.RootFolderType != FolderType.Privacy) throw new NotSupportedException();

            var tmpFiles = await FileStorageService.GetSharedInfoAsync(new List<T> { fileId }, new List<T> { });
            var fileShares = tmpFiles.ToList();
            fileShares = fileShares.Where(share => !share.SubjectGroup
                                            && !share.SubjectId.Equals(FileConstant.ShareLinkId)
                                            && share.Share == FileShare.ReadWrite).ToList();

            var fileKeysPair = fileShares.Select(share =>
            {
                var fileKeyPairString = EncryptionLoginProvider.GetKeys(share.SubjectId);
                if (string.IsNullOrEmpty(fileKeyPairString)) return null;


                var options = new JsonSerializerOptions
                {
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = true
                };
                var fileKeyPair = JsonSerializer.Deserialize<EncryptionKeyPair>(fileKeyPairString, options);
                if (fileKeyPair.UserId != share.SubjectId) return null;

                fileKeyPair.PrivateKeyEnc = null;

                return fileKeyPair;
            })
                .Where(keyPair => keyPair != null);

            return fileKeysPair;
        }
    }
}