import React, { useState } from "react";
import {
  Link,
  Text,
  Box,
  Textarea,
  Button,
  ModalDialog,
  Icons,
} from "asc-web-components";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { toastr, store } from "asc-web-common";
import {
  markAsVersion,
  restoreVersion,
  updateCommentVersion,
} from "../../../../../store/files/actions";
import VersionBadge from "./VersionBadge";
import StyledVersionRow from "./StyledVersionRow";

const { getLanguage } = store.auth.selectors;

const StyledRow = styled(Row)`
  min-height: 70px;

  @media ${tablet} {
    min-height: 69px;
  }
  .version_badge {
    cursor: ${(props) => (props.canEdit ? "pointer" : "default")};

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
    display: ${(props) =>
      props.showEditPanel ? "none" : props.canEdit ? "block" : "none"};
    white-space: break-spaces;
    margin-left: -7px;
    margin-top: 4px;

    @media ${tablet} {
      display: none;
      text-decoration: none;
    }
  }

  .version_text {
    display: ${(props) => (props.canEdit ? "none" : "block")};
    margin-left: -7px;
    margin-top: 5px;

    @media ${tablet} {
      display: block;
      margin-left: 1px;
      margin-top: 5px;
    }
  }

  .version_links-container {
    display: flex;
    margin-left: auto;

    .version_link-action {
      display: block;
      margin-top: 5px;

      :last-child {
        margin-right: -7px;
        margin-left: 8px;
      }

      @media ${tablet} {
        display: none;
      }
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
    isVersion,
    t,
    markAsVersion,
    restoreVersion,
    updateCommentVersion,
  } = props;
  const [showEditPanel, setShowEditPanel] = useState(false);
  const [commentValue, setCommentValue] = useState(info.comment);

  const canEdit = info.access === 1 || info.access === 0;

  const title = `${new Date(info.created).toLocaleString(culture)} ${
    info.createdBy.displayName
  }`;

  const linkStyles = { isHovered: true, type: "action" };

  const onDownloadAction = () =>
    window.open(`${info.viewUrl}&version=${info.version}`);
  const onEditComment = () => setShowEditPanel(!showEditPanel);

  const onChange = (e) => setCommentValue(e.target.value);

  const onSaveClick = () => {
    updateCommentVersion(info.id, commentValue, info.version)
      .catch((err) => toastr.error(err))
      .finally(() => {
        onEditComment();
      });
  };

  const onCancelClick = () => {
    setCommentValue(info.comment);
    setShowEditPanel(!showEditPanel);
  };
  const onOpenFile = () => window.open(info.webUrl);

  const onRestoreClick = () => {
    restoreVersion(info.id, info.version).catch((err) => toastr.error(err));
  };

  const onVersionClick = () => {
    markAsVersion(info.id, isVersion, info.version).catch((err) =>
      toastr.error(err)
    );
  };

  const contextOptions = [
    canEdit && { key: "edit", label: t("EditComment"), onClick: onEditComment },
    canEdit && { key: "restore", label: t("Restore"), onClick: onRestoreClick },
    {
      key: "download",
      label: `${t("Download")} (${info.contentLength})`,
      onClick: onDownloadAction,
    },
  ];

  const onClickProp = canEdit ? { onClick: onVersionClick } : {};

  return (
    <StyledRow
      showEditPanel={showEditPanel}
      contextOptions={contextOptions}
      canEdit={canEdit}
    >
      <>
        <Box displayProp="flex">
          <VersionBadge
            className="version_badge"
            isVersion={isVersion}
            index={index}
            versionGroup={info.versionGroup}
            {...onClickProp}
          />
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
        <Box
          className="version-comment-wrapper"
          marginProp="0 0 0 70px"
          displayProp="flex"
        >
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

            <Link
              type="action"
              isHovered
              onClick={onEditComment}
              className="version_link"
            >
              {info.comment}
            </Link>
            <Text className="version_text">{info.comment}</Text>
          </>

          <div className="version_links-container">
            {canEdit && (
              <Link
                onClick={onRestoreClick}
                {...linkStyles}
                className="version_link-action"
              >
                {t("Restore")}
              </Link>
            )}
            <Link
              onClick={onDownloadAction}
              {...linkStyles}
              className="version_link-action"
            >
              {t("Download")}
            </Link>
          </div>
        </Box>
        {showEditPanel && (
          <Box className="version_edit-comment" marginProp="8px 0 2px 70px">
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
    </StyledVersionRow>
  );
};

const mapStateToProps = (state) => {
  return {
    culture: getLanguage(state),
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    markAsVersion: (id, isVersion, version) =>
      dispatch(markAsVersion(id, isVersion, version)),
    restoreVersion: (id, version) => dispatch(restoreVersion(id, version)),
    updateCommentVersion: (id, comment, version) =>
      dispatch(updateCommentVersion(id, comment, version)),
  };
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(withRouter(withTranslation()(VersionRow)));
