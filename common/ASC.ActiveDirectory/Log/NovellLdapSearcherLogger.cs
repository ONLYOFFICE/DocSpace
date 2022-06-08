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


namespace ASC.ActiveDirectory.Log;
static internal partial class NovellLdapSearcherLogger
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "ldapConnection.Connect(Server='{server}', PortNumber='{portNumber}');")]
    public static partial void DebugldapConnection(this ILogger<NovellLdapSearcher> logger, string server, int portNumber);

    [LoggerMessage(Level = LogLevel.Debug, Message = "ldapConnection.StartTls();")]
    public static partial void DebugStartTls(this ILogger<NovellLdapSearcher> logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "LDAP certificate confirmation requested.")]
    public static partial void DebugLdapCertificateConfirmationRequested(this ILogger<NovellLdapSearcher> logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "ldapConnection.Bind(Anonymous)")]
    public static partial void DebugBindAnonymous(this ILogger<NovellLdapSearcher> logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "ldapConnection.Bind(Login: '{login}')")]
    public static partial void DebugBind(this ILogger<NovellLdapSearcher> logger, string login);

    [LoggerMessage(Level = LogLevel.Warning, Message = "ServerCertValidationHandler: sslPolicyErrors = {sslPolicyErrors}")]
    public static partial void WarnSslPolicyErrors(this ILogger<NovellLdapSearcher> logger, SslPolicyErrors sslPolicyErrors);

    [LoggerMessage(Level = LogLevel.Warning, Message = "The size of the search results is limited. Start TrySearchSimple()")]
    public static partial void WarnStartTrySearchSimple(this ILogger<NovellLdapSearcher> logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "Search({searchFilter}) failed")]
    public static partial void ErrorSearch(this ILogger<NovellLdapSearcher> logger, string searchFilter, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "TrySearchSimple() failed")]
    public static partial void ErrorTrySearchSimple(this ILogger<NovellLdapSearcher> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "SearchSimple({searchFilter}) failed")]
    public static partial void ErrorSearchSimple(this ILogger<NovellLdapSearcher> logger, string searchFilter, Exception exception);

    [LoggerMessage(Level = LogLevel.Debug, Message = "{i}. DN: {distinguishedName}")]
    public static partial void DebugDnEnumeration(this ILogger<NovellLdapSearcher> logger, int i, string distinguishedName);

    [LoggerMessage(Level = LogLevel.Debug, Message = "No controls returned")]
    public static partial void DebugNoControlsReturned(this ILogger<NovellLdapSearcher> logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "GetCapabilities()->LoopResults failed")]
    public static partial void ErrorGetCapabilitiesLoopResultsFailed(this ILogger<NovellLdapSearcher> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "GetCapabilities() failed")]
    public static partial void ErrorGetCapabilitiesFailed(this ILogger<NovellLdapSearcher> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "GetLdapUniqueId()")]
    public static partial void ErrorGetLdapUniqueId(this ILogger<NovellLdapSearcher> logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Debug, Message = "ldapConnection.StopTls();")]
    public static partial void DebugLdapConnectionStopTls(this ILogger<NovellLdapSearcher> logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "ldapConnection.Disconnect();")]
    public static partial void DebugLdapConnectionDisconnect(this ILogger<NovellLdapSearcher> logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "ldapConnection.Dispose();")]
    public static partial void DebugLdapConnectionDispose(this ILogger<NovellLdapSearcher> logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "LDAP->Dispose() failed")]
    public static partial void ErrorLdapDisposeFailed(this ILogger<NovellLdapSearcher> logger, Exception exception);
}
