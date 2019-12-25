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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;

using Microsoft.Extensions.Configuration;

namespace ASC.Web.Core.Files
{
    public class FileUtility
    {
        public DbContextManager<FilesDbContext> FilesDbContext { get; set; }
        public FileUtility(IConfiguration configuration, FilesLinkUtility filesLinkUtility, DbContextManager<FilesDbContext> dbContextManager)
        {
            Configuration = configuration;
            FilesLinkUtility = filesLinkUtility;
            FilesDbContext = dbContextManager;
        }

        #region method

        public static string GetFileExtension(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return string.Empty;
            string extension = null;
            try
            {
                extension = Path.GetExtension(fileName);
            }
            catch (Exception)
            {
                var position = fileName.LastIndexOf('.');
                if (0 <= position)
                    extension = fileName.Substring(position).Trim().ToLower();
            }
            return extension == null ? string.Empty : extension.Trim().ToLower();
        }

        public string GetInternalExtension(string fileName)
        {
            var extension = GetFileExtension(fileName);
            return InternalExtension.TryGetValue(GetFileTypeByExtention(extension), out var internalExtension)
                       ? internalExtension
                       : extension;
        }

        public string GetGoogleDownloadableExtension(string googleExtension)
        {
            googleExtension = GetFileExtension(googleExtension);
            if (googleExtension.Equals(".gdraw")) return ".pdf";
            return GetInternalExtension(googleExtension);
        }

        public static string ReplaceFileExtension(string fileName, string newExtension)
        {
            newExtension = string.IsNullOrEmpty(newExtension) ? string.Empty : newExtension;
            return Path.GetFileNameWithoutExtension(fileName) + newExtension;
        }

        public static FileType GetFileTypeByFileName(string fileName)
        {
            return GetFileTypeByExtention(GetFileExtension(fileName));
        }

        public static FileType GetFileTypeByExtention(string extension)
        {
            extension = extension.ToLower();

            if (ExtsDocument.Contains(extension)) return FileType.Document;
            if (ExtsSpreadsheet.Contains(extension)) return FileType.Spreadsheet;
            if (ExtsPresentation.Contains(extension)) return FileType.Presentation;
            if (ExtsImage.Contains(extension)) return FileType.Image;
            if (ExtsArchive.Contains(extension)) return FileType.Archive;
            if (ExtsAudio.Contains(extension)) return FileType.Audio;
            if (ExtsVideo.Contains(extension)) return FileType.Video;

            return FileType.Unknown;
        }

        public bool CanImageView(string fileName)
        {
            return ExtsImagePreviewed.Contains(GetFileExtension(fileName), StringComparer.CurrentCultureIgnoreCase);
        }

        public bool CanMediaView(string fileName)
        {
            return ExtsMediaPreviewed.Contains(GetFileExtension(fileName), StringComparer.CurrentCultureIgnoreCase);
        }

        public bool CanWebView(string fileName)
        {
            return ExtsWebPreviewed.Contains(GetFileExtension(fileName), StringComparer.CurrentCultureIgnoreCase);
        }

        public bool CanWebEdit(string fileName)
        {
            return ExtsWebEdited.Contains(GetFileExtension(fileName), StringComparer.CurrentCultureIgnoreCase);
        }

        public bool CanWebReview(string fileName)
        {
            return ExtsWebReviewed.Contains(GetFileExtension(fileName), StringComparer.CurrentCultureIgnoreCase);
        }

        public bool CanWebRestrictedEditing(string fileName)
        {
            return ExtsWebRestrictedEditing.Contains(GetFileExtension(fileName), StringComparer.CurrentCultureIgnoreCase);
        }

        public bool CanWebComment(string fileName)
        {
            return ExtsWebCommented.Contains(GetFileExtension(fileName), StringComparer.CurrentCultureIgnoreCase);
        }

        public bool CanCoAuhtoring(string fileName)
        {
            return ExtsCoAuthoring.Contains(GetFileExtension(fileName), StringComparer.CurrentCultureIgnoreCase);
        }

        public bool CanIndex(string fileName)
        {
            return extsIndexing.Contains(GetFileExtension(fileName), StringComparer.CurrentCultureIgnoreCase);
        }

        #endregion

        #region member

        private Dictionary<string, List<string>> _extsConvertible;

        public Dictionary<string, List<string>> ExtsConvertible
        {
            get
            {
                if (_extsConvertible == null)
                {
                    _extsConvertible = new Dictionary<string, List<string>>();
                    if (string.IsNullOrEmpty(FilesLinkUtility.DocServiceConverterUrl)) return _extsConvertible;

                    const string databaseId = "files";

                    var dbManager = FilesDbContext.Get(databaseId);
                    var list = dbManager.FilesConverts.Select(r => new { r.Input, r.Ouput }).ToList();


                    list.ForEach(item =>
                        {
                            var input = item.Input;
                            var output = item.Ouput;
                            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(output))
                                return;
                            input = input.ToLower().Trim();
                            output = output.ToLower().Trim();
                            if (!_extsConvertible.ContainsKey(input))
                                _extsConvertible[input] = new List<string>();
                            _extsConvertible[input].Add(output);
                        });
                }
                return _extsConvertible;
            }
        }

        private List<string> _extsUploadable;

        public List<string> ExtsUploadable
        {
            get
            {
                if (_extsUploadable == null)
                {
                    _extsUploadable = new List<string>();
                    _extsUploadable.AddRange(ExtsWebPreviewed);
                    _extsUploadable.AddRange(ExtsWebEdited);
                    _extsUploadable.AddRange(ExtsImagePreviewed);
                    _extsUploadable = _extsUploadable.Distinct().ToList();
                }
                return _extsUploadable;
            }
        }

        private List<string> extsWebPreviewed { get => (Configuration["files.docservice.viewed-docs"] ?? "").Split(new char[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(); }
        private List<string> extsWebEdited { get => (Configuration["files.docservice.edited-docs"] ?? "").Split(new char[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(); }
        private List<string> extsWebReviewed { get => (Configuration["files.docservice.reviewed-docs"] ?? "").Split(new char[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(); }
        private List<string> extsWebRestrictedEditing { get => (Configuration["files.docservice.formfilling-docs"] ?? "").Split(new char[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(); }
        private List<string> extsWebCommented { get => (Configuration["files.docservice.commented-docs"] ?? "").Split(new char[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(); }
        private List<string> extsMustConvert { get => (Configuration["files.docservice.convert-docs"] ?? "").Split(new char[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(); }
        private List<string> extsIndexing { get => (Configuration["files.index.formats"] ?? "").Split(new[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(); }

        public List<string> ExtsImagePreviewed { get => (Configuration["files.viewed-images"] ?? "").Split(new char[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(); }

        public List<string> ExtsMediaPreviewed { get => (Configuration["files.viewed-media"] ?? "").Split(new char[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(); }

        public List<string> ExtsWebPreviewed
        {
            get { return string.IsNullOrEmpty(FilesLinkUtility.DocServiceApiUrl) ? new List<string>() : extsWebPreviewed; }
        }

        public List<string> ExtsWebEdited
        {
            get { return string.IsNullOrEmpty(FilesLinkUtility.DocServiceApiUrl) ? new List<string>() : extsWebEdited; }
        }

        public List<string> ExtsWebEncrypt { get => (Configuration["files.docservice.encrypted-docs"] ?? "").Split(new char[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(); }

        public List<string> ExtsWebReviewed
        {
            get { return string.IsNullOrEmpty(FilesLinkUtility.DocServiceApiUrl) ? new List<string>() : extsWebReviewed; }
        }

        public List<string> ExtsWebRestrictedEditing
        {
            get { return string.IsNullOrEmpty(FilesLinkUtility.DocServiceApiUrl) ? new List<string>() : extsWebRestrictedEditing; }
        }

        public List<string> ExtsWebCommented
        {
            get { return string.IsNullOrEmpty(FilesLinkUtility.DocServiceApiUrl) ? new List<string>() : extsWebCommented; }
        }

        public List<string> ExtsMustConvert
        {
            get { return string.IsNullOrEmpty(FilesLinkUtility.DocServiceConverterUrl) ? new List<string>() : extsMustConvert; }
        }

        public List<string> ExtsCoAuthoring { get => (Configuration["files.docservice.coauthor-docs"] ?? "").Split(new char[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(); }
        public IConfiguration Configuration { get; }
        public FilesLinkUtility FilesLinkUtility { get; }

        public static readonly List<string> ExtsArchive = new List<string>
            {
                ".zip", ".rar", ".ace", ".arc", ".arj",
                ".bh", ".cab", ".enc", ".gz", ".ha",
                ".jar", ".lha", ".lzh", ".pak", ".pk3",
                ".tar", ".tgz", ".gz", ".uu", ".uue", ".xxe",
                ".z", ".zoo"
            };

        public static readonly List<string> ExtsVideo = new List<string>
            {
                ".3gp", ".asf", ".avi", ".f4v",
                ".fla", ".flv", ".m2ts", ".m4v",
                ".mkv", ".mov", ".mp4", ".mpeg",
                ".mpg", ".mts", ".ogv", ".svi",
                ".vob", ".webm", ".wmv"
            };

        public static readonly List<string> ExtsAudio = new List<string>
            {
                ".aac", ".ac3", ".aiff", ".amr",
                ".ape", ".cda", ".flac", ".m4a",
                ".mid", ".mka", ".mp3", ".mpc",
                ".oga", ".ogg", ".pcm", ".ra",
                ".raw", ".wav", ".wma"
            };

        public static readonly List<string> ExtsImage = new List<string>
            {
                ".bmp", ".cod", ".gif", ".ief", ".jpe", ".jpeg", ".jpg",
                ".jfif", ".tiff", ".tif", ".cmx", ".ico", ".pnm", ".pbm",
                ".png", ".ppm", ".rgb", ".svg", ".xbm", ".xpm", ".xwd",
                ".svgt", ".svgy", ".gdraw"
            };

        public static readonly List<string> ExtsSpreadsheet = new List<string>
            {
                ".xls", ".xlsx", ".xlsm",
                ".xlt", ".xltx", ".xltm",
                ".ods", ".fods", ".ots", ".csv",
                ".xlst", ".xlsy", ".xlsb",
                ".gsheet"
            };

        public static readonly List<string> ExtsPresentation = new List<string>
            {
                ".pps", ".ppsx", ".ppsm",
                ".ppt", ".pptx", ".pptm",
                ".pot", ".potx", ".potm",
                ".odp", ".fodp", ".otp",
                ".pptt", ".ppty",
                ".gslides"
            };

        public static readonly List<string> ExtsDocument = new List<string>
            {
                ".doc", ".docx", ".docm",
                ".dot", ".dotx", ".dotm",
                ".odt", ".fodt", ".ott", ".rtf", ".txt",
                ".html", ".htm", ".mht",
                ".pdf", ".djvu", ".fb2", ".epub", ".xps",
                ".doct", ".docy",
                ".gdoc"
            };

        public Dictionary<FileType, string> InternalExtension
        {
            get => new Dictionary<FileType, string>
            {
                { FileType.Document, Configuration["files.docservice.internal-doc"] ?? ".docx" },
                { FileType.Spreadsheet, Configuration["files.docservice.internal-xls"] ?? ".xlsx" },
                { FileType.Presentation, Configuration["files.docservice.internal-ppt"] ?? ".pptx" }
            };
        }

        public enum CsvDelimiter
        {
            None = 0,
            Tab = 1,
            Semicolon = 2,
            Colon = 3,
            Comma = 4,
            Space = 5
        }
        public string SignatureSecret { get => GetSignatureSecret(); }
        public string SignatureHeader { get => GetSignatureHeader(); }

        private string GetSignatureSecret()
        {
            var result = Configuration["files.docservice.secret"] ?? "";

            var regex = new Regex(@"^\s+$");

            if (regex.IsMatch(result))
                result = "";

            return result;
        }

        private string GetSignatureHeader()
        {
            var result = (Configuration["files.docservice.secret.header"] ?? "").Trim();
            if (string.IsNullOrEmpty(result))
                result = "Authorization";
            return result;
        }

        #endregion
    }
}