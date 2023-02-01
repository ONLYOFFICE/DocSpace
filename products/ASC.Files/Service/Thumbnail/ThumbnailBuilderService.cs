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

namespace ASC.Files.ThumbnailBuilder;

[Singletone]
public class ThumbnailBuilderService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ThumbnailSettings _thumbnailSettings;
    private readonly ILogger<ThumbnailBuilderService> _logger;
    private readonly BuilderQueue<int> _builderQueue;

    public ThumbnailBuilderService(
        BuilderQueue<int> builderQueue,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<ThumbnailBuilderService> logger,
        ThumbnailSettings settings)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _thumbnailSettings = settings;
        _logger = logger;
        _builderQueue = builderQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.InformationThumbnailWorkerRunnig();

        while (!stoppingToken.IsCancellationRequested)
        {
            await Procedure(stoppingToken);
        }

        _logger.InformationThumbnailWorkerStopping();
    }

    private async Task Procedure(CancellationToken stoppingToken)
    {
        _logger.TraceProcedureStart();

        if (stoppingToken.IsCancellationRequested)
        {
            return;
        }

        //var configSection = (ConfigSection)ConfigurationManager.GetSection("thumbnailBuilder") ?? new ConfigSection();
        //CommonLinkUtility.Initialize(configSection.ServerRoot, false);
        
        await using (var scope = _serviceScopeFactory.CreateAsyncScope())
        {
            var fileDataProvider = scope.ServiceProvider.GetService<FileDataProvider>();

            var filesWithoutThumbnails = FileDataQueue.Queue.Select(pair => pair.Value)
                                                      .ToList();

            filesWithoutThumbnails.AddRange(await fileDataProvider.GetFreezingThumbnailsAsync());

            if (filesWithoutThumbnails.Count == 0)
            {
                _logger.TraceProcedureWaiting(_thumbnailSettings.LaunchFrequency);

                await Task.Delay(TimeSpan.FromSeconds(_thumbnailSettings.LaunchFrequency), stoppingToken);

                return;
            }

            var premiumTenants = fileDataProvider.GetPremiumTenants();

            filesWithoutThumbnails = filesWithoutThumbnails
                .OrderByDescending(fileData => Array.IndexOf(premiumTenants, fileData.TenantId))
                .ToList();

            await _builderQueue.BuildThumbnails(filesWithoutThumbnails);
        }

        _logger.TraceProcedureFinish();
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