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

using ASCShare = ASC.Files.Core.Security.FileShare;
using File = System.IO.File;

namespace ASC.Migration.NextcloudWorkspace.Models.Parse;

public class NCMigratingFiles : MigratingFiles
{
    public override int FoldersCount => _foldersCount;

    public override int FilesCount => _filesCount;

    public override long BytesTotal => _bytesTotal;

    public override string ModuleName => MigrationResource.NextcloudModuleNameDocuments;

    private readonly GlobalFolderHelper _globalFolderHelper;
    private readonly IDaoFactory _daoFactory;
    private readonly FileSecurity _fileSecurity;
    private readonly FileStorageService _fileStorageService;
    private readonly NCMigratingUser _user;
    private readonly string _rootFolder;
    private List<NCFileCache> _files;
    private List<NCFileCache> _folders;
    private int _foldersCount;
    private int _filesCount;
    private long _bytesTotal;
    private readonly NCStorages _storages;
    private Dictionary<string, NCMigratingUser> _users;
    private Dictionary<string, NCMigratingGroups> _groups;
    private Dictionary<object, int> _matchingFileId;
    private string _folderCreation;
    public NCMigratingFiles(GlobalFolderHelper globalFolderHelper, IDaoFactory daoFactory, FileSecurity fileSecurity, FileStorageService fileStorageService, NCMigratingUser user, NCStorages storages, string rootFolder, Action<string, Exception> log) : base(log)
    {
        _globalFolderHelper = globalFolderHelper;
        _daoFactory = daoFactory;
        _fileSecurity = fileSecurity;
        _fileStorageService = fileStorageService;
        _user = user;
        _rootFolder = rootFolder;
        _storages = storages;
    }

    public override void Parse()
    {
        var drivePath = Directory.Exists(Path.Combine(_rootFolder, "data", _user.Key, "files")) ?
            Path.Combine(_rootFolder, "data", _user.Key, "files") : null;
        if (drivePath == null)
        {
            return;
        }

        _files = new List<NCFileCache>();
        _folders = new List<NCFileCache>();
        _folderCreation = _folderCreation != null ? _folderCreation : DateTime.Now.ToString("dd.MM.yyyy");
        foreach (var entry in _storages.FileCache)
        {
            var paths = entry.Path.Split('/');
            if (paths[0] != "files")
            {
                continue;
            }

            paths[0] = "NextCloud’s Files " + _folderCreation;
            entry.Path = string.Join("/", paths);

            if (paths.Length >= 1)
            {
                var tmpPath = drivePath;
                for (var i = 1; i < paths.Length; i++)
                {
                    tmpPath = Path.Combine(tmpPath, paths[i]);
                }
                if (Directory.Exists(tmpPath) || File.Exists(tmpPath))
                {
                    var attr = File.GetAttributes(tmpPath);
                    if (attr.HasFlag(FileAttributes.Directory))
                    {
                        _foldersCount++;
                        _folders.Add(entry);
                    }
                    else
                    {
                        _filesCount++;
                        var fi = new FileInfo(tmpPath);
                        _bytesTotal += fi.Length;
                        _files.Add(entry);
                    }
                }
            }
        }
    }

    public override async Task MigrateAsync()
    {
        if (!ShouldImport)
        {
            return;
        }

        var drivePath = Directory.Exists(Path.Combine(_rootFolder, "data", _user.Key, "files")) ?
            Path.Combine(_rootFolder, "data", _user.Key) : null;
        if (drivePath == null)
        {
            return;
        }

        _matchingFileId = new Dictionary<object, int>();
        var foldersDict = new Dictionary<string, Folder<int>>();
        if (_folders != null)
        {
            foreach (var folder in _folders)
            {
                var split = folder.Path.Split('/');
                for (var i = 0; i < split.Length; i++)
                {
                    var path = string.Join(Path.DirectorySeparatorChar.ToString(), split.Take(i + 1));
                    if (foldersDict.ContainsKey(path))
                    {
                        continue;
                    }

                    var parentId = i == 0 ? await _globalFolderHelper.FolderMyAsync : foldersDict[string.Join(Path.DirectorySeparatorChar.ToString(), split.Take(i))].Id;
                    try
                    {
                        var newFolder = await _fileStorageService.CreateNewFolderAsync(parentId, split[i]);
                        foldersDict.Add(path, newFolder);
                        _matchingFileId.Add(newFolder.Id, folder.FileId);
                    }
                    catch (Exception ex)
                    {
                        Log($"Couldn't create folder {path}", ex);
                    }
                }
            }
        }

        if (_files != null)
        {
            foreach (var file in _files)
            {
                var maskPaths = file.Path.Split('/');
                if (maskPaths[0] == "NextCloud’s Files " + DateTime.Now.ToString("dd.MM.yyyy"))
                {
                    maskPaths[0] = "files";
                }
                var maskPath = string.Join(Path.DirectorySeparatorChar.ToString(), maskPaths);
                var parentPath = Path.GetDirectoryName(file.Path);
                try
                {
                    var realPath = Path.Combine(drivePath, maskPath);
                    using var fs = new FileStream(realPath, FileMode.Open);
                    var fileDao = _daoFactory.GetFileDao<int>();
                    var folderDao = _daoFactory.GetFolderDao<int>();
                    {
                        var parentFolder = string.IsNullOrWhiteSpace(parentPath) ? await folderDao.GetFolderAsync(await _globalFolderHelper.FolderMyAsync) : foldersDict[parentPath];

                        var newFile = new File<int>
                        {
                            ParentId = parentFolder.Id,
                            Comment = FilesCommonResource.CommentCreate,
                            Title = Path.GetFileName(file.Path),
                            ContentLength = fs.Length
                        };
                        newFile = await fileDao.SaveFileAsync(newFile, fs);
                        _matchingFileId.Add(newFile.Id, file.FileId);
                    }
                }
                catch (Exception ex)
                {
                    Log($"Couldn't create file {parentPath}/{Path.GetFileName(file.Path)}", ex);
                }
            }
        }

        foreach (var item in _matchingFileId)
        {
            var list = new List<AceWrapper>();
            var entryIsFile = _files.Exists(el => el.FileId == item.Value) ? true : false;
            var entry = entryIsFile ? _files.Find(el => el.FileId == item.Value) : _folders.Find(el => el.FileId == item.Value);
            if (entry.Share.Count == 0)
            {
                continue;
            }

            foreach (var shareInfo in entry.Share)
            {
                if (shareInfo.ShareWith == null)
                {
                    continue;
                }

                var shareType = GetPortalShare(shareInfo.Premissions, entryIsFile);
                _users.TryGetValue(shareInfo.ShareWith, out var userToShare);
                _groups.TryGetValue(shareInfo.ShareWith, out var groupToShare);

                if (userToShare != null || groupToShare != null)
                {
                    var entryGuid = userToShare == null ? groupToShare.Guid : userToShare.Guid;
                    list.Add(new AceWrapper
                    {
                        Access = shareType.Value,
                        Id = entryGuid,
                        SubjectGroup = false
                    });
                }
            }
            if (!list.Any())
            {
                continue;
            }

            var aceCollection = new AceCollection<int>
            {
                Files = new List<int>(),
                Folders = new List<int>(),
                Aces = list,
                Message = null
            };

            if (entryIsFile)
            {
                aceCollection.Files = new List<int>() { (int)item.Key };
            }
            else
            {
                aceCollection.Folders = new List<int>() { (int)item.Key };
            }

            try
            {
                await _fileStorageService.SetAceObjectAsync(aceCollection, false);
            }
            catch (Exception ex)
            {
                Log($"Couldn't change file permissions for {item.Key}", ex);
            }
        }
    }

    public void SetUsersDict(IEnumerable<NCMigratingUser> users)
    {
        this._users = users.ToDictionary(user => user.Key, user => user);
    }

    public void SetGroupsDict(IEnumerable<NCMigratingGroups> groups)
    {
        this._groups = groups.ToDictionary(group => group.GroupName, group => group);
    }

    private ASCShare? GetPortalShare(int role, bool entryType)
    {
        if (entryType)
        {
            if (role == 1 || role == 17)
            {
                return ASCShare.Read;
            }

            return ASCShare.ReadWrite;//permission = 19 => denySharing = true, permission = 3 => denySharing = false; ASCShare.ReadWrite
        }
        else
        {
            if (Array.Exists(new int[] { 1, 17, 9, 25, 5, 21, 13, 29, 3, 19, 11, 27 }, el => el == role))
            {
                return ASCShare.Read;
            }

            return ASCShare.ReadWrite;//permission = 19||23 => denySharing = true, permission = 7||15 => denySharing = false; ASCShare.ReadWrite
        }
    }
}
