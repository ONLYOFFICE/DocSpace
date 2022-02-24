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

namespace ASC.Files.ThumbnailBuilder
{
    [Singletone]
    public class Worker
    {
        private readonly IServiceProvider _serviceProvider;
        private CancellationToken _cancellationToken;
        private readonly ThumbnailSettings _thumbnailSettings;
        private readonly ILog _logger;

        private Timer _timer;

        public Worker(
            IServiceProvider serviceProvider,
            IOptionsMonitor<ILog> options,
            ThumbnailSettings settings)
        {
            _serviceProvider = serviceProvider;
            _thumbnailSettings = settings;
            _logger = options.Get("ASC.Files.ThumbnailBuilder");
        }

        public void Start(CancellationToken cancellationToken)
        {
            _timer = new Timer(Procedure, null, 0, Timeout.Infinite);
            _cancellationToken = cancellationToken;
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                _timer.Dispose();
                _timer = null;
            }
        }

        private void Procedure(object _)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _logger.Trace("Procedure: Start.");

            if (_cancellationToken.IsCancellationRequested)
            {
                Stop();
                return;
            }

            //var configSection = (ConfigSection)ConfigurationManager.GetSection("thumbnailBuilder") ?? new ConfigSection();
            //CommonLinkUtility.Initialize(configSection.ServerRoot, false);


            var filesWithoutThumbnails = Launcher.Queue.Select(pair => pair.Value).ToList();

            if (filesWithoutThumbnails.Count == 0)
            {
                _logger.TraceFormat("Procedure: Waiting for data. Sleep {0}.", _thumbnailSettings.LaunchFrequency);
                _timer.Change(TimeSpan.FromSeconds(_thumbnailSettings.LaunchFrequency), TimeSpan.FromMilliseconds(-1));
                return;
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                var fileDataProvider = scope.ServiceProvider.GetService<FileDataProvider>();
                var builder = scope.ServiceProvider.GetService<BuilderQueue<int>>();
                var premiumTenants = fileDataProvider.GetPremiumTenants();

                filesWithoutThumbnails = filesWithoutThumbnails
                    .OrderByDescending(fileData => Array.IndexOf(premiumTenants, fileData.TenantId))
                    .ToList();

                builder.BuildThumbnails(filesWithoutThumbnails);
            }

            _logger.Trace("Procedure: Finish.");
            _timer.Change(0, Timeout.Infinite);
        }
    }

    public static class WorkerExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<FileDataProvider>();
            services.TryAdd<BuilderQueue<int>>();
            services.TryAdd<Builder<int>>();
        }
    }
}