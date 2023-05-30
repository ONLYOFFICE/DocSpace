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
    private readonly ChannelReader<FileData<int>> _channelReader;

    public ThumbnailBuilderService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<ThumbnailBuilderService> logger,
        ThumbnailSettings settings,
        ChannelReader<FileData<int>> channelReader)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _thumbnailSettings = settings;
        _logger = logger;
        _channelReader = channelReader;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.InformationThumbnailWorkerRunnig();

        stoppingToken.Register(() => _logger.InformationThumbnailWorkerStopping());

        var fetchedData = new List<FileData<int>>();

        _logger.TraceProcedureStart();

        var readers = new List<ChannelReader<FileData<int>>>() {
            _channelReader 
        };

        if (((int)(_thumbnailSettings.MaxDegreeOfParallelism * 0.3)) > 0)
        {
            var splitter = _channelReader.Split(2, (n, i, p) => p.TariffState == TariffState.Paid ? 0 : 1, stoppingToken);
            var premiumChannels = splitter[0].Split((int)(_thumbnailSettings.MaxDegreeOfParallelism * 0.7), null, stoppingToken);
            var freeChannel = splitter[1].Split((int)(_thumbnailSettings.MaxDegreeOfParallelism * 0.3), null, stoppingToken);
            readers = premiumChannels.Union(freeChannel).ToList();
        }

        var tasks = new List<Task>();

        for (var i = 0; i < readers.Count; i++)
        {
            var reader = readers[i];

            tasks.Add(Task.Run(async () =>
            {
                await foreach (var fileData in reader.ReadAllAsync(stoppingToken))
                {
                    await using var scope = _serviceScopeFactory.CreateAsyncScope();

                    var commonLinkUtility = scope.ServiceProvider.GetService<CommonLinkUtility>();
                    commonLinkUtility.ServerUri = fileData.BaseUri;

                    var builder = scope.ServiceProvider.GetService<Builder<int>>();

                    await builder.BuildThumbnail(fileData);
                }
            }, stoppingToken));
        }

        await Task.WhenAll(tasks);
    }


}