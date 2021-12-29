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
  sectionWidth,
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
  const isForm = fileExst === ".oform";
  const showActionsEdit = isEditing || isEditingWithFav;

  const iconEdit = showActionsEdit
    ? "/static/images/file.actions.convert.edit.doc.react.svg"
    : "/static/images/access.edit.react.svg";

  const iconRefresh = "/static/images/refresh.react.svg";

  const countVersions = versionGroup > 999 ? "999+" : versionGroup;

  const contentNewItems = newItems > 999 ? "999+" : newItems;

  const tabletViewBadge = sectionWidth > 500 && sectionWidth <= 1024;

  const sizeBadge = tabletViewBadge ? "medium" : "small";

  const lineHeightBadge = tabletViewBadge ? "16px" : "12px";

  const paddingBadge = tabletViewBadge ? "0 5px" : "0 3px";

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
            className="badge tablet-edit tablet-badge icons-group"
            size={sizeBadge}
            onClick={onFilesClick}
            iconName={
              isForm
                ? "/static/images/access.edit.form.react.svg"
                : "/static/images/access.edit.react.svg"
            }
            className="badge icons-group edit"
            size="small"
            isfill={true}
            color="#A3A9AE"
            hoverColor="#3B72A7"
          />
        )}
      {(isEditing || isEditingWithFav) &&
        React.createElement(
          isForm
            ? StyledFileActionsEditFormIcon
            : StyledFileActionsConvertEditDocIcon,
          {
            onClick: onFilesClick,
            className: "badge icons-group is-editing",
            size: "small",
          }
        )}
      {canConvert && !isTrashFolder && (
        <StyledIcon
          onClick={setConvertDialogVisible}
          iconName={iconRefresh}
          className="badge tablet-badge icons-group can-convert"
          size={sizeBadge}
          hoverColor="#3B72A7"
        />
      )}
      {version > 1 && (
        <Badge
          className="badge-version tablet-badge icons-group"
          backgroundColor="#A3A9AE"
          borderRadius="11px"
          color="#FFFFFF"
          fontSize="9px"
          fontWeight={800}
          label={t("VersionBadge:Version", { version: countVersions })}
          maxWidth="50px"
          onClick={onShowVersionHistory}
          padding={paddingBadge}
          lineHeight={lineHeightBadge}
          data-id={id}
        />
      )}
      {(showNew || isNewWithFav) && (
        <Badge
          className="badge-version badge-new-version tablet-badge icons-group"
          backgroundColor="#ED7309"
          borderRadius="11px"
          color="#FFFFFF"
          fontSize="9px"
          fontWeight={800}
          label={t("New")}
          maxWidth="50px"
          onClick={onBadgeClick}
          padding={paddingBadge}
          lineHeight={lineHeightBadge}
          data-id={id}
        />
      )}
    </div>
  ) : (
    showNew && (
      <Badge
        className="new-items tablet-badge"
        backgroundColor="#ED7309"
        borderRadius="11px"
        color="#FFFFFF"
        fontSize="9px"
        fontWeight={800}
        label={contentNewItems}
        maxWidth="50px"
        onClick={onBadgeClick}
        padding={paddingBadge}
        lineHeight={lineHeightBadge}
        data-id={id}
      />
    )
  );
};

export default Badges;
