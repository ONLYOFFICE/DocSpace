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

namespace ASC.Web.Core.Log;
internal static partial class StudioPeriodicNotifyLogger
{
    [LoggerMessage(Level = LogLevel.Error, Message = "SendSaasLettersAsync")]
    public static partial void ErrorSendSaasLettersAsync(this ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "SendEnterpriseLetters")]
    public static partial void ErrorSendEnterpriseLetters(this ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "SendOpensourceLetters")]
    public static partial void ErrorSendOpensourceLetters(this ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "SendPersonalLetters")]
    public static partial void ErrorSendPersonalLetters(this ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "SendMsgWhatsNew")]
    public static partial void ErrorSendMsgWhatsNew(this ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Information, Message = "Start SendSaasTariffLetters")]
    public static partial void InformationStartSendSaasTariffLetters(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "End SendSaasTariffLetters")]
    public static partial void InformationEndSendSaasTariffLetters(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Start SendTariffEnterpriseLetters")]
    public static partial void InformationStartSendTariffEnterpriseLetters(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "End SendTariffEnterpriseLetters")]
    public static partial void InformationEndSendTariffEnterpriseLetters(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Start SendOpensourceTariffLetters")]
    public static partial void InformationStartSendOpensourceTariffLetters(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "End SendOpensourceTariffLetters")]
    public static partial void InformationEndSendOpensourceTariffLetters(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Start SendLettersPersonal...")]
    public static partial void InformationStartSendLettersPersonal(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Current tenant: {tenantId}")]
    public static partial void InformationCurrentTenant(this ILogger logger, int tenantId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Send letter personal '{id}' to {email} culture {culture}. tenant id: {tenantId} user culture {userCulture} create on {createDate} now date {scheduleDate}")]
    public static partial void InformationSendLetterPersonal(this ILogger logger, string id, string email, CultureInfo culture, int tenantId, CultureInfo userCulture, DateTime createDate, DateTime scheduleDate);

    [LoggerMessage(Level = LogLevel.Information, Message = "Total send count: {sendCount}")]
    public static partial void InformationTotalSendCount(this ILogger logger, int sendCount);

    [LoggerMessage(Level = LogLevel.Information, Message = "End SendLettersPersonal.")]
    public static partial void InformationEndSendLettersPersonal(this ILogger logger);
}
