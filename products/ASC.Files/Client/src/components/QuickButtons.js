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

  return fileExst ? (
    <div className="badges additional-badges">
      {item.canShare && showShare ? (
        <SharedButton
          t={t}
          id={item.id}
          shared={item.shared}
          isFolder={item.isFolder}
        />
      ) : null}
      {accessToEdit && !isTrashFolder && (
        <IconButton
          iconName={
            locked
              ? "/static/images/file.actions.locked.react.svg"
              : "/static/images/locked.react.svg"
          }
          className="badge lock-file icons-group"
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
    </div>
  ) : null;
};

export default QuickButtons;
