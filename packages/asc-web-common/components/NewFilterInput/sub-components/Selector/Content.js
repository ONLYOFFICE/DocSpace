import React from 'react';
import { FixedSizeList } from 'react-window';

import CustomScrollbarsVirtualList from '@appserver/components/scrollbar/custom-scrollbars-virtual-list';

import Item from './Item';

import { StyledSelectorContent } from './StyledSelector';

const Row = React.memo(({ data, index, style }) => {
  const isSelected = data[0][index].isGroup
    ? data[0][index].itemsCount === data[0][index].selectedItemsCount
    : data[0][index].isSelected;

  return (
    <Item
      style={style}
      key={data[0][index].id}
      propId={data[0][index].id}
      name={data[0][index].name}
      avatar={data[0][index].avatar}
      itemsCount={data[0][index].itemsCount}
      selectedItemsCount={data[0][index].selectedItemsCount}
      isGroup={data[0][index].isGroup}
      isSelected={isSelected}
      isHeader={false}
      isSeparator={false}
      onClickItem={data[1]}
    />
  );
});

const Content = ({ itemsList, listHeader, isLoading, onClickItem }) => {
  const [listHeight, setListHeight] = React.useState(0);

  React.useEffect(() => {
    const height = window.innerHeight - 164;

    if (listHeader) {
      setListHeight(height - 65);
    } else {
      setListHeight(height);
    }
  }, [listHeader]);

  return (
    <StyledSelectorContent>
      {listHeader && (
        <>
          <Item
            key={listHeader.id}
            propId={listHeader.id}
            name={listHeader.name}
            avatar={listHeader.avatar}
            itemsCount={listHeader.itemsCount}
            selectedItemsCount={listHeader.selectedItemsCount}
            isGroup={listHeader.isGroup}
            isSelected={listHeader.itemsCount === listHeader.selectedItemsCount}
            isHeader={true}
            isSeparator={false}
          />
          <Item isSeparator={true} />
        </>
      )}
      {!isLoading && (
        <FixedSizeList
          height={listHeight}
          width={425}
          itemCount={itemsList.length}
          itemSize={48}
          itemData={[itemsList, onClickItem]}
          outerElementType={CustomScrollbarsVirtualList}>
          {Row}
        </FixedSizeList>
      )}
    </StyledSelectorContent>
    // <div></div>
  );
};

export default React.memo(Content);
