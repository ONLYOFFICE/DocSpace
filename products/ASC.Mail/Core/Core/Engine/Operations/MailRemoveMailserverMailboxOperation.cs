/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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

using ASC.Common.Logging;
using ASC.Core;
using ASC.Mail.Core.Engine.Operations.Base;
using ASC.Mail.Models;
using ASC.Mail.Storage;

using Microsoft.Extensions.Options;

namespace ASC.Mail.Core.Engine.Operations
{
    public class MailRemoveMailserverMailboxOperation : MailOperation
    {
        private readonly MailBoxData _mailBox;

        public override MailOperationType OperationType
        {
            get { return MailOperationType.RemoveMailbox; }
        }

        public ServerMailboxEngine ServerMailboxEngine { get; }
        public OperationEngine OperationEngine { get; }
        public CacheEngine CacheEngine { get; }
        public IndexEngine IndexEngine { get; }

        public MailRemoveMailserverMailboxOperation(
            TenantManager tenantManager,
            SecurityContext securityContext,
            IMailDaoFactory mailDaoFactory,
            ServerMailboxEngine serverMailboxEngine,
            OperationEngine operationEngine,
            CacheEngine cacheEngine,
            IndexEngine indexEngine,
            CoreSettings coreSettings,
            StorageManager storageManager,
            IOptionsMonitor<ILog> optionsMonitor,
            MailBoxData mailBox)
            : base(tenantManager, securityContext, mailDaoFactory, coreSettings, storageManager, optionsMonitor)
        {
            ServerMailboxEngine = serverMailboxEngine;
            OperationEngine = operationEngine;
            CacheEngine = cacheEngine;
            IndexEngine = indexEngine;
            _mailBox = mailBox;
            SetSource(_mailBox.MailBoxId.ToString());
        }

        protected override void Do()
        {
            try
            {
                SetProgress((int?)MailOperationRemoveMailboxProgress.Init, "Setup tenant and user");

                var tenant = _mailBox.TenantId;
                var user = _mailBox.UserId;

                TenantManager.SetCurrentTenant(tenant);

                try
                {
                    SecurityContext.AuthenticateMe(new Guid(user));
                }
                catch
                {
                    // User was removed
                    SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);
                }

                SetProgress((int?)MailOperationRemoveMailboxProgress.RemoveFromDb, "Remove mailbox from Db");

                ServerMailboxEngine.RemoveMailbox(_mailBox);

                SetProgress((int?)MailOperationRemoveMailboxProgress.RecalculateFolder, "Recalculate folders counters");

                OperationEngine.RecalculateFolders();

                SetProgress((int?)MailOperationRemoveMailboxProgress.ClearCache, "Clear accounts cache");

                CacheEngine.Clear(user);

                SetProgress((int?)MailOperationRemoveMailboxProgress.RemoveIndex, "Remove Elastic Search index by messages");

                IndexEngine.Remove(_mailBox);
            }
            catch (Exception e)
            {
                Logger.Error("Mail operation error -> Remove mailbox: {0}", e);
                Error = "InternalServerError";
            }
        }
    }
}
