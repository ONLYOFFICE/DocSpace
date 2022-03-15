namespace ASC.Web.Files.Core.Entries;

public class EncryptionKeyPairDto
{
    public string PrivateKeyEnc { get; set; }
    public string PublicKey { get; set; }
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

    public void SetKeyPair(string publicKey, string privateKeyEnc)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(publicKey);
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(privateKeyEnc);

        var user = _userManager.GetUsers(_authContext.CurrentAccount.ID);
        if (!_authContext.IsAuthenticated || user.IsVisitor(_userManager))
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
        _encryptionLoginProvider.SetKeys(user.Id, keyPairString);
    }

    public EncryptionKeyPairDto GetKeyPair()
    {
        var currentAddressString = _encryptionLoginProvider.GetKeys();
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

    public async Task<IEnumerable<EncryptionKeyPairDto>> GetKeyPairAsync<T>(T fileId, FileStorageService<T> FileStorageService)
    {
        var fileDao = _daoFactory.GetFileDao<T>();

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

        if (file.RootFolderType != FolderType.Privacy)
        {
            throw new NotSupportedException();
        }

        var tmpFiles = await FileStorageService.GetSharedInfoAsync(new List<T> { fileId }, new List<T> { });
        var fileShares = tmpFiles.ToList();
        fileShares = fileShares.Where(share => !share.SubjectGroup
                                        && !share.SubjectId.Equals(FileConstant.ShareLinkId)
                                        && share.Share == FileShare.ReadWrite).ToList();

        var fileKeysPair = fileShares.Select(share =>
        {
            var fileKeyPairString = _encryptionLoginProvider.GetKeys(share.SubjectId);
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
            if (fileKeyPair.UserId != share.SubjectId)
            {
                return null;
            }

            fileKeyPair.PrivateKeyEnc = null;

            return fileKeyPair;
        })
            .Where(keyPair => keyPair != null);

        return fileKeysPair;
    }
}
