import React, { useCallback } from "react";
import styled, { css } from "styled-components";
import { withRouter } from "react-router";
import { Headline, store } from 'asc-web-common';
import { connect } from "react-redux";
import { withTranslation } from "react-i18next";
import {
  toastr,
  ContextMenuButton
} from "asc-web-components";

const { isAdmin } = store.auth.selectors;

const StyledContainer = styled.div`

  position: relative;
  display: flex;
  align-items: center;
  max-width: calc(100vw - 32px);

  @media (min-width: 1024px) {
    ${props => props.isHeaderVisible && css`width: calc(100% + 76px);`}
  }

    .action-button {
      margin-bottom: -1px;
      margin-left: 16px;

      @media (max-width: 1024px) {
        margin-left: auto;

        & > div:first-child {
          padding: 8px 16px 8px 16px;
          margin-right: -16px;
        }
      }
    }
`;

const SectionHeaderContent = props => {

  const { t, folder, title } = props;

  const createDocument = useCallback(
    () => toastr.info("New Document click"),
    []
  );

  const createSpreadsheet = useCallback(
    () => toastr.info("New Spreadsheet click"),
    []
  );

  const createPresentation = useCallback(
    () => toastr.info("New Presentation click"),
    []
  );

  const createFolder = useCallback(
    () => toastr.info("New Folder click"),
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
        onClick: uploadToFolder
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

  const renameAction = useCallback(
    () => toastr.info("renameAction click"),
    []
  );

  const deleteAction = useCallback(
    () => toastr.info("deleteAction click"),
    []
  );


  const getContextOptionsFolder = useCallback(() => {
    return [
      {
        key: "sharing-settings",
        label: t('SharingSettings'),
        onClick: openSharingSettings
      },
      {
        key: "link-portal-users",
        label: t('LinkForPortalUsers'),
        onClick: createLinkForPortalUsers
      },
      { key: "separator-2", isSeparator: true },
      {
        key: "move-to",
        label: t('MoveTo'),
        onClick: moveAction
      },
      {
        key: "copy",
        label: t('Copy'),
        onClick: copyAction
      },
      {
        key: "download",
        label: t('Download'),
        onClick: downloadAction
      },
      {
        key: "rename",
        label: t('Rename'),
        onClick: renameAction
      },
      {
        key: "delete",
        label: t('Delete'),
        onClick: deleteAction
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
    deleteAction
  ]);

  return (
    <StyledContainer isHeaderVisible={true}>
      <Headline className='headline-header' type="content" truncate={true}>{title}</Headline>
      {folder ? (
        <>
          <ContextMenuButton
            className="action-button"
            directionX="right"
            iconName="PlusIcon"
            size={16}
            color="#657077"
            getData={getContextOptionsPlus}
            isDisabled={false}
          />

          <ContextMenuButton
            className="action-button"
            directionX="right"
            iconName="VerticalDotsIcon"
            size={16}
            color="#A3A9AE"
            getData={getContextOptionsFolder}
            isDisabled={false}
          />
        </>
      ) : (
          <>
            <ContextMenuButton
              className="action-button"
              directionX="right"
              iconName="PlusIcon"
              size={16}
              color="#657077"
              getData={getContextOptionsPlus}
              isDisabled={false}
            />
          </>
        )}
    </StyledContainer>
  );
};

const mapStateToProps = state => {
  return {
    isAdmin: isAdmin(state.auth.user),
    title: state.files.selectedFolder.title,
    folder: state.files.selectedFolder.parentId !== 0
  };
};

export default connect(
  mapStateToProps
)(withTranslation()(withRouter(SectionHeaderContent)));
