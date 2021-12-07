import React, { useState, useEffect } from "react";
import styled from "styled-components";
import Badge from "@appserver/components/badge";
import IconButton from "@appserver/components/icon-button";
import { StyledFileActionsConvertEditDocIcon } from "./Icons";
import SharedButton from "@appserver/files/src/components/SharedButton";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";

export const StyledIcon = styled(IconButton)`
  ${commonIconsStyles}
`;

const Badges = ({
  t,
  newItems,
  item,
  canWebEdit,
  isTrashFolder,
  isPrivacyFolder,
  isDesktopClient,
  canConvert,
  accessToEdit,
  showNew,
  showShare,
  onFilesClick,
  onClickLock,
  onClickFavorite,
  onShowVersionHistory,
  onBadgeClick,
  setConvertDialogVisible,
}) => {
  const {
    id,
    locked,
    fileStatus,
    version,
    versionGroup,
    title,
    fileExst,
  } = item;

  const isFavorite = fileStatus === 32;
  const isEditing = fileStatus === 1;
  const isNewWithFav = fileStatus === 34;
  const isEditingWithFav = fileStatus === 33;
  const showEditBadge = !locked || item.access === 0;
  const isPrivacy = isPrivacyFolder && isDesktopClient;

  const showFavorite = isFavorite || isNewWithFav || isEditingWithFav;
  const showActionsEdit = isEditing || isEditingWithFav;

  return fileExst ? (
    <div className="badges additional-badges">
      {version > 1 && (
        <Badge
          className="badge-version icons-group"
          backgroundColor="#A3A9AE"
          borderRadius="11px"
          color="#FFFFFF"
          fontSize="10px"
          fontWeight={800}
          label={`V.${versionGroup}`}
          maxWidth="50px"
          onClick={onShowVersionHistory}
          padding="0 5px"
          data-id={id}
        />
      )}
      {canConvert && !isTrashFolder && (
        <IconButton
          onClick={setConvertDialogVisible}
          iconName="/static/images/refresh.react.svg"
          className="badge icons-group can-convert"
          size="small"
          isfill={true}
          color="#A3A9AE"
          hoverColor="#3B72A7"
        />
      )}
      {canWebEdit &&
        !isTrashFolder &&
        !isPrivacy &&
        accessToEdit &&
        showEditBadge &&
        !canConvert && (
          <StyledIcon
            iconName={
              showActionsEdit
                ? "/static/images/file.actions.convert.edit.doc.react.svg"
                : "/static/images/access.edit.react.svg"
            }
            className="badge icons-group"
            size="medium"
            onClick={onFilesClick}
            hoverColor="#3B72A7"
          />
        )}
      {item.canShare && showShare ? (
        <SharedButton
          t={t}
          id={item.id}
          shared={item.shared}
          isFolder={item.isFolder}
        />
      ) : null}
      {accessToEdit && !isTrashFolder && (
        <StyledIcon
          iconName={
            locked
              ? "/static/images/file.actions.locked.react.svg"
              : "/static/images/locked.react.svg"
          }
          className="badge lock-file icons-group"
          size="medium"
          data-id={id}
          data-locked={locked ? true : false}
          onClick={onClickLock}
          hoverColor="#3B72A7"
        />
      )}
      {!isTrashFolder && (
        <StyledIcon
          iconName={
            showFavorite
              ? "/static/images/file.actions.favorite.react.svg"
              : "/static/images/favorite.react.svg"
          }
          className="favorite badge icons-group"
          size="medium"
          data-id={id}
          data-title={title}
          onClick={() => onClickFavorite(showFavorite)}
          hoverColor="#3B72A7"
        />
      )}
      {(showNew || isNewWithFav) && (
        <Badge
          className="badge-version icons-group"
          backgroundColor="#ED7309"
          borderRadius="11px"
          color="#FFFFFF"
          fontSize="10px"
          fontWeight={800}
          label={t("New")}
          maxWidth="50px"
          onClick={onBadgeClick}
          padding="0 5px"
          data-id={id}
        />
      )}
    </div>
  ) : (
    showNew && (
      <Badge
        className="new-items"
        backgroundColor="#ED7309"
        borderRadius="11px"
        color="#FFFFFF"
        fontSize="10px"
        fontWeight={800}
        label={newItems}
        maxWidth="50px"
        onClick={onBadgeClick}
        padding="0 5px"
        data-id={id}
      />
    )
  );
};

export default Badges;
