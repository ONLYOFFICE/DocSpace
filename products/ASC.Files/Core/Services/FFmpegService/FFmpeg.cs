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

using System.Runtime.InteropServices;

using File = System.IO.File;

namespace ASC.Web.Files.Services.FFmpegService;

[Singletone]
public class FFmpegService
{
    public List<string> MustConvertable
    {
        get
        {
            if (string.IsNullOrEmpty(_fFmpegPath))
            {
                return new List<string>();
            }

            return _convertableMedia;
        }
    }

    private readonly List<string> _convertableMedia;
    private readonly List<string> _fFmpegExecutables = new List<string>() { "ffmpeg", "avconv" };
    private readonly string _fFmpegPath;
    private readonly string _fFmpegArgs;
    private readonly string _fFmpegThumbnailsArgs;
    private readonly List<string> _fFmpegFormats;

    private readonly ILogger<FFmpegService> _logger;

    public bool IsConvertable(string extension)
    {
        return MustConvertable.Contains(extension.TrimStart('.'));
    }

    public bool ExistFormat(string extension)
    {
        return _fFmpegFormats.Contains(extension);
    }

    public async ValueTask<Stream> ConvertAsync(Stream inputStream, string inputFormat)
    {
        if (inputStream == null)
        {
            throw new ArgumentException(nameof(inputStream));
        }

        if (string.IsNullOrEmpty(inputFormat))
        {
            throw new ArgumentException(nameof(inputFormat));
        }

        var startInfo = PrepareFFmpeg(inputFormat);

        using var process = Process.Start(startInfo);

        await inputStream.CopyToAsync(process.StandardInput.BaseStream);

        await ProcessLog(process.StandardError.BaseStream);

        return process.StandardOutput.BaseStream;
    }

    public FFmpegService(ILogger<FFmpegService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _fFmpegPath = configuration["files:ffmpeg:value"];
        _fFmpegArgs = configuration["files:ffmpeg:args"] ?? "-i - -preset ultrafast -movflags frag_keyframe+empty_moov -f {0} -";
        _fFmpegThumbnailsArgs = configuration["files:ffmpeg:thumbnails:args"] ?? "-i \"{0}\" -frames:v 1 \"{1}\" -y";
        _fFmpegFormats = configuration.GetSection("files:ffmpeg:thumbnails:formats").Get<List<string>>() ?? FileUtility.ExtsVideo;

        _convertableMedia = (configuration.GetSection("files:ffmpeg:exts").Get<string[]>() ?? Array.Empty<string>()).ToList();

        if (string.IsNullOrEmpty(_fFmpegPath))
        {
            var pathvar = Environment.GetEnvironmentVariable("PATH");
            var folders = pathvar.Split(Path.PathSeparator).Distinct();

            foreach (var folder in folders)
            {
                if (!Directory.Exists(folder))
                {
                    continue;
                }
                
                foreach (var name in _fFmpegExecutables)
                {
                    var path = CrossPlatform.PathCombine(folder, RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? name + ".exe" : name);

                    if (File.Exists(path))
                    {
                        _fFmpegPath = path;
                        _logger.InformationFFmpegFoundIn(path);

                        break;
                    }
                }

                if (!string.IsNullOrEmpty(_fFmpegPath))
                {
                    break;
                }
            }
        }
    }

    private ProcessStartInfo PrepareFFmpeg(string inputFormat)
    {
        if (!_convertableMedia.Contains(inputFormat.TrimStart('.')))
        {
            throw new ArgumentException("input format");
        }

        var startInfo = PrepareCommonFFmpeg();

        startInfo.Arguments = string.Format(_fFmpegArgs, "mp4");

        return startInfo;
    }

    private ProcessStartInfo PrepareCommonFFmpeg()
    {
        var startInfo = new ProcessStartInfo();

        if (string.IsNullOrEmpty(_fFmpegPath))
        {
            _logger.ErrorFFmpeg();
            throw new Exception("no ffmpeg");
        }

        startInfo.FileName = _fFmpegPath;
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardInput = true;
        startInfo.RedirectStandardError = true;
        startInfo.CreateNoWindow = true;
        startInfo.WindowStyle = ProcessWindowStyle.Normal;
        return startInfo;
    }

    private async Task ProcessLog(Stream stream)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8);
        string line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            _logger.Information(line);
        }
    }

    public async Task CreateThumbnail(string sourcePath, string destPath)
    {
        var startInfo = PrepareCommonFFmpeg();

        startInfo.Arguments = string.Format(_fFmpegThumbnailsArgs, sourcePath, destPath);

        using var process = Process.Start(startInfo);

        await ProcessLog(process.StandardError.BaseStream);

        process.WaitForExit();
    }
}
