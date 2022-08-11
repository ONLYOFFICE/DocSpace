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
        var foldersIntWithRights = await GetFoldersIntWithRightsAsync<int>();
        var foldersStringWithRights = await GetFoldersIntWithRightsAsync<string>();
        var files = new List<FileEntryDto>();
        var folders = new List<FileEntryDto>();

        var fileEntries = folderItems.Entries.Where(r => r.FileEntryType == FileEntryType.File);
        foreach (var r in fileEntries)
        {
            FileEntryDto wrapper = null;
            if (r is File<int> fol1)
            {
                wrapper = await _fileDtoHelper.GetAsync(fol1, foldersIntWithRights);
            }
            else if (r is File<string> fol2)
            {
                wrapper = await _fileDtoHelper.GetAsync(fol2, foldersStringWithRights);
            }

            files.Add(wrapper);
        }

        var folderEntries = folderItems.Entries.Where(r => r.FileEntryType == FileEntryType.Folder);

        foreach (var r in folderEntries)
        {
            FileEntryDto wrapper = null;
            if (r is Folder<int> fol1)
            {
                wrapper = await _folderDtoHelper.GetAsync(fol1, foldersIntWithRights);
            }
            else if (r is Folder<string> fol2)
            {
                wrapper = await _folderDtoHelper.GetAsync(fol2, foldersStringWithRights);
            }

            folders.Add(wrapper);
        }

        var result = new FolderContentDto<T>
        {
            Files = files,
            Folders = folders,
            PathParts = folderItems.FolderPathParts,
            StartIndex = startIndex
        };

        result.Current = await _folderDtoHelper.GetAsync(folderItems.FolderInfo);
        result.Count = result.Files.Count + result.Folders.Count;
        result.Total = folderItems.Total;
        result.New = folderItems.New;

        return result;


        async ValueTask<List<Tuple<FileEntry<T1>, bool>>> GetFoldersIntWithRightsAsync<T1>()
        {
            var ids = folderItems.Entries.OfType<FileEntry<T1>>().Select(r => r.ParentId).Distinct();
            if (ids.Any())
            {
                var folderDao = _daoFactory.GetFolderDao<T1>();
                var folders = await folderDao.GetFoldersAsync(ids).ToListAsync();

                return await _fileSecurity.CanReadAsync(folders);
            }

            return new List<Tuple<FileEntry<T1>, bool>>();
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

        JsonSerializer.Serialize(writer, value, options);
    }
}
