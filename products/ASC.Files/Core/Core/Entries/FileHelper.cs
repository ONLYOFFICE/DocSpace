/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
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

using System;

using ASC.Common;
using ASC.Web.Core.Files;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Utils;

namespace ASC.Files.Core
{
    [Transient]
    public class FileHelper<T>
    {
        private FileTrackerHelper FileTracker { get; set; }

        private FilesLinkUtility FilesLinkUtility { get; set; }

        private FileUtility FileUtility { get; set; }

        private FileConverter FileConverter { get; set; }

        private Global Global { get; set; }

        public FileEntry FileEntry { get; set; }

        public FileHelper(FileTrackerHelper fileTracker, FilesLinkUtility filesLinkUtility, FileUtility fileUtility, FileConverter fileConverter, Global global)
        {
            FileTracker = fileTracker;
            FilesLinkUtility = filesLinkUtility;
            FileUtility = fileUtility;
            FileConverter = fileConverter;
            Global = global;
        }

        public string CreateByString
        {
            get { return !FileEntry.CreateBy.Equals(Guid.Empty) ? Global.GetUserName(FileEntry.CreateBy) : FileEntry._createByString; }
        }

        public string ModifiedByString
        {
            get { return !FileEntry.ModifiedBy.Equals(Guid.Empty) ? Global.GetUserName(FileEntry.ModifiedBy) : FileEntry._modifiedByString; }
        }

        public string Title
        {
            get
            {
                var File = (File<T>)FileEntry;
                return string.IsNullOrEmpty(File.ConvertedType)
                           ? File.Title
                           : FileUtility.ReplaceFileExtension(File.Title, FileUtility.GetInternalExtension(File.Title));
            }
        }

        public FileStatus FileStatus
        {
            get
            {

                var File = (File<T>)FileEntry;
                if (FileTracker.IsEditing(File.ID))
                {
                    File._status |= FileStatus.IsEditing;
                }

                if (FileTracker.IsEditingAlone(File.ID))
                {
                    File._status |= FileStatus.IsEditingAlone;
                }

                if (FileConverter.IsConverting(File))
                {
                    File._status |= FileStatus.IsConverting;
                }

                return File._status;
            }
        }

        public string DownloadUrl
        {
            get { return FilesLinkUtility.GetFileDownloadUrl(((FileEntry<T>)FileEntry).ID); }
        }
    }
}
