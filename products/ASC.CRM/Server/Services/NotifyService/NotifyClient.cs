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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;

using ASC.Common;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;
using ASC.Notify;
using ASC.Notify.Engine;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;

using Microsoft.Extensions.Logging;

namespace ASC.Web.CRM.Services.NotifyService
{
    [Scope]
    public class NotifyClient
    {
        private readonly INotifySource _notifySource;
        private readonly Context _notifyContext;
        private readonly NotifyEngineQueue _notifyEngineQueue;
        private readonly PathProvider _pathProvider;
        private readonly SecurityContext _securityContext;
        private readonly CoreBaseSettings _coreBaseSettings;
        private readonly DaoFactory _daoFactory;
        private readonly TenantManager _tenantManager;
        private readonly PaymentManager _paymentManager;
        private readonly UserManager _userManager;
        private readonly ILogger _logger;
        private readonly CoreSettings _coreSettings;
        private readonly TenantUtil _tenantUtil;

        public NotifyClient(Context notifyContext, 
            NotifyEngineQueue notifyEngineQueue,
            INotifySource notifySource,
            PathProvider pathProvider,
            SecurityContext securityContext,
            CoreBaseSettings coreBaseSettings,
            DaoFactory daoFactory,
            TenantManager tenantManager,
            PaymentManager paymentManager,
            UserManager userManager,
            CoreSettings coreSettings,
            ILogger logger,
            TenantUtil tenantUtil)
        {
            _notifyContext = notifyContext;
            _notifyEngineQueue = notifyEngineQueue;
            _notifySource = notifySource;
            _pathProvider = pathProvider;
            _securityContext = securityContext;
            _coreBaseSettings = coreBaseSettings;
            _daoFactory = daoFactory;
            _tenantManager = tenantManager;
            _paymentManager = paymentManager;
            _userManager = userManager;
            _logger = logger;
            _coreSettings = coreSettings;
            _tenantUtil = tenantUtil;
        }

        public void SendAboutCreateNewContact(List<Guid> recipientID,
                                              int contactID,
                                              string contactTitle, NameValueCollection fields)
        {
            if ((recipientID.Count == 0) || String.IsNullOrEmpty(contactTitle)) return;

            var client = _notifyContext.RegisterClient(_notifyEngineQueue, _notifySource);

            client.SendNoticeToAsync(
                NotifyConstants.Event_CreateNewContact,
                null,
                recipientID.ConvertAll(item => _notifySource.GetRecipientsProvider().GetRecipient(item.ToString())).ToArray(),
                true,
                new TagValue(NotifyConstants.Tag_AdditionalData, fields),
                new TagValue(NotifyConstants.Tag_EntityTitle, contactTitle),
                new TagValue(NotifyConstants.Tag_EntityID, contactID)
             );
        }

        public void SendAboutSetAccess(EntityType entityType, int entityID, DaoFactory daoFactory, params Guid[] userID)
        {
            if (userID.Length == 0) return;

            var client = _notifyContext.RegisterClient(_notifyEngineQueue, _notifySource);

            var baseData = ExtractBaseDataFrom(entityType, entityID, daoFactory);

            client.SendNoticeToAsync(
                   NotifyConstants.Event_SetAccess,
                   null,
                   userID.Select(item => _notifySource.GetRecipientsProvider().GetRecipient(item.ToString())).ToArray(),
                   true,
                   new TagValue(NotifyConstants.Tag_EntityID, baseData["id"]),
                   new TagValue(NotifyConstants.Tag_EntityTitle, baseData["title"]),
                   new TagValue(NotifyConstants.Tag_EntityRelativeURL, baseData["entityRelativeURL"])
                );
        }

        private NameValueCollection ExtractBaseDataFrom(EntityType entityType, int entityID, DaoFactory daoFactory)
        {
            var result = new NameValueCollection();

            String title;
            String relativeURL;

            switch (entityType)
            {
                case EntityType.Person:
                case EntityType.Company:
                case EntityType.Contact:
                {
                    var contact = daoFactory.GetContactDao().GetByID(entityID);
                    title = contact != null ? contact.GetTitle() : string.Empty;
                    relativeURL = "default.aspx?id=" + entityID;
                    break;
                }
                case EntityType.Opportunity:
                {
                    var deal = daoFactory.GetDealDao().GetByID(entityID);
                    title = deal != null ? deal.Title : string.Empty;
                    relativeURL = "deals.aspx?id=" + entityID;
                    break;
                }
                case EntityType.Case:
                {
                    var cases = daoFactory.GetCasesDao().GetByID(entityID);
                    title = cases != null ? cases.Title : string.Empty;
                    relativeURL = "cases.aspx?id=" + entityID;
                    break;
                }

                default:
                    throw new ArgumentException();
            }

            result.Add("title", title);
            result.Add("id", entityID.ToString());
            result.Add("entityRelativeURL", String.Concat(_pathProvider.BaseAbsolutePath, relativeURL));

            return result;
        }

        public void SendAboutAddRelationshipEventAdd(RelationshipEvent entity, Hashtable fileListInfoHashtable, DaoFactory daoFactory, params Guid[] userID)
        {
            if (userID.Length == 0) return;

            var client = _notifyContext.RegisterClient(_notifyEngineQueue, _notifySource);

            NameValueCollection baseEntityData;

            if (entity.EntityID != 0)
            {
                baseEntityData = ExtractBaseDataFrom(entity.EntityType, entity.EntityID, daoFactory);
            }
            else
            {
                var contact = daoFactory.GetContactDao().GetByID(entity.ContactID);

                baseEntityData = new NameValueCollection();
                baseEntityData["title"] = contact.GetTitle();
                baseEntityData["id"] = contact.ID.ToString();
                baseEntityData["entityRelativeURL"] = "default.aspx?id=" + contact.ID;

                if (contact is Person)
                    baseEntityData["entityRelativeURL"] += "&type=people";

                baseEntityData["entityRelativeURL"] = String.Concat(_pathProvider.BaseAbsolutePath,
                                                                    baseEntityData["entityRelativeURL"]);
            }

            client.BeginSingleRecipientEvent("send about add relationship event add");

            var interceptor = new InitiatorInterceptor(new DirectRecipient(_securityContext.CurrentAccount.ID.ToString(), ""));

            client.AddInterceptor(interceptor);

            try
            {

                client.SendNoticeToAsync(
                      NotifyConstants.Event_AddRelationshipEvent,
                      null,
                      userID.Select(item => _notifySource.GetRecipientsProvider().GetRecipient(item.ToString())).ToArray(),
                      true,
                      new TagValue(NotifyConstants.Tag_EntityTitle, baseEntityData["title"]),
                      new TagValue(NotifyConstants.Tag_EntityID, baseEntityData["id"]),
                      new TagValue(NotifyConstants.Tag_EntityRelativeURL, baseEntityData["entityRelativeURL"]),
                      new TagValue(NotifyConstants.Tag_AdditionalData,
                      new Hashtable {
                      { "Files", fileListInfoHashtable },
                      {"EventContent", entity.Content}}));

            }
            finally
            {
                client.RemoveInterceptor(interceptor.Name);
                client.EndSingleRecipientEvent("send about add relationship event add");
            }


        }

        public void SendAboutExportCompleted(Guid recipientID, String fileName, String filePath)
        {
            if (recipientID == Guid.Empty) return;

            var client = _notifyContext.RegisterClient(_notifyEngineQueue, _notifySource);

            var recipient = _notifySource.GetRecipientsProvider().GetRecipient(recipientID.ToString());

            client.SendNoticeToAsync(_coreBaseSettings.CustomMode ? NotifyConstants.Event_ExportCompletedCustomMode : NotifyConstants.Event_ExportCompleted,
               null,
               new[] { recipient },
               true,
               new TagValue(NotifyConstants.Tag_EntityTitle, fileName),
               new TagValue(NotifyConstants.Tag_EntityRelativeURL, filePath));

        }

        public void SendAboutImportCompleted(Guid recipientID, EntityType entityType)
        {
            if (recipientID == Guid.Empty) return;

            var client = _notifyContext.RegisterClient(_notifyEngineQueue, _notifySource);

            var recipient = _notifySource.GetRecipientsProvider().GetRecipient(recipientID.ToString());

            string entitiyListTitle;
            string entitiyListRelativeURL;

            switch (entityType)
            {
                case EntityType.Contact:
                    entitiyListTitle = CRMContactResource.Contacts;
                    entitiyListRelativeURL = "products/crm/";
                    break;
                case EntityType.Opportunity:
                    entitiyListTitle = CRMCommonResource.DealModuleName;
                    entitiyListRelativeURL = "products/crm/deals.aspx";
                    break;
                case EntityType.Case:
                    entitiyListTitle = CRMCommonResource.CasesModuleName;
                    entitiyListRelativeURL = "products/crm/cases.aspx";
                    break;
                case EntityType.Task:
                    entitiyListTitle = CRMCommonResource.TaskModuleName;
                    entitiyListRelativeURL = "products/crm/tasks.aspx";
                    break;
                default:
                    throw new ArgumentException(CRMErrorsResource.EntityTypeUnknown);
            }

            client.SendNoticeToAsync(
                _coreBaseSettings.CustomMode ? NotifyConstants.Event_ImportCompletedCustomMode : NotifyConstants.Event_ImportCompleted,
                null,
                new[] { recipient },
                true,
                new TagValue(NotifyConstants.Tag_EntityListRelativeURL, entitiyListRelativeURL),
                new TagValue(NotifyConstants.Tag_EntityListTitle, entitiyListTitle));
        }

        public void SendAutoReminderAboutTask(DateTime scheduleDate)
        {
            var execAlert = new List<int>();

            foreach (var row in _daoFactory.GetTaskDao()
                .GetInfoForReminder(scheduleDate))
            {

                var tenantId = Convert.ToInt32(row[0]);
                var taskId = Convert.ToInt32(row[1]);
                var deadline = Convert.ToDateTime(row[2]);
                var alertValue = Convert.ToInt32(row[3]);
                var responsibleID = !string.IsNullOrEmpty(Convert.ToString(row[4]))
                    ? new Guid(Convert.ToString(row[4]))
                    : Guid.Empty;

                var deadlineReminderDate = deadline.AddMinutes(-alertValue);

                if (deadlineReminderDate.Subtract(scheduleDate).Minutes > 1) continue;

                execAlert.Add(taskId);

                var tenant = _tenantManager.GetTenant(tenantId);
                if (tenant == null ||
                    tenant.Status != TenantStatus.Active ||
                    TariffState.NotPaid <= _paymentManager.GetTariff(tenant.Id).State)
                {
                    continue;
                }

                try
                {
                    _tenantManager.SetCurrentTenant(tenant);
                    _securityContext.AuthenticateMeWithoutCookie(ASC.Core.Configuration.Constants.CoreSystem);

                    var user = _userManager.GetUsers(responsibleID);

                    if (!(!Constants.LostUser.Equals(user) && user.Status == EmployeeStatus.Active)) continue;

                    _securityContext.AuthenticateMeWithoutCookie(user.Id);

                    Thread.CurrentThread.CurrentCulture = user.GetCulture();
                    Thread.CurrentThread.CurrentUICulture = user.GetCulture();

                    var dao = _daoFactory;
                    var task = dao.GetTaskDao().GetByID(taskId);

                    if (task == null) continue;

                    ASC.CRM.Core.Entities.Contact taskContact = null;
                    ASC.CRM.Core.Entities.Cases taskCase = null;
                    ASC.CRM.Core.Entities.Deal taskDeal = null;

                    if (task.ContactID > 0)
                    {
                        taskContact = dao.GetContactDao().GetByID(task.ContactID);
                    }

                    if (task.EntityID > 0)
                    {
                        switch (task.EntityType)
                        {
                            case EntityType.Case:
                                taskCase = dao.GetCasesDao().GetByID(task.EntityID);
                                break;
                            case EntityType.Opportunity:
                                taskDeal = dao.GetDealDao().GetByID(task.EntityID);
                                break;
                        }
                    }

                    var listItem = dao.GetListItemDao().GetByID(task.CategoryID);

                    SendTaskReminder(task,
                        listItem != null ? listItem.Title : string.Empty,
                        taskContact, taskCase, taskDeal);
                }
                catch (Exception ex)
                {
                    _logger.LogError("SendAutoReminderAboutTask, tenant: " + tenant.GetTenantDomain(_coreSettings), ex);
                }
            }

            _daoFactory.GetTaskDao().ExecAlert(execAlert);

        }

        public void SendTaskReminder(Task task, String taskCategoryTitle, Contact taskContact, ASC.CRM.Core.Entities.Cases taskCase, ASC.CRM.Core.Entities.Deal taskDeal)
        {
            var client = _notifyContext.RegisterClient(_notifyEngineQueue, _notifySource);

            var recipient = _notifySource.GetRecipientsProvider().GetRecipient(task.ResponsibleID.ToString());

            if (recipient == null) return;


            var deadLineString = task.DeadLine.Hour == 0 && task.DeadLine.Minute == 0
                ? task.DeadLine.ToShortDateString()
                : task.DeadLine.ToString(CultureInfo.InvariantCulture);

            string taskContactRelativeUrl = null;
            string taskContactTitle = null;

            string taskCaseRelativeUrl = null;
            string taskCaseTitle = null;

            string taskDealRelativeUrl = null;
            string taskDealTitle = null;

            if (taskContact != null)
            {
                taskContactRelativeUrl = String.Format("products/crm/default.aspx?id={0}{1}", taskContact.ID, taskContact is Person ? "&type=people" : "");
                taskContactTitle = taskContact.GetTitle();
            }

            if (taskCase != null)
            {
                taskCaseRelativeUrl = String.Format("products/crm/cases.aspx?id={0}", taskCase.ID);
                taskCaseTitle = taskCase.Title.HtmlEncode();
            }

            if (taskDeal != null)
            {
                taskDealRelativeUrl = String.Format("products/crm/deals.aspx?id={0}", taskDeal.ID);
                taskDealTitle = taskDeal.Title.HtmlEncode();
            }


            client.SendNoticeToAsync(
              NotifyConstants.Event_TaskReminder,
              null,
              new[] { recipient },
              true,
              new TagValue(NotifyConstants.Tag_EntityTitle, task.Title),
              new TagValue(NotifyConstants.Tag_AdditionalData,
                 new Hashtable {
                      { "TaskDescription", HttpUtility.HtmlEncode(task.Description) },
                      { "TaskCategory", taskCategoryTitle },

                      { "ContactRelativeUrl", taskContactRelativeUrl },
                      { "ContactTitle", taskContactTitle },

                      { "CaseRelativeUrl", taskCaseRelativeUrl },
                      { "CaseTitle", taskCaseTitle },

                      { "DealRelativeUrl", taskDealRelativeUrl },
                      { "DealTitle", taskDealTitle },

                      { "DueDate", deadLineString }
                 })
            );
        }

        public void SendAboutResponsibleByTask(Task task, String taskCategoryTitle, Contact taskContact, Cases taskCase, ASC.CRM.Core.Entities.Deal taskDeal, Hashtable fileListInfoHashtable)
        {
            var client = _notifyContext.RegisterClient(_notifyEngineQueue, _notifySource);

            var recipient = _notifySource.GetRecipientsProvider().GetRecipient(task.ResponsibleID.ToString());

            if (recipient == null) return;

            task.DeadLine = _tenantUtil.DateTimeFromUtc(task.DeadLine);

            var deadLineString = task.DeadLine.Hour == 0 && task.DeadLine.Minute == 0
                ? task.DeadLine.ToShortDateString()
                : task.DeadLine.ToString();


            string taskContactRelativeUrl = null;
            string taskContactTitle = null;

            string taskCaseRelativeUrl = null;
            string taskCaseTitle = null;

            string taskDealRelativeUrl = null;
            string taskDealTitle = null;

            if (taskContact != null)
            {
                taskContactRelativeUrl = String.Format("products/crm/default.aspx?id={0}{1}", taskContact.ID, taskContact is Person ? "&type=people" : "");
                taskContactTitle = taskContact.GetTitle();
            }

            if (taskCase != null)
            {
                taskCaseRelativeUrl = String.Format("products/crm/cases.aspx?id={0}", taskCase.ID);
                taskCaseTitle = taskCase.Title.HtmlEncode();
            }

            if (taskDeal != null)
            {
                taskDealRelativeUrl = String.Format("products/crm/deals.aspx?id={0}", taskDeal.ID);
                taskDealTitle = taskDeal.Title.HtmlEncode();
            }

            client.SendNoticeToAsync(
               NotifyConstants.Event_ResponsibleForTask,
               null,
               new[] { recipient },
               true,
               new TagValue(NotifyConstants.Tag_EntityTitle, task.Title),
               new TagValue(NotifyConstants.Tag_AdditionalData,
                 new Hashtable {
                      { "TaskDescription", HttpUtility.HtmlEncode(task.Description) },
                      { "Files", fileListInfoHashtable },
                      { "TaskCategory", taskCategoryTitle },

                      { "ContactRelativeUrl", taskContactRelativeUrl },
                      { "ContactTitle", taskContactTitle },

                      { "CaseRelativeUrl", taskCaseRelativeUrl },
                      { "CaseTitle", taskCaseTitle },

                      { "DealRelativeUrl", taskDealRelativeUrl },
                      { "DealTitle", taskDealTitle },

                      { "DueDate", deadLineString }
                 })
               );
        }

        public void SendAboutResponsibleForOpportunity(Deal deal)
        {
            var client = _notifyContext.RegisterClient(_notifyEngineQueue, _notifySource);

            var recipient = _notifySource.GetRecipientsProvider().GetRecipient(deal.ResponsibleID.ToString());

            if (recipient == null) return;

            client.SendNoticeToAsync(
            NotifyConstants.Event_ResponsibleForOpportunity,
            null,
            new[] { recipient },
            true,
            new TagValue(NotifyConstants.Tag_EntityTitle, deal.Title),
            new TagValue(NotifyConstants.Tag_EntityID, deal.ID),
            new TagValue(NotifyConstants.Tag_AdditionalData,
            new Hashtable {
                      { "OpportunityDescription", HttpUtility.HtmlEncode(deal.Description) }
                 })
            );
        }
    }
}
