import UnpinReactSvgUrl from "PUBLIC_DIR/images/unpin.react.svg?url";
import FormFillRectSvgUrl from "PUBLIC_DIR/images/form.fill.rect.svg?url";
import AccessEditFormReactSvgUrl from "PUBLIC_DIR/images/access.edit.form.react.svg?url";
import FileActionsConvertEditDocReactSvgUrl from "PUBLIC_DIR/images/file.actions.convert.edit.doc.react.svg?url";
import RefreshReactSvgUrl from "PUBLIC_DIR/images/refresh.react.svg?url";
import React, { useState } from "react";
import styled from "styled-components";
import Badge from "@docspace/components/badge";
import IconButton from "@docspace/components/icon-button";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import { isTablet } from "react-device-detect";
import { FileStatus } from "@docspace/common/constants";
import { Base } from "@docspace/components/themes";

import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

export const StyledIcon = styled(IconButton)`
  ${commonIconsStyles}
`;

const StyledWrapper = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;

  background: ${(props) => props.theme.filesBadges.backgroundColor};
  padding: 6px;
  border-radius: 4px;
  box-shadow: 0px 2px 4px rgba(4, 15, 27, 0.16);
`;

StyledWrapper.defaultProps = { theme: Base };

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
  theme,
  newItems,
  sectionWidth,
  item,
  isTrashFolder,
  isPrivacyFolder,
  isDesktopClient,
  accessToEdit,
  showNew,
  onFilesClick,
  onShowVersionHistory,
  onBadgeClick,
  setConvertDialogVisible,
  viewAs,
  onUnpinClick,
  isMutedBadge,
  isArchiveFolderRoot,
  isVisitor,
}) => {
  const {
    id,
    locked,
    version,
    versionGroup,
    fileExst,
    isEditing,
    isRoom,
    pinned,
  } = item;

  const showEditBadge = !locked || item.access === 0;
  const isPrivacy = isPrivacyFolder && isDesktopClient;
  const isForm = fileExst === ".oform";
  const isTile = viewAs === "tile";

  const countVersions = versionGroup > 999 ? "999+" : versionGroup;

  const contentNewItems = newItems > 999 ? "999+" : newItems;

  const tabletViewBadge =
    !isTile && ((sectionWidth > 500 && sectionWidth <= 1024) || isTablet);

  const sizeBadge = isTile || tabletViewBadge ? "medium" : "small";

  const lineHeightBadge = isTile || tabletViewBadge ? "1.46" : "1.34";

  const paddingBadge = isTile || tabletViewBadge ? "0 3px" : "0 5px";

  const fontSizeBadge = isTile || tabletViewBadge ? "11px" : "9px";

  const iconForm =
    sizeBadge === "medium" ? FormFillRectSvgUrl : AccessEditFormReactSvgUrl;

  const iconEdit = !isForm ? FileActionsConvertEditDocReactSvgUrl : iconForm;

  const iconRefresh = RefreshReactSvgUrl;

  const iconPin = UnpinReactSvgUrl;

  const unpinIconProps = {
    "data-id": id,
    "data-action": "unpin",
  };

  const commonBadgeProps = {
    borderRadius: "11px",
    fontSize: fontSizeBadge,
    fontWeight: 800,
    maxWidth: "50px",
    padding: paddingBadge,
    lineHeight: lineHeightBadge,
    "data-id": id,
    isMutedBadge,
  };

  const versionBadgeProps = {
    borderRadius: "50px",
    color: theme.filesBadges.color,
    fontSize: "9px",
    fontWeight: 800,
    maxWidth: "50px",
    padding: isTile || tabletViewBadge ? "2px 5px" : "0 4px",
    lineHeight: "12px",
    "data-id": id,
  };

  const onShowVersionHistoryProp = item.security?.ReadHistory
    ? { onClick: onShowVersionHistory }
    : {};

  return fileExst ? (
    <div className="badges additional-badges">
      {isEditing && !isVisitor && (
        <ColorTheme
          themeId={ThemeType.IconButton}
          isEditing={isEditing}
          iconName={iconEdit}
          className="badge icons-group is-editing tablet-badge tablet-edit"
          size={sizeBadge}
          onClick={onFilesClick}
          hoverColor={theme.filesBadges.hoverIconColor}
          title={isForm ? t("Common:FillFormButton") : t("Common:EditButton")}
        />
      )}
      {item.viewAccessability?.Convert &&
        item.security?.Convert &&
        !isTrashFolder &&
        !isArchiveFolderRoot && (
          <ColorTheme
            themeId={ThemeType.IconButton}
            onClick={setConvertDialogVisible}
            iconName={iconRefresh}
            className="badge tablet-badge icons-group can-convert"
            size={sizeBadge}
            hoverColor={theme.filesBadges.hoverIconColor}
          />
        )}
      {version > 1 && (
        <BadgeWrapper {...onShowVersionHistoryProp} isTile={isTile}>
          <Badge
            {...versionBadgeProps}
            className="badge-version badge-version-current tablet-badge icons-group"
            backgroundColor={theme.filesBadges.badgeBackgroundColor}
            label={t("VersionBadge", { version: countVersions })}
            {...onShowVersionHistoryProp}
            noHover={true}
            isVersionBadge={true}
          />
        </BadgeWrapper>
      )}
      {showNew && (
        <BadgeWrapper onClick={onBadgeClick} isTile={isTile}>
          <Badge
            {...commonBadgeProps}
            className="badge-version badge-new-version tablet-badge icons-group"
            label={t("New")}
            onClick={onBadgeClick}
          />
        </BadgeWrapper>
      )}
    </div>
  ) : (
    <>
      {isRoom && pinned && (
        <ColorTheme
          themeId={ThemeType.IconButtonPin}
          onClick={onUnpinClick}
          className="badge icons-group is-pinned tablet-badge tablet-pinned"
          iconName={iconPin}
          size={sizeBadge}
          {...unpinIconProps}
        />
      )}
      {showNew && (
        <Badge
          {...commonBadgeProps}
          className="new-items tablet-badge"
          label={contentNewItems}
          onClick={onBadgeClick}
        />
      )}
    </>
  );
};

export default Badges;
