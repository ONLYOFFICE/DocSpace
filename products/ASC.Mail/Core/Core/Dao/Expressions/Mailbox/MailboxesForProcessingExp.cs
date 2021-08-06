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


using System;
using System.Linq;
using System.Linq.Expressions;

using ASC.Mail.Configuration;
using ASC.Mail.Core.Dao.Entities;

using Microsoft.EntityFrameworkCore;

namespace ASC.Mail.Core.Dao.Expressions.Mailbox
{
    public class MailboxesForProcessingExp : IMailboxesExp
    {
        public string OrderBy
        {
            get { return MailMailboxNames.DateChecked; }
        }
        public bool? OrderAsc { get; private set; }
        public int? Limit { get; private set; }

        private MailSettings MailSettings { get; set; }
        private bool OnlyActive { get; set; }

        public MailboxesForProcessingExp(MailSettings mailSettings, int tasksLimit, bool active)
        {
            MailSettings = mailSettings;

            Limit = tasksLimit > 0 ? tasksLimit : null;

            OnlyActive = active;

            OrderAsc = true;
        }

        Expression<Func<MailMailbox, bool>> IMailboxesExp.GetExpression()
        {
            var now = DateTime.UtcNow;

            Expression<Func<MailMailbox, bool>> exp = mb =>
            mb.IsProcessed == false
            && mb.DateLoginDelayExpires < now
            && mb.IsRemoved == false
            && mb.Enabled == true;

            if (MailSettings.Aggregator.AggregateMode != MailSettings.AggregatorConfig.AggregateModeType.All)
            {
                exp = exp.And(mb => mb.IsServerMailbox == (MailSettings.Aggregator.AggregateMode == MailSettings.AggregatorConfig.AggregateModeType.Internal));
            }

            if (MailSettings.Aggregator.EnableSignalr)
            {
                exp = exp.And(mb => mb.UserOnline == OnlyActive);
            }
            else
            {
                exp = exp.And(mb => mb.DateUserChecked == null || OnlyActive
                ? EF.Functions.DateDiffSecond(mb.DateUserChecked, now) < MailSettings.Defines.ActiveInterval.TotalSeconds
                : EF.Functions.DateDiffSecond(mb.DateUserChecked, now) >= MailSettings.Defines.ActiveInterval.TotalSeconds);
            }

            if (MailSettings.Defines.WorkOnUsersOnlyList.Any())
            {
                exp = exp.And(mb => MailSettings.Defines.WorkOnUsersOnlyList.Contains(mb.IdUser));
            }

            return exp;
        }
    }
}