import React from "react";
import copy from "copy-to-clipboard";
import styled, { css } from "styled-components";
import { withRouter } from "react-router";
import toastr from "@docspace/components/toast/toastr";
import Loaders from "@docspace/common/components/Loaders";
import {
  AppServerConfig,
  FolderType,
  RoomSearchArea,
} from "@docspace/common/constants";
import { withTranslation } from "react-i18next";
import { isMobile, isTablet, isMobileOnly } from "react-device-detect";
import DropDownItem from "@docspace/components/drop-down-item";
import { tablet, mobile } from "@docspace/components/utils/device";
import { Consumer } from "@docspace/components/utils/context";
import { inject, observer } from "mobx-react";
import TableGroupMenu from "@docspace/components/table-container/TableGroupMenu";
import Navigation from "@docspace/common/components/Navigation";
import { Events } from "@docspace/common/constants";
import config from "PACKAGE_FILE";
import { combineUrl } from "@docspace/common/utils";
import RoomsFilter from "@docspace/common/api/rooms/filter";
import { getMainButtonItems } from "SRC_DIR/helpers/plugins";

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

    @media ${tablet} {
      height: 60px;
    }
  }
`;

class SectionHeaderContent extends React.Component {
  constructor(props) {
    super(props);
    this.state = { navigationItems: [] };
  }

  onCreate = (format) => {
    const event = new Event(Events.CREATE);

    const payload = {
      extension: format,
      id: -1,
    };

    event.payload = payload;

    window.dispatchEvent(event);
  };

  onCreateRoom = () => {
    const event = new Event(Events.ROOM_CREATE);
    window.dispatchEvent(event);
  };

  createDocument = () => this.onCreate("docx");

  createSpreadsheet = () => this.onCreate("xlsx");

  createPresentation = () => this.onCreate("pptx");

  createForm = () => this.onCreate("docxf");

  createFormFromFile = () => {
    this.props.setSelectFileDialogVisible(true);
  };

  onShowGallery = () => {
    const { history, currentFolderId } = this.props;
    history.push(
      combineUrl(
        AppServerConfig.proxyURL,
        config.homepage,
        `/form-gallery/${currentFolderId}/`
      )
    );
  };

  createFolder = () => this.onCreate();

  uploadToFolder = () => console.log("Upload To Folder click");

  getContextOptionsPlus = () => {
    const { t, isPrivacyFolder, isRoomsFolder, enablePlugins } = this.props;

    const options = isRoomsFolder
      ? [
          {
            key: "new-room",
            label: t("NewRoom"),
            onClick: this.onCreateRoom,
            icon: "images/folder.locked.react.svg",
          },
        ]
      : [
          {
            id: "personal_new-documnet",
            key: "new-document",
            label: t("NewDocument"),
            onClick: this.createDocument,
            icon: "images/actions.documents.react.svg",
          },
          {
            id: "personal_new-spreadsheet",
            key: "new-spreadsheet",
            label: t("NewSpreadsheet"),
            onClick: this.createSpreadsheet,
            icon: "images/spreadsheet.react.svg",
          },
          {
            id: "personal_new-presentation",
            key: "new-presentation",
            label: t("NewPresentation"),
            onClick: this.createPresentation,
            icon: "images/actions.presentation.react.svg",
          },
          {
            id: "personal_form-template",
            icon: "images/form.react.svg",
            label: t("Translations:NewForm"),
            key: "new-form-base",
            items: [
              {
                id: "personal_template_black",
                key: "new-form",
                label: t("Translations:SubNewForm"),
                icon: "images/form.blank.react.svg",
                onClick: this.createForm,
              },
              {
                id: "personal_template_new-form-file",
                key: "new-form-file",
                label: t("Translations:SubNewFormFile"),
                icon: "images/form.file.react.svg",
                onClick: this.createFormFromFile,
                disabled: isPrivacyFolder,
              },
              {
                id: "personal_template_oforms-gallery",
                key: "oforms-gallery",
                label: t("Common:OFORMsGallery"),
                icon: "images/form.gallery.react.svg",
                onClick: this.onShowGallery,
                disabled: isPrivacyFolder || (isMobile && isTablet),
              },
            ],
          },
          {
            id: "personal_new-folder",
            key: "new-folder",
            label: t("NewFolder"),
            onClick: this.createFolder,
            icon: "images/catalog.folder.react.svg",
          },
          /*{ key: "separator", isSeparator: true },
      {
        key: "upload-to-folder",
        label: t("UploadToFolder"),
        onClick: this.uploadToFolder,
        disabled: true,
        icon: "images/actions.upload.react.svg",
      },*/
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

  createLinkForPortalUsers = () => {
    const { currentFolderId } = this.props;
    const { t } = this.props;

    copy(
      `${window.location.origin}/filter?folder=${currentFolderId}` //TODO: Change url by category
    );

    toastr.success(t("Translations:LinkCopySuccess"));
  };

  onMoveAction = () => {
    this.props.setIsFolderActions(true);
    this.props.setBufferSelection(this.props.selectedFolder);
    return this.props.setMoveToPanelVisible(true);
  };
  onCopyAction = () => {
    this.props.setIsFolderActions(true);
    this.props.setBufferSelection(this.props.currentFolderId);
    return this.props.setCopyPanelVisible(true);
  };
  downloadAction = () => {
    this.props.setBufferSelection(this.props.currentFolderId);
    this.props.setIsFolderActions(true);
    this.props
      .downloadAction(this.props.t("Translations:ArchivingData"), [
        this.props.currentFolderId,
      ])
      .catch((err) => toastr.error(err));
  };

  renameAction = () => {
    const { selectedFolder } = this.props;

    const event = new Event(Events.RENAME);

    event.item = selectedFolder;

    window.dispatchEvent(event);
  };

  onOpenSharingPanel = () => {
    this.props.setBufferSelection(this.props.currentFolderId);
    this.props.setIsFolderActions(true);
    return this.props.setSharingPanelVisible(true);
  };

  onDeleteAction = () => {
    const {
      t,
      deleteAction,
      confirmDelete,
      setDeleteDialogVisible,
      isThirdPartySelection,
      currentFolderId,
      getFolderInfo,
      setBufferSelection,
    } = this.props;

    this.props.setIsFolderActions(true);

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

  onEmptyTrashAction = () => {
    const { activeFiles, activeFolders } = this.props;

    const isExistActiveItems = [...activeFiles, ...activeFolders].length > 0;

    if (isExistActiveItems) return;

    this.props.setEmptyTrashDialogVisible(true);
  };

  onRestoreAllAction = () => {
    const { activeFiles, activeFolders } = this.props;
    const isExistActiveItems = [...activeFiles, ...activeFolders].length > 0;

    if (isExistActiveItems) return;

    this.props.setRestoreAllPanelVisible(true);
  };

  onRestoreAllArchiveAction = () => {
    const { activeFiles, activeFolders } = this.props;
    const isExistActiveItems = [...activeFiles, ...activeFolders].length > 0;

    if (isExistActiveItems) return;

    this.props.setArchiveAction("unarchive");
    this.props.setRestoreAllArchive(true);
    this.props.setArchiveDialogVisible(true);
  };

  onShowInfo = () => {
    const { setIsInfoPanelVisible } = this.props;
    setIsInfoPanelVisible(true);
  };

  onToggleInfoPanel = () => {
    const { isInfoPanelVisible, setIsInfoPanelVisible } = this.props;
    setIsInfoPanelVisible(!isInfoPanelVisible);
  };

  onCopyLinkAction = () => {
    const { t, selectedFolder, onCopyLink } = this.props;

    onCopyLink && onCopyLink({ ...selectedFolder, isFolder: true }, t);
  };

  getContextOptionsFolder = () => {
    const {
      t,
      isRoom,
      isRecycleBinFolder,
      isArchiveFolder,
      isPersonalRoom,

      selectedFolder,

      onClickEditRoom,
      onClickInviteUsers,
      onShowInfoPanel,
      onClickArchive,
      onClickReconnectStorage,

      canRestoreAll,
      canDeleteAll,
    } = this.props;

    const isDisabled = isRecycleBinFolder || isRoom;

    if (isArchiveFolder) {
      return [
        {
          id: "header_option_empty-archive",
          key: "empty-archive",
          label: t("ArchiveAction"),
          onClick: this.onEmptyTrashAction,
          disabled: !canDeleteAll,
          icon: "images/clear.trash.react.svg",
        },
        {
          id: "header_option_restore-all",
          key: "restore-all",
          label: t("RestoreAll"),
          onClick: this.onRestoreAllArchiveAction,
          disabled: !canRestoreAll,
          icon: "images/subtract.react.svg",
        },
      ];
    }

    return [
      {
        id: "header_option_sharing-settings",
        key: "sharing-settings",
        label: t("SharingPanel:SharingSettingsTitle"),
        onClick: this.onOpenSharingPanel,
        disabled: true,
        icon: "/static/images/share.react.svg",
      },
      {
        id: "header_option_link-portal-users",
        key: "link-portal-users",
        label: t("LinkForPortalUsers"),
        onClick: this.createLinkForPortalUsers,
        disabled: true,
        icon: "/static/images/invitation.link.react.svg",
      },
      {
        id: "header_option_link-for-room-members",
        key: "link-for-room-members",
        label: t("LinkForRoomMembers"),
        onClick: this.onCopyLinkAction,
        disabled: isRecycleBinFolder || isPersonalRoom,
        icon: "/static/images/invitation.link.react.svg",
      },
      {
        id: "header_option_empty-trash",
        key: "empty-trash",
        label: t("RecycleBinAction"),
        onClick: this.onEmptyTrashAction,
        disabled: !isRecycleBinFolder,
        icon: "images/clear.trash.react.svg",
      },
      {
        id: "header_option_restore-all",
        key: "restore-all",
        label: t("RestoreAll"),
        onClick: this.onRestoreAllAction,
        disabled: !isRecycleBinFolder,
        icon: "images/subtract.react.svg",
      },
      {
        id: "header_option_show-info",
        key: "show-info",
        label: t("InfoPanel:ViewDetails"),
        onClick: this.onShowInfo,
        disabled: isDisabled,
        icon: "/static/images/info.outline.react.svg",
      },
      {
        id: "header_option_reconnect-storage",
        key: "reconnect-storage",
        label: t("Common:ReconnectStorage"),
        icon: "images/reconnect.svg",
        onClick: () => onClickReconnectStorage(selectedFolder, t),
        disabled: !selectedFolder.providerKey || !isRoom,
      },
      {
        id: "header_option_edit-room",
        key: "edit-room",
        label: t("EditRoom"),
        icon: "images/settings.react.svg",
        onClick: () => onClickEditRoom(selectedFolder),
        disabled: !isRoom,
      },
      {
        id: "header_option_invite-users-to-room",
        key: "invite-users-to-room",
        label: t("Common:InviteUsers"),
        icon: "/static/images/person.react.svg",
        onClick: () => onClickInviteUsers(selectedFolder.id),
        disabled: !isRoom,
      },
      {
        id: "header_option_room-info",
        key: "room-info",
        label: t("Common:Info"),
        icon: "/static/images/info.outline.react.svg",
        onClick: this.onToggleInfoPanel,
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
        label: t("Archived"),
        icon: "/static/images/room.archive.svg",
        onClick: (e) => onClickArchive(e, selectedFolder, t),
        disabled: !isRoom,
        "data-action": "archive",
        action: "archive",
      },
      {
        id: "header_option_download",
        key: "download",
        label: t("Common:Download"),
        onClick: this.downloadAction,
        disabled: isDisabled,
        icon: "images/download.react.svg",
      },
      {
        id: "header_option_move-to",
        key: "move-to",
        label: t("MoveTo"),
        onClick: this.onMoveAction,
        disabled: isDisabled,
        icon: "images/move.react.svg",
      },
      {
        id: "header_option_copy",
        key: "copy",
        label: t("Translations:Copy"),
        onClick: this.onCopyAction,
        disabled: isDisabled,
        icon: "/static/images/copy.react.svg",
      },
      {
        id: "header_option_rename",
        key: "rename",
        label: t("Rename"),
        onClick: this.renameAction,
        disabled: isDisabled,
        icon: "images/rename.react.svg",
      },
      {
        id: "header_option_separator-3",
        key: "separator-3",
        isSeparator: true,
        disabled: isDisabled,
      },
      {
        id: "header_option_delete",
        key: "delete",
        label: t("Common:Delete"),
        onClick: this.onDeleteAction,
        disabled: isDisabled,
        icon: "/static/images/catalog.trash.react.svg",
      },
    ];
  };

  onBackToParentFolder = () => {
    if (this.props.isRoom) {
      return this.moveToRoomsPage();
    }

    this.props.backToParentFolder();
  };

  onSelect = (e) => {
    const key = e.currentTarget.dataset.key;
    this.props.setSelected(key);
  };

  onClose = () => {
    this.props.setSelected("close");
  };

  getMenuItems = () => {
    const {
      t,
      cbMenuItems,
      getCheckboxItemLabel,
      getCheckboxItemId,
    } = this.props;
    const checkboxOptions = (
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
              onClick={this.onSelect}
            />
          );
        })}
      </>
    );

    return checkboxOptions;
  };

  onChange = (checked) => {
    this.props.setSelected(checked ? "all" : "none");
  };

  onClickFolder = (id, isRootRoom) => {
    const { setSelectedNode, setIsLoading, fetchFiles } = this.props;

    if (isRootRoom) {
      return this.moveToRoomsPage();
    }

    setSelectedNode(id);
    setIsLoading(true);
    fetchFiles(id, null, true, false)
      .catch((err) => toastr.error(err))
      .finally(() => setIsLoading(false));
  };

  moveToRoomsPage = () => {
    const {
      setIsLoading,

      fetchRooms,

      setAlreadyFetchingRooms,

      rootFolderType,
    } = this.props;

    setIsLoading(true);

    setAlreadyFetchingRooms(true);

    const filter = RoomsFilter.getDefault();

    if (rootFolderType === FolderType.Archive) {
      filter.searchArea = RoomSearchArea.Archive;
    }

    fetchRooms(null, filter).finally(() => {
      setIsLoading(false);
    });
  };

  render() {
    //console.log("Body header render");

    const {
      t,
      tReady,
      isInfoPanelVisible,
      isRootFolder,
      title,
      canCreate,
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
      isRoomsFolder,
      isEmptyPage,
      canCreateFiles,
      isEmptyArchive,
      isVisitor,
      isRoom,
    } = this.props;

    const menuItems = this.getMenuItems();
    const isLoading = !title || !tReady;
    const headerMenu = getHeaderMenu(t);

    return (
      <Consumer>
        {(context) => (
          <StyledContainer>
            {isHeaderVisible && headerMenu.length ? (
              <TableGroupMenu
                checkboxOptions={menuItems}
                onChange={this.onChange}
                isChecked={isHeaderChecked}
                isIndeterminate={isHeaderIndeterminate}
                headerMenu={headerMenu}
                isInfoPanelVisible={isInfoPanelVisible}
                toggleInfoPanel={this.onToggleInfoPanel}
              />
            ) : (
              <div className="header-container">
                {isLoading ? (
                  <Loaders.SectionHeader />
                ) : (
                  <Navigation
                    sectionWidth={context.sectionWidth}
                    showText={showText}
                    isRootFolder={isRootFolder}
                    canCreate={
                      canCreate &&
                      !isVisitor &&
                      (canCreateFiles || isRoomsFolder)
                    }
                    title={title}
                    isDesktop={isDesktop}
                    isTabletView={isTabletView}
                    personal={personal}
                    tReady={tReady}
                    menuItems={menuItems}
                    navigationItems={navigationPath}
                    getContextOptionsPlus={this.getContextOptionsPlus}
                    getContextOptionsFolder={this.getContextOptionsFolder}
                    onClose={this.onClose}
                    onClickFolder={this.onClickFolder}
                    isRecycleBinFolder={isRecycleBinFolder || isArchiveFolder}
                    isEmptyFilesList={
                      isArchiveFolder ? isEmptyArchive : isEmptyFilesList
                    }
                    clearTrash={this.onEmptyTrashAction}
                    onBackToParentFolder={this.onBackToParentFolder}
                    toggleInfoPanel={this.onToggleInfoPanel}
                    isInfoPanelVisible={isInfoPanelVisible}
                    titles={{
                      trash: t("EmptyRecycleBin"),
                    }}
                    withMenu={!isRoomsFolder}
                    onPlusClick={this.onCreateRoom}
                    isEmptyPage={isEmptyPage}
                    isRoom={isRoom}
                  />
                )}
              </div>
            )}
          </StyledContainer>
        )}
      </Consumer>
    );
  }
}

export default inject(
  ({
    auth,
    filesStore,

    dialogsStore,
    selectedFolderStore,
    treeFoldersStore,
    filesActionsStore,
    settingsStore,
    accessRightsStore,
    contextOptionsStore,
  }) => {
    const {
      setSelected,
      canCreate,
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
      setArchiveDialogVisible,
      setRestoreAllArchive,
      setArchiveAction,
    } = dialogsStore;

    const {
      isRecycleBinFolder,
      isPrivacyFolder,
      isRoomsFolder,
      isArchiveFolder,
      isPersonalRoom,
    } = treeFoldersStore;

    const {
      deleteAction,
      downloadAction,
      getHeaderMenu,
      backToParentFolder,
    } = filesActionsStore;

    const { setIsVisible, isVisible } = auth.infoPanelStore;

    const {
      title,
      id,
      roomType,
      pathParts,
      navigationPath,
      rootFolderType,
    } = selectedFolderStore;

    const selectedFolder = { ...selectedFolderStore };

    const { enablePlugins } = auth.settingsStore;

    const isRoom = !!roomType;

    const {
      onClickEditRoom,
      onClickInviteUsers,
      onShowInfoPanel,
      onClickArchive,
      onClickReconnectStorage,
      onCopyLink,
    } = contextOptionsStore;

    const { canCreateFiles } = accessRightsStore;

    const canRestoreAll = isArchiveFolder && roomsForRestore.length > 0;

    const canDeleteAll = isArchiveFolder && roomsForDelete.length > 0;

    const isEmptyArchive = !canRestoreAll && !canDeleteAll;

    return {
      showText: auth.settingsStore.showText,
      isDesktop: auth.settingsStore.isDesktopClient,
      isVisitor: auth.userStore.user.isVisitor,
      isRootFolder: pathParts?.length === 1,
      isPersonalRoom,
      title,
      isRoom,
      currentFolderId: id,
      pathParts: pathParts,
      navigationPath: navigationPath,
      canCreate,
      canCreateFiles,
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

      setSharingPanelVisible,
      setMoveToPanelVisible,
      setCopyPanelVisible,
      setBufferSelection,
      setIsFolderActions,
      deleteAction,
      setDeleteDialogVisible,
      downloadAction,
      getHeaderMenu,
      backToParentFolder,
      getCheckboxItemLabel,
      getCheckboxItemId,
      setSelectFileDialogVisible,

      isRecycleBinFolder,
      setEmptyTrashDialogVisible,
      isEmptyFilesList,
      isEmptyArchive,
      isPrivacyFolder,
      isArchiveFolder,

      setIsLoading,
      fetchFiles,
      fetchRooms,

      activeFiles,
      activeFolders,

      isRoomsFolder,

      setAlreadyFetchingRooms,

      enablePlugins,

      setRestoreAllPanelVisible,
      isEmptyPage,
      setArchiveDialogVisible,
      setRestoreAllArchive,
      setArchiveAction,

      selectedFolder,

      onClickEditRoom,
      onClickInviteUsers,
      onShowInfoPanel,
      onClickArchive,
      onCopyLink,

      rootFolderType,

      isEmptyArchive,
      canRestoreAll,
      canDeleteAll,
    };
  }
)(
  withTranslation([
    "Files",
    "Common",
    "Translations",
    "InfoPanel",
    "SharingPanel",
  ])(withRouter(observer(SectionHeaderContent)))
);
