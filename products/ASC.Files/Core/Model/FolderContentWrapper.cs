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

namespace ASC.Api.Documents
{
    /// <summary>
    /// </summary>
    public class FolderContentWrapper<T>
    {
        /// <summary>
        /// </summary>
        public List<FileEntryWrapper> Files { get; set; }

        /// <summary>
        /// </summary>
        public List<FileEntryWrapper> Folders { get; set; }

        /// <summary>
        /// </summary>
        public FolderWrapper<T> Current { get; set; }

        /// <summary>
        /// </summary>
        public object PathParts { get; set; }

        /// <summary>
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// </summary>
        public int Total { get; set; }

        public int New { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="folderItems"></param>
        /// <param name="startIndex"></param>
        public FolderContentWrapper()
        {

        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static FolderContentWrapper<int> GetSample()
        {
            return new FolderContentWrapper<int>
            {
                Current = FolderWrapper<int>.GetSample(),
                Files = new List<FileEntryWrapper>(new[] { FileWrapper<int>.GetSample(), FileWrapper<int>.GetSample() }),
                Folders = new List<FileEntryWrapper>(new[] { FolderWrapper<int>.GetSample(), FolderWrapper<int>.GetSample() }),
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
        private FileSecurity FileSecurity { get; }
        private IDaoFactory DaoFactory { get; }
        private FileWrapperHelper FileWrapperHelper { get; }
        private FolderWrapperHelper FolderWrapperHelper { get; }

        public FolderContentWrapperHelper(
            FileSecurity fileSecurity,
            IDaoFactory daoFactory,
            FileWrapperHelper fileWrapperHelper,
            FolderWrapperHelper folderWrapperHelper)
        {
            FileSecurity = fileSecurity;
            DaoFactory = daoFactory;
            FileWrapperHelper = fileWrapperHelper;
            FolderWrapperHelper = folderWrapperHelper;
        }

        public FolderContentWrapper<T> Get<T>(DataWrapper<T> folderItems, int startIndex)
        {
            var foldersIntWithRights = GetFoldersIntWithRights<int>();
            var foldersStringWithRights = GetFoldersIntWithRights<string>();

            var result = new FolderContentWrapper<T>
            {
                Files = folderItems.Entries
                .Where(r => r.FileEntryType == FileEntryType.File)
                .Select(r =>
                {
                    FileEntryWrapper wrapper = null;
                    if (r is File<int> fol1)
                    {
                        wrapper = FileWrapperHelper.Get(fol1, foldersIntWithRights);
                    }
                    if (r is File<string> fol2)
                    {
                        wrapper = FileWrapperHelper.Get(fol2, foldersStringWithRights);
                    }

                    return wrapper;
                }
                )
                .ToList(),
                Folders = folderItems.Entries
                .Where(r => r.FileEntryType == FileEntryType.Folder)
                .Select(r =>
                {
                    FileEntryWrapper wrapper = null;
                    if (r is Folder<int> fol1)
                    {
                        wrapper = FolderWrapperHelper.Get(fol1, foldersIntWithRights);
                    }
                    if (r is Folder<string> fol2)
                    {
                        wrapper = FolderWrapperHelper.Get(fol2, foldersStringWithRights);
                    }

                    return wrapper;
                }
                ).ToList(),
                PathParts = folderItems.FolderPathParts,
                StartIndex = startIndex
            };

            result.Current = FolderWrapperHelper.Get(folderItems.FolderInfo);
            result.Count = result.Files.Count + result.Folders.Count;
            result.Total = folderItems.Total;
            result.New = folderItems.New;

            return result;


            List<Tuple<FileEntry<T1>, bool>> GetFoldersIntWithRights<T1>()
            {
                var folderDao = DaoFactory.GetFolderDao<T1>();
                var folders = folderDao.GetFolders(folderItems.Entries.OfType<FileEntry<T1>>().Select(r => r.FolderID).Distinct().ToList());
                return FileSecurity.CanRead(folders);
            }
        }
    }

    public class FileEntryWrapperConverter : System.Text.Json.Serialization.JsonConverter<FileEntryWrapper>
    {
        public override FileEntryWrapper Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return null;
        }

        public override void Write(Utf8JsonWriter writer, FileEntryWrapper value, JsonSerializerOptions options)
        {
            if (value is FolderWrapper<string> f1)
            {
                JsonSerializer.Serialize(writer, f1, typeof(FolderWrapper<string>), options);
                return;
            }

            if (value is FolderWrapper<int> f2)
            {
                JsonSerializer.Serialize(writer, f2, typeof(FolderWrapper<int>), options);
                return;
            }

            if (value is FileWrapper<string> f3)
            {
                JsonSerializer.Serialize(writer, f3, typeof(FileWrapper<string>), options);
                return;
            }

            if (value is FileWrapper<int> f4)
            {
                JsonSerializer.Serialize(writer, f4, typeof(FileWrapper<int>), options);
                return;
            }

            JsonSerializer.Serialize(writer, value, options);
        }
    }
}