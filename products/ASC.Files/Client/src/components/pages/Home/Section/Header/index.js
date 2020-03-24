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
import { fetchFiles } from "../../../../../store/files/actions";
import { default as filesStore } from "../../../../../store/store";
import { EmptyTrashDialog, DeleteDialog } from '../../../../dialogs';
import { isCanBeDeleted } from "../../../../../store/files/selectors";

const { isAdmin } = store.auth.selectors;
const { FilterType } = constants;

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
    onCreate,
    onCheck,
    onSelect,
    onClose,
    isHeaderVisible,
    isHeaderChecked,
    isHeaderIndeterminate,
    selection,
    isRecycleBinFolder,
    filter,
    deleteDialogVisible
  } = props;

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
    () => onCreate('folder'),
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

  const openSharingSettings = useCallback(
    () => toastr.info("openSharingSettings click"),
    []
  );

  const createLinkForPortalUsers = useCallback(
    () => toastr.info("createLinkForPortalUsers click"),
    []
  );

  const moveAction = useCallback(
    () => toastr.info("moveAction click"),
    []
  );

  const copyAction = useCallback(
    () => toastr.info("copyAction click"),
    []
  );

  const downloadAction = useCallback(
    () => toastr.info("downloadAction click"),
    []
  );

  const downloadAsAction = useCallback(
    () => toastr.info("downloadAsAction click"),
    []
  );

  const renameAction = useCallback(
    () => toastr.info("renameAction click"),
    []
  );


  
  const [showDeleteDialog, setDeleteDialog] = useState(false);
  const onDeleteAction = useCallback(() => setDeleteDialog(!showDeleteDialog), [showDeleteDialog]);

  const [showEmptyTrashDialog, setEmptyTrashDialog] = useState(false);
  const onEmptyTrashAction = useCallback(() => setEmptyTrashDialog(!showEmptyTrashDialog), [showEmptyTrashDialog]);



  const getContextOptionsFolder = useCallback(() => {
    return [
      {
        key: "sharing-settings",
        label: t('SharingSettings'),
        onClick: openSharingSettings,
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
    openSharingSettings,
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
      onClick: openSharingSettings
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
    </StyledContainer>
  );
};

const mapStateToProps = state => {
  const { selectedFolder, selection, treeFolders, filter } = state.files;
  const { parentId, title, id } = selectedFolder;
  const { user } = state.auth;

  const indexOfTrash = 3;

  return {
    folder: parentId !== 0,
    isAdmin: isAdmin(user),
    isRecycleBinFolder: treeFolders[indexOfTrash].id === id,
    parentId,
    selection,
    title: title,
    filter,

    deleteDialogVisible: isCanBeDeleted(selectedFolder, user)
  };
};

export default connect(mapStateToProps)(
  withTranslation()(withRouter(SectionHeaderContent))
);
