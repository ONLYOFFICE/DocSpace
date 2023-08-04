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

namespace ASC.Thumbnail.IntegrationEvents.EventHandling;

[Scope]
public class ThumbnailRequestedIntegrationEventHandler : IIntegrationEventHandler<ThumbnailRequestedIntegrationEvent>
{
    private readonly ILogger _logger;
    private readonly ChannelWriter<FileData<int>> _channelWriter;
    private readonly ITariffService _tariffService;
    private readonly IDbContextFactory<FilesDbContext> _dbContextFactory;

    private ThumbnailRequestedIntegrationEventHandler() : base()
    {

    }

    public ThumbnailRequestedIntegrationEventHandler(
        ILogger<ThumbnailRequestedIntegrationEventHandler> logger,
        IDbContextFactory<FilesDbContext> dbContextFactory,
        ITariffService tariffService,
        ChannelWriter<FileData<int>> channelWriter)
    {
        _logger = logger;
        _channelWriter = channelWriter;
        _tariffService = tariffService;
        _dbContextFactory = dbContextFactory;
    }

    private async Task<IEnumerable<FileData<int>>> GetFreezingThumbnailsAsync()
    {
        await using var filesDbContext = _dbContextFactory.CreateDbContext();

        var files = await Queries.DbFilesAsync(filesDbContext).ToListAsync();

        if (files.Count == 0)
        {
            return new List<FileData<int>>();
        }

        foreach (var f in files)
        {
            f.ThumbnailStatus = Files.Core.Thumbnail.Waiting;
        }

        await filesDbContext.SaveChangesAsync();

        return await files.ToAsyncEnumerable().SelectAwait(async r =>
        {
            var tariff = await _tariffService.GetTariffAsync(r.TenantId);
            var fileData = new FileData<int>(r.TenantId, r.Id, "", tariff.State);

            return fileData;
        }).ToListAsync();
    }


    public async Task Handle(ThumbnailRequestedIntegrationEvent @event)
    {
        CustomSynchronizationContext.CreateContext();
        using (_logger.BeginScope(new[] { new KeyValuePair<string, object>("integrationEventContext", $"{@event.Id}-{Program.AppName}") }))
        {
            _logger.InformationHandlingIntegrationEvent(@event.Id, Program.AppName, @event);

            var tariff = await _tariffService.GetTariffAsync(@event.TenantId);
            var freezingThumbnails = await GetFreezingThumbnailsAsync();
            var data = @event.FileIds.Select(fileId => new FileData<int>(@event.TenantId, Convert.ToInt32(fileId), @event.BaseUrl, tariff.State))
                          .Union(freezingThumbnails);


            if (await _channelWriter.WaitToWriteAsync())
            {
                foreach (var item in data)
                {
                    await _channelWriter.WriteAsync(item);
                }
            }
        }
    }
}


static file class Queries
{
    public static readonly Func<FilesDbContext, IAsyncEnumerable<DbFile>> DbFilesAsync =
        EF.CompileAsyncQuery(
            (FilesDbContext ctx) =>
                ctx.Files
                    .Where(r => r.CurrentVersion && r.ThumbnailStatus == Files.Core.Thumbnail.Creating &&
                                EF.Functions.DateDiffMinute(r.ModifiedOn, DateTime.UtcNow) > 5));
}