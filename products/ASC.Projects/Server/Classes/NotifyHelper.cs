/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Notify.Cron;
using ASC.Notify.Messages;
using ASC.Notify.Patterns;
using ASC.Projects.Core.DataInterfaces;
using ASC.Projects.Core.Domain.Reports;
using ASC.Projects.Core.Services.NotifyService;
using ASC.Projects.Engine;
using ASC.Web.Core.Files;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Utility;

using Microsoft.Extensions.Options;

namespace ASC.Projects.Classes
{
    [Scope]
    public class NotifyHelper
    {
        private ILog Log { get; set; }
        private ITaskDao TaskDao { get; set; }
        private IReportDao ReportDao { get; set; }
        private IMilestoneDao MilestoneDao { get; set; }
        private TenantManager TenantManager { get; set; }
        private PaymentManager PaymentManager { get; set; }
        private TenantUtil TenantUtil { get; set; }
        private SecurityContext SecurityContext { get; set; }
        private TaskEngine TaskEngine { get; set; }
        private UserManager UserManager { get; set; }
        private DocbuilderReportsUtility DocbuilderReportsUtility { get; set; }
        private TimeZoneConverter TimeZoneConverter { get; set; }
        private ReportHelper ReportHelper { get; set; }
        private FilesLinkUtility FilesLinkUtility { get; set; }
        private CommonLinkUtility CommonLinkUtility { get; set; }
        private NotifyClient NotifyClient { get; set; }
        private ReportTemplateHelper ReportTemplateHelper { get; set; }

        public NotifyHelper(IOptionsMonitor<ILog> options,
            IDaoFactory daoFactory, 
            TenantManager tenantManager,
            PaymentManager paymentManager,
            TenantUtil tenantUtil,
            SecurityContext securityContext,
            EngineFactory engineFactory, 
            UserManager userManager, 
            DocbuilderReportsUtility docbuilderReportsUtility,
            TimeZoneConverter timeZoneConverter,
            ReportHelper reportHelper, 
            FilesLinkUtility filesLinkUtility, 
            CommonLinkUtility commonLinkUtility,
            NotifyClient notifyClient)
        {
            Log = options.CurrentValue;
            TaskDao = daoFactory.GetTaskDao();
            ReportDao = daoFactory.GetReportDao();
            MilestoneDao = daoFactory.GetMilestoneDao();
            TenantManager = tenantManager;
            PaymentManager = paymentManager;
            TenantUtil = tenantUtil;
            SecurityContext = securityContext;
            TaskEngine = engineFactory.GetTaskEngine();
            UserManager = userManager;
            DocbuilderReportsUtility = docbuilderReportsUtility;
            TimeZoneConverter = timeZoneConverter;
            ReportHelper = reportHelper;
            FilesLinkUtility = filesLinkUtility;
            CommonLinkUtility = commonLinkUtility;
            NotifyClient = notifyClient;
        }

        public void SendAutoReminderAboutTask(DateTime state)
        {
            try
            {
                var now = DateTime.UtcNow;
                List<object[]> tasks;

                tasks = TaskDao.GetTasksForReminder(now);

                foreach (var r in tasks)
                {
                    var tenant = TenantManager.GetTenant((int)r[0]);
                    if (tenant == null ||
                        tenant.Status != TenantStatus.Active ||
                        TariffState.NotPaid <= PaymentManager.GetTariff(tenant.TenantId).State)
                    {
                        continue;
                    }

                    var localTime = TenantUtil.DateTimeFromUtc(tenant.TimeZone, now);
                    if (!TimeToSendReminderAboutTask(localTime)) continue;

                    var deadline = (DateTime)r[2];
                    if (deadline.Date != localTime.Date) continue;

                    try
                    {
                        TenantManager.SetCurrentTenant(tenant);
                        SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);
                        var t = TaskEngine.GetByID((int)r[1]);
                        if (t == null) continue;

                        foreach (var responsible in t.Responsibles)
                        {
                            var user = UserManager.GetUsers(t.CreateBy);
                            if (!Constants.LostUser.Equals(user) && user.Status == EmployeeStatus.Active)
                            {
                                SecurityContext.AuthenticateMe(user.ID);

                                Thread.CurrentThread.CurrentCulture = user.GetCulture();
                                Thread.CurrentThread.CurrentUICulture = user.GetCulture();

                                NotifyClient.SendReminderAboutTaskDeadline(new List<Guid> { responsible }, t);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("SendAutoReminderAboutTask", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("SendAutoReminderAboutTask", ex);
            }
        }

        private static bool TimeToSendReminderAboutTask(DateTime currentTime)
        {
            var hourToSend = 7;
            /*if (!string.IsNullOrEmpty(ConfigurationManagerExtension.AppSettings["remindertime"]))
            {
                int hour;
                if (int.TryParse(ConfigurationManagerExtension.AppSettings["remindertime"], out hour))
                {
                    hourToSend = hour;
                }
            }*/
            return currentTime.Hour == hourToSend;
        }

        public void SendAutoReports(DateTime datetime)
        {
            try
            {
                var now = DateTime.UtcNow.Date.AddHours(DateTime.UtcNow.Hour);

                List<ReportTemplate> templates;

                
                templates = ReportDao.GetAutoTemplates();

                foreach (var tGrouped in templates.GroupBy(r => r.Tenant))
                {
                    try
                    {
                        SendByTenant(now, tGrouped);
                    }
                    catch (Exception e)
                    {
                        Log.Error("SendByTenant", e);
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error("SendAutoReports", ex);
            }
        }

        public void SendMsgMilestoneDeadline(DateTime scheduleDate)
        {
            var date = DateTime.UtcNow.AddDays(2);

            List<object[]> milestones;

            milestones = MilestoneDao.GetInfoForReminder(date);

            foreach (var r in milestones)
            {
                var tenant = TenantManager.GetTenant((int)r[0]);
                if (tenant == null ||
                    tenant.Status != TenantStatus.Active ||
                    TariffState.NotPaid <= PaymentManager.GetTariff(tenant.TenantId).State)
                {
                    continue;
                }

                var localTime = TenantUtil.DateTimeFromUtc(tenant.TimeZone, date);
                if (localTime.Date == ((DateTime)r[2]).Date)
                {
                    try
                    {
                        TenantManager.SetCurrentTenant(tenant);
                        SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);
                        
                        var m = MilestoneDao.GetById((int)r[1]);
                        if (m != null)
                        {
                            var sender = !m.Responsible.Equals(Guid.Empty) ? m.Responsible : m.Project.Responsible;
                            var user = UserManager.GetUsers(sender);
                            if (!Constants.LostUser.Equals(user) && user.Status == EmployeeStatus.Active)
                            {
                                SecurityContext.AuthenticateMe(user.ID);

                                Thread.CurrentThread.CurrentCulture = user.GetCulture();
                                Thread.CurrentThread.CurrentUICulture = user.GetCulture();

                                NotifyClient.SendMilestoneDeadline(sender, m);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("SendMsgMilestoneDeadline", ex);
                    }
                }
            }
        }

        private void SendByTenant(DateTime now, IGrouping<int, ReportTemplate> tGrouped)
        {
            var tenant = TenantManager.GetTenant(tGrouped.Key);
            if (tenant == null || tenant.Status != TenantStatus.Active || PaymentManager.GetTariff(tenant.TenantId).State >= TariffState.NotPaid) return;

            TenantManager.SetCurrentTenant(tenant);
            DocbuilderReportsUtility.Terminate(ReportOrigin.ProjectsAuto, tenant.TenantId, SecurityContext.CurrentAccount.ID);

            var timeZone = TimeZoneConverter.GetTimeZone(TenantManager.GetCurrentTenant().TimeZone);

            foreach (var t in tGrouped)
            {
                try
                {
                    SendReport(now, timeZone, t);
                }
                catch (System.Security.SecurityException se)
                {
                    Log.Error("SendAutoReports", se);
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("TemplateId: {0}, Temaplate: {1}\r\n{2}", t.Id, t.Filter.ToXml(), ex);
                }
            }
        }

        private void SendReport(DateTime now, TimeZoneInfo timeZone, ReportTemplate t)
        {
            var cron = new CronExpression(t.Cron) { TimeZone = timeZone };
            var date = cron.GetTimeAfter(now.AddTicks(-1));

            Log.DebugFormat("Find auto report: {0} - {1}, now: {2}, date: {3}", t.Name, t.Cron, now, date);
            if (date != now) return;

            var user = UserManager.GetUsers(t.CreateBy);
            if (user.ID == Constants.LostUser.ID || user.Status != EmployeeStatus.Active) return;

            SecurityContext.AuthenticateMe(user.ID);

            Thread.CurrentThread.CurrentCulture = user.GetCulture();
            Thread.CurrentThread.CurrentUICulture = user.GetCulture();

            var message = new NoticeMessage(user, HttpUtility.HtmlDecode(t.Name), "", "html");
            message.AddArgument(new TagValue(CommonTags.SendFrom, TenantManager.GetCurrentTenant().Name));
            message.AddArgument(new TagValue(CommonTags.Priority, 1));

            ReportState state;
            var template = t;
            var result = ReportHelper.TryCreateReportFromTemplate(t, (s, u) =>
            {
                try
                {
                    if (string.IsNullOrEmpty(s.Exception))
                    {
                        ReportTemplateHelper.SaveDocbuilderReport(s, u, template);
                    }

                    SendWhenGenerated(s, user);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }, message, out state, true);

            if (!result)
            {
                SendWhenGenerated(state, user);
            }
        }

        private void SendWhenGenerated(ReportState state, UserInfo user)
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = user.GetCulture();
                Thread.CurrentThread.CurrentUICulture = user.GetCulture();

                var message = (NoticeMessage)state.Obj;

                message.Body = !string.IsNullOrEmpty(state.Exception) ? state.Exception : string.Format(Resources.ReportResource.AutoGeneratedReportMail, message.Subject, CommonLinkUtility.GetFullAbsolutePath(FilesLinkUtility.GetFileWebEditorUrl(state.FileId)));

                WorkContext.NotifyContext.DispatchEngine.Dispatch(message, "email.sender");

                Log.DebugFormat("Send auto report: {0} to {1}, tenant: {2}", message.Subject, SecurityContext.CurrentAccount.ID, TenantManager.GetCurrentTenant().TenantId);
            }
            catch (Exception e)
            {
                Log.Error("SendWhenGenerated", e);
            }
        }
    }
}
