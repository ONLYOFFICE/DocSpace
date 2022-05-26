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

    public bool IsConvertable(string extension)
    {
        return MustConvertable.Contains(extension.TrimStart('.'));
    }

    public Task<Stream> Convert(Stream inputStream, string inputFormat)
    {
        if (inputStream == null)
        {
            throw new ArgumentException(nameof(inputStream));
        }

        if (string.IsNullOrEmpty(inputFormat))
        {
            throw new ArgumentException(nameof(inputFormat));
        }

        return ConvertInternal(inputStream, inputFormat);
    }

    private async Task<Stream> ConvertInternal(Stream inputStream, string inputFormat)
    {
        var startInfo = PrepareFFmpeg(inputFormat);

        Process process;
        using (process = new Process { StartInfo = startInfo })
        {
            process.Start();

            await StreamCopyToAsync(inputStream, process.StandardInput.BaseStream, closeDst: true);

            await ProcessLog(process.StandardError.BaseStream);

            return process.StandardOutput.BaseStream;
        }
    }

    public FFmpegService(ILogger<FFmpegService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _fFmpegPath = configuration["files:ffmpeg:value"];
        _fFmpegArgs = configuration["files:ffmpeg:args"] ?? "-i - -preset ultrafast -movflags frag_keyframe+empty_moov -f {0} -";

        _convertableMedia = (configuration.GetSection("files:ffmpeg:exts").Get<string[]>() ?? Array.Empty<string>()).ToList();

        if (string.IsNullOrEmpty(_fFmpegPath))
        {
            var pathvar = Environment.GetEnvironmentVariable("PATH");
            var folders = pathvar.Split(WorkContext.IsMono ? ':' : ';').Distinct();
            foreach (var folder in folders)
            {
                if (!Directory.Exists(folder))
                {
                    continue;
                }

                foreach (var name in _fFmpegExecutables)
                {
                    var path = CrossPlatform.PathCombine(folder, WorkContext.IsMono ? name : name + ".exe");
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

    private readonly List<string> _convertableMedia;
    private readonly List<string> _fFmpegExecutables = new List<string>() { "ffmpeg", "avconv" };
    private readonly string _fFmpegPath;
    private readonly string _fFmpegArgs;

    private readonly ILogger<FFmpegService> _logger;

    private ProcessStartInfo PrepareFFmpeg(string inputFormat)
    {
        if (!_convertableMedia.Contains(inputFormat.TrimStart('.')))
        {
            throw new ArgumentException("input format");
        }

        var startInfo = new ProcessStartInfo();

        if (string.IsNullOrEmpty(_fFmpegPath))
        {
            _logger.ErrorFFmpeg();
            throw new Exception("no ffmpeg");
        }

        startInfo.FileName = _fFmpegPath;
        startInfo.WorkingDirectory = Path.GetDirectoryName(_fFmpegPath);
        startInfo.Arguments = string.Format(_fFmpegArgs, "mp4");
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardInput = true;
        startInfo.RedirectStandardError = true;
        startInfo.CreateNoWindow = true;
        startInfo.WindowStyle = ProcessWindowStyle.Normal;

        return startInfo;
    }

    private static Task<int> StreamCopyToAsync(Stream srcStream, Stream dstStream, bool closeSrc = false, bool closeDst = false)
    {
        ArgumentNullException.ThrowIfNull(srcStream);
        ArgumentNullException.ThrowIfNull(dstStream);

        return StreamCopyToAsyncInternal(srcStream, dstStream, closeSrc, closeDst);
    }

    private static async Task<int> StreamCopyToAsyncInternal(Stream srcStream, Stream dstStream, bool closeSrc, bool closeDst)
    {
        const int bufs = 2048 * 4;

        var buffer = new byte[bufs];
        int readed;
        var total = 0;
        while ((readed = await srcStream.ReadAsync(buffer, 0, bufs)) > 0)
        {
            await dstStream.WriteAsync(buffer, 0, readed);
            await dstStream.FlushAsync();
            total += readed;
        }

        if (closeSrc)
        {
            srcStream.Dispose();
            srcStream.Close();
        }

        if (closeDst)
        {
            await dstStream.FlushAsync();
            dstStream.Dispose();
            dstStream.Close();
        }

        return total;
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
}
