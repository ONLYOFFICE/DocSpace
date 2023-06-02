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

namespace ASC.EventBus.Extensions.Logger.Services;

public class IntegrationEventLogService : IIntegrationEventLogService
{
    private readonly List<Type> _eventTypes;
    private readonly IDbContextFactory<IntegrationEventLogContext> _dbContextFactory;

    public IntegrationEventLogService(IDbContextFactory<IntegrationEventLogContext> dbContextFactory)
    {
        _eventTypes = Assembly.Load(Assembly.GetEntryAssembly().FullName)
            .GetTypes()
            .Where(t => t.Name.EndsWith(nameof(IntegrationEvent)))
            .ToList();
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId)
    {
        var tid = transactionId.ToString();

        using var _integrationEventLogContext = _dbContextFactory.CreateDbContext();
        var result = await _integrationEventLogContext.IntegrationEventLogs
            .Where(e => e.TransactionId == tid && e.State == EventStateEnum.NotPublished).ToListAsync();

        if (result != null && result.Any())
        {
            return result.OrderBy(o => o.CreateOn)
                .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventTypeShortName)));
        }

        return new List<IntegrationEventLogEntry>();
    }

    public async Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException(nameof(transaction));
        }

        var eventLogEntry = new IntegrationEventLogEntry(@event, transaction.TransactionId);

        using var _integrationEventLogContext = _dbContextFactory.CreateDbContext();
        _integrationEventLogContext.Database.UseTransaction(transaction.GetDbTransaction());
        _integrationEventLogContext.IntegrationEventLogs.Add(eventLogEntry);

        await _integrationEventLogContext.SaveChangesAsync();
    }

    public async Task MarkEventAsPublishedAsync(Guid eventId)
    {
        await UpdateEventStatusAsync(eventId, EventStateEnum.Published);
    }

    public async Task MarkEventAsInProgressAsync(Guid eventId)
    {
        await UpdateEventStatusAsync(eventId, EventStateEnum.InProgress);
    }

    public async Task MarkEventAsFailedAsync(Guid eventId)
    {
        await UpdateEventStatusAsync(eventId, EventStateEnum.PublishedFailed);
    }

    private async Task UpdateEventStatusAsync(Guid eventId, EventStateEnum status)
    {
        using var _integrationEventLogContext = _dbContextFactory.CreateDbContext();
        var eventLogEntry = _integrationEventLogContext.IntegrationEventLogs.Single(ie => ie.EventId == eventId);
        eventLogEntry.State = status;

        if (status == EventStateEnum.InProgress)
        {
            eventLogEntry.TimesSent++;
        }

        _integrationEventLogContext.IntegrationEventLogs.Update(eventLogEntry);

        await _integrationEventLogContext.SaveChangesAsync();
    }
}
