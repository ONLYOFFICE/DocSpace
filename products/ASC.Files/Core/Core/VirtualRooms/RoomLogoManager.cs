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
    private const string LogosPath = "{0}_size_{1}-{2}.{3}";
    private const string ModuleName = "room_logos";
    private const string TempDomainPath = "logos_temp";
    private static Size _originalLogoSize = new Size(1280, 1280);
    private static Size _bigLogoSize = new Size(32, 32);
    private static Size _smallLogoSize = new Size(16, 16);
    private readonly IDaoFactory _daoFactory;
    private readonly FileSecurity _fileSecurity;
    private readonly ILogger<RoomLogoManager> _logger;
    private readonly StorageFactory _storageFactory;
    private readonly TenantManager _tenantManager;
    private IDataStore _dataStore;
    private readonly ICache _cache;
    private readonly FilesMessageService _filesMessageService;
    private static readonly Regex _pattern = new Regex(@"\d+-\d+", RegexOptions.Compiled);
    private static readonly Regex _cachePattern = new Regex(@"\d+\/\S+\/\d+\/\d+", RegexOptions.Compiled);
    private static readonly TimeSpan _cacheLifeTime = TimeSpan.FromMinutes(30);
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RoomLogoManager(StorageFactory storageFactory, TenantManager tenantManager, IDaoFactory daoFactory, FileSecurity fileSecurity, ILogger<RoomLogoManager> logger, AscCache cache, FilesMessageService filesMessageService, IHttpContextAccessor httpContextAccessor)
    {
        _storageFactory = storageFactory;
        _tenantManager = tenantManager;
        _daoFactory = daoFactory;
        _fileSecurity = fileSecurity;
        _logger = logger;
        _cache = cache;
        _filesMessageService = filesMessageService;
        _httpContextAccessor = httpContextAccessor;
    }

    public bool EnableAudit { get; set; } = true;
    private IDataStore DataStore => _dataStore ??= _storageFactory.GetStorage(TenantId.ToString(), ModuleName);
    private int TenantId => _tenantManager.GetCurrentTenant().Id;
    private IDictionary<string, StringValues> Headers => _httpContextAccessor?.HttpContext?.Request?.Headers;

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
            throw new ItemNotFoundException("Virtual room not found");
        }

        if (room.RootFolderType == FolderType.Archive || !await _fileSecurity.CanEditRoomAsync(room))
        {
            throw new InvalidOperationException("You don't have permission to edit the room");
        }

        var fileName = Path.GetFileName(tempFile);
        var data = await GetTempAsync(fileName);

        id = GetId(room);

        await DeleteLogo(id);
        await SaveWithProcessAsync(id, data, -1, new Point(x, y), new Size(width, height));

        if (EnableAudit)
        {
            _filesMessageService.Send(room, Headers, MessageAction.RoomLogoCreated);
        }

        return room;
    }

    public async Task<Folder<T>> DeleteAsync<T>(T id)
    {
        var folderDao = _daoFactory.GetFolderDao<T>();
        var room = await folderDao.GetFolderAsync(id);

        if (!await _fileSecurity.CanEditRoomAsync(room))
        {
            throw new InvalidOperationException("You don't have permission to edit the room");
        }

        id = GetId(room);

        try
        {
            await DeleteLogo(id);

            if (EnableAudit)
            {
                _filesMessageService.Send(room, Headers, MessageAction.RoomLogoDeleted);
            }
        }
        catch (DirectoryNotFoundException e)
        {
            _logger.ErrorRemoveRoomLogo(e);
        }

        return room;
    }

    public async Task<Logo> GetLogo<T>(Folder<T> room)
    {
        var id = GetId(room);

        return new Logo
        {
            Original = await GetOriginalLogoPath(id),
            Big = await GetBigLogoPath(id),
            Small = await GetSmallLogoPath(id),
        };
    }

    public async Task<byte[]> GetTempAsync(string fileName)
    {
        using var stream = await DataStore.GetReadStreamAsync(TempDomainPath, fileName);

        var data = new MemoryStream();
        var buffer = new byte[1024 * 10];
        while (true)
        {
            var count = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (count == 0)
            {
                break;
            }

            data.Write(buffer, 0, count);
        }

        return data.ToArray();
    }

    public async Task<string> SaveTempAsync(byte[] data, long maxFileSize)
    {
        data = UserPhotoThumbnailManager.TryParseImage(data, maxFileSize, _originalLogoSize, out var imgFormat, out _, out _);

        var fileName = Guid.NewGuid() + "." + CommonPhotoManager.GetImgFormatName(imgFormat);

        using var stream = new MemoryStream(data);
        var path = await DataStore.SaveAsync(TempDomainPath, fileName, stream);

        return path.RemoveQueryParams("auth", "expire").ToString();
    }

    public async Task<string> SaveWithProcessAsync<T>(T id, byte[] imageData, long maxFileSize, Point position, Size cropSize)
    {
        imageData = UserPhotoThumbnailManager.TryParseImage(imageData, maxFileSize, _originalLogoSize, out var imageFormat, out var width, out var height);

        var imageExtension = CommonPhotoManager.GetImgFormatName(imageFormat);

        var fileName = $"{ProcessFolderId(id)}_orig_{width}-{height}.{imageExtension}";

        if (imageData == null || imageData.Length == 0)
        {
            return string.Empty;
        }

        using var stream = new MemoryStream(imageData);
        var path = await DataStore.SaveAsync(fileName, stream);

        await ResizeAndSaveAsync(id, imageData, maxFileSize, _bigLogoSize, position, cropSize);
        await ResizeAndSaveAsync(id, imageData, maxFileSize, _smallLogoSize, position, cropSize);

        return path.ToString();
    }

    private async Task ResizeAndSaveAsync<T>(T id, byte[] data, long maxFileSize, Size size, Point position, Size cropSize)
    {
        if (data == null || data.Length <= 0)
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
            using var img = Image.Load(stream, out var format);
            var imgFormat = format;

            if (size != img.Size())
            {
                using var img2 = UserPhotoThumbnailManager.GetImage(img, size, new UserPhotoThumbnailSettings(position, cropSize));
                data = CommonPhotoManager.SaveToBytes(img2);
            }
            else
            {
                data = CommonPhotoManager.SaveToBytes(img);
            }

            var extension = CommonPhotoManager.GetImgFormatName(imgFormat);
            var fileName = string.Format(LogosPath, ProcessFolderId(id), size.Width, size.Height, extension);

            using var stream2 = new MemoryStream(data);
            await DataStore.SaveAsync(fileName, stream2);
        }
        catch (ArgumentException error)
        {
            throw new Web.Core.Users.UnknownImageFormatException(error);
        }
    }

    public async ValueTask<string> GetOriginalLogoPath<T>(T id)
    {
        var path = _cache.Get<string>(GetKey(id));

        if (!string.IsNullOrEmpty(path))
        {
            return await ValueTask.FromResult(path);
        }

        await LoadPathToCache(id);

        path = _cache.Get<string>(GetKey(id));

        return path ?? string.Empty;
    }

    public async ValueTask<string> GetBigLogoPath<T>(T id)
    {
        return await GetLogoPath(id, _bigLogoSize);
    }

    public async ValueTask<string> GetSmallLogoPath<T>(T id)
    {
        return await GetLogoPath(id, _smallLogoSize);
    }

    public async ValueTask<string> GetLogoPath<T>(T id, Size size)
    {
        var key = GetKey(id, size);

        var path = _cache.Get<string>(key);

        if (!string.IsNullOrEmpty(path))
        {
            return await ValueTask.FromResult(path);
        }

        await LoadPathToCache(id);

        path = _cache.Get<string>(key);

        return path ?? string.Empty;
    }

    public async Task LoadPathToCache<T>(T id)
    {
        var logoPath = await DataStore.ListFilesAsync(string.Empty, $"{ProcessFolderId(id)}*", false)
            .Select(u => u.ToString()).ToListAsync();

        var original = logoPath.Where(u => u.Contains("orig")).FirstOrDefault();

        _cache.Insert(GetKey(id), original, _cacheLifeTime);

        logoPath.Remove(original);

        foreach (var (k, v) in logoPath.ToDictionary(p => _pattern.Match(p).Value.Split('-')))
        {
            _cache.Insert(GetKey(id, new Size(int.Parse(k[0]), int.Parse(k[1]))), v, _cacheLifeTime);
        }
    }

    private async Task DeleteLogo<T>(T id)
    {
        await DataStore.DeleteFilesAsync(string.Empty, $"{ProcessFolderId(id)}*.*", false);

        _cache.Remove(_cachePattern);
        _cache.Remove(GetKey(id));
    }

    private string ProcessFolderId<T>(T id)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));

        return id.GetType() != typeof(string)
            ? id.ToString()
            : id.ToString()?.Replace("-", "").Replace("|", "");
    }

    private string GetKey<T>(T id, Size size)
    {
        return $"{TenantId}/{id}/{size.Width}/{size.Height}";
    }

    private string GetKey<T>(T id)
    {
        return $"{TenantId}/{id}/orig";
    }

    private T GetId<T>(Folder<T> room)
    {
        return room.ProviderEntry && room.RootId.ToString().Contains("sbox") ? room.RootId : room.Id;
    }
}