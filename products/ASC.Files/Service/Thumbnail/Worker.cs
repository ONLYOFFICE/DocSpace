/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


using System;
using System.Linq;
using System.Threading;

using ASC.Common;
using ASC.Common.Logging;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Files.ThumbnailBuilder
{
    internal class Worker
    {
        private readonly IServiceProvider serviceProvider;
        private readonly CancellationToken cancellationToken;

        private Timer timer;

        public Worker(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            this.serviceProvider = serviceProvider;
            this.cancellationToken = cancellationToken;
        }

        public void Start()
        {
            timer = new Timer(Procedure, null, 0, Timeout.Infinite);
        }

        public void Stop()
        {
            if (timer != null)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer.Dispose();
                timer = null;
            }
        }


        private void Procedure(object _)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Stop();
                return;
            }

            using var scope = serviceProvider.CreateScope();
            var fileDataProvider = scope.ServiceProvider.GetService<FileDataProvider>();
            var logger = scope.ServiceProvider.GetService<IOptionsMonitor<ILog>>().Get("ASC.Files.ThumbnailBuilder");
            var builder = scope.ServiceProvider.GetService<BuilderQueue<int>>();
            var config = scope.ServiceProvider.GetService<ThumbnailSettings>();

            //            var configSection = (ConfigSection)ConfigurationManager.GetSection("thumbnailBuilder") ?? new ConfigSection();
            //CommonLinkUtility.Initialize(configSection.ServerRoot, false);
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            logger.Trace("Procedure: Start.");

            var filesWithoutThumbnails = Launcher.Queue.Select(pair => pair.Value).ToList();

            if (!filesWithoutThumbnails.Any())
            {
                logger.TraceFormat("Procedure: Waiting for data. Sleep {0}.", config.LaunchFrequency);
                timer.Change(TimeSpan.FromSeconds(config.LaunchFrequency), TimeSpan.FromMilliseconds(-1));
                return;
            }

            var premiumTenants = fileDataProvider.GetPremiumTenants();

            filesWithoutThumbnails = filesWithoutThumbnails
                .OrderByDescending(fileData => Array.IndexOf(premiumTenants, fileData.TenantId))
                .ToList();

            builder.BuildThumbnails(filesWithoutThumbnails);

            logger.Trace("Procedure: Finish.");
            timer.Change(0, Timeout.Infinite);
        }
    }

    public class WorkerExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<FileDataProvider>();
            services.TryAdd<BuilderQueue<int>>();
            services.TryAdd<Builder<int>>();
            services.TryAdd<ThumbnailSettings>();
        }
    }
}