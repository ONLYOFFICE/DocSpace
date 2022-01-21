import React from "react";
import styled from "styled-components";
import Badge from "@appserver/components/badge";
import IconButton from "@appserver/components/icon-button";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import { isTablet } from "react-device-detect";

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

  const iconEdit = isForm
    ? "/static/images/access.edit.form.react.svg"
    : "/static/images/file.actions.convert.edit.doc.react.svg";

  const iconForm = "/static/images/access.edit.form.react.svg";

  const iconRefresh = "/static/images/refresh.react.svg";

  const countVersions = versionGroup > 999 ? "999+" : versionGroup;

  const contentNewItems = newItems > 999 ? "999+" : newItems;

  const tabletViewBadge =
    (sectionWidth > 500 && sectionWidth <= 1024) || isTablet;

  const sizeBadge = tabletViewBadge ? "medium" : "small";

  const lineHeightBadge = tabletViewBadge ? "1.46" : "1.34";

  const paddingBadge = tabletViewBadge ? "0 5px" : "0 3px";

  const fontSizeBadge = tabletViewBadge ? "11px" : "9px";

  return fileExst ? (
    <div className="badges additional-badges">
      {canWebEdit &&
        !isEditing &&
        !isEditingWithFav &&
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
      {(isEditing || isEditingWithFav) && (
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
        <Badge
          className="badge-version tablet-badge icons-group"
          backgroundColor="#A3A9AE"
          borderRadius="11px"
          color="#FFFFFF"
          fontSize={fontSizeBadge}
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
          fontSize={fontSizeBadge}
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
        fontSize={fontSizeBadge}
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
