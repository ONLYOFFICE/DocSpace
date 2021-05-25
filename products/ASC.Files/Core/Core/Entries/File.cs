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


using System;
using System.Diagnostics;

using ASC.Common;
using ASC.Web.Core.Files;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Core;

namespace ASC.Files.Core
{
    [Flags]
    public enum FileStatus
    {
        None = 0x0,

        IsEditing = 0x1,

        IsNew = 0x2,

        IsConverting = 0x4,

        IsOriginal = 0x8,

        IsEditingAlone = 0x10,

        IsFavorite = 0x20,

        IsTemplate = 0x40
    }

    [Transient]
    [DebuggerDisplay("{Title} ({ID} v{Version})")]
    public class File<T> : FileEntry<T>
    {
        private FileStatus _status;

        public File(Global global,
            FilesLinkUtility filesLinkUtility,
            FileUtility fileUtility,
            FileConverter fileConverter,
            FileTrackerHelper fileTracker)
            : base(global)
        {
            Version = 1;
            VersionGroup = 1;
            FileEntryType = FileEntryType.File;
            FilesLinkUtility = filesLinkUtility;
            FileUtility = fileUtility;
            FileConverter = fileConverter;
            FileTracker = fileTracker;
        }

        public int Version { get; set; }

        public int VersionGroup { get; set; }

        public string Comment { get; set; }

        public string PureTitle
        {
            get { return base.Title; }
            set { base.Title = value; }
        }

        public override string Title
        {
            get
            {
                return string.IsNullOrEmpty(ConvertedType)
                           ? base.Title
                           : FileUtility.ReplaceFileExtension(base.Title, FileUtility.GetInternalExtension(base.Title));
            }
            set { base.Title = value; }
        }

        public long ContentLength { get; set; }

        public string ContentLengthString
        {
            get { return FileSizeComment.FilesSizeToString(ContentLength); }
            set { }
        }

        public FilterType FilterType
        {
            get
            {
                switch (FileUtility.GetFileTypeByFileName(Title))
                {
                    case FileType.Image:
                        return FilterType.ImagesOnly;
                    case FileType.Document:
                        return FilterType.DocumentsOnly;
                    case FileType.Presentation:
                        return FilterType.PresentationsOnly;
                    case FileType.Spreadsheet:
                        return FilterType.SpreadsheetsOnly;
                    case FileType.Archive:
                        return FilterType.ArchiveOnly;
                    case FileType.Audio:
                    case FileType.Video:
                        return FilterType.MediaOnly;
                }

                return FilterType.None;
            }
        }

        public FileStatus FileStatus
        {
            get
            {
                if (FileTracker.IsEditing(ID))
                {
                    _status |= FileStatus.IsEditing;
                }

                if (FileTracker.IsEditingAlone(ID))
                {
                    _status |= FileStatus.IsEditingAlone;
                }

                if (FileConverter.IsConverting(this))
                {
                    _status |= FileStatus.IsConverting;
                }

                return _status;
            }
            set { _status = value; }
        }

        public bool Locked { get; set; }

        public string LockedBy { get; set; }

        public override bool IsNew
        {
            get { return (_status & FileStatus.IsNew) == FileStatus.IsNew; }
            set
            {
                if (value)
                    _status |= FileStatus.IsNew;
                else
                    _status ^= FileStatus.IsNew;
            }
        }

        public bool IsFavorite
        {
            get { return (_status & FileStatus.IsFavorite) == FileStatus.IsFavorite; }
            set
            {
                if (value)
                    _status |= FileStatus.IsFavorite;
                else
                    _status ^= FileStatus.IsFavorite;
            }
        }

        public bool IsTemplate
        {
            get { return (_status & FileStatus.IsTemplate) == FileStatus.IsTemplate; }
            set
            {
                if (value)
                    _status |= FileStatus.IsTemplate;
                else
                    _status ^= FileStatus.IsTemplate;
            }
        }

        public bool Encrypted { get; set; }

        public ForcesaveType Forcesave { get; set; }

        public string DownloadUrl
        {
            get { return FilesLinkUtility.GetFileDownloadUrl(ID); }
        }

        public string ConvertedType { get; set; }

        public string ConvertedExtension
        {
            get
            {
                if (string.IsNullOrEmpty(ConvertedType)) return FileUtility.GetFileExtension(Title);

                var curFileType = FileUtility.GetFileTypeByFileName(Title);
                return curFileType switch
                {
                    FileType.Image => ConvertedType.Trim('.') == "zip" ? ".pptt" : ConvertedType,
                    FileType.Spreadsheet => ConvertedType.Trim('.') != "xlsx" ? ".xlst" : ConvertedType,
                    FileType.Document => ConvertedType.Trim('.') == "zip" ? ".doct" : ConvertedType,
                    _ => ConvertedType,
                };
            }
        }

        public object NativeAccessor { get; set; }

        private FileTrackerHelper FileTracker;

        private readonly FilesLinkUtility FilesLinkUtility;

        private readonly FileUtility FileUtility;

        private readonly FileConverter FileConverter;
    }
}