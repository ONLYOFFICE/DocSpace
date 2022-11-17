import React from "react";

import { inject, observer } from "mobx-react";

import MainButton from "@docspace/components/main-button";
import { withTranslation } from "react-i18next";
import { isMobile } from "react-device-detect";
import Loaders from "@docspace/common/components/Loaders";
import { AppServerConfig, FileAction } from "@docspace/common/constants";
import { encryptionUploadDialog } from "../../../helpers/desktop";
import { withRouter } from "react-router";

import MobileView from "./MobileView";
import { combineUrl } from "@docspace/common/utils";
import config from "PACKAGE_FILE";
import withLoader from "../../../HOCs/withLoader";
import { Events, EmployeeType } from "@docspace/common/constants";
import { getMainButtonItems } from "SRC_DIR/helpers/plugins";

import toastr from "@docspace/components/toast/toastr";
import styled from "styled-components";
import Button from "@docspace/components/button";

const StyledButton = styled(Button)`
  font-weight: 700;
  font-size: 16px;
  padding: 0;
  opacity: 1;

  background-color: ${({ currentColorScheme }) =>
    currentColorScheme.main.accent};

  :hover {
    background-color: ${({ currentColorScheme }) =>
      currentColorScheme.main.accent};
    opacity: 0.85;
  }

  :active {
    background-color: ${({ currentColorScheme }) =>
      currentColorScheme.main.accent};

    opacity: 1;
    filter: brightness(90%);
    cursor: pointer;
  }

  .button-content {
    position: relative;
    display: flex;
    justify-content: space-between;
    vertical-align: middle;
    box-sizing: border-box;
    padding: 5px 14px 5px 12px;
    line-height: 22px;
    border-radius: 3px;

    user-select: none;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  }
`;

const ArticleMainButtonContent = (props) => {
  const {
    t,
    isMobileArticle,
    canCreate,
    isPrivacy,
    encryptedFile,
    encrypted,
    startUpload,
    setAction,
    setSelectFileDialogVisible,
    isArticleLoading,
    isFavoritesFolder,
    isShareFolder,
    isRecentFolder,
    isCommonFolder,
    isRecycleBinFolder,
    history,
    currentFolderId,
    isRoomsFolder,
    isArchiveFolder,

    selectedTreeNode,

    enablePlugins,

    currentColorScheme,

    isOwner,
    isAdmin,

    canCreateFiles,

    setInvitePanelOptions,
  } = props;

  const isAccountsPage = selectedTreeNode[0] === "accounts";

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
    history.push(
      combineUrl(
        AppServerConfig.proxyURL,
        config.homepage,
        `/form-gallery/${currentFolderId}/`
      )
    );
  };

  const onInvite = React.useCallback((e) => {
    const type = e.action;

    setInvitePanelOptions({
      visible: true,
      roomId: -1,
      hideSelector: true,
      defaultAccess: type,
    });
  }, []);

  const onInviteAgain = React.useCallback(() => {
    console.log("invite again");
    toastr.warning("Work in progress (invite again)");
  }, []);

  React.useEffect(() => {
    const isSettingFolder =
      window.location.pathname.endsWith("/settings/common") ||
      window.location.pathname.endsWith("/settings/admin");

    const isFolderHiddenDropdown =
      isArchiveFolder ||
      isFavoritesFolder ||
      isRecentFolder ||
      isRecycleBinFolder ||
      isSettingFolder;

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
    window.location.pathname,
  ]);

  React.useEffect(() => {
    if (isRoomsFolder) return;

    const folderUpload = !isMobile
      ? [
          {
            id: "actions_upload-folders",
            className: "main-button_drop-down",
            icon: "images/actions.upload.react.svg",
            label: t("UploadFolder"),
            disabled: isPrivacy,
            onClick: onUploadFolderClick,
            key: "upload-folder",
          },
        ]
      : [];

    const formActions = [
      {
        id: "actions_template",
        className: "main-button_drop-down",
        icon: "images/form.react.svg",
        label: t("Translations:NewForm"),
        key: "new-form",
        items: [
          {
            id: "actions_template_blank",
            className: "main-button_drop-down_sub",
            icon: "images/form.blank.react.svg",
            label: t("Translations:SubNewForm"),
            onClick: onCreate,
            action: "docxf",
            key: "docxf",
          },
          {
            id: "actions_template_from-file",
            className: "main-button_drop-down_sub",
            icon: "images/form.file.react.svg",
            label: t("Translations:SubNewFormFile"),
            onClick: onShowSelectFileDialog,
            disabled: isPrivacy,
            key: "form-file",
          },
          {
            id: "actions_template_oforms-gallery",
            className: "main-button_drop-down_sub",
            icon: "images/form.gallery.react.svg",
            label: t("Common:OFORMsGallery"),
            onClick: onShowGallery,
            disabled: isPrivacy,
            key: "form-gallery",
          },
        ],
      },
    ];

    const actions = isAccountsPage
      ? [
          isOwner && {
            id: "invite_doc-space-administrator",
            className: "main-button_drop-down",
            icon: "/static/images/person.admin.react.svg",
            label: t("Common:DocSpaceAdmin"),
            onClick: onInvite,
            action: EmployeeType.Admin,
            key: "administrator",
          },
          {
            id: "invite_room-admin",
            className: "main-button_drop-down",
            icon: "/static/images/person.manager.react.svg",
            label: t("Common:RoomAdmin"),
            onClick: onInvite,
            action: EmployeeType.User,
            key: "manager",
          },
          {
            id: "invite_user",
            className: "main-button_drop-down",
            icon: "/static/images/person.user.react.svg",
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
            icon: "images/actions.documents.react.svg",
            label: t("NewDocument"),
            onClick: onCreate,
            action: "docx",
            key: "docx",
          },
          {
            id: "actions_new-spreadsheet",
            className: "main-button_drop-down",
            icon: "images/spreadsheet.react.svg",
            label: t("NewSpreadsheet"),
            onClick: onCreate,
            action: "xlsx",
            key: "xlsx",
          },
          {
            id: "actions_new-presentation",
            className: "main-button_drop-down",
            icon: "images/actions.presentation.react.svg",
            label: t("NewPresentation"),
            onClick: onCreate,
            action: "pptx",
            key: "pptx",
          },
          ...formActions,
          {
            id: "actions_new-folder",
            className: "main-button_drop-down",
            icon: "images/catalog.folder.react.svg",
            label: t("NewFolder"),
            onClick: onCreate,
            key: "new-folder",
          },
        ];

    const uploadActions = isAccountsPage
      ? [
          {
            id: "invite_again",
            className: "main-button_drop-down",
            icon: "/static/images/invite.again.react.svg",
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
            icon: "images/actions.upload.react.svg",
            label: t("UploadFiles"),
            onClick: onUploadFileClick,
            key: "upload-files",
          },
          ...folderUpload,
        ];

    const menuModel = [...actions];

    menuModel.push({
      isSeparator: true,
      key: "separator",
    });

    menuModel.push(...uploadActions);
    setUploadActions(uploadActions);

    if (enablePlugins) {
      const pluginOptions = getMainButtonItems();

      if (pluginOptions) {
        pluginOptions.forEach((option) => {
          menuModel.splice(option.value.position, 0, {
            key: option.key,
            ...option.value,
          });
        });
      }
    }

    setModel(menuModel);
    setActions(actions);
  }, [
    t,
    isPrivacy,
    currentFolderId,
    isAccountsPage,
    enablePlugins,
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

  const canInvite =
    isAccountsPage &&
    selectedTreeNode.length > 1 &&
    selectedTreeNode[1] === "filter";
  const mainButtonText = isAccountsPage
    ? t("Common:Invite")
    : t("Common:Actions");

  const isDisabled =
    ((!canCreate || (!canCreateFiles && !isRoomsFolder)) && !canInvite) ||
    isArchiveFolder;
  const isProfile = history.location.pathname === "/accounts/view/@self";

  return (
    <>
      {isMobileArticle ? (
        <>
          {!isFavoritesFolder &&
            !isRecentFolder &&
            !isCommonFolder &&
            !isShareFolder &&
            !isRecycleBinFolder &&
            !isArchiveFolder &&
            !isArticleLoading &&
            !isProfile &&
            ((canCreate && (canCreateFiles || isRoomsFolder)) || canInvite) && (
              <MobileView
                t={t}
                titleProp={t("Upload")}
                actionOptions={actions}
                buttonOptions={uploadActions}
                isRooms={isRoomsFolder}
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
          currentColorScheme={currentColorScheme}
          isDisabled={isDisabled}
          size="small"
          primary
          scale
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
    accessRightsStore,
  }) => {
    const { isLoaded, firstLoad, isLoading, canCreate } = filesStore;
    const {
      isPrivacyFolder,
      isFavoritesFolder,
      isRecentFolder,
      isCommonFolder,
      isRecycleBinFolder,
      isShareFolder,
      isRoomsFolder,
      isArchiveFolder,
      selectedTreeNode,
    } = treeFoldersStore;
    const { startUpload } = uploadDataStore;
    const { setSelectFileDialogVisible, setInvitePanelOptions } = dialogsStore;

    const isArticleLoading = (!isLoaded || isLoading) && firstLoad;

    const { enablePlugins, currentColorScheme } = auth.settingsStore;

    const currentFolderId = selectedFolderStore.id;

    const { isAdmin, isOwner, isVisitor } = auth.userStore.user;

    const { canCreateFiles } = accessRightsStore;

    return {
      showText: auth.settingsStore.showText,
      isMobileArticle: auth.settingsStore.isMobileArticle,

      isArticleLoading,
      isPrivacy: isPrivacyFolder,
      isFavoritesFolder,
      isRecentFolder,
      isCommonFolder,
      isRecycleBinFolder,
      isShareFolder,
      isRoomsFolder,
      isArchiveFolder,
      selectedTreeNode,

      canCreate,
      canCreateFiles,

      startUpload,

      setSelectFileDialogVisible,
      setInvitePanelOptions,

      isLoading,
      isLoaded,
      firstLoad,
      currentFolderId,

      enablePlugins,
      currentColorScheme,

      isAdmin,
      isOwner,
      isVisitor,
    };
  }
)(
  withTranslation(["Article", "UploadPanel", "Common", "Files", "People"])(
    withLoader(observer(withRouter(ArticleMainButtonContent)))(
      <Loaders.ArticleButton height="28px" />
    )
  )
);
