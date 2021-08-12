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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using ASC.Common;
using ASC.Mail.Core.Entities;
using ASC.Mail.Enums;
using ASC.Mail.Models;
using ASC.Mail.Utils;

namespace ASC.Mail.Core.Engine
{
    [Scope]
    public class AlertEngine
    {
        private IMailDaoFactory MailDaoFactory { get; }

        public AlertEngine(IMailDaoFactory mailDaoFactory)
        {
            MailDaoFactory = mailDaoFactory;
        }

        public List<MailAlertData> GetAlerts(/*int mailboxId = -1, MailAlertTypes type = MailAlertTypes.Empty*/)
        {
            var alerts = MailDaoFactory.GetAlertDao().GetAlerts();

            var alertsList = new List<MailAlertData>();

            foreach (var alert in alerts)
            {

                alertsList.Add(ToMailAlert(alert));
            }

            return alertsList;
        }

        public bool DeleteAlert(long id)
        {
            var result = MailDaoFactory.GetAlertDao().DeleteAlert(id);

            if (result <= 0)
                return false;

            return true;
        }

        public bool DeleteAlert(MailAlertTypes type)
        {
            var quotaAlerts = MailDaoFactory.GetAlertDao().GetAlerts(-1, type);

            if (!quotaAlerts.Any())
                return true;

            var result = MailDaoFactory.GetAlertDao().DeleteAlerts(quotaAlerts.Select(a => a.Id).ToList());

            if (result <= 0)
                throw new Exception("Delete old alerts failed");

            return true;
        }

        [DataContract]
        private struct UploadToDocumentsFailure
        {
            [DataMember]
            public int error_type;
        }

        public int CreateUploadToDocumentsFailureAlert(int tenant, string user, int mailboxId, UploadToDocumentsErrorType errorType)
        {
            var data = new UploadToDocumentsFailure
            {
                error_type = (int)errorType
            };

            var jsonData = MailUtil.GetJsonString(data);

            var alert = new Alert
            {
                Tenant = tenant,
                User = user,
                MailboxId = mailboxId,
                Type = MailAlertTypes.UploadFailure,
                Data = jsonData
            };

            var result = MailDaoFactory.GetAlertDao().SaveAlert(alert);

            if (result <= 0)
                throw new Exception("Save alert failed");

            return result;

        }

        public int CreateDisableAllMailboxesAlert(int tenant, List<string> users)
        {
            var result = 0;

            if (!users.Any()) return result;

            var dao = MailDaoFactory.GetAlertDao();

            foreach (var user in users)
            {
                var alert = new Alert
                {
                    Tenant = tenant,
                    User = user,
                    MailboxId = -1,
                    Type = MailAlertTypes.DisableAllMailboxes,
                    Data = null
                };

                var r = dao.SaveAlert(alert);

                if (r <= 0)
                    throw new Exception("Save alert failed");

                result += r;
            }

            return result;
        }

        public int CreateAuthErrorWarningAlert(int tenant, string user, int mailboxId)
        {
            var alert = new Alert
            {
                Tenant = tenant,
                User = user,
                MailboxId = mailboxId,
                Type = MailAlertTypes.AuthConnectFailure,
                Data = null
            };

            var result = MailDaoFactory.GetAlertDao().SaveAlert(alert, true);

            if (result <= 0)
                throw new Exception("Save alert failed");

            return result;
        }

        public int CreateAuthErrorDisableAlert(int tenant, string user, int mailboxId)
        {
            var dao = MailDaoFactory.GetAlertDao();

            var alerts = dao.GetAlerts(mailboxId, MailAlertTypes.AuthConnectFailure);

            if (alerts.Any())
            {
                var r = dao.DeleteAlerts(alerts.Select(a => a.Id).ToList());

                if (r <= 0)
                    throw new Exception("Delete alerts failed");
            }

            var alert = new Alert
            {
                Tenant = tenant,
                User = user,
                MailboxId = mailboxId,
                Type = MailAlertTypes.TooManyAuthError,
                Data = null
            };

            var result = dao.SaveAlert(alert, true);

            if (result <= 0)
                throw new Exception("Save alert failed");

            return result;
        }

        public int CreateQuotaErrorWarningAlert(int tenant, string user)
        {
            var alert = new Alert
            {
                Tenant = tenant,
                User = user,
                MailboxId = -1,
                Type = MailAlertTypes.QuotaError,
                Data = null
            };

            var result = MailDaoFactory.GetAlertDao().SaveAlert(alert, true);

            if (result <= 0)
                throw new Exception("Save alert failed");

            return result;
        }

        [DataContract]
        private struct DeliveryFailure
        {
            [DataMember]
            public string subject;
            [DataMember]
            public string from;
            [DataMember]
            public int message_id;
            [DataMember]
            public int failure_id;
        }

        public int CreateDeliveryFailureAlert(int tenant, string user, int mailboxId, string subject, string from,
            int messageId, int mailDaemonMessageid)
        {
            var data = new DeliveryFailure
            {
                @from = from,
                message_id = messageId,
                subject = subject,
                failure_id = mailDaemonMessageid
            };

            var jsonData = MailUtil.GetJsonString(data);

            var alert = new Alert
            {
                Tenant = tenant,
                User = user,
                MailboxId = mailboxId,
                Type = MailAlertTypes.DeliveryFailure,
                Data = jsonData
            };

            var result = MailDaoFactory.GetAlertDao().SaveAlert(alert);

            if (result <= 0)
                throw new Exception("Save alert failed");

            return result;
        }

        [DataContract]
        private struct CrmOperationFailure
        {
            [DataMember]
            public int message_id;
        }

        public int CreateCrmOperationFailureAlert(int tenant, string user, int messageId, MailAlertTypes type)
        {
            var data = new CrmOperationFailure
            {
                message_id = messageId
            };

            var jsonData = MailUtil.GetJsonString(data);

            var alert = new Alert
            {
                Tenant = tenant,
                User = user,
                MailboxId = -1,
                Type = type,
                Data = jsonData
            };

            var result = MailDaoFactory.GetAlertDao().SaveAlert(alert);

            if (result <= 0)
                throw new Exception("Save alert failed");

            return result;
        }

        protected MailAlertData ToMailAlert(Alert a)
        {
            return new MailAlertData
            {
                id = a.Id,
                id_mailbox = a.MailboxId,
                type = a.Type,
                data = a.Data
            };
        }
    }
}
