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

using System.Threading.Channels;

namespace ASC.Files.ThumbnailBuilder;

[Singletone]
public class ThumbnailBuilderService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ThumbnailSettings _thumbnailSettings;
    private readonly ILogger<ThumbnailBuilderService> _logger;
    private readonly BuilderQueue<int> _builderQueue;
    private readonly ChannelReader<IEnumerable<FileData<int>>> _channelReader;

    public ThumbnailBuilderService(
        BuilderQueue<int> builderQueue,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<ThumbnailBuilderService> logger,
        ThumbnailSettings settings,
        ChannelReader<IEnumerable<FileData<int>>> channelReader)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _thumbnailSettings = settings;
        _logger = logger;
        _builderQueue = builderQueue;
        _channelReader = channelReader;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.InformationThumbnailWorkerRunnig();

        stoppingToken.Register(() => _logger.InformationThumbnailWorkerStopping());

        var fetchedData = new List<FileData<int>>();

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.TraceProcedureStart();

            await foreach (var rawData in _channelReader.ReadAllAsync(stoppingToken))
            {
                fetchedData.AddRange(rawData);

                if (_channelReader.CanCount && _channelReader.Count > 0 && _thumbnailSettings.BatchSize >= fetchedData.Count())
                {
                    continue;
                }

                await _builderQueue.BuildThumbnails(fetchedData);

                fetchedData = new List<FileData<int>>();
            }
        }

        _logger.InformationThumbnailWorkerStopping();
    }
}