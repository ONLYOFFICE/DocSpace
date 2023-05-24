import React, { useState, useEffect } from "react";
import styled from "styled-components";
import Link from "@docspace/components/link";
import Text from "@docspace/components/text";
import Box from "@docspace/components/box";
import Textarea from "@docspace/components/textarea";
import Button from "@docspace/components/button";
import ModalDialog from "@docspace/components/modal-dialog";
import { withTranslation } from "react-i18next";
import VersionBadge from "./VersionBadge";
import { StyledVersionRow } from "./StyledVersionHistory";
import ExternalLinkIcon from "PUBLIC_DIR/images/external.link.react.svg";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import { inject, observer } from "mobx-react";
import toastr from "@docspace/components/toast/toastr";
import { Encoder } from "@docspace/common/utils/encoder";
import { Base } from "@docspace/components/themes";

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
    canChangeVersionFileHistory,
  } = props;
  const [showEditPanel, setShowEditPanel] = useState(false);
  const [commentValue, setCommentValue] = useState(info.comment);
  const [isSavingComment, setIsSavingComment] = useState(false);

  const title = `${new Date(info.updated).toLocaleString(
    culture
  )} ${Encoder.htmlDecode(info.updatedBy?.displayName)}`;

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
    canChangeVersionFileHistory && {
      key: "edit",
      label: t("EditComment"),
      onClick: onEditComment,
    },
    index !== 0 &&
      canChangeVersionFileHistory && {
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

  const onClickProp = canChangeVersionFileHistory
    ? { onClick: onVersionClick }
    : {};

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
      canEdit={canChangeVersionFileHistory}
      isTabletView={isTabletView}
      isSavingComment={isSavingComment}
      isEditing={isEditing}
      contextTitle={t("Common:Actions")}
    >
      <div className={`version-row_${index}`}>
        <Box displayProp="flex" className="row-header">
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
            title={
              index > 0
                ? isVersion
                  ? t("Files:MarkAsRevision")
                  : t("Files:MarkAsVersion")
                : ""
            }
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

          {/*<Text
            className="version_content-length"
            fontWeight={600}
            color={theme.filesVersionHistory.color}
            fontSize="14px"
          >
            {info.contentLength}
          </Text>*/}
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
                  className="version_edit-comment"
                  onChange={onChange}
                  fontSize={12}
                  heightTextArea={54}
                  value={commentValue}
                  isDisabled={isSavingComment}
                  autoFocus={true}
                  areaSelect={true}
                />
              </>
            )}

            <Text className="version_text">{info.comment}</Text>
          </>
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

export default inject(({ auth, versionHistoryStore, selectedFolderStore }) => {
  const { user } = auth.userStore;
  const { culture, isTabletView } = auth.settingsStore;
  const language = (user && user.cultureName) || culture || "en";

  const {
    markAsVersion,
    restoreVersion,
    updateCommentVersion,
    isEditing,
    isEditingVersion,
    fileSecurity,
  } = versionHistoryStore;

  const isEdit = isEditingVersion || isEditing;
  const canChangeVersionFileHistory = !isEdit && fileSecurity?.EditHistory;

  return {
    theme: auth.settingsStore.theme,
    culture: language,
    isTabletView,
    markAsVersion,
    restoreVersion,
    updateCommentVersion,
    isEditing: isEdit,
    canChangeVersionFileHistory,
  };
})(
  withTranslation(["VersionHistory", "Common", "Translations"])(
    observer(VersionRow)
  )
);
