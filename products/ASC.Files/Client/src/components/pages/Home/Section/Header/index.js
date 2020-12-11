import React from "react";
import copy from "copy-to-clipboard";
import styled, { css } from "styled-components";
import { withRouter } from "react-router";
import {
  constants,
  Headline,
  store,
  api,
  toastr,
  Loaders,
} from "asc-web-common";
import { connect } from "react-redux";
import { withTranslation } from "react-i18next";
import {
  ContextMenuButton,
  DropDownItem,
  GroupButtonsMenu,
  IconButton,
  utils,
} from "asc-web-components";
import {
  fetchFiles,
  setAction,
  setSecondaryProgressBarData,
  clearSecondaryProgressData,
  setIsLoading,
  setSelected,
  setSharingPanelVisible,
} from "../../../../../store/files/actions";
import { TIMEOUT } from "../../../../../helpers/constants";
import {
  EmptyTrashDialog,
  DeleteDialog,
  DownloadDialog,
} from "../../../../dialogs";
import { OperationsPanel } from "../../../../panels";
import {
  isCanBeDeleted,
  getIsRecycleBinFolder,
  canCreate,
  getSelectedFolderTitle,
  getFilter,
  getSelectedFolderId,
  getSelection,
  getSelectedFolderParentId,
  getIsRootFolder,
  getHeaderVisible,
  getHeaderIndeterminate,
  getHeaderChecked,
  getOnlyFoldersSelected,
  getAccessedSelected,
  getSelectionLength,
  getSharePanelVisible,
} from "../../../../../store/files/selectors";

const { isAdmin } = store.auth.selectors;
const { FilterType, FileAction } = constants;
const { tablet, desktop } = utils.device;
const { Consumer } = utils.context;

const StyledContainer = styled.div`
  .header-container {
    position: relative;
    display: grid;
    grid-template-columns: ${(props) =>
      props.isRootFolder
        ? "auto auto 1fr"
        : props.canCreate
        ? "auto auto auto auto 1fr"
        : "auto auto auto 1fr"};

    align-items: center;
    max-width: calc(100vw - 32px);

    @media ${tablet} {
      grid-template-columns: ${(props) =>
        props.isRootFolder
          ? "1fr auto"
          : props.canCreate
          ? "auto 1fr auto auto"
          : "auto 1fr auto"};

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

    @media ${tablet} {
      & > div:first-child {
        ${(props) =>
          props.width &&
          css`
            width: ${props.width + 16 + "px"};
          `}
        position: absolute;
        top: 56px;
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

    this.state = {
      showDeleteDialog: false,
      showDownloadDialog: false,
      showEmptyTrashDialog: false,
      showMoveToPanel: false,
      showCopyPanel: false,
    };
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

  uploadToFolder = () => toastr.info("Upload To Folder click");

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

    toastr.success(t("LinkCopySuccess"));
  };

  onMoveAction = () =>
    this.setState({ showMoveToPanel: !this.state.showMoveToPanel });

  onCopyAction = () =>
    this.setState({ showCopyPanel: !this.state.showCopyPanel });

  loop = (url) => {
    api.files
      .getProgress()
      .then((res) => {
        if (!url) {
          this.props.setSecondaryProgressBarData({
            icon: "file",
            visible: true,
            percent: res[0].progress,
            label: this.props.t("ArchivingData"),
            alert: false,
          });
          setTimeout(() => this.loop(res[0].url), 1000);
        } else {
          setTimeout(() => this.props.clearSecondaryProgressData(), TIMEOUT);
          return window.open(url, "_blank");
        }
      })
      .catch((err) => {
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        //toastr.error(err);
        setTimeout(() => this.props.clearSecondaryProgressData(), TIMEOUT);
      });
  };

  downloadAction = () => {
    const {
      t,
      selection,
      setSecondaryProgressBarData,
      clearSecondaryProgressData,
    } = this.props;
    const fileIds = [];
    const folderIds = [];
    const items = [];

    for (let item of selection) {
      if (item.fileExst) {
        fileIds.push(item.id);
        items.push({ id: item.id, fileExst: item.fileExst });
      } else {
        folderIds.push(item.id);
        items.push({ id: item.id });
      }
    }

    setSecondaryProgressBarData({
      icon: "file",
      visible: true,
      percent: 0,
      label: t("ArchivingData"),
      alert: false,
    });

    api.files
      .downloadFiles(fileIds, folderIds)
      .then((res) => {
        this.loop(res[0].url);
      })
      .catch((err) => {
        setSecondaryProgressBarData({
          visible: true,
          alert: true,
        });
        //toastr.error(err);
        setTimeout(() => clearSecondaryProgressData(), TIMEOUT);
      });
  };

  downloadAsAction = () =>
    this.setState({ showDownloadDialog: !this.state.showDownloadDialog });

  renameAction = () => toastr.info("renameAction click");

  onOpenSharingPanel = () =>
    this.props.setSharingPanelVisible(!this.props.sharingPanelVisible);

  onDeleteAction = () =>
    this.setState({ showDeleteDialog: !this.state.showDeleteDialog });

  onEmptyTrashAction = () =>
    this.setState({ showEmptyTrashDialog: !this.state.showEmptyTrashDialog });

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
        label: t("Copy"),
        onClick: this.onCopyAction,
        disabled: true,
      },
      {
        key: "download",
        label: t("Download"),
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
        label: t("Delete"),
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
      isItemsSelected,
      isAccessedSelected,
      isOnlyFoldersSelected,
      deleteDialogVisible,
      isRecycleBin,
    } = this.props;

    let menu = [
      {
        label: t("LblSelect"),
        isDropdown: true,
        isSeparator: true,
        isSelect: true,
        fontWeight: "bold",
        children: [
          <DropDownItem key="all" label={t("All")} data-index={0} />,
          <DropDownItem
            key={FilterType.FoldersOnly}
            label={t("Folders")}
            data-index={1}
          />,
          <DropDownItem
            key={FilterType.DocumentsOnly}
            label={t("Documents")}
            data-index={2}
          />,
          <DropDownItem
            key={FilterType.PresentationsOnly}
            label={t("Presentations")}
            data-index={3}
          />,
          <DropDownItem
            key={FilterType.SpreadsheetsOnly}
            label={t("Spreadsheets")}
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
        disabled: !isAccessedSelected,
        onClick: this.onOpenSharingPanel,
      },
      {
        label: t("Download"),
        disabled: !isItemsSelected,
        onClick: this.downloadAction,
      },
      {
        label: t("DownloadAs"),
        disabled: !isItemsSelected || isOnlyFoldersSelected,
        onClick: this.downloadAsAction,
      },
      {
        label: t("MoveTo"),
        disabled: !isItemsSelected,
        onClick: this.onMoveAction,
      },
      {
        label: t("Copy"),
        disabled: !isItemsSelected,
        onClick: this.onCopyAction,
      },
      {
        label: t("Delete"),
        disabled: !isItemsSelected || !deleteDialogVisible,
        onClick: this.onDeleteAction,
      },
    ];

    if (isRecycleBin) {
      menu.push({
        label: t("EmptyRecycleBin"),
        onClick: this.onEmptyTrashAction,
      });

      menu.splice(4, 2, {
        label: t("Restore"),
        onClick: this.onMoveAction,
      });

      menu.splice(1, 1);
    }

    return menu;
  };

  render() {
    //console.log("Body header render");

    const {
      t,
      selection,
      isHeaderVisible,
      isRecycleBin,
      isHeaderChecked,
      isHeaderIndeterminate,
      isRootFolder,
      title,
      canCreate,
    } = this.props;

    const {
      showDeleteDialog,
      showEmptyTrashDialog,
      showDownloadDialog,
      showMoveToPanel,
      showCopyPanel,
    } = this.state;

    const menuItems = this.getMenuItems();

    return (
      <Consumer>
        {(context) => (
          <StyledContainer
            width={context.sectionWidth}
            isRootFolder={isRootFolder}
            canCreate={canCreate}
          >
            {isHeaderVisible ? (
              <div className="group-button-menu-container">
                <GroupButtonsMenu
                  checked={isHeaderChecked}
                  isIndeterminate={isHeaderIndeterminate}
                  onChange={this.onCheck}
                  menuItems={menuItems}
                  visible={isHeaderVisible}
                  moreLabel={t("More")}
                  closeTitle={t("CloseButton")}
                  onClose={this.onClose}
                  selected={menuItems[0].label}
                  sectionWidth={context.sectionWidth}
                />
              </div>
            ) : (
              <div className="header-container">
                {!title ? (
                  <Loaders.SectionHeader />
                ) : (
                  <>
                    {!isRootFolder && (
                      <IconButton
                        iconName="ArrowPathIcon"
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
                          iconName="PlusIcon"
                          size={17}
                          color="#A3A9AE"
                          hoverColor="#657077"
                          isFill
                          getData={this.getContextOptionsPlus}
                          isDisabled={false}
                        />
                        <ContextMenuButton
                          className="option-button"
                          directionX="right"
                          iconName="VerticalDotsIcon"
                          size={17}
                          color="#A3A9AE"
                          hoverColor="#657077"
                          isFill
                          getData={this.getContextOptionsFolder}
                          isDisabled={false}
                        />
                      </>
                    ) : (
                      canCreate && (
                        <ContextMenuButton
                          className="add-button"
                          directionX="right"
                          iconName="PlusIcon"
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

            {showDeleteDialog && (
              <DeleteDialog
                isRecycleBin={isRecycleBin}
                visible={showDeleteDialog}
                onClose={this.onDeleteAction}
                selection={selection}
              />
            )}

            {showEmptyTrashDialog && (
              <EmptyTrashDialog
                visible={showEmptyTrashDialog}
                onClose={this.onEmptyTrashAction}
              />
            )}

            {showMoveToPanel && (
              <OperationsPanel
                isCopy={false}
                visible={showMoveToPanel}
                onClose={this.onMoveAction}
              />
            )}

            {showCopyPanel && (
              <OperationsPanel
                isCopy={true}
                visible={showCopyPanel}
                onClose={this.onCopyAction}
              />
            )}

            {showDownloadDialog && (
              <DownloadDialog
                visible={showDownloadDialog}
                onClose={this.downloadAsAction}
                onDownloadProgress={this.loop}
              />
            )}
          </StyledContainer>
        )}
      </Consumer>
    );
  }
}

const mapStateToProps = (state) => {
  return {
    isRootFolder: getIsRootFolder(state),
    isAdmin: isAdmin(state),
    isRecycleBin: getIsRecycleBinFolder(state),
    parentId: getSelectedFolderParentId(state),
    selection: getSelection(state),
    title: getSelectedFolderTitle(state),
    filter: getFilter(state),
    deleteDialogVisible: isCanBeDeleted(state),
    currentFolderId: getSelectedFolderId(state),
    canCreate: canCreate(state),
    isHeaderVisible: getHeaderVisible(state),
    isHeaderIndeterminate: getHeaderIndeterminate(state),
    isHeaderChecked: getHeaderChecked(state),
    isAccessedSelected: getAccessedSelected(state),
    isOnlyFoldersSelected: getOnlyFoldersSelected(state),
    isItemsSelected: getSelectionLength(state),
    sharingPanelVisible: getSharePanelVisible(state),
  };
};

export default connect(mapStateToProps, {
  setAction,
  setSecondaryProgressBarData,
  setIsLoading,
  clearSecondaryProgressData,
  fetchFiles,
  setSelected,
  setSharingPanelVisible,
})(withTranslation()(withRouter(SectionHeaderContent)));
