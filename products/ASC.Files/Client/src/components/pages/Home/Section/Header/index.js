import React from "react";
import styled, { css } from "styled-components";
import { withRouter } from "react-router";
import { constants, Headline, store } from "asc-web-common";
import { connect } from "react-redux";
import { withTranslation } from "react-i18next";
import {
  ContextMenuButton,
  DropDownItem,
  GroupButtonsMenu,
  IconButton,
  toastr
} from "asc-web-components";
import { fetchFiles, setAction } from "../../../../../store/files/actions";
import { default as filesStore } from "../../../../../store/store";
import { EmptyTrashDialog, DeleteDialog } from "../../../../dialogs";
import { SharingPanel } from "../../../../panels";
import {
  isCanBeDeleted,
  getAccessOption,
  checkFolderType
} from "../../../../../store/files/selectors";

const { isAdmin } = store.auth.selectors;
const { FilterType, FileAction } = constants;

const StyledContainer = styled.div`
  @media (min-width: 1024px) {
    ${props =>
      props.isHeaderVisible &&
      css`
        width: calc(100% + 76px);
      `}
  }

  .header-container {
    position: relative;
    display: flex;
    align-items: center;
    max-width: calc(100vw - 32px);

    .arrow-button {
      margin-right: 16px;

      @media (max-width: 1024px) {
        padding: 8px 0 8px 8px;
        margin-left: -8px;
      }
    }

    .add-button {
      margin-bottom: -1px;
      margin-left: 16px;

      @media (max-width: 1024px) {
        margin-left: auto;

        & > div:first-child {
          padding: 8px 8px 8px 8px;
          margin-right: -8px;
        }
      }
    }

    .option-button {
      margin-bottom: -1px;
      margin-left: 16px;

      @media (max-width: 1024px) {
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

    @media (max-width: 1024px) {
      & > div:first-child {
        ${props =>
          props.isArticlePinned &&
          css`
            width: calc(100% - 240px);
          `}
        position: absolute;
        top: 56px;
        z-index: 180;
      }
    }

    @media (min-width: 1024px) {
      margin: 0 -24px;
    }
  }
`;

class SectionHeaderContent extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      showSharingPanel: false,
      showDeleteDialog: false,
      showEmptyTrashDialog: false
    };
  }

  onCreate = format => {
    this.props.setAction({
      type: FileAction.Create,
      extension: format,
      id: -1
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
        onClick: this.createDocument
      },
      {
        key: "new-spreadsheet",
        label: t("NewSpreadsheet"),
        onClick: this.createSpreadsheet
      },
      {
        key: "new-presentation",
        label: t("NewPresentation"),
        onClick: this.createPresentation
      },
      {
        key: "new-folder",
        label: t("NewFolder"),
        onClick: this.createFolder
      },
      { key: "separator", isSeparator: true },
      {
        key: "make-invitation-link",
        label: t("UploadToFolder"),
        onClick: this.uploadToFolder,
        disabled: true
      }
    ];
  };

  createLinkForPortalUsers = () =>
    toastr.info("createLinkForPortalUsers click");

  moveAction = () => toastr.info("moveAction click");

  copyAction = () => toastr.info("copyAction click");

  downloadAction = () => toastr.info("downloadAction click");

  downloadAsAction = () => toastr.info("downloadAsAction click");

  renameAction = () => toastr.info("renameAction click");

  onOpenSharingPanel = () => 
    this.setState({ showSharingPanel: !this.state.showSharingPanel });

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
        disabled: true
      },
      {
        key: "link-portal-users",
        label: t("LinkForPortalUsers"),
        onClick: this.createLinkForPortalUsers,
        disabled: true
      },
      { key: "separator-2", isSeparator: true },
      {
        key: "move-to",
        label: t("MoveTo"),
        onClick: this.moveAction,
        disabled: true
      },
      {
        key: "copy",
        label: t("Copy"),
        onClick: this.copyAction,
        disabled: true
      },
      {
        key: "download",
        label: t("Download"),
        onClick: this.downloadAction,
        disabled: true
      },
      {
        key: "rename",
        label: t("Rename"),
        onClick: this.renameAction,
        disabled: true
      },
      {
        key: "delete",
        label: t("Delete"),
        onClick: this.onDeleteAction,
        disabled: true
      }
    ];
  };

  onBackToParentFolder = () => {
    const { onLoading, parentId, filter } = this.props;
    onLoading(true);
    fetchFiles(parentId, filter, filesStore.dispatch).finally(() =>
      onLoading(false)
    );
  };

  

  render() {
    //console.log("Body header render");

    const {
      t,
      selection,
      isHeaderVisible,
      onClose,
      isRecycleBinFolder,
      isHeaderChecked,
      isHeaderIndeterminate,
      onSelect,
      deleteDialogVisible,
      folder,
      onCheck,
      title,
      accessOptions,
      shareDataItems,
      currentFolderId
    } = this.props;
    const {
      showDeleteDialog,
      showSharingPanel,
      showEmptyTrashDialog
    } = this.state;
    const isItemsSelected = selection.length;
    const isOnlyFolderSelected = selection.every(
      selected => !selected.fileType
    );

    const menuItems = [
      {
        label: t("LblSelect"),
        isDropdown: true,
        isSeparator: true,
        isSelect: true,
        fontWeight: "bold",
        children: [
          <DropDownItem key="all" label={t("All")} />,
          <DropDownItem key={FilterType.FoldersOnly} label={t("Folders")} />,
          <DropDownItem
            key={FilterType.DocumentsOnly}
            label={t("Documents")}
          />,
          <DropDownItem
            key={FilterType.PresentationsOnly}
            label={t("Presentations")}
          />,
          <DropDownItem
            key={FilterType.SpreadsheetsOnly}
            label={t("Spreadsheets")}
          />,
          <DropDownItem key={FilterType.ImagesOnly} label={t("Images")} />,
          <DropDownItem key={FilterType.MediaOnly} label={t("Media")} />,
          <DropDownItem key={FilterType.ArchiveOnly} label={t("Archives")} />,
          <DropDownItem key={FilterType.FilesOnly} label={t("AllFiles")} />
        ],
        onSelect: item => onSelect(item.key)
      },
      {
        label: t("Share"),
        disabled: !isItemsSelected || shareDataItems.length === 0,
        onClick: this.onOpenSharingPanel
      },
      {
        label: t("Download"),
        disabled: !isItemsSelected,
        onClick: this.downloadAction
      },
      {
        label: t("DownloadAs"),
        disabled: !isItemsSelected || isOnlyFolderSelected,
        onClick: this.downloadAsAction
      },
      {
        label: t("MoveTo"),
        disabled: !isItemsSelected,
        onClick: this.moveAction
      },
      {
        label: t("Copy"),
        disabled: !isItemsSelected,
        onClick: this.copyAction
      },
      {
        label: t("Delete"),
        disabled: !isItemsSelected || !deleteDialogVisible,
        onClick: this.onDeleteAction
      }
    ];

    isRecycleBinFolder &&
      menuItems.push({
        label: t("EmptyRecycleBin"),
        onClick: this.onEmptyTrashAction
      });

    return (
      <StyledContainer isHeaderVisible={isHeaderVisible}>
        {isHeaderVisible ? (
          <div className="group-button-menu-container">
            <GroupButtonsMenu
              checked={isHeaderChecked}
              isIndeterminate={isHeaderIndeterminate}
              onChange={onCheck}
              menuItems={menuItems}
              visible={isHeaderVisible}
              moreLabel={t("More")}
              closeTitle={t("CloseButton")}
              onClose={onClose}
              selected={menuItems[0].label}
            />
          </div>
        ) : (
          <div className="header-container">
            {folder && (
              <IconButton
                iconName="ArrowPathIcon"
                size="16"
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
            {folder ? (
              <>
                <ContextMenuButton
                  className="add-button"
                  directionX="right"
                  iconName="PlusIcon"
                  size={16}
                  color="#657077"
                  getData={this.getContextOptionsPlus}
                  isDisabled={false}
                />
                <ContextMenuButton
                  className="option-button"
                  directionX="right"
                  iconName="VerticalDotsIcon"
                  size={16}
                  color="#A3A9AE"
                  getData={this.getContextOptionsFolder}
                  isDisabled={false}
                />
              </>
            ) : (
              <ContextMenuButton
                className="add-button"
                directionX="right"
                iconName="PlusIcon"
                size={16}
                color="#657077"
                getData={this.getContextOptionsPlus}
                isDisabled={false}
              />
            )}
          </div>
        )}

        {showDeleteDialog && (
          <DeleteDialog
            isRecycleBinFolder={isRecycleBinFolder}
            visible={showDeleteDialog}
            onClose={this.onDeleteAction}
            selection={selection}
          />
        )}

        {showEmptyTrashDialog && (
          <EmptyTrashDialog
            currentFolderId={currentFolderId}
            visible={showEmptyTrashDialog}
            onClose={this.onEmptyTrashAction}
          />
        )}

        <SharingPanel
          onClose={this.onOpenSharingPanel}
          visible={showSharingPanel}
          accessOptions={accessOptions}
        />
      </StyledContainer>
    );
  }
}

const mapStateToProps = state => {
  const {
    selectedFolder,
    selection,
    treeFolders,
    filter,
    shareDataItems
  } = state.files;
  const { parentId, title, id } = selectedFolder;
  const { user } = state.auth;

  const indexOfTrash = 3;
  user.rights = { icon: "AccessEditIcon", rights: "FullAccess" };

  return {
    folder: parentId !== 0,
    isAdmin: isAdmin(user),
    isRecycleBinFolder: checkFolderType(id, indexOfTrash, treeFolders),
    parentId,
    selection,
    title,
    filter,
    deleteDialogVisible: isCanBeDeleted(selectedFolder, user),
    accessOptions: getAccessOption(selection),
    shareDataItems,
    currentFolderId: id
  };
};

export default connect(mapStateToProps, { setAction })(
  withTranslation()(withRouter(SectionHeaderContent))
);
