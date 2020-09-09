import React, { useState } from "react";
import styled from "styled-components";
import {
  Row,
  Link,
  Text,
  Box,
  Textarea,
  Button,
  ModalDialog,
  toastr,
  utils
} from "asc-web-components";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { api } from "asc-web-common";
import { fetchFiles } from "../../../../../store/files/actions";
import store from "../../../../../store/store";

const { tablet } = utils.device;

const StyledRow = styled(Row)`
  .version_badge {
    cursor: pointer;

    .version_badge-text {
      position: absolute;
      left: 16px;
    }
  }

  .version_modal-dialog {
    display: none;

    @media ${tablet} {
      display: block;
    }
  }

  .version_edit-comment {
    display: block;

    @media ${tablet} {
      display: none;
    }
  }

  .version_content-length {
    display: block;
    margin-left: auto;

    @media ${tablet} {
      display: none;
    }
  }

  .version_link {
    display: ${props => (props.showEditPanel ? "none" : "block")};
    text-decoration: underline dashed;
    white-space: break-spaces;

    @media ${tablet} {
      display: none;
      text-decoration: none;
    }
  }

  .version_text {
    display: none;

    @media ${tablet} {
      display: block;
    }
  }

  .version_link-action {
    display: block;
    margin-left: auto;

    :last-child {
      margin-left: 8px;
    }

    @media ${tablet} {
      display: none;
    }
  }

  .row_context-menu-wrapper {
    display: none;

    @media ${tablet} {
      display: block;
    }
  }

  .row_content {
    display: block;
  }

  .modal-dialog-aside-footer {
    width: 90%;

    .version_save-button {
      width: 100%;
    }
  }
`;

const VersionRow = props => {
  const {
    info,
    index,
    culture,
    selectedFolderId,
    filter,
    onLoading,
    isVersion,
    t,
    getFileVersions
  } = props;
  const [showEditPanel, setShowEditPanel] = useState(false);
  const [commentValue, setCommentValue] = useState(info.comment);
  const [displayComment, setDisplayComment] = useState(info.comment);

  const VersionBadge = props => (
    <Box {...props} marginProp="0 8px" displayProp="flex">
      <svg
        width="55"
        height="18"
        viewBox="0 0 55 18"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
        stroke={isVersion ? "none" : "#A3A9AE"}
        strokeDasharray={isVersion ? "none" : "2px"}
        strokeWidth={isVersion ? "none" : "2px"}
      >
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M0 1C0 0.447716 0.447715 0 1 0L53.9994 0C54.6787 0 55.1603 0.662806 54.9505 1.3089L52.5529 8.6911C52.4877 8.89187 52.4877 9.10813 52.5529 9.3089L54.9505 16.6911C55.1603 17.3372 54.6787 18 53.9994 18H0.999999C0.447714 18 0 17.5523 0 17V1Z"
          fill={!isVersion ? "#FFF" : index === 0 ? "#A3A9AE" : "#ED7309"}
        />
      </svg>
      <Text className="version_badge-text" color="#FFF" isBold fontSize="12px">
        {isVersion && `Ver.${info.versionGroup}`}
      </Text>
    </Box>
  );

  const title = `${new Date(info.created).toLocaleString(culture)} ${
    info.createdBy.displayName
  }`;

  const linkStyles = { isHovered: true, type: "action" };

  const onDownloadAction = () =>
    window.open(`${info.viewUrl}&version=${info.version}`);
  const onEditComment = () => setShowEditPanel(!showEditPanel);

  const onChange = e => setCommentValue(e.target.value);

  const onSaveClick = () =>
    api.files
      .versionEditComment(info.id, commentValue, info.version)
      .then(() => setDisplayComment(commentValue))
      .catch(err => toastr.error(err))
      .finally(() => onEditComment());

  const onCancelClick = () => {
    setCommentValue(info.comment);
    setShowEditPanel(!showEditPanel);
  };
  const onOpenFile = () => window.open(info.webUrl);

  const onRestoreClick = () => {
    onLoading(true);
    api.files
      .versionRestore(info.id, info.version)
      .then(() => getFileVersions(info.id))
      .catch(err => toastr.error(err))
      .finally(() => onLoading(false));
  };

  const onVersionClick = () => {
    onLoading(true);
    api.files
      .markAsVersion(info.id, isVersion, info.version)
      .then(() => getFileVersions(info.id))
      .catch(err => toastr.error(err))
      .finally(() => onLoading(false));
  };

  const contextOptions = [
    { key: "edit", label: t("EditComment"), onClick: onEditComment },
    { key: "restore", label: t("Restore"), onClick: onRestoreClick },
    {
      key: "download",
      label: `${t("Download")}(${info.contentLength})`,
      onClick: onDownloadAction
    }
  ];

  return (
    <StyledRow showEditPanel={showEditPanel} contextOptions={contextOptions}>
      <>
        <Box displayProp="flex">
          <VersionBadge className="version_badge" onClick={onVersionClick} />
          <Link
            onClick={onOpenFile}
            fontWeight={600}
            fontSize="14px"
            title={title}
          >
            {title}
          </Link>
          <Text
            className="version_content-length"
            fontWeight={600}
            color="#A3A9AE"
          >
            {info.contentLength}
          </Text>
        </Box>
        <Box marginProp="0 0 0 70px" displayProp="flex">
          <>
            {showEditPanel && (
              <>
                <Textarea
                  className="version_edit-comment"
                  style={{ margin: "8px 24px 8px 0" }}
                  //placeholder="Add comment"
                  onChange={onChange}
                  value={commentValue}
                />
                <Box className="version_modal-dialog">
                  <ModalDialog
                    displayType="aside"
                    visible={showEditPanel}
                    onClose={onEditComment}
                  >
                    <ModalDialog.Header>{t("EditComment")}</ModalDialog.Header>
                    <ModalDialog.Body>
                      <Textarea
                        //className="version_edit-comment"
                        style={{ margin: "8px 24px 8px 0" }}
                        //placeholder="Add comment"
                        onChange={onChange}
                        value={commentValue}
                      />
                    </ModalDialog.Body>
                    <ModalDialog.Footer>
                      <Button
                        className="version_save-button"
                        label={t("AddButton")}
                        size="medium"
                        primary
                        onClick={onSaveClick}
                      />
                    </ModalDialog.Footer>
                  </ModalDialog>
                </Box>
              </>
            )}
            <Link onClick={onEditComment} className="version_link">
              {displayComment}
            </Link>
            <Text className="version_text">{displayComment}</Text>
          </>

          <Link
            onClick={onRestoreClick}
            {...linkStyles}
            className="version_link-action"
          >
            {t("Restore")}
          </Link>
          <Link
            onClick={onDownloadAction}
            {...linkStyles}
            className="version_link-action"
          >
            {t("Download")}
          </Link>
        </Box>
        {showEditPanel && (
          <Box className="version_edit-comment" marginProp="8px 0 16px 70px">
            <Button
              size="medium"
              primary
              style={{ marginRight: "8px" }}
              onClick={onSaveClick}
              label={t("AddButton")}
            />
            <Button
              size="medium"
              onClick={onCancelClick}
              label={t("CancelButton")}
            />
          </Box>
        )}
      </>
    </StyledRow>
  );
};

const mapStateToProps = state => {
  const { selectedFolder } = state.files;
  const { id } = selectedFolder;

  return {
    filter: state.files.filter,
    selectedFolderId: id
  };
};

export default connect(
  mapStateToProps,
  {}
)(withRouter(withTranslation()(VersionRow)));
