import React from 'react';

import Avatar from '@appserver/components/avatar';
import Checkbox from '@appserver/components/checkbox';

import {
  StyledSelectorItem,
  StyledSelectorItemSeparator,
  StyledSelectorItemUser,
  StyledSelectorItemUserText,
} from './StyledSelector';

const Item = ({
  propId,
  name,
  avatar,
  itemsCount,
  selectedItemsCount,
  isGroup,
  isSelected,
  isSeparator,
  isHeader,
  onClickItem,
  id,
  className,
  style,
}) => {
  const [isIndeterminate, setIsIndeterminate] = React.useState(false);

  React.useEffect(() => {
    setIsIndeterminate(selectedItemsCount > 0 && selectedItemsCount !== itemsCount);
  }, [itemsCount, selectedItemsCount]);

  const onClickItemAction = React.useCallback(() => {
    onClickItem && onClickItem(isGroup, propId, isHeader);
  }, [onClickItem, isGroup, propId, isHeader]);

  return !isSeparator ? (
    <StyledSelectorItem
      isHeader={isHeader}
      onClick={onClickItemAction}
      id={id}
      className={className}
      style={style}>
      <StyledSelectorItemUser>
        <Avatar
          className="selector-item__avatar"
          role="user"
          size="min"
          source={avatar}
          userName={name}
        />
        <StyledSelectorItemUserText
          isBold={true}
          truncate={true}
          noSelect={true}
          isHeader={isHeader}
          isGroup={isGroup}>
          {name} {isGroup && `(${selectedItemsCount}/${itemsCount})`}
        </StyledSelectorItemUserText>
      </StyledSelectorItemUser>

      <Checkbox
        className="selector-item__checkbox"
        isIndeterminate={isIndeterminate}
        isChecked={isSelected}
      />
    </StyledSelectorItem>
  ) : (
    <StyledSelectorItemSeparator />
  );
};

export default React.memo(Item);
