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

namespace ASC.MessagingSystem.Core;

[EnumExtensions]
public enum MessageAction
{
    None = -1,

    #region Login

    LoginSuccess = 1000,
    LoginSuccessViaSocialAccount = 1001,
    LoginSuccessViaSms = 1007,
    LoginSuccessViaApi = 1010,
    LoginSuccessViaSocialApp = 1011,
    LoginSuccessViaApiSms = 1012,
    LoginSuccessViaApiTfa = 1024,
    LoginSuccessViaApiSocialAccount = 1019,
    LoginSuccessViaSSO = 1015,
    LoginSuccesViaTfaApp = 1021,
    LoginFailViaSSO = 1018,
    LoginFailInvalidCombination = 1002,
    LoginFailSocialAccountNotFound = 1003,
    LoginFailDisabledProfile = 1004,
    LoginFail = 1005,
    LoginFailViaSms = 1008,
    LoginFailViaApi = 1013,
    LoginFailViaApiSms = 1014,
    LoginFailViaApiTfa = 1025,
    LoginFailViaApiSocialAccount = 1020,
    LoginFailViaTfaApp = 1022,
    LoginFailIpSecurity = 1009,
    LoginFailBruteForce = 1023,
    LoginFailRecaptcha = 1026,  // last login
    Logout = 1006,

    SessionStarted = 1016,
    SessionCompleted = 1017,

    #endregion

    #region People

    UserCreated = 4000,
    GuestCreated = 4001,
    UserCreatedViaInvite = 4002,
    GuestCreatedViaInvite = 4003,

    UserActivated = 4004,
    GuestActivated = 4005,

    UserUpdated = 4006,
    UserUpdatedMobileNumber = 4029,
    UserUpdatedLanguage = 4007,
    UserAddedAvatar = 4008,
    UserDeletedAvatar = 4009,
    UserUpdatedAvatarThumbnails = 4010,

    UserLinkedSocialAccount = 4011,
    UserUnlinkedSocialAccount = 4012,

    UserConnectedTfaApp = 4032,
    UserDisconnectedTfaApp = 4033,

    UserSentActivationInstructions = 4013,
    UserSentEmailChangeInstructions = 4014,
    UserSentPasswordChangeInstructions = 4015,
    UserSentDeleteInstructions = 4016,

    UserUpdatedEmail = 5047,
    UserUpdatedPassword = 4017,
    UserDeleted = 4018,

    UsersUpdatedType = 4019,
    UsersUpdatedStatus = 4020,
    UsersSentActivationInstructions = 4021,
    UsersDeleted = 4022,
    SentInviteInstructions = 4023,

    UserImported = 4024,
    GuestImported = 4025,

    GroupCreated = 4026,
    GroupUpdated = 4027,
    GroupDeleted = 4028,

    UserDataReassigns = 4030,
    UserDataRemoving = 4031,

    UserLogoutActiveConnections = 4034,
    UserLogoutActiveConnection = 4035,
    UserLogoutActiveConnectionsForUser = 4036,

    #endregion

    #region Documents

    FileCreated = 5000,
    FileRenamed = 5001,
    FileUpdated = 5002,
    UserFileUpdated = 5034,
    FileCreatedVersion = 5003,
    FileDeletedVersion = 5004,
    FileRestoreVersion = 5044,
    FileUpdatedRevisionComment = 5005,
    FileLocked = 5006,
    FileUnlocked = 5007,
    FileUpdatedAccess = 5008,
    FileUpdatedAccessFor = 5068,
    FileSendAccessLink = 5036, // not used
    FileOpenedForChange = 5054,
    FileRemovedFromList = 5058,
    FileExternalLinkAccessUpdated = 5060,

    FileDownloaded = 5009,
    FileDownloadedAs = 5010,
    FileRevisionDownloaded = 5062,

    FileUploaded = 5011,
    FileImported = 5012,

    FileCopied = 5013,
    FileCopiedWithOverwriting = 5014,
    FileMoved = 5015,
    FileMovedWithOverwriting = 5016,
    FileMovedToTrash = 5017,
    FileDeleted = 5018,

    FolderCreated = 5019,
    FolderRenamed = 5020,
    FolderUpdatedAccess = 5021,
    FolderUpdatedAccessFor = 5066,

    FolderCopied = 5022,
    FolderCopiedWithOverwriting = 5023,
    FolderMoved = 5024,
    FolderMovedWithOverwriting = 5025,
    FolderMovedToTrash = 5026,
    FolderDeleted = 5027,
    FolderRemovedFromList = 5059,

    FolderDownloaded = 5057,

    ThirdPartyCreated = 5028,
    ThirdPartyUpdated = 5029,
    ThirdPartyDeleted = 5030,

    DocumentsThirdPartySettingsUpdated = 5031,
    DocumentsOverwritingSettingsUpdated = 5032,
    DocumentsForcesave = 5049,
    DocumentsStoreForcesave = 5048,
    DocumentsUploadingFormatsSettingsUpdated = 5033,
    DocumentsExternalShareSettingsUpdated = 5069,
    DocumentsKeepNewFileNameSettingsUpdated = 5083,

    FileConverted = 5035,

    FileChangeOwner = 5043,

    DocumentSignComplete = 5046,
    DocumentSendToSign = 5045,

    FileMarkedAsFavorite = 5055,
    FileRemovedFromFavorite = 5056,
    FileMarkedAsRead = 5063,
    FileReaded = 5064,

    TrashEmptied = 5061,

    FolderMarkedAsRead = 5065,

    RoomCreated = 5070,
    RoomRenamed = 5071,
    RoomArchived = 5072,
    RoomUnarchived = 5073,
    RoomDeleted = 5074,
    
    RoomUpdateAccessForUser = 5075,
    RoomRemoveUser = 5084,
    RoomCreateUser = 5085,// last
    RoomLinkUpdated = 5082,
    RoomLinkCreated = 5086,
    RoomLinkDeleted = 5087,

    TagCreated = 5076,
    TagsDeleted = 5077,
    AddedRoomTags = 5078,
    DeletedRoomTags = 5079,

    RoomLogoCreated = 5080,
    RoomLogoDeleted = 5081,

    #endregion

    #region Settings

    LanguageSettingsUpdated = 6000,
    TimeZoneSettingsUpdated = 6001,
    DnsSettingsUpdated = 6002,
    TrustedMailDomainSettingsUpdated = 6003,
    PasswordStrengthSettingsUpdated = 6004,
    TwoFactorAuthenticationSettingsUpdated = 6005, // deprecated - use 6036-6038 instead
    AdministratorMessageSettingsUpdated = 6006,
    DefaultStartPageSettingsUpdated = 6007,

    ProductsListUpdated = 6008,

    AdministratorAdded = 6009,
    AdministratorOpenedFullAccess = 6010,
    AdministratorDeleted = 6011,

    UsersOpenedProductAccess = 6012,
    GroupsOpenedProductAccess = 6013,

    ProductAccessOpened = 6014,
    ProductAccessRestricted = 6015, // not used

    ProductAddedAdministrator = 6016,
    ProductDeletedAdministrator = 6017,

    GreetingSettingsUpdated = 6018,
    TeamTemplateChanged = 6019,
    ColorThemeChanged = 6020,

    OwnerSentChangeOwnerInstructions = 6021,
    OwnerUpdated = 6022,

    OwnerSentPortalDeactivationInstructions = 6023,
    OwnerSentPortalDeleteInstructions = 6024,

    PortalDeactivated = 6025,
    PortalDeleted = 6026,

    LoginHistoryReportDownloaded = 6027,
    AuditTrailReportDownloaded = 6028,

    SSOEnabled = 6029,
    SSODisabled = 6030,

    PortalAccessSettingsUpdated = 6031,

    CookieSettingsUpdated = 6032,
    MailServiceSettingsUpdated = 6033,

    CustomNavigationSettingsUpdated = 6034,

    AuditSettingsUpdated = 6035,

    TwoFactorAuthenticationDisabled = 6036,
    TwoFactorAuthenticationEnabledBySms = 6037,
    TwoFactorAuthenticationEnabledByTfaApp = 6038,

    DocumentServiceLocationSetting = 5037,
    AuthorizationKeysSetting = 5038,
    FullTextSearchSetting = 5039,

    StartTransferSetting = 5040,
    StartBackupSetting = 5041,

    LicenseKeyUploaded = 5042,

    StartStorageEncryption = 5050,

    PrivacyRoomEnable = 5051,
    PrivacyRoomDisable = 5052,

    StartStorageDecryption = 5053,

    #endregion

    #region others

    ContactAdminMailSent = 7000,
    RoomInviteLinkUsed = 7001,
    UserCreatedAndAddedToRoom = 7002,
    GuestCreatedAndAddedToRoom = 7003,
    ContactSalesMailSent = 7004,

    #endregion
}
