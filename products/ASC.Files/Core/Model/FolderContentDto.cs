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

namespace ASC.Api.Documents;

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
public class FolderContentWrapperHelper
{
    private readonly FileSecurity _fileSecurity;
    private readonly IDaoFactory _daoFactory;
    private readonly FileWrapperHelper _fileWrapperHelper;
    private readonly FolderWrapperHelper _folderWrapperHelper;

    public FolderContentWrapperHelper(
        FileSecurity fileSecurity,
        IDaoFactory daoFactory,
        FileWrapperHelper fileWrapperHelper,
        FolderWrapperHelper folderWrapperHelper)
    {
        _fileSecurity = fileSecurity;
        _daoFactory = daoFactory;
        _fileWrapperHelper = fileWrapperHelper;
        _folderWrapperHelper = folderWrapperHelper;
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
                wrapper = await _fileWrapperHelper.GetAsync(fol1, foldersIntWithRights);
            }
            if (r is File<string> fol2)
            {
                wrapper = await _fileWrapperHelper.GetAsync(fol2, foldersStringWithRights);
            }

            files.Add(wrapper);
        }

        var folderEntries = folderItems.Entries.Where(r => r.FileEntryType == FileEntryType.Folder);
        foreach (var r in folderEntries)
        {
            FileEntryDto wrapper = null;
            if (r is Folder<int> fol1)
            {
                wrapper = await _folderWrapperHelper.GetAsync(fol1, foldersIntWithRights);
            }
            if (r is Folder<string> fol2)
            {
                wrapper = await _folderWrapperHelper.GetAsync(fol2, foldersStringWithRights);
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

        result.Current = await _folderWrapperHelper.GetAsync(folderItems.FolderInfo);
        result.Count = result.Files.Count + result.Folders.Count;
        result.Total = folderItems.Total;
        result.New = folderItems.New;

        return result;


        async ValueTask<List<Tuple<FileEntry<T1>, bool>>> GetFoldersIntWithRightsAsync<T1>()
        {
            var ids = folderItems.Entries.OfType<FileEntry<T1>>().Select(r => r.FolderID).Distinct();
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
