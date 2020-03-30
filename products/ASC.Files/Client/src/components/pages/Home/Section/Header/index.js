import React, { useCallback, useState } from "react";
import styled, { css } from "styled-components";
import { withRouter } from "react-router";
import {
  constants,
  Headline,
  store
} from 'asc-web-common';
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
import { EmptyTrashDialog, DeleteDialog } from '../../../../dialogs';
import { isCanBeDeleted } from "../../../../../store/files/selectors";

import SharingPanel from "./panels/SharingPanel";
import AddUsersPanel from "./panels/AddUsersPanel";
import AddGroupPanel from "./panels/AddGroupPanel";

const { isAdmin } = store.auth.selectors;
const { FilterType, FileAction } = constants;

const StyledContainer = styled.div`

  @media (min-width: 1024px) {
    ${props => props.isHeaderVisible &&
    css`width: calc(100% + 76px);`}
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

const SectionHeaderContent = props => {

  const {
    t,
    folder,
    title,
    onCheck,
    onSelect,
    onClose,
    isHeaderVisible,
    isHeaderChecked,
    isHeaderIndeterminate,
    selection,
    isRecycleBinFolder,
    filter,
    deleteDialogVisible,
    user,
    groupsCaption
  } = props;

  const onCreate = (format) => {
    props.setAction(
      {
        type: FileAction.Create,
        extension: format,
        id: -1
      });
  }

  const createDocument = useCallback(
    () => onCreate('docx'),
    []
  );

  const createSpreadsheet = useCallback(
    () => onCreate('xlsx'),
    []
  );

  const createPresentation = useCallback(
    () => onCreate('pptx'),
    []
  );

  const createFolder = useCallback(
    () => onCreate(),
    []
  );

  const uploadToFolder = useCallback(
    () => toastr.info("Upload To Folder click"),
    []
  );

  const getContextOptionsPlus = useCallback(() => {
    return [
      {
        key: "new-document",
        label: t('NewDocument'),
        onClick: createDocument
      },
      {
        key: "new-spreadsheet",
        label: t('NewSpreadsheet'),
        onClick: createSpreadsheet
      },
      {
        key: "new-presentation",
        label: t('NewPresentation'),
        onClick: createPresentation
      },
      {
        key: "new-folder",
        label: t('NewFolder'),
        onClick: createFolder
      },
      { key: "separator", isSeparator: true },
      {
        key: "make-invitation-link",
        label: t('UploadToFolder'),
        onClick: uploadToFolder,
        disabled: true
      }
    ];
  }, [
    t,
    createDocument,
    createSpreadsheet,
    createPresentation,
    createFolder,
    uploadToFolder
  ]);

  const createLinkForPortalUsers = useCallback(
    () => toastr.info("createLinkForPortalUsers click"),
    []
  );

  const moveAction = useCallback(() => toastr.info("moveAction click"), []);

  const copyAction = useCallback(() => toastr.info("copyAction click"), []);

  const downloadAction = useCallback(
    () => toastr.info("downloadAction click"),
    []
  );

  const downloadAsAction = useCallback(
    () => toastr.info("downloadAsAction click"),
    []
  );

  const renameAction = useCallback(() => toastr.info("renameAction click"), []);

  const [showSharingPanel, setShowSharingPanel] = useState(false);
  const openSharingPanel = useCallback(
    () => setShowSharingPanel(!showSharingPanel),
    [showSharingPanel]
  );


  
  const [showDeleteDialog, setDeleteDialog] = useState(false);
  const onDeleteAction = useCallback(() => setDeleteDialog(!showDeleteDialog), [
    showDeleteDialog
  ]);

  const [showEmptyTrashDialog, setEmptyTrashDialog] = useState(false);
  const onEmptyTrashAction = useCallback(
    () => setEmptyTrashDialog(!showEmptyTrashDialog),
    [showEmptyTrashDialog]
  );

  const [showAddUsersPanel, setShowAddUsersPanel] = useState(false);
  const onShowUsersPanel = useCallback(
    () => setShowAddUsersPanel(!showAddUsersPanel),
    [showAddUsersPanel]
  );

  const [showAddGroupsPanel, setShowAddGroupsPanel] = useState(false);
  const onShowGroupsPanel = useCallback(
    () => setShowAddGroupsPanel(!showAddGroupsPanel),
    [showAddGroupsPanel]
  );

  const getContextOptionsFolder = useCallback(() => {
    return [
      {
        key: "sharing-settings",
        label: t('SharingSettings'),
        onClick: openSharingPanel,
        disabled: true
      },
      {
        key: "link-portal-users",
        label: t('LinkForPortalUsers'),
        onClick: createLinkForPortalUsers,
        disabled: true
      },
      { key: "separator-2", isSeparator: true },
      {
        key: "move-to",
        label: t('MoveTo'),
        onClick: moveAction,
        disabled: true
      },
      {
        key: "copy",
        label: t('Copy'),
        onClick: copyAction,
        disabled: true
      },
      {
        key: "download",
        label: t('Download'),
        onClick: downloadAction,
        disabled: true
      },
      {
        key: "rename",
        label: t('Rename'),
        onClick: renameAction,
        disabled: true
      },
      {
        key: "delete",
        label: t('Delete'),
        onClick: onDeleteAction,
        disabled: true
      }
    ];
  }, [
    t,
    openSharingPanel,
    createLinkForPortalUsers,
    moveAction,
    copyAction,
    downloadAction,
    renameAction,
    onDeleteAction
  ]);

  const onBackToParentFolder = () => {
    fetchFiles(props.parentId, filter, filesStore.dispatch);
  };

  const isItemsSelected = selection.length;
  const isOnlyFolderSelected = selection.every(selected => !selected.fileType);

  const menuItems = [
    {
      label: t("LblSelect"),
      isDropdown: true,
      isSeparator: true,
      isSelect: true,
      fontWeight: "bold",
      children: [
        <DropDownItem key='all' label={t("All")} />,
        <DropDownItem key={FilterType.FoldersOnly} label={t("Folders")} />,
        <DropDownItem key={FilterType.DocumentsOnly} label={t("Documents")} />,
        <DropDownItem key={FilterType.PresentationsOnly} label={t("Presentations")} />,
        <DropDownItem key={FilterType.SpreadsheetsOnly} label={t("Spreadsheets")} />,
        <DropDownItem key={FilterType.ImagesOnly} label={t("Images")} />,
        <DropDownItem key={FilterType.MediaOnly} label={t("Media")} />,
        <DropDownItem key={FilterType.ArchiveOnly} label={t("Archives")} />,
        <DropDownItem key={FilterType.FilesOnly} label={t("AllFiles")} />,
      ],
      onSelect: item => onSelect(item.key)
    },
    {
      label: t("Share"),
      disabled: !isItemsSelected,
      onClick: openSharingPanel
    },
    {
      label: t("Download"),
      disabled: !isItemsSelected,
      onClick: downloadAction
    },
    {
      label: t("DownloadAs"),
      disabled: !isItemsSelected || isOnlyFolderSelected,
      onClick: downloadAsAction
    },
    {
      label: t("MoveTo"),
      disabled: !isItemsSelected,
      onClick: moveAction
    },
    {
      label: t("Copy"),
      disabled: !isItemsSelected,
      onClick: copyAction
    },
    {
      label: t("Delete"),
      disabled: !isItemsSelected || !deleteDialogVisible,
      onClick: onDeleteAction
    }
  ];

  isRecycleBinFolder &&
    menuItems.push({
      label: t("EmptyRecycleBin"),
      onClick: onEmptyTrashAction
    });

    const onFullAccessClick = () => {
      console.log("onFullAccessClick");
      //this.setState({ currentIconName: "AccessEditIcon" });
    };
    const onReadOnlyClick = () => {
      console.log("onReadOnlyClick");
      //this.setState({ currentIconName: "EyeIcon" });
    };
    const onReviewClick = () => {
      console.log("onReviewClick");
      //this.setState({ currentIconName: "AccessReviewIcon" });
    };
    const onCommentClick = () => {
      console.log("onCommentClick");
      //this.setState({ currentIconName: "AccessCommentIcon" });
    };
    const onFormFillingClick = () => {
      console.log("onFormFillingClick");
      //this.setState({ currentIconName: "AccessFormIcon" });
    };
    const onDenyAccessClick = () => {
      console.log("onDenyAccessClick");
      //this.setState({ currentIconName: "AccessNoneIcon" });
    };

    const advancedOptions = (
      <>
        <DropDownItem
          label="Full access"
          icon="AccessEditIcon"
          onClick={onFullAccessClick}
        />
        <DropDownItem
          label="Read only"
          icon="EyeIcon"
          onClick={onReadOnlyClick}
        />
        <DropDownItem
          label="Review"
          icon="AccessReviewIcon"
          onClick={onReviewClick}
        />
        <DropDownItem
          label="Comment"
          icon="AccessCommentIcon"
          onClick={onCommentClick}
        />
        <DropDownItem
          label="Form filling"
          icon="AccessFormIcon"
          onClick={onFormFillingClick}
        />
        <DropDownItem
          label="Deny access"
          icon="AccessNoneIcon"
          onClick={onDenyAccessClick}
        />
      </>
    );

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
              onClick={onBackToParentFolder}
              className="arrow-button"
            />
          )}
          <Headline className="headline-header" type="content" truncate={true}>
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
                getData={getContextOptionsPlus}
                isDisabled={false}
              />
              <ContextMenuButton
                className="option-button"
                directionX="right"
                iconName="VerticalDotsIcon"
                size={16}
                color="#A3A9AE"
                getData={getContextOptionsFolder}
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
              getData={getContextOptionsPlus}
              isDisabled={false}
            />
          )}
        </div>
      )}

      {showDeleteDialog && (
        <DeleteDialog
          isRecycleBinFolder={isRecycleBinFolder}
          visible={showDeleteDialog}
          onClose={onDeleteAction}
          selection={selection}
        />
      )}

      {showEmptyTrashDialog && (
        <EmptyTrashDialog
          visible={showEmptyTrashDialog}
          onClose={onEmptyTrashAction}
        />
      )}

      <SharingPanel
        onClose={openSharingPanel}
        selection={selection}
        visible={showSharingPanel}
        onShowUsersPanel={onShowUsersPanel}
        onShowGroupsPanel={onShowGroupsPanel}
      />

      <AddUsersPanel
        onSharingPanelClose={openSharingPanel}
        onClose={onShowUsersPanel}
        visible={showAddUsersPanel}
        me={user}
        groupsCaption={groupsCaption}
        advancedOptions={advancedOptions}
      />

      <AddGroupPanel
        onSharingPanelClose={openSharingPanel}
        onClose={onShowGroupsPanel}
        visible={showAddGroupsPanel}
        advancedOptions={advancedOptions}
        //me={user}
        //groupsCaption={groupsCaption}
      />
    </StyledContainer>
  );
};

const mapStateToProps = state => {
  const { selectedFolder, selection, treeFolders, filter } = state.files;
  const { parentId, title, id } = selectedFolder;
  const { user, settings } = state.auth;

  const indexOfTrash = 3;

  return {
    folder: parentId !== 0,
    isAdmin: isAdmin(user),
    isRecycleBinFolder: treeFolders[indexOfTrash].id === id,
    parentId,
    selection,
    title,
    filter,
    deleteDialogVisible: isCanBeDeleted(selectedFolder, user),
    groupsCaption: settings.customNames.groupsCaption
  };
};

export default connect(mapStateToProps, {setAction})(
  withTranslation()(withRouter(SectionHeaderContent))
);
