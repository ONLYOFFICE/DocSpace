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
import { api, toastr, store } from "asc-web-common";
import { setIsLoading } from "../../../../../store/files/actions";
import VersionBadge from "./VersionBadge";
import StyledVersionRow from "./StyledVersionRow";

const { getLanguage } = store.auth.selectors;

const VersionRow = (props) => {
  const {
    info,
    index,
    culture,
    setIsLoading,
    isVersion,
    t,
    getFileVersions,
  } = props;
  const [showEditPanel, setShowEditPanel] = useState(false);
  const [commentValue, setCommentValue] = useState(info.comment);
  const [displayComment, setDisplayComment] = useState(info.comment);

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
    <StyledVersionRow
      showEditPanel={showEditPanel}
      contextOptions={contextOptions}
    >
      <>
        <Box displayProp="flex">
          <VersionBadge
            className="version_badge"
            isVersion={isVersion}
            index={index}
            versionGroup={info.versionGroup}
            onClick={onVersionClick}
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
    </StyledVersionRow>
  );
};

const mapStateToProps = (state) => {
  return {
    culture: getLanguage(state),
  };
};

export default connect(mapStateToProps, {
  setIsLoading,
})(withRouter(withTranslation()(VersionRow)));
