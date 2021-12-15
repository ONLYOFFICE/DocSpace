import React from "react";
import styled from "styled-components";
import Badge from "@appserver/components/badge";
import IconButton from "@appserver/components/icon-button";
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
  onFilesClick,
  onShowVersionHistory,
  onBadgeClick,
  setConvertDialogVisible,
}) => {
  const { id, locked, fileStatus, version, versionGroup, fileExst } = item;

  const isEditing = fileStatus === 1;
  const isNewWithFav = fileStatus === 34;
  const isEditingWithFav = fileStatus === 33;
  const showEditBadge = !locked || item.access === 0;
  const isPrivacy = isPrivacyFolder && isDesktopClient;
  const showActionsEdit = isEditing || isEditingWithFav;

  const onSharedVersionClick = () => {
    onBadgeClick(), onShowVersionHistory();
  };

  const iconEdit = showActionsEdit
    ? "/static/images/file.actions.convert.edit.doc.react.svg"
    : "/static/images/access.edit.react.svg";

  const contentBadgeVersion =
    `V.${versionGroup}` > 999 ? `V.999+` : `V.${versionGroup}`;

  const contentNewItems = newItems > 999 ? "999+" : newItems;

  return fileExst ? (
    <div className="badges additional-badges">
      {canWebEdit &&
        !isTrashFolder &&
        !isPrivacy &&
        accessToEdit &&
        showEditBadge &&
        !canConvert && (
          <StyledIcon
            iconName={iconEdit}
            className="badge tablet icons-group"
            size="small"
            onClick={onFilesClick}
            hoverColor="#3B72A7"
          />
        )}
      {canConvert && !isTrashFolder && (
        <IconButton
          onClick={setConvertDialogVisible}
          iconName="/static/images/refresh.react.svg"
          className="badge tablet icons-group can-convert"
          size="medium"
          color="#A3A9AE"
          hoverColor="#3B72A7"
        />
      )}
      {version > 1 && !(showNew || isNewWithFav) && (
        <Badge
          className="badge-version tablet icons-group"
          backgroundColor="#A3A9AE"
          borderRadius="11px"
          color="#FFFFFF"
          fontSize="9px"
          fontWeight={800}
          label={contentBadgeVersion}
          maxWidth="50px"
          onClick={onShowVersionHistory}
          padding="0 3px"
          data-id={id}
        />
      )}
      {version > 1 && (showNew || isNewWithFav) && (
        <Badge
          className="badge-version tablet icons-group"
          backgroundColor="#ED7309"
          borderRadius="11px"
          color="#FFFFFF"
          fontSize="9px"
          fontWeight={800}
          label={contentBadgeVersion}
          maxWidth="50px"
          onClick={onSharedVersionClick}
          padding="0 3px"
          data-id={id}
        />
      )}
      {(showNew || isNewWithFav) && !(version > 1) && (
        <Badge
          className="badge-version tablet icons-group"
          backgroundColor="#ED7309"
          borderRadius="11px"
          color="#FFFFFF"
          fontSize="9px"
          fontWeight={800}
          label={t("New")}
          maxWidth="50px"
          onClick={onBadgeClick}
          padding="0 3px"
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
        fontSize="9px"
        fontWeight={800}
        label={contentNewItems}
        maxWidth="50px"
        onClick={onBadgeClick}
        padding="0 3px"
        data-id={id}
      />
    )
  );
};

export default Badges;
