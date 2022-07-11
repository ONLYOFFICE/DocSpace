/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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
using System.Diagnostics;
using System.IO;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Module;
using ASC.Common.Utils;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;


namespace ASC.UrlShortener.Svc
{
    [Singletone]
    public class UrlShortenerService : IServiceController
    {
        private readonly ILog log;

        private readonly IConfiguration configuration;

        private readonly IHostEnvironment hostEnvironment;
        private readonly ConfigurationExtension configurationExtension;

        private Process process;

        public UrlShortenerService(
            IOptionsMonitor<ILog> options,
            IConfiguration config,
            IHostEnvironment host,
            ConfigurationExtension configurationExtension)
        {
            log = options.Get("ASC.UrlShortener.Svc");

            configuration = config;

            hostEnvironment = host;
            this.configurationExtension = configurationExtension;
        }

        public void Start()
        {
            try
            {
                Stop();

                var processStartInfo = GetProcessStartInfo();
                process = Process.Start(processStartInfo);
            }
            catch (Exception e)
            {
                log.Fatal("Start", e);
            }
        }

        public void Stop()
        {
            try
            {
                if (process == null || process.HasExited)
                {
                    return;
                }

                process.Kill();
                process.WaitForExit(10000);

                process.Close();
                process.Dispose();
                process = null;
            }
            catch (Exception e)
            {
                log.Error("stop", e);
            }
        }

        private ProcessStartInfo GetProcessStartInfo()
        {
            var path = configuration["urlshortener:path"] ?? "../../ASC.UrlShortener/index.js";
            var port = configuration["urlshortener:port"] ?? "9999";

            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = "node",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = $"\"{Path.GetFullPath(CrossPlatform.PathCombine(hostEnvironment.ContentRootPath, path))}\"",
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
            };

            startInfo.EnvironmentVariables.Add("core.machinekey", configuration["core:machinekey"]);

            startInfo.EnvironmentVariables.Add("port", port);

            var conString = configurationExtension.GetConnectionStrings()["default"].ConnectionString;

            var dict = new Dictionary<string, string>
                {
                    {"Server", "host"},
                    {"Database", "database"},
                    {"User ID", "user"},
                    {"Password", "password"}
                };

            foreach (var conf in conString.Split(';'))
            {
                var splited = conf.Split('=');

                if (splited.Length < 2) continue;

                if (dict.TryGetValue(splited[0], out var value))
                {
                    startInfo.EnvironmentVariables.Add("sql:" + dict[value], splited[1]);
                }
            }

            startInfo.EnvironmentVariables.Add("logPath", CrossPlatform.PathCombine(log.LogDirectory, "web.urlshortener.log"));

            return startInfo;
        }
    }
}
