import React from "react";
import copy from "copy-to-clipboard";
import styled, { css } from "styled-components";
import { withRouter } from "react-router";
import toastr from "studio/toastr";
import Loaders from "@appserver/common/components/Loaders";
import {
  AppServerConfig,
  FileAction,
  FolderType,
  RoomSearchArea,
} from "@appserver/common/constants";
import { withTranslation } from "react-i18next";
import { isMobile, isTablet } from "react-device-detect";
import DropDownItem from "@appserver/components/drop-down-item";
import { tablet } from "@appserver/components/utils/device";
import { Consumer } from "@appserver/components/utils/context";
import { inject, observer } from "mobx-react";
import TableGroupMenu from "@appserver/components/table-container/TableGroupMenu";
import Navigation from "@appserver/common/components/Navigation";
import { Events } from "../../../../helpers/constants";
import config from "../../../../../package.json";
import { combineUrl } from "@appserver/common/utils";
import RoomsFilter from "@appserver/common/api/rooms/filter";

const StyledContainer = styled.div`
  .table-container_group-menu {
    ${(props) =>
      props.viewAs === "table"
        ? css`
            margin: 0px -20px;
            width: calc(100% + 40px);
          `
        : css`
            margin: 0px -20px;
            width: calc(100% + 40px);
          `}

    @media ${tablet} {
      margin: 0 -16px;
      width: calc(100% + 32px);
    }

    ${isMobile &&
    css`
      margin: 0 -16px;
      width: calc(100% + 32px);
    `}
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
    const { setSelectFileDialogVisible } = this.props;
    setSelectFileDialogVisible(true);
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
    const { t, isPrivacyFolder, isRoomsFolder } = this.props;

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
            key: "new-document",
            label: t("NewDocument"),
            onClick: this.createDocument,
            icon: "images/actions.documents.react.svg",
          },
          {
            key: "new-spreadsheet",
            label: t("NewSpreadsheet"),
            onClick: this.createSpreadsheet,
            icon: "images/spreadsheet.react.svg",
          },
          {
            key: "new-presentation",
            label: t("NewPresentation"),
            onClick: this.createPresentation,
            icon: "images/actions.presentation.react.svg",
          },
          {
            icon: "images/form.react.svg",
            label: t("Translations:NewForm"),
            key: "new-form-base",
            items: [
              {
                key: "new-form",
                label: t("Translations:SubNewForm"),
                icon: "images/form.blank.react.svg",
                onClick: this.createForm,
              },
              {
                key: "new-form-file",
                label: t("Translations:SubNewFormFile"),
                icon: "images/form.file.react.svg",
                onClick: this.createFormFromFile,
                disabled: isPrivacyFolder,
              },
              {
                key: "oforms-gallery",
                label: t("Common:OFORMsGallery"),
                icon: "images/form.gallery.react.svg",
                onClick: this.onShowGallery,
                disabled: isPrivacyFolder || (isMobile && isTablet),
              },
            ],
          },
          {
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

    return options;
  };

  createLinkForPortalUsers = () => {
    const { currentFolderId } = this.props;
    const { t } = this.props;

    copy(
      `${window.location.origin}/products/files/filter?folder=${currentFolderId}`
    );

    toastr.success(t("Translations:LinkCopySuccess"));
  };

  onMoveAction = () => {
    this.props.setIsFolderActions(true);
    this.props.setBufferSelection(this.props.currentFolderId);
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

  renameAction = () => console.log("renameAction click");
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

  getContextOptionsFolder = () => {
    const { t, toggleInfoPanel, personal } = this.props;

    return [
      {
        key: "sharing-settings",
        label: t("SharingPanel:SharingSettingsTitle"),
        onClick: this.onOpenSharingPanel,
        disabled: personal ? true : false,
        icon: "/static/images/share.react.svg",
      },
      {
        key: "link-portal-users",
        label: t("LinkForPortalUsers"),
        onClick: this.createLinkForPortalUsers,
        disabled: personal ? true : false,
        icon: "/static/images/invitation.link.react.svg",
      },
      {
        key: "show-info",
        label: t("InfoPanel:ViewDetails"),
        onClick: toggleInfoPanel,
        disabled: false,
        icon: "/static/images/info.react.svg",
      },
      { key: "separator-2", isSeparator: true },
      {
        key: "move-to",
        label: t("MoveTo"),
        onClick: this.onMoveAction,
        disabled: false,
        icon: "images/move.react.svg",
      },
      {
        key: "copy",
        label: t("Translations:Copy"),
        onClick: this.onCopyAction,
        disabled: false,
        icon: "/static/images/copy.react.svg",
      },
      {
        key: "download",
        label: t("Common:Download"),
        onClick: this.downloadAction,
        disabled: false,
        icon: "images/download.react.svg",
      },
      {
        key: "rename",
        label: t("Rename"),
        onClick: this.renameAction,
        disabled: true,
        icon: "images/rename.react.svg",
      },
      {
        key: "delete",
        label: t("Common:Delete"),
        onClick: this.onDeleteAction,
        disabled: false,
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
    const { t, cbMenuItems, getCheckboxItemLabel } = this.props;

    const checkboxOptions = (
      <>
        {cbMenuItems.map((key) => {
          const label = getCheckboxItemLabel(t, key);
          return (
            <DropDownItem
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
      history,

      setAlreadyFetchingRooms,
    } = this.props;

    setIsLoading(true);

    setAlreadyFetchingRooms(true);

    fetchRooms(null, null)
      .then(() => {
        const filter = RoomsFilter.getDefault();

        const urlFilter = filter.toUrlParams();

        history.push(
          combineUrl(
            AppServerConfig.proxyURL,
            config.homepage,
            `/rooms?${urlFilter}`
          )
        );
      })
      .finally(() => {
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
      viewAs,
      isRecycleBinFolder,
      isEmptyFilesList,
      isHeaderVisible,
      isHeaderChecked,
      isHeaderIndeterminate,
      showText,
      toggleInfoPanel,
    } = this.props;
    const menuItems = this.getMenuItems();
    const isLoading = !title || !tReady;
    const headerMenu = getHeaderMenu(t);

    return (
      <Consumer>
        {(context) => (
          <StyledContainer
            width={context.sectionWidth}
            isRootFolder={isRootFolder}
            canCreate={canCreate}
            isRecycleBinFolder={isRecycleBinFolder}
            isTitle={title}
            isDesktop={isDesktop}
            isTabletView={isTabletView}
            isLoading={isLoading}
            viewAs={viewAs}
          >
            {isHeaderVisible ? (
              <TableGroupMenu
                checkboxOptions={menuItems}
                onChange={this.onChange}
                isChecked={isHeaderChecked}
                isIndeterminate={isHeaderIndeterminate}
                headerMenu={headerMenu}
                isInfoPanelVisible={isInfoPanelVisible}
                toggleInfoPanel={toggleInfoPanel}
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
                    canCreate={canCreate}
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
                    isRecycleBinFolder={isRecycleBinFolder}
                    isEmptyFilesList={isEmptyFilesList}
                    clearTrash={this.onEmptyTrashAction}
                    onBackToParentFolder={this.onBackToParentFolder}
                    toggleInfoPanel={toggleInfoPanel}
                    isInfoPanelVisible={isInfoPanelVisible}
                    titles={{
                      trash: t("EmptyRecycleBin"),
                    }}
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
  }) => {
    const {
      setSelected,
      setSelection,

      canCreate,
      isHeaderVisible,
      isHeaderIndeterminate,
      isHeaderChecked,
      isThirdPartySelection,
      cbMenuItems,
      getCheckboxItemLabel,
      isEmptyFilesList,
      getFolderInfo,
      setBufferSelection,
      viewAs,
      setIsLoading,
      fetchFiles,
      fetchRooms,
      activeFiles,
      activeFolders,

      setAlreadyFetchingRooms,
    } = filesStore;

    const {
      setSharingPanelVisible,
      setMoveToPanelVisible,
      setCopyPanelVisible,
      setDeleteDialogVisible,
      setEmptyTrashDialogVisible,
      setSelectFileDialogVisible,
      setIsFolderActions,
    } = dialogsStore;

    const {
      isRecycleBinFolder,
      isPrivacyFolder,
      isRoomsFolder,
      isArchiveFolder,
    } = treeFoldersStore;
    const {
      deleteAction,
      downloadAction,
      getHeaderMenu,
      backToParentFolder,
    } = filesActionsStore;

    const { toggleIsVisible, isVisible } = auth.infoPanelStore;

    const {
      title,
      id,
      roomType,
      rootFolderType,
      pathParts,
      navigationPath,
    } = selectedFolderStore;

    const isRoom = !!roomType;

    return {
      showText: auth.settingsStore.showText,
      isDesktop: auth.settingsStore.isDesktopClient,
      isRootFolder: pathParts?.length === 1,
      title,
      isRoom,
      rootFolderType,
      currentFolderId: id,
      pathParts: pathParts,
      navigationPath: navigationPath,
      canCreate,
      toggleInfoPanel: toggleIsVisible,
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
      setSelection,

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
      setSelectFileDialogVisible,

      isRecycleBinFolder,
      setEmptyTrashDialogVisible,
      isEmptyFilesList,
      isPrivacyFolder,
      viewAs,

      setIsLoading,
      fetchFiles,
      fetchRooms,

      activeFiles,
      activeFolders,

      isRoomsFolder,
      isArchiveFolder,

      setAlreadyFetchingRooms,
    };
  }
)(
  withTranslation([
    "Home",
    "Common",
    "Translations",
    "InfoPanel",
    "SharingPanel",
  ])(withRouter(observer(SectionHeaderContent)))
);
