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
public class StudioNotifySource : NotifySource
{
    public StudioNotifySource(UserManager userManager, IRecipientProvider recipientsProvider, SubscriptionManager subscriptionManager)
        : base("asc.web.studio", userManager, recipientsProvider, subscriptionManager)
    {
    }


    protected override IActionProvider CreateActionProvider()
    {
        return new ConstActionProvider(
                Actions.SelfProfileUpdated,
                Actions.JoinUsers,
                Actions.SendWhatsNew,
                Actions.UserHasJoin,
                Actions.BackupCreated,
                Actions.RestoreStarted,
                Actions.RestoreCompletedV115,
                Actions.PortalDeactivate,
                Actions.PortalDelete,
                Actions.PortalDeleteSuccessV1,
                Actions.DnsChange,
                Actions.ConfirmOwnerChange,
                Actions.EmailChangeV115,
                Actions.PasswordChangeV115,
                Actions.ActivateEmail,
                Actions.ProfileDelete,
                Actions.ProfileHasDeletedItself,
                Actions.ReassignsCompleted,
                Actions.ReassignsFailed,
                Actions.RemoveUserDataCompleted,
                Actions.RemoveUserDataCompletedCustomMode,
                Actions.RemoveUserDataFailed,
                Actions.PhoneChange,
                Actions.MigrationPortalStart,
                Actions.MigrationPortalSuccessV115,
                Actions.MigrationPortalError,
                Actions.MigrationPortalServerFailure,

                Actions.UserMessageToAdmin,

                Actions.SaasGuestActivationV115,
                Actions.EnterpriseGuestActivationV10,
                Actions.EnterpriseWhitelabelGuestActivationV10,
                Actions.OpensourceGuestActivationV11,

                Actions.SaasGuestWelcomeV115,
                Actions.EnterpriseGuestWelcomeV10,
                Actions.EnterpriseWhitelabelGuestWelcomeV10,
                Actions.OpensourceGuestWelcomeV11,

                Actions.EnterpriseAdminInviteTeammatesV10,
                Actions.EnterpriseAdminWithoutActivityV10,
                Actions.EnterpriseAdminUserAppsTipsV10,

                Actions.EnterpriseAdminTrialWarningBefore7V10,
                Actions.EnterpriseAdminTrialWarningV10,

                Actions.EnterpriseAdminPaymentWarningBefore7V10,
                Actions.EnterpriseWhitelabelAdminPaymentWarningBefore7V10,
                Actions.EnterpriseAdminPaymentWarningV10,
                Actions.EnterpriseWhitelabelAdminPaymentWarningV10,

                Actions.SaasAdminModulesV1,

                Actions.PersonalActivate,
                Actions.PersonalAfterRegistration1,
                Actions.PersonalAfterRegistration7,
                Actions.PersonalAfterRegistration14,
                Actions.PersonalAfterRegistration21,
                Actions.PersonalAfterRegistration14V1,
                Actions.PersonalConfirmation,
                Actions.PersonalPasswordChangeV115,
                Actions.PersonalEmailChangeV115,
                Actions.PersonalProfileDelete,
                Actions.PersonalAlreadyExist,

                Actions.MailboxCreated,
                Actions.MailboxWithoutSettingsCreated,

                Actions.MailboxCreated,
                Actions.MailboxWithoutSettingsCreated,

                Actions.PersonalCustomModeAfterRegistration1,
                Actions.PersonalCustomModeConfirmation,
                Actions.PersonalCustomModePasswordChangeV115,
                Actions.PersonalCustomModeEmailChangeV115,
                Actions.PersonalCustomModeProfileDelete,
                Actions.PersonalCustomModeAlreadyExist,

                Actions.SaasCustomModeRegData,

                Actions.StorageEncryptionStart,
                Actions.StorageEncryptionSuccess,
                Actions.StorageEncryptionError,
                Actions.StorageDecryptionStart,
                Actions.StorageDecryptionSuccess,
                Actions.StorageDecryptionError,

                Actions.SaasAdminActivationV1,
                Actions.EnterpriseAdminActivationV1,
                Actions.EnterpriseWhitelabelAdminActivationV1,
                Actions.OpensourceAdminActivationV1,


                Actions.SaasAdminWelcomeV1,
                Actions.EnterpriseAdminWelcomeV1,
                Actions.EnterpriseWhitelabelAdminWelcomeV1,
                Actions.OpensourceAdminWelcomeV1,

                Actions.SaasAdminUserDocsTipsV1,
                Actions.OpensourceAdminDocsTipsV1,
                Actions.OpensourceUserDocsTipsV1,
                Actions.EnterpriseAdminUserDocsTipsV1,

                Actions.SaasAdminTrialWarningAfterHalfYearV1,

                Actions.SaasUserWelcomeV1,
                Actions.EnterpriseUserWelcomeV1,
                Actions.EnterpriseWhitelabelUserWelcomeV1,
                Actions.EnterpriseWhitelabelUserWelcomeCustomMode,
                Actions.OpensourceUserWelcomeV1,

                Actions.SaasUserActivationV1,
                Actions.EnterpriseUserActivationV1,
                Actions.EnterpriseWhitelabelUserActivationV1,
                Actions.OpensourceUserActivationV1
            );
    }

    protected override IPatternProvider CreatePatternsProvider()
    {
        return new XmlPatternProvider2(WebPatternResource.webstudio_patterns);
    }

    protected override ISubscriptionProvider CreateSubscriptionProvider()
    {
        return new AdminNotifySubscriptionProvider(base.CreateSubscriptionProvider());
    }


    private class AdminNotifySubscriptionProvider : ISubscriptionProvider
    {
        private readonly ISubscriptionProvider _provider;


        public AdminNotifySubscriptionProvider(ISubscriptionProvider provider)
        {
            this._provider = provider;
        }

        public object GetSubscriptionRecord(INotifyAction action, IRecipient recipient, string objectID)
        {
            return _provider.GetSubscriptionRecord(GetAdminAction(action), recipient, objectID);
        }

        public string[] GetSubscriptions(INotifyAction action, IRecipient recipient, bool checkSubscription = true)
        {
            return _provider.GetSubscriptions(GetAdminAction(action), recipient, checkSubscription);
        }

        public void Subscribe(INotifyAction action, string objectID, IRecipient recipient)
        {
            _provider.Subscribe(GetAdminAction(action), objectID, recipient);
        }

        public void UnSubscribe(INotifyAction action, IRecipient recipient)
        {
            _provider.UnSubscribe(GetAdminAction(action), recipient);
        }

        public void UnSubscribe(INotifyAction action)
        {
            _provider.UnSubscribe(GetAdminAction(action));
        }

        public void UnSubscribe(INotifyAction action, string objectID)
        {
            _provider.UnSubscribe(GetAdminAction(action), objectID);
        }

        public void UnSubscribe(INotifyAction action, string objectID, IRecipient recipient)
        {
            _provider.UnSubscribe(GetAdminAction(action), objectID, recipient);
        }

        public void UpdateSubscriptionMethod(INotifyAction action, IRecipient recipient, params string[] senderNames)
        {
            _provider.UpdateSubscriptionMethod(GetAdminAction(action), recipient, senderNames);
        }

        public IRecipient[] GetRecipients(INotifyAction action, string objectID)
        {
            return _provider.GetRecipients(GetAdminAction(action), objectID);
        }

        public string[] GetSubscriptionMethod(INotifyAction action, IRecipient recipient)
        {
            return _provider.GetSubscriptionMethod(GetAdminAction(action), recipient);
        }

        public bool IsUnsubscribe(IDirectRecipient recipient, INotifyAction action, string objectID)
        {
            return _provider.IsUnsubscribe(recipient, action, objectID);
        }

        private INotifyAction GetAdminAction(INotifyAction action)
        {
            if (Actions.SelfProfileUpdated.ID == action.ID ||
                Actions.UserHasJoin.ID == action.ID ||
                Actions.UserMessageToAdmin.ID == action.ID
                )
            {
                return Actions.AdminNotify;
            }
            else
            {
                return action;
            }
        }
    }
}
