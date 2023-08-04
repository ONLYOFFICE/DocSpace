// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Web.Files.Core.Entries;

/// <summary>
/// </summary>
public class EncryptionKeyPairDto
{
    /// <summary>Private key</summary>
    /// <type>System.String, System</type>
    public string PrivateKeyEnc { get; set; }

    /// <summary>Public key</summary>
    /// <type>System.String, System</type>
    public string PublicKey { get; set; }

    /// <summary>User ID</summary>
    /// <type>System.String, System</type>
    public Guid UserId { get; set; }
}

[Scope]
public class EncryptionKeyPairDtoHelper
{
    private readonly UserManager _userManager;
    private readonly AuthContext _authContext;
    private readonly EncryptionLoginProvider _encryptionLoginProvider;
    private readonly FileSecurity _fileSecurity;
    private readonly IDaoFactory _daoFactory;

    public EncryptionKeyPairDtoHelper(
        UserManager userManager,
        AuthContext authContext,
        EncryptionLoginProvider encryptionLoginProvider,
        FileSecurity fileSecurity,
        IDaoFactory daoFactory)
    {
        _userManager = userManager;
        _authContext = authContext;
        _encryptionLoginProvider = encryptionLoginProvider;
        _fileSecurity = fileSecurity;
        _daoFactory = daoFactory;
    }

    public async Task SetKeyPairAsync(string publicKey, string privateKeyEnc)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(publicKey);
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(privateKeyEnc);

        var user = await _userManager.GetUsersAsync(_authContext.CurrentAccount.ID);
        if (!_authContext.IsAuthenticated || await _userManager.IsUserAsync(user))
        {
            throw new SecurityException();
        }

        var keyPair = new EncryptionKeyPairDto
        {
            PrivateKeyEnc = privateKeyEnc,
            PublicKey = publicKey,
            UserId = user.Id,
        };

        var keyPairString = JsonSerializer.Serialize(keyPair);
        await _encryptionLoginProvider.SetKeysAsync(user.Id, keyPairString);
    }

    public async Task<EncryptionKeyPairDto> GetKeyPairAsync()
    {
        var currentAddressString = await _encryptionLoginProvider.GetKeysAsync();
        if (string.IsNullOrEmpty(currentAddressString))
        {
            return null;
        }

        var options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true
        };
        var keyPair = JsonSerializer.Deserialize<EncryptionKeyPairDto>(currentAddressString, options);
        if (keyPair.UserId != _authContext.CurrentAccount.ID)
        {
            return null;
        }

        return keyPair;
    }

    public async Task<IEnumerable<EncryptionKeyPairDto>> GetKeyPairAsync<T>(T fileId, FileStorageService FileStorageService)
    {
        var fileDao = _daoFactory.GetFileDao<T>();
        var folderDao = _daoFactory.GetFolderDao<T>();

        await fileDao.InvalidateCacheAsync(fileId);

        var file = await fileDao.GetFileAsync(fileId);
        if (file == null)
        {
            throw new FileNotFoundException(FilesCommonResource.ErrorMassage_FileNotFound);
        }

        if (!await _fileSecurity.CanEditAsync(file))
        {
            throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_EditFile);
        }

        var locatedInPrivateRoom = file.RootFolderType == FolderType.VirtualRooms
            && await DocSpaceHelper.LocatedInPrivateRoomAsync(file, folderDao);

        if (file.RootFolderType != FolderType.Privacy && !locatedInPrivateRoom)
        {
            throw new NotSupportedException();
        }

        var tmpFiles = await FileStorageService.GetSharedInfoAsync(new List<T> { fileId }, new List<T> { });
        var fileShares = tmpFiles.ToList();
        fileShares = fileShares.Where(share => !share.SubjectGroup
                                        && !share.Id.Equals(FileConstant.ShareLinkId)
                                        && share.Access == FileShare.ReadWrite).ToList();

        var tasks = fileShares.Select(async share =>
        {
            var fileKeyPairString = await _encryptionLoginProvider.GetKeysAsync(share.Id);
            if (string.IsNullOrEmpty(fileKeyPairString))
            {
                return null;
            }

            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };
            var fileKeyPair = JsonSerializer.Deserialize<EncryptionKeyPairDto>(fileKeyPairString, options);
            if (fileKeyPair.UserId != share.Id)
            {
                return null;
            }

            fileKeyPair.PrivateKeyEnc = null;

            return fileKeyPair;
        });

        var fileKeysPair = (await Task.WhenAll(tasks))
            .Where(keyPair => keyPair != null);

        return fileKeysPair;
    }
}
