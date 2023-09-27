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

namespace ASC.Files.Core.Helpers;

[Singletone]
public class FileUtilityConfiguration
{
    private readonly IConfiguration _configuration;

    public FileUtilityConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private List<string> _extsIndexing;
    public List<string> ExtsIndexing { get => _extsIndexing ??= _configuration.GetSection("files:index").Get<List<string>>() ?? new List<string>(); }

    private List<string> _extsImagePreviewed;
    public List<string> ExtsImagePreviewed { get => _extsImagePreviewed ??= _configuration.GetSection("files:viewed-images").Get<List<string>>() ?? new List<string>(); }

    private List<string> _extsMediaPreviewed;
    public List<string> ExtsMediaPreviewed { get => _extsMediaPreviewed ??= _configuration.GetSection("files:viewed-media").Get<List<string>>() ?? new List<string>(); }

    private List<string> _extsWebPreviewed;
    public List<string> ExtsWebPreviewed
    {
        get
        {
            return _extsWebPreviewed ??= _configuration.GetSection("files:docservice:viewed-docs").Get<List<string>>() ?? new List<string>();
        }
    }

    private List<string> _extsWebEdited;
    public List<string> ExtsWebEdited
    {
        get
        {
            return _extsWebEdited ??= _configuration.GetSection("files:docservice:edited-docs").Get<List<string>>() ?? new List<string>();
        }
    }

    private List<string> _extsWebEncrypt;
    public List<string> ExtsWebEncrypt { get => _extsWebEncrypt ??= _configuration.GetSection("files:docservice:encrypted-docs").Get<List<string>>() ?? new List<string>(); }

    private List<string> _extsWebReviewed;
    public List<string> ExtsWebReviewed
    {
        get
        {
            return _extsWebReviewed ??= _configuration.GetSection("files:docservice:reviewed-docs").Get<List<string>>() ?? new List<string>();
        }
    }

    private List<string> _extsWebCustomFilterEditing;
    public List<string> ExtsWebCustomFilterEditing
    {
        get
        {
            return _extsWebCustomFilterEditing ??= _configuration.GetSection("files:docservice:customfilter-docs").Get<List<string>>() ?? new List<string>();
        }
    }

    private List<string> _extsWebRestrictedEditing;
    public List<string> ExtsWebRestrictedEditing
    {
        get
        {
            return _extsWebRestrictedEditing ??= _configuration.GetSection("files:docservice:formfilling-docs").Get<List<string>>() ?? new List<string>();
        }
    }

    private List<string> _extsWebCommented;
    public List<string> ExtsWebCommented
    {
        get
        {
            return _extsWebCommented ??= _configuration.GetSection("files:docservice:commented-docs").Get<List<string>>() ?? new List<string>();
        }
    }

    private List<string> _extsWebTemplate;
    public List<string> ExtsWebTemplate
    {
        get
        {
            return _extsWebTemplate ??= _configuration.GetSection("files:docservice:template-docs").Get<List<string>>() ?? new List<string>();
        }
    }

    private List<string> _extsMustConvert;
    public List<string> ExtsMustConvert
    {
        get
        {
            return _extsMustConvert ??= _configuration.GetSection("files:docservice:convert-docs").Get<List<string>>() ?? new List<string>();
        }
    }

    private List<string> _extsCoAuthoring;
    public List<string> ExtsCoAuthoring
    {
        get => _extsCoAuthoring ??= _configuration.GetSection("files:docservice:coauthor-docs").Get<List<string>>() ?? new List<string>();
    }

    private string _masterFormExtension;
    public string MasterFormExtension
    {
        get => _masterFormExtension ??= _configuration["files:docservice:internal-form"] ?? ".docxf";
    }

    private List<LogoColor> _logoColors;
    public List<LogoColor> LogoColors
    {
        get => _logoColors ??= _configuration.GetSection("logocolors").Get<List<LogoColor>>() ?? new List<LogoColor>();
    }

    public Dictionary<FileType, string> InternalExtension
    {
        get => new Dictionary<FileType, string>
            {
                { FileType.Document, _configuration["files:docservice:internal-doc"] ?? ".docx" },
                { FileType.Spreadsheet, _configuration["files:docservice:internal-xls"] ?? ".xlsx" },
                { FileType.Presentation, _configuration["files:docservice:internal-ppt"] ?? ".pptx" }
            };
    }

    internal string GetSignatureSecret()
    {
        var result = _configuration["files:docservice:secret:value"] ?? "";

        var regex = new Regex(@"^\s+$");

        if (regex.IsMatch(result))
        {
            result = "";
        }

        return result;
    }

    internal string GetSignatureHeader()
    {
        var result = (_configuration["files:docservice:secret:header"] ?? "").Trim();
        if (string.IsNullOrEmpty(result))
        {
            result = "Authorization";
        }

        return result;
    }


    internal bool GetCanForcesave()
    {
        return !bool.TryParse(_configuration["files:docservice:forcesave"] ?? "", out var canForcesave) || canForcesave;
    }
}

public class LogoColor
{
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
}

public enum Accessability
{
    ImageView,
    MediaView,
    WebView,
    WebEdit,
    WebReview,
    WebCustomFilterEditing,
    WebRestrictedEditing,
    WebComment,
    CoAuhtoring,
    Convert
}

[Scope]
public class FileUtility
{

    public FileUtility(
        FileUtilityConfiguration fileUtilityConfiguration,
        FilesLinkUtility filesLinkUtility,
        IDbContextFactory<FilesDbContext> dbContextFactory)
    {
        _fileUtilityConfiguration = fileUtilityConfiguration;
        _filesLinkUtility = filesLinkUtility;
        _dbContextFactory = dbContextFactory;
        CanForcesave = GetCanForcesave();
    }

    #region method

    public static string GetFileExtension(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return string.Empty;
        }

        string extension = null;
        try
        {
            extension = Path.GetExtension(fileName);
        }
        catch (Exception)
        {
            var position = fileName.LastIndexOf('.');
            if (0 <= position)
            {
                extension = fileName.Substring(position).Trim().ToLower();
            }
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
        if (googleExtension.Equals(".gdraw"))
        {
            return ".pdf";
        }

        return GetInternalExtension(googleExtension);
    }

    public string GetInternalConvertExtension(string fileName)
    {
        return "ooxml";
    }

    public static string ReplaceFileExtension(string fileName, string newExtension)
    {
        newExtension = string.IsNullOrEmpty(newExtension) ? string.Empty : newExtension;
        return Path.GetFileNameWithoutExtension(fileName)
                 + "." + newExtension.TrimStart('.');
    }

    public static FileType GetFileTypeByFileName(string fileName)
    {
        return GetFileTypeByExtention(GetFileExtension(fileName));
    }

    public static FileType GetFileTypeByExtention(string extension)
    {
        extension = extension.ToLower();

        if (ExtsDocument.Contains(extension))
        {
            return FileType.Document;
        }

        if (ExtsFormTemplate.Contains(extension))
        {
            return FileType.OFormTemplate;
        }

        if (ExtsOForm.Contains(extension))
        {
            return FileType.OForm;
        }

        if (ExtsSpreadsheet.Contains(extension))
        {
            return FileType.Spreadsheet;
        }

        if (ExtsPresentation.Contains(extension))
        {
            return FileType.Presentation;
        }

        if (ExtsImage.Contains(extension))
        {
            return FileType.Image;
        }

        if (ExtsArchive.Contains(extension))
        {
            return FileType.Archive;
        }

        if (ExtsAudio.Contains(extension))
        {
            return FileType.Audio;
        }

        if (ExtsVideo.Contains(extension))
        {
            return FileType.Video;
        }

        return FileType.Unknown;
    }

    public IDictionary<Accessability, bool> GetAccessability(string fileName)
    {
        var result = new Dictionary<Accessability, bool>();

        foreach (var r in Enum.GetValues<Accessability>())
        {
            var val = false;

            switch (r)
            {
                case Accessability.ImageView:
                    val = CanImageView(fileName);
                    break;
                case Accessability.MediaView:
                    val = CanMediaView(fileName);
                    break;
                case Accessability.WebView:
                    val = CanWebView(fileName);
                    break;
                case Accessability.WebEdit:
                    val = CanWebEdit(fileName);
                    break;
                case Accessability.WebReview:
                    val = CanWebReview(fileName);
                    break;
                case Accessability.WebCustomFilterEditing:
                    val = CanWebCustomFilterEditing(fileName);
                    break;
                case Accessability.WebRestrictedEditing:
                    val = CanWebRestrictedEditing(fileName);
                    break;
                case Accessability.WebComment:
                    val = CanWebComment(fileName);
                    break;
                case Accessability.CoAuhtoring:
                    val = CanCoAuhtoring(fileName);
                    break;
                case Accessability.Convert:
                    val = CanConvert(fileName);
                    break;
            }

            result.Add(r, val);
        }

        return result;
    }

    public bool CanImageView(string fileName)
    {
        var ext = GetFileExtension(fileName);
        return ExtsImagePreviewed.Exists(r => r.Equals(ext, StringComparison.OrdinalIgnoreCase));
    }

    public bool CanMediaView(string fileName)
    {
        var ext = GetFileExtension(fileName);
        return ExtsMediaPreviewed.Exists(r => r.Equals(ext, StringComparison.OrdinalIgnoreCase));
    }

    public bool CanWebView(string fileName)
    {
        var ext = GetFileExtension(fileName);
        return ExtsWebPreviewed.Exists(r => r.Equals(ext, StringComparison.OrdinalIgnoreCase));
    }

    public bool CanWebEdit(string fileName)
    {
        var ext = GetFileExtension(fileName);
        return ExtsWebEdited.Exists(r => r.Equals(ext, StringComparison.OrdinalIgnoreCase));
    }

    public bool CanWebReview(string fileName)
    {
        var ext = GetFileExtension(fileName);
        return ExtsWebReviewed.Exists(r => r.Equals(ext, StringComparison.OrdinalIgnoreCase));
    }

    public bool CanWebCustomFilterEditing(string fileName)
    {
        var ext = GetFileExtension(fileName);
        return ExtsWebCustomFilterEditing.Exists(r => r.Equals(ext, StringComparison.OrdinalIgnoreCase));
    }

    public bool CanWebRestrictedEditing(string fileName)
    {
        var ext = GetFileExtension(fileName);
        return ExtsWebRestrictedEditing.Exists(r => r.Equals(ext, StringComparison.OrdinalIgnoreCase));
    }

    public bool CanWebComment(string fileName)
    {
        var ext = GetFileExtension(fileName);
        return ExtsWebCommented.Exists(r => r.Equals(ext, StringComparison.OrdinalIgnoreCase));
    }

    public bool CanCoAuhtoring(string fileName)
    {
        var ext = GetFileExtension(fileName);
        return ExtsCoAuthoring.Exists(r => r.Equals(ext, StringComparison.OrdinalIgnoreCase));
    }


    public bool CanConvert(string fileName)
    {
        var ext = GetFileExtension(fileName);
        return ExtsMustConvert.Exists(r => r.Equals(ext, StringComparison.OrdinalIgnoreCase));
    }

    public bool CanIndex(string fileName)
    {
        var ext = GetFileExtension(fileName);
        return ExtsIndexing.Exists(r => r.Equals(ext, StringComparison.OrdinalIgnoreCase));
    }

    #endregion

    #region member

    private Dictionary<string, List<string>> _extsConvertible;

    public Dictionary<string, List<string>> GetExtsConvertible()
    {
        if (_extsConvertible == null)
        {
            _extsConvertible = new Dictionary<string, List<string>>();
            if (string.IsNullOrEmpty(_filesLinkUtility.DocServiceConverterUrl))
            {
                return _extsConvertible;
            }

            using var filesDbContext = _dbContextFactory.CreateDbContext();
            var list = Queries.Folders(filesDbContext);

            foreach (var item in list)
            {
                var input = item.Input;
                var output = item.Output;
                if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(output))
                {
                    continue;
                }

                input = input.ToLower().Trim();
                output = output.ToLower().Trim();
                if (!_extsConvertible.ContainsKey(input))
                {
                    _extsConvertible[input] = new List<string>();
                }

                _extsConvertible[input].Add(output);
            }
        }
        return _extsConvertible;
    }

    public async Task<Dictionary<string, List<string>>> GetExtsConvertibleAsync()
    {
        if (_extsConvertible == null)
        {
            _extsConvertible = new Dictionary<string, List<string>>();
            if (string.IsNullOrEmpty(_filesLinkUtility.DocServiceConverterUrl))
            {
                return _extsConvertible;
            }

            await using var filesDbContext = _dbContextFactory.CreateDbContext();
            var list = await Queries.FoldersAsync(filesDbContext).ToListAsync();

            foreach (var item in list)
            {
                var input = item.Input;
                var output = item.Output;
                if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(output))
                {
                    continue;
                }

                input = input.ToLower().Trim();
                output = output.ToLower().Trim();
                if (!_extsConvertible.ContainsKey(input))
                {
                    _extsConvertible[input] = new List<string>();
                }

                _extsConvertible[input].Add(output);
            }
        }
        return _extsConvertible;
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

    private List<string> ExtsIndexing { get => _fileUtilityConfiguration.ExtsIndexing; }

    public List<string> ExtsImagePreviewed { get => _fileUtilityConfiguration.ExtsImagePreviewed; }

    public List<string> ExtsMediaPreviewed { get => _fileUtilityConfiguration.ExtsMediaPreviewed; }

    public List<string> ExtsWebPreviewed
    {
        get
        {
            if (string.IsNullOrEmpty(_filesLinkUtility.DocServiceApiUrl))
            {
                return new List<string>();
            }

            return _fileUtilityConfiguration.ExtsWebPreviewed;
        }
    }

    public List<string> ExtsWebEdited
    {
        get
        {
            if (string.IsNullOrEmpty(_filesLinkUtility.DocServiceApiUrl))
            {
                return new List<string>();
            }

            return _fileUtilityConfiguration.ExtsWebEdited;
        }
    }

    public List<string> ExtsWebEncrypt { get => _fileUtilityConfiguration.ExtsWebEncrypt; }

    public List<string> ExtsWebReviewed
    {
        get
        {
            if (string.IsNullOrEmpty(_filesLinkUtility.DocServiceApiUrl))
            {
                return new List<string>();
            }

            return _fileUtilityConfiguration.ExtsWebReviewed;
        }
    }

    public List<string> ExtsWebCustomFilterEditing
    {
        get
        {
            if (string.IsNullOrEmpty(_filesLinkUtility.DocServiceApiUrl))
            {
                return new List<string>();
            }

            return _fileUtilityConfiguration.ExtsWebCustomFilterEditing;
        }
    }

    public List<string> ExtsWebRestrictedEditing
    {
        get
        {
            if (string.IsNullOrEmpty(_filesLinkUtility.DocServiceApiUrl))
            {
                return new List<string>();
            }

            return _fileUtilityConfiguration.ExtsWebRestrictedEditing;
        }
    }

    public List<string> ExtsWebCommented
    {
        get
        {
            if (string.IsNullOrEmpty(_filesLinkUtility.DocServiceApiUrl))
            {
                return new List<string>();
            }

            return _fileUtilityConfiguration.ExtsWebCommented;
        }
    }

    public List<string> ExtsWebTemplate
    {
        get => _fileUtilityConfiguration.ExtsWebTemplate;
    }

    public List<string> ExtsMustConvert
    {
        get
        {
            if (string.IsNullOrEmpty(_filesLinkUtility.DocServiceConverterUrl))
            {
                return new List<string>();
            }

            return _fileUtilityConfiguration.ExtsMustConvert;
        }
    }

    public List<string> ExtsCoAuthoring
    {
        get => _fileUtilityConfiguration.ExtsCoAuthoring;
    }

    private readonly FileUtilityConfiguration _fileUtilityConfiguration;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly IDbContextFactory<FilesDbContext> _dbContextFactory;
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
                ".svgt", ".svgy", ".gdraw", ".webp"
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
                ".html", ".htm", ".mht", ".xml",
                ".pdf", ".djvu", ".fb2", ".epub", ".xps",".oxps",
                ".doct", ".docy",
                ".gdoc"
            };

    public static readonly List<string> ExtsFormTemplate = new List<string>
            {
                ".docxf"
            };

    public static readonly List<string> ExtsOForm = new List<string>
            {
                ".oform"
            };

    public static readonly List<string> ExtsTemplate = new List<string>
            {
                ".ott", ".ots", ".otp",
                ".dot", ".dotm", ".dotx",
                ".xlt", ".xltm", ".xltx",
                ".pot", ".potm", ".potx",
            };
    public Dictionary<FileType, string> InternalExtension => _fileUtilityConfiguration.InternalExtension;

    public string MasterFormExtension { get => _fileUtilityConfiguration.MasterFormExtension; }
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

    private string GetSignatureSecret() => _fileUtilityConfiguration.GetSignatureSecret();

    private string GetSignatureHeader() => _fileUtilityConfiguration.GetSignatureHeader();

    public readonly bool CanForcesave;

    private bool GetCanForcesave() => _fileUtilityConfiguration.GetCanForcesave();

    #endregion
}

static file class Queries
{
    public static readonly Func<FilesDbContext, IEnumerable<FilesConverts>> Folders =
        Microsoft.EntityFrameworkCore.EF.CompileQuery(
            (FilesDbContext ctx) =>
                ctx.FilesConverts);

    public static readonly Func<FilesDbContext, IAsyncEnumerable<FilesConverts>> FoldersAsync =
        Microsoft.EntityFrameworkCore.EF.CompileAsyncQuery(
            (FilesDbContext ctx) =>
                ctx.FilesConverts);
}