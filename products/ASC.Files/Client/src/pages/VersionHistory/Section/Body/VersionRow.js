import React, { useState } from "react";
import styled from "styled-components";
import Link from "@appserver/components/link";
import Text from "@appserver/components/text";
import Box from "@appserver/components/box";
import Textarea from "@appserver/components/textarea";
import Button from "@appserver/components/button";
import ModalDialog from "@appserver/components/modal-dialog";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import VersionBadge from "./VersionBadge";
import StyledVersionRow from "./StyledVersionRow";
import ExternalLinkIcon from "../../../../../public/images/external.link.react.svg";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import { inject, observer } from "mobx-react";
import toastr from "studio/toastr";

const StyledExternalLinkIcon = styled(ExternalLinkIcon)`
  ${commonIconsStyles}
  path {
    fill: "#333333";
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

  const title = `${new Date(info.updated).toLocaleString(culture)} ${
    info.updatedBy.displayName
  }`;

  const linkStyles = { isHovered: true, type: "action" };

  const onDownloadAction = () =>
    window.open(`${info.viewUrl}&version=${info.version}`, "_self");
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
    canEdit && {
      key: "restore",
      label: t("Translations:Restore"),
      onClick: onRestoreClick,
    },
    {
      key: "download",
      label: `${t("Common:Download")} (${info.contentLength})`,
      onClick: onDownloadAction,
    },
  ];

  const onClickProp = canEdit ? { onClick: onVersionClick } : {};
  return (
    <StyledVersionRow
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
            t={t}
          />
          <Link
            onClick={onOpenFile}
            fontWeight={600}
            fontSize="14px"
            title={title}
            isTextOverflow={true}
            className="version-link-file"
          >
            {title}
          </Link>
          <Link className="icon-link" onClick={onOpenFile}>
            <StyledExternalLinkIcon size="scale" />
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
                        label={t("Common:SaveButton")}
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
                {t("Translations:Restore")}
              </Link>
            )}
            <Link
              onClick={onDownloadAction}
              {...linkStyles}
              className="version_link-action"
            >
              {t("Common:Download")}
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
                label={t("Common:SaveButton")}
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
                label={t("Common:CancelButton")}
              />
            </Box>
          </Box>
        )}
      </>
    </StyledVersionRow>
  );
};

export default inject(({ auth, versionHistoryStore }) => {
  const { user } = auth.userStore;
  const { culture } = auth.settingsStore;
  const language = (user && user.cultureName) || culture || "en-US";

  const {
    markAsVersion,
    restoreVersion,
    updateCommentVersion,
  } = versionHistoryStore;

  return {
    culture: language,

    markAsVersion,
    restoreVersion,
    updateCommentVersion,
  };
})(
  withRouter(
    withTranslation(["VersionHistory", "Common", "Translations"])(
      observer(VersionRow)
    )
  )
);
