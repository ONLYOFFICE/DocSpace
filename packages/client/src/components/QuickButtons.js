import FileActionsLockedReactSvgUrl from "PUBLIC_DIR/images/file.actions.locked.react.svg?url";
import LockedReactSvgUrl from "PUBLIC_DIR/images/locked.react.svg?url";
import FileActionsFavoriteReactSvgUrl from "PUBLIC_DIR/images/file.actions.favorite.react.svg?url";
import FavoriteReactSvgUrl from "PUBLIC_DIR/images/favorite.react.svg?url";
import React from "react";
import styled from "styled-components";
import IconButton from "@docspace/components/icon-button";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import { isMobile, isTablet } from "react-device-detect";
import { FileStatus } from "@docspace/common/constants";

import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";

const QuickButtons = (props) => {
  const {
    t,
    item,
    theme,
    sectionWidth,
    onClickLock,
    isDisabled,
    onClickFavorite,
    viewAs,
    folderCategory,
  } = props;

  const { id, locked, fileStatus, title, fileExst } = item;

  const isFavorite =
    (fileStatus & FileStatus.IsFavorite) === FileStatus.IsFavorite;

  const isTile = viewAs === "tile";

  const iconLock = locked ? FileActionsLockedReactSvgUrl : LockedReactSvgUrl;

  const colorLock = locked
    ? theme.filesQuickButtons.sharedColor
    : theme.filesQuickButtons.color;

  const iconFavorite = isFavorite
    ? FileActionsFavoriteReactSvgUrl
    : FavoriteReactSvgUrl;

  const colorFavorite = isFavorite
    ? theme.filesQuickButtons.sharedColor
    : theme.filesQuickButtons.color;

  const tabletViewQuickButton =
    (sectionWidth > 500 && sectionWidth <= 1024) || isTablet;

  const sizeQuickButton = isTile || tabletViewQuickButton ? "medium" : "small";

  const displayBadges = viewAs === "table" || isTile || tabletViewQuickButton;

  const setFavorite = () => onClickFavorite(isFavorite);

  const isAvailableLockFile = !folderCategory && fileExst && displayBadges;

  return (
    <div className="badges additional-badges">
      {isAvailableLockFile && (
        <ColorTheme
          themeId={ThemeType.IconButton}
          iconName={iconLock}
          className="badge lock-file icons-group"
          size={sizeQuickButton}
          data-id={id}
          data-locked={locked ? true : false}
          onClick={onClickLock}
          color={colorLock}
          isDisabled={isDisabled}
          hoverColor={theme.filesQuickButtons.sharedColor}
          title={t("UnblockVersion")}
        />
      )}
      {/* {fileExst && !isTrashFolder && displayBadges && (
        <ColorTheme
          themeId={ThemeType.IconButton}
          iconName={iconFavorite}
          isFavorite={isFavorite}
          className="favorite badge icons-group"
          size={sizeQuickButton}
          data-id={id}
          data-title={title}
          color={colorFavorite}
          onClick={setFavorite}
          hoverColor={theme.filesQuickButtons.hoverColor}
        />
      )} */}
    </div>
  );
};

export default QuickButtons;
