import React from "react";
import copy from "copy-to-clipboard";
import styled, { css } from "styled-components";
import { withRouter } from "react-router";
import toastr from "studio/toastr";
import Loaders from "@appserver/common/components/Loaders";
import Headline from "@appserver/common/components/Headline";
import { FilterType, FileAction } from "@appserver/common/constants";
import { withTranslation } from "react-i18next";
import { isMobile } from "react-device-detect";
import ContextMenuButton from "@appserver/components/context-menu-button";
import DropDownItem from "@appserver/components/drop-down-item";
import IconButton from "@appserver/components/icon-button";
import { tablet, desktop } from "@appserver/components/utils/device";
import { Consumer } from "@appserver/components/utils/context";
import { inject, observer } from "mobx-react";
import TableGroupMenu from "@appserver/components/table-container/TableGroupMenu";
import Navigation from "@appserver/common/components/Navigation";

const StyledContainer = styled.div`
  .table-container_group-menu {
    ${(props) =>
      props.viewAs === "table"
        ? css`
            margin: 0px -20px;
            width: calc(100% + 44px);
          `
        : css`
            margin: 0px -24px;
            width: calc(100% + 48px);
          `}

    @media ${tablet} {
      margin: 0 -16px;
      width: calc(100% + 32px);
    }
  }
`;
// .header-container {
//   position: relative;
//   ${(props) =>
//     props.title &&
//     css`
//       display: grid;
//       grid-template-columns: ${(props) =>
//         props.isRootFolder
//           ? "auto auto 1fr"
//           : props.canCreate
//           ? "auto auto auto auto 1fr"
//           : "auto auto auto 1fr"};

//       @media ${tablet} {
//         grid-template-columns: ${(props) =>
//           props.isRootFolder
//             ? "1fr auto"
//             : props.canCreate
//             ? "auto 1fr auto auto"
//             : "auto 1fr auto"};
//         ${(props) => !props.isLoading && "top: 7px;"}
//       }
//     `}
//   align-items: center;
//   max-width: calc(100vw - 32px);

//   @media ${tablet} {
//     .headline-header {
//       margin-left: -1px;
//     }
//   }
//   .arrow-button {
//     margin-right: 15px;
//     min-width: 17px;

//     @media ${tablet} {
//       padding: 8px 0 8px 8px;
//       margin-left: -8px;
//       margin-right: 16px;
//     }
//   }

//   .add-button {
//     margin-bottom: -1px;
//     margin-left: 16px;

//     @media ${tablet} {
//       margin-left: auto;

//       & > div:first-child {
//         padding: 8px 8px 8px 8px;
//         margin-right: -8px;
//       }
//     }
//   }

//   .option-button {
//     margin-bottom: -1px;

//     @media (min-width: 1024px) {
//       margin-left: 8px;
//     }

//     @media ${tablet} {
//       & > div:first-child {
//         padding: 8px 8px 8px 8px;
//         margin-right: -8px;
//       }
//     }
//   }

//   .trash-button {
//     margin-bottom: -1px;

//     @media (min-width: 1024px) {
//       margin-left: 8px;
//     }

//     @media ${tablet} {
//       & > div:first-child {
//         margin-right: -8px;
//       }
//     }
//   }
// }

// .group-button-menu-container {
//   margin: 0 -16px;
//   -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

//   ${isMobile &&
//   css`
//     position: sticky;
//   `}

//   ${(props) =>
//     !props.isTabletView
//       ? props.width &&
//         isMobile &&
//         css`
//           width: ${props.width + 40 + "px"};
//         `
//       : props.width &&
//         isMobile &&
//         css`
//           width: ${props.width + 32 + "px"};
//         `}

//   @media ${tablet} {
//     padding-bottom: 0;
//     ${!isMobile &&
//     css`
//       height: 56px;
//     `}
//     & > div:first-child {
//       ${(props) =>
//         !isMobile &&
//         props.width &&
//         css`
//           width: ${props.width + 16 + "px"};
//         `}

//       position: absolute;
//       ${(props) =>
//         !props.isDesktop &&
//         css`
//           top: 48px;
//         `}
//       z-index: 180;
//     }
//   }

//   @media ${desktop} {
//     margin: 0 -24px;
//   }
// }

class SectionHeaderContent extends React.Component {
  constructor(props) {
    super(props);
    this.state = { navigationItems: [] };
  }

  onCreate = (format) => {
    this.props.setAction({
      type: FileAction.Create,
      extension: format,
      id: -1,
    });
  };

  createDocument = () => this.onCreate("docx");

  createSpreadsheet = () => this.onCreate("xlsx");

  createPresentation = () => this.onCreate("pptx");

  createForm = () => this.onCreate("docxf");

  createFormFromFile = () => {
    const { setSelectFileDialogVisible } = this.props;
    setSelectFileDialogVisible(true);
  };

  createFolder = () => this.onCreate();

  uploadToFolder = () => console.log("Upload To Folder click");

  getContextOptionsPlus = () => {
    const { t, isPrivacyFolder } = this.props;

    return [
      {
        key: "new-document",
        label: t("NewDocument"),
        onClick: this.createDocument,
      },
      {
        key: "new-spreadsheet",
        label: t("NewSpreadsheet"),
        onClick: this.createSpreadsheet,
      },
      {
        key: "new-presentation",
        label: t("NewPresentation"),
        onClick: this.createPresentation,
      },
      {
        label: t("Translations:NewForm"),
        onClick: this.createForm,
      },
      {
        label: t("Translations:NewFormFile"),
        onClick: this.createFormFromFile,
        disabled: isPrivacyFolder,
      },
      {
        key: "new-folder",
        label: t("NewFolder"),
        onClick: this.createFolder,
      },
      { key: "separator", isSeparator: true },
      {
        key: "make-invitation-link",
        label: t("UploadToFolder"),
        onClick: this.uploadToFolder,
        disabled: true,
      },
    ];
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

  onEmptyTrashAction = () => this.props.setEmptyTrashDialogVisible(true);

  getContextOptionsFolder = () => {
    const { t, personal } = this.props;

    return [
      {
        key: "sharing-settings",
        label: t("SharingSettings"),
        onClick: this.onOpenSharingPanel,
        disabled: personal ? true : false,
      },
      {
        key: "link-portal-users",
        label: t("LinkForPortalUsers"),
        onClick: this.createLinkForPortalUsers,
        disabled: personal ? true : false,
      },
      { key: "separator-2", isSeparator: true },
      {
        key: "move-to",
        label: t("MoveTo"),
        onClick: this.onMoveAction,
        disabled: false,
      },
      {
        key: "copy",
        label: t("Translations:Copy"),
        onClick: this.onCopyAction,
        disabled: false,
      },
      {
        key: "download",
        label: t("Common:Download"),
        onClick: this.downloadAction,
        disabled: false,
      },
      {
        key: "rename",
        label: t("Rename"),
        onClick: this.renameAction,
        disabled: true,
      },
      {
        key: "delete",
        label: t("Common:Delete"),
        onClick: this.onDeleteAction,
        disabled: false,
      },
    ];
  };

  onBackToParentFolder = () => {
    const { setIsLoading, parentId, filter, fetchFiles } = this.props;
    setIsLoading(true);
    fetchFiles(parentId, null, true, false).finally(() => setIsLoading(false));
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

  onClickFolder = (data) => {
    const { setSelectedNode, setIsLoading, fetchFiles } = this.props;
    setSelectedNode(data);
    setIsLoading(true);
    fetchFiles(data, null, true, false)
      .catch((err) => toastr.error(err))
      .finally(() => setIsLoading(false));
  };

  render() {
    //console.log("Body header render");

    const {
      t,
      tReady,
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
              />
            ) : (
              <div className="header-container">
                {isLoading ? (
                  <Loaders.SectionHeader />
                ) : (
                  <Navigation
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
      fileActionStore,
      fetchFiles,
      filter,
      canCreate,
      isHeaderVisible,
      isHeaderIndeterminate,
      isHeaderChecked,
      isThirdPartySelection,
      setIsLoading,
      cbMenuItems,
      getCheckboxItemLabel,
      isEmptyFilesList,
      getFolderInfo,
      setBufferSelection,
      viewAs,
    } = filesStore;
    const { setAction } = fileActionStore;
    const {
      setSharingPanelVisible,
      setMoveToPanelVisible,
      setCopyPanelVisible,
      setDeleteDialogVisible,
      setEmptyTrashDialogVisible,
      setSelectFileDialogVisible,
      setIsFolderActions,
    } = dialogsStore;

    const { isRecycleBinFolder, isPrivacyFolder } = treeFoldersStore;
    const { deleteAction, downloadAction, getHeaderMenu } = filesActionsStore;

    return {
      showText: auth.settingsStore.showText,

      isDesktop: auth.settingsStore.isDesktopClient,
      isRootFolder: selectedFolderStore.parentId === 0,
      title: selectedFolderStore.title,
      parentId: selectedFolderStore.parentId,
      currentFolderId: selectedFolderStore.id,
      pathParts: selectedFolderStore.pathParts,
      navigationPath: selectedFolderStore.navigationPath,
      filter,
      canCreate,
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
      setAction,
      setIsLoading,
      fetchFiles,
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
      setSelectFileDialogVisible,

      isRecycleBinFolder,
      setEmptyTrashDialogVisible,
      isEmptyFilesList,
      isPrivacyFolder,
      viewAs,
    };
  }
)(
  withTranslation(["Home", "Common", "Translations"])(
    withRouter(observer(SectionHeaderContent))
  )
);
