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

namespace ASC.Files.Thirdparty.Box;

[Scope]
internal class BoxDaoBase : ThirdPartyProviderDao<BoxFile, BoxFolder, BoxItem>, IDaoBase<BoxFile, BoxFolder, BoxItem>
{
    private BoxProviderInfo _providerInfo;
    public BoxDaoBase(IServiceProvider serviceProvider,
        UserManager userManager,
        TenantManager tenantManager,
        TenantUtil tenantUtil, 
        IDbContextFactory<FilesDbContext> dbContextFactory,
        SetupInfo setupInfo,
        FileUtility fileUtility,
        TempPath tempPath, 
        AuthContext authContext,
        RegexDaoSelectorBase<BoxFile, BoxFolder, BoxItem> regexDaoSelectorBase) : base(serviceProvider, userManager, tenantManager, tenantUtil, dbContextFactory, setupInfo, fileUtility, tempPath, authContext, regexDaoSelectorBase)
    {
    }

    public void Init(string pathPrefix, IProviderInfo<BoxFile, BoxFolder, BoxItem> providerInfo)
    {
        PathPrefix = pathPrefix;
        ProviderInfo = providerInfo;
        _providerInfo = providerInfo as BoxProviderInfo;
    }

    public string GetName(BoxItem item)
    {
        return item.Name;
    }
    public string GetId(BoxItem item)
    {
        return item.Id;
    }

    public string MakeThirdId(object entryId)
    {
        var id = Convert.ToString(entryId, CultureInfo.InvariantCulture);

        return string.IsNullOrEmpty(id)
                   ? "0"
                   : id.TrimStart('/');
    }

    public string GetParentFolderId(BoxItem boxItem)
    {
        return boxItem == null || boxItem.Parent == null
                   ? null
                   : boxItem.Parent.Id;
    }

    public string MakeId(BoxItem boxItem)
    {
        var path = string.Empty;
        if (boxItem != null)
        {
            path = boxItem.Id;
        }

        return MakeId(path);
    }

    public override string MakeId(string path = null)
    {
        var p = string.IsNullOrEmpty(path) || path == "0" ? "" : ("-|" + path.TrimStart('/'));

        return $"{PathPrefix}{p}";
    }

    public string MakeFolderTitle(BoxFolder boxFolder)
    {
        if (boxFolder == null || IsRoot(boxFolder))
        {
            return ProviderInfo.CustomerTitle;
        }

        return Global.ReplaceInvalidCharsAndTruncate(boxFolder.Name);
    }

    public string MakeFileTitle(BoxFile boxFile)
    {
        if (boxFile == null || string.IsNullOrEmpty(boxFile.Name))
        {
            return ProviderInfo.ProviderKey;
        }

        return Global.ReplaceInvalidCharsAndTruncate(boxFile.Name);
    }

    public Folder<string> ToFolder(BoxFolder boxFolder)
    {
        if (boxFolder == null)
        {
            return null;
        }

        if (boxFolder is ErrorFolder)
        {
            //Return error entry
            return ToErrorFolder(boxFolder as ErrorFolder);
        }

        var isRoot = IsRoot(boxFolder);

        var folder = GetFolder();

        folder.Id = MakeThirdId(boxFolder.Id);
        folder.ParentId = isRoot ? null : MakeThirdId(GetParentFolderId(boxFolder));
        folder.CreateOn = isRoot ? ProviderInfo.CreateOn : (boxFolder.CreatedAt?.UtcDateTime ?? default);
        folder.ModifiedOn = isRoot ? ProviderInfo.CreateOn : (boxFolder.ModifiedAt?.UtcDateTime ?? default);

        folder.Title = MakeFolderTitle(boxFolder);
        folder.FilesCount = boxFolder.ItemCollection != null ? boxFolder.ItemCollection.Entries.Count(item => item is BoxFile) : 0;
        folder.FoldersCount = boxFolder.ItemCollection != null ? boxFolder.ItemCollection.Entries.Count(item => item is BoxFolder) : 0;
        folder.Private = ProviderInfo.Private;
        folder.HasLogo = ProviderInfo.HasLogo;
        SetFolderType(folder, isRoot);

        if (folder.CreateOn != DateTime.MinValue && folder.CreateOn.Kind == DateTimeKind.Utc)
        {
            folder.CreateOn = _tenantUtil.DateTimeFromUtc(folder.CreateOn);
        }

        if (folder.ModifiedOn != DateTime.MinValue && folder.ModifiedOn.Kind == DateTimeKind.Utc)
        {
            folder.ModifiedOn = _tenantUtil.DateTimeFromUtc(folder.ModifiedOn);
        }

        return folder;
    }

    public bool IsRoot(BoxFolder boxFolder)
    {
        return boxFolder.Id == "0";
    }

    private File<string> ToErrorFile(ErrorFile boxFile)
    {
        if (boxFile == null)
        {
            return null;
        }

        var file = GetErrorFile(new ErrorEntry(boxFile.Error, boxFile.ErrorId));

        file.Title = MakeFileTitle(boxFile);

        return file;
    }

    private Folder<string> ToErrorFolder(ErrorFolder boxFolder)
    {
        if (boxFolder == null)
        {
            return null;
        }

        var folder = GetErrorFolder(new ErrorEntry(boxFolder.Error, boxFolder.ErrorId));

        folder.Title = MakeFolderTitle(boxFolder);

        return folder;
    }

    public File<string> ToFile(BoxFile boxFile)
    {
        if (boxFile == null)
        {
            return null;
        }

        if (boxFile is ErrorFile)
        {
            //Return error entry
            return ToErrorFile(boxFile as ErrorFile);
        }

        var file = GetFile();

        file.Id = MakeThirdId(boxFile.Id);
        file.ContentLength = boxFile.Size.HasValue ? (long)boxFile.Size : 0;
        file.CreateOn = boxFile.CreatedAt.HasValue ? _tenantUtil.DateTimeFromUtc(boxFile.CreatedAt.Value.UtcDateTime) : default;
        file.ParentId = MakeThirdId(GetParentFolderId(boxFile));
        file.ModifiedOn = boxFile.ModifiedAt.HasValue ? _tenantUtil.DateTimeFromUtc(boxFile.ModifiedAt.Value.UtcDateTime) : default;
        file.NativeAccessor = boxFile;
        file.Title = MakeFileTitle(boxFile);
        file.ThumbnailStatus = Thumbnail.Created;
        file.Encrypted = ProviderInfo.Private;

        return file;
    }

    public async Task<Folder<string>> GetRootFolderAsync()
    {
        return ToFolder(await GetFolderAsync("0"));
    }

    public async Task<BoxFolder> GetFolderAsync(string folderId)
    {
        var boxFolderId = MakeThirdId(folderId);
        try
        {
            var folder = await _providerInfo.GetFolderAsync(boxFolderId);

            return folder;
        }
        catch (Exception ex)
        {
            return new ErrorFolder(ex, boxFolderId);
        }
    }

    public Task<BoxFile> GetFileAsync(string fileId)
    {
        var boxFileId = MakeThirdId(fileId);
        try
        {
            var file = _providerInfo.GetFileAsync(boxFileId);

            return file;
        }
        catch (Exception ex)
        {
            return Task.FromResult<BoxFile>(new ErrorFile(ex, boxFileId));
        }
    }

    public override async Task<IEnumerable<string>> GetChildrenAsync(string folderId)
    {
        var items = await GetItemsAsync(folderId);

        return items.Select(entry => MakeThirdId(entry.Id));
    }

    public async Task<List<BoxItem>> GetItemsAsync(string parentId, bool? folder = null)
    {
        var boxFolderId = MakeThirdId(parentId);
        var items = await _providerInfo.GetItemsAsync(boxFolderId);

        if (folder.HasValue)
        {
            if (folder.Value)
            {
                return items.Where(i => i is BoxFolder).ToList();
            }

            return items.Where(i => i is BoxFile).ToList();
        }

        return items;
    }

    protected sealed class ErrorFolder : BoxFolder, IErrorItem
    {
        public string Error { get; set; }
        public string ErrorId { get; private set; }

        public ErrorFolder(Exception e, object id)
        {
            ErrorId = id.ToString();
            if (e != null)
            {
                Error = e.Message;
            }
        }
    }

    protected sealed class ErrorFile : BoxFile, IErrorItem
    {
        public string Error { get; set; }
        public string ErrorId { get; private set; }

        public ErrorFile(Exception e, object id)
        {
            ErrorId = id.ToString();
            if (e != null)
            {
                Error = e.Message;
            }
        }
    }

    public async Task<string> GetAvailableTitleAsync(string requestTitle, string parentFolderId, Func<string, string, Task<bool>> isExist)
    {
        if (!await isExist(requestTitle, parentFolderId))
        {
            return requestTitle;
        }

        var re = new Regex(@"( \(((?<index>[0-9])+)\)(\.[^\.]*)?)$");
        var match = re.Match(requestTitle);

        if (!match.Success)
        {
            var insertIndex = requestTitle.Length;
            if (requestTitle.LastIndexOf(".", StringComparison.InvariantCulture) != -1)
            {
                insertIndex = requestTitle.LastIndexOf(".", StringComparison.InvariantCulture);
            }

            requestTitle = requestTitle.Insert(insertIndex, " (1)");
        }

        while (!await isExist(requestTitle, parentFolderId))
        {
            requestTitle = re.Replace(requestTitle, MatchEvaluator);
        }

        return requestTitle;
    }

    private string MatchEvaluator(Match match)
    {
        var index = Convert.ToInt32(match.Groups[2].Value);
        var staticText = match.Value.Substring(string.Format(" ({0})", index).Length);

        return string.Format(" ({0}){1}", index + 1, staticText);
    }
}
