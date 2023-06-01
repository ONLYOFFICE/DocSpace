import FolderLockedReactSvgUrl from "PUBLIC_DIR/images/folder.locked.react.svg?url";
import ActionsDocumentsReactSvgUrl from "PUBLIC_DIR/images/actions.documents.react.svg?url";
import SpreadsheetReactSvgUrl from "PUBLIC_DIR/images/spreadsheet.react.svg?url";
import ActionsPresentationReactSvgUrl from "PUBLIC_DIR/images/actions.presentation.react.svg?url";
import FormReactSvgUrl from "PUBLIC_DIR/images/access.form.react.svg?url";
import FormBlankReactSvgUrl from "PUBLIC_DIR/images/form.blank.react.svg?url";
import FormFileReactSvgUrl from "PUBLIC_DIR/images/form.file.react.svg?url";
import FormGalleryReactSvgUrl from "PUBLIC_DIR/images/form.gallery.react.svg?url";
import CatalogFolderReactSvgUrl from "PUBLIC_DIR/images/catalog.folder.react.svg?url";
import ActionsUploadReactSvgUrl from "PUBLIC_DIR/images/actions.upload.react.svg?url";
import ClearTrashReactSvgUrl from "PUBLIC_DIR/images/clear.trash.react.svg?url";
import ReconnectSvgUrl from "PUBLIC_DIR/images/reconnect.svg?url";
import SettingsReactSvgUrl from "PUBLIC_DIR/images/catalog.settings.react.svg?url";
import DownloadReactSvgUrl from "PUBLIC_DIR/images/download.react.svg?url";
import MoveReactSvgUrl from "PUBLIC_DIR/images/move.react.svg?url";
import RenameReactSvgUrl from "PUBLIC_DIR/images/rename.react.svg?url";
import ShareReactSvgUrl from "PUBLIC_DIR/images/share.react.svg?url";
import InvitationLinkReactSvgUrl from "PUBLIC_DIR/images/invitation.link.react.svg?url";
import InfoOutlineReactSvgUrl from "PUBLIC_DIR/images/info.outline.react.svg?url";
import PersonReactSvgUrl from "PUBLIC_DIR/images/person.react.svg?url";
import RoomArchiveSvgUrl from "PUBLIC_DIR/images/room.archive.svg?url";
import CopyReactSvgUrl from "PUBLIC_DIR/images/copy.react.svg?url";
import CatalogTrashReactSvgUrl from "PUBLIC_DIR/images/catalog.trash.react.svg?url";
import PlusSvgUrl from "PUBLIC_DIR/images/plus.svg?url";
import PanelReactSvgUrl from "PUBLIC_DIR/images/panel.react.svg?url";
import PersonAdminReactSvgUrl from "PUBLIC_DIR/images/person.admin.react.svg?url";
import PersonManagerReactSvgUrl from "PUBLIC_DIR/images/person.manager.react.svg?url";
import PersonUserReactSvgUrl from "PUBLIC_DIR/images/person.user.react.svg?url";
import InviteAgainReactSvgUrl from "PUBLIC_DIR/images/invite.again.react.svg?url";

import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { isMobile, isTablet, isMobileOnly } from "react-device-detect";
import styled, { css } from "styled-components";
import copy from "copy-to-clipboard";
import { useNavigate, useLocation } from "react-router-dom";

import Loaders from "@docspace/common/components/Loaders";
import Navigation from "@docspace/common/components/Navigation";
import TrashWarning from "@docspace/common/components/Navigation/sub-components/trash-warning";
import { Events, EmployeeType } from "@docspace/common/constants";

import { resendInvitesAgain } from "@docspace/common/api/people";

import DropDownItem from "@docspace/components/drop-down-item";
import { tablet, mobile } from "@docspace/components/utils/device";
import { Consumer } from "@docspace/components/utils/context";
import toastr from "@docspace/components/toast/toastr";
import TableGroupMenu from "@docspace/components/table-container/TableGroupMenu";

import { getMainButtonItems } from "SRC_DIR/helpers/plugins";
import withLoader from "../../../../HOCs/withLoader";

const StyledContainer = styled.div`
  width: 100%;
  min-height: 33px;

  .table-container_group-menu {
    margin: 0 0 0 -20px;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

    width: calc(100% + 40px);
    height: 68px;

    @media ${tablet} {
      height: 60px;
      margin: 0 0 0 -16px;
      width: calc(100% + 32px);
    }

    ${isMobile &&
    css`
      height: 60px;
      margin: 0 0 0 -16px;
      width: calc(100% + 32px);
    `}

    @media ${mobile} {
      height: 52px;
      margin: 0 0 0 -16px;
      width: calc(100% + 32px);
    }

    ${isMobileOnly &&
    css`
      height: 52px;
      margin: 0 0 0 -16px;
      width: calc(100% + 32px);
    `}
  }

  .header-container {
    min-height: 33px;

    ${(props) =>
      props.hideContextMenuInsideArchiveRoom &&
      `.option-button {
      display: none;}`}

    @media ${tablet} {
      height: 60px;
    }
  }
`;

const SectionHeaderContent = (props) => {
  const {
    currentFolderId,
    setSelectFileDialogVisible,
    t,
    isPrivacyFolder,
    isRoomsFolder,
    enablePlugins,
    security,
    setIsFolderActions,
    setBufferSelection,
    setMoveToPanelVisible,
    tReady,
    isInfoPanelVisible,
    isRootFolder,
    title,

    isDesktop,
    isTabletView,
    personal,
    navigationPath,
    getHeaderMenu,
    isRecycleBinFolder,
    isArchiveFolder,
    isEmptyFilesList,
    isHeaderVisible,
    isHeaderChecked,
    isHeaderIndeterminate,
    showText,

    isEmptyArchive,

    isRoom,
    isGroupMenuBlocked,

    onClickBack,
    hideContextMenuInsideArchiveRoom,
    activeFiles,
    activeFolders,
    selectedFolder,
    setCopyPanelVisible,
    setSharingPanelVisible,
    deleteAction,
    confirmDelete,
    setDeleteDialogVisible,
    isThirdPartySelection,

    getFolderInfo,

    setEmptyTrashDialogVisible,
    setRestoreAllPanelVisible,
    isGracePeriod,
    setInviteUsersWarningDialogVisible,
    setArchiveAction,
    setRestoreAllArchive,
    setRestoreRoomDialogVisible,
    setArchiveDialogVisible,
    onCopyLink,

    isPersonalRoom,

    onClickEditRoom,
    onClickInviteUsers,
    onShowInfoPanel,
    onClickArchive,
    onClickReconnectStorage,

    canRestoreAll,
    canDeleteAll,
    setSelected,
    cbMenuItems,
    getCheckboxItemLabel,
    getCheckboxItemId,
    setSelectedNode,
    setIsLoading,
    fetchFiles,
    moveToRoomsPage,
    setIsInfoPanelVisible,

    getAccountsHeaderMenu,
    isAccountsHeaderVisible,
    isAccountsHeaderIndeterminate,
    isAccountsHeaderChecked,
    accountsCbMenuItems,
    getAccountsMenuItemId,
    getAccountsCheckboxItemLabel,
    setAccountsSelected,
    isOwner,
    isAdmin,
    setInvitePanelOptions,
    isEmptyPage,
    pathParts,
    emptyTrashInProgress
  } = props;

  const navigate = useNavigate();
  const location = useLocation();

  const isAccountsPage = location.pathname.includes("accounts");
  const isSettingsPage = location.pathname.includes("settings");

  const onCreate = (format) => {
    const event = new Event(Events.CREATE);

    const payload = {
      extension: format,
      id: -1,
    };

    event.payload = payload;

    window.dispatchEvent(event);
  };

  const onCreateRoom = () => {
    if (isGracePeriod) {
      setInviteUsersWarningDialogVisible(true);
      return;
    }

    const event = new Event(Events.ROOM_CREATE);
    window.dispatchEvent(event);
  };

  const createDocument = () => onCreate("docx");

  const createSpreadsheet = () => onCreate("xlsx");

  const createPresentation = () => onCreate("pptx");

  const createForm = () => onCreate("docxf");

  const createFormFromFile = () => {
    setSelectFileDialogVisible(true);
  };

  const onShowGallery = () => {
    navigate(`/form-gallery/${currentFolderId}/`);
  };

  const createFolder = () => onCreate();

  // TODO: add privacy room check for files
  const onUploadAction = (type) => {
    const element =
      type === "file"
        ? document.getElementById("customFileInput")
        : document.getElementById("customFolderInput");

    element?.click();
  };

  const getContextOptionsPlus = () => {
    if (isAccountsPage) {
      return [
        isOwner && {
          id: "accounts-add_administrator",
          className: "main-button_drop-down",
          icon: PersonAdminReactSvgUrl,
          label: t("Common:DocSpaceAdmin"),
          onClick: onInvite,
          "data-type": EmployeeType.Admin,
          key: "administrator",
        },
        {
          id: "accounts-add_manager",
          className: "main-button_drop-down",
          icon: PersonManagerReactSvgUrl,
          label: t("Common:RoomAdmin"),
          onClick: onInvite,
          "data-type": EmployeeType.User,
          key: "manager",
        },
        {
          id: "accounts-add_collaborator",
          className: "main-button_drop-down",
          icon: PersonReactSvgUrl,
          label: t("Common:PowerUser"),
          onClick: onInvite,
          "data-type": EmployeeType.Collaborator,
          key: "collaborator",
        },
        {
          id: "accounts-add_user",
          className: "main-button_drop-down",
          icon: PersonUserReactSvgUrl,
          label: t("Common:User"),
          onClick: onInvite,
          "data-type": EmployeeType.Guest,
          key: "user",
        },
        {
          key: "separator",
          isSeparator: true,
        },
        {
          id: "accounts-add_invite-again",
          className: "main-button_drop-down",
          icon: InviteAgainReactSvgUrl,
          label: t("People:LblInviteAgain"),
          onClick: onInviteAgain,
          "data-action": "invite-again",
          key: "invite-again",
        },
      ];
    }

    const options = isRoomsFolder
      ? [
          {
            key: "new-room",
            label: t("NewRoom"),
            onClick: onCreateRoom,
            icon: FolderLockedReactSvgUrl,
          },
        ]
      : [
          {
            id: "personal_new-documnet",
            key: "new-document",
            label: t("Common:NewDocument"),
            onClick: createDocument,
            icon: ActionsDocumentsReactSvgUrl,
          },
          {
            id: "personal_new-spreadsheet",
            key: "new-spreadsheet",
            label: t("Common:NewSpreadsheet"),
            onClick: createSpreadsheet,
            icon: SpreadsheetReactSvgUrl,
          },
          {
            id: "personal_new-presentation",
            key: "new-presentation",
            label: t("Common:NewPresentation"),
            onClick: createPresentation,
            icon: ActionsPresentationReactSvgUrl,
          },
          {
            id: "personal_form-template",
            icon: FormReactSvgUrl,
            label: t("Translations:NewForm"),
            key: "new-form-base",
            items: [
              {
                id: "personal_template_black",
                key: "new-form",
                label: t("Translations:SubNewForm"),
                icon: FormBlankReactSvgUrl,
                onClick: createForm,
              },
              {
                id: "personal_template_new-form-file",
                key: "new-form-file",
                label: t("Translations:SubNewFormFile"),
                icon: FormFileReactSvgUrl,
                onClick: createFormFromFile,
                disabled: isPrivacyFolder,
              },
              {
                id: "personal_template_oforms-gallery",
                key: "oforms-gallery",
                label: t("Common:OFORMsGallery"),
                icon: FormGalleryReactSvgUrl,
                onClick: onShowGallery,
                disabled: isPrivacyFolder || (isMobile && isTablet),
              },
            ],
          },
          {
            id: "personal_new-folder",
            key: "new-folder",
            label: t("Common:NewFolder"),
            onClick: createFolder,
            icon: CatalogFolderReactSvgUrl,
          },
          { key: "separator", isSeparator: true },
          {
            key: "upload-files",
            label: t("Article:UploadFiles"),
            onClick: () => onUploadAction("file"),
            icon: ActionsUploadReactSvgUrl,
          },
          {
            key: "upload-folder",
            label: t("Article:UploadFolder"),
            onClick: () => onUploadAction("folder"),
            icon: ActionsUploadReactSvgUrl,
          },
        ];

    if (enablePlugins) {
      const pluginOptions = getMainButtonItems();

      if (pluginOptions) {
        pluginOptions.forEach((option) => {
          options.splice(option.value.position, 0, {
            key: option.key,
            ...option.value,
          });
        });
      }
    }

    return options;
  };

  const createLinkForPortalUsers = () => {
    copy(
      `${window.location.origin}/filter?folder=${currentFolderId}` //TODO: Change url by category
    );

    toastr.success(t("Translations:LinkCopySuccess"));
  };

  const onMoveAction = () => {
    setIsFolderActions(true);
    setBufferSelection(selectedFolder);
    return setMoveToPanelVisible(true);
  };

  const onCopyAction = () => {
    setIsFolderActions(true);
    setBufferSelection(currentFolderId);
    return setCopyPanelVisible(true);
  };

  const onDownloadAction = () => {
    setBufferSelection(currentFolderId);
    setIsFolderActions(true);
    props
      .downloadAction(t("Translations:ArchivingData"), [currentFolderId])
      .catch((err) => toastr.error(err));
  };

  const renameAction = () => {
    const event = new Event(Events.RENAME);

    event.item = selectedFolder;

    window.dispatchEvent(event);
  };

  const onOpenSharingPanel = () => {
    setBufferSelection(currentFolderId);
    setIsFolderActions(true);
    return setSharingPanelVisible(true);
  };

  const onDeleteAction = () => {
    setIsFolderActions(true);

    if (confirmDelete || isThirdPartySelection) {
      getFolderInfo(currentFolderId).then((data) => {
        setBufferSelection(data);
        setDeleteDialogVisible(true);
      });
    } else {
      const translations = {
        deleteOperation: t("Translations:DeleteOperation"),
        deleteFromTrash: t("Translations:DeleteFromTrash"),
        deleteSelectedElem: t("Translations:DeleteSelectedElem"),
        FolderRemoved: t("Files:FolderRemoved"),
      };

      deleteAction(translations, [currentFolderId], true).catch((err) =>
        toastr.error(err)
      );
    }
  };

  const onEmptyTrashAction = () => {
    const isExistActiveItems = [...activeFiles, ...activeFolders].length > 0;

    if (isExistActiveItems || emptyTrashInProgress) return;

    setEmptyTrashDialogVisible(true);
  };

  const onRestoreAllAction = () => {
    setRestoreAllPanelVisible;
    const isExistActiveItems = [...activeFiles, ...activeFolders].length > 0;

    if (isExistActiveItems) return;

    setRestoreAllPanelVisible(true);
  };

  const onRestoreAllArchiveAction = () => {
    const isExistActiveItems = [...activeFiles, ...activeFolders].length > 0;

    if (isExistActiveItems) return;

    if (isGracePeriod) {
      setInviteUsersWarningDialogVisible(true);
      return;
    }

    setRestoreAllArchive(true);
    setRestoreRoomDialogVisible(true);
  };

  const onShowInfo = () => {
    const { setIsInfoPanelVisible } = props;
    setIsInfoPanelVisible(true);
  };

  const onToggleInfoPanel = () => {
    setIsInfoPanelVisible(!isInfoPanelVisible);
  };

  const onCopyLinkAction = () => {
    onCopyLink && onCopyLink({ ...selectedFolder, isFolder: true }, t);
  };

  const getContextOptionsFolder = () => {
    const isDisabled = isRecycleBinFolder || isRoom;

    if (isArchiveFolder) {
      return [
        {
          id: "header_option_empty-archive",
          key: "empty-archive",
          label: t("ArchiveAction"),
          onClick: onEmptyTrashAction,
          disabled: !canDeleteAll,
          icon: ClearTrashReactSvgUrl,
        },
        {
          id: "header_option_restore-all",
          key: "restore-all",
          label: t("RestoreAll"),
          onClick: onRestoreAllArchiveAction,
          disabled: !canRestoreAll,
          icon: MoveReactSvgUrl,
        },
      ];
    }

    return [
      {
        id: "header_option_sharing-settings",
        key: "sharing-settings",
        label: t("SharingPanel:SharingSettingsTitle"),
        onClick: onOpenSharingPanel,
        disabled: true,
        icon: ShareReactSvgUrl,
      },
      {
        id: "header_option_link-portal-users",
        key: "link-portal-users",
        label: t("LinkForPortalUsers"),
        onClick: createLinkForPortalUsers,
        disabled: true,
        icon: InvitationLinkReactSvgUrl,
      },
      {
        id: "header_option_link-for-room-members",
        key: "link-for-room-members",
        label: t("LinkForRoomMembers"),
        onClick: onCopyLinkAction,
        disabled: isRecycleBinFolder || isPersonalRoom,
        icon: InvitationLinkReactSvgUrl,
      },
      {
        id: "header_option_empty-trash",
        key: "empty-trash",
        label: t("RecycleBinAction"),
        onClick: onEmptyTrashAction,
        disabled: !isRecycleBinFolder,
        icon: ClearTrashReactSvgUrl,
      },
      {
        id: "header_option_restore-all",
        key: "restore-all",
        label: t("RestoreAll"),
        onClick: onRestoreAllAction,
        disabled: !isRecycleBinFolder,
        icon: MoveReactSvgUrl,
      },
      {
        id: "header_option_show-info",
        key: "show-info",
        label: t("Common:Info"),
        onClick: onShowInfo,
        disabled: isDisabled,
        icon: InfoOutlineReactSvgUrl,
      },
      {
        id: "header_option_reconnect-storage",
        key: "reconnect-storage",
        label: t("Common:ReconnectStorage"),
        icon: ReconnectSvgUrl,
        onClick: () => onClickReconnectStorage(selectedFolder, t),
        disabled: !selectedFolder.providerKey || !isRoom,
      },
      {
        id: "header_option_edit-room",
        key: "edit-room",
        label: t("EditRoom"),
        icon: SettingsReactSvgUrl,
        onClick: () => onClickEditRoom(selectedFolder),
        disabled: !isRoom || !security?.EditRoom,
      },
      {
        id: "header_option_invite-users-to-room",
        key: "invite-users-to-room",
        label: t("Common:InviteUsers"),
        icon: PersonReactSvgUrl,
        onClick: () => onClickInviteUsers(selectedFolder.id),
        disabled: !isRoom || !security?.EditAccess,
      },
      {
        id: "header_option_room-info",
        key: "room-info",
        label: t("Common:Info"),
        icon: InfoOutlineReactSvgUrl,
        onClick: onToggleInfoPanel,
        disabled: !isRoom,
      },
      {
        id: "header_option_separator-2",
        key: "separator-2",
        isSeparator: true,
        disabled: isRecycleBinFolder,
      },
      {
        id: "header_option_archive-room",
        key: "archive-room",
        label: t("MoveToArchive"),
        icon: RoomArchiveSvgUrl,
        onClick: (e) => onClickArchive(e),
        disabled: !isRoom || !security?.Move,
        "data-action": "archive",
        action: "archive",
      },
      {
        id: "header_option_download",
        key: "download",
        label: t("Common:Download"),
        onClick: onDownloadAction,
        disabled: isDisabled,
        icon: DownloadReactSvgUrl,
      },
      {
        id: "header_option_move-to",
        key: "move-to",
        label: t("Common:MoveTo"),
        onClick: onMoveAction,
        disabled: isDisabled || !security?.MoveTo,
        icon: MoveReactSvgUrl,
      },
      {
        id: "header_option_copy",
        key: "copy",
        label: t("Common:Copy"),
        onClick: onCopyAction,
        disabled: isDisabled || !security?.CopyTo,
        icon: CopyReactSvgUrl,
      },
      {
        id: "header_option_rename",
        key: "rename",
        label: t("Common:Rename"),
        onClick: renameAction,
        disabled: isDisabled || !security?.Rename,
        icon: RenameReactSvgUrl,
      },
      {
        id: "header_option_separator-3",
        key: "separator-3",
        isSeparator: true,
        disabled: isDisabled || !security?.Delete,
      },
      {
        id: "header_option_delete",
        key: "delete",
        label: t("Common:Delete"),
        onClick: onDeleteAction,
        disabled: isDisabled || !security?.Delete,
        icon: CatalogTrashReactSvgUrl,
      },
    ];
  };

  const onSelect = (e) => {
    const key = e.currentTarget.dataset.key;

    isAccountsPage ? setAccountsSelected(key) : setSelected(key);
  };

  const onClose = () => {
    setSelected("close");
  };

  const getMenuItems = () => {
    const checkboxOptions = isAccountsPage ? (
      <>
        {accountsCbMenuItems.map((key) => {
          const label = getAccountsCheckboxItemLabel(t, key);
          const id = getAccountsMenuItemId(key);
          return (
            <DropDownItem
              id={id}
              key={key}
              label={label}
              data-key={key}
              onClick={onSelect}
            />
          );
        })}
      </>
    ) : (
      <>
        {cbMenuItems.map((key) => {
          const label = getCheckboxItemLabel(t, key);
          const id = getCheckboxItemId(key);
          return (
            <DropDownItem
              id={id}
              key={key}
              label={label}
              data-key={key}
              onClick={onSelect}
            />
          );
        })}
      </>
    );

    return checkboxOptions;
  };

  const onChange = (checked) => {
    isAccountsPage
      ? setAccountsSelected(checked ? "all" : "none")
      : setSelected(checked ? "all" : "none");
  };

  const onClickFolder = (id, isRootRoom) => {
    if (isRootRoom) {
      return moveToRoomsPage();
    }

    setSelectedNode(id);
    setIsLoading(true);
    fetchFiles(id, null, true, false)
      .catch((err) => toastr.error(err))
      .finally(() => setIsLoading(false));
  };

  const onInvite = (e) => {
    const type = e.item["data-type"];

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
  };

  const onInviteAgain = React.useCallback(() => {
    resendInvitesAgain()
      .then(() =>
        toastr.success(t("PeopleTranslations:SuccessSentMultipleInvitatios"))
      )
      .catch((err) => toastr.error(err));
  }, [resendInvitesAgain]);

  const headerMenu = isAccountsPage
    ? getAccountsHeaderMenu(t)
    : getHeaderMenu(t);
  const menuItems = getMenuItems();

  let tableGroupMenuVisible = headerMenu.length;
  const tableGroupMenuProps = {
    checkboxOptions: menuItems,
    onChange,
    headerMenu,
    isInfoPanelVisible,
    toggleInfoPanel: onToggleInfoPanel,
    isMobileView: isMobileOnly,
  };

  if (isAccountsPage) {
    tableGroupMenuVisible =
      isAccountsHeaderVisible &&
      tableGroupMenuVisible &&
      headerMenu.some((x) => !x.disabled);
    tableGroupMenuProps.isChecked = isAccountsHeaderChecked;
    tableGroupMenuProps.isIndeterminate = isAccountsHeaderIndeterminate;
    tableGroupMenuProps.withoutInfoPanelToggler = false;
  } else {
    tableGroupMenuVisible = isHeaderVisible && tableGroupMenuVisible;
    tableGroupMenuProps.isChecked = isHeaderChecked;
    tableGroupMenuProps.isIndeterminate = isHeaderIndeterminate;
    tableGroupMenuProps.isBlocked = isGroupMenuBlocked;
  }

  const fromAccounts = location?.state?.fromAccounts;
  const fromSettings = location?.state?.fromSettings;

  const isRoot =
    pathParts === null && (fromAccounts || fromSettings)
      ? true
      : isRootFolder || isAccountsPage || isSettingsPage;
  const currentTitle =
    isSettingsPage || (!title && fromSettings)
      ? t("Common:Settings")
      : isAccountsPage || (!title && fromAccounts)
      ? t("Common:Accounts")
      : title;

  return (
    <Consumer key="header">
      {(context) => (
        <StyledContainer
          isRecycleBinFolder={isRecycleBinFolder}
          hideContextMenuInsideArchiveRoom={hideContextMenuInsideArchiveRoom}
        >
          {tableGroupMenuVisible ? (
            <TableGroupMenu {...tableGroupMenuProps} />
          ) : (
            <div className="header-container">
              <Navigation
                sectionWidth={context.sectionWidth}
                showText={showText}
                isRootFolder={isRoot}
                canCreate={
                  security?.Create || isAccountsPage || !isSettingsPage
                }
                title={currentTitle}
                isDesktop={isDesktop}
                isTabletView={isTabletView}
                personal={personal}
                tReady={tReady}
                menuItems={menuItems}
                navigationItems={navigationPath}
                getContextOptionsPlus={getContextOptionsPlus}
                getContextOptionsFolder={getContextOptionsFolder}
                onClose={onClose}
                onClickFolder={onClickFolder}
                isTrashFolder={isRecycleBinFolder}
                isRecycleBinFolder={isRecycleBinFolder || isArchiveFolder}
                isEmptyFilesList={
                  isArchiveFolder ? isEmptyArchive : isEmptyFilesList
                }
                clearTrash={onEmptyTrashAction}
                onBackToParentFolder={onClickBack}
                toggleInfoPanel={onToggleInfoPanel}
                isInfoPanelVisible={isInfoPanelVisible}
                titles={{
                  trash: t("EmptyRecycleBin"),
                  trashWarning: t("TrashErasureWarning"),
                  actions: isRoomsFolder
                    ? t("Files:NewRoom")
                    : t("Common:Actions"),
                  contextMenu: t("Translations:TitleShowFolderActions"),
                  infoPanel: t("Common:InfoPanel"),
                }}
                withMenu={!isRoomsFolder}
                onPlusClick={onCreateRoom}
                isEmptyPage={isEmptyPage}
                isRoom={isRoom}
                hideInfoPanel={isSettingsPage}
              />
            </div>
          )}
        </StyledContainer>
      )}
    </Consumer>
  );
};

export default inject(
  ({
    auth,
    filesStore,
    peopleStore,
    dialogsStore,
    selectedFolderStore,
    treeFoldersStore,
    filesActionsStore,
    settingsStore,

    contextOptionsStore,
  }) => {
    const { isOwner, isAdmin } = auth.userStore.user;

    const {
      setSelected,

      isHeaderVisible,
      isHeaderIndeterminate,
      isHeaderChecked,
      isThirdPartySelection,
      cbMenuItems,
      getCheckboxItemLabel,
      getCheckboxItemId,
      isEmptyFilesList,
      getFolderInfo,
      setBufferSelection,
      setIsLoading,
      fetchFiles,
      fetchRooms,
      activeFiles,
      activeFolders,

      setAlreadyFetchingRooms,

      roomsForRestore,
      roomsForDelete,

      isEmptyPage,
    } = filesStore;

    const {
      setSharingPanelVisible,
      setMoveToPanelVisible,
      setCopyPanelVisible,
      setDeleteDialogVisible,
      setEmptyTrashDialogVisible,
      setSelectFileDialogVisible,
      setIsFolderActions,
      setRestoreAllPanelVisible,
      setRestoreRoomDialogVisible,
      setRestoreAllArchive,
      setInvitePanelOptions,
      setInviteUsersWarningDialogVisible,
    } = dialogsStore;

    const {
      isRecycleBinFolder,
      isPrivacyFolder,
      isRoomsFolder,
      isArchiveFolder,
      isPersonalRoom,
      isArchiveFolderRoot,
    } = treeFoldersStore;

    const {
      deleteAction,
      downloadAction,
      getHeaderMenu,
      isGroupMenuBlocked,
      moveToRoomsPage,
      onClickBack,
      emptyTrashInProgress,
    } = filesActionsStore;

    const { setIsVisible, isVisible } = auth.infoPanelStore;

    const { title, id, roomType, pathParts, navigationPath, security } =
      selectedFolderStore;

    const selectedFolder = { ...selectedFolderStore };

    const { enablePlugins } = auth.settingsStore;
    const { isGracePeriod } = auth.currentTariffStatusStore;

    const isRoom = !!roomType;

    const {
      onClickEditRoom,
      onClickInviteUsers,
      onShowInfoPanel,
      onClickArchive,
      onClickReconnectStorage,
      onCopyLink,
    } = contextOptionsStore;

    const canRestoreAll = isArchiveFolder && roomsForRestore.length > 0;

    const canDeleteAll = isArchiveFolder && roomsForDelete.length > 0;

    const isEmptyArchive = !canRestoreAll && !canDeleteAll;

    const hideContextMenuInsideArchiveRoom = isArchiveFolderRoot
      ? !isArchiveFolder
      : false;

    const {
      selectionStore,
      headerMenuStore,
      getHeaderMenu: getAccountsHeaderMenu,
    } = peopleStore;

    const {
      isHeaderVisible: isAccountsHeaderVisible,
      isHeaderIndeterminate: isAccountsHeaderIndeterminate,
      isHeaderChecked: isAccountsHeaderChecked,
      cbMenuItems: accountsCbMenuItems,
      getMenuItemId: getAccountsMenuItemId,
      getCheckboxItemLabel: getAccountsCheckboxItemLabel,
    } = headerMenuStore;

    const { setSelected: setAccountsSelected } = selectionStore;

    return {
      isGracePeriod,
      setInviteUsersWarningDialogVisible,
      showText: auth.settingsStore.showText,
      isDesktop: auth.settingsStore.isDesktopClient,

      isRootFolder: pathParts?.length === 1,
      isPersonalRoom,
      title,
      isRoom,
      currentFolderId: id,
      pathParts: pathParts,
      navigationPath: navigationPath,

      setIsInfoPanelVisible: setIsVisible,
      isInfoPanelVisible: isVisible,
      isHeaderVisible,
      isHeaderIndeterminate,
      isHeaderChecked,
      isThirdPartySelection,
      isTabletView: auth.settingsStore.isTabletView,
      confirmDelete: settingsStore.confirmDelete,
      personal: auth.settingsStore.personal,
      cbMenuItems,
      setSelectedNode: treeFoldersStore.setSelectedNode,
      getFolderInfo,

      setSelected,
      security,

      setSharingPanelVisible,
      setMoveToPanelVisible,
      setCopyPanelVisible,
      setBufferSelection,
      setIsFolderActions,
      deleteAction,
      setDeleteDialogVisible,
      downloadAction,
      getHeaderMenu,
      getCheckboxItemLabel,
      getCheckboxItemId,
      setSelectFileDialogVisible,

      isRecycleBinFolder,
      setEmptyTrashDialogVisible,
      isEmptyFilesList,
      isEmptyArchive,
      isPrivacyFolder,
      isArchiveFolder,
      hideContextMenuInsideArchiveRoom,

      setIsLoading,
      fetchFiles,
      fetchRooms,

      activeFiles,
      activeFolders,

      isRoomsFolder,

      setAlreadyFetchingRooms,

      enablePlugins,

      setRestoreAllPanelVisible,

      setRestoreRoomDialogVisible,
      setRestoreAllArchive,

      selectedFolder,

      onClickEditRoom,
      onClickInviteUsers,
      onShowInfoPanel,
      onClickArchive,
      onCopyLink,

      isEmptyArchive,
      canRestoreAll,
      canDeleteAll,
      isGroupMenuBlocked,

      moveToRoomsPage,
      onClickBack,

      getAccountsHeaderMenu,
      isAccountsHeaderVisible,
      isAccountsHeaderIndeterminate,
      isAccountsHeaderChecked,
      accountsCbMenuItems,
      getAccountsMenuItemId,
      getAccountsCheckboxItemLabel,
      setAccountsSelected,
      isOwner,
      isAdmin,
      setInvitePanelOptions,
      isEmptyPage,
      emptyTrashInProgress,
    };
  }
)(
  withTranslation([
    "Files",
    "Common",
    "Translations",
    "InfoPanel",
    "SharingPanel",
    "Article",
    "People",
    "PeopleTranslations",
    "ChangeUserTypeDialog",
  ])(withLoader(observer(SectionHeaderContent))(<Loaders.SectionHeader />))
);
