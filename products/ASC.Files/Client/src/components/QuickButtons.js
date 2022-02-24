import React from "react";
import styled from "styled-components";
import IconButton from "@appserver/components/icon-button";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import { isMobile, isTablet } from "react-device-detect";
import { FileStatus } from "@appserver/common/constants";

export const StyledIcon = styled(IconButton)`
  ${commonIconsStyles}
`;

const QuickButtons = ({
  item,
  sectionWidth,
  isTrashFolder,
  accessToEdit,
  showShare,
  onClickLock,
  onClickFavorite,
  onClickShare,
  viewAs,
}) => {
  const { id, locked, fileStatus, title, fileExst, shared } = item;

  const isFavorite =
    (fileStatus & FileStatus.IsFavorite) === FileStatus.IsFavorite;

  const isTile = viewAs === "tile";

  const iconShare = shared
    ? "/static/images/file.actions.share.react.svg"
    : "/static/images/catalog.share.react.svg";

  const iconLock = locked
    ? "/static/images/file.actions.locked.react.svg"
    : "/static/images/locked.react.svg";

  const iconFavorite = isFavorite
    ? "/static/images/file.actions.favorite.react.svg"
    : "/static/images/favorite.react.svg";

  const tabletViewQuickButton =
    !isTile && ((sectionWidth > 500 && sectionWidth <= 1024) || isTablet);
  const sizeQuickButton = isTile || tabletViewQuickButton ? "medium" : "small";

  const displayShare = viewAs === "row" && (isMobile || sectionWidth <= 500);
  const displayLock = !locked && (isMobile || sectionWidth <= 500);
  const displayFavorite = !isFavorite && (isMobile || sectionWidth <= 500);

  const setFavorite = () => onClickFavorite(isFavorite);

  return (
    <div className="badges additional-badges">
      {item.canShare && showShare && (!displayShare || isTile) && (
        <StyledIcon
          iconName={iconShare}
          className="badge share-button-icon"
          size={sizeQuickButton}
          onClick={onClickShare}
          hoverColor="#3B72A7"
        />
      )}
      {fileExst &&
        accessToEdit &&
        !isTrashFolder &&
        (!displayLock || isTile) && (
          <StyledIcon
            iconName={iconLock}
            className="badge lock-file icons-group"
            size={sizeQuickButton}
            data-id={id}
            data-locked={locked ? true : false}
            onClick={onClickLock}
            hoverColor="#3B72A7"
          />
        )}
      {fileExst && !isTrashFolder && (!displayFavorite || isTile) && (
        <StyledIcon
          iconName={iconFavorite}
          className="favorite badge icons-group"
          size={sizeQuickButton}
          data-id={id}
          data-title={title}
          onClick={setFavorite}
          hoverColor="#3B72A7"
        />
      )}
    </div>
  );
};

export default QuickButtons;
