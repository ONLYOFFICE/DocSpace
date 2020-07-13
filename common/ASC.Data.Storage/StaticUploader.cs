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
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Common.Threading.Progress;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Data.Storage.Configuration;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Data.Storage
{
    public class StaticUploader
    {
        private static readonly TaskScheduler Scheduler;
        private static readonly CancellationTokenSource TokenSource;

        private static readonly ICache Cache;
        private static readonly object Locker;

        public IServiceProvider ServiceProvider { get; }
        public TenantManager TenantManager { get; }
        public SettingsManager SettingsManager { get; }
        public StorageSettingsHelper StorageSettingsHelper { get; }

        static StaticUploader()
        {
            Scheduler = new ConcurrentExclusiveSchedulerPair(TaskScheduler.Default, 4).ConcurrentScheduler;
            Cache = AscCache.Memory;
            Locker = new object();
            TokenSource = new CancellationTokenSource();
        }

        public StaticUploader(
            IServiceProvider serviceProvider,
            TenantManager tenantManager,
            SettingsManager settingsManager,
            StorageSettingsHelper storageSettingsHelper)
        {
            ServiceProvider = serviceProvider;
            TenantManager = tenantManager;
            SettingsManager = settingsManager;
            StorageSettingsHelper = storageSettingsHelper;
        }

        public string UploadFile(string relativePath, string mappedPath, Action<string> onComplete = null)
        {
            if (TokenSource.Token.IsCancellationRequested) return null;
            if (!CanUpload()) return null;
            if (!File.Exists(mappedPath)) return null;

            var tenantId = TenantManager.GetCurrentTenant().TenantId;
            UploadOperation uploadOperation;
            var key = GetCacheKey(tenantId.ToString(), relativePath);

            lock (Locker)
            {
                uploadOperation = Cache.Get<UploadOperation>(key);
                if (uploadOperation != null)
                {
                    return !string.IsNullOrEmpty(uploadOperation.Result) ? uploadOperation.Result : string.Empty;
                }

                uploadOperation = new UploadOperation(ServiceProvider, tenantId, relativePath, mappedPath);
                Cache.Insert(key, uploadOperation, DateTime.MaxValue);
            }

            uploadOperation.DoJob();
            onComplete?.Invoke(uploadOperation.Result);

            return uploadOperation.Result;
        }

        public Task<string> UploadFileAsync(string relativePath, string mappedPath, Action<string> onComplete = null)
        {
            var tenantId = TenantManager.GetCurrentTenant().TenantId;
            var task = new Task<string>(() =>
            {
                using var scope = ServiceProvider.CreateScope();
                var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
                tenantManager.SetCurrentTenant(tenantId);
                var staticUploader = scope.ServiceProvider.GetService<StaticUploader>();
                return staticUploader.UploadFile(relativePath, mappedPath, onComplete);
            }, TaskCreationOptions.LongRunning);

            task.ConfigureAwait(false);

            task.Start(Scheduler);

            return task;
        }

        public async void UploadDir(string relativePath, string mappedPath)
        {
            if (!CanUpload()) return;
            if (!Directory.Exists(mappedPath)) return;

            var tenant = TenantManager.GetCurrentTenant();
            var key = typeof(UploadOperationProgress).FullName + tenant.TenantId;
            UploadOperationProgress uploadOperation;

            lock (Locker)
            {
                uploadOperation = Cache.Get<UploadOperationProgress>(key);
                if (uploadOperation != null) return;

                uploadOperation = new UploadOperationProgress(this, relativePath, mappedPath);
                Cache.Insert(key, uploadOperation, DateTime.MaxValue);
            }


            tenant.SetStatus(TenantStatus.Migrating);
            TenantManager.SaveTenant(tenant);

            await uploadOperation.RunJobAsync();

            tenant.SetStatus(Core.Tenants.TenantStatus.Active);
            TenantManager.SaveTenant(tenant);
        }

        public bool CanUpload()
        {
            var current = StorageSettingsHelper.DataStoreConsumer(SettingsManager.Load<CdnStorageSettings>());
            if (current == null || !current.IsSet || (string.IsNullOrEmpty(current["cnamessl"]) && string.IsNullOrEmpty(current["cname"])))
            {
                return false;
            }

            return true;
        }

        public static void Stop()
        {
            TokenSource.Cancel();
        }

        public static UploadOperationProgress GetProgress(int tenantId)
        {
            lock (Locker)
            {
                var key = typeof(UploadOperationProgress).FullName + tenantId;
                return Cache.Get<UploadOperationProgress>(key);
            }
        }

        private static string GetCacheKey(string tenantId, string path)
        {
            return typeof(UploadOperation).FullName + tenantId + path;
        }
    }

    public class UploadOperation
    {
        private readonly ILog Log;
        private readonly int tenantId;
        private readonly string path;
        private readonly string mappedPath;
        public string Result { get; private set; }
        public IServiceProvider ServiceProvider { get; }

        public UploadOperation(IServiceProvider serviceProvider, int tenantId, string path, string mappedPath)
        {
            ServiceProvider = serviceProvider;
            Log = ServiceProvider.GetService<IOptionsMonitor<ILog>>().CurrentValue;
            this.tenantId = tenantId;
            this.path = path.TrimStart('/');
            this.mappedPath = mappedPath;
            Result = string.Empty;
        }

        public string DoJob()
        {
            try
            {
                using var scope = ServiceProvider.CreateScope();
                var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
                var tenant = tenantManager.GetTenant(tenantId);
                tenantManager.SetCurrentTenant(tenant);

                var SecurityContext = scope.ServiceProvider.GetService<SecurityContext>();
                var SettingsManager = scope.ServiceProvider.GetService<SettingsManager>();
                var StorageSettingsHelper = scope.ServiceProvider.GetService<StorageSettingsHelper>();
                SecurityContext.AuthenticateMe(tenant.OwnerId);

                var dataStore = StorageSettingsHelper.DataStore(SettingsManager.Load<CdnStorageSettings>());

                if (File.Exists(mappedPath))
                {
                    if (!dataStore.IsFile(path))
                    {
                        using var stream = File.OpenRead(mappedPath);
                        dataStore.Save(path, stream);
                    }

                    Result = dataStore.GetInternalUri("", path, TimeSpan.Zero, null).AbsoluteUri.ToLower();
                    Log.DebugFormat("UploadFile {0}", Result);
                    return Result;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return null;
        }
    }

    [DataContract]
    public class UploadOperationProgress : ProgressBase
    {
        private readonly string relativePath;
        private readonly string mappedPath;
        private readonly IEnumerable<string> directoryFiles;

        public IServiceProvider ServiceProvider { get; }
        public StaticUploader StaticUploader { get; }

        public UploadOperationProgress(StaticUploader staticUploader, string relativePath, string mappedPath)
        {
            StaticUploader = staticUploader;
            this.relativePath = relativePath;
            this.mappedPath = mappedPath;

            var extensions = ".png|.jpeg|.jpg|.gif|.ico|.swf|.mp3|.ogg|.eot|.svg|.ttf|.woff|.woff2|.css|.less|.js";
            var extensionsArray = extensions.Split('|');

            directoryFiles = Directory.GetFiles(mappedPath, "*", SearchOption.AllDirectories)
                .Where(r => extensionsArray.Contains(Path.GetExtension(r)))
                .ToList();

            StepCount = directoryFiles.Count();
        }

        protected override async Task DoJobAsync()
        {
            var tasks = new List<Task>();
            foreach (var file in directoryFiles)
            {
                var filePath = file.Substring(mappedPath.TrimEnd('/').Length);
                tasks.Add(StaticUploader.UploadFileAsync(Path.Combine(relativePath, filePath), file, (res) => StepDone()));
            }

            await Task.WhenAll(tasks);
        }

        protected override void DoJob()
        {
            foreach (var file in directoryFiles)
            {
                var filePath = file.Substring(mappedPath.TrimEnd('/').Length);
                StaticUploader.UploadFileAsync(Path.Combine(relativePath, filePath), file, (res) => StepDone()).Wait();
            }
        }
    }

    public static class StaticUploaderExtension
    {
        public static DIHelper AddStaticUploaderService(this DIHelper services)
        {
            services.TryAddScoped<StaticUploader>();

            return services
                .AddTenantManagerService()
                .AddCdnStorageSettingsService();
        }
    }
}
