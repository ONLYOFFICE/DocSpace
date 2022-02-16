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
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Billing;
using ASC.Core.Common.Billing;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Web.Core.Helpers;
using ASC.Web.Core.PublicResources;
using ASC.Web.Core.Users;
using ASC.Web.Core.WhiteLabel;
using ASC.Web.Studio.Utility;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Web.Studio.Core.Notify
{
    [Singletone(Additional = typeof(StudioPeriodicNotifyExtension))]
    public class StudioPeriodicNotify
    {
        private IServiceProvider ServiceProvider { get; }
        public ILog Log { get; }

        public StudioPeriodicNotify(IServiceProvider serviceProvider, IOptionsMonitor<ILog> log)
        {
            ServiceProvider = serviceProvider;
            Log = log.Get("ASC.Notify");
        }

        public Task SendSaasLettersAsync(string senderName, DateTime scheduleDate)
        {
            Log.Info("Start SendSaasTariffLetters");

            List<Tenant> activeTenants;

            using (var scope = ServiceProvider.CreateScope())
            {
                var tenantManager = scope.ServiceProvider.GetService<TenantManager>();

                activeTenants = tenantManager.GetTenants().ToList();

                if (activeTenants.Count <= 0)
                {
                    Log.Info("End SendSaasTariffLetters");
                    return Task.CompletedTask;
                }
            }

            return InternalSendSaasLettersAsync(senderName, scheduleDate, activeTenants);
        }

        private async Task InternalSendSaasLettersAsync(string senderName, DateTime scheduleDate, List<Tenant> activeTenants)
        {
            var nowDate = scheduleDate.Date;

            foreach (var tenant in activeTenants)
            {
                try
                {
                    using var scope = ServiceProvider.CreateScope();
                    var scopeClass = scope.ServiceProvider.GetService<StudioPeriodicNotifyScope>();
                    var (tenantManager, userManager, studioNotifyHelper, paymentManager, tenantExtra, authContext, commonLinkUtility, apiSystemHelper, setupInfo, dbContextManager, couponManager, _, _, coreBaseSettings, _, _, _) = scopeClass;
                    tenantManager.SetCurrentTenant(tenant.TenantId);
                    var client = WorkContext.NotifyContext.NotifyService.RegisterClient(studioNotifyHelper.NotifySource, scope);

                    var tariff = paymentManager.GetTariff(tenant.TenantId);
                    var quota = tenantManager.GetTenantQuota(tenant.TenantId);
                    var createdDate = tenant.CreatedDateTime.Date;

                    var dueDateIsNotMax = tariff.DueDate != DateTime.MaxValue;
                    var dueDate = tariff.DueDate.Date;

                    var delayDueDateIsNotMax = tariff.DelayDueDate != DateTime.MaxValue;
                    var delayDueDate = tariff.DelayDueDate.Date;

                    INotifyAction action = null;
                    var paymentMessage = true;

                    var toadmins = false;
                    var tousers = false;
                    var toowner = false;

                    var coupon = string.Empty;

                    Func<string> greenButtonText = () => string.Empty;

                    string blueButtonText() => WebstudioNotifyPatternResource.ButtonRequestCallButton;
                    var greenButtonUrl = string.Empty;

                    Func<string> tableItemText1 = () => string.Empty;
                    Func<string> tableItemText2 = () => string.Empty;
                    Func<string> tableItemText3 = () => string.Empty;
                    Func<string> tableItemText4 = () => string.Empty;
                    Func<string> tableItemText5 = () => string.Empty;
                    Func<string> tableItemText6 = () => string.Empty;
                    Func<string> tableItemText7 = () => string.Empty;

                    var tableItemUrl1 = string.Empty;
                    var tableItemUrl2 = string.Empty;
                    var tableItemUrl3 = string.Empty;
                    var tableItemUrl4 = string.Empty;
                    var tableItemUrl5 = string.Empty;
                    var tableItemUrl6 = string.Empty;
                    var tableItemUrl7 = string.Empty;

                    var tableItemImg1 = string.Empty;
                    var tableItemImg2 = string.Empty;
                    var tableItemImg3 = string.Empty;
                    var tableItemImg4 = string.Empty;
                    var tableItemImg5 = string.Empty;
                    var tableItemImg6 = string.Empty;
                    var tableItemImg7 = string.Empty;

                    Func<string> tableItemComment1 = () => string.Empty;
                    Func<string> tableItemComment2 = () => string.Empty;
                    Func<string> tableItemComment3 = () => string.Empty;
                    Func<string> tableItemComment4 = () => string.Empty;
                    Func<string> tableItemComment5 = () => string.Empty;
                    Func<string> tableItemComment6 = () => string.Empty;
                    Func<string> tableItemComment7 = () => string.Empty;

                    Func<string> tableItemLearnMoreText1 = () => string.Empty;

                    string tableItemLearnMoreText2() => string.Empty;

                    string tableItemLearnMoreText3() => string.Empty;

                    string tableItemLearnMoreText4() => string.Empty;

                    string tableItemLearnMoreText5() => string.Empty;

                    string tableItemLearnMoreText6() => string.Empty;

                    string tableItemLearnMoreText7() => string.Empty;

                    var tableItemLearnMoreUrl1 = string.Empty;
                    var tableItemLearnMoreUrl2 = string.Empty;
                    var tableItemLearnMoreUrl3 = string.Empty;
                    var tableItemLearnMoreUrl4 = string.Empty;
                    var tableItemLearnMoreUrl5 = string.Empty;
                    var tableItemLearnMoreUrl6 = string.Empty;
                    var tableItemLearnMoreUrl7 = string.Empty;


                    if (quota.Free)
                    {
                        #region Free tariff every 2 months during 1 year

                        if (createdDate.AddMonths(2) == nowDate || createdDate.AddMonths(4) == nowDate || createdDate.AddMonths(6) == nowDate || createdDate.AddMonths(8) == nowDate || createdDate.AddMonths(10) == nowDate || createdDate.AddMonths(12) == nowDate)
                        {
                            action = Actions.SaasAdminPaymentWarningEvery2MonthsV115;
                            toadmins = true;

                            greenButtonText = () => WebstudioNotifyPatternResource.ButtonUseDiscount;
                            greenButtonUrl = commonLinkUtility.GetFullAbsolutePath("~/Tariffs.aspx");
                        }

                        #endregion
                    }
                    else if (quota.Trial)
                    {
                        #region After registration letters

                        #region 1 days after registration to admins SAAS TRIAL

                        if (createdDate.AddDays(1) == nowDate)
                        {
                            action = Actions.SaasAdminModulesV115;
                            paymentMessage = false;
                            toadmins = true;

                            greenButtonText = () => WebstudioNotifyPatternResource.ButtonAccessYouWebOffice;
                            greenButtonUrl = $"{commonLinkUtility.GetFullAbsolutePath("~").TrimEnd('/')}/";
                        }

                        #endregion

                        #region  4 days after registration to admins SAAS TRIAL

                        else if (createdDate.AddDays(4) == nowDate)
                        {
                            action = Actions.SaasAdminComfortTipsV115;
                            paymentMessage = false;
                            toadmins = true;

                            greenButtonText = () => WebstudioNotifyPatternResource.ButtonUseDiscount;
                            greenButtonUrl = commonLinkUtility.GetFullAbsolutePath("~/Tariffs.aspx");
                        }

                        #endregion

                        #region 7 days after registration to admins and users SAAS TRIAL

                        else if (createdDate.AddDays(7) == nowDate)
                        {
                            action = Actions.SaasAdminUserDocsTipsV115;
                            paymentMessage = false;
                            toadmins = true;
                            tousers = true;

                            tableItemImg1 = studioNotifyHelper.GetNotificationImageUrl("tips-documents-formatting-100.png");
                            tableItemText1 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_formatting_hdr;
                            tableItemComment1 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_formatting;

                            if (!coreBaseSettings.CustomMode)
                            {
                                tableItemLearnMoreUrl1 = studioNotifyHelper.Helplink + "/onlyoffice-editors/index.aspx";
                                tableItemLearnMoreText1 = () => WebstudioNotifyPatternResource.LinkLearnMore;
                            }

                            tableItemImg2 = studioNotifyHelper.GetNotificationImageUrl("tips-documents-share-100.png");
                            tableItemText2 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_share_hdr;
                            tableItemComment2 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_share;

                            tableItemImg3 = studioNotifyHelper.GetNotificationImageUrl("tips-documents-coediting-100.png");
                            tableItemText3 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_coediting_hdr;
                            tableItemComment3 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_coediting;

                            tableItemImg4 = studioNotifyHelper.GetNotificationImageUrl("tips-documents-review-100.png");
                            tableItemText4 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_review_hdr;
                            tableItemComment4 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_review;

                            tableItemImg5 = studioNotifyHelper.GetNotificationImageUrl("tips-customize-modules-100.png");
                            tableItemText5 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_contentcontrols_hdr;
                            tableItemComment5 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_contentcontrols;

                            tableItemImg6 = studioNotifyHelper.GetNotificationImageUrl("tips-customize-customize-100.png");
                            tableItemText6 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_spreadsheets_hdr;
                            tableItemComment6 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_spreadsheets;

                            tableItemImg7 = studioNotifyHelper.GetNotificationImageUrl("tips-documents-attach-100.png");
                            tableItemText7 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_differences_hdr;
                            tableItemComment7 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_differences;

                            greenButtonText = () => WebstudioNotifyPatternResource.ButtonAccessYouWebOffice;
                            greenButtonUrl = $"{commonLinkUtility.GetFullAbsolutePath("~").TrimEnd('/')}/products/files/";
                        }

                        #endregion

                        #region 14 days after registration to admins and users SAAS TRIAL

                        else if (createdDate.AddDays(14) == nowDate)
                        {
                            action = Actions.SaasAdminUserAppsTipsV115;
                            paymentMessage = false;
                            toadmins = true;
                            tousers = true;
                        }

                        #endregion

                        #endregion

                        #region Trial warning letters

                        #region 5 days before SAAS TRIAL ends to admins

                        else if (!coreBaseSettings.CustomMode && dueDateIsNotMax && dueDate.AddDays(-5) == nowDate)
                        {
                            toadmins = true;
                            action = Actions.SaasAdminTrialWarningBefore5V115;
                            coupon = "PortalCreation10%";

                            if (string.IsNullOrEmpty(coupon))
                            {
                                try
                                {
                                    Log.InfoFormat("start CreateCoupon to {0}", tenant.TenantAlias);

                                    coupon = SetupInfo.IsSecretEmail(userManager.GetUsers(tenant.OwnerId).Email)
                                                ? tenant.TenantAlias
                                                : couponManager.CreateCoupon(tenantManager);

                                    Log.InfoFormat("end CreateCoupon to {0} coupon = {1}", tenant.TenantAlias, coupon);
                                }
                                catch (AggregateException ae)
                                {
                                    foreach (var ex in ae.InnerExceptions)
                                        Log.Error(ex);
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex);
                                }
                            }

                            greenButtonText = () => WebstudioNotifyPatternResource.ButtonUseDiscount;
                            greenButtonUrl = commonLinkUtility.GetFullAbsolutePath("~/tariffs.aspx");
                        }

                        #endregion

                        #region SAAS TRIAL expires today to admins

                        else if (dueDate == nowDate)
                        {
                            action = Actions.SaasAdminTrialWarningV115;
                            toadmins = true;
                        }

                        #endregion

                        #region 1 day after SAAS TRIAL expired to admins

                        if (dueDateIsNotMax && dueDate.AddDays(1) == nowDate)
                        {
                            action = Actions.SaasAdminTrialWarningAfter1V115;
                            toadmins = true;

                            greenButtonText = () => WebstudioNotifyPatternResource.ButtonRenewNow;
                            greenButtonUrl = commonLinkUtility.GetFullAbsolutePath("~/Tariffs.aspx");
                        }

                        #region 6 months after SAAS TRIAL expired

                        else if (dueDateIsNotMax && dueDate.AddMonths(6) == nowDate)
                        {
                            action = Actions.SaasAdminTrialWarningAfterHalfYearV115;
                            toowner = true;

                            greenButtonText = () => WebstudioNotifyPatternResource.ButtonLeaveFeedback;

                            var owner = userManager.GetUsers(tenant.OwnerId);
                            greenButtonUrl = setupInfo.TeamlabSiteRedirect + "/remove-portal-feedback-form.aspx#" +
                                          System.Web.HttpUtility.UrlEncode(Convert.ToBase64String(
                                              System.Text.Encoding.UTF8.GetBytes("{\"firstname\":\"" + owner.FirstName +
                                                                                 "\",\"lastname\":\"" + owner.LastName +
                                                                                 "\",\"alias\":\"" + tenant.TenantAlias +
                                                                                 "\",\"email\":\"" + owner.Email + "\"}")));
                        }
                        else if (dueDateIsNotMax && dueDate.AddMonths(6).AddDays(7) <= nowDate)
                        {
                            tenantManager.RemoveTenant(tenant.TenantId, true);

                            if (!string.IsNullOrEmpty(apiSystemHelper.ApiCacheUrl))
                            {
                                await apiSystemHelper.RemoveTenantFromCacheAsync(tenant.TenantAlias, authContext.CurrentAccount.ID);
                            }
                        }

                        #endregion

                        #endregion

                        #endregion
                    }

                    else if (tariff.State >= TariffState.Paid)
                    {
                        #region Payment warning letters

                        #region 6 months after SAAS PAID expired

                        if (tariff.State == TariffState.NotPaid && dueDateIsNotMax && dueDate.AddMonths(6) == nowDate)
                        {
                            action = Actions.SaasAdminTrialWarningAfterHalfYearV115;
                            toowner = true;

                            greenButtonText = () => WebstudioNotifyPatternResource.ButtonLeaveFeedback;

                            var owner = userManager.GetUsers(tenant.OwnerId);
                            greenButtonUrl = setupInfo.TeamlabSiteRedirect + "/remove-portal-feedback-form.aspx#" +
                                          System.Web.HttpUtility.UrlEncode(Convert.ToBase64String(
                                              System.Text.Encoding.UTF8.GetBytes("{\"firstname\":\"" + owner.FirstName +
                                                                                 "\",\"lastname\":\"" + owner.LastName +
                                                                                 "\",\"alias\":\"" + tenant.TenantAlias +
                                                                                 "\",\"email\":\"" + owner.Email + "\"}")));
                        }
                        else if (tariff.State == TariffState.NotPaid && dueDateIsNotMax && dueDate.AddMonths(6).AddDays(7) <= nowDate)
                        {
                            tenantManager.RemoveTenant(tenant.TenantId, true);

                            if (!string.IsNullOrEmpty(apiSystemHelper.ApiCacheUrl))
                            {
                                await apiSystemHelper.RemoveTenantFromCacheAsync(tenant.TenantAlias, authContext.CurrentAccount.ID);
                            }
                        }

                        #endregion

                        #endregion
                    }


                    if (action == null) continue;

                    var users = toowner
                                    ? new List<UserInfo> { userManager.GetUsers(tenant.OwnerId) }
                                    : studioNotifyHelper.GetRecipients(toadmins, tousers, false);

                    foreach (var u in users.Where(u => paymentMessage || studioNotifyHelper.IsSubscribedToNotify(u, Actions.PeriodicNotify)))
                    {
                        var culture = string.IsNullOrEmpty(u.CultureName) ? tenant.GetCulture() : u.GetCulture();
                        Thread.CurrentThread.CurrentCulture = culture;
                        Thread.CurrentThread.CurrentUICulture = culture;
                        var rquota = tenantExtra.GetRightQuota() ?? TenantQuota.Default;

                        client.SendNoticeToAsync(
                            action,
                            new[] { studioNotifyHelper.ToRecipient(u.ID) },
                            new[] { senderName },
                            new TagValue(Tags.UserName, u.FirstName.HtmlEncode()),
                            new TagValue(Tags.PricingPage, commonLinkUtility.GetFullAbsolutePath("~/tariffs.aspx")),
                            new TagValue(Tags.ActiveUsers, userManager.GetUsers().Length),
                            new TagValue(Tags.Price, rquota.Price),
                            new TagValue(Tags.PricePeriod, rquota.Year3 ? UserControlsCommonResource.TariffPerYear3 : rquota.Year ? UserControlsCommonResource.TariffPerYear : UserControlsCommonResource.TariffPerMonth),
                            new TagValue(Tags.DueDate, dueDate.ToLongDateString()),
                            new TagValue(Tags.DelayDueDate, (delayDueDateIsNotMax ? delayDueDate : dueDate).ToLongDateString()),
                            TagValues.BlueButton(blueButtonText, "http://www.onlyoffice.com/call-back-form.aspx"),
                            TagValues.GreenButton(greenButtonText, greenButtonUrl),
                            TagValues.TableTop(),
                            TagValues.TableItem(1, tableItemText1, tableItemUrl1, tableItemImg1, tableItemComment1, tableItemLearnMoreText1, tableItemLearnMoreUrl1),
                            TagValues.TableItem(2, tableItemText2, tableItemUrl2, tableItemImg2, tableItemComment2, tableItemLearnMoreText2, tableItemLearnMoreUrl2),
                            TagValues.TableItem(3, tableItemText3, tableItemUrl3, tableItemImg3, tableItemComment3, tableItemLearnMoreText3, tableItemLearnMoreUrl3),
                            TagValues.TableItem(4, tableItemText4, tableItemUrl4, tableItemImg4, tableItemComment4, tableItemLearnMoreText4, tableItemLearnMoreUrl4),
                            TagValues.TableItem(5, tableItemText5, tableItemUrl5, tableItemImg5, tableItemComment5, tableItemLearnMoreText5, tableItemLearnMoreUrl5),
                            TagValues.TableItem(6, tableItemText6, tableItemUrl6, tableItemImg6, tableItemComment6, tableItemLearnMoreText6, tableItemLearnMoreUrl6),
                            TagValues.TableItem(7, tableItemText7, tableItemUrl7, tableItemImg7, tableItemComment7, tableItemLearnMoreText7, tableItemLearnMoreUrl7),
                            TagValues.TableBottom(),
                            new TagValue(CommonTags.Footer, u.IsAdmin(userManager) ? "common" : "social"),
                            new TagValue(Tags.Coupon, coupon));
                    }
                }
                catch (Exception err)
                {
                    Log.Error(err);
                }
            }

            Log.Info("End SendSaasTariffLetters");
        }

        public void SendEnterpriseLetters(string senderName, DateTime scheduleDate)
        {
            var nowDate = scheduleDate.Date;
            const string dbid = "webstudio";

            Log.Info("Start SendTariffEnterpriseLetters");

            List<Tenant> activeTenants;

            using (var scope = ServiceProvider.CreateScope())
            {
                var tenantManager = scope.ServiceProvider.GetService<TenantManager>();

                activeTenants = tenantManager.GetTenants().ToList();

                if (activeTenants.Count <= 0)
                {
                    Log.Info("End SendTariffEnterpriseLetters");
                    return;
                }
            }

            foreach (var tenant in activeTenants)
            {
                try
                {
                    using var scope = ServiceProvider.CreateScope();
                    var scopeClass = scope.ServiceProvider.GetService<StudioPeriodicNotifyScope>();
                    var (tenantManager, userManager, studioNotifyHelper, paymentManager, tenantExtra, _, commonLinkUtility, _, _, dbContextManager, _, configuration, settingsManager, coreBaseSettings, _, _, _) = scopeClass;
                    var defaultRebranding = MailWhiteLabelSettings.IsDefault(settingsManager, configuration);
                    tenantManager.SetCurrentTenant(tenant.TenantId);
                    var client = WorkContext.NotifyContext.NotifyService.RegisterClient(studioNotifyHelper.NotifySource, scope);

                    var tariff = paymentManager.GetTariff(tenant.TenantId);
                    var quota = tenantManager.GetTenantQuota(tenant.TenantId);
                    var createdDate = tenant.CreatedDateTime.Date;

                    var actualEndDate = tariff.DueDate != DateTime.MaxValue ? tariff.DueDate : tariff.LicenseDate;
                    var dueDateIsNotMax = actualEndDate != DateTime.MaxValue;
                    var dueDate = actualEndDate.Date;

                    var delayDueDateIsNotMax = tariff.DelayDueDate != DateTime.MaxValue;
                    var delayDueDate = tariff.DelayDueDate.Date;

                    INotifyAction action = null;
                    var paymentMessage = true;

                    var toadmins = false;
                    var tousers = false;

                    Func<string> greenButtonText = () => string.Empty;

                    string blueButtonText() => WebstudioNotifyPatternResource.ButtonRequestCallButton;
                    var greenButtonUrl = string.Empty;

                    Func<string> tableItemText1 = () => string.Empty;
                    Func<string> tableItemText2 = () => string.Empty;
                    Func<string> tableItemText3 = () => string.Empty;
                    Func<string> tableItemText4 = () => string.Empty;
                    Func<string> tableItemText5 = () => string.Empty;
                    Func<string> tableItemText6 = () => string.Empty;
                    Func<string> tableItemText7 = () => string.Empty;

                    var tableItemUrl1 = string.Empty;
                    var tableItemUrl2 = string.Empty;
                    var tableItemUrl3 = string.Empty;
                    var tableItemUrl4 = string.Empty;
                    var tableItemUrl5 = string.Empty;
                    var tableItemUrl6 = string.Empty;
                    var tableItemUrl7 = string.Empty;

                    var tableItemImg1 = string.Empty;
                    var tableItemImg2 = string.Empty;
                    var tableItemImg3 = string.Empty;
                    var tableItemImg4 = string.Empty;
                    var tableItemImg5 = string.Empty;
                    var tableItemImg6 = string.Empty;
                    var tableItemImg7 = string.Empty;

                    Func<string> tableItemComment1 = () => string.Empty;
                    Func<string> tableItemComment2 = () => string.Empty;
                    Func<string> tableItemComment3 = () => string.Empty;
                    Func<string> tableItemComment4 = () => string.Empty;
                    Func<string> tableItemComment5 = () => string.Empty;
                    Func<string> tableItemComment6 = () => string.Empty;
                    Func<string> tableItemComment7 = () => string.Empty;

                    Func<string> tableItemLearnMoreText1 = () => string.Empty;

                    string tableItemLearnMoreText2() => string.Empty;

                    string tableItemLearnMoreText3() => string.Empty;

                    string tableItemLearnMoreText4() => string.Empty;

                    string tableItemLearnMoreText5() => string.Empty;

                    string tableItemLearnMoreText6() => string.Empty;

                    string tableItemLearnMoreText7() => string.Empty;

                    var tableItemLearnMoreUrl1 = string.Empty;
                    var tableItemLearnMoreUrl2 = string.Empty;
                    var tableItemLearnMoreUrl3 = string.Empty;
                    var tableItemLearnMoreUrl4 = string.Empty;
                    var tableItemLearnMoreUrl5 = string.Empty;
                    var tableItemLearnMoreUrl6 = string.Empty;
                    var tableItemLearnMoreUrl7 = string.Empty;


                    if (quota.Trial && defaultRebranding)
                    {
                        #region After registration letters

                        #region 1 day after registration to admins ENTERPRISE TRIAL + defaultRebranding

                        if (createdDate.AddDays(1) == nowDate)
                        {
                            action = Actions.EnterpriseAdminCustomizePortalV10;
                            paymentMessage = false;
                            toadmins = true;

                            tableItemImg1 = studioNotifyHelper.GetNotificationImageUrl("tips-customize-brand-100.png");
                            tableItemText1 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_brand_hdr;
                            tableItemComment1 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_brand;

                            tableItemImg2 = studioNotifyHelper.GetNotificationImageUrl("tips-customize-regional-100.png");
                            tableItemText2 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_regional_hdr;
                            tableItemComment2 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_regional;

                            tableItemImg3 = studioNotifyHelper.GetNotificationImageUrl("tips-customize-customize-100.png");
                            tableItemText3 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_customize_hdr;
                            tableItemComment3 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_customize;

                            tableItemImg4 = studioNotifyHelper.GetNotificationImageUrl("tips-customize-modules-100.png");
                            tableItemText4 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_modules_hdr;
                            tableItemComment4 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_modules;

                            tableItemImg5 = studioNotifyHelper.GetNotificationImageUrl("tips-customize-3rdparty-100.png");
                            tableItemText5 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_3rdparty_hdr;
                            tableItemComment5 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_3rdparty;

                            greenButtonText = () => WebstudioNotifyPatternResource.ButtonConfigureRightNow;
                            greenButtonUrl = commonLinkUtility.GetFullAbsolutePath(commonLinkUtility.GetAdministration(ManagementType.General));
                        }

                        #endregion

                        #region 4 days after registration to admins ENTERPRISE TRIAL + only 1 user + defaultRebranding

                        else if (createdDate.AddDays(4) == nowDate && userManager.GetUsers().Length == 1)
                        {
                            action = Actions.EnterpriseAdminInviteTeammatesV10;
                            paymentMessage = false;
                            toadmins = true;

                            greenButtonText = () => WebstudioNotifyPatternResource.ButtonInviteRightNow;
                            greenButtonUrl = $"{commonLinkUtility.GetFullAbsolutePath("~").TrimEnd('/')}/products/people/";
                        }

                        #endregion

                        #region 5 days after registration to admins ENTERPRISE TRAIL + without activity in 1 or more days + defaultRebranding

                        else if (createdDate.AddDays(5) == nowDate)
                        {
                            List<DateTime> datesWithActivity;

                            datesWithActivity =
                                dbContextManager.Get(dbid).FeedAggregates
                                .Where(r => r.Tenant == tenantManager.GetCurrentTenant().TenantId)
                                .Where(r => r.CreatedDate <= nowDate.AddDays(-1))
                                .GroupBy(r => r.CreatedDate.Date)
                                .Select(r => r.Key)
                                .ToList();

                            if (datesWithActivity.Count < 5)
                            {
                                action = Actions.EnterpriseAdminWithoutActivityV10;
                                paymentMessage = false;
                                toadmins = true;
                            }
                        }

                        #endregion

                        #region 7 days after registration to admins and users ENTERPRISE TRIAL + defaultRebranding

                        else if (createdDate.AddDays(7) == nowDate)
                        {
                            action = Actions.EnterpriseAdminUserDocsTipsV10;
                            paymentMessage = false;
                            toadmins = true;
                            tousers = true;

                            tableItemImg1 = studioNotifyHelper.GetNotificationImageUrl("tips-documents-formatting-100.png");
                            tableItemText1 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_formatting_hdr;
                            tableItemComment1 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_formatting;

                            if (!coreBaseSettings.CustomMode)
                            {
                                tableItemLearnMoreUrl1 = studioNotifyHelper.Helplink + "/onlyoffice-editors/index.aspx";
                                tableItemLearnMoreText1 = () => WebstudioNotifyPatternResource.LinkLearnMore;
                            }

                            tableItemImg2 = studioNotifyHelper.GetNotificationImageUrl("tips-documents-share-100.png");
                            tableItemText2 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_share_hdr;
                            tableItemComment2 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_share;

                            tableItemImg3 = studioNotifyHelper.GetNotificationImageUrl("tips-documents-coediting-100.png");
                            tableItemText3 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_coediting_hdr;
                            tableItemComment3 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_coediting;

                            tableItemImg4 = studioNotifyHelper.GetNotificationImageUrl("tips-documents-review-100.png");
                            tableItemText4 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_review_hdr;
                            tableItemComment4 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_review;

                            tableItemImg5 = studioNotifyHelper.GetNotificationImageUrl("tips-documents-3rdparty-100.png");
                            tableItemText5 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_3rdparty_hdr;
                            tableItemComment5 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_3rdparty;

                            tableItemImg6 = studioNotifyHelper.GetNotificationImageUrl("tips-documents-attach-100.png");
                            tableItemText6 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_attach_hdr;
                            tableItemComment6 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_attach;

                            tableItemImg7 = studioNotifyHelper.GetNotificationImageUrl("tips-documents-apps-100.png");
                            tableItemText7 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_apps_hdr;
                            tableItemComment7 = () => WebstudioNotifyPatternResource.pattern_saas_admin_user_docs_tips_v115_item_apps;

                            greenButtonText = () => WebstudioNotifyPatternResource.ButtonAccessYouWebOffice;
                            greenButtonUrl = $"{commonLinkUtility.GetFullAbsolutePath("~").TrimEnd('/')}/products/files/";
                        }

                        #endregion

                        #region 21 days after registration to admins and users ENTERPRISE TRIAL + defaultRebranding

                        else if (createdDate.AddDays(21) == nowDate)
                        {
                            action = Actions.EnterpriseAdminUserAppsTipsV10;
                            paymentMessage = false;
                            toadmins = true;
                            tousers = true;
                        }

                        #endregion

                        #endregion

                        #region Trial warning letters

                        #region 7 days before ENTERPRISE TRIAL ends to admins + defaultRebranding

                        else if (dueDateIsNotMax && dueDate.AddDays(-7) == nowDate)
                        {
                            action = Actions.EnterpriseAdminTrialWarningBefore7V10;
                            toadmins = true;

                            greenButtonText = () => WebstudioNotifyPatternResource.ButtonSelectPricingPlans;
                            greenButtonUrl = "http://www.onlyoffice.com/enterprise-edition.aspx";
                        }

                        #endregion

                        #region ENTERPRISE TRIAL expires today to admins + defaultRebranding

                        else if (dueDate == nowDate)
                        {
                            action = Actions.EnterpriseAdminTrialWarningV10;
                            toadmins = true;
                        }

                        #endregion

                        #endregion
                    }
                    else if (quota.Trial && !defaultRebranding)
                    {
                        #region After registration letters

                        #region 1 day after registration to admins ENTERPRISE TRIAL + !defaultRebranding

                        if (createdDate.AddDays(1) == nowDate)
                        {
                            action = Actions.EnterpriseWhitelabelAdminCustomizePortalV10;
                            paymentMessage = false;
                            toadmins = true;

                            tableItemImg1 = studioNotifyHelper.GetNotificationImageUrl("tips-customize-brand-100.png");
                            tableItemText1 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_brand_hdr;
                            tableItemComment1 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_brand;

                            tableItemImg2 = studioNotifyHelper.GetNotificationImageUrl("tips-customize-regional-100.png");
                            tableItemText2 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_regional_hdr;
                            tableItemComment2 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_regional;

                            tableItemImg3 = studioNotifyHelper.GetNotificationImageUrl("tips-customize-customize-100.png");
                            tableItemText3 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_customize_hdr;
                            tableItemComment3 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_customize;

                            tableItemImg4 = studioNotifyHelper.GetNotificationImageUrl("tips-customize-modules-100.png");
                            tableItemText4 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_modules_hdr;
                            tableItemComment4 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_modules;

                            if (!coreBaseSettings.CustomMode)
                            {
                                tableItemImg5 = studioNotifyHelper.GetNotificationImageUrl("tips-customize-3rdparty-100.png");
                                tableItemText5 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_3rdparty_hdr;
                                tableItemComment5 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_3rdparty;
                            }

                            greenButtonText = () => WebstudioNotifyPatternResource.ButtonConfigureRightNow;
                            greenButtonUrl = commonLinkUtility.GetFullAbsolutePath(commonLinkUtility.GetAdministration(ManagementType.General));
                        }

                        #endregion

                        #endregion
                    }
                    else if (tariff.State == TariffState.Paid)
                    {
                        #region Payment warning letters

                        #region 7 days before ENTERPRISE PAID expired to admins

                        if (dueDateIsNotMax && dueDate.AddDays(-7) == nowDate)
                        {
                            action = defaultRebranding
                                         ? Actions.EnterpriseAdminPaymentWarningBefore7V10
                                         : Actions.EnterpriseWhitelabelAdminPaymentWarningBefore7V10;
                            toadmins = true;
                            greenButtonText = () => WebstudioNotifyPatternResource.ButtonSelectPricingPlans;
                            greenButtonUrl = commonLinkUtility.GetFullAbsolutePath("~/tariffs.aspx");
                        }

                        #endregion

                        #region ENTERPRISE PAID expires today to admins

                        else if (dueDate == nowDate)
                        {
                            action = defaultRebranding
                                         ? Actions.EnterpriseAdminPaymentWarningV10
                                         : Actions.EnterpriseWhitelabelAdminPaymentWarningV10;
                            toadmins = true;
                            greenButtonText = () => WebstudioNotifyPatternResource.ButtonSelectPricingPlans;
                            greenButtonUrl = commonLinkUtility.GetFullAbsolutePath("~/tariffs.aspx");
                        }

                        #endregion

                        #endregion
                    }


                    if (action == null) continue;

                    var users = studioNotifyHelper.GetRecipients(toadmins, tousers, false);

                    foreach (var u in users.Where(u => paymentMessage || studioNotifyHelper.IsSubscribedToNotify(u, Actions.PeriodicNotify)))
                    {
                        var culture = string.IsNullOrEmpty(u.CultureName) ? tenant.GetCulture() : u.GetCulture();
                        Thread.CurrentThread.CurrentCulture = culture;
                        Thread.CurrentThread.CurrentUICulture = culture;

                        var rquota = tenantExtra.GetRightQuota() ?? TenantQuota.Default;

                        client.SendNoticeToAsync(
                            action,
                            new[] { studioNotifyHelper.ToRecipient(u.ID) },
                            new[] { senderName },
                            new TagValue(Tags.UserName, u.FirstName.HtmlEncode()),
                            new TagValue(Tags.PricingPage, commonLinkUtility.GetFullAbsolutePath("~/tariffs.aspx")),
                            new TagValue(Tags.ActiveUsers, userManager.GetUsers().Length),
                            new TagValue(Tags.Price, rquota.Price),
                            new TagValue(Tags.PricePeriod, rquota.Year3 ? UserControlsCommonResource.TariffPerYear3 : rquota.Year ? UserControlsCommonResource.TariffPerYear : UserControlsCommonResource.TariffPerMonth),
                            new TagValue(Tags.DueDate, dueDate.ToLongDateString()),
                            new TagValue(Tags.DelayDueDate, (delayDueDateIsNotMax ? delayDueDate : dueDate).ToLongDateString()),
                            TagValues.BlueButton(blueButtonText, "http://www.onlyoffice.com/call-back-form.aspx"),
                            TagValues.GreenButton(greenButtonText, greenButtonUrl),
                            TagValues.TableTop(),
                            TagValues.TableItem(1, tableItemText1, tableItemUrl1, tableItemImg1, tableItemComment1, tableItemLearnMoreText1, tableItemLearnMoreUrl1),
                            TagValues.TableItem(2, tableItemText2, tableItemUrl2, tableItemImg2, tableItemComment2, tableItemLearnMoreText2, tableItemLearnMoreUrl2),
                            TagValues.TableItem(3, tableItemText3, tableItemUrl3, tableItemImg3, tableItemComment3, tableItemLearnMoreText3, tableItemLearnMoreUrl3),
                            TagValues.TableItem(4, tableItemText4, tableItemUrl4, tableItemImg4, tableItemComment4, tableItemLearnMoreText4, tableItemLearnMoreUrl4),
                            TagValues.TableItem(5, tableItemText5, tableItemUrl5, tableItemImg5, tableItemComment5, tableItemLearnMoreText5, tableItemLearnMoreUrl5),
                            TagValues.TableItem(6, tableItemText6, tableItemUrl6, tableItemImg6, tableItemComment6, tableItemLearnMoreText6, tableItemLearnMoreUrl6),
                            TagValues.TableItem(7, tableItemText7, tableItemUrl7, tableItemImg7, tableItemComment7, tableItemLearnMoreText7, tableItemLearnMoreUrl7),
                            TagValues.TableBottom());
                    }
                }
                catch (Exception err)
                {
                    Log.Error(err);
                }
            }

            Log.Info("End SendTariffEnterpriseLetters");
        }

        public void SendOpensourceLetters(string senderName, DateTime scheduleDate)
        {
            var nowDate = scheduleDate.Date;

            Log.Info("Start SendOpensourceTariffLetters");

            List<Tenant> activeTenants;

            using (var scope = ServiceProvider.CreateScope())
            {
                var tenantManager = scope.ServiceProvider.GetService<TenantManager>();

                activeTenants = tenantManager.GetTenants().ToList();

                if (activeTenants.Count <= 0)
                {
                    Log.Info("End SendOpensourceTariffLetters");
                    return;
                }
            }

            foreach (var tenant in activeTenants)
            {
                try
                {
                    using var scope = ServiceProvider.CreateScope();
                    var scopeClass = scope.ServiceProvider.GetService<StudioPeriodicNotifyScope>();
                    var (tenantManager, userManager, studioNotifyHelper, _, _, _, _, _, _, _, _, _, _, _, displayUserSettingsHelper, _, _) = scopeClass;
                    tenantManager.SetCurrentTenant(tenant.TenantId);
                    var client = WorkContext.NotifyContext.NotifyService.RegisterClient(studioNotifyHelper.NotifySource, scope);

                    var createdDate = tenant.CreatedDateTime.Date;


                    #region After registration letters

                    #region 5 days after registration to admins

                    if (createdDate.AddDays(5) == nowDate)
                    {
                        var users = studioNotifyHelper.GetRecipients(true, true, false);


                        foreach (var u in users.Where(u => studioNotifyHelper.IsSubscribedToNotify(u, Actions.PeriodicNotify)))
                        {
                            var culture = string.IsNullOrEmpty(u.CultureName) ? tenant.GetCulture() : u.GetCulture();
                            Thread.CurrentThread.CurrentCulture = culture;
                            Thread.CurrentThread.CurrentUICulture = culture;

                            client.SendNoticeToAsync(
                                u.IsAdmin(userManager) ? Actions.OpensourceAdminDocsTipsV11 : Actions.OpensourceUserDocsTipsV11,
                                new[] { studioNotifyHelper.ToRecipient(u.ID) },
                                new[] { senderName },
                                new TagValue(Tags.UserName, u.DisplayUserName(displayUserSettingsHelper)),
                                new TagValue(CommonTags.Footer, "opensource"));
                        }
                    }
                    #endregion

                    #endregion
                }
                catch (Exception err)
                {
                    Log.Error(err);
                }
            }

            Log.Info("End SendOpensourceTariffLetters");
        }

        public void SendPersonalLetters(string senderName, DateTime scheduleDate)
        {
            Log.Info("Start SendLettersPersonal...");


            List<Tenant> activeTenants;

            using (var scope = ServiceProvider.CreateScope())
            {
                var tenantManager = scope.ServiceProvider.GetService<TenantManager>();

                activeTenants = tenantManager.GetTenants().ToList();
            }

            foreach (var tenant in activeTenants)
            {
                try
                {
                    Func<string> greenButtonText = () => string.Empty;
                    var greenButtonUrl = string.Empty;

                    var sendCount = 0;

                    using var scope = ServiceProvider.CreateScope();
                    var scopeClass = scope.ServiceProvider.GetService<StudioPeriodicNotifyScope>();
                    var (tenantManager, userManager, studioNotifyHelper, _, _, _, _, _, _, _, _, _, _, coreBaseSettings, _, authManager, securityContext) = scopeClass;

                    tenantManager.SetCurrentTenant(tenant.TenantId);
                    var client = WorkContext.NotifyContext.NotifyService.RegisterClient(studioNotifyHelper.NotifySource, scope);

                    Log.InfoFormat("Current tenant: {0}", tenant.TenantId);

                    var users = userManager.GetUsers(EmployeeStatus.Active);

                    foreach (var user in users.Where(u => studioNotifyHelper.IsSubscribedToNotify(u, Actions.PeriodicNotify)))
                    {
                        INotifyAction action;

                        securityContext.AuthenticateMeWithoutCookie(authManager.GetAccountByID(tenant.TenantId, user.ID));

                        var culture = tenant.GetCulture();
                        if (!string.IsNullOrEmpty(user.CultureName))
                        {
                            try
                            {
                                culture = user.GetCulture();
                            }
                            catch (CultureNotFoundException exception)
                            {

                                Log.Error(exception);
                            }
                        }

                        Thread.CurrentThread.CurrentCulture = culture;
                        Thread.CurrentThread.CurrentUICulture = culture;

                        var dayAfterRegister = (int)scheduleDate.Date.Subtract(user.CreateDate.Date).TotalDays;

                        if (coreBaseSettings.CustomMode)
                        {
                            switch (dayAfterRegister)
                            {
                                case 7:
                                    action = Actions.PersonalCustomModeAfterRegistration7;
                                    break;
                                default:
                                    continue;
                            }
                        }
                        else
                        {

                            switch (dayAfterRegister)
                            {
                                case 7:
                                    action = Actions.PersonalAfterRegistration7;
                                    break;
                                case 14:
                                    action = Actions.PersonalAfterRegistration14;
                                    break;
                                case 21:
                                    action = Actions.PersonalAfterRegistration21;
                                    break;
                                case 28:
                                    action = Actions.PersonalAfterRegistration28;
                                    greenButtonText = () => WebstudioNotifyPatternResource.ButtonStartFreeTrial;
                                    greenButtonUrl = "https://www.onlyoffice.com/download-workspace.aspx";
                                    break;
                                default:
                                    continue;
                            }
                        }

                        if (action == null) continue;

                        Log.InfoFormat(@"Send letter personal '{1}'  to {0} culture {2}. tenant id: {3} user culture {4} create on {5} now date {6}",
                              user.Email, action.ID, culture, tenant.TenantId, user.GetCulture(), user.CreateDate, scheduleDate.Date);

                        sendCount++;

                        client.SendNoticeToAsync(
                          action,
                          null,
                          studioNotifyHelper.RecipientFromEmail(user.Email, true),
                          new[] { senderName },
                          TagValues.PersonalHeaderStart(),
                          TagValues.PersonalHeaderEnd(),
                          TagValues.GreenButton(greenButtonText, greenButtonUrl),
                          new TagValue(CommonTags.Footer, coreBaseSettings.CustomMode ? "personalCustomMode" : "personal"));
                    }

                    Log.InfoFormat("Total send count: {0}", sendCount);
                }
                catch (Exception err)
                {
                    Log.Error(err);
                }
            }

            Log.Info("End SendLettersPersonal.");
        }

        public static bool ChangeSubscription(Guid userId, StudioNotifyHelper studioNotifyHelper)
        {
            var recipient = studioNotifyHelper.ToRecipient(userId);

            var isSubscribe = studioNotifyHelper.IsSubscribedToNotify(recipient, Actions.PeriodicNotify);

            studioNotifyHelper.SubscribeToNotify(recipient, Actions.PeriodicNotify, !isSubscribe);

            return !isSubscribe;
        }
    }

    [Scope]
    public class StudioPeriodicNotifyScope
    {
        private TenantManager TenantManager { get; }
        private UserManager UserManager { get; }
        private StudioNotifyHelper StudioNotifyHelper { get; }
        private PaymentManager PaymentManager { get; }
        private TenantExtra TenantExtra { get; }
        private AuthContext AuthContext { get; }
        private CommonLinkUtility CommonLinkUtility { get; }
        private ApiSystemHelper ApiSystemHelper { get; }
        private SetupInfo SetupInfo { get; }
        private DbContextManager<FeedDbContext> DbContextManager { get; }
        private CouponManager CouponManager { get; }
        private IConfiguration Configuration { get; }
        private SettingsManager SettingsManager { get; }
        private CoreBaseSettings CoreBaseSettings { get; }
        private DisplayUserSettingsHelper DisplayUserSettingsHelper { get; }
        private AuthManager AuthManager { get; }
        private SecurityContext SecurityContext { get; }

        public StudioPeriodicNotifyScope(TenantManager tenantManager,
            UserManager userManager,
            StudioNotifyHelper studioNotifyHelper,
            PaymentManager paymentManager,
            TenantExtra tenantExtra,
            AuthContext authContext,
            CommonLinkUtility commonLinkUtility,
            ApiSystemHelper apiSystemHelper,
            SetupInfo setupInfo,
            DbContextManager<FeedDbContext> dbContextManager,
            CouponManager couponManager,
            IConfiguration configuration,
            SettingsManager settingsManager,
            CoreBaseSettings coreBaseSettings,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            AuthManager authManager,
            SecurityContext securityContext)
        {
            TenantManager = tenantManager;
            UserManager = userManager;
            StudioNotifyHelper = studioNotifyHelper;
            PaymentManager = paymentManager;
            TenantExtra = tenantExtra;
            AuthContext = authContext;
            CommonLinkUtility = commonLinkUtility;
            ApiSystemHelper = apiSystemHelper;
            SetupInfo = setupInfo;
            DbContextManager = dbContextManager;
            CouponManager = couponManager;
            Configuration = configuration;
            SettingsManager = settingsManager;
            CoreBaseSettings = coreBaseSettings;
            DisplayUserSettingsHelper = displayUserSettingsHelper;
            AuthManager = authManager;
            SecurityContext = securityContext;
        }

        public void Deconstruct(out TenantManager tenantManager,
            out UserManager userManager,
            out StudioNotifyHelper studioNotifyHelper,
            out PaymentManager paymentManager,
            out TenantExtra tenantExtra,
            out AuthContext authContext,
            out CommonLinkUtility commonLinkUtility,
            out ApiSystemHelper apiSystemHelper,
            out SetupInfo setupInfo,
            out DbContextManager<FeedDbContext> dbContextManager,
            out CouponManager couponManager,
            out IConfiguration configuration,
            out SettingsManager settingsManager,
            out CoreBaseSettings coreBaseSettings,
            out DisplayUserSettingsHelper displayUserSettingsHelper,
            out AuthManager authManager,
            out SecurityContext securityContext)
        {
            tenantManager = TenantManager;
            userManager = UserManager;
            studioNotifyHelper = StudioNotifyHelper;
            paymentManager = PaymentManager;
            tenantExtra = TenantExtra;
            authContext = AuthContext;
            commonLinkUtility = CommonLinkUtility;
            apiSystemHelper = ApiSystemHelper;
            setupInfo = SetupInfo;
            dbContextManager = DbContextManager;
            couponManager = CouponManager;
            configuration = Configuration;
            settingsManager = SettingsManager;
            coreBaseSettings = CoreBaseSettings;
            displayUserSettingsHelper = DisplayUserSettingsHelper;
            authManager = AuthManager;
            securityContext = SecurityContext;
        }
    }

    public static class StudioPeriodicNotifyExtension
    {
        public static void Register(DIHelper services)
        {
            services.TryAdd<StudioPeriodicNotifyScope>();
        }
    }
}