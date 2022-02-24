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

namespace ASC.Files.ThumbnailBuilder;

[Singletone]
public class ThumbnailBuilderService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ThumbnailSettings _thumbnailSettings;
    private readonly ILog _logger;
    private readonly BuilderQueue<int> _builderQueue;

    public ThumbnailBuilderService(
        BuilderQueue<int> builderQueue,
        IServiceScopeFactory serviceScopeFactory,
        IOptionsMonitor<ILog> options,
        ThumbnailSettings settings)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _thumbnailSettings = settings;
        _logger = options.Get("ASC.Files.ThumbnailBuilder");
        _builderQueue = builderQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.Info("Thumbnail Worker running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Procedure(stoppingToken);
        }

        _logger.Info("Thumbnail Worker is stopping.");
    }

    private async Task Procedure(CancellationToken stoppingToken)
    {
        _logger.Trace("Procedure: Start.");

        if (stoppingToken.IsCancellationRequested)
        {
            return;
        }

        //var configSection = (ConfigSection)ConfigurationManager.GetSection("thumbnailBuilder") ?? new ConfigSection();
        //CommonLinkUtility.Initialize(configSection.ServerRoot, false);

        var filesWithoutThumbnails = FileDataQueue.Queue.Select(pair => pair.Value).ToList();

        if (filesWithoutThumbnails.Count == 0)
        {
            _logger.TraceFormat("Procedure: Waiting for data. Sleep {0}.", _thumbnailSettings.LaunchFrequency);

            await Task.Delay(TimeSpan.FromSeconds(_thumbnailSettings.LaunchFrequency), stoppingToken);

            return;
        }

        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var fileDataProvider = scope.ServiceProvider.GetService<FileDataProvider>();
            var premiumTenants = fileDataProvider.GetPremiumTenants();

            filesWithoutThumbnails = filesWithoutThumbnails
                .OrderByDescending(fileData => Array.IndexOf(premiumTenants, fileData.TenantId))
                .ToList();

            _builderQueue.BuildThumbnails(filesWithoutThumbnails);
        }

        _logger.Trace("Procedure: Finish.");
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