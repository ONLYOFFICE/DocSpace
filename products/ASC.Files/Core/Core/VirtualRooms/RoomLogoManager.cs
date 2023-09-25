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

using Image = SixLabors.ImageSharp.Image;

namespace ASC.Files.Core.VirtualRooms;

[Scope]
public class RoomLogoManager
{
    private const string LogosPath = "{0}_{1}.png";
    private const string ModuleName = "room_logos";
    private const string TempDomainPath = "logos_temp";

    private static readonly (SizeName, Size) _originalLogoSize = (SizeName.Original, new Size(1280, 1280));
    private static readonly (SizeName, Size) _largeLogoSize = (SizeName.Large, new Size(96, 96));
    private static readonly (SizeName, Size) _mediumLogoSize = (SizeName.Medium, new Size(32, 32));
    private static readonly (SizeName, Size) _smallLogoSize = (SizeName.Small, new Size(16, 16));

    private readonly IDaoFactory _daoFactory;
    private readonly FileSecurity _fileSecurity;
    private readonly ILogger<RoomLogoManager> _logger;
    private readonly StorageFactory _storageFactory;
    private readonly TenantManager _tenantManager;
    private IDataStore _dataStore;
    private readonly FilesMessageService _filesMessageService;
    private readonly EmailValidationKeyProvider _emailValidationKeyProvider;
    private readonly SecurityContext _securityContext;
    private readonly FileUtilityConfiguration _fileUtilityConfiguration;

    public RoomLogoManager(
        StorageFactory storageFactory,
        TenantManager tenantManager,
        IDaoFactory daoFactory,
        FileSecurity fileSecurity,
        ILogger<RoomLogoManager> logger,
        FilesMessageService filesMessageService,
        EmailValidationKeyProvider emailValidationKeyProvider,
        SecurityContext securityContext,
        FileUtilityConfiguration fileUtilityConfiguration)
    {
        _storageFactory = storageFactory;
        _tenantManager = tenantManager;
        _daoFactory = daoFactory;
        _fileSecurity = fileSecurity;
        _logger = logger;
        _filesMessageService = filesMessageService;
        _emailValidationKeyProvider = emailValidationKeyProvider;
        _securityContext = securityContext;
        _fileUtilityConfiguration = fileUtilityConfiguration;
    }

    public bool EnableAudit { get; set; } = true;
    private int TenantId => _tenantManager.GetCurrentTenant().Id;

    private async ValueTask<IDataStore> GetDataStoreAsync()
    {
        return _dataStore ??= await _storageFactory.GetStorageAsync(TenantId, ModuleName);
    }

    public async Task<Folder<T>> CreateAsync<T>(T id, string tempFile, int x, int y, int width, int height)
    {
        var folderDao = _daoFactory.GetFolderDao<T>();
        var room = await folderDao.GetFolderAsync(id);

        if (string.IsNullOrEmpty(tempFile))
        {
            return room;
        }

        if (room == null || !DocSpaceHelper.IsRoom(room.FolderType))
        {
            throw new ItemNotFoundException();
        }

        if (room.RootFolderType == FolderType.Archive || !await _fileSecurity.CanEditRoomAsync(room))
        {
            throw new InvalidOperationException(FilesCommonResource.ErrorMessage_SecurityException_EditRoom);
        }

        var fileName = Path.GetFileName(tempFile);
        var data = await GetTempAsync(fileName);

        var stringId = GetId(room);

        await SaveWithProcessAsync(stringId, data, -1, new Point(x, y), new Size(width, height));
        await RemoveTempAsync(fileName);

        room.HasLogo = true;

        if (room.ProviderEntry)
        {
            await _daoFactory.ProviderDao.UpdateProviderInfoAsync(room.ProviderId, true);
        }
        else
        {
            await folderDao.SaveFolderAsync(room);
        }

        if (EnableAudit)
        {
            _ = _filesMessageService.SendAsync(MessageAction.RoomLogoCreated, room, room.Title);
        }

        return room;
    }

    public async Task<Folder<T>> DeleteAsync<T>(T id, bool checkPermissions = true)
    {
        var folderDao = _daoFactory.GetFolderDao<T>();
        var room = await folderDao.GetFolderAsync(id);

        if (checkPermissions && !await _fileSecurity.CanEditRoomAsync(room))
        {
            throw new InvalidOperationException(FilesCommonResource.ErrorMessage_SecurityException_EditRoom);
        }

        var stringId = GetId(room);

        try
        {
            await (await GetDataStoreAsync()).DeleteFilesAsync(string.Empty, $"{ProcessFolderId(stringId)}*.*", false);
            room.HasLogo = false;

            if (room.ProviderEntry)
            {
                await _daoFactory.ProviderDao.UpdateProviderInfoAsync(room.ProviderId, false);
            }
            else
            {
                await folderDao.SaveFolderAsync(room);
            }

            if (EnableAudit)
            {
                _ = _filesMessageService.SendAsync(MessageAction.RoomLogoDeleted, room, room.Title);
            }
        }
        catch (Exception e)
        {
            _logger.ErrorRemoveRoomLogo(e);
        }

        return room;
    }

    public async ValueTask<Logo> GetLogoAsync<T>(Folder<T> room)
    {
        if (!room.HasLogo)
        {
            if (string.IsNullOrEmpty(room.Color))
            {
                room.Color = GetRandomColour();

                var folderDao = _daoFactory.GetFolderDao<T>();
                await folderDao.SaveFolderAsync(room);
            }

            return new Logo
            {
                Original = string.Empty,
                Large = string.Empty,
                Medium = string.Empty,
                Small = string.Empty,
                Color = room.Color
            };
        }

        var id = GetId(room);

        var cacheKey = Math.Abs(room.ModifiedOn.GetHashCode());
        var secure = !_securityContext.IsAuthenticated;

        return new Logo
        {
            Original = await GetLogoPathAsync(id, SizeName.Original, cacheKey, secure),
            Large = await GetLogoPathAsync(id, SizeName.Large, cacheKey, secure),
            Medium = await GetLogoPathAsync(id, SizeName.Medium, cacheKey, secure),
            Small = await GetLogoPathAsync(id, SizeName.Small, cacheKey, secure)
        };
    }

    public async Task<string> SaveTempAsync(byte[] data, long maxFileSize)
    {
        data = UserPhotoThumbnailManager.TryParseImage(data, maxFileSize, _originalLogoSize.Item2, out _, out _, out _);

        var fileName = $"{Guid.NewGuid()}.png";

        using var stream = new MemoryStream(data);
        var path = await (await GetDataStoreAsync()).SaveAsync(TempDomainPath, fileName, stream);

        var pathAsString = path.ToString();

        var pathWithoutQuery = pathAsString;

        if (pathAsString.IndexOf('?') > 0)
        {
            pathWithoutQuery = pathAsString.Substring(0, pathAsString.IndexOf('?'));
        }

        return pathWithoutQuery;
    }

    internal string GetRandomColour()
    {
        var rand = new Random();
        var color = _fileUtilityConfiguration.LogoColors[rand.Next(_fileUtilityConfiguration.LogoColors.Count - 1)];
        var result = Color.FromRgba(color.R, color.G, color.B, 1).ToHex();
        return result.Substring(0, result.Length - 2);//without opacity
    }

    private async Task RemoveTempAsync(string fileName)
    {
        var index = fileName.LastIndexOf('.');
        var fileNameWithoutExt = (index != -1) ? fileName.Substring(0, index) : fileName;

        try
        {
            await (await GetDataStoreAsync()).DeleteFilesAsync(TempDomainPath, "", fileNameWithoutExt + "*.*", false);
        }
        catch (Exception e)
        {
            _logger.ErrorRemoveTempPhoto(e);
        }
    }

    private async Task SaveWithProcessAsync(string id, byte[] imageData, long maxFileSize, Point position, Size cropSize)
    {
        imageData = UserPhotoThumbnailManager.TryParseImage(imageData, maxFileSize, _originalLogoSize.Item2, out var _, out var _, out var _);

        var fileName = string.Format(LogosPath, ProcessFolderId(id), SizeName.Original.ToStringLowerFast());

        if (imageData == null || imageData.Length == 0)
        {
            return;
        }

        using var stream = new MemoryStream(imageData);
        await (await GetDataStoreAsync()).SaveAsync(fileName, stream);

        await ResizeAndSaveAsync(id, imageData, maxFileSize, _mediumLogoSize, position, cropSize);
        await ResizeAndSaveAsync(id, imageData, maxFileSize, _smallLogoSize, position, cropSize);
        await ResizeAndSaveAsync(id, imageData, maxFileSize, _largeLogoSize, position, cropSize);
    }

    private async Task ResizeAndSaveAsync<T>(T id, byte[] data, long maxFileSize, (SizeName, Size) size, Point position, Size cropSize)
    {
        if (data is not { Length: > 0 })
        {
            throw new Web.Core.Users.UnknownImageFormatException();
        }
        if (maxFileSize != -1 && data.Length > maxFileSize)
        {
            throw new ImageWeightLimitException();
        }

        try
        {
            using var stream = new MemoryStream(data);
            using var img = Image.Load(stream);

            if (size.Item2 != img.Size)
            {
                using var img2 = UserPhotoThumbnailManager.GetImage(img, size.Item2, new UserPhotoThumbnailSettings(position, cropSize));
                data = CommonPhotoManager.SaveToBytes(img2);
            }
            else
            {
                data = CommonPhotoManager.SaveToBytes(img);
            }

            var fileName = string.Format(LogosPath, ProcessFolderId(id), size.Item1.ToStringLowerFast());

            using var stream2 = new MemoryStream(data);
            await (await GetDataStoreAsync()).SaveAsync(fileName, stream2);
        }
        catch (ArgumentException error)
        {
            throw new Web.Core.Users.UnknownImageFormatException(error);
        }
    }

    private async ValueTask<string> GetLogoPathAsync<T>(T id, SizeName size, int hash, bool secure = false)
    {
        var fileName = string.Format(LogosPath, ProcessFolderId(id), size.ToStringLowerFast());
        var headers = secure ? new[] { SecureHelper.GenerateSecureKeyHeader(fileName, _emailValidationKeyProvider) } : null;

        var store = await GetDataStoreAsync();

        var uri = await store.GetPreSignedUriAsync(string.Empty, fileName, TimeSpan.MaxValue, headers);

        return uri + (secure ? "&" : "?") + $"hash={hash}";
    }

    private async Task<byte[]> GetTempAsync(string fileName)
    {
        await using var stream = await (await GetDataStoreAsync()).GetReadStreamAsync(TempDomainPath, fileName);

        var data = new MemoryStream();
        var buffer = new byte[1024 * 10];
        while (true)
        {
            var count = await stream.ReadAsync(buffer);
            if (count == 0)
            {
                break;
            }

            await data.WriteAsync(buffer.AsMemory(0, count));
        }

        return data.ToArray();
    }

    private static string ProcessFolderId<T>(T id)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        return id.GetType() != typeof(string)
            ? id.ToString()
            : id.ToString()?.Replace("-", "").Replace("|", "");
    }

    private static string GetId<T>(Folder<T> room)
    {
        if (!room.ProviderEntry)
        {
            return room.Id.ToString();
        }

        if (room.Id.ToString()!.Contains(Selectors.SharpBox.Id))
        {
            return $"{Selectors.SharpBox.Id}-{room.ProviderId}";
        }

        if (room.Id.ToString()!.Contains(Selectors.SharePoint.Id))
        {
            return $"{Selectors.SharePoint.Id}-{room.ProviderId}";
        }

        return room.Id.ToString();
    }
}

[EnumExtensions]
public enum SizeName
{
    Original = 0,
    Large = 1,
    Medium = 2,
    Small = 3,
}