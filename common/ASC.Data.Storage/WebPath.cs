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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Data.Storage.Configuration;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ASC.Data.Storage
{
    [Singletone]
    public class WebPathSettings
    {
        private readonly IEnumerable<Appender> Appenders;


        public WebPathSettings(Configuration.Storage storage)
        {
            var section = storage;
            if (section != null)
            {
                Appenders = section.Appender;
            }
        }

        public string GetRelativePath(HttpContext httpContext, IOptionsMonitor<ILog> options, string absolutePath)
        {
            if (!Uri.IsWellFormedUriString(absolutePath, UriKind.Absolute))
            {
                throw new ArgumentException($"bad path format {absolutePath} is not absolute");
            }

            var appender = Appenders.FirstOrDefault(x => absolutePath.Contains(x.Append) || (absolutePath.Contains(x.AppendSecure) && !string.IsNullOrEmpty(x.AppendSecure)));
            if (appender == null)
            {
                return absolutePath;
            }
            return SecureHelper.IsSecure(httpContext, options) && !string.IsNullOrEmpty(appender.AppendSecure) ?
                absolutePath.Remove(0, appender.AppendSecure.Length) :
                absolutePath.Remove(0, appender.Append.Length);
        }

        public string GetPath(HttpContext httpContext, IOptionsMonitor<ILog> options, string relativePath)
        {
            if (!string.IsNullOrEmpty(relativePath) && relativePath.IndexOf('~') == 0)
            {
                throw new ArgumentException($"bad path format {relativePath} remove '~'", nameof(relativePath));
            }

            var result = relativePath;
            var ext = Path.GetExtension(relativePath).ToLowerInvariant();

            if (Appenders.Any())
            {
                var avaliableAppenders = Appenders.Where(x => x.Extensions != null && x.Extensions.Split('|').Contains(ext) || string.IsNullOrEmpty(ext)).ToList();
                var avaliableAppendersCount = avaliableAppenders.Count;

                Appender appender;
                if (avaliableAppendersCount > 1)
                {
                    appender = avaliableAppenders[relativePath.Length % avaliableAppendersCount];
                }
                else if (avaliableAppendersCount == 1)
                {
                    appender = avaliableAppenders.First();
                }
                else
                {
                    appender = Appenders.First();
                }

                if (appender.Append.IndexOf('~') == 0)
                {
                    var query = string.Empty;
                    //Rel path
                    if (relativePath.IndexOfAny(new[] { '?', '=', '&' }) != -1)
                    {
                        //Cut it
                        query = relativePath.Substring(relativePath.IndexOf('?'));
                        relativePath = relativePath.Substring(0, relativePath.IndexOf('?'));
                    }
                    //if (HostingEnvironment.IsHosted)
                    //{
                    //    result = VirtualPathUtility.ToAbsolute(string.Format("{0}/{1}{2}", appender.Append.TrimEnd('/'), relativePath.TrimStart('/'), query));
                    //}
                    //else
                    //{
                    result = $"{appender.Append.TrimEnd('/').TrimStart('~')}/{relativePath.TrimStart('/')}{query}";
                    //}
                }
                else
                {
                    //TODO HostingEnvironment.IsHosted
                    if (SecureHelper.IsSecure(httpContext, options) && !string.IsNullOrEmpty(appender.AppendSecure))
                    {
                        result = $"{appender.AppendSecure.TrimEnd('/')}/{relativePath.TrimStart('/')}";
                    }
                    else
                    {
                        //Append directly
                        result = $"{appender.Append.TrimEnd('/')}/{relativePath.TrimStart('/')}";
                    }
                }
            }

            return result;
        }
    }

    [Scope]
    public class WebPath
    {
        private static readonly IDictionary<string, bool> Existing = new ConcurrentDictionary<string, bool>();

        private WebPathSettings WebPathSettings { get; }
        public IServiceProvider ServiceProvider { get; }
        private SettingsManager SettingsManager { get; }
        private StorageSettingsHelper StorageSettingsHelper { get; }
        private IHttpContextAccessor HttpContextAccessor { get; }
        public IHostEnvironment HostEnvironment { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private IOptionsMonitor<ILog> Options { get; }
        private IHttpClientFactory ClientFactory { get; }

        public WebPath(
            WebPathSettings webPathSettings,
            IServiceProvider serviceProvider,
            SettingsManager settingsManager,
            StorageSettingsHelper storageSettingsHelper,
            IHostEnvironment hostEnvironment,
            CoreBaseSettings coreBaseSettings,
            IOptionsMonitor<ILog> options,
            IHttpClientFactory clientFactory)
        {
            WebPathSettings = webPathSettings;
            ServiceProvider = serviceProvider;
            SettingsManager = settingsManager;
            StorageSettingsHelper = storageSettingsHelper;
            HostEnvironment = hostEnvironment;
            CoreBaseSettings = coreBaseSettings;
            Options = options;
            ClientFactory = clientFactory;
        }

        public WebPath(
            WebPathSettings webPathSettings,
            IServiceProvider serviceProvider,
            StaticUploader staticUploader,
            SettingsManager settingsManager,
            StorageSettingsHelper storageSettingsHelper,
            IHttpContextAccessor httpContextAccessor,
            IHostEnvironment hostEnvironment,
            CoreBaseSettings coreBaseSettings,
            IOptionsMonitor<ILog> options,
            IHttpClientFactory clientFactory)
            : this(webPathSettings, serviceProvider, settingsManager, storageSettingsHelper, hostEnvironment, coreBaseSettings, options, clientFactory)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public Task<string> GetPathAsync(string relativePath)
        {
            if (!string.IsNullOrEmpty(relativePath) && relativePath.IndexOf('~') == 0)
            {
                throw new ArgumentException($"bad path format {relativePath} remove '~'", nameof(relativePath));
            }

            return InternalGetPathAsync(relativePath);
        }

        private async Task<string> InternalGetPathAsync(string relativePath)
        {
            if (CoreBaseSettings.Standalone && ServiceProvider.GetService<StaticUploader>().CanUpload()) //hack for skip resolve DistributedTaskQueueOptionsManager
            {
                try
                {
                    var uri = await StorageSettingsHelper.DataStore(SettingsManager.Load<CdnStorageSettings>()).GetInternalUriAsync("", relativePath, TimeSpan.Zero, null);
                    var result = uri.AbsoluteUri.ToLower();
                    if (!string.IsNullOrEmpty(result)) return result;
                }
                catch (Exception)
                {

                }
            }

            return WebPathSettings.GetPath(HttpContextAccessor?.HttpContext, Options, relativePath);
        }

        public async Task<bool> ExistsAsync(string relativePath)
        {
            var path = await GetPathAsync(relativePath);
            if (!Existing.ContainsKey(path))
            {
                if (Uri.IsWellFormedUriString(path, UriKind.Relative) && HttpContextAccessor?.HttpContext != null)
                {
                    //Local
                    Existing[path] = File.Exists(CrossPlatform.PathCombine(HostEnvironment.ContentRootPath, path));
                }
                if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
                {
                    //Make request
                    Existing[path] = CheckWebPath(path);
                }
            }
            return Existing[path];
        }

        private bool CheckWebPath(string path)
        {
            try
            {
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(path);
                request.Method = HttpMethod.Head;
                var httpClient = ClientFactory.CreateClient();
                using var response = httpClient.Send(request);

                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}