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

namespace ASC.EventBus.ActiveMQ.Log;
internal static partial class EventBusActiveMQLogger
{
    [LoggerMessage(Level = LogLevel.Warning, Message = "Could not publish event: {eventId} after {timeout}s")]
    public static partial void WarningCouldNotPublishEvent(this ILogger<EventBusActiveMQ> logger, Guid eventId, double timeout, Exception exception);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Creating ActiveMQ session to publish event: {eventId} ({eventName})")]
    public static partial void TraceCreatingActiveMQSession(this ILogger<EventBusActiveMQ> logger, Guid eventId, string eventName);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Declaring ActiveMQ exchange to publish event: {eventId}")]
    public static partial void TraceDeclaringActiveMQSession(this ILogger<EventBusActiveMQ> logger, Guid eventId);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Publishing event to ActiveMQ: {eventId}")]
    public static partial void TracePublishingEvent(this ILogger<EventBusActiveMQ> logger, Guid eventId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Subscribing to dynamic event {eventName} with {eventHandler}")]
    public static partial void InformationSubscribingDynamic(this ILogger<EventBusActiveMQ> logger, string eventName, string eventHandler);

    [LoggerMessage(Level = LogLevel.Information, Message = "Subscribing to event {eventName} with {eventHandler}")]
    public static partial void InformationSubscribing(this ILogger<EventBusActiveMQ> logger, string eventName, string eventHandler);

    [LoggerMessage(Level = LogLevel.Information, Message = "Unsubscribing from event {eventName}")]
    public static partial void InformationUnsubscribing(this ILogger<EventBusActiveMQ> logger, string eventName);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Starting ActiveMQ basic consume")]
    public static partial void TraceStartingBasicConsume(this ILogger<EventBusActiveMQ> logger);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Consumer tag {consumerTag} already exist. Cancelled BasicConsume again")]
    public static partial void TraceConsumerTagExist(this ILogger<EventBusActiveMQ> logger, string consumerTag);

    [LoggerMessage(Level = LogLevel.Error, Message = "StartBasicConsume can't call on _consumerSession == null")]
    public static partial void ErrorStartBasicConsumeCantCall(this ILogger<EventBusActiveMQ> logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "----- ERROR Processing message \"{message}\"")]
    public static partial void WarningProcessingMessage(this ILogger<EventBusActiveMQ> logger, string message, Exception exception);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Creating ActiveMQ consumer session")]
    public static partial void TraceCreatingConsumerSession(this ILogger<EventBusActiveMQ> logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Recreating ActiveMQ consumer session")]
    public static partial void WarningRecreatingConsumerSession(this ILogger<EventBusActiveMQ> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Processing ActiveMQ event: {eventName}")]
    public static partial void TraceProcessingEvent(this ILogger<EventBusActiveMQ> logger, string eventName);

    [LoggerMessage(Level = LogLevel.Warning, Message = "No subscription for ActiveMQ event: {eventName}")]
    public static partial void WarningNoSubscription(this ILogger<EventBusActiveMQ> logger, string eventName);
}
