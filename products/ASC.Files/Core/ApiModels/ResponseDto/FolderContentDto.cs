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

namespace ASC.Files.Core.ApiModels.ResponseDto;

public class FolderContentDto<T>
{
    public List<FileEntryDto> Files { get; set; }
    public List<FileEntryDto> Folders { get; set; }
    public FolderDto<T> Current { get; set; }
    public object PathParts { get; set; }
    public int StartIndex { get; set; }
    public int Count { get; set; }
    public int Total { get; set; }
    public int New { get; set; }

    public FolderContentDto() { }

    public static FolderContentDto<int> GetSample()
    {
        return new FolderContentDto<int>
        {
            Current = FolderDto<int>.GetSample(),
            Files = new List<FileEntryDto>(new[] { FileDto<int>.GetSample(), FileDto<int>.GetSample() }),
            Folders = new List<FileEntryDto>(new[] { FolderDto<int>.GetSample(), FolderDto<int>.GetSample() }),
            PathParts = new
            {
                key = "Key",
                path = "//path//to//folder"
            },

            StartIndex = 0,
            Count = 4,
            Total = 4,
        };
    }
}

[Scope]
public class FolderContentDtoHelper
{
    private readonly FileSecurity _fileSecurity;
    private readonly IDaoFactory _daoFactory;
    private readonly FileDtoHelper _fileDtoHelper;
    private readonly FolderDtoHelper _folderDtoHelper;

    public FolderContentDtoHelper(
        FileSecurity fileSecurity,
        IDaoFactory daoFactory,
        FileDtoHelper fileWrapperHelper,
        FolderDtoHelper folderWrapperHelper)
    {
        _fileSecurity = fileSecurity;
        _daoFactory = daoFactory;
        _fileDtoHelper = fileWrapperHelper;
        _folderDtoHelper = folderWrapperHelper;
    }

    public async Task<FolderContentDto<T>> GetAsync<T>(DataWrapper<T> folderItems, int startIndex)
    {
        var foldersIntWithRightsTask = GetFoldersIntWithRightsAsync<int>();
        var foldersStringWithRightsTask = GetFoldersIntWithRightsAsync<string>();

        var foldersIntWithRights = await foldersIntWithRightsTask;
        var foldersStringWithRights = await foldersStringWithRightsTask;

        var filesTask = GetFilesDto().ToListAsync();
        var foldersTask = GetFoldersDto().ToListAsync();
        var currentTask = _folderDtoHelper.GetAsync(folderItems.FolderInfo);

        var result = new FolderContentDto<T>
        {
            Files = await filesTask,
            Folders = await foldersTask,
            PathParts = folderItems.FolderPathParts,
            StartIndex = startIndex,
            Current = await currentTask,
            Total = folderItems.Total,
            New = folderItems.New
        };

        result.Count = result.Files.Count + result.Folders.Count;

        return result;


        async ValueTask<List<Tuple<FileEntry<T1>, bool>>> GetFoldersIntWithRightsAsync<T1>()
        {
            var ids = folderItems.Entries.OfType<FileEntry<T1>>().Select(r => r.ParentId).Distinct().ToList();
            if (ids.Any())
            {
                var folderDao = _daoFactory.GetFolderDao<T1>();

                return await _fileSecurity.CanReadAsync(folderDao.GetFoldersAsync(ids));
            }

            return new List<Tuple<FileEntry<T1>, bool>>();
        }

        async IAsyncEnumerable<FileEntryDto> GetFilesDto()
        {
            var fileEntries = folderItems.Entries.Where(r => r.FileEntryType == FileEntryType.File);
            foreach (var r in fileEntries)
            {
                if (r is File<int> fol1)
                {
                    yield return await _fileDtoHelper.GetAsync(fol1, foldersIntWithRights);
                }

                if (r is File<string> fol2)
                {
                    yield return await _fileDtoHelper.GetAsync(fol2, foldersStringWithRights);
                }
            }
        }

        async IAsyncEnumerable<FileEntryDto> GetFoldersDto()
        {
            var folderEntries = folderItems.Entries.Where(r => r.FileEntryType == FileEntryType.Folder);

            foreach (var r in folderEntries)
            {
                if (r is Folder<int> fol1)
                {
                    yield return await _folderDtoHelper.GetAsync(fol1, foldersIntWithRights);
                }
                else if (r is Folder<string> fol2)
                {
                    yield return await _folderDtoHelper.GetAsync(fol2, foldersStringWithRights);
                }
            }
        }
    }
}

public class FileEntryWrapperConverter : System.Text.Json.Serialization.JsonConverter<FileEntryDto>
{
    public override FileEntryDto Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            return JsonSerializer.Deserialize<FileDto<int>>(ref reader, options);
        }
        catch (Exception)
        {

        }

        try
        {
            return JsonSerializer.Deserialize<FileDto<string>>(ref reader, options);
        }
        catch (Exception)
        {

        }

        try
        {
            return JsonSerializer.Deserialize<FileDto<string>>(ref reader, options);
        }
        catch (Exception)
        {

        }

        try
        {
            return JsonSerializer.Deserialize<FolderDto<int>>(ref reader, options);
        }
        catch (Exception)
        {

        }

        try
        {
            return JsonSerializer.Deserialize<FolderDto<string>>(ref reader, options);
        }
        catch (Exception)
        {

        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, FileEntryDto value, JsonSerializerOptions options)
    {
        if (value.EntryType == FileEntryType.Folder)
        {
            if (value is FolderDto<string> f1)
            {
                JsonSerializer.Serialize(writer, f1, typeof(FolderDto<string>), options);

                return;
            }

            if (value is FolderDto<int> f2)
            {
                JsonSerializer.Serialize(writer, f2, typeof(FolderDto<int>), options);

                return;
            }
        }
        else
        {
            if (value is FileDto<string> f3)
            {
                JsonSerializer.Serialize(writer, f3, typeof(FileDto<string>), options);

                return;
            }

            if (value is FileDto<int> f4)
            {
                JsonSerializer.Serialize(writer, f4, typeof(FileDto<int>), options);

                return;
            }
        }

        JsonSerializer.Serialize(writer, value, options);
    }
}
