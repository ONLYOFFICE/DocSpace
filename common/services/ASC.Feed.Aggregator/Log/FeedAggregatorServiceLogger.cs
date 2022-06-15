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

namespace ASC.Feed.Aggregator.Log;
internal static partial class FeedAggregatorServiceLogger
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Feed Aggregator service running.")]
    public static partial void InformationAggregatorServiceRunning(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Feed Aggregator service stopping.")]
    public static partial void InformationAggregatorServiceStopping(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Start of collecting feeds...")]
    public static partial void DebugStartCollectiongFeeds(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Find {tenantsCount} tenants for module {moduleName}.")]
    public static partial void DebugFindCountTenants(this ILogger logger, int tenantsCount, string moduleName);

    [LoggerMessage(Level = LogLevel.Debug, Message = "{count} feeds in {tenant} tenant.")]
    public static partial void DebugCountFeeds(this ILogger logger, int count, int tenant);

    [LoggerMessage(Level = LogLevel.Error, Message = "Tenant: {tenant}")]
    public static partial void ErrorTenant(this ILogger logger, int tenant, Exception exception);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Time of collecting news: {date}")]
    public static partial void DebugTimeCollectingNews(this ILogger logger, TimeSpan date);

    [LoggerMessage(Level = LogLevel.Debug, Message = "AggregateFeeds")]
    public static partial void ErrorAggregateFeeds(this ILogger logger, Exception exception);
}
