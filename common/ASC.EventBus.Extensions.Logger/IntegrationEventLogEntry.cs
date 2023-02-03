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

namespace ASC.EventBus.Extensions.Logger;
public class IntegrationEventLogEntry
{
    private IntegrationEventLogEntry() { }
    public IntegrationEventLogEntry(IntegrationEvent @event, Guid transactionId)
    {
        EventId = @event.Id;
        CreateOn = @event.CreateOn;
        CreateBy = @event.CreateBy;
        TenantId = @event.TenantId;
        EventTypeName = @event.GetType().FullName;
        Content = JsonSerializer.Serialize(@event, @event.GetType(), new JsonSerializerOptions
        {
            WriteIndented = true
        });
        State = EventStateEnum.NotPublished;
        TimesSent = 0;
        TransactionId = transactionId.ToString();
    }
    public Guid EventId { get; private set; }

    public string EventTypeName { get; private set; }

    [NotMapped]
    public string EventTypeShortName => EventTypeName.Split('.')?.Last();

    [NotMapped]
    public IntegrationEvent IntegrationEvent { get; private set; }
    public EventStateEnum State { get; set; }
    public int TimesSent { get; set; }
    public DateTime CreateOn { get; private set; }
    public Guid CreateBy { get; private set; }
    public string Content { get; private set; }
    public string TransactionId { get; private set; }
    public int TenantId { get; private set; }

    public DbTenant Tenant { get; private set; }

    public IntegrationEventLogEntry DeserializeJsonContent(Type type)
    {
        IntegrationEvent = JsonSerializer.Deserialize(Content, type, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) as IntegrationEvent;
        return this;
    }
}
