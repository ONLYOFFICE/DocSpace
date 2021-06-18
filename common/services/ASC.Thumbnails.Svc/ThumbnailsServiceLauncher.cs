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
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Utils;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ASC.Thumbnails.Svc
{
    [Singletone]
    public class ThumbnailsServiceLauncher : IHostedService
    {
        private ProcessStartInfo StartInfo { get; set; }
        private Process Proc { get; set; }
        private ILog Logger { get; set; }
        private ConfigurationExtension Configuration { get; set; }
        private IHostEnvironment HostEnvironment { get; set; }

        public ThumbnailsServiceLauncher(IOptionsMonitor<ILog> options, ConfigurationExtension configuration, IHostEnvironment hostEnvironment)
        {
            Logger = options.CurrentValue;
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var settings = Configuration.GetSetting<ThumbnailsSettings>("thumb");

                StartInfo = new ProcessStartInfo
                {
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    FileName = "node",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = string.Format("\"{0}\"", Path.GetFullPath(CrossPlatform.PathCombine(HostEnvironment.ContentRootPath, settings.Path, "index.js"))),
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                };

                var savePath = settings.SavePath;
                if (!savePath.EndsWith("/"))
                {
                    savePath += "/";
                }
                StartInfo.EnvironmentVariables.Add("port", settings.Port);
                StartInfo.EnvironmentVariables.Add("logPath", CrossPlatform.PathCombine(Logger.LogDirectory, "web.thumbnails.log"));
                StartInfo.EnvironmentVariables.Add("savePath", Path.GetFullPath(savePath));

                StartNode(cancellationToken);
            }
            catch (Exception e)
            {
                Logger.Error("Start", e);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (Proc != null && !Proc.HasExited)
                {
                    Proc.Kill();
                    Proc.WaitForExit(10000);

                    Proc.Close();
                    Proc.Dispose();
                    Proc = null;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Stop", e);
            }
            return Task.CompletedTask;
        }

        private void StartNode(CancellationToken cancellationToken)
        {
            StopAsync(cancellationToken);
            Proc = Process.Start(StartInfo);
        }
    }
}