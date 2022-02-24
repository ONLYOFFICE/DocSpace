import React, { useState } from "react";
import styled from "styled-components";
import Badge from "@appserver/components/badge";
import IconButton from "@appserver/components/icon-button";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import { isTablet } from "react-device-detect";
import { FileStatus } from "@appserver/common/constants";

export const StyledIcon = styled(IconButton)`
  ${commonIconsStyles}
`;

const StyledWrapper = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  background: white;
  padding: 6px;
  border-radius: 4px;
  box-shadow: 0px 2px 4px rgba(4, 15, 27, 0.16);
`;

const BadgeWrapper = ({ onClick, isTile, children: badge }) => {
  if (!isTile) return badge;

  const [isHovered, setIsHovered] = useState(false);

  const onMouseEnter = () => {
    setIsHovered(true);
  };

  const onMouseLeave = () => {
    setIsHovered(false);
  };

  const newBadge = React.cloneElement(badge, { isHovered: isHovered });

  return (
    <StyledWrapper
      onClick={onClick}
      onMouseEnter={onMouseEnter}
      onMouseLeave={onMouseLeave}
    >
      {newBadge}
    </StyledWrapper>
  );
};

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
  viewAs,
}) => {
  const { id, locked, version, versionGroup, fileExst, isEditing } = item;

  const showEditBadge = !locked || item.access === 0;
  const isPrivacy = isPrivacyFolder && isDesktopClient;
  const isForm = fileExst === ".oform";
  const isTile = viewAs === "tile";

  const iconEdit = isForm
    ? "/static/images/access.edit.form.react.svg"
    : "/static/images/file.actions.convert.edit.doc.react.svg";

  const iconForm = "/static/images/access.edit.form.react.svg";

  const iconRefresh = "/static/images/refresh.react.svg";

  const countVersions = versionGroup > 999 ? "999+" : versionGroup;

  const contentNewItems = newItems > 999 ? "999+" : newItems;

  const tabletViewBadge =
    !isTile && ((sectionWidth > 500 && sectionWidth <= 1024) || isTablet);

  const sizeBadge = isTile || tabletViewBadge ? "medium" : "small";

  const lineHeightBadge = isTile || tabletViewBadge ? "1.46" : "1.34";

  const paddingBadge = isTile || tabletViewBadge ? "0 5px" : "0 3px";

  const fontSizeBadge = isTile || tabletViewBadge ? "11px" : "9px";

  const commonBadgeProps = {
    borderRadius: "11px",
    color: "#FFFFFF",
    fontSize: fontSizeBadge,
    fontWeight: 800,
    maxWidth: "50px",
    padding: paddingBadge,
    lineHeight: lineHeightBadge,
    "data-id": id,
  };

  return fileExst ? (
    <div className="badges additional-badges">
      {canWebEdit &&
        !isEditing &&
        !isTrashFolder &&
        !isPrivacy &&
        accessToEdit &&
        showEditBadge &&
        !canConvert &&
        isForm && (
          <StyledIcon
            iconName={iconForm}
            className="badge tablet-badge icons-group tablet-edit edit"
            size={sizeBadge}
            onClick={onFilesClick}
            hoverColor="#3B72A7"
            title={t("Common:FillFormButton")}
          />
        )}
      {isEditing && (
        <StyledIcon
          iconName={iconEdit}
          className="badge icons-group is-editing tablet-badge tablet-edit"
          size={sizeBadge}
          onClick={onFilesClick}
          hoverColor="#3B72A7"
          title={t("Common:EditButton")}
        />
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
        <BadgeWrapper onClick={onShowVersionHistory} isTile={isTile}>
          <Badge
            {...commonBadgeProps}
            className="badge-version badge-version-current tablet-badge icons-group"
            backgroundColor="#A3A9AE"
            label={t("VersionBadge:Version", { version: countVersions })}
            onClick={onShowVersionHistory}
          />
        </BadgeWrapper>
      )}
      {showNew && (
        <BadgeWrapper onClick={onBadgeClick} isTile={isTile}>
          <Badge
            {...commonBadgeProps}
            className="badge-version badge-new-version tablet-badge icons-group"
            backgroundColor="#ED7309"
            label={t("New")}
            onClick={onBadgeClick}
          />
        </BadgeWrapper>
      )}
    </div>
  ) : (
    showNew && (
      <Badge
        {...commonBadgeProps}
        className="new-items tablet-badge"
        backgroundColor="#ED7309"
        label={contentNewItems}
        onClick={onBadgeClick}
      />
    )
  );
};

export default Badges;
