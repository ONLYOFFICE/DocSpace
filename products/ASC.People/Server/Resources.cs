using ASC.Core.Common.Resources;
namespace ASC.People
{
    public class PeopleJSResource
    {
        private static JsonResourceManager JsonResourceManager { get; set; }
        static PeopleJSResource()
        {
            JsonResourceManager = new JsonResourceManager("PeopleJSResource");
        }

        public static string ChooseUser
        {
            get
            {
                return JsonResourceManager.GetString("ChooseUser");
            }
        }
        public static string ErrorMessage_NotImageSupportFormat
        {
            get
            {
                return JsonResourceManager.GetString("ErrorMessage_NotImageSupportFormat");
            }
        }
        public static string ErrorMessage_SaveImageError
        {
            get
            {
                return JsonResourceManager.GetString("ErrorMessage_SaveImageError");
            }
        }
        public static string HelpContentAccessRights
        {
            get
            {
                return JsonResourceManager.GetString("HelpContentAccessRights");
            }
        }
        public static string HelpContentAddMore
        {
            get
            {
                return JsonResourceManager.GetString("HelpContentAddMore");
            }
        }
        public static string HelpContentEditProfile
        {
            get
            {
                return JsonResourceManager.GetString("HelpContentEditProfile");
            }
        }
        public static string HelpContentViewUser
        {
            get
            {
                return JsonResourceManager.GetString("HelpContentViewUser");
            }
        }
        public static string HelpTitleAccessRights
        {
            get
            {
                return JsonResourceManager.GetString("HelpTitleAccessRights");
            }
        }
        public static string HelpTitleAddMore
        {
            get
            {
                return JsonResourceManager.GetString("HelpTitleAddMore");
            }
        }
        public static string HelpTitleEditProfile
        {
            get
            {
                return JsonResourceManager.GetString("HelpTitleEditProfile");
            }
        }
        public static string HelpTitleViewUser
        {
            get
            {
                return JsonResourceManager.GetString("HelpTitleViewUser");
            }
        }
        public static string LblActive
        {
            get
            {
                return JsonResourceManager.GetString("LblActive");
            }
        }
        public static string LblByName
        {
            get
            {
                return JsonResourceManager.GetString("LblByName");
            }
        }
        public static string LblByType
        {
            get
            {
                return JsonResourceManager.GetString("LblByType");
            }
        }
        public static string LblOther
        {
            get
            {
                return JsonResourceManager.GetString("LblOther");
            }
        }
        public static string LblPending
        {
            get
            {
                return JsonResourceManager.GetString("LblPending");
            }
        }
        public static string LblStatus
        {
            get
            {
                return JsonResourceManager.GetString("LblStatus");
            }
        }
        public static string LblTerminated
        {
            get
            {
                return JsonResourceManager.GetString("LblTerminated");
            }
        }
        public static string People
        {
            get
            {
                return JsonResourceManager.GetString("People");
            }
        }
        public static string SelectedCount
        {
            get
            {
                return JsonResourceManager.GetString("SelectedCount");
            }
        }
        public static string SuccessChangeUserStatus
        {
            get
            {
                return JsonResourceManager.GetString("SuccessChangeUserStatus");
            }
        }
        public static string SuccessChangeUserType
        {
            get
            {
                return JsonResourceManager.GetString("SuccessChangeUserType");
            }
        }
        public static string SuccessSendInvitation
        {
            get
            {
                return JsonResourceManager.GetString("SuccessSendInvitation");
            }
        }
        public static string TariffActiveUserLimit
        {
            get
            {
                return JsonResourceManager.GetString("TariffActiveUserLimit");
            }
        }
        public static string TariffActiveUserLimitExcludingGuests
        {
            get
            {
                return JsonResourceManager.GetString("TariffActiveUserLimitExcludingGuests");
            }
        }
    }
    public class PeopleResource
    {
        private static JsonResourceManager JsonResourceManager { get; set; }
        static PeopleResource()
        {
            JsonResourceManager = new JsonResourceManager("PeopleResource");
        }

        public static string AccessRightsSettings
        {
            get
            {
                return JsonResourceManager.GetString("AccessRightsSettings");
            }
        }
        public static string Actions
        {
            get
            {
                return JsonResourceManager.GetString("Actions");
            }
        }
        public static string AddButton
        {
            get
            {
                return JsonResourceManager.GetString("AddButton");
            }
        }
        public static string AddDepartmentDlgTitle
        {
            get
            {
                return JsonResourceManager.GetString("AddDepartmentDlgTitle");
            }
        }
        public static string AddImage
        {
            get
            {
                return JsonResourceManager.GetString("AddImage");
            }
        }
        public static string AddMembers
        {
            get
            {
                return JsonResourceManager.GetString("AddMembers");
            }
        }
        public static string BlockedMessage
        {
            get
            {
                return JsonResourceManager.GetString("BlockedMessage");
            }
        }
        public static string CancelButton
        {
            get
            {
                return JsonResourceManager.GetString("CancelButton");
            }
        }
        public static string ChangeStatus
        {
            get
            {
                return JsonResourceManager.GetString("ChangeStatus");
            }
        }
        public static string ChangeStatusButton
        {
            get
            {
                return JsonResourceManager.GetString("ChangeStatusButton");
            }
        }
        public static string ChangeStatusDialogConstraint
        {
            get
            {
                return JsonResourceManager.GetString("ChangeStatusDialogConstraint");
            }
        }
        public static string ChangeStatusDialogHeader
        {
            get
            {
                return JsonResourceManager.GetString("ChangeStatusDialogHeader");
            }
        }
        public static string ChangeStatusDialogRestriction
        {
            get
            {
                return JsonResourceManager.GetString("ChangeStatusDialogRestriction");
            }
        }
        public static string ChangeStatusDialogToActive
        {
            get
            {
                return JsonResourceManager.GetString("ChangeStatusDialogToActive");
            }
        }
        public static string ChangeStatusDialogToTerminate
        {
            get
            {
                return JsonResourceManager.GetString("ChangeStatusDialogToTerminate");
            }
        }
        public static string ChangeType
        {
            get
            {
                return JsonResourceManager.GetString("ChangeType");
            }
        }
        public static string ChangeTypeDialogConstraint
        {
            get
            {
                return JsonResourceManager.GetString("ChangeTypeDialogConstraint");
            }
        }
        public static string ChangeTypeDialogHeader
        {
            get
            {
                return JsonResourceManager.GetString("ChangeTypeDialogHeader");
            }
        }
        public static string ChangeTypeDialogRestriction
        {
            get
            {
                return JsonResourceManager.GetString("ChangeTypeDialogRestriction");
            }
        }
        public static string ChangeTypeDialogToGuest
        {
            get
            {
                return JsonResourceManager.GetString("ChangeTypeDialogToGuest");
            }
        }
        public static string ChangeTypeDialogToUser
        {
            get
            {
                return JsonResourceManager.GetString("ChangeTypeDialogToUser");
            }
        }
        public static string ChooseUser
        {
            get
            {
                return JsonResourceManager.GetString("ChooseUser");
            }
        }
        public static string ClearButton
        {
            get
            {
                return JsonResourceManager.GetString("ClearButton");
            }
        }
        public static string Confirmation
        {
            get
            {
                return JsonResourceManager.GetString("Confirmation");
            }
        }
        public static string CreateNewProfile
        {
            get
            {
                return JsonResourceManager.GetString("CreateNewProfile");
            }
        }
        public static string DeleteBtnHint
        {
            get
            {
                return JsonResourceManager.GetString("DeleteBtnHint");
            }
        }
        public static string DeleteButton
        {
            get
            {
                return JsonResourceManager.GetString("DeleteButton");
            }
        }
        public static string DeleteProfileAfterReassignment
        {
            get
            {
                return JsonResourceManager.GetString("DeleteProfileAfterReassignment");
            }
        }
        public static string DeleteUserProfiles
        {
            get
            {
                return JsonResourceManager.GetString("DeleteUserProfiles");
            }
        }
        public static string DeleteUsersDataConfirmation
        {
            get
            {
                return JsonResourceManager.GetString("DeleteUsersDataConfirmation");
            }
        }
        public static string DeleteUsersDescription
        {
            get
            {
                return JsonResourceManager.GetString("DeleteUsersDescription");
            }
        }
        public static string DeleteUsersDescriptionText
        {
            get
            {
                return JsonResourceManager.GetString("DeleteUsersDescriptionText");
            }
        }
        public static string DepartmentMaster
        {
            get
            {
                return JsonResourceManager.GetString("DepartmentMaster");
            }
        }
        public static string DeselectAll
        {
            get
            {
                return JsonResourceManager.GetString("DeselectAll");
            }
        }
        public static string DisableUserButton
        {
            get
            {
                return JsonResourceManager.GetString("DisableUserButton");
            }
        }
        public static string DisableUserHelp
        {
            get
            {
                return JsonResourceManager.GetString("DisableUserHelp");
            }
        }
        public static string EditButton
        {
            get
            {
                return JsonResourceManager.GetString("EditButton");
            }
        }
        public static string EditImage
        {
            get
            {
                return JsonResourceManager.GetString("EditImage");
            }
        }
        public static string Email
        {
            get
            {
                return JsonResourceManager.GetString("Email");
            }
        }
        public static string EnableUserButton
        {
            get
            {
                return JsonResourceManager.GetString("EnableUserButton");
            }
        }
        public static string EnableUserHelp
        {
            get
            {
                return JsonResourceManager.GetString("EnableUserHelp");
            }
        }
        public static string ErrorEmptyName
        {
            get
            {
                return JsonResourceManager.GetString("ErrorEmptyName");
            }
        }
        public static string ErrorEmptyUploadFileSelected
        {
            get
            {
                return JsonResourceManager.GetString("ErrorEmptyUploadFileSelected");
            }
        }
        public static string ErrorImageSizetLimit
        {
            get
            {
                return JsonResourceManager.GetString("ErrorImageSizetLimit");
            }
        }
        public static string ErrorImageWeightLimit
        {
            get
            {
                return JsonResourceManager.GetString("ErrorImageWeightLimit");
            }
        }
        public static string ErrorUnknownFileImageType
        {
            get
            {
                return JsonResourceManager.GetString("ErrorUnknownFileImageType");
            }
        }
        public static string ExampleValues
        {
            get
            {
                return JsonResourceManager.GetString("ExampleValues");
            }
        }
        public static string FieldsInFile
        {
            get
            {
                return JsonResourceManager.GetString("FieldsInFile");
            }
        }
        public static string FieldsOnPortal
        {
            get
            {
                return JsonResourceManager.GetString("FieldsOnPortal");
            }
        }
        public static string FirstName
        {
            get
            {
                return JsonResourceManager.GetString("FirstName");
            }
        }
        public static string Hide
        {
            get
            {
                return JsonResourceManager.GetString("Hide");
            }
        }
        public static string HideSelectedUserList
        {
            get
            {
                return JsonResourceManager.GetString("HideSelectedUserList");
            }
        }
        public static string ImportClear
        {
            get
            {
                return JsonResourceManager.GetString("ImportClear");
            }
        }
        public static string ImportColumn
        {
            get
            {
                return JsonResourceManager.GetString("ImportColumn");
            }
        }
        public static string ImportDelimiterDQ
        {
            get
            {
                return JsonResourceManager.GetString("ImportDelimiterDQ");
            }
        }
        public static string ImportDelimiterSQ
        {
            get
            {
                return JsonResourceManager.GetString("ImportDelimiterSQ");
            }
        }
        public static string ImportEncodingASCII
        {
            get
            {
                return JsonResourceManager.GetString("ImportEncodingASCII");
            }
        }
        public static string ImportEncodingCP866
        {
            get
            {
                return JsonResourceManager.GetString("ImportEncodingCP866");
            }
        }
        public static string ImportEncodingKOI8R
        {
            get
            {
                return JsonResourceManager.GetString("ImportEncodingKOI8R");
            }
        }
        public static string ImportEncodingUTF8
        {
            get
            {
                return JsonResourceManager.GetString("ImportEncodingUTF8");
            }
        }
        public static string ImportEncodingWindows1251
        {
            get
            {
                return JsonResourceManager.GetString("ImportEncodingWindows1251");
            }
        }
        public static string ImportFromFile
        {
            get
            {
                return JsonResourceManager.GetString("ImportFromFile");
            }
        }
        public static string ImportFromGoogle
        {
            get
            {
                return JsonResourceManager.GetString("ImportFromGoogle");
            }
        }
        public static string ImportFromYahoo
        {
            get
            {
                return JsonResourceManager.GetString("ImportFromYahoo");
            }
        }
        public static string ImportPeople
        {
            get
            {
                return JsonResourceManager.GetString("ImportPeople");
            }
        }
        public static string ImportSeparatorColon
        {
            get
            {
                return JsonResourceManager.GetString("ImportSeparatorColon");
            }
        }
        public static string ImportSeparatorComma
        {
            get
            {
                return JsonResourceManager.GetString("ImportSeparatorComma");
            }
        }
        public static string ImportSeparatorSemicolon
        {
            get
            {
                return JsonResourceManager.GetString("ImportSeparatorSemicolon");
            }
        }
        public static string ImportSeparatorSpace
        {
            get
            {
                return JsonResourceManager.GetString("ImportSeparatorSpace");
            }
        }
        public static string ImportSeparatorTab
        {
            get
            {
                return JsonResourceManager.GetString("ImportSeparatorTab");
            }
        }
        public static string ImportWizardFirstStep
        {
            get
            {
                return JsonResourceManager.GetString("ImportWizardFirstStep");
            }
        }
        public static string ImportWizardFourthStep
        {
            get
            {
                return JsonResourceManager.GetString("ImportWizardFourthStep");
            }
        }
        public static string ImportWizardSecondStep
        {
            get
            {
                return JsonResourceManager.GetString("ImportWizardSecondStep");
            }
        }
        public static string ImportWizardThirdStep
        {
            get
            {
                return JsonResourceManager.GetString("ImportWizardThirdStep");
            }
        }
        public static string InviteLink
        {
            get
            {
                return JsonResourceManager.GetString("InviteLink");
            }
        }
        public static string LastName
        {
            get
            {
                return JsonResourceManager.GetString("LastName");
            }
        }
        public static string LblActive
        {
            get
            {
                return JsonResourceManager.GetString("LblActive");
            }
        }
        public static string LblByName
        {
            get
            {
                return JsonResourceManager.GetString("LblByName");
            }
        }
        public static string LblByType
        {
            get
            {
                return JsonResourceManager.GetString("LblByType");
            }
        }
        public static string LblCancelButton
        {
            get
            {
                return JsonResourceManager.GetString("LblCancelButton");
            }
        }
        public static string LblChangeEmail
        {
            get
            {
                return JsonResourceManager.GetString("LblChangeEmail");
            }
        }
        public static string LblChangePassword
        {
            get
            {
                return JsonResourceManager.GetString("LblChangePassword");
            }
        }
        public static string LblCreateNew
        {
            get
            {
                return JsonResourceManager.GetString("LblCreateNew");
            }
        }
        public static string LblDeleteProfile
        {
            get
            {
                return JsonResourceManager.GetString("LblDeleteProfile");
            }
        }
        public static string LblEdit
        {
            get
            {
                return JsonResourceManager.GetString("LblEdit");
            }
        }
        public static string LblImportAccounts
        {
            get
            {
                return JsonResourceManager.GetString("LblImportAccounts");
            }
        }
        public static string LblMobilePhone
        {
            get
            {
                return JsonResourceManager.GetString("LblMobilePhone");
            }
        }
        public static string LblOKButton
        {
            get
            {
                return JsonResourceManager.GetString("LblOKButton");
            }
        }
        public static string LblOther
        {
            get
            {
                return JsonResourceManager.GetString("LblOther");
            }
        }
        public static string LblPassword
        {
            get
            {
                return JsonResourceManager.GetString("LblPassword");
            }
        }
        public static string LblPending
        {
            get
            {
                return JsonResourceManager.GetString("LblPending");
            }
        }
        public static string LblReassignData
        {
            get
            {
                return JsonResourceManager.GetString("LblReassignData");
            }
        }
        public static string LblRemoveData
        {
            get
            {
                return JsonResourceManager.GetString("LblRemoveData");
            }
        }
        public static string LblResendInvites
        {
            get
            {
                return JsonResourceManager.GetString("LblResendInvites");
            }
        }
        public static string LblSendActivation
        {
            get
            {
                return JsonResourceManager.GetString("LblSendActivation");
            }
        }
        public static string LblSendEmail
        {
            get
            {
                return JsonResourceManager.GetString("LblSendEmail");
            }
        }
        public static string LblSendMessage
        {
            get
            {
                return JsonResourceManager.GetString("LblSendMessage");
            }
        }
        public static string LblStatus
        {
            get
            {
                return JsonResourceManager.GetString("LblStatus");
            }
        }
        public static string LblSubscriptions
        {
            get
            {
                return JsonResourceManager.GetString("LblSubscriptions");
            }
        }
        public static string LblTerminated
        {
            get
            {
                return JsonResourceManager.GetString("LblTerminated");
            }
        }
        public static string LblTips
        {
            get
            {
                return JsonResourceManager.GetString("LblTips");
            }
        }
        public static string MainEmail
        {
            get
            {
                return JsonResourceManager.GetString("MainEmail");
            }
        }
        public static string Members
        {
            get
            {
                return JsonResourceManager.GetString("Members");
            }
        }
        public static string NoSameValuesOptions
        {
            get
            {
                return JsonResourceManager.GetString("NoSameValuesOptions");
            }
        }
        public static string NotFoundDescription
        {
            get
            {
                return JsonResourceManager.GetString("NotFoundDescription");
            }
        }
        public static string NotFoundTitle
        {
            get
            {
                return JsonResourceManager.GetString("NotFoundTitle");
            }
        }
        public static string NotImport
        {
            get
            {
                return JsonResourceManager.GetString("NotImport");
            }
        }
        public static string OnTop
        {
            get
            {
                return JsonResourceManager.GetString("OnTop");
            }
        }
        public static string ProductAdminOpportunities
        {
            get
            {
                return JsonResourceManager.GetString("ProductAdminOpportunities");
            }
        }
        public static string ProductDescription
        {
            get
            {
                return JsonResourceManager.GetString("ProductDescription");
            }
        }
        public static string ProductName
        {
            get
            {
                return JsonResourceManager.GetString("ProductName");
            }
        }
        public static string ProductUserOpportunities
        {
            get
            {
                return JsonResourceManager.GetString("ProductUserOpportunities");
            }
        }
        public static string ReadAboutNonProfit
        {
            get
            {
                return JsonResourceManager.GetString("ReadAboutNonProfit");
            }
        }
        public static string ReassignAbortButton
        {
            get
            {
                return JsonResourceManager.GetString("ReassignAbortButton");
            }
        }
        public static string ReassignAbortToastrMsg
        {
            get
            {
                return JsonResourceManager.GetString("ReassignAbortToastrMsg");
            }
        }
        public static string ReassignButton
        {
            get
            {
                return JsonResourceManager.GetString("ReassignButton");
            }
        }
        public static string ReassignCrmModule
        {
            get
            {
                return JsonResourceManager.GetString("ReassignCrmModule");
            }
        }
        public static string ReassignDocumentsModule
        {
            get
            {
                return JsonResourceManager.GetString("ReassignDocumentsModule");
            }
        }
        public static string ReassignErrorToastrMsg
        {
            get
            {
                return JsonResourceManager.GetString("ReassignErrorToastrMsg");
            }
        }
        public static string ReassignMailModule
        {
            get
            {
                return JsonResourceManager.GetString("ReassignMailModule");
            }
        }
        public static string ReassignmentData
        {
            get
            {
                return JsonResourceManager.GetString("ReassignmentData");
            }
        }
        public static string ReassignProjectsModule
        {
            get
            {
                return JsonResourceManager.GetString("ReassignProjectsModule");
            }
        }
        public static string ReassignRestartButton
        {
            get
            {
                return JsonResourceManager.GetString("ReassignRestartButton");
            }
        }
        public static string ReassignsProgressNotifyInfo
        {
            get
            {
                return JsonResourceManager.GetString("ReassignsProgressNotifyInfo");
            }
        }
        public static string ReassignsProgressText
        {
            get
            {
                return JsonResourceManager.GetString("ReassignsProgressText");
            }
        }
        public static string ReassignsProgressUserInfo
        {
            get
            {
                return JsonResourceManager.GetString("ReassignsProgressUserInfo");
            }
        }
        public static string ReassignsReadMore
        {
            get
            {
                return JsonResourceManager.GetString("ReassignsReadMore");
            }
        }
        public static string ReassignStatusAborted
        {
            get
            {
                return JsonResourceManager.GetString("ReassignStatusAborted");
            }
        }
        public static string ReassignStatusError
        {
            get
            {
                return JsonResourceManager.GetString("ReassignStatusError");
            }
        }
        public static string ReassignStatusFinished
        {
            get
            {
                return JsonResourceManager.GetString("ReassignStatusFinished");
            }
        }
        public static string ReassignStatusNotStarted
        {
            get
            {
                return JsonResourceManager.GetString("ReassignStatusNotStarted");
            }
        }
        public static string ReassignStatusQueued
        {
            get
            {
                return JsonResourceManager.GetString("ReassignStatusQueued");
            }
        }
        public static string ReassignStatusStarted
        {
            get
            {
                return JsonResourceManager.GetString("ReassignStatusStarted");
            }
        }
        public static string ReassignsToUser
        {
            get
            {
                return JsonResourceManager.GetString("ReassignsToUser");
            }
        }
        public static string ReassignsTransferedListHdr
        {
            get
            {
                return JsonResourceManager.GetString("ReassignsTransferedListHdr");
            }
        }
        public static string ReassignsTransferedListItem1
        {
            get
            {
                return JsonResourceManager.GetString("ReassignsTransferedListItem1");
            }
        }
        public static string ReassignsTransferedListItem2
        {
            get
            {
                return JsonResourceManager.GetString("ReassignsTransferedListItem2");
            }
        }
        public static string ReassignsTransferedListItem3
        {
            get
            {
                return JsonResourceManager.GetString("ReassignsTransferedListItem3");
            }
        }
        public static string ReassignTalkModule
        {
            get
            {
                return JsonResourceManager.GetString("ReassignTalkModule");
            }
        }
        public static string RemovingAbortButton
        {
            get
            {
                return JsonResourceManager.GetString("RemovingAbortButton");
            }
        }
        public static string RemovingAbortToastrMsg
        {
            get
            {
                return JsonResourceManager.GetString("RemovingAbortToastrMsg");
            }
        }
        public static string RemovingData
        {
            get
            {
                return JsonResourceManager.GetString("RemovingData");
            }
        }
        public static string RemovingErrorToastrMsg
        {
            get
            {
                return JsonResourceManager.GetString("RemovingErrorToastrMsg");
            }
        }
        public static string RemovingListHdr
        {
            get
            {
                return JsonResourceManager.GetString("RemovingListHdr");
            }
        }
        public static string RemovingListItem1
        {
            get
            {
                return JsonResourceManager.GetString("RemovingListItem1");
            }
        }
        public static string RemovingListItem2
        {
            get
            {
                return JsonResourceManager.GetString("RemovingListItem2");
            }
        }
        public static string RemovingListItem3
        {
            get
            {
                return JsonResourceManager.GetString("RemovingListItem3");
            }
        }
        public static string RemovingListItem4
        {
            get
            {
                return JsonResourceManager.GetString("RemovingListItem4");
            }
        }
        public static string RemovingProgressUserInfo
        {
            get
            {
                return JsonResourceManager.GetString("RemovingProgressUserInfo");
            }
        }
        public static string RemovingReadMore
        {
            get
            {
                return JsonResourceManager.GetString("RemovingReadMore");
            }
        }
        public static string RemovingRestartButton
        {
            get
            {
                return JsonResourceManager.GetString("RemovingRestartButton");
            }
        }
        public static string RemovingStatusAborted
        {
            get
            {
                return JsonResourceManager.GetString("RemovingStatusAborted");
            }
        }
        public static string RemovingStatusFinished
        {
            get
            {
                return JsonResourceManager.GetString("RemovingStatusFinished");
            }
        }
        public static string RemovingStatusNotStarted
        {
            get
            {
                return JsonResourceManager.GetString("RemovingStatusNotStarted");
            }
        }
        public static string ResendInviteDialogAfterActivation
        {
            get
            {
                return JsonResourceManager.GetString("ResendInviteDialogAfterActivation");
            }
        }
        public static string ResendInviteDialogHeader
        {
            get
            {
                return JsonResourceManager.GetString("ResendInviteDialogHeader");
            }
        }
        public static string ResendInviteDialogTargetUsers
        {
            get
            {
                return JsonResourceManager.GetString("ResendInviteDialogTargetUsers");
            }
        }
        public static string SaveButton
        {
            get
            {
                return JsonResourceManager.GetString("SaveButton");
            }
        }
        public static string SelectAll
        {
            get
            {
                return JsonResourceManager.GetString("SelectAll");
            }
        }
        public static string SelectedCount
        {
            get
            {
                return JsonResourceManager.GetString("SelectedCount");
            }
        }
        public static string Settings
        {
            get
            {
                return JsonResourceManager.GetString("Settings");
            }
        }
        public static string Show
        {
            get
            {
                return JsonResourceManager.GetString("Show");
            }
        }
        public static string ShowOnPage
        {
            get
            {
                return JsonResourceManager.GetString("ShowOnPage");
            }
        }
        public static string ShowSelectedUserList
        {
            get
            {
                return JsonResourceManager.GetString("ShowSelectedUserList");
            }
        }
        public static string SocialProfiles
        {
            get
            {
                return JsonResourceManager.GetString("SocialProfiles");
            }
        }
        public static string SuccessfullyDeleteUserInfoMessage
        {
            get
            {
                return JsonResourceManager.GetString("SuccessfullyDeleteUserInfoMessage");
            }
        }
        public static string SuccessfullySentNotificationDeleteUserInfoMessage
        {
            get
            {
                return JsonResourceManager.GetString("SuccessfullySentNotificationDeleteUserInfoMessage");
            }
        }
        public static string TariffActiveUserLimit
        {
            get
            {
                return JsonResourceManager.GetString("TariffActiveUserLimit");
            }
        }
        public static string TerminateButton
        {
            get
            {
                return JsonResourceManager.GetString("TerminateButton");
            }
        }
        public static string Title
        {
            get
            {
                return JsonResourceManager.GetString("Title");
            }
        }
        public static string TitleThumbnailPhoto
        {
            get
            {
                return JsonResourceManager.GetString("TitleThumbnailPhoto");
            }
        }
        public static string TotalCount
        {
            get
            {
                return JsonResourceManager.GetString("TotalCount");
            }
        }
        public static string WaitingForConfirmation
        {
            get
            {
                return JsonResourceManager.GetString("WaitingForConfirmation");
            }
        }
        public static string WriteButton
        {
            get
            {
                return JsonResourceManager.GetString("WriteButton");
            }
        }
    }
}

