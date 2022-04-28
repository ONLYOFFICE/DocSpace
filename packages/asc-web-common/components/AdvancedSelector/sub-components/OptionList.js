import React from "react";
import { FixedSizeList as List } from "react-window";
import InfiniteLoader from "react-window-infinite-loader";
import AutoSizer from "react-virtualized-auto-sizer";
import CustomScrollbarsVirtualList from "@appserver/components/scrollbar/custom-scrollbars-virtual-list";

import Option from "./Option";

const OptionList = ({
  listOptionsRef,
  loadingLabel,
  options,
  isOptionChecked,
  isMultiSelect,
  onOptionChange,
  onLinkClick,
  isItemLoaded,
  itemCount,
  loadMoreItems,
}) => {
  const renderOption = React.useCallback(
    ({ index, style }) => {
      const isLoaded = isItemLoaded(index);

      if (!isLoaded) {
        return <Option isLoader={true} loadingLabel={loadingLabel} />;
      }

      const option = options[index];
      const isChecked = isOptionChecked(option);

      return (
        <Option
          index={index}
          style={style}
          {...option}
          isChecked={isChecked}
          onOptionChange={onOptionChange}
          onLinkClick={onLinkClick}
          isMultiSelect={isMultiSelect}
        />
      );
    },
    [
      options,
      loadingLabel,
      isMultiSelect,

      isItemLoaded,
      isOptionChecked,

      onOptionChange,
      onLinkClick,
    ]
  );

  return (
    <AutoSizer>
      {({ width, height }) => (
        <InfiniteLoader
          ref={listOptionsRef}
          isItemLoaded={isItemLoaded}
          itemCount={itemCount}
          loadMoreItems={loadMoreItems}
        >
          {({ onItemsRendered, ref }) => (
            <List
              className="options-list"
              height={height - 73}
              itemCount={itemCount}
              itemSize={48}
              onItemsRendered={onItemsRendered}
              ref={ref}
              width={width + 8}
              outerElementType={CustomScrollbarsVirtualList}
            >
              {renderOption}
            </List>
          )}
        </InfiniteLoader>
      )}
    </AutoSizer>
  );
};

export default React.memo(OptionList);
