using ASC.Core.Common.Resources;
namespace ASC.Studio.Notify
{
    public class CustomModeResource
    {
        private static JsonResourceManager JsonResourceManager { get; set; }
        static CustomModeResource()
        {
            JsonResourceManager = new JsonResourceManager("CustomModeResource");
        }

        public static string AllRightsReservedCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("AllRightsReservedCustomMode");
            }
        }
        public static string AuthDocsMetaDescriptionCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("AuthDocsMetaDescriptionCustomMode");
            }
        }
        public static string AuthDocsMetaKeywordsCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("AuthDocsMetaKeywordsCustomMode");
            }
        }
        public static string AuthDocsOrCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("AuthDocsOrCustomMode");
            }
        }
        public static string BrowserAndDesktopTextCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("BrowserAndDesktopTextCustomMode");
            }
        }
        public static string CalendarEventEditor_attendeesLabelHelpInfoCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("calendarEventEditor_attendeesLabelHelpInfoCustomMode");
            }
        }
        public static string CheckEmailCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("CheckEmailCustomMode");
            }
        }
        public static string CollaborativeEditingTextCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("CollaborativeEditingTextCustomMode");
            }
        }
        public static string CollaborativeWorkCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("CollaborativeWorkCustomMode");
            }
        }
        public static string CompatibleTextCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("CompatibleTextCustomMode");
            }
        }
        public static string ConnectionInstructionsCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("ConnectionInstructionsCustomMode");
            }
        }
        public static string CreateAccountCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("CreateAccountCustomMode");
            }
        }
        public static string CreateOfficeCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("CreateOfficeCustomMode");
            }
        }
        public static string DiscussionDocumentCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("DiscussionDocumentCustomMode");
            }
        }
        public static string DiscussionDocumentDescriptionCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("DiscussionDocumentDescriptionCustomMode");
            }
        }
        public static string DocumentEditorCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("DocumentEditorCustomMode");
            }
        }
        public static string DocumentEditorDescriptionCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("DocumentEditorDescriptionCustomMode");
            }
        }
        public static string EmailCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("EmailCustomMode");
            }
        }
        public static string EmailHintCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("EmailHintCustomMode");
            }
        }
        public static string EnterViaSocialCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("EnterViaSocialCustomMode");
            }
        }
        public static string ForgotPswdCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("ForgotPswdCustomMode");
            }
        }
        public static string HelpAnswerPortalRenameCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("HelpAnswerPortalRenameCustomMode");
            }
        }
        public static string LicenseActivateDescrCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("LicenseActivateDescrCustomMode");
            }
        }
        public static string LicenseModulesListCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("LicenseModulesListCustomMode");
            }
        }
        public static string LoginAccountCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("LoginAccountCustomMode");
            }
        }
        public static string LoginAccountDesktopCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("LoginAccountDesktopCustomMode");
            }
        }
        public static string LoginCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("LoginCustomMode");
            }
        }
        public static string NearAtHandCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("NearAtHandCustomMode");
            }
        }
        public static string NotRegisteredCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("NotRegisteredCustomMode");
            }
        }
        public static string OfficeDesktopCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("OfficeDesktopCustomMode");
            }
        }
        public static string OfficeDesktopDescriptionCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("OfficeDesktopDescriptionCustomMode");
            }
        }
        public static string OfficeOnlineCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("OfficeOnlineCustomMode");
            }
        }
        public static string OfficeOnlineDescriptionCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("OfficeOnlineDescriptionCustomMode");
            }
        }
        public static string OnlineVersionCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("OnlineVersionCustomMode");
            }
        }
        public static string PasswordCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("PasswordCustomMode");
            }
        }
        public static string Pattern_enterprise_whitelabel_user_welcome_custom_mode
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_whitelabel_user_welcome_custom_mode");
            }
        }
        public static string Pattern_personal_custom_mode_after_registration1
        {
            get
            {
                return JsonResourceManager.GetString("pattern_personal_custom_mode_after_registration1");
            }
        }
        public static string Pattern_personal_custom_mode_after_registration7
        {
            get
            {
                return JsonResourceManager.GetString("pattern_personal_custom_mode_after_registration7");
            }
        }
        public static string Pattern_personal_custom_mode_change_email
        {
            get
            {
                return JsonResourceManager.GetString("pattern_personal_custom_mode_change_email");
            }
        }
        public static string Pattern_personal_custom_mode_change_password
        {
            get
            {
                return JsonResourceManager.GetString("pattern_personal_custom_mode_change_password");
            }
        }
        public static string Pattern_personal_custom_mode_confirmation
        {
            get
            {
                return JsonResourceManager.GetString("pattern_personal_custom_mode_confirmation");
            }
        }
        public static string Pattern_personal_custom_mode_profile_delete
        {
            get
            {
                return JsonResourceManager.GetString("pattern_personal_custom_mode_profile_delete");
            }
        }
        public static string Pattern_remove_user_data_completed_custom_mode
        {
            get
            {
                return JsonResourceManager.GetString("pattern_remove_user_data_completed_custom_mode");
            }
        }
        public static string PresentationEditorCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("PresentationEditorCustomMode");
            }
        }
        public static string PresentationEditorDescriptionCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("PresentationEditorDescriptionCustomMode");
            }
        }
        public static string ProductAdminOpportunitiesCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("ProductAdminOpportunitiesCustomMode");
            }
        }
        public static string ProductUserOpportunitiesCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("ProductUserOpportunitiesCustomMode");
            }
        }
        public static string RegistryButtonCreateNowCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("RegistryButtonCreateNowCustomMode");
            }
        }
        public static string RegistrySettingSpamCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("RegistrySettingSpamCustomMode");
            }
        }
        public static string RegistrySettingTermsCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("RegistrySettingTermsCustomMode");
            }
        }
        public static string RememberCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("RememberCustomMode");
            }
        }
        public static string ReviewingCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("ReviewingCustomMode");
            }
        }
        public static string ReviewingDescriptionCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("ReviewingDescriptionCustomMode");
            }
        }
        public static string SendActivationEmailCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("SendActivationEmailCustomMode");
            }
        }
        public static string SignUpCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("SignUpCustomMode");
            }
        }
        public static string SignUpDesktopCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("SignUpDesktopCustomMode");
            }
        }
        public static string SmsAuthDescriptionCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("SmsAuthDescriptionCustomMode");
            }
        }
        public static string SpreadsheetEditorCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("SpreadsheetEditorCustomMode");
            }
        }
        public static string SpreadsheetEditorDescriptionCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("SpreadsheetEditorDescriptionCustomMode");
            }
        }
        public static string Subject_personal_custom_mode_after_registration1
        {
            get
            {
                return JsonResourceManager.GetString("subject_personal_custom_mode_after_registration1");
            }
        }
        public static string Subject_personal_custom_mode_after_registration7
        {
            get
            {
                return JsonResourceManager.GetString("subject_personal_custom_mode_after_registration7");
            }
        }
        public static string Subject_personal_custom_mode_change_email
        {
            get
            {
                return JsonResourceManager.GetString("subject_personal_custom_mode_change_email");
            }
        }
        public static string Subject_personal_custom_mode_change_password
        {
            get
            {
                return JsonResourceManager.GetString("subject_personal_custom_mode_change_password");
            }
        }
        public static string Subject_personal_custom_mode_confirmation
        {
            get
            {
                return JsonResourceManager.GetString("subject_personal_custom_mode_confirmation");
            }
        }
        public static string Subject_personal_custom_mode_profile_delete
        {
            get
            {
                return JsonResourceManager.GetString("subject_personal_custom_mode_profile_delete");
            }
        }
        public static string Subject_remove_user_data_completed_custom_mode
        {
            get
            {
                return JsonResourceManager.GetString("subject_remove_user_data_completed_custom_mode");
            }
        }
        public static string TariffOverdueStandaloneCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("TariffOverdueStandaloneCustomMode");
            }
        }
        public static string TariffPaidStandaloneCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("TariffPaidStandaloneCustomMode");
            }
        }
        public static string TermsServiceCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("TermsServiceCustomMode");
            }
        }
        public static string ThanksRegistrationCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("ThanksRegistrationCustomMode");
            }
        }
        public static string TitleCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("TitleCustomMode");
            }
        }
        public static string TitlePageNewCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("TitlePageNewCustomMode");
            }
        }
        public static string TrackingChangesCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("TrackingChangesCustomMode");
            }
        }
        public static string TrackingChangesDescriptionCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("TrackingChangesDescriptionCustomMode");
            }
        }
        public static string TwoEditingModesCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("TwoEditingModesCustomMode");
            }
        }
        public static string TwoEditingModesDescriptionCustomMode
        {
            get
            {
                return JsonResourceManager.GetString("TwoEditingModesDescriptionCustomMode");
            }
        }
    }
    public class WebstudioNotifyPatternResource
    {
        private static JsonResourceManager JsonResourceManager { get; set; }
        static WebstudioNotifyPatternResource()
        {
            JsonResourceManager = new JsonResourceManager("WebstudioNotifyPatternResource");
        }

        public static string ActionCreateBlog
        {
            get
            {
                return JsonResourceManager.GetString("ActionCreateBlog");
            }
        }
        public static string ActionCreateBookmark
        {
            get
            {
                return JsonResourceManager.GetString("ActionCreateBookmark");
            }
        }
        public static string ActionCreateCase
        {
            get
            {
                return JsonResourceManager.GetString("ActionCreateCase");
            }
        }
        public static string ActionCreateComment
        {
            get
            {
                return JsonResourceManager.GetString("ActionCreateComment");
            }
        }
        public static string ActionCreateContact
        {
            get
            {
                return JsonResourceManager.GetString("ActionCreateContact");
            }
        }
        public static string ActionCreateCrmTask
        {
            get
            {
                return JsonResourceManager.GetString("ActionCreateCrmTask");
            }
        }
        public static string ActionCreateDeal
        {
            get
            {
                return JsonResourceManager.GetString("ActionCreateDeal");
            }
        }
        public static string ActionCreateDiscussion
        {
            get
            {
                return JsonResourceManager.GetString("ActionCreateDiscussion");
            }
        }
        public static string ActionCreateEvent
        {
            get
            {
                return JsonResourceManager.GetString("ActionCreateEvent");
            }
        }
        public static string ActionCreateFile
        {
            get
            {
                return JsonResourceManager.GetString("ActionCreateFile");
            }
        }
        public static string ActionCreateFolder
        {
            get
            {
                return JsonResourceManager.GetString("ActionCreateFolder");
            }
        }
        public static string ActionCreateForum
        {
            get
            {
                return JsonResourceManager.GetString("ActionCreateForum");
            }
        }
        public static string ActionCreateForumPoll
        {
            get
            {
                return JsonResourceManager.GetString("ActionCreateForumPoll");
            }
        }
        public static string ActionCreateForumPost
        {
            get
            {
                return JsonResourceManager.GetString("ActionCreateForumPost");
            }
        }
        public static string ActionCreateMilestone
        {
            get
            {
                return JsonResourceManager.GetString("ActionCreateMilestone");
            }
        }
        public static string ActionCreateProject
        {
            get
            {
                return JsonResourceManager.GetString("ActionCreateProject");
            }
        }
        public static string ActionCreateTask
        {
            get
            {
                return JsonResourceManager.GetString("ActionCreateTask");
            }
        }
        public static string ButtonAccept
        {
            get
            {
                return JsonResourceManager.GetString("ButtonAccept");
            }
        }
        public static string ButtonAccessControlPanel
        {
            get
            {
                return JsonResourceManager.GetString("ButtonAccessControlPanel");
            }
        }
        public static string ButtonAccessCRMSystem
        {
            get
            {
                return JsonResourceManager.GetString("ButtonAccessCRMSystem");
            }
        }
        public static string ButtonAccessMail
        {
            get
            {
                return JsonResourceManager.GetString("ButtonAccessMail");
            }
        }
        public static string ButtonAccessYourPortal
        {
            get
            {
                return JsonResourceManager.GetString("ButtonAccessYourPortal");
            }
        }
        public static string ButtonAccessYouWebOffice
        {
            get
            {
                return JsonResourceManager.GetString("ButtonAccessYouWebOffice");
            }
        }
        public static string ButtonActivateEmail
        {
            get
            {
                return JsonResourceManager.GetString("ButtonActivateEmail");
            }
        }
        public static string ButtonBuyNow
        {
            get
            {
                return JsonResourceManager.GetString("ButtonBuyNow");
            }
        }
        public static string ButtonChangeEmail
        {
            get
            {
                return JsonResourceManager.GetString("ButtonChangeEmail");
            }
        }
        public static string ButtonChangePassword
        {
            get
            {
                return JsonResourceManager.GetString("ButtonChangePassword");
            }
        }
        public static string ButtonChangePhone
        {
            get
            {
                return JsonResourceManager.GetString("ButtonChangePhone");
            }
        }
        public static string ButtonChangeTfa
        {
            get
            {
                return JsonResourceManager.GetString("ButtonChangeTfa");
            }
        }
        public static string ButtonClickForConfirm
        {
            get
            {
                return JsonResourceManager.GetString("ButtonClickForConfirm");
            }
        }
        public static string ButtonConfigureRightNow
        {
            get
            {
                return JsonResourceManager.GetString("ButtonConfigureRightNow");
            }
        }
        public static string ButtonConfirm
        {
            get
            {
                return JsonResourceManager.GetString("ButtonConfirm");
            }
        }
        public static string ButtonConfirmPortalAddressChange
        {
            get
            {
                return JsonResourceManager.GetString("ButtonConfirmPortalAddressChange");
            }
        }
        public static string ButtonConfirmPortalOwnerUpdate
        {
            get
            {
                return JsonResourceManager.GetString("ButtonConfirmPortalOwnerUpdate");
            }
        }
        public static string ButtonConfirmTermination
        {
            get
            {
                return JsonResourceManager.GetString("ButtonConfirmTermination");
            }
        }
        public static string ButtonDeactivatePortal
        {
            get
            {
                return JsonResourceManager.GetString("ButtonDeactivatePortal");
            }
        }
        public static string ButtonDeletePortal
        {
            get
            {
                return JsonResourceManager.GetString("ButtonDeletePortal");
            }
        }
        public static string ButtonDownloadNow
        {
            get
            {
                return JsonResourceManager.GetString("ButtonDownloadNow");
            }
        }
        public static string ButtonExtendTrialButton
        {
            get
            {
                return JsonResourceManager.GetString("ButtonExtendTrialButton");
            }
        }
        public static string ButtonForConfirmation
        {
            get
            {
                return JsonResourceManager.GetString("ButtonForConfirmation");
            }
        }
        public static string ButtonGoToAppStore
        {
            get
            {
                return JsonResourceManager.GetString("ButtonGoToAppStore");
            }
        }
        public static string ButtonInviteRightNow
        {
            get
            {
                return JsonResourceManager.GetString("ButtonInviteRightNow");
            }
        }
        public static string ButtonJoin
        {
            get
            {
                return JsonResourceManager.GetString("ButtonJoin");
            }
        }
        public static string ButtonLeaveFeedback
        {
            get
            {
                return JsonResourceManager.GetString("ButtonLeaveFeedback");
            }
        }
        public static string ButtonMoveRightNow
        {
            get
            {
                return JsonResourceManager.GetString("ButtonMoveRightNow");
            }
        }
        public static string ButtonRemoveProfile
        {
            get
            {
                return JsonResourceManager.GetString("ButtonRemoveProfile");
            }
        }
        public static string ButtonRenewNow
        {
            get
            {
                return JsonResourceManager.GetString("ButtonRenewNow");
            }
        }
        public static string ButtonRequestCallButton
        {
            get
            {
                return JsonResourceManager.GetString("ButtonRequestCallButton");
            }
        }
        public static string ButtonSelectPricingPlans
        {
            get
            {
                return JsonResourceManager.GetString("ButtonSelectPricingPlans");
            }
        }
        public static string ButtonSendRequest
        {
            get
            {
                return JsonResourceManager.GetString("ButtonSendRequest");
            }
        }
        public static string ButtonSignUpPersonal
        {
            get
            {
                return JsonResourceManager.GetString("ButtonSignUpPersonal");
            }
        }
        public static string ButtonStartFreeTrial
        {
            get
            {
                return JsonResourceManager.GetString("ButtonStartFreeTrial");
            }
        }
        public static string ButtonStartNow
        {
            get
            {
                return JsonResourceManager.GetString("ButtonStartNow");
            }
        }
        public static string ButtonUseDiscount
        {
            get
            {
                return JsonResourceManager.GetString("ButtonUseDiscount");
            }
        }
        public static string Item3rdParty
        {
            get
            {
                return JsonResourceManager.GetString("Item3rdParty");
            }
        }
        public static string Item3rdPartyText
        {
            get
            {
                return JsonResourceManager.GetString("Item3rdPartyText");
            }
        }
        public static string ItemAddEmailAccount
        {
            get
            {
                return JsonResourceManager.GetString("ItemAddEmailAccount");
            }
        }
        public static string ItemAddFilesCreatWorkspace
        {
            get
            {
                return JsonResourceManager.GetString("ItemAddFilesCreatWorkspace");
            }
        }
        public static string ItemAddTeamlabMail
        {
            get
            {
                return JsonResourceManager.GetString("ItemAddTeamlabMail");
            }
        }
        public static string ItemAdjustRegionalSettings
        {
            get
            {
                return JsonResourceManager.GetString("ItemAdjustRegionalSettings");
            }
        }
        public static string ItemAdjustRegionalSettingsText
        {
            get
            {
                return JsonResourceManager.GetString("ItemAdjustRegionalSettingsText");
            }
        }
        public static string ItemARM
        {
            get
            {
                return JsonResourceManager.GetString("ItemARM");
            }
        }
        public static string ItemARMText
        {
            get
            {
                return JsonResourceManager.GetString("ItemARMText");
            }
        }
        public static string ItemBrandYourWebOffice
        {
            get
            {
                return JsonResourceManager.GetString("ItemBrandYourWebOffice");
            }
        }
        public static string ItemBrandYourWebOfficeText
        {
            get
            {
                return JsonResourceManager.GetString("ItemBrandYourWebOfficeText");
            }
        }
        public static string ItemCoAuthoringDocuments
        {
            get
            {
                return JsonResourceManager.GetString("ItemCoAuthoringDocuments");
            }
        }
        public static string ItemCreateWorkspaceDocs
        {
            get
            {
                return JsonResourceManager.GetString("ItemCreateWorkspaceDocs");
            }
        }
        public static string ItemCustomization
        {
            get
            {
                return JsonResourceManager.GetString("ItemCustomization");
            }
        }
        public static string ItemCustomizationText
        {
            get
            {
                return JsonResourceManager.GetString("ItemCustomizationText");
            }
        }
        public static string ItemCustomizeWebOfficeInterface
        {
            get
            {
                return JsonResourceManager.GetString("ItemCustomizeWebOfficeInterface");
            }
        }
        public static string ItemCustomizeWebOfficeInterfaceText
        {
            get
            {
                return JsonResourceManager.GetString("ItemCustomizeWebOfficeInterfaceText");
            }
        }
        public static string ItemEnterpriseDocsTips1
        {
            get
            {
                return JsonResourceManager.GetString("ItemEnterpriseDocsTips1");
            }
        }
        public static string ItemEnterpriseDocsTips2
        {
            get
            {
                return JsonResourceManager.GetString("ItemEnterpriseDocsTips2");
            }
        }
        public static string ItemEnterpriseDocsTips3
        {
            get
            {
                return JsonResourceManager.GetString("ItemEnterpriseDocsTips3");
            }
        }
        public static string ItemEnterpriseDocsTips4
        {
            get
            {
                return JsonResourceManager.GetString("ItemEnterpriseDocsTips4");
            }
        }
        public static string ItemEnterpriseDocsTips5
        {
            get
            {
                return JsonResourceManager.GetString("ItemEnterpriseDocsTips5");
            }
        }
        public static string ItemEnterpriseDocsTips6
        {
            get
            {
                return JsonResourceManager.GetString("ItemEnterpriseDocsTips6");
            }
        }
        public static string ItemEnterpriseDocsTips7
        {
            get
            {
                return JsonResourceManager.GetString("ItemEnterpriseDocsTips7");
            }
        }
        public static string ItemFeatureCommunity
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureCommunity");
            }
        }
        public static string ItemFeatureCommunityText
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureCommunityText");
            }
        }
        public static string ItemFeatureDocCoAuthoring
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureDocCoAuthoring");
            }
        }
        public static string ItemFeatureDocCoAuthoringText
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureDocCoAuthoringText");
            }
        }
        public static string ItemFeatureEmailSignature
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureEmailSignature");
            }
        }
        public static string ItemFeatureEmailSignatureText
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureEmailSignatureText");
            }
        }
        public static string ItemFeatureFolderForAtts
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureFolderForAtts");
            }
        }
        public static string ItemFeatureFolderForAttsText
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureFolderForAttsText");
            }
        }
        public static string ItemFeatureGanttChart
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureGanttChart");
            }
        }
        public static string ItemFeatureGanttChartText
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureGanttChartText");
            }
        }
        public static string ItemFeatureLinksVSAttachments
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureLinksVSAttachments");
            }
        }
        public static string ItemFeatureLinksVSAttachmentsText
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureLinksVSAttachmentsText");
            }
        }
        public static string ItemFeatureMailboxAliases
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureMailboxAliases");
            }
        }
        public static string ItemFeatureMailboxAliasesText
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureMailboxAliasesText");
            }
        }
        public static string ItemFeatureMailGroups
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureMailGroups");
            }
        }
        public static string ItemFeatureMailGroupsText
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureMailGroupsText");
            }
        }
        public static string ItemFeatureProjectDiscussions
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureProjectDiscussions");
            }
        }
        public static string ItemFeatureProjectDiscussionsText
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureProjectDiscussionsText");
            }
        }
        public static string ItemFeatureTalk
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureTalk");
            }
        }
        public static string ItemFeatureTalkText
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureTalkText");
            }
        }
        public static string ItemFeatureTips1CoEditingText
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureTips1CoEditingText");
            }
        }
        public static string ItemFeatureTips2VersionHistoryText
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureTips2VersionHistoryText");
            }
        }
        public static string ItemFeatureTips3ShareDocsText
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureTips3ShareDocsText");
            }
        }
        public static string ItemFeatureTips3ShareDocsTextEnterprise
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureTips3ShareDocsTextEnterprise");
            }
        }
        public static string ItemFeatureTips4CloudStoragesText
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureTips4CloudStoragesText");
            }
        }
        public static string ItemFeatureTips4MailMergeText
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureTips4MailMergeText");
            }
        }
        public static string ItemFeatureTips5iOSText
        {
            get
            {
                return JsonResourceManager.GetString("ItemFeatureTips5iOSText");
            }
        }
        public static string ItemImportProjects
        {
            get
            {
                return JsonResourceManager.GetString("ItemImportProjects");
            }
        }
        public static string ItemImportProjectsBasecamp
        {
            get
            {
                return JsonResourceManager.GetString("ItemImportProjectsBasecamp");
            }
        }
        public static string ItemIntegrateDocs
        {
            get
            {
                return JsonResourceManager.GetString("ItemIntegrateDocs");
            }
        }
        public static string ItemIntegrateIM
        {
            get
            {
                return JsonResourceManager.GetString("ItemIntegrateIM");
            }
        }
        public static string ItemLinkWithProjects
        {
            get
            {
                return JsonResourceManager.GetString("ItemLinkWithProjects");
            }
        }
        public static string ItemLinkWithProjectsText
        {
            get
            {
                return JsonResourceManager.GetString("ItemLinkWithProjectsText");
            }
        }
        public static string ItemMailIntegration
        {
            get
            {
                return JsonResourceManager.GetString("ItemMailIntegration");
            }
        }
        public static string ItemMailIntegrationText
        {
            get
            {
                return JsonResourceManager.GetString("ItemMailIntegrationText");
            }
        }
        public static string ItemModulesAndTools
        {
            get
            {
                return JsonResourceManager.GetString("ItemModulesAndTools");
            }
        }
        public static string ItemModulesAndToolsText
        {
            get
            {
                return JsonResourceManager.GetString("ItemModulesAndToolsText");
            }
        }
        public static string ItemOpensourceDocsTips1
        {
            get
            {
                return JsonResourceManager.GetString("ItemOpensourceDocsTips1");
            }
        }
        public static string ItemOpensourceDocsTips2
        {
            get
            {
                return JsonResourceManager.GetString("ItemOpensourceDocsTips2");
            }
        }
        public static string ItemOpensourceDocsTips3
        {
            get
            {
                return JsonResourceManager.GetString("ItemOpensourceDocsTips3");
            }
        }
        public static string ItemOpensourceDocsTips4
        {
            get
            {
                return JsonResourceManager.GetString("ItemOpensourceDocsTips4");
            }
        }
        public static string ItemOpensourceDocsTips5
        {
            get
            {
                return JsonResourceManager.GetString("ItemOpensourceDocsTips5");
            }
        }
        public static string ItemOpensourceDocsTips6
        {
            get
            {
                return JsonResourceManager.GetString("ItemOpensourceDocsTips6");
            }
        }
        public static string ItemOpensourceDocsTips7
        {
            get
            {
                return JsonResourceManager.GetString("ItemOpensourceDocsTips7");
            }
        }
        public static string ItemSaasDocsTips1
        {
            get
            {
                return JsonResourceManager.GetString("ItemSaasDocsTips1");
            }
        }
        public static string ItemSaasDocsTips2
        {
            get
            {
                return JsonResourceManager.GetString("ItemSaasDocsTips2");
            }
        }
        public static string ItemSaasDocsTips3
        {
            get
            {
                return JsonResourceManager.GetString("ItemSaasDocsTips3");
            }
        }
        public static string ItemSaasDocsTips4
        {
            get
            {
                return JsonResourceManager.GetString("ItemSaasDocsTips4");
            }
        }
        public static string ItemSaasDocsTips5
        {
            get
            {
                return JsonResourceManager.GetString("ItemSaasDocsTips5");
            }
        }
        public static string ItemSaasDocsTips6
        {
            get
            {
                return JsonResourceManager.GetString("ItemSaasDocsTips6");
            }
        }
        public static string ItemSaasDocsTips7
        {
            get
            {
                return JsonResourceManager.GetString("ItemSaasDocsTips7");
            }
        }
        public static string ItemSecureAccess
        {
            get
            {
                return JsonResourceManager.GetString("ItemSecureAccess");
            }
        }
        public static string ItemSecureAccessText
        {
            get
            {
                return JsonResourceManager.GetString("ItemSecureAccessText");
            }
        }
        public static string ItemShareDocuments
        {
            get
            {
                return JsonResourceManager.GetString("ItemShareDocuments");
            }
        }
        public static string ItemTryOnlineDocEditor
        {
            get
            {
                return JsonResourceManager.GetString("ItemTryOnlineDocEditor");
            }
        }
        public static string ItemUploadCrm
        {
            get
            {
                return JsonResourceManager.GetString("ItemUploadCrm");
            }
        }
        public static string ItemUploadCrmContacts
        {
            get
            {
                return JsonResourceManager.GetString("ItemUploadCrmContacts");
            }
        }
        public static string ItemUploadCrmContactsCsv
        {
            get
            {
                return JsonResourceManager.GetString("ItemUploadCrmContactsCsv");
            }
        }
        public static string ItemVoIP
        {
            get
            {
                return JsonResourceManager.GetString("ItemVoIP");
            }
        }
        public static string ItemVoIPText
        {
            get
            {
                return JsonResourceManager.GetString("ItemVoIPText");
            }
        }
        public static string ItemWebToLead
        {
            get
            {
                return JsonResourceManager.GetString("ItemWebToLead");
            }
        }
        public static string ItemWebToLeadText
        {
            get
            {
                return JsonResourceManager.GetString("ItemWebToLeadText");
            }
        }
        public static string LinkLearnMore
        {
            get
            {
                return JsonResourceManager.GetString("LinkLearnMore");
            }
        }
        public static string Pattern_activate
        {
            get
            {
                return JsonResourceManager.GetString("pattern_activate");
            }
        }
        public static string Pattern_activate_email
        {
            get
            {
                return JsonResourceManager.GetString("pattern_activate_email");
            }
        }
        public static string Pattern_activate_enterprise
        {
            get
            {
                return JsonResourceManager.GetString("pattern_activate_enterprise");
            }
        }
        public static string Pattern_activate_freecloud
        {
            get
            {
                return JsonResourceManager.GetString("pattern_activate_freecloud");
            }
        }
        public static string Pattern_activate_guest
        {
            get
            {
                return JsonResourceManager.GetString("pattern_activate_guest");
            }
        }
        public static string Pattern_activate_guest_enterprise
        {
            get
            {
                return JsonResourceManager.GetString("pattern_activate_guest_enterprise");
            }
        }
        public static string Pattern_activate_guest_freecloud
        {
            get
            {
                return JsonResourceManager.GetString("pattern_activate_guest_freecloud");
            }
        }
        public static string Pattern_activate_guest_whitelabel
        {
            get
            {
                return JsonResourceManager.GetString("pattern_activate_guest_whitelabel");
            }
        }
        public static string Pattern_activate_personal
        {
            get
            {
                return JsonResourceManager.GetString("pattern_activate_personal");
            }
        }
        public static string Pattern_activate_whitelabel
        {
            get
            {
                return JsonResourceManager.GetString("pattern_activate_whitelabel");
            }
        }
        public static string Pattern_admin_sms_balance
        {
            get
            {
                return JsonResourceManager.GetString("pattern_admin_sms_balance");
            }
        }
        public static string Pattern_admin_voip_blocked
        {
            get
            {
                return JsonResourceManager.GetString("pattern_admin_voip_blocked");
            }
        }
        public static string Pattern_admin_voip_warning
        {
            get
            {
                return JsonResourceManager.GetString("pattern_admin_voip_warning");
            }
        }
        public static string Pattern_admin_welcome
        {
            get
            {
                return JsonResourceManager.GetString("pattern_admin_welcome");
            }
        }
        public static string Pattern_admin_welcome_enterprise
        {
            get
            {
                return JsonResourceManager.GetString("pattern_admin_welcome_enterprise");
            }
        }
        public static string Pattern_admin_welcome_whitelabel
        {
            get
            {
                return JsonResourceManager.GetString("pattern_admin_welcome_whitelabel");
            }
        }
        public static string Pattern_after_creation1
        {
            get
            {
                return JsonResourceManager.GetString("pattern_after_creation1");
            }
        }
        public static string Pattern_after_creation1_enterprise
        {
            get
            {
                return JsonResourceManager.GetString("pattern_after_creation1_enterprise");
            }
        }
        public static string Pattern_after_creation1_freecloud
        {
            get
            {
                return JsonResourceManager.GetString("pattern_after_creation1_freecloud");
            }
        }
        public static string Pattern_after_creation2
        {
            get
            {
                return JsonResourceManager.GetString("pattern_after_creation2");
            }
        }
        public static string Pattern_after_creation2_enterprise
        {
            get
            {
                return JsonResourceManager.GetString("pattern_after_creation2_enterprise");
            }
        }
        public static string Pattern_after_creation3
        {
            get
            {
                return JsonResourceManager.GetString("pattern_after_creation3");
            }
        }
        public static string Pattern_after_creation3_enterprise
        {
            get
            {
                return JsonResourceManager.GetString("pattern_after_creation3_enterprise");
            }
        }
        public static string Pattern_after_creation30_freecloud
        {
            get
            {
                return JsonResourceManager.GetString("pattern_after_creation30_freecloud");
            }
        }
        public static string Pattern_after_creation4
        {
            get
            {
                return JsonResourceManager.GetString("pattern_after_creation4");
            }
        }
        public static string Pattern_after_creation4_enterprise
        {
            get
            {
                return JsonResourceManager.GetString("pattern_after_creation4_enterprise");
            }
        }
        public static string Pattern_after_creation5
        {
            get
            {
                return JsonResourceManager.GetString("pattern_after_creation5");
            }
        }
        public static string Pattern_after_creation6
        {
            get
            {
                return JsonResourceManager.GetString("pattern_after_creation6");
            }
        }
        public static string Pattern_after_creation6_enterprise
        {
            get
            {
                return JsonResourceManager.GetString("pattern_after_creation6_enterprise");
            }
        }
        public static string Pattern_after_creation7
        {
            get
            {
                return JsonResourceManager.GetString("pattern_after_creation7");
            }
        }
        public static string Pattern_after_creation7_enterprise
        {
            get
            {
                return JsonResourceManager.GetString("pattern_after_creation7_enterprise");
            }
        }
        public static string Pattern_after_creation8_enterprise
        {
            get
            {
                return JsonResourceManager.GetString("pattern_after_creation8_enterprise");
            }
        }
        public static string Pattern_after_payment1
        {
            get
            {
                return JsonResourceManager.GetString("pattern_after_payment1");
            }
        }
        public static string Pattern_after_registration_personal1
        {
            get
            {
                return JsonResourceManager.GetString("pattern_after_registration_personal1");
            }
        }
        public static string Pattern_after_registration_personal14
        {
            get
            {
                return JsonResourceManager.GetString("pattern_after_registration_personal14");
            }
        }
        public static string Pattern_after_registration_personal21
        {
            get
            {
                return JsonResourceManager.GetString("pattern_after_registration_personal21");
            }
        }
        public static string Pattern_after_registration_personal7
        {
            get
            {
                return JsonResourceManager.GetString("pattern_after_registration_personal7");
            }
        }
        public static string Pattern_backup_created
        {
            get
            {
                return JsonResourceManager.GetString("pattern_backup_created");
            }
        }
        public static string Pattern_change_email
        {
            get
            {
                return JsonResourceManager.GetString("pattern_change_email");
            }
        }
        public static string Pattern_change_email_personal
        {
            get
            {
                return JsonResourceManager.GetString("pattern_change_email_personal");
            }
        }
        public static string Pattern_change_password
        {
            get
            {
                return JsonResourceManager.GetString("pattern_change_password");
            }
        }
        public static string Pattern_change_password_personal
        {
            get
            {
                return JsonResourceManager.GetString("pattern_change_password_personal");
            }
        }
        public static string Pattern_change_phone
        {
            get
            {
                return JsonResourceManager.GetString("pattern_change_phone");
            }
        }
        public static string Pattern_change_pwd
        {
            get
            {
                return JsonResourceManager.GetString("pattern_change_pwd");
            }
        }
        public static string Pattern_change_tfa
        {
            get
            {
                return JsonResourceManager.GetString("pattern_change_tfa");
            }
        }
        public static string Pattern_confirm_owner_change
        {
            get
            {
                return JsonResourceManager.GetString("pattern_confirm_owner_change");
            }
        }
        public static string Pattern_confirmation_personal
        {
            get
            {
                return JsonResourceManager.GetString("pattern_confirmation_personal");
            }
        }
        public static string Pattern_congratulations
        {
            get
            {
                return JsonResourceManager.GetString("pattern_congratulations");
            }
        }
        public static string Pattern_congratulations_enterprise
        {
            get
            {
                return JsonResourceManager.GetString("pattern_congratulations_enterprise");
            }
        }
        public static string Pattern_congratulations_freecloud
        {
            get
            {
                return JsonResourceManager.GetString("pattern_congratulations_freecloud");
            }
        }
        public static string Pattern_congratulations_whitelabel
        {
            get
            {
                return JsonResourceManager.GetString("pattern_congratulations_whitelabel");
            }
        }
        public static string Pattern_dns_change
        {
            get
            {
                return JsonResourceManager.GetString("pattern_dns_change");
            }
        }
        public static string Pattern_enterprise_admin_activation
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_activation");
            }
        }
        public static string Pattern_enterprise_admin_activation_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_activation_v10");
            }
        }
        public static string Pattern_enterprise_admin_customize_portal
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_customize_portal");
            }
        }
        public static string Pattern_enterprise_admin_customize_portal_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_customize_portal_v10");
            }
        }
        public static string Pattern_enterprise_admin_customize_portal_v10_item_3rdparty
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_customize_portal_v10_item_3rdparty");
            }
        }
        public static string Pattern_enterprise_admin_customize_portal_v10_item_3rdparty_hdr
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_customize_portal_v10_item_3rdparty_hdr");
            }
        }
        public static string Pattern_enterprise_admin_customize_portal_v10_item_brand
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_customize_portal_v10_item_brand");
            }
        }
        public static string Pattern_enterprise_admin_customize_portal_v10_item_brand_hdr
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_customize_portal_v10_item_brand_hdr");
            }
        }
        public static string Pattern_enterprise_admin_customize_portal_v10_item_customize
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_customize_portal_v10_item_customize");
            }
        }
        public static string Pattern_enterprise_admin_customize_portal_v10_item_customize_hdr
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_customize_portal_v10_item_customize_hdr");
            }
        }
        public static string Pattern_enterprise_admin_customize_portal_v10_item_modules
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_customize_portal_v10_item_modules");
            }
        }
        public static string Pattern_enterprise_admin_customize_portal_v10_item_modules_hdr
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_customize_portal_v10_item_modules_hdr");
            }
        }
        public static string Pattern_enterprise_admin_customize_portal_v10_item_regional
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_customize_portal_v10_item_regional");
            }
        }
        public static string Pattern_enterprise_admin_customize_portal_v10_item_regional_hdr
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_customize_portal_v10_item_regional_hdr");
            }
        }
        public static string Pattern_enterprise_admin_invite_teammates
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_invite_teammates");
            }
        }
        public static string Pattern_enterprise_admin_invite_teammates_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_invite_teammates_v10");
            }
        }
        public static string Pattern_enterprise_admin_payment_warning
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_payment_warning");
            }
        }
        public static string Pattern_enterprise_admin_payment_warning_before7
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_payment_warning_before7");
            }
        }
        public static string Pattern_enterprise_admin_payment_warning_before7_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_payment_warning_before7_v10");
            }
        }
        public static string Pattern_enterprise_admin_payment_warning_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_payment_warning_v10");
            }
        }
        public static string Pattern_enterprise_admin_trial_warning
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_trial_warning");
            }
        }
        public static string Pattern_enterprise_admin_trial_warning_before7
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_trial_warning_before7");
            }
        }
        public static string Pattern_enterprise_admin_trial_warning_before7_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_trial_warning_before7_v10");
            }
        }
        public static string Pattern_enterprise_admin_trial_warning_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_trial_warning_v10");
            }
        }
        public static string Pattern_enterprise_admin_user_apps_tips_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_user_apps_tips_v10");
            }
        }
        public static string Pattern_enterprise_admin_user_crm_tips
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_user_crm_tips");
            }
        }
        public static string Pattern_enterprise_admin_user_docs_tips
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_user_docs_tips");
            }
        }
        public static string Pattern_enterprise_admin_user_docs_tips_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_user_docs_tips_v10");
            }
        }
        public static string Pattern_enterprise_admin_user_mail_tips
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_user_mail_tips");
            }
        }
        public static string Pattern_enterprise_admin_user_powerful_tips
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_user_powerful_tips");
            }
        }
        public static string Pattern_enterprise_admin_user_team_tips
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_user_team_tips");
            }
        }
        public static string Pattern_enterprise_admin_welcome
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_welcome");
            }
        }
        public static string Pattern_enterprise_admin_welcome_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_welcome_v10");
            }
        }
        public static string Pattern_enterprise_admin_without_activity
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_without_activity");
            }
        }
        public static string Pattern_enterprise_admin_without_activity_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_admin_without_activity_v10");
            }
        }
        public static string Pattern_enterprise_guest_activation
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_guest_activation");
            }
        }
        public static string Pattern_enterprise_guest_activation_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_guest_activation_v10");
            }
        }
        public static string Pattern_enterprise_guest_welcome
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_guest_welcome");
            }
        }
        public static string Pattern_enterprise_guest_welcome_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_guest_welcome_v10");
            }
        }
        public static string Pattern_enterprise_user_activation
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_user_activation");
            }
        }
        public static string Pattern_enterprise_user_activation_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_user_activation_v10");
            }
        }
        public static string Pattern_enterprise_user_organize_workplace
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_user_organize_workplace");
            }
        }
        public static string Pattern_enterprise_user_welcome
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_user_welcome");
            }
        }
        public static string Pattern_enterprise_user_welcome_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_user_welcome_v10");
            }
        }
        public static string Pattern_enterprise_whitelabel_admin_activation
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_whitelabel_admin_activation");
            }
        }
        public static string Pattern_enterprise_whitelabel_admin_activation_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_whitelabel_admin_activation_v10");
            }
        }
        public static string Pattern_enterprise_whitelabel_admin_customize_portal_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_whitelabel_admin_customize_portal_v10");
            }
        }
        public static string Pattern_enterprise_whitelabel_admin_payment_warning
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_whitelabel_admin_payment_warning");
            }
        }
        public static string Pattern_enterprise_whitelabel_admin_payment_warning_before7
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_whitelabel_admin_payment_warning_before7");
            }
        }
        public static string Pattern_enterprise_whitelabel_admin_payment_warning_before7_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_whitelabel_admin_payment_warning_before7_v10");
            }
        }
        public static string Pattern_enterprise_whitelabel_admin_payment_warning_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_whitelabel_admin_payment_warning_v10");
            }
        }
        public static string Pattern_enterprise_whitelabel_admin_welcome
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_whitelabel_admin_welcome");
            }
        }
        public static string Pattern_enterprise_whitelabel_admin_welcome_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_whitelabel_admin_welcome_v10");
            }
        }
        public static string Pattern_enterprise_whitelabel_guest_activation
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_whitelabel_guest_activation");
            }
        }
        public static string Pattern_enterprise_whitelabel_guest_activation_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_whitelabel_guest_activation_v10");
            }
        }
        public static string Pattern_enterprise_whitelabel_guest_welcome
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_whitelabel_guest_welcome");
            }
        }
        public static string Pattern_enterprise_whitelabel_guest_welcome_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_whitelabel_guest_welcome_v10");
            }
        }
        public static string Pattern_enterprise_whitelabel_user_activation
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_whitelabel_user_activation");
            }
        }
        public static string Pattern_enterprise_whitelabel_user_activation_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_whitelabel_user_activation_v10");
            }
        }
        public static string Pattern_enterprise_whitelabel_user_welcome
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_whitelabel_user_welcome");
            }
        }
        public static string Pattern_enterprise_whitelabel_user_welcome_r7
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_whitelabel_user_welcome_r7");
            }
        }
        public static string Pattern_enterprise_whitelabel_user_welcome_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_enterprise_whitelabel_user_welcome_v10");
            }
        }
        public static string Pattern_for_admin_notify
        {
            get
            {
                return JsonResourceManager.GetString("pattern_for_admin_notify");
            }
        }
        public static string Pattern_freecloud_admin_activation
        {
            get
            {
                return JsonResourceManager.GetString("pattern_freecloud_admin_activation");
            }
        }
        public static string Pattern_freecloud_guest_activation
        {
            get
            {
                return JsonResourceManager.GetString("pattern_freecloud_guest_activation");
            }
        }
        public static string Pattern_freecloud_guest_welcome
        {
            get
            {
                return JsonResourceManager.GetString("pattern_freecloud_guest_welcome");
            }
        }
        public static string Pattern_freecloud_user_activation
        {
            get
            {
                return JsonResourceManager.GetString("pattern_freecloud_user_activation");
            }
        }
        public static string Pattern_freecloud_user_welcome
        {
            get
            {
                return JsonResourceManager.GetString("pattern_freecloud_user_welcome");
            }
        }
        public static string Pattern_hosted_admin_activation
        {
            get
            {
                return JsonResourceManager.GetString("pattern_hosted_admin_activation");
            }
        }
        public static string Pattern_hosted_admin_invite_teammates
        {
            get
            {
                return JsonResourceManager.GetString("pattern_hosted_admin_invite_teammates");
            }
        }
        public static string Pattern_hosted_admin_welcome
        {
            get
            {
                return JsonResourceManager.GetString("pattern_hosted_admin_welcome");
            }
        }
        public static string Pattern_hosted_guest_activation
        {
            get
            {
                return JsonResourceManager.GetString("pattern_hosted_guest_activation");
            }
        }
        public static string Pattern_hosted_guest_welcome
        {
            get
            {
                return JsonResourceManager.GetString("pattern_hosted_guest_welcome");
            }
        }
        public static string Pattern_hosted_user_activation
        {
            get
            {
                return JsonResourceManager.GetString("pattern_hosted_user_activation");
            }
        }
        public static string Pattern_hosted_user_welcome
        {
            get
            {
                return JsonResourceManager.GetString("pattern_hosted_user_welcome");
            }
        }
        public static string Pattern_hosted_whitelabel_admin_activation
        {
            get
            {
                return JsonResourceManager.GetString("pattern_hosted_whitelabel_admin_activation");
            }
        }
        public static string Pattern_hosted_whitelabel_admin_welcome
        {
            get
            {
                return JsonResourceManager.GetString("pattern_hosted_whitelabel_admin_welcome");
            }
        }
        public static string Pattern_hosted_whitelabel_guest_activation
        {
            get
            {
                return JsonResourceManager.GetString("pattern_hosted_whitelabel_guest_activation");
            }
        }
        public static string Pattern_hosted_whitelabel_guest_welcome
        {
            get
            {
                return JsonResourceManager.GetString("pattern_hosted_whitelabel_guest_welcome");
            }
        }
        public static string Pattern_hosted_whitelabel_user_activation
        {
            get
            {
                return JsonResourceManager.GetString("pattern_hosted_whitelabel_user_activation");
            }
        }
        public static string Pattern_hosted_whitelabel_user_welcome
        {
            get
            {
                return JsonResourceManager.GetString("pattern_hosted_whitelabel_user_welcome");
            }
        }
        public static string Pattern_invite
        {
            get
            {
                return JsonResourceManager.GetString("pattern_invite");
            }
        }
        public static string Pattern_join
        {
            get
            {
                return JsonResourceManager.GetString("pattern_join");
            }
        }
        public static string Pattern_mailbox_created
        {
            get
            {
                return JsonResourceManager.GetString("pattern_mailbox_created");
            }
        }
        public static string Pattern_mailbox_password_changed
        {
            get
            {
                return JsonResourceManager.GetString("pattern_mailbox_password_changed");
            }
        }
        public static string Pattern_mailbox_without_settings_created
        {
            get
            {
                return JsonResourceManager.GetString("pattern_mailbox_without_settings_created");
            }
        }
        public static string Pattern_migration_error
        {
            get
            {
                return JsonResourceManager.GetString("pattern_migration_error");
            }
        }
        public static string Pattern_migration_server_failure
        {
            get
            {
                return JsonResourceManager.GetString("pattern_migration_server_failure");
            }
        }
        public static string Pattern_migration_start
        {
            get
            {
                return JsonResourceManager.GetString("pattern_migration_start");
            }
        }
        public static string Pattern_migration_success
        {
            get
            {
                return JsonResourceManager.GetString("pattern_migration_success");
            }
        }
        public static string Pattern_opensource_admin_activation
        {
            get
            {
                return JsonResourceManager.GetString("pattern_opensource_admin_activation");
            }
        }
        public static string Pattern_opensource_admin_docs_tips
        {
            get
            {
                return JsonResourceManager.GetString("pattern_opensource_admin_docs_tips");
            }
        }
        public static string Pattern_opensource_admin_security_tips
        {
            get
            {
                return JsonResourceManager.GetString("pattern_opensource_admin_security_tips");
            }
        }
        public static string Pattern_payment_warning
        {
            get
            {
                return JsonResourceManager.GetString("pattern_payment_warning");
            }
        }
        public static string Pattern_payment_warning_after3
        {
            get
            {
                return JsonResourceManager.GetString("pattern_payment_warning_after3");
            }
        }
        public static string Pattern_payment_warning_before7
        {
            get
            {
                return JsonResourceManager.GetString("pattern_payment_warning_before7");
            }
        }
        public static string Pattern_payment_warning_before7_whitelabel
        {
            get
            {
                return JsonResourceManager.GetString("pattern_payment_warning_before7_whitelabel");
            }
        }
        public static string Pattern_payment_warning_delaydue
        {
            get
            {
                return JsonResourceManager.GetString("pattern_payment_warning_delaydue");
            }
        }
        public static string Pattern_payment_warning_whitelabel
        {
            get
            {
                return JsonResourceManager.GetString("pattern_payment_warning_whitelabel");
            }
        }
        public static string Pattern_personal_activate
        {
            get
            {
                return JsonResourceManager.GetString("pattern_personal_activate");
            }
        }
        public static string Pattern_personal_after_registration1
        {
            get
            {
                return JsonResourceManager.GetString("pattern_personal_after_registration1");
            }
        }
        public static string Pattern_personal_after_registration14
        {
            get
            {
                return JsonResourceManager.GetString("pattern_personal_after_registration14");
            }
        }
        public static string Pattern_personal_after_registration21
        {
            get
            {
                return JsonResourceManager.GetString("pattern_personal_after_registration21");
            }
        }
        public static string Pattern_personal_after_registration28
        {
            get
            {
                return JsonResourceManager.GetString("pattern_personal_after_registration28");
            }
        }
        public static string Pattern_personal_after_registration7
        {
            get
            {
                return JsonResourceManager.GetString("pattern_personal_after_registration7");
            }
        }
        public static string Pattern_personal_change_email
        {
            get
            {
                return JsonResourceManager.GetString("pattern_personal_change_email");
            }
        }
        public static string Pattern_personal_change_password
        {
            get
            {
                return JsonResourceManager.GetString("pattern_personal_change_password");
            }
        }
        public static string Pattern_personal_confirmation
        {
            get
            {
                return JsonResourceManager.GetString("pattern_personal_confirmation");
            }
        }
        public static string Pattern_personal_profile_delete
        {
            get
            {
                return JsonResourceManager.GetString("pattern_personal_profile_delete");
            }
        }
        public static string Pattern_personal_r7_after_registration1
        {
            get
            {
                return JsonResourceManager.GetString("pattern_personal_r7_after_registration1");
            }
        }
        public static string Pattern_personal_r7_after_registration7
        {
            get
            {
                return JsonResourceManager.GetString("pattern_personal_r7_after_registration7");
            }
        }
        public static string Pattern_personal_r7_change_email
        {
            get
            {
                return JsonResourceManager.GetString("pattern_personal_r7_change_email");
            }
        }
        public static string Pattern_personal_r7_change_password
        {
            get
            {
                return JsonResourceManager.GetString("pattern_personal_r7_change_password");
            }
        }
        public static string Pattern_personal_r7_confirmation
        {
            get
            {
                return JsonResourceManager.GetString("pattern_personal_r7_confirmation");
            }
        }
        public static string Pattern_personal_r7_profile_delete
        {
            get
            {
                return JsonResourceManager.GetString("pattern_personal_r7_profile_delete");
            }
        }
        public static string Pattern_portal_deactivate
        {
            get
            {
                return JsonResourceManager.GetString("pattern_portal_deactivate");
            }
        }
        public static string Pattern_portal_delete
        {
            get
            {
                return JsonResourceManager.GetString("pattern_portal_delete");
            }
        }
        public static string Pattern_portal_delete_success
        {
            get
            {
                return JsonResourceManager.GetString("pattern_portal_delete_success");
            }
        }
        public static string Pattern_portal_delete_success_freecloud
        {
            get
            {
                return JsonResourceManager.GetString("pattern_portal_delete_success_freecloud");
            }
        }
        public static string Pattern_portal_delete_success_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_portal_delete_success_v10");
            }
        }
        public static string Pattern_portal_rename
        {
            get
            {
                return JsonResourceManager.GetString("pattern_portal_rename");
            }
        }
        public static string Pattern_profile_delete
        {
            get
            {
                return JsonResourceManager.GetString("pattern_profile_delete");
            }
        }
        public static string Pattern_profile_has_deleted_itself
        {
            get
            {
                return JsonResourceManager.GetString("pattern_profile_has_deleted_itself");
            }
        }
        public static string Pattern_profile_updated
        {
            get
            {
                return JsonResourceManager.GetString("pattern_profile_updated");
            }
        }
        public static string Pattern_pwd_reminder
        {
            get
            {
                return JsonResourceManager.GetString("pattern_pwd_reminder");
            }
        }
        public static string Pattern_reassigns_completed
        {
            get
            {
                return JsonResourceManager.GetString("pattern_reassigns_completed");
            }
        }
        public static string Pattern_reassigns_failed
        {
            get
            {
                return JsonResourceManager.GetString("pattern_reassigns_failed");
            }
        }
        public static string Pattern_remove_user_data_completed
        {
            get
            {
                return JsonResourceManager.GetString("pattern_remove_user_data_completed");
            }
        }
        public static string Pattern_remove_user_data_failed
        {
            get
            {
                return JsonResourceManager.GetString("pattern_remove_user_data_failed");
            }
        }
        public static string Pattern_request_license
        {
            get
            {
                return JsonResourceManager.GetString("pattern_request_license");
            }
        }
        public static string Pattern_request_tariff
        {
            get
            {
                return JsonResourceManager.GetString("pattern_request_tariff");
            }
        }
        public static string Pattern_restore_completed
        {
            get
            {
                return JsonResourceManager.GetString("pattern_restore_completed");
            }
        }
        public static string Pattern_restore_started
        {
            get
            {
                return JsonResourceManager.GetString("pattern_restore_started");
            }
        }
        public static string Pattern_saas_admin_activation
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_activation");
            }
        }
        public static string Pattern_saas_admin_activation_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_activation_v10");
            }
        }
        public static string Pattern_saas_admin_invite_teammates
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_invite_teammates");
            }
        }
        public static string Pattern_saas_admin_invite_teammates_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_invite_teammates_v10");
            }
        }
        public static string Pattern_saas_admin_payment_after_monthly_subscriptions
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_payment_after_monthly_subscriptions");
            }
        }
        public static string Pattern_saas_admin_payment_after_monthly_subscriptions_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_payment_after_monthly_subscriptions_v10");
            }
        }
        public static string Pattern_saas_admin_payment_warning
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_payment_warning");
            }
        }
        public static string Pattern_saas_admin_payment_warning_after1_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_payment_warning_after1_v10");
            }
        }
        public static string Pattern_saas_admin_payment_warning_after3
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_payment_warning_after3");
            }
        }
        public static string Pattern_saas_admin_payment_warning_before5_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_payment_warning_before5_v10");
            }
        }
        public static string Pattern_saas_admin_payment_warning_before7
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_payment_warning_before7");
            }
        }
        public static string Pattern_saas_admin_payment_warning_delaydue
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_payment_warning_delaydue");
            }
        }
        public static string Pattern_saas_admin_trial_warning
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_trial_warning");
            }
        }
        public static string Pattern_saas_admin_trial_warning_after_half_year
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_trial_warning_after_half_year");
            }
        }
        public static string Pattern_saas_admin_trial_warning_after_half_year_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_trial_warning_after_half_year_v10");
            }
        }
        public static string Pattern_saas_admin_trial_warning_after30
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_trial_warning_after30");
            }
        }
        public static string Pattern_saas_admin_trial_warning_after30_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_trial_warning_after30_v10");
            }
        }
        public static string Pattern_saas_admin_trial_warning_after5
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_trial_warning_after5");
            }
        }
        public static string Pattern_saas_admin_trial_warning_after5_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_trial_warning_after5_v10");
            }
        }
        public static string Pattern_saas_admin_trial_warning_before5
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_trial_warning_before5");
            }
        }
        public static string Pattern_saas_admin_trial_warning_before5_coupon
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_trial_warning_before5_coupon");
            }
        }
        public static string Pattern_saas_admin_trial_warning_before5_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_trial_warning_before5_v10");
            }
        }
        public static string Pattern_saas_admin_trial_warning_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_trial_warning_v10");
            }
        }
        public static string Pattern_saas_admin_user_apps_tips_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_user_apps_tips_v10");
            }
        }
        public static string Pattern_saas_admin_user_comfort_tips_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_user_comfort_tips_v10");
            }
        }
        public static string Pattern_saas_admin_user_crm_tips
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_user_crm_tips");
            }
        }
        public static string Pattern_saas_admin_user_docs_tips
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_user_docs_tips");
            }
        }
        public static string Pattern_saas_admin_user_docs_tips_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_user_docs_tips_v10");
            }
        }
        public static string Pattern_saas_admin_user_docs_tips_v10_item_3rdparty
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_user_docs_tips_v10_item_3rdparty");
            }
        }
        public static string Pattern_saas_admin_user_docs_tips_v10_item_3rdparty_hdr
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_user_docs_tips_v10_item_3rdparty_hdr");
            }
        }
        public static string Pattern_saas_admin_user_docs_tips_v10_item_apps
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_user_docs_tips_v10_item_apps");
            }
        }
        public static string Pattern_saas_admin_user_docs_tips_v10_item_apps_hdr
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_user_docs_tips_v10_item_apps_hdr");
            }
        }
        public static string Pattern_saas_admin_user_docs_tips_v10_item_attach
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_user_docs_tips_v10_item_attach");
            }
        }
        public static string Pattern_saas_admin_user_docs_tips_v10_item_attach_hdr
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_user_docs_tips_v10_item_attach_hdr");
            }
        }
        public static string Pattern_saas_admin_user_docs_tips_v10_item_coediting
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_user_docs_tips_v10_item_coediting");
            }
        }
        public static string Pattern_saas_admin_user_docs_tips_v10_item_coediting_hdr
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_user_docs_tips_v10_item_coediting_hdr");
            }
        }
        public static string Pattern_saas_admin_user_docs_tips_v10_item_formatting
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_user_docs_tips_v10_item_formatting");
            }
        }
        public static string Pattern_saas_admin_user_docs_tips_v10_item_formatting_hdr
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_user_docs_tips_v10_item_formatting_hdr");
            }
        }
        public static string Pattern_saas_admin_user_docs_tips_v10_item_review
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_user_docs_tips_v10_item_review");
            }
        }
        public static string Pattern_saas_admin_user_docs_tips_v10_item_review_hdr
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_user_docs_tips_v10_item_review_hdr");
            }
        }
        public static string Pattern_saas_admin_user_docs_tips_v10_item_share
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_user_docs_tips_v10_item_share");
            }
        }
        public static string Pattern_saas_admin_user_docs_tips_v10_item_share_hdr
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_user_docs_tips_v10_item_share_hdr");
            }
        }
        public static string Pattern_saas_admin_user_mail_tips
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_user_mail_tips");
            }
        }
        public static string Pattern_saas_admin_user_powerful_tips
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_user_powerful_tips");
            }
        }
        public static string Pattern_saas_admin_user_team_tips
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_user_team_tips");
            }
        }
        public static string Pattern_saas_admin_welcome
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_welcome");
            }
        }
        public static string Pattern_saas_admin_welcome_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_welcome_v10");
            }
        }
        public static string Pattern_saas_admin_welcome_v10_item_backup
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_welcome_v10_item_backup");
            }
        }
        public static string Pattern_saas_admin_welcome_v10_item_brand
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_welcome_v10_item_brand");
            }
        }
        public static string Pattern_saas_admin_welcome_v10_item_customize
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_welcome_v10_item_customize");
            }
        }
        public static string Pattern_saas_admin_welcome_v10_item_regional
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_welcome_v10_item_regional");
            }
        }
        public static string Pattern_saas_admin_welcome_v10_item_security
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_welcome_v10_item_security");
            }
        }
        public static string Pattern_saas_admin_welcome_v10_item_telephony
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_welcome_v10_item_telephony");
            }
        }
        public static string Pattern_saas_admin_welcome_v10_item_usertrack
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_welcome_v10_item_usertrack");
            }
        }
        public static string Pattern_saas_admin_without_activity
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_without_activity");
            }
        }
        public static string Pattern_saas_admin_without_activity_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_admin_without_activity_v10");
            }
        }
        public static string Pattern_saas_guest_activation
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_guest_activation");
            }
        }
        public static string Pattern_saas_guest_activation_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_guest_activation_v10");
            }
        }
        public static string Pattern_saas_guest_welcome
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_guest_welcome");
            }
        }
        public static string Pattern_saas_guest_welcome_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_guest_welcome_v10");
            }
        }
        public static string Pattern_saas_user_activation
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_user_activation");
            }
        }
        public static string Pattern_saas_user_activation_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_user_activation_v10");
            }
        }
        public static string Pattern_saas_user_organize_workplace
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_user_organize_workplace");
            }
        }
        public static string Pattern_saas_user_welcome
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_user_welcome");
            }
        }
        public static string Pattern_saas_user_welcome_v10
        {
            get
            {
                return JsonResourceManager.GetString("pattern_saas_user_welcome_v10");
            }
        }
        public static string Pattern_self_profile_updated
        {
            get
            {
                return JsonResourceManager.GetString("pattern_self_profile_updated");
            }
        }
        public static string Pattern_send_whats_new
        {
            get
            {
                return JsonResourceManager.GetString("pattern_send_whats_new");
            }
        }
        public static string Pattern_smtp_test
        {
            get
            {
                return JsonResourceManager.GetString("pattern_smtp_test");
            }
        }
        public static string Pattern_tariff_warning_trial
        {
            get
            {
                return JsonResourceManager.GetString("pattern_tariff_warning_trial");
            }
        }
        public static string Pattern_tariff_warning_trial_enterprise
        {
            get
            {
                return JsonResourceManager.GetString("pattern_tariff_warning_trial_enterprise");
            }
        }
        public static string Pattern_tariff_warning_trial2
        {
            get
            {
                return JsonResourceManager.GetString("pattern_tariff_warning_trial2");
            }
        }
        public static string Pattern_tariff_warning_trial2_enterprise
        {
            get
            {
                return JsonResourceManager.GetString("pattern_tariff_warning_trial2_enterprise");
            }
        }
        public static string Pattern_tariff_warning_trial3
        {
            get
            {
                return JsonResourceManager.GetString("pattern_tariff_warning_trial3");
            }
        }
        public static string Pattern_tariff_warning_trial4
        {
            get
            {
                return JsonResourceManager.GetString("pattern_tariff_warning_trial4");
            }
        }
        public static string Pattern_user_has_join
        {
            get
            {
                return JsonResourceManager.GetString("pattern_user_has_join");
            }
        }
        public static string Pattern_user_ldap_activation
        {
            get
            {
                return JsonResourceManager.GetString("pattern_user_ldap_activation");
            }
        }
        public static string Pattern_you_added_after_invite
        {
            get
            {
                return JsonResourceManager.GetString("pattern_you_added_after_invite");
            }
        }
        public static string Pattern_you_added_after_invite_enterprise
        {
            get
            {
                return JsonResourceManager.GetString("pattern_you_added_after_invite_enterprise");
            }
        }
        public static string Pattern_you_added_after_invite_freecloud
        {
            get
            {
                return JsonResourceManager.GetString("pattern_you_added_after_invite_freecloud");
            }
        }
        public static string Pattern_you_added_after_invite_whitelabel
        {
            get
            {
                return JsonResourceManager.GetString("pattern_you_added_after_invite_whitelabel");
            }
        }
        public static string Pattern_you_added_like_guest
        {
            get
            {
                return JsonResourceManager.GetString("pattern_you_added_like_guest");
            }
        }
        public static string Pattern_you_added_like_guest_enterprise
        {
            get
            {
                return JsonResourceManager.GetString("pattern_you_added_like_guest_enterprise");
            }
        }
        public static string Pattern_you_added_like_guest_freecloud
        {
            get
            {
                return JsonResourceManager.GetString("pattern_you_added_like_guest_freecloud");
            }
        }
        public static string Pattern_you_added_like_guest_whitelabel
        {
            get
            {
                return JsonResourceManager.GetString("pattern_you_added_like_guest_whitelabel");
            }
        }
        public static string Subject_activate
        {
            get
            {
                return JsonResourceManager.GetString("subject_activate");
            }
        }
        public static string Subject_activate_email
        {
            get
            {
                return JsonResourceManager.GetString("subject_activate_email");
            }
        }
        public static string Subject_activate_guest
        {
            get
            {
                return JsonResourceManager.GetString("subject_activate_guest");
            }
        }
        public static string Subject_activate_personal
        {
            get
            {
                return JsonResourceManager.GetString("subject_activate_personal");
            }
        }
        public static string Subject_admin_sms_balance
        {
            get
            {
                return JsonResourceManager.GetString("subject_admin_sms_balance");
            }
        }
        public static string Subject_admin_voip_blocked
        {
            get
            {
                return JsonResourceManager.GetString("subject_admin_voip_blocked");
            }
        }
        public static string Subject_admin_voip_warning
        {
            get
            {
                return JsonResourceManager.GetString("subject_admin_voip_warning");
            }
        }
        public static string Subject_admin_welcome
        {
            get
            {
                return JsonResourceManager.GetString("subject_admin_welcome");
            }
        }
        public static string Subject_admin_welcome_enterprise
        {
            get
            {
                return JsonResourceManager.GetString("subject_admin_welcome_enterprise");
            }
        }
        public static string Subject_after_creation1
        {
            get
            {
                return JsonResourceManager.GetString("subject_after_creation1");
            }
        }
        public static string Subject_after_creation1_enterprise
        {
            get
            {
                return JsonResourceManager.GetString("subject_after_creation1_enterprise");
            }
        }
        public static string Subject_after_creation2
        {
            get
            {
                return JsonResourceManager.GetString("subject_after_creation2");
            }
        }
        public static string Subject_after_creation3
        {
            get
            {
                return JsonResourceManager.GetString("subject_after_creation3");
            }
        }
        public static string Subject_after_creation30_freecloud
        {
            get
            {
                return JsonResourceManager.GetString("subject_after_creation30_freecloud");
            }
        }
        public static string Subject_after_creation4
        {
            get
            {
                return JsonResourceManager.GetString("subject_after_creation4");
            }
        }
        public static string Subject_after_creation4_enterprise
        {
            get
            {
                return JsonResourceManager.GetString("subject_after_creation4_enterprise");
            }
        }
        public static string Subject_after_creation5
        {
            get
            {
                return JsonResourceManager.GetString("subject_after_creation5");
            }
        }
        public static string Subject_after_creation6
        {
            get
            {
                return JsonResourceManager.GetString("subject_after_creation6");
            }
        }
        public static string Subject_after_creation7
        {
            get
            {
                return JsonResourceManager.GetString("subject_after_creation7");
            }
        }
        public static string Subject_after_creation8_enterprise
        {
            get
            {
                return JsonResourceManager.GetString("subject_after_creation8_enterprise");
            }
        }
        public static string Subject_after_payment1
        {
            get
            {
                return JsonResourceManager.GetString("subject_after_payment1");
            }
        }
        public static string Subject_after_registration_personal1
        {
            get
            {
                return JsonResourceManager.GetString("subject_after_registration_personal1");
            }
        }
        public static string Subject_after_registration_personal14
        {
            get
            {
                return JsonResourceManager.GetString("subject_after_registration_personal14");
            }
        }
        public static string Subject_after_registration_personal21
        {
            get
            {
                return JsonResourceManager.GetString("subject_after_registration_personal21");
            }
        }
        public static string Subject_after_registration_personal7
        {
            get
            {
                return JsonResourceManager.GetString("subject_after_registration_personal7");
            }
        }
        public static string Subject_backup_created
        {
            get
            {
                return JsonResourceManager.GetString("subject_backup_created");
            }
        }
        public static string Subject_change_email
        {
            get
            {
                return JsonResourceManager.GetString("subject_change_email");
            }
        }
        public static string Subject_change_email_personal
        {
            get
            {
                return JsonResourceManager.GetString("subject_change_email_personal");
            }
        }
        public static string Subject_change_password
        {
            get
            {
                return JsonResourceManager.GetString("subject_change_password");
            }
        }
        public static string Subject_change_password_personal
        {
            get
            {
                return JsonResourceManager.GetString("subject_change_password_personal");
            }
        }
        public static string Subject_change_phone
        {
            get
            {
                return JsonResourceManager.GetString("subject_change_phone");
            }
        }
        public static string Subject_change_pwd
        {
            get
            {
                return JsonResourceManager.GetString("subject_change_pwd");
            }
        }
        public static string Subject_change_tfa
        {
            get
            {
                return JsonResourceManager.GetString("subject_change_tfa");
            }
        }
        public static string Subject_confirm_owner_change
        {
            get
            {
                return JsonResourceManager.GetString("subject_confirm_owner_change");
            }
        }
        public static string Subject_confirmation_personal
        {
            get
            {
                return JsonResourceManager.GetString("subject_confirmation_personal");
            }
        }
        public static string Subject_congratulations
        {
            get
            {
                return JsonResourceManager.GetString("subject_congratulations");
            }
        }
        public static string Subject_dns_change
        {
            get
            {
                return JsonResourceManager.GetString("subject_dns_change");
            }
        }
        public static string Subject_enterprise_admin_activation
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_activation");
            }
        }
        public static string Subject_enterprise_admin_activation_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_activation_v10");
            }
        }
        public static string Subject_enterprise_admin_customize_portal
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_customize_portal");
            }
        }
        public static string Subject_enterprise_admin_customize_portal_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_customize_portal_v10");
            }
        }
        public static string Subject_enterprise_admin_invite_teammates
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_invite_teammates");
            }
        }
        public static string Subject_enterprise_admin_invite_teammates_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_invite_teammates_v10");
            }
        }
        public static string Subject_enterprise_admin_payment_warning
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_payment_warning");
            }
        }
        public static string Subject_enterprise_admin_payment_warning_before7
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_payment_warning_before7");
            }
        }
        public static string Subject_enterprise_admin_payment_warning_before7_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_payment_warning_before7_v10");
            }
        }
        public static string Subject_enterprise_admin_payment_warning_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_payment_warning_v10");
            }
        }
        public static string Subject_enterprise_admin_trial_warning
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_trial_warning");
            }
        }
        public static string Subject_enterprise_admin_trial_warning_before7
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_trial_warning_before7");
            }
        }
        public static string Subject_enterprise_admin_trial_warning_before7_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_trial_warning_before7_v10");
            }
        }
        public static string Subject_enterprise_admin_trial_warning_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_trial_warning_v10");
            }
        }
        public static string Subject_enterprise_admin_user_apps_tips_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_user_apps_tips_v10");
            }
        }
        public static string Subject_enterprise_admin_user_crm_tips
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_user_crm_tips");
            }
        }
        public static string Subject_enterprise_admin_user_docs_tips
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_user_docs_tips");
            }
        }
        public static string Subject_enterprise_admin_user_docs_tips_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_user_docs_tips_v10");
            }
        }
        public static string Subject_enterprise_admin_user_mail_tips
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_user_mail_tips");
            }
        }
        public static string Subject_enterprise_admin_user_powerful_tips
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_user_powerful_tips");
            }
        }
        public static string Subject_enterprise_admin_user_team_tips
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_user_team_tips");
            }
        }
        public static string Subject_enterprise_admin_welcome
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_welcome");
            }
        }
        public static string Subject_enterprise_admin_welcome_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_welcome_v10");
            }
        }
        public static string Subject_enterprise_admin_without_activity
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_without_activity");
            }
        }
        public static string Subject_enterprise_admin_without_activity_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_admin_without_activity_v10");
            }
        }
        public static string Subject_enterprise_guest_activation
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_guest_activation");
            }
        }
        public static string Subject_enterprise_guest_activation_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_guest_activation_v10");
            }
        }
        public static string Subject_enterprise_guest_welcome
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_guest_welcome");
            }
        }
        public static string Subject_enterprise_guest_welcome_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_guest_welcome_v10");
            }
        }
        public static string Subject_enterprise_user_activation
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_user_activation");
            }
        }
        public static string Subject_enterprise_user_activation_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_user_activation_v10");
            }
        }
        public static string Subject_enterprise_user_organize_workplace
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_user_organize_workplace");
            }
        }
        public static string Subject_enterprise_user_welcome
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_user_welcome");
            }
        }
        public static string Subject_enterprise_user_welcome_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_user_welcome_v10");
            }
        }
        public static string Subject_enterprise_whitelabel_admin_activation
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_whitelabel_admin_activation");
            }
        }
        public static string Subject_enterprise_whitelabel_admin_activation_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_whitelabel_admin_activation_v10");
            }
        }
        public static string Subject_enterprise_whitelabel_admin_customize_portal_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_whitelabel_admin_customize_portal_v10");
            }
        }
        public static string Subject_enterprise_whitelabel_admin_payment_warning
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_whitelabel_admin_payment_warning");
            }
        }
        public static string Subject_enterprise_whitelabel_admin_payment_warning_before7
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_whitelabel_admin_payment_warning_before7");
            }
        }
        public static string Subject_enterprise_whitelabel_admin_payment_warning_before7_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_whitelabel_admin_payment_warning_before7_v10");
            }
        }
        public static string Subject_enterprise_whitelabel_admin_payment_warning_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_whitelabel_admin_payment_warning_v10");
            }
        }
        public static string Subject_enterprise_whitelabel_admin_welcome
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_whitelabel_admin_welcome");
            }
        }
        public static string Subject_enterprise_whitelabel_admin_welcome_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_whitelabel_admin_welcome_v10");
            }
        }
        public static string Subject_enterprise_whitelabel_guest_activation
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_whitelabel_guest_activation");
            }
        }
        public static string Subject_enterprise_whitelabel_guest_activation_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_whitelabel_guest_activation_v10");
            }
        }
        public static string Subject_enterprise_whitelabel_guest_welcome
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_whitelabel_guest_welcome");
            }
        }
        public static string Subject_enterprise_whitelabel_guest_welcome_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_whitelabel_guest_welcome_v10");
            }
        }
        public static string Subject_enterprise_whitelabel_user_activation
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_whitelabel_user_activation");
            }
        }
        public static string Subject_enterprise_whitelabel_user_activation_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_whitelabel_user_activation_v10");
            }
        }
        public static string Subject_enterprise_whitelabel_user_welcome
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_whitelabel_user_welcome");
            }
        }
        public static string Subject_enterprise_whitelabel_user_welcome_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_enterprise_whitelabel_user_welcome_v10");
            }
        }
        public static string Subject_for_admin_notify
        {
            get
            {
                return JsonResourceManager.GetString("subject_for_admin_notify");
            }
        }
        public static string Subject_freecloud_admin_activation
        {
            get
            {
                return JsonResourceManager.GetString("subject_freecloud_admin_activation");
            }
        }
        public static string Subject_freecloud_guest_activation
        {
            get
            {
                return JsonResourceManager.GetString("subject_freecloud_guest_activation");
            }
        }
        public static string Subject_freecloud_guest_welcome
        {
            get
            {
                return JsonResourceManager.GetString("subject_freecloud_guest_welcome");
            }
        }
        public static string Subject_freecloud_user_activation
        {
            get
            {
                return JsonResourceManager.GetString("subject_freecloud_user_activation");
            }
        }
        public static string Subject_freecloud_user_welcome
        {
            get
            {
                return JsonResourceManager.GetString("subject_freecloud_user_welcome");
            }
        }
        public static string Subject_hosted_admin_activation
        {
            get
            {
                return JsonResourceManager.GetString("subject_hosted_admin_activation");
            }
        }
        public static string Subject_hosted_admin_invite_teammates
        {
            get
            {
                return JsonResourceManager.GetString("subject_hosted_admin_invite_teammates");
            }
        }
        public static string Subject_hosted_admin_welcome
        {
            get
            {
                return JsonResourceManager.GetString("subject_hosted_admin_welcome");
            }
        }
        public static string Subject_hosted_guest_activation
        {
            get
            {
                return JsonResourceManager.GetString("subject_hosted_guest_activation");
            }
        }
        public static string Subject_hosted_guest_welcome
        {
            get
            {
                return JsonResourceManager.GetString("subject_hosted_guest_welcome");
            }
        }
        public static string Subject_hosted_user_activation
        {
            get
            {
                return JsonResourceManager.GetString("subject_hosted_user_activation");
            }
        }
        public static string Subject_hosted_user_welcome
        {
            get
            {
                return JsonResourceManager.GetString("subject_hosted_user_welcome");
            }
        }
        public static string Subject_hosted_whitelabel_admin_activation
        {
            get
            {
                return JsonResourceManager.GetString("subject_hosted_whitelabel_admin_activation");
            }
        }
        public static string Subject_hosted_whitelabel_admin_welcome
        {
            get
            {
                return JsonResourceManager.GetString("subject_hosted_whitelabel_admin_welcome");
            }
        }
        public static string Subject_hosted_whitelabel_guest_activation
        {
            get
            {
                return JsonResourceManager.GetString("subject_hosted_whitelabel_guest_activation");
            }
        }
        public static string Subject_hosted_whitelabel_guest_welcome
        {
            get
            {
                return JsonResourceManager.GetString("subject_hosted_whitelabel_guest_welcome");
            }
        }
        public static string Subject_hosted_whitelabel_user_activation
        {
            get
            {
                return JsonResourceManager.GetString("subject_hosted_whitelabel_user_activation");
            }
        }
        public static string Subject_hosted_whitelabel_user_welcome
        {
            get
            {
                return JsonResourceManager.GetString("subject_hosted_whitelabel_user_welcome");
            }
        }
        public static string Subject_invite
        {
            get
            {
                return JsonResourceManager.GetString("subject_invite");
            }
        }
        public static string Subject_join
        {
            get
            {
                return JsonResourceManager.GetString("subject_join");
            }
        }
        public static string Subject_mailbox_created
        {
            get
            {
                return JsonResourceManager.GetString("subject_mailbox_created");
            }
        }
        public static string Subject_mailbox_password_changed
        {
            get
            {
                return JsonResourceManager.GetString("subject_mailbox_password_changed");
            }
        }
        public static string Subject_migration_error
        {
            get
            {
                return JsonResourceManager.GetString("subject_migration_error");
            }
        }
        public static string Subject_migration_start
        {
            get
            {
                return JsonResourceManager.GetString("subject_migration_start");
            }
        }
        public static string Subject_migration_success
        {
            get
            {
                return JsonResourceManager.GetString("subject_migration_success");
            }
        }
        public static string Subject_opensource_admin_activation
        {
            get
            {
                return JsonResourceManager.GetString("subject_opensource_admin_activation");
            }
        }
        public static string Subject_opensource_admin_docs_tips
        {
            get
            {
                return JsonResourceManager.GetString("subject_opensource_admin_docs_tips");
            }
        }
        public static string Subject_opensource_admin_security_tips
        {
            get
            {
                return JsonResourceManager.GetString("subject_opensource_admin_security_tips");
            }
        }
        public static string Subject_payment_warning
        {
            get
            {
                return JsonResourceManager.GetString("subject_payment_warning");
            }
        }
        public static string Subject_payment_warning_after3
        {
            get
            {
                return JsonResourceManager.GetString("subject_payment_warning_after3");
            }
        }
        public static string Subject_payment_warning_before7
        {
            get
            {
                return JsonResourceManager.GetString("subject_payment_warning_before7");
            }
        }
        public static string Subject_payment_warning_delaydue
        {
            get
            {
                return JsonResourceManager.GetString("subject_payment_warning_delaydue");
            }
        }
        public static string Subject_personal_activate
        {
            get
            {
                return JsonResourceManager.GetString("subject_personal_activate");
            }
        }
        public static string Subject_personal_after_registration1
        {
            get
            {
                return JsonResourceManager.GetString("subject_personal_after_registration1");
            }
        }
        public static string Subject_personal_after_registration14
        {
            get
            {
                return JsonResourceManager.GetString("subject_personal_after_registration14");
            }
        }
        public static string Subject_personal_after_registration21
        {
            get
            {
                return JsonResourceManager.GetString("subject_personal_after_registration21");
            }
        }
        public static string Subject_personal_after_registration28
        {
            get
            {
                return JsonResourceManager.GetString("subject_personal_after_registration28");
            }
        }
        public static string Subject_personal_after_registration7
        {
            get
            {
                return JsonResourceManager.GetString("subject_personal_after_registration7");
            }
        }
        public static string Subject_personal_change_email
        {
            get
            {
                return JsonResourceManager.GetString("subject_personal_change_email");
            }
        }
        public static string Subject_personal_change_password
        {
            get
            {
                return JsonResourceManager.GetString("subject_personal_change_password");
            }
        }
        public static string Subject_personal_confirmation
        {
            get
            {
                return JsonResourceManager.GetString("subject_personal_confirmation");
            }
        }
        public static string Subject_personal_profile_delete
        {
            get
            {
                return JsonResourceManager.GetString("subject_personal_profile_delete");
            }
        }
        public static string Subject_personal_r7_after_registration1
        {
            get
            {
                return JsonResourceManager.GetString("subject_personal_r7_after_registration1");
            }
        }
        public static string Subject_personal_r7_after_registration7
        {
            get
            {
                return JsonResourceManager.GetString("subject_personal_r7_after_registration7");
            }
        }
        public static string Subject_personal_r7_change_email
        {
            get
            {
                return JsonResourceManager.GetString("subject_personal_r7_change_email");
            }
        }
        public static string Subject_personal_r7_change_password
        {
            get
            {
                return JsonResourceManager.GetString("subject_personal_r7_change_password");
            }
        }
        public static string Subject_personal_r7_confirmation
        {
            get
            {
                return JsonResourceManager.GetString("subject_personal_r7_confirmation");
            }
        }
        public static string Subject_personal_r7_profile_delete
        {
            get
            {
                return JsonResourceManager.GetString("subject_personal_r7_profile_delete");
            }
        }
        public static string Subject_portal_deactivate
        {
            get
            {
                return JsonResourceManager.GetString("subject_portal_deactivate");
            }
        }
        public static string Subject_portal_delete
        {
            get
            {
                return JsonResourceManager.GetString("subject_portal_delete");
            }
        }
        public static string Subject_portal_delete_success
        {
            get
            {
                return JsonResourceManager.GetString("subject_portal_delete_success");
            }
        }
        public static string Subject_portal_delete_success_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_portal_delete_success_v10");
            }
        }
        public static string Subject_portal_rename
        {
            get
            {
                return JsonResourceManager.GetString("subject_portal_rename");
            }
        }
        public static string Subject_profile_delete
        {
            get
            {
                return JsonResourceManager.GetString("subject_profile_delete");
            }
        }
        public static string Subject_profile_has_deleted_itself
        {
            get
            {
                return JsonResourceManager.GetString("subject_profile_has_deleted_itself");
            }
        }
        public static string Subject_profile_updated
        {
            get
            {
                return JsonResourceManager.GetString("subject_profile_updated");
            }
        }
        public static string Subject_pwd_reminder
        {
            get
            {
                return JsonResourceManager.GetString("subject_pwd_reminder");
            }
        }
        public static string Subject_reassigns_completed
        {
            get
            {
                return JsonResourceManager.GetString("subject_reassigns_completed");
            }
        }
        public static string Subject_reassigns_failed
        {
            get
            {
                return JsonResourceManager.GetString("subject_reassigns_failed");
            }
        }
        public static string Subject_remove_user_data_completed
        {
            get
            {
                return JsonResourceManager.GetString("subject_remove_user_data_completed");
            }
        }
        public static string Subject_remove_user_data_failed
        {
            get
            {
                return JsonResourceManager.GetString("subject_remove_user_data_failed");
            }
        }
        public static string Subject_request_license
        {
            get
            {
                return JsonResourceManager.GetString("subject_request_license");
            }
        }
        public static string Subject_request_tariff
        {
            get
            {
                return JsonResourceManager.GetString("subject_request_tariff");
            }
        }
        public static string Subject_restore_completed
        {
            get
            {
                return JsonResourceManager.GetString("subject_restore_completed");
            }
        }
        public static string Subject_restore_started
        {
            get
            {
                return JsonResourceManager.GetString("subject_restore_started");
            }
        }
        public static string Subject_saas_admin_activation
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_activation");
            }
        }
        public static string Subject_saas_admin_activation_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_activation_v10");
            }
        }
        public static string Subject_saas_admin_invite_teammates
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_invite_teammates");
            }
        }
        public static string Subject_saas_admin_invite_teammates_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_invite_teammates_v10");
            }
        }
        public static string Subject_saas_admin_payment_after_monthly_subscriptions
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_payment_after_monthly_subscriptions");
            }
        }
        public static string Subject_saas_admin_payment_after_monthly_subscriptions_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_payment_after_monthly_subscriptions_v10");
            }
        }
        public static string Subject_saas_admin_payment_warning
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_payment_warning");
            }
        }
        public static string Subject_saas_admin_payment_warning_after1_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_payment_warning_after1_v10");
            }
        }
        public static string Subject_saas_admin_payment_warning_after3
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_payment_warning_after3");
            }
        }
        public static string Subject_saas_admin_payment_warning_before5_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_payment_warning_before5_v10");
            }
        }
        public static string Subject_saas_admin_payment_warning_before7
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_payment_warning_before7");
            }
        }
        public static string Subject_saas_admin_payment_warning_delaydue
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_payment_warning_delaydue");
            }
        }
        public static string Subject_saas_admin_trial_warning
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_trial_warning");
            }
        }
        public static string Subject_saas_admin_trial_warning_after_half_year
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_trial_warning_after_half_year");
            }
        }
        public static string Subject_saas_admin_trial_warning_after_half_year_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_trial_warning_after_half_year_v10");
            }
        }
        public static string Subject_saas_admin_trial_warning_after30
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_trial_warning_after30");
            }
        }
        public static string Subject_saas_admin_trial_warning_after30_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_trial_warning_after30_v10");
            }
        }
        public static string Subject_saas_admin_trial_warning_after5
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_trial_warning_after5");
            }
        }
        public static string Subject_saas_admin_trial_warning_after5_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_trial_warning_after5_v10");
            }
        }
        public static string Subject_saas_admin_trial_warning_before5
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_trial_warning_before5");
            }
        }
        public static string Subject_saas_admin_trial_warning_before5_coupon
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_trial_warning_before5_coupon");
            }
        }
        public static string Subject_saas_admin_trial_warning_before5_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_trial_warning_before5_v10");
            }
        }
        public static string Subject_saas_admin_trial_warning_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_trial_warning_v10");
            }
        }
        public static string Subject_saas_admin_user_apps_tips_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_user_apps_tips_v10");
            }
        }
        public static string Subject_saas_admin_user_comfort_tips_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_user_comfort_tips_v10");
            }
        }
        public static string Subject_saas_admin_user_crm_tips
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_user_crm_tips");
            }
        }
        public static string Subject_saas_admin_user_docs_tips
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_user_docs_tips");
            }
        }
        public static string Subject_saas_admin_user_docs_tips_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_user_docs_tips_v10");
            }
        }
        public static string Subject_saas_admin_user_mail_tips
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_user_mail_tips");
            }
        }
        public static string Subject_saas_admin_user_powerful_tips
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_user_powerful_tips");
            }
        }
        public static string Subject_saas_admin_user_team_tips
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_user_team_tips");
            }
        }
        public static string Subject_saas_admin_welcome
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_welcome");
            }
        }
        public static string Subject_saas_admin_welcome_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_welcome_v10");
            }
        }
        public static string Subject_saas_admin_without_activity
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_without_activity");
            }
        }
        public static string Subject_saas_admin_without_activity_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_admin_without_activity_v10");
            }
        }
        public static string Subject_saas_guest_activation
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_guest_activation");
            }
        }
        public static string Subject_saas_guest_activation_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_guest_activation_v10");
            }
        }
        public static string Subject_saas_guest_welcome
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_guest_welcome");
            }
        }
        public static string Subject_saas_guest_welcome_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_guest_welcome_v10");
            }
        }
        public static string Subject_saas_user_activation
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_user_activation");
            }
        }
        public static string Subject_saas_user_activation_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_user_activation_v10");
            }
        }
        public static string Subject_saas_user_organize_workplace
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_user_organize_workplace");
            }
        }
        public static string Subject_saas_user_welcome
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_user_welcome");
            }
        }
        public static string Subject_saas_user_welcome_v10
        {
            get
            {
                return JsonResourceManager.GetString("subject_saas_user_welcome_v10");
            }
        }
        public static string Subject_self_profile_updated
        {
            get
            {
                return JsonResourceManager.GetString("subject_self_profile_updated");
            }
        }
        public static string Subject_send_whats_new
        {
            get
            {
                return JsonResourceManager.GetString("subject_send_whats_new");
            }
        }
        public static string Subject_smtp_test
        {
            get
            {
                return JsonResourceManager.GetString("subject_smtp_test");
            }
        }
        public static string Subject_tariff_warning_trial
        {
            get
            {
                return JsonResourceManager.GetString("subject_tariff_warning_trial");
            }
        }
        public static string Subject_tariff_warning_trial_enterprise
        {
            get
            {
                return JsonResourceManager.GetString("subject_tariff_warning_trial_enterprise");
            }
        }
        public static string Subject_tariff_warning_trial2
        {
            get
            {
                return JsonResourceManager.GetString("subject_tariff_warning_trial2");
            }
        }
        public static string Subject_tariff_warning_trial2_enterprise
        {
            get
            {
                return JsonResourceManager.GetString("subject_tariff_warning_trial2_enterprise");
            }
        }
        public static string Subject_tariff_warning_trial3
        {
            get
            {
                return JsonResourceManager.GetString("subject_tariff_warning_trial3");
            }
        }
        public static string Subject_tariff_warning_trial4
        {
            get
            {
                return JsonResourceManager.GetString("subject_tariff_warning_trial4");
            }
        }
        public static string Subject_user_has_join
        {
            get
            {
                return JsonResourceManager.GetString("subject_user_has_join");
            }
        }
        public static string Subject_user_ldap_activation
        {
            get
            {
                return JsonResourceManager.GetString("subject_user_ldap_activation");
            }
        }
        public static string Subject_you_added_after_invite
        {
            get
            {
                return JsonResourceManager.GetString("subject_you_added_after_invite");
            }
        }
        public static string Subject_you_added_after_invite_freecloud
        {
            get
            {
                return JsonResourceManager.GetString("subject_you_added_after_invite_freecloud");
            }
        }
        public static string Subject_you_added_like_guest
        {
            get
            {
                return JsonResourceManager.GetString("subject_you_added_like_guest");
            }
        }
    }
}

