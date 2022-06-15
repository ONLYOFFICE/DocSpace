import React, { useState, useEffect } from "react";
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
import { StyledVersionRow } from "./StyledVersionHistory";
import ExternalLinkIcon from "../../../../../public/images/external.link.react.svg";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import { inject, observer } from "mobx-react";
import toastr from "studio/toastr";
import { Encoder } from "@appserver/common/utils/encoder";
import { Base } from "@appserver/components/themes";

const StyledExternalLinkIcon = styled(ExternalLinkIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.theme.filesVersionHistory.fill};
  }
`;

StyledExternalLinkIcon.defaultProps = { theme: Base };
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
    onSetRestoreProcess,
    isTabletView,
    onUpdateHeight,
    versionsListLength,
    isEditing,
    theme,
  } = props;
  const [showEditPanel, setShowEditPanel] = useState(false);
  const [commentValue, setCommentValue] = useState(info.comment);
  const [isSavingComment, setIsSavingComment] = useState(false);

  const canEdit = (info.access === 1 || info.access === 0) && !isEditing;

  const title = `${new Date(info.updated).toLocaleString(
    culture
  )} ${Encoder.htmlDecode(info.updatedBy.displayName)}`;

  const linkStyles = { isHovered: true, type: "action" };

  const onDownloadAction = () =>
    window.open(`${info.viewUrl}&version=${info.version}`, "_self");
  const onEditComment = () => !isEditing && setShowEditPanel(!showEditPanel);

  const onChange = (e) => setCommentValue(e.target.value);

  const onSaveClick = () => {
    setIsSavingComment(true);
    updateCommentVersion(info.id, commentValue, info.version)
      .catch((err) => toastr.error(err))
      .finally(() => {
        onEditComment();
        setIsSavingComment(false);
      });
  };

  const onCancelClick = () => {
    setCommentValue(info.comment);
    setShowEditPanel(!showEditPanel);
  };
  const onOpenFile = () => window.open(info.webUrl);

  const onRestoreClick = () => {
    onSetRestoreProcess(true);
    restoreVersion(info.id, info.version)
      .catch((err) => toastr.error(err))
      .finally(() => {
        onSetRestoreProcess(false);
      });
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
      label: t("Common:Restore"),
      onClick: onRestoreClick,
    },
    {
      key: "download",
      label: `${t("Common:Download")} (${info.contentLength})`,
      onClick: onDownloadAction,
    },
  ];

  const onClickProp = canEdit ? { onClick: onVersionClick } : {};

  useEffect(() => {
    const newRowHeight = document.getElementsByClassName(
      `version-row_${index}`
    )[0]?.clientHeight;

    newRowHeight && onUpdateHeight(index, newRowHeight);
  }, [showEditPanel, versionsListLength]);

  return (
    <StyledVersionRow
      showEditPanel={showEditPanel}
      contextOptions={contextOptions}
      canEdit={canEdit}
      isTabletView={isTabletView}
      isSavingComment={isSavingComment}
      isEditing={isEditing}
    >
      <div className={`version-row_${index}`}>
        <Box displayProp="flex">
          <VersionBadge
            theme={theme}
            className={`version_badge ${
              isVersion ? "versioned" : "not-versioned"
            }`}
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
            color={theme.filesVersionHistory.color}
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
                  isDisabled={isSavingComment}
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
                        isDisabled={isSavingComment}
                      />
                    </ModalDialog.Body>
                    <ModalDialog.Footer>
                      <Button
                        isDisabled={isSavingComment}
                        className="version_save-button"
                        label={t("Common:SaveButton")}
                        size="normal"
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
              isHovered={!isEditing}
              noHover={isEditing}
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
                isDisabled={isSavingComment}
                size="extraSmall"
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
                isDisabled={isSavingComment}
                size="extraSmall"
                scale={true}
                onClick={onCancelClick}
                label={t("Common:CancelButton")}
              />
            </Box>
          </Box>
        )}
      </div>
    </StyledVersionRow>
  );
};

export default inject(({ auth, versionHistoryStore }) => {
  const { user } = auth.userStore;
  const { culture, isTabletView } = auth.settingsStore;
  const language = (user && user.cultureName) || culture || "en";

  const {
    markAsVersion,
    restoreVersion,
    updateCommentVersion,
    isEditing,
    isEditingVersion,
  } = versionHistoryStore;

  return {
    theme: auth.settingsStore.theme,
    culture: language,
    isTabletView,
    markAsVersion,
    restoreVersion,
    updateCommentVersion,
    isEditing: isEditingVersion || isEditing,
  };
})(
  withRouter(
    withTranslation(["VersionHistory", "Common", "Translations"])(
      observer(VersionRow)
    )
  )
);
