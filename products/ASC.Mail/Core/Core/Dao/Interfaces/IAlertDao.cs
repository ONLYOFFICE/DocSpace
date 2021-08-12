/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System.Collections.Generic;

using ASC.Common;
using ASC.Mail.Core.Entities;
using ASC.Mail.Enums;

namespace ASC.Mail.Core.Dao.Interfaces
{
    [Scope(typeof(AlertDao))]
    public interface IAlertDao
    {
        /// <summary>
        ///     Get alert by id.
        /// </summary>
        /// <param name="id">id</param>
        Alert GetAlert(long id);

        /// <summary>
        ///     Get a list of alerts
        /// </summary>
        List<Alert> GetAlerts(int mailboxId = -1, MailAlertTypes type = MailAlertTypes.Empty);

        /// <summary>
        ///     Save or update alert
        /// </summary>
        /// <param name="alert"></param>
        /// <param name="unique"></param>
        int SaveAlert(Alert alert, bool unique = false);

        /// <summary>
        ///     Delete alertс
        /// </summary>
        /// <param name="id">id</param>
        int DeleteAlert(long id);

        /// <summary>
        ///     Delete all mailbox's alerts
        /// </summary>
        /// <param name="mailboxId">id mailbox</param>
        int DeleteAlerts(int mailboxId);

        /// <summary>
        ///     Delete alerts
        /// </summary>
        /// <param name="ids">list of id</param>
        int DeleteAlerts(List<int> ids);
    }
}
