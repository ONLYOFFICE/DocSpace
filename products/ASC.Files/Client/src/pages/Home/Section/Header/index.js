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
import GroupButtonsMenu from "@appserver/components/group-buttons-menu";
import IconButton from "@appserver/components/icon-button";
import { tablet, desktop } from "@appserver/components/utils/device";
import { Consumer } from "@appserver/components/utils/context";
import { inject, observer } from "mobx-react";

const StyledContainer = styled.div`
  .header-container {
    position: relative;
    ${(props) =>
      props.title &&
      css`
        display: grid;
        grid-template-columns: ${(props) =>
          props.isRootFolder
            ? "auto auto 1fr"
            : props.canCreate
            ? "auto auto auto auto 1fr"
            : "auto auto auto 1fr"};

        @media ${tablet} {
          grid-template-columns: ${(props) =>
            props.isRootFolder
              ? "1fr auto"
              : props.canCreate
              ? "auto 1fr auto auto"
              : "auto 1fr auto"};
        }
      `}
    align-items: center;
    max-width: calc(100vw - 32px);

    @media ${tablet} {
      .headline-header {
        margin-left: -1px;
      }
    }
    .arrow-button {
      margin-right: 15px;
      min-width: 17px;

      @media ${tablet} {
        padding: 8px 0 8px 8px;
        margin-left: -8px;
        margin-right: 16px;
      }
    }

    .add-button {
      margin-bottom: -1px;
      margin-left: 16px;

      @media ${tablet} {
        margin-left: auto;

        & > div:first-child {
          padding: 8px 8px 8px 8px;
          margin-right: -8px;
        }
      }
    }

    .option-button {
      margin-bottom: -1px;

      @media (min-width: 1024px) {
        margin-left: 8px;
      }

      @media ${tablet} {
        & > div:first-child {
          padding: 8px 8px 8px 8px;
          margin-right: -8px;
        }
      }
    }
  }

  .group-button-menu-container {
    margin: 0 -16px;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
    padding-bottom: 56px;

    ${isMobile &&
    css`
      position: sticky;
    `}

    ${(props) =>
      !props.isTabletView
        ? props.width &&
          isMobile &&
          css`
            width: ${props.width + 40 + "px"};
          `
        : props.width &&
          isMobile &&
          css`
            width: ${props.width + 32 + "px"};
          `}

    @media ${tablet} {
      padding-bottom: 0;
      ${!isMobile &&
      css`
        height: 56px;
      `}
      & > div:first-child {
        ${(props) =>
          !isMobile &&
          props.width &&
          css`
            width: ${props.width + 16 + "px"};
          `}

        position: absolute;
        ${(props) =>
          !props.isDesktop &&
          css`
            top: 56px;
          `}
        z-index: 180;
      }
    }

    @media ${desktop} {
      margin: 0 -24px;
    }
  }
`;

class SectionHeaderContent extends React.Component {
  constructor(props) {
    super(props);
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

  createFolder = () => this.onCreate();

  uploadToFolder = () => console.log("Upload To Folder click");

  getContextOptionsPlus = () => {
    const { t } = this.props;

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

  onMoveAction = () => this.props.setMoveToPanelVisible(true);
  onCopyAction = () => this.props.setCopyPanelVisible(true);
  downloadAction = () =>
    this.props
      .downloadAction(this.props.t("Translations:ArchivingData"))
      .catch((err) => toastr.error(err));

  downloadAsAction = () => this.props.setDownloadDialogVisible(true);
  renameAction = () => console.log("renameAction click");
  onOpenSharingPanel = () => this.props.setSharingPanelVisible(true);

  onDeleteAction = () => {
    const {
      t,
      deleteAction,
      confirmDelete,
      setDeleteDialogVisible,
    } = this.props;

    if (confirmDelete) {
      setDeleteDialogVisible(true);
    } else {
      const translations = {
        deleteOperation: t("Translations:DeleteOperation"),
        deleteFromTrash: t("Translations:DeleteFromTrash"),
        deleteSelectedElem: t("Translations:DeleteSelectedElem"),
      };

      deleteAction(translations).catch((err) => toastr.error(err));
    }
  };

  onEmptyTrashAction = () => this.props.setEmptyTrashDialogVisible(true);

  getContextOptionsFolder = () => {
    const { t } = this.props;
    return [
      {
        key: "sharing-settings",
        label: t("SharingSettings"),
        onClick: this.onOpenSharingPanel,
        disabled: true,
      },
      {
        key: "link-portal-users",
        label: t("LinkForPortalUsers"),
        onClick: this.createLinkForPortalUsers,
        disabled: false,
      },
      { key: "separator-2", isSeparator: true },
      {
        key: "move-to",
        label: t("MoveTo"),
        onClick: this.onMoveAction,
        disabled: true,
      },
      {
        key: "copy",
        label: t("Translations:Copy"),
        onClick: this.onCopyAction,
        disabled: true,
      },
      {
        key: "download",
        label: t("Common:Download"),
        onClick: this.downloadAction,
        disabled: true,
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
        disabled: true,
      },
    ];
  };

  onBackToParentFolder = () => {
    const { setIsLoading, parentId, filter, fetchFiles } = this.props;
    setIsLoading(true);
    fetchFiles(parentId, filter).finally(() => setIsLoading(false));
  };

  onCheck = (checked) => {
    this.props.setSelected(checked ? "all" : "none");
  };

  onSelect = (item) => {
    this.props.setSelected(item.key);
  };

  onClose = () => {
    this.props.setSelected("close");
  };

  getMenuItems = () => {
    const {
      t,
      selectionCount,
      isAccessedSelected,
      isWebEditSelected,
      deleteDialogVisible,
      isRecycleBin,
      isThirdPartySelection,
      isPrivacy,
      isFavoritesFolder,
      isRecentFolder,
      isShareFolder,
      personal,
    } = this.props;

    let menu = [
      {
        label: t("Common:Select"),
        isDropdown: true,
        isSeparator: true,
        isSelect: true,
        fontWeight: "bold",
        children: [
          <DropDownItem key="all" label={t("All")} data-index={0} />,
          <DropDownItem
            key={FilterType.FoldersOnly}
            label={t("Translations:Folders")}
            data-index={1}
          />,
          <DropDownItem
            key={FilterType.DocumentsOnly}
            label={t("Common:Documents")}
            data-index={2}
          />,
          <DropDownItem
            key={FilterType.PresentationsOnly}
            label={t("Translations:Presentations")}
            data-index={3}
          />,
          <DropDownItem
            key={FilterType.SpreadsheetsOnly}
            label={t("Translations:Spreadsheets")}
            data-index={4}
          />,
          <DropDownItem
            key={FilterType.ImagesOnly}
            label={t("Images")}
            data-index={5}
          />,
          <DropDownItem
            key={FilterType.MediaOnly}
            label={t("Media")}
            data-index={6}
          />,
          <DropDownItem
            key={FilterType.ArchiveOnly}
            label={t("Archives")}
            data-index={7}
          />,
          <DropDownItem
            key={FilterType.FilesOnly}
            label={t("AllFiles")}
            data-index={8}
          />,
        ],
        onSelect: this.onSelect,
      },
      {
        label: t("Share"),
        disabled:
          isFavoritesFolder ||
          isRecentFolder ||
          !isAccessedSelected ||
          selectionCount > 1,
        onClick: this.onOpenSharingPanel,
      },
      {
        label: t("Common:Download"),
        disabled: !selectionCount,
        onClick: this.downloadAction,
      },
      {
        label: t("Translations:DownloadAs"),
        disabled: !selectionCount || !isWebEditSelected,
        onClick: this.downloadAsAction,
      },
      {
        label: t("MoveTo"),
        disabled:
          isFavoritesFolder ||
          isRecentFolder ||
          !isAccessedSelected ||
          !selectionCount ||
          isThirdPartySelection,
        onClick: this.onMoveAction,
      },
      {
        label: t("Translations:Copy"),
        disabled: !selectionCount,
        onClick: this.onCopyAction,
      },
      {
        label: t("Common:Delete"),
        disabled:
          !selectionCount || !deleteDialogVisible || isThirdPartySelection,
        onClick: this.onDeleteAction,
      },
    ];

    if (isRecycleBin) {
      menu.push({
        label: t("EmptyRecycleBin"),
        onClick: this.onEmptyTrashAction,
      });

      menu.splice(4, 2, {
        label: t("Translations:Restore"),
        onClick: this.onMoveAction,
      });

      menu.splice(1, 1);
    }

    if (isPrivacy) {
      menu.splice(1, 1);
      menu.splice(2, 1);
      menu.splice(3, 1);
    }

    if (isShareFolder) {
      menu.splice(4, 1);
    }

    if ((personal && !isWebEditSelected) || selectionCount > 1) {
      menu.splice(1, 1);
    }

    return menu;
  };

  render() {
    //console.log("Body header render");

    const {
      t,
      tReady,
      isHeaderVisible,
      isHeaderChecked,
      isHeaderIndeterminate,
      isRootFolder,
      title,
      canCreate,
      isDesktop,
      isTabletView,
      personal,
    } = this.props;

    const menuItems = this.getMenuItems();

    return (
      <Consumer>
        {(context) => (
          <StyledContainer
            width={context.sectionWidth}
            isRootFolder={isRootFolder}
            canCreate={canCreate}
            title={title}
            isDesktop={isDesktop}
            isTabletView={isTabletView}
          >
            {isHeaderVisible ? (
              <div className="group-button-menu-container">
                <GroupButtonsMenu
                  checked={isHeaderChecked}
                  isIndeterminate={isHeaderIndeterminate}
                  onChange={this.onCheck}
                  menuItems={menuItems}
                  visible={isHeaderVisible}
                  moreLabel={t("Common:More")}
                  closeTitle={t("Common:CloseButton")}
                  onClose={this.onClose}
                  selected={menuItems[0].label}
                  sectionWidth={context.sectionWidth}
                />
              </div>
            ) : (
              <div className="header-container">
                {!title || !tReady ? (
                  <Loaders.SectionHeader />
                ) : (
                  <>
                    {!isRootFolder && (
                      <IconButton
                        iconName="/static/images/arrow.path.react.svg"
                        size="17"
                        color="#A3A9AE"
                        hoverColor="#657077"
                        isFill={true}
                        onClick={this.onBackToParentFolder}
                        className="arrow-button"
                      />
                    )}
                    <Headline
                      className="headline-header"
                      type="content"
                      truncate={true}
                    >
                      {title}
                    </Headline>
                    {!isRootFolder && canCreate ? (
                      <>
                        <ContextMenuButton
                          className="add-button"
                          directionX="right"
                          iconName="images/plus.svg"
                          size={17}
                          color="#A3A9AE"
                          hoverColor="#657077"
                          isFill
                          getData={this.getContextOptionsPlus}
                          isDisabled={false}
                        />
                        {!personal && (
                          <ContextMenuButton
                            className="option-button"
                            directionX="right"
                            iconName="images/vertical-dots.react.svg"
                            size={17}
                            color="#A3A9AE"
                            hoverColor="#657077"
                            isFill
                            getData={this.getContextOptionsFolder}
                            isDisabled={false}
                          />
                        )}
                      </>
                    ) : (
                      canCreate && (
                        <ContextMenuButton
                          className="add-button"
                          directionX="right"
                          iconName="images/plus.svg"
                          size={17}
                          color="#A3A9AE"
                          hoverColor="#657077"
                          isFill
                          getData={this.getContextOptionsPlus}
                          isDisabled={false}
                        />
                      )
                    )}
                  </>
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
    treeFoldersStore,
    selectedFolderStore,
    filesActionsStore,
    settingsStore,
  }) => {
    const {
      setSelected,
      fileActionStore,
      fetchFiles,
      selection,
      filter,
      canCreate,
      isHeaderVisible,
      isHeaderIndeterminate,
      isHeaderChecked,
      userAccess,
      isAccessedSelected,
      isThirdPartySelection,
      isWebEditSelected,
      setIsLoading,
    } = filesStore;
    const {
      isRecycleBinFolder,
      isPrivacyFolder,
      isFavoritesFolder,
      isRecentFolder,
      isShareFolder,
    } = treeFoldersStore;
    const { setAction } = fileActionStore;
    const {
      setSharingPanelVisible,
      setMoveToPanelVisible,
      setCopyPanelVisible,
      setEmptyTrashDialogVisible,
      setDownloadDialogVisible,
      setDeleteDialogVisible,
    } = dialogsStore;

    const { deleteAction, downloadAction } = filesActionsStore;

    return {
      isDesktop: auth.settingsStore.isDesktopClient,
      isRootFolder: selectedFolderStore.parentId === 0,
      title: selectedFolderStore.title,
      parentId: selectedFolderStore.parentId,
      currentFolderId: selectedFolderStore.id,
      isRecycleBin: isRecycleBinFolder,
      isPrivacy: isPrivacyFolder,
      isFavoritesFolder,
      isRecentFolder,
      isShareFolder,
      filter,
      canCreate,
      selectionCount: selection.length,
      isHeaderVisible,
      isHeaderIndeterminate,
      isHeaderChecked,
      deleteDialogVisible: userAccess,
      isAccessedSelected,
      isThirdPartySelection,
      isWebEditSelected,
      isTabletView: auth.settingsStore.isTabletView,
      confirmDelete: settingsStore.confirmDelete,
      personal: auth.settingsStore.personal,

      setSelected,
      setAction,
      setIsLoading,
      fetchFiles,
      setSharingPanelVisible,
      setMoveToPanelVisible,
      setCopyPanelVisible,
      setEmptyTrashDialogVisible,
      deleteAction,
      setDeleteDialogVisible,
      setDownloadDialogVisible,
      downloadAction,
    };
  }
)(
  withTranslation(["Home", "Common", "Translations"])(
    withRouter(observer(SectionHeaderContent))
  )
);
