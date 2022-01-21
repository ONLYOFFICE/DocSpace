import React from 'react';
import styled from 'styled-components';
import IconButton from '@appserver/components/icon-button';
import commonIconsStyles from '@appserver/components/utils/common-icons-style';
import { isMobile, isTablet } from 'react-device-detect';

export const StyledIcon = styled(IconButton)`
  ${commonIconsStyles}
`;

const QuickButtons = ({
  item,
  theme,
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

  const isFavorite = fileStatus === 32;
  const isNewWithFav = fileStatus === 34;
  const isEditingWithFav = fileStatus === 33;
  const showFavorite = isFavorite || isNewWithFav || isEditingWithFav;

  const colorSharedButton = shared
    ? theme.filesQuickButtons.sharedColor
    : theme.filesQuickButtons.color;

  const iconShare = '/static/images/catalog.share.react.svg';

  const iconLock = locked
    ? '/static/images/file.actions.locked.react.svg'
    : '/static/images/locked.react.svg';

  const iconFavorite = showFavorite
    ? '/static/images/file.actions.favorite.react.svg'
    : '/static/images/favorite.react.svg';

  const tabletViewQuickButton = (sectionWidth > 500 && sectionWidth <= 1024) || isTablet;
  const sizeQuickButton = tabletViewQuickButton ? 'medium' : 'small';

  const displayShare = viewAs === 'row' && (isMobile || sectionWidth <= 500);
  const displayLock = !locked && (isMobile || sectionWidth <= 500);
  const displayFavorite = !showFavorite && (isMobile || sectionWidth <= 500);

  return (
    <div className="badges additional-badges">
      {item.canShare && showShare && !displayShare && (
        <StyledIcon
          iconName={iconShare}
          className="badge share-button-icon"
          size={sizeQuickButton}
          onClick={onClickShare}
          color={colorSharedButton}
        />
      )}
      {fileExst && accessToEdit && !isTrashFolder && !displayLock && (
        <StyledIcon
          iconName={iconLock}
          className="badge lock-file icons-group"
          size={sizeQuickButton}
          data-id={id}
          data-locked={locked ? true : false}
          onClick={onClickLock}
          hoverColor={theme.filesQuickButtons.hoverColor}
        />
      )}
      {fileExst && !isTrashFolder && !displayFavorite && (
        <StyledIcon
          iconName={iconFavorite}
          className="favorite badge icons-group"
          size={sizeQuickButton}
          data-id={id}
          data-title={title}
          onClick={() => onClickFavorite(showFavorite)}
          hoverColor={theme.filesQuickButtons.hoverColor}
        />
      )}
    </div>
  );
};

export default QuickButtons;
