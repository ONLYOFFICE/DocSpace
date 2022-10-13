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

namespace ASC.Web.Studio.Core.Notify;

[Scope]
public class StudioPeriodicNotify
{
    private readonly NotifyEngineQueue _notifyEngineQueue;
    private readonly WorkContext _workContext;
    private readonly TenantManager _tenantManager;
    private readonly UserManager _userManager;
    private readonly StudioNotifyHelper _studioNotifyHelper;
    private readonly PaymentManager _paymentManager;
    private readonly TenantExtra _tenantExtra;
    private readonly AuthContext _authContext;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly ApiSystemHelper _apiSystemHelper;
    private readonly SetupInfo _setupInfo;
    private readonly IDbContextFactory<FeedDbContext> _dbContextFactory;
    private readonly IConfiguration _configuration;
    private readonly SettingsManager _settingsManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly DisplayUserSettingsHelper _displayUserSettingsHelper;
    private readonly AuthManager _authManager;
    private readonly SecurityContext _securityContext;
    private readonly ILogger _log;

    public StudioPeriodicNotify(
        ILoggerProvider log,
        NotifyEngineQueue notifyEngineQueue,
        WorkContext workContext,
        TenantManager tenantManager,
        UserManager userManager,
        StudioNotifyHelper studioNotifyHelper,
        PaymentManager paymentManager,
        TenantExtra tenantExtra,
        AuthContext authContext,
        CommonLinkUtility commonLinkUtility,
        ApiSystemHelper apiSystemHelper,
        SetupInfo setupInfo,
        IDbContextFactory<FeedDbContext> dbContextFactory,
        IConfiguration configuration,
        SettingsManager settingsManager,
        CoreBaseSettings coreBaseSettings,
        DisplayUserSettingsHelper displayUserSettingsHelper,
        AuthManager authManager,
        SecurityContext securityContext)
    {
        _notifyEngineQueue = notifyEngineQueue;
        _workContext = workContext;
        _tenantManager = tenantManager;
        _userManager = userManager;
        _studioNotifyHelper = studioNotifyHelper;
        _paymentManager = paymentManager;
        _tenantExtra = tenantExtra;
        _authContext = authContext;
        _commonLinkUtility = commonLinkUtility;
        _apiSystemHelper = apiSystemHelper;
        _setupInfo = setupInfo;
        _dbContextFactory = dbContextFactory;
        _configuration = configuration;
        _settingsManager = settingsManager;
        _coreBaseSettings = coreBaseSettings;
        _displayUserSettingsHelper = displayUserSettingsHelper;
        _authManager = authManager;
        _securityContext = securityContext;
        _log = log.CreateLogger("ASC.Notify");
    }

    public Task SendSaasLettersAsync(string senderName, DateTime scheduleDate)
    {
        _log.InformationStartSendSaasTariffLetters();

        var activeTenants = _tenantManager.GetTenants().ToList();

        if (activeTenants.Count <= 0)
        {
            _log.InformationEndSendSaasTariffLetters();
            return Task.CompletedTask;
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
                _tenantManager.SetCurrentTenant(tenant.Id);
                var client = _workContext.NotifyContext.RegisterClient(_notifyEngineQueue, _studioNotifyHelper.NotifySource);

                var tariff = _paymentManager.GetTariff(tenant.Id);
                var quota = _tenantManager.GetTenantQuota(tenant.Id);
                var createdDate = tenant.CreationDateTime.Date;

                var dueDateIsNotMax = tariff.DueDate != DateTime.MaxValue;
                var dueDate = tariff.DueDate.Date;

                var delayDueDateIsNotMax = tariff.DelayDueDate != DateTime.MaxValue;
                var delayDueDate = tariff.DelayDueDate.Date;

                INotifyAction action = null;
                var paymentMessage = true;

                var toadmins = false;
                var tousers = false;
                var toowner = false;

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
                        greenButtonUrl = _commonLinkUtility.GetFullAbsolutePath("~/Tariffs.aspx");
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
                        greenButtonUrl = $"{_commonLinkUtility.GetFullAbsolutePath("~").TrimEnd('/')}/";
                    }

                    #endregion

                    #region  4 days after registration to admins SAAS TRIAL

                    else if (createdDate.AddDays(4) == nowDate)
                    {
                        action = Actions.SaasAdminComfortTipsV115;
                        paymentMessage = false;
                        toadmins = true;

                        greenButtonText = () => WebstudioNotifyPatternResource.ButtonUseDiscount;
                        greenButtonUrl = _commonLinkUtility.GetFullAbsolutePath("~/Tariffs.aspx");
                    }

                    #endregion

                    #region 7 days after registration to admins and users SAAS TRIAL

                    else if (createdDate.AddDays(7) == nowDate)
                    {
                        action = Actions.SaasAdminUserDocsTipsV1;
                        paymentMessage = false;
                        toadmins = true;
                        tousers = true;

                        greenButtonText = () => WebstudioNotifyPatternResource.CollaborateDocSpace;
                        greenButtonUrl = $"{_commonLinkUtility.GetFullAbsolutePath("~")}/rooms/personal/";
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

                    else if (!_coreBaseSettings.CustomMode && dueDateIsNotMax && dueDate.AddDays(-5) == nowDate)
                    {
                        toadmins = true;
                        action = Actions.SaasAdminTrialWarningBefore5V115;

                        greenButtonText = () => WebstudioNotifyPatternResource.ButtonUseDiscount;
                        greenButtonUrl = _commonLinkUtility.GetFullAbsolutePath("~/tariffs.aspx");
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
                        greenButtonUrl = _commonLinkUtility.GetFullAbsolutePath("~/Tariffs.aspx");
                    }

                    #region 6 months after SAAS TRIAL expired

                    else if (dueDateIsNotMax && dueDate.AddMonths(6) == nowDate)
                    {
                        action = Actions.SaasAdminTrialWarningAfterHalfYearV1;
                        toowner = true;

                        greenButtonText = () => WebstudioNotifyPatternResource.LeaveFeedbackDocSpace;

                        var owner = _userManager.GetUsers(tenant.OwnerId);
                        greenButtonUrl = _setupInfo.TeamlabSiteRedirect + "/remove-portal-feedback-form.aspx#" +
                                  HttpUtility.UrlEncode(Convert.ToBase64String(
                                      Encoding.UTF8.GetBytes("{\"firstname\":\"" + owner.FirstName +
                                                                         "\",\"lastname\":\"" + owner.LastName +
                                                                         "\",\"alias\":\"" + tenant.Alias +
                                                                         "\",\"email\":\"" + owner.Email + "\"}")));
                    }
                    else if (dueDateIsNotMax && dueDate.AddMonths(6).AddDays(7) <= nowDate)
                    {
                        _tenantManager.RemoveTenant(tenant.Id, true);

                        if (!string.IsNullOrEmpty(_apiSystemHelper.ApiCacheUrl))
                        {
                            await _apiSystemHelper.RemoveTenantFromCacheAsync(tenant.Alias, _authContext.CurrentAccount.ID);
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
                        action = Actions.SaasAdminTrialWarningAfterHalfYearV1;
                        toowner = true;

                        greenButtonText = () => WebstudioNotifyPatternResource.LeaveFeedbackDocSpace;

                        var owner = _userManager.GetUsers(tenant.OwnerId);
                        greenButtonUrl = _setupInfo.TeamlabSiteRedirect + "/remove-portal-feedback-form.aspx#" +
                                  HttpUtility.UrlEncode(Convert.ToBase64String(
                                      Encoding.UTF8.GetBytes("{\"firstname\":\"" + owner.FirstName +
                                                                         "\",\"lastname\":\"" + owner.LastName +
                                                                         "\",\"alias\":\"" + tenant.Alias +
                                                                         "\",\"email\":\"" + owner.Email + "\"}")));
                    }
                    else if (tariff.State == TariffState.NotPaid && dueDateIsNotMax && dueDate.AddMonths(6).AddDays(7) <= nowDate)
                    {
                        _tenantManager.RemoveTenant(tenant.Id, true);

                        if (!string.IsNullOrEmpty(_apiSystemHelper.ApiCacheUrl))
                        {
                            await _apiSystemHelper.RemoveTenantFromCacheAsync(tenant.Alias, _authContext.CurrentAccount.ID);
                        }
                    }

                    #endregion

                    #endregion
                }


                if (action == null)
                {
                    continue;
                }

                var users = toowner
                                    ? new List<UserInfo> { _userManager.GetUsers(tenant.OwnerId) }
                                    : _studioNotifyHelper.GetRecipients(toadmins, tousers, false);

                foreach (var u in users.Where(u => paymentMessage || _studioNotifyHelper.IsSubscribedToNotify(u, Actions.PeriodicNotify)))
                {
                    var culture = string.IsNullOrEmpty(u.CultureName) ? tenant.GetCulture() : u.GetCulture();
                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;
                    var rquota = _tenantExtra.GetRightQuota() ?? TenantQuota.Default;

                    client.SendNoticeToAsync(
                        action,
                            new[] { _studioNotifyHelper.ToRecipient(u.Id) },
                        new[] { senderName },
                        new TagValue(Tags.UserName, u.FirstName.HtmlEncode()),
                            new TagValue(Tags.PricingPage, _commonLinkUtility.GetFullAbsolutePath("~/tariffs.aspx")),
                            new TagValue(Tags.ActiveUsers, _userManager.GetUsers().Length),
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
                        new TagValue(CommonTags.Footer, _userManager.IsAdmin(u) ? "common" : "social"));
                }
            }
            catch (Exception err)
            {
                _log.ErrorSendSaasLettersAsync(err);
            }
        }

        _log.InformationEndSendSaasTariffLetters();
    }

    public void SendEnterpriseLetters(string senderName, DateTime scheduleDate)
    {
        var nowDate = scheduleDate.Date;

        _log.InformationStartSendTariffEnterpriseLetters();

        var activeTenants = _tenantManager.GetTenants().ToList();

        if (activeTenants.Count <= 0)
        {
            _log.InformationEndSendTariffEnterpriseLetters();
            return;
        }

        foreach (var tenant in activeTenants)
        {
            try
            {
                var defaultRebranding = MailWhiteLabelSettings.IsDefault(_settingsManager);
                _tenantManager.SetCurrentTenant(tenant.Id);
                var client = _workContext.NotifyContext.RegisterClient(_notifyEngineQueue, _studioNotifyHelper.NotifySource);

                var tariff = _paymentManager.GetTariff(tenant.Id);
                var quota = _tenantManager.GetTenantQuota(tenant.Id);
                var createdDate = tenant.CreationDateTime.Date;

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

                        tableItemImg1 = _studioNotifyHelper.GetNotificationImageUrl("tips-customize-brand-100.png");
                        tableItemText1 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_brand_hdr;
                        tableItemComment1 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_brand;

                        tableItemImg2 = _studioNotifyHelper.GetNotificationImageUrl("tips-customize-regional-100.png");
                        tableItemText2 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_regional_hdr;
                        tableItemComment2 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_regional;

                        tableItemImg3 = _studioNotifyHelper.GetNotificationImageUrl("tips-customize-customize-100.png");
                        tableItemText3 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_customize_hdr;
                        tableItemComment3 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_customize;

                        tableItemImg4 = _studioNotifyHelper.GetNotificationImageUrl("tips-customize-modules-100.png");
                        tableItemText4 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_modules_hdr;
                        tableItemComment4 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_modules;

                        tableItemImg5 = _studioNotifyHelper.GetNotificationImageUrl("tips-customize-3rdparty-100.png");
                        tableItemText5 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_3rdparty_hdr;
                        tableItemComment5 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_3rdparty;

                        greenButtonText = () => WebstudioNotifyPatternResource.ButtonConfigureRightNow;
                        greenButtonUrl = _commonLinkUtility.GetFullAbsolutePath(_commonLinkUtility.GetAdministration(ManagementType.General));
                    }

                    #endregion

                    #region 4 days after registration to admins ENTERPRISE TRIAL + only 1 user + defaultRebranding

                    else if (createdDate.AddDays(4) == nowDate && _userManager.GetUsers().Length == 1)
                    {
                        action = Actions.EnterpriseAdminInviteTeammatesV10;
                        paymentMessage = false;
                        toadmins = true;

                        greenButtonText = () => WebstudioNotifyPatternResource.ButtonInviteRightNow;
                        greenButtonUrl = $"{_commonLinkUtility.GetFullAbsolutePath("~").TrimEnd('/')}/products/people/";
                    }

                    #endregion

                    #region 5 days after registration to admins ENTERPRISE TRAIL + without activity in 1 or more days + defaultRebranding

                    else if (createdDate.AddDays(5) == nowDate)
                    {
                        List<DateTime> datesWithActivity;

                        datesWithActivity =
                                _dbContextFactory.CreateDbContext().FeedAggregates
                                .Where(r => r.Tenant == _tenantManager.GetCurrentTenant().Id)
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
                        action = Actions.EnterpriseAdminUserDocsTipsV1;
                        paymentMessage = false;
                        toadmins = true;
                        tousers = true;
                        greenButtonText = () => WebstudioNotifyPatternResource.ButtonAccessYouWebOffice;
                        greenButtonUrl = $"{_commonLinkUtility.GetFullAbsolutePath("~")}/products/files/";
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

                        tableItemImg1 = _studioNotifyHelper.GetNotificationImageUrl("tips-customize-brand-100.png");
                        tableItemText1 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_brand_hdr;
                        tableItemComment1 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_brand;

                        tableItemImg2 = _studioNotifyHelper.GetNotificationImageUrl("tips-customize-regional-100.png");
                        tableItemText2 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_regional_hdr;
                        tableItemComment2 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_regional;

                        tableItemImg3 = _studioNotifyHelper.GetNotificationImageUrl("tips-customize-customize-100.png");
                        tableItemText3 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_customize_hdr;
                        tableItemComment3 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_customize;

                        tableItemImg4 = _studioNotifyHelper.GetNotificationImageUrl("tips-customize-modules-100.png");
                        tableItemText4 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_modules_hdr;
                        tableItemComment4 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_modules;

                        if (!_coreBaseSettings.CustomMode)
                        {
                            tableItemImg5 = _studioNotifyHelper.GetNotificationImageUrl("tips-customize-3rdparty-100.png");
                            tableItemText5 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_3rdparty_hdr;
                            tableItemComment5 = () => WebstudioNotifyPatternResource.pattern_enterprise_admin_customize_portal_v10_item_3rdparty;
                        }

                        greenButtonText = () => WebstudioNotifyPatternResource.ButtonConfigureRightNow;
                        greenButtonUrl = _commonLinkUtility.GetFullAbsolutePath(_commonLinkUtility.GetAdministration(ManagementType.General));
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
                        greenButtonUrl = _commonLinkUtility.GetFullAbsolutePath("~/tariffs.aspx");
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
                        greenButtonUrl = _commonLinkUtility.GetFullAbsolutePath("~/tariffs.aspx");
                    }

                    #endregion

                    #endregion
                }


                if (action == null)
                {
                    continue;
                }

                var users = _studioNotifyHelper.GetRecipients(toadmins, tousers, false);

                foreach (var u in users.Where(u => paymentMessage || _studioNotifyHelper.IsSubscribedToNotify(u, Actions.PeriodicNotify)))
                {
                    var culture = string.IsNullOrEmpty(u.CultureName) ? tenant.GetCulture() : u.GetCulture();
                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;

                    var rquota = _tenantExtra.GetRightQuota() ?? TenantQuota.Default;

                    client.SendNoticeToAsync(
                        action,
                            new[] { _studioNotifyHelper.ToRecipient(u.Id) },
                        new[] { senderName },
                        new TagValue(Tags.UserName, u.FirstName.HtmlEncode()),
                            new TagValue(Tags.PricingPage, _commonLinkUtility.GetFullAbsolutePath("~/tariffs.aspx")),
                            new TagValue(Tags.ActiveUsers, _userManager.GetUsers().Length),
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
                _log.ErrorSendEnterpriseLetters(err);
            }
        }

        _log.InformationEndSendTariffEnterpriseLetters();
    }

    public void SendOpensourceLetters(string senderName, DateTime scheduleDate)
    {
        var nowDate = scheduleDate.Date;

        _log.InformationStartSendOpensourceTariffLetters();

        var activeTenants = _tenantManager.GetTenants().ToList();

        if (activeTenants.Count <= 0)
        {
            _log.InformationEndSendOpensourceTariffLetters();
            return;
        }

        foreach (var tenant in activeTenants)
        {
            try
            {
                _tenantManager.SetCurrentTenant(tenant.Id);
                var client = _workContext.NotifyContext.RegisterClient(_notifyEngineQueue, _studioNotifyHelper.NotifySource);

                var createdDate = tenant.CreationDateTime.Date;


                #region After registration letters

                #region 7 days after registration to admins

                if (createdDate.AddDays(7) == nowDate)
                {
                    var users = _studioNotifyHelper.GetRecipients(true, true, false);


                    foreach (var u in users.Where(u => _studioNotifyHelper.IsSubscribedToNotify(u, Actions.PeriodicNotify)))
                    {
                        var culture = string.IsNullOrEmpty(u.CultureName) ? tenant.GetCulture() : u.GetCulture();
                        Thread.CurrentThread.CurrentCulture = culture;
                        Thread.CurrentThread.CurrentUICulture = culture;

                        client.SendNoticeToAsync(
                                _userManager.IsAdmin(u) ? Actions.OpensourceAdminDocsTipsV1 : Actions.OpensourceUserDocsTipsV1,
                                new[] { _studioNotifyHelper.ToRecipient(u.Id) },
                            new[] { senderName },
                                new TagValue(Tags.UserName, u.DisplayUserName(_displayUserSettingsHelper)),
                            new TagValue(CommonTags.Footer, "opensource"));
                    }
                }
                #endregion

                #endregion
            }
            catch (Exception err)
            {
                _log.ErrorSendOpensourceLetters(err);
            }
        }

        _log.InformationEndSendOpensourceTariffLetters();
    }

    public void SendPersonalLetters(string senderName, DateTime scheduleDate)
    {
        _log.InformationStartSendLettersPersonal();

        var activeTenants = _tenantManager.GetTenants().ToList();

        foreach (var tenant in activeTenants)
        {
            try
            {
                Func<string> greenButtonText = () => string.Empty;
                var greenButtonUrl = string.Empty;

                var sendCount = 0;

                _tenantManager.SetCurrentTenant(tenant.Id);
                var client = _workContext.NotifyContext.RegisterClient(_notifyEngineQueue, _studioNotifyHelper.NotifySource);

                _log.InformationCurrentTenant(tenant.Id);

                var users = _userManager.GetUsers(EmployeeStatus.Active);

                foreach (var user in users.Where(u => _studioNotifyHelper.IsSubscribedToNotify(u, Actions.PeriodicNotify)))
                {
                    INotifyAction action;

                    _securityContext.AuthenticateMeWithoutCookie(_authManager.GetAccountByID(tenant.Id, user.Id));

                    var culture = tenant.GetCulture();
                    if (!string.IsNullOrEmpty(user.CultureName))
                    {
                        try
                        {
                            culture = user.GetCulture();
                        }
                        catch (CultureNotFoundException exception)
                        {

                            _log.ErrorSendPersonalLetters(exception);
                        }
                    }

                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;

                    var dayAfterRegister = (int)scheduleDate.Date.Subtract(user.CreateDate.Date).TotalDays;

                    switch (dayAfterRegister)
                    {
                        case 7:
                            action = Actions.PersonalAfterRegistration7;
                            break;
                        case 14:
                            action = Actions.PersonalAfterRegistration14V1;
                            break;
                        case 21:
                            action = Actions.PersonalAfterRegistration21;
                            break;
                        default:
                            continue;
                    }

                    if (action == null)
                    {
                        continue;
                    }

                    _log.InformationSendLetterPersonal(action.ID, user.Email, culture, tenant.Id, user.GetCulture(), user.CreateDate, scheduleDate.Date);

                    sendCount++;

                    client.SendNoticeToAsync(
                      action,
                      null,
                          _studioNotifyHelper.RecipientFromEmail(user.Email, true),
                      new[] { senderName },
                      TagValues.PersonalHeaderStart(),
                      TagValues.PersonalHeaderEnd(),
                      TagValues.GreenButton(greenButtonText, greenButtonUrl),
                          new TagValue(CommonTags.Footer, _coreBaseSettings.CustomMode ? "personalCustomMode" : "personal"));
                }

                _log.InformationTotalSendCount(sendCount);
            }
            catch (Exception err)
            {
                _log.ErrorSendPersonalLetters(err);
            }
        }

        _log.InformationEndSendLettersPersonal();
    }

    public static bool ChangeSubscription(Guid userId, StudioNotifyHelper studioNotifyHelper)
    {
        var recipient = studioNotifyHelper.ToRecipient(userId);

        var isSubscribe = studioNotifyHelper.IsSubscribedToNotify(recipient, Actions.PeriodicNotify);

        studioNotifyHelper.SubscribeToNotify(recipient, Actions.PeriodicNotify, !isSubscribe);

        return !isSubscribe;
    }
}
