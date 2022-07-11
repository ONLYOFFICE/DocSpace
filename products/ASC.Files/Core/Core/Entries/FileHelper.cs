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

using ASC.Common;
using ASC.Web.Core.Files;
using ASC.Web.Files.Utils;

namespace ASC.Files.Core
{
    [Scope]
    public class FileHelper
    {
        private FileTrackerHelper FileTracker { get; set; }

        private FilesLinkUtility FilesLinkUtility { get; set; }

        private FileUtility FileUtility { get; set; }

        private FileConverter FileConverter { get; set; }

        public FileHelper(FileTrackerHelper fileTracker, FilesLinkUtility filesLinkUtility, FileUtility fileUtility, FileConverter fileConverter)
        {
            FileTracker = fileTracker;
            FilesLinkUtility = filesLinkUtility;
            FileUtility = fileUtility;
            FileConverter = fileConverter;
        }

        internal string GetTitle<T>(File<T> file)
        {
            return string.IsNullOrEmpty(file.ConvertedType)
                        ? file.PureTitle
                        : FileUtility.ReplaceFileExtension(file.PureTitle, FileUtility.GetInternalExtension(file.PureTitle));
        }

        internal FileStatus GetFileStatus<T>(File<T> file, ref FileStatus currentStatus)
        {
            if (FileTracker.IsEditing(file.ID))
            {
                currentStatus |= FileStatus.IsEditing;
            }

            if (FileTracker.IsEditingAlone(file.ID))
            {
                currentStatus |= FileStatus.IsEditingAlone;
            }

            if (FileConverter.IsConverting(file))
            {
                currentStatus |= FileStatus.IsConverting;
            }

            return currentStatus;
        }

        public string GetDownloadUrl<T>(FileEntry<T> fileEntry)
        {
            return FilesLinkUtility.GetFileDownloadUrl(fileEntry.ID);
        }
    }
}
