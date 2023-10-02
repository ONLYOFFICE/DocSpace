﻿import ActionsUploadReactSvgUrl from "PUBLIC_DIR/images/actions.upload.react.svg?url";
import FormReactSvgUrl from "PUBLIC_DIR/images/access.form.react.svg?url";
import FormBlankReactSvgUrl from "PUBLIC_DIR/images/form.blank.react.svg?url";
import FormFileReactSvgUrl from "PUBLIC_DIR/images/form.file.react.svg?url";
import FormGalleryReactSvgUrl from "PUBLIC_DIR/images/form.gallery.react.svg?url";
import ActionsDocumentsReactSvgUrl from "PUBLIC_DIR/images/actions.documents.react.svg?url";
import SpreadsheetReactSvgUrl from "PUBLIC_DIR/images/spreadsheet.react.svg?url";
import ActionsPresentationReactSvgUrl from "PUBLIC_DIR/images/actions.presentation.react.svg?url";
import CatalogFolderReactSvgUrl from "PUBLIC_DIR/images/catalog.folder.react.svg?url";
import PersonAdminReactSvgUrl from "PUBLIC_DIR/images/person.admin.react.svg?url";
import PersonManagerReactSvgUrl from "PUBLIC_DIR/images/person.manager.react.svg?url";
import PersonReactSvgUrl from "PUBLIC_DIR/images/person.react.svg?url";
import PersonUserReactSvgUrl from "PUBLIC_DIR/images/person.user.react.svg?url";
import InviteAgainReactSvgUrl from "PUBLIC_DIR/images/invite.again.react.svg?url";
import PluginMoreReactSvgUrl from "PUBLIC_DIR/images/plugin.more.react.svg?url";
import React from "react";

import { isMobileOnly } from "react-device-detect";
import { inject, observer } from "mobx-react";

import MainButton from "@docspace/components/main-button";
import { withTranslation } from "react-i18next";
import Loaders from "@docspace/common/components/Loaders";
import { encryptionUploadDialog } from "../../../helpers/desktop";
import { useNavigate, useLocation } from "react-router-dom";

import MobileView from "./MobileView";

import { Events, EmployeeType } from "@docspace/common/constants";
import toastr from "@docspace/components/toast/toastr";
import styled, { css } from "styled-components";
import Button from "@docspace/components/button";

import { resendInvitesAgain } from "@docspace/common/api/people";
import { getCorrectFourValuesStyle } from "@docspace/components/utils/rtlUtils";

const StyledButton = styled(Button)`
  font-weight: 700;
  font-size: 16px;
  padding: 0;
  opacity: ${(props) => (props.isDisabled ? 0.6 : 1)};

  background-color: ${({ $currentColorScheme }) =>
    $currentColorScheme.main.accent} !important;
  background: ${({ $currentColorScheme }) => $currentColorScheme.main.accent};
  border: ${({ $currentColorScheme }) => $currentColorScheme.main.accent};

  ${(props) =>
    !props.isDisabled &&
    css`
      :hover {
        background-color: ${({ $currentColorScheme }) =>
          $currentColorScheme.main.accent};
        opacity: 0.85;
        background: ${({ $currentColorScheme }) =>
          $currentColorScheme.main.accent};
        border: ${({ $currentColorScheme }) => $currentColorScheme.main.accent};
      }

      :active {
        background-color: ${({ $currentColorScheme }) =>
          $currentColorScheme.main.accent};
        background: ${({ $currentColorScheme }) =>
          $currentColorScheme.main.accent};
        border: ${({ $currentColorScheme }) => $currentColorScheme.main.accent};
        opacity: 1;
        filter: brightness(90%);
        cursor: pointer;
      }
    `}

  .button-content {
    color: ${({ $currentColorScheme }) => $currentColorScheme.text.accent};
    position: relative;
    display: flex;
    justify-content: space-between;
    vertical-align: middle;
    box-sizing: border-box;
    padding: ${({ theme }) =>
      getCorrectFourValuesStyle("5px 14px 5px 12px", theme.interfaceDirection)};
    line-height: 22px;
    border-radius: 3px;

    user-select: none;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  }
`;

const ArticleMainButtonContent = (props) => {
  const {
    t,
    tReady,
    isMobileArticle,

    isPrivacy,
    encryptedFile,
    encrypted,
    startUpload,
    setAction,
    setSelectFileDialogVisible,
    selectFileDialogVisible,
    showArticleLoader,
    isFavoritesFolder,
    isRecentFolder,
    isRecycleBinFolder,

    currentFolderId,
    isRoomsFolder,
    isArchiveFolder,

    enablePlugins,
    mainButtonItemsList,

    currentColorScheme,

    isOwner,
    isAdmin,

    setInvitePanelOptions,

    mainButtonMobileVisible,
    moveToPanelVisible,
    copyPanelVisible,

    security,
    isGracePeriod,
    setInviteUsersWarningDialogVisible,
  } = props;

  const navigate = useNavigate();
  const location = useLocation();

  const isAccountsPage = location.pathname.includes("/accounts");
  const isSettingsPage = location.pathname.includes("settings");

  const inputFilesElement = React.useRef(null);
  const inputFolderElement = React.useRef(null);

  const [actions, setActions] = React.useState([]);
  const [uploadActions, setUploadActions] = React.useState([]);
  const [model, setModel] = React.useState([]);
  const [isDropdownMainButton, setIsDropdownMainButton] = React.useState(true);

  const onCreate = React.useCallback(
    (e) => {
      const format = e.action || null;

      const event = new Event(Events.CREATE);

      const payload = {
        extension: format,
        id: -1,
      };
      event.payload = payload;

      window.dispatchEvent(event);
    },
    [setAction]
  );

  const onCreateRoom = React.useCallback(() => {
    if (isGracePeriod) {
      setInviteUsersWarningDialogVisible(true);
      return;
    }

    const event = new Event(Events.ROOM_CREATE);
    window.dispatchEvent(event);
  }, []);

  const onShowSelectFileDialog = React.useCallback(() => {
    setSelectFileDialogVisible(true);
  }, [setSelectFileDialogVisible]);

  const onFileChange = React.useCallback(
    (e) => {
      startUpload(e.target.files, null, t);
    },
    [startUpload, t]
  );

  const onUploadFileClick = React.useCallback(() => {
    if (isPrivacy) {
      encryptionUploadDialog((encryptedFile, encrypted) => {
        encryptedFile.encrypted = encrypted;
        startUpload([encryptedFile], null, t);
      });
    } else {
      inputFilesElement.current.click();
    }
  }, [
    isPrivacy,
    encrypted,
    encryptedFile,
    encryptionUploadDialog,
    startUpload,
  ]);

  const onUploadFolderClick = React.useCallback(() => {
    inputFolderElement.current.click();
  }, []);

  const onInputClick = React.useCallback((e) => (e.target.value = null), []);

  const onShowGallery = () => {
    navigate(`/form-gallery/${currentFolderId}/`);
  };

  const onInvite = React.useCallback((e) => {
    const type = e.action;

    if (isGracePeriod) {
      setInviteUsersWarningDialogVisible(true);
      return;
    }

    setInvitePanelOptions({
      visible: true,
      roomId: -1,
      hideSelector: true,
      defaultAccess: type,
    });
  }, []);

  const onInviteAgain = React.useCallback(() => {
    resendInvitesAgain()
      .then(() =>
        toastr.success(t("PeopleTranslations:SuccessSentMultipleInvitatios"))
      )
      .catch((err) => toastr.error(err));
  }, [resendInvitesAgain]);

  React.useEffect(() => {
    const isFolderHiddenDropdown =
      isArchiveFolder ||
      isFavoritesFolder ||
      isRecentFolder ||
      isRecycleBinFolder ||
      isSettingsPage;

    if (isFolderHiddenDropdown) {
      setIsDropdownMainButton(false);
    } else {
      setIsDropdownMainButton(true);
    }
  }, [
    isArchiveFolder,
    isFavoritesFolder,
    isRecentFolder,
    isRecycleBinFolder,
    isSettingsPage,
  ]);

  React.useEffect(() => {
    if (isRoomsFolder || isSettingsPage) return;

    const pluginItems = [];

    if (mainButtonItemsList && enablePlugins && !isAccountsPage) {
      mainButtonItemsList.forEach((option) => {
        pluginItems.push({
          key: option.key,
          ...option.value,
        });
      });
    }

    const formActions = [
      {
        id: "actions_template",
        className: "main-button_drop-down",
        icon: FormReactSvgUrl,
        label: t("Translations:NewForm"),
        key: "new-form",
        items: [
          {
            id: "actions_template_blank",
            className: "main-button_drop-down_sub",
            icon: FormBlankReactSvgUrl,
            label: t("Translations:SubNewForm"),
            onClick: onCreate,
            action: "docxf",
            key: "docxf",
          },
          {
            id: "actions_template_from-file",
            className: "main-button_drop-down_sub",
            icon: FormFileReactSvgUrl,
            label: t("Translations:SubNewFormFile"),
            onClick: onShowSelectFileDialog,
            disabled: isPrivacy,
            key: "form-file",
          },
          {
            id: "actions_template_oforms-gallery",
            className: "main-button_drop-down_sub",
            icon: FormGalleryReactSvgUrl,
            label: t("Common:OFORMsGallery"),
            onClick: onShowGallery,
            disabled: isPrivacy,
            key: "form-gallery",
          },
        ],
      },
    ];

    const addAdmin = isOwner
      ? [
          {
            id: "invite_doc-space-administrator",
            className: "main-button_drop-down",
            icon: PersonAdminReactSvgUrl,
            label: t("Common:DocSpaceAdmin"),
            onClick: onInvite,
            action: EmployeeType.Admin,
            key: "administrator",
          },
        ]
      : [];

    const actions = isAccountsPage
      ? [
          ...addAdmin,
          {
            id: "invite_room-admin",
            className: "main-button_drop-down",
            icon: PersonManagerReactSvgUrl,
            label: t("Common:RoomAdmin"),
            onClick: onInvite,
            action: EmployeeType.User,
            key: "manager",
          },
          {
            id: "invite_room-collaborator",
            className: "main-button_drop-down",
            icon: PersonReactSvgUrl,
            label: t("Common:PowerUser"),
            onClick: onInvite,
            action: EmployeeType.Collaborator,
            key: "collaborator",
          },
          {
            id: "invite_user",
            className: "main-button_drop-down",
            icon: PersonUserReactSvgUrl,
            label: t("Common:User"),
            onClick: onInvite,
            action: EmployeeType.Guest,
            key: "user",
          },
        ]
      : [
          {
            id: "actions_new-document",
            className: "main-button_drop-down",
            icon: ActionsDocumentsReactSvgUrl,
            label: t("Files:Document"),
            onClick: onCreate,
            action: "docx",
            key: "docx",
          },
          {
            id: "actions_new-spreadsheet",
            className: "main-button_drop-down",
            icon: SpreadsheetReactSvgUrl,
            label: t("Files:Spreadsheet"),
            onClick: onCreate,
            action: "xlsx",
            key: "xlsx",
          },
          {
            id: "actions_new-presentation",
            className: "main-button_drop-down",
            icon: ActionsPresentationReactSvgUrl,
            label: t("Files:Presentation"),
            onClick: onCreate,
            action: "pptx",
            key: "pptx",
          },
          ...formActions,
          {
            id: "actions_new-folder",
            className: "main-button_drop-down",
            icon: CatalogFolderReactSvgUrl,
            label: t("Files:Folder"),
            onClick: onCreate,
            key: "new-folder",
          },
        ];

    const uploadActions = isAccountsPage
      ? [
          {
            id: "invite_again",
            className: "main-button_drop-down",
            icon: InviteAgainReactSvgUrl,
            label: t("People:LblInviteAgain"),
            onClick: onInviteAgain,
            action: "invite-again",
            key: "invite-again",
          },
        ]
      : [
          {
            id: "actions_upload-files",
            className: "main-button_drop-down",
            icon: ActionsUploadReactSvgUrl,
            label: t("UploadFiles"),
            onClick: onUploadFileClick,
            key: "upload-files",
          },
          {
            id: "actions_upload-folders",
            className: "main-button_drop-down",
            icon: ActionsUploadReactSvgUrl,
            label: t("UploadFolder"),
            disabled: isPrivacy,
            onClick: onUploadFolderClick,
            key: "upload-folder",
          },
        ];

    const menuModel = [...actions];

    if (pluginItems.length > 0) {
      menuModel.push({
        id: "actions_more-plugins",
        className: "main-button_drop-down",
        icon: PluginMoreReactSvgUrl,
        label: t("Common:More"),
        disabled: false,
        key: "more-plugins",
        items: pluginItems,
      });
    }

    menuModel.push({
      isSeparator: true,
      key: "separator",
    });

    menuModel.push(...uploadActions);
    setUploadActions(uploadActions);

    setModel(menuModel);
    setActions(actions);
  }, [
    t,
    isPrivacy,
    currentFolderId,
    isAccountsPage,
    isSettingsPage,
    enablePlugins,
    mainButtonItemsList,
    isRoomsFolder,
    isOwner,
    isAdmin,

    onCreate,
    onCreateRoom,
    onInvite,
    onInviteAgain,
    onShowSelectFileDialog,
    onUploadFileClick,
    onUploadFolderClick,
  ]);

  const mainButtonText = isAccountsPage
    ? t("Common:Invite")
    : t("Common:Actions");

  const isDisabled = isSettingsPage
    ? isSettingsPage
    : isAccountsPage
    ? !isAccountsPage
    : !security?.Create;

  const isProfile = location.pathname.includes("/profile");

  let mainButtonVisible = true;

  if (isMobileOnly) {
    mainButtonVisible =
      moveToPanelVisible || copyPanelVisible || selectFileDialogVisible
        ? false
        : true;
  }

  if (showArticleLoader)
    return isMobileArticle ? null : <Loaders.ArticleButton height="32px" />;

  return (
    <>
      {isMobileArticle ? (
        <>
          {!isProfile && (security?.Create || isAccountsPage) && (
            <MobileView
              t={t}
              titleProp={t("Upload")}
              actionOptions={actions}
              buttonOptions={uploadActions}
              isRooms={isRoomsFolder}
              mainButtonMobileVisible={
                mainButtonMobileVisible && mainButtonVisible
              }
              onMainButtonClick={onCreateRoom}
            />
          )}
        </>
      ) : isRoomsFolder ? (
        <StyledButton
          className="create-room-button"
          id="rooms-shared_create-room-button"
          label={t("Files:NewRoom")}
          onClick={onCreateRoom}
          $currentColorScheme={currentColorScheme}
          isDisabled={isDisabled}
          size="small"
          primary
          scale
          title={t("Files:NewRoom")}
        />
      ) : (
        <MainButton
          id={
            isAccountsPage
              ? "accounts_invite-main-button"
              : "actions-main-button"
          }
          isDisabled={isDisabled}
          isDropdown={isDropdownMainButton}
          text={mainButtonText}
          model={model}
          title={mainButtonText}
        />
      )}

      <input
        id="customFileInput"
        className="custom-file-input"
        multiple
        type="file"
        onChange={onFileChange}
        onClick={onInputClick}
        ref={inputFilesElement}
        style={{ display: "none" }}
      />
      <input
        id="customFolderInput"
        className="custom-file-input"
        webkitdirectory=""
        mozdirectory=""
        type="file"
        onChange={onFileChange}
        onClick={onInputClick}
        ref={inputFolderElement}
        style={{ display: "none" }}
      />
    </>
  );
};

export default inject(
  ({
    auth,
    filesStore,
    dialogsStore,
    uploadDataStore,
    treeFoldersStore,
    selectedFolderStore,
    clientLoadingStore,
    pluginStore,
  }) => {
    const { showArticleLoader } = clientLoadingStore;
    const { mainButtonMobileVisible } = filesStore;
    const {
      isPrivacyFolder,
      isFavoritesFolder,
      isRecentFolder,
      isRecycleBinFolder,
      isRoomsFolder,
      isArchiveFolder,
      selectedTreeNode,
    } = treeFoldersStore;
    const { startUpload } = uploadDataStore;
    const {
      setSelectFileDialogVisible,
      setInvitePanelOptions,
      setInviteUsersWarningDialogVisible,
      copyPanelVisible,
      moveToPanelVisible,
      selectFileDialogVisible,
    } = dialogsStore;

    const { enablePlugins, currentColorScheme } = auth.settingsStore;

    const security = selectedFolderStore.security;

    const currentFolderId = selectedFolderStore.id;

    const { isAdmin, isOwner } = auth.userStore.user;
    const { isGracePeriod } = auth.currentTariffStatusStore;

    const { mainButtonItemsList } = pluginStore;

    return {
      isGracePeriod,
      setInviteUsersWarningDialogVisible,
      showText: auth.settingsStore.showText,
      isMobileArticle: auth.settingsStore.isMobileArticle,

      showArticleLoader,
      isPrivacy: isPrivacyFolder,
      isFavoritesFolder,
      isRecentFolder,
      isRecycleBinFolder,

      isRoomsFolder,
      isArchiveFolder,
      selectedTreeNode,

      startUpload,

      setSelectFileDialogVisible,
      selectFileDialogVisible,
      setInvitePanelOptions,

      currentFolderId,

      enablePlugins,
      mainButtonItemsList,

      currentColorScheme,

      isAdmin,
      isOwner,

      mainButtonMobileVisible,
      moveToPanelVisible,
      copyPanelVisible,
      security,
    };
  }
)(
  withTranslation([
    "Article",
    "UploadPanel",
    "Common",
    "Files",
    "People",
    "PeopleTranslations",
  ])(observer(ArticleMainButtonContent))
);
