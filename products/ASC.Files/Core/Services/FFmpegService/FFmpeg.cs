using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ASC.Web.Files.Services.FFmpegService
{
    [Singletone]
    public class FFmpegService
    {
        public List<string> MustConvertable
        {
            get
            {
                if (string.IsNullOrEmpty(FFmpegPath)) return new List<string>();
                return ConvertableMedia;
            }
        }

        public bool IsConvertable(string extension)
        {
            return MustConvertable.Contains(extension.TrimStart('.'));
        }

        public Task<Stream> Convert(Stream inputStream, string inputFormat)
        {
            if (inputStream == null) throw new ArgumentException();
            if (string.IsNullOrEmpty(inputFormat)) throw new ArgumentException();

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

        public FFmpegService(IOptionsMonitor<ILog> optionsMonitor, IConfiguration configuration)
        {
            logger = optionsMonitor.CurrentValue;
            FFmpegPath = configuration["files:ffmpeg:value"];
            FFmpegArgs = configuration["files:ffmpeg:args"] ?? "-i - -preset ultrafast -movflags frag_keyframe+empty_moov -f {0} -";

            ConvertableMedia = (configuration.GetSection("files:ffmpeg:exts").Get<string[]>() ?? Array.Empty<string>()).ToList();

            if (string.IsNullOrEmpty(FFmpegPath))
            {
                var pathvar = Environment.GetEnvironmentVariable("PATH");
                var folders = pathvar.Split(WorkContext.IsMono ? ':' : ';').Distinct();
                foreach (var folder in folders)
                {
                    if (!Directory.Exists(folder)) continue;

                    foreach (var name in FFmpegExecutables)
                    {
                        var path = CrossPlatform.PathCombine(folder, WorkContext.IsMono ? name : name + ".exe");
                        if (File.Exists(path))
                        {
                            FFmpegPath = path;
                            logger.InfoFormat("FFmpeg found in {0}", path);
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(FFmpegPath)) break;
                }
            }
        }

        private readonly List<string> ConvertableMedia;
        private readonly List<string> FFmpegExecutables = new List<string>() { "ffmpeg", "avconv" };
        private readonly string FFmpegPath;
        private readonly string FFmpegArgs;

        private readonly ILog logger;

        private ProcessStartInfo PrepareFFmpeg(string inputFormat)
        {
            if (!ConvertableMedia.Contains(inputFormat.TrimStart('.'))) throw new ArgumentException("input format");

            var startInfo = new ProcessStartInfo();

            if (string.IsNullOrEmpty(FFmpegPath))
            {
                logger.Error("FFmpeg/avconv was not found in PATH or 'files.ffmpeg' setting");
                throw new Exception("no ffmpeg");
            }

            startInfo.FileName = FFmpegPath;
            startInfo.WorkingDirectory = Path.GetDirectoryName(FFmpegPath);
            startInfo.Arguments = string.Format(FFmpegArgs, "mp4");
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
            if (srcStream == null) throw new ArgumentNullException(nameof(srcStream));
            if (dstStream == null) throw new ArgumentNullException(nameof(dstStream));

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
                logger.Info(line);
            }
        }
    }
}