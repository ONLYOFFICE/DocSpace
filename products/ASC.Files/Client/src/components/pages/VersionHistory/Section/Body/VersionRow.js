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
  utils,
  Icons,
} from "asc-web-components";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { api, toastr } from "asc-web-common";
import { setIsLoading } from "../../../../../store/files/actions";
import {
  getFilter,
  getSelectedFolderId,
} from "../../../../../store/files/selectors";

const { tablet } = utils.device;

const StyledRow = styled(Row)`
  min-height: 70px;

  @media ${tablet} {
    min-height: 69px;
  }
  .version_badge {
    cursor: pointer;

    .version_badge-text {
      position: absolute;
      left: -2px;
    }

    margin-left: -8px;
    margin-right: 16px;
    margin-top: ${(props) => (props.showEditPanel ? "13px" : "-2px")};

    @media ${tablet} {
      margin-left: 0px;
      margin-top: 0px;
      .version_badge-text {
        left: 6px;
      }
    }
  }

  .version-link-file {
    margin-top: ${(props) => (props.showEditPanel ? "12px" : "-3px")};
    @media ${tablet} {
      margin-top: -1px;
    }
  }

  .icon-link {
    width: 10px;
    height: 10px;
    margin-left: 9px;
    margin-top: ${(props) => (props.showEditPanel ? "11px" : "-3px")};
    @media ${tablet} {
      margin-top: -1px;
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
    margin-left: 63px;

    @media ${tablet} {
      display: none;
    }
  }

  .textarea-desktop {
    margin: 9px 23px 1px -7px;
  }

  .version_content-length {
    display: block;
    margin-left: auto;
    margin-top: ${(props) => (props.showEditPanel ? "12px" : "-3px")};
    margin-right: -7px;

    @media ${tablet} {
      display: none;
    }
  }

  .version_link {
    display: ${(props) => (props.showEditPanel ? "none" : "block")};
    text-decoration: underline dashed;
    white-space: break-spaces;
    margin-left: -7px;
    margin-top: 4px;

    @media ${tablet} {
      display: none;
      text-decoration: none;
    }
  }

  .version_text {
    display: none;

    @media ${tablet} {
      display: block;
      margin-left: 1px;
      margin-top: 5px;
    }
  }

  .version_link-action {
    display: block;
    margin-left: auto;
    margin-top: 5px;

    :last-child {
      margin-left: 8px;
      margin-right: -7px;
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

  .row_context-menu-wrapper {
    margin-right: -3px;
    margin-top: -25px;
  }

  .version_edit-comment-button-primary {
    margin-right: 8px;
    width: 87px;
  }
  .version_edit-comment-button-second {
    width: 87px;
  }
  .version_modal-dialog .modal-dialog-aside-header {
    border-bottom: unset;
  }
  .version_modal-dialog .modal-dialog-aside-body {
    margin-top: -24px;
  }
`;

const VersionRow = (props) => {
  const {
    info,
    index,
    culture,
    selectedFolderId,
    filter,
    setIsLoading,
    isVersion,
    t,
    getFileVersions,
  } = props;
  const [showEditPanel, setShowEditPanel] = useState(false);
  const [commentValue, setCommentValue] = useState(info.comment);
  const [displayComment, setDisplayComment] = useState(info.comment);

  const VersionBadge = (props) => (
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

  const onChange = (e) => setCommentValue(e.target.value);

  const onSaveClick = () =>
    api.files
      .versionEditComment(info.id, commentValue, info.version)
      .then(() => setDisplayComment(commentValue))
      .catch((err) => toastr.error(err))
      .finally(() => onEditComment());

  const onCancelClick = () => {
    setCommentValue(info.comment);
    setShowEditPanel(!showEditPanel);
  };
  const onOpenFile = () => window.open(info.webUrl);

  const onRestoreClick = () => {
    setIsLoading(true);
    api.files
      .versionRestore(info.id, info.version)
      .then(() => getFileVersions(info.id))
      .catch((err) => toastr.error(err))
      .finally(() => setIsLoading(false));
  };

  const onVersionClick = () => {
    setIsLoading(true);
    api.files
      .markAsVersion(info.id, isVersion, info.version)
      .then(() => getFileVersions(info.id))
      .catch((err) => toastr.error(err))
      .finally(() => setIsLoading(false));
  };

  const contextOptions = [
    { key: "edit", label: t("EditComment"), onClick: onEditComment },
    { key: "restore", label: t("Restore"), onClick: onRestoreClick },
    {
      key: "download",
      label: `${t("Download")} (${info.contentLength})`,
      onClick: onDownloadAction,
    },
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
            className="version-link-file"
          >
            {title}
          </Link>
          <Link className="icon-link" onClick={onOpenFile}>
            <Icons.ExternalLinkIcon color="#333333" size="scale" />
          </Link>
          <Text
            className="version_content-length"
            fontWeight={600}
            color="#A3A9AE"
            fontSize="14px"
          >
            {info.contentLength}
          </Text>
        </Box>
        <Box marginProp="0 0 0 70px" displayProp="flex">
          <>
            {showEditPanel && (
              <>
                <Textarea
                  className="version_edit-comment textarea-desktop"
                  onChange={onChange}
                  fontSize={12}
                  heightTextArea={54}
                  value={commentValue}
                />
                <Box className="version_modal-dialog">
                  <ModalDialog
                    displayType="aside"
                    visible={showEditPanel}
                    onClose={onEditComment}
                  >
                    <ModalDialog.Header className="header-version-modal-dialog">
                      {t("EditComment")}
                    </ModalDialog.Header>
                    <ModalDialog.Body>
                      <Textarea
                        className="text-area-mobile-edit-comment"
                        style={{ margin: "8px 24px 8px 0" }}
                        //placeholder="Add comment"
                        onChange={onChange}
                        heightTextArea={298}
                        value={commentValue}
                      />
                    </ModalDialog.Body>
                    <ModalDialog.Footer>
                      <Button
                        className="version_save-button"
                        label={t("AddButton")}
                        size="big"
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
            <Box
              className="version_edit-comment-button-primary"
              displayProp="inline-block"
            >
              <Button
                size="base"
                scale={true}
                primary
                onClick={onSaveClick}
                label={t("AddButton")}
              />
            </Box>
            <Box
              className="version_edit-comment-button-second"
              displayProp="inline-block"
            >
              <Button
                size="base"
                scale={true}
                onClick={onCancelClick}
                label={t("CancelButton")}
              />
            </Box>
          </Box>
        )}
      </>
    </StyledRow>
  );
};

const mapStateToProps = (state) => {
  return {
    filter: getFilter(state),
    selectedFolderId: getSelectedFolderId(state),
  };
};

export default connect(mapStateToProps, {
  setIsLoading,
})(withRouter(withTranslation()(VersionRow)));
