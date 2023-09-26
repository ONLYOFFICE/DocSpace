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

public static class Actions
{
    public static readonly INotifyAction AdminNotify = new NotifyAction("admin_notify", "admin notifications");
    public static readonly INotifyAction PeriodicNotify = new NotifyAction("periodic_notify", "periodic notifications");

    public static readonly INotifyAction SelfProfileUpdated = new NotifyAction("self_profile_updated", "self profile updated");
    public static readonly INotifyAction UserHasJoin = new NotifyAction("user_has_join", "user has join");
    public static readonly INotifyAction UserMessageToAdmin = new NotifyAction("for_admin_notify", "for_admin_notify");
    public static readonly INotifyAction UserMessageToSales = new NotifyAction("for_sales_notify", "for_sales_notify");
    public static readonly INotifyAction RequestTariff = new NotifyAction("request_tariff", "request_tariff");
    public static readonly INotifyAction RequestLicense = new NotifyAction("request_license", "request_license");

    public static readonly INotifyAction YourProfileUpdated = new NotifyAction("profile_updated", "profile updated");
    public static readonly INotifyAction JoinUsers = new NotifyAction("join", "join users");
    public static readonly INotifyAction SendWhatsNew = new NotifyAction("send_whats_new", "send whats new");
    public static readonly INotifyAction BackupCreated = new NotifyAction("backup_created", "backup created");
    public static readonly INotifyAction RestoreStarted = new NotifyAction("restore_started", "restore_started");
    public static readonly INotifyAction RestoreCompletedV115 = new NotifyAction("restore_completed_v115");
    public static readonly INotifyAction PortalDeactivate = new NotifyAction("portal_deactivate", "portal deactivate");
    public static readonly INotifyAction PortalDelete = new NotifyAction("portal_delete", "portal delete");

    public static readonly INotifyAction ProfileDelete = new NotifyAction("profile_delete", "profile_delete");
    public static readonly INotifyAction ProfileHasDeletedItself = new NotifyAction("profile_has_deleted_itself", "profile_has_deleted_itself");
    public static readonly INotifyAction ReassignsCompleted = new NotifyAction("reassigns_completed", "reassigns_completed");
    public static readonly INotifyAction ReassignsFailed = new NotifyAction("reassigns_failed", "reassigns_failed");
    public static readonly INotifyAction RemoveUserDataCompleted = new NotifyAction("remove_user_data_completed", "remove_user_data_completed");
    public static readonly INotifyAction RemoveUserDataCompletedCustomMode = new NotifyAction("remove_user_data_completed_custom_mode");
    public static readonly INotifyAction RemoveUserDataFailed = new NotifyAction("remove_user_data_failed", "remove_user_data_failed");
    public static readonly INotifyAction DnsChange = new NotifyAction("dns_change", "dns_change");

    public static readonly INotifyAction ConfirmOwnerChange = new NotifyAction("owner_confirm_change", "owner_confirm_change");
    public static readonly INotifyAction ActivateEmail = new NotifyAction("activate_email", "activate_email");
    public static readonly INotifyAction EmailChangeV115 = new NotifyAction("change_email_v115", "change_email_v115");
    public static readonly INotifyAction PasswordChangeV115 = new NotifyAction("change_password_v115", "change_password_v115");
    public static readonly INotifyAction PhoneChange = new NotifyAction("change_phone", "change_phone");
    public static readonly INotifyAction TfaChange = new NotifyAction("change_tfa", "change_tfa");
    public static readonly INotifyAction MigrationPortalStart = new NotifyAction("migration_start", "migration start");
    public static readonly INotifyAction MigrationPortalSuccessV115 = new NotifyAction("migration_success_v115");
    public static readonly INotifyAction MigrationPortalError = new NotifyAction("migration_error", "migration error");
    public static readonly INotifyAction MigrationPortalServerFailure = new NotifyAction("migration_server_failure", "migration_server_failure");
    public static readonly INotifyAction PortalRename = new NotifyAction("portal_rename", "portal_rename");

    public static readonly INotifyAction MailboxCreated = new NotifyAction("mailbox_created");
    public static readonly INotifyAction MailboxWithoutSettingsCreated = new NotifyAction("mailbox_without_settings_created");
    public static readonly INotifyAction MailboxPasswordChanged = new NotifyAction("mailbox_password_changed");

    public static readonly INotifyAction SaasGuestActivationV115 = new NotifyAction("saas_guest_activation_v115");
    public static readonly INotifyAction EnterpriseGuestActivationV10 = new NotifyAction("enterprise_guest_activation_v10");
    public static readonly INotifyAction EnterpriseWhitelabelGuestActivationV10 = new NotifyAction("enterprise_whitelabel_guest_activation_v10");
    public static readonly INotifyAction OpensourceGuestActivationV11 = new NotifyAction("opensource_guest_activation_v11");

    public static readonly INotifyAction SaasGuestWelcomeV1 = new NotifyAction("saas_guest_welcome_v1");
    public static readonly INotifyAction EnterpriseGuestWelcomeV1 = new NotifyAction("enterprise_guest_welcome_v1");
    public static readonly INotifyAction EnterpriseWhitelabelGuestWelcomeV1 = new NotifyAction("enterprise_whitelabel_guest_welcome_v1");
    public static readonly INotifyAction OpensourceGuestWelcomeV1 = new NotifyAction("opensource_guest_welcome_v1");

    public static readonly INotifyAction PersonalActivate = new NotifyAction("personal_activate");
    public static readonly INotifyAction PersonalAfterRegistration1 = new NotifyAction("personal_after_registration1");
    public static readonly INotifyAction PersonalConfirmation = new NotifyAction("personal_confirmation");
    public static readonly INotifyAction PersonalEmailChangeV115 = new NotifyAction("personal_change_email_v115");
    public static readonly INotifyAction PersonalPasswordChangeV115 = new NotifyAction("personal_change_password_v115");
    public static readonly INotifyAction PersonalProfileDelete = new NotifyAction("personal_profile_delete");
    public static readonly INotifyAction PersonalAlreadyExist = new NotifyAction("personal_already_exist");

    public static readonly INotifyAction PersonalCustomModeAfterRegistration1 = new NotifyAction("personal_custom_mode_after_registration1");
    public static readonly INotifyAction PersonalCustomModeConfirmation = new NotifyAction("personal_custom_mode_confirmation");
    public static readonly INotifyAction PersonalCustomModeEmailChangeV115 = new NotifyAction("personal_custom_mode_change_email_v115");
    public static readonly INotifyAction PersonalCustomModePasswordChangeV115 = new NotifyAction("personal_custom_mode_change_password_v115");
    public static readonly INotifyAction PersonalCustomModeProfileDelete = new NotifyAction("personal_custom_mode_profile_delete");
    public static readonly INotifyAction PersonalCustomModeAlreadyExist = new NotifyAction("personal_custom_mode_already_exist");

    public static readonly INotifyAction SaasCustomModeRegData = new NotifyAction("saas_custom_mode_reg_data");

    public static readonly INotifyAction StorageEncryptionStart = new NotifyAction("storage_encryption_start");
    public static readonly INotifyAction StorageEncryptionSuccess = new NotifyAction("storage_encryption_success");
    public static readonly INotifyAction StorageEncryptionError = new NotifyAction("storage_encryption_error");
    public static readonly INotifyAction StorageDecryptionStart = new NotifyAction("storage_decryption_start");
    public static readonly INotifyAction StorageDecryptionSuccess = new NotifyAction("storage_decryption_success");
    public static readonly INotifyAction StorageDecryptionError = new NotifyAction("storage_decryption_error");

    public static readonly INotifyAction SaasRoomInvite = new NotifyAction("saas_room_invite");
    public static readonly INotifyAction SaasDocSpaceInvite = new NotifyAction("saas_docspace_invite");

    public static readonly INotifyAction SaasAdminActivationV1 = new NotifyAction("saas_admin_activation_v1");
    public static readonly INotifyAction EnterpriseAdminActivationV1 = new NotifyAction("enterprise_admin_activation_v1");
    public static readonly INotifyAction EnterpriseWhitelabelAdminActivationV1 = new NotifyAction("enterprise_whitelabel_admin_activation_v1");
    public static readonly INotifyAction OpensourceAdminActivationV1 = new NotifyAction("opensource_admin_activation_v1");

    public static readonly INotifyAction SaasAdminWelcomeV1 = new NotifyAction("saas_admin_welcome_v1");
    public static readonly INotifyAction EnterpriseAdminWelcomeV1 = new NotifyAction("enterprise_admin_welcome_v1");
    public static readonly INotifyAction EnterpriseWhitelabelAdminWelcomeV1 = new NotifyAction("enterprise_whitelabel_admin_welcome_v1");
    public static readonly INotifyAction OpensourceAdminWelcomeV1 = new NotifyAction("opensource_admin_welcome_v1");

    public static readonly INotifyAction SaasAdminUserDocsTipsV1 = new NotifyAction("saas_admin_user_docs_tips_v1");
    public static readonly INotifyAction OpensourceAdminDocsTipsV1 = new NotifyAction("opensource_admin_docs_tips_v1");
    public static readonly INotifyAction OpensourceUserDocsTipsV1 = new NotifyAction("opensource_user_docs_tips_v1");
    public static readonly INotifyAction EnterpriseAdminUserDocsTipsV1 = new NotifyAction("enterprise_admin_user_docs_tips_v1");

    public static readonly INotifyAction SaasAdminTrialWarningAfterHalfYearV1 = new NotifyAction("saas_admin_trial_warning_after_half_year_v1");

    public static readonly INotifyAction PortalDeleteSuccessV1 = new NotifyAction("portal_delete_success_v1");

    public static readonly INotifyAction SaasUserWelcomeV1 = new NotifyAction("saas_user_welcome_v1");
    public static readonly INotifyAction EnterpriseUserWelcomeV1 = new NotifyAction("enterprise_user_welcome_v1");
    public static readonly INotifyAction EnterpriseWhitelabelUserWelcomeV1 = new NotifyAction("enterprise_whitelabel_user_welcome_v1");
    public static readonly INotifyAction EnterpriseWhitelabelUserWelcomeCustomModeV1 = new NotifyAction("enterprise_whitelabel_user_welcome_custom_mode_v1");
    public static readonly INotifyAction OpensourceUserWelcomeV1 = new NotifyAction("opensource_user_welcome_v1");

    public static readonly INotifyAction SaasUserActivationV1 = new NotifyAction("saas_user_activation_v1");
    public static readonly INotifyAction EnterpriseUserActivationV1 = new NotifyAction("enterprise_user_activation_v1");
    public static readonly INotifyAction EnterpriseWhitelabelUserActivationV1 = new NotifyAction("enterprise_whitelabel_user_activation_v1");
    public static readonly INotifyAction OpensourceUserActivationV1 = new NotifyAction("opensource_user_activation_v1");

    public static readonly INotifyAction PersonalAfterRegistration14V1 = new NotifyAction("personal_after_registration14_v1");

    public static readonly INotifyAction SaasAdminModulesV1 = new NotifyAction("saas_admin_modules_v1");

    public static readonly INotifyAction SaasAdminUserAppsTipsV1 = new NotifyAction("saas_admin_user_apps_tips_v1");
    public static readonly INotifyAction EnterpriseAdminUserAppsTipsV1 = new NotifyAction("enterprise_admin_user_apps_tips_v1");

    public static readonly INotifyAction RoomsActivity = new NotifyAction("rooms_activity", "rooms activity");

    public static readonly INotifyAction SaasOwnerPaymentWarningGracePeriodBeforeActivation = new NotifyAction("saas_owner_payment_warning_grace_period_before_activation");
    public static readonly INotifyAction SaasOwnerPaymentWarningGracePeriodActivation = new NotifyAction("saas_owner_payment_warning_grace_period_activation");
    public static readonly INotifyAction SaasOwnerPaymentWarningGracePeriodLastDay = new NotifyAction("saas_owner_payment_warning_grace_period_last_day");
    public static readonly INotifyAction SaasOwnerPaymentWarningGracePeriodExpired = new NotifyAction("saas_owner_payment_warning_grace_period_expired");

    public static readonly INotifyAction SaasAdminVideoGuides = new NotifyAction("saas_video_guides_v1");
} 
