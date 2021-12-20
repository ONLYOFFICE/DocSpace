import React from "react";
import styled from "styled-components";
import IconButton from "@appserver/components/icon-button";
import SharedButton from "@appserver/files/src/components/SharedButton";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";

export const StyledIcon = styled(IconButton)`
  ${commonIconsStyles}
`;

const QuickButtons = ({
  t,
  item,
  sectionWidth,
  isTrashFolder,
  accessToEdit,
  showShare,
  onClickLock,
  onClickFavorite,
}) => {
  const { id, locked, fileStatus, title, fileExst } = item;

  const isFavorite = fileStatus === 32;
  const isNewWithFav = fileStatus === 34;
  const isEditingWithFav = fileStatus === 33;
  const showFavorite = isFavorite || isNewWithFav || isEditingWithFav;

  const iconLock = locked
    ? "/static/images/file.actions.locked.react.svg"
    : "/static/images/locked.react.svg";

  const iconFavorite = showFavorite
    ? "/static/images/file.actions.favorite.react.svg"
    : "/static/images/favorite.react.svg";

  const tabletViewQuickButton = sectionWidth > 500 && sectionWidth <= 1024;
  const sizeQuickButton = tabletViewQuickButton ? "medium" : "small";

  return (
    <div className="badges additional-badges">
      {item.canShare && showShare && (
        <SharedButton
          t={t}
          id={item.id}
          shared={item.shared}
          isFolder={item.isFolder}
          isSmallIcon={!tabletViewQuickButton}
        />
      )}
      {fileExst && accessToEdit && !isTrashFolder && (
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
      {fileExst && !isTrashFolder && (
        <StyledIcon
          iconName={iconFavorite}
          className="favorite badge icons-group"
          size={sizeQuickButton}
          data-id={id}
          data-title={title}
          onClick={() => onClickFavorite(showFavorite)}
          hoverColor="#3B72A7"
        />
      )}
    </div>
  );
};

export default QuickButtons;
