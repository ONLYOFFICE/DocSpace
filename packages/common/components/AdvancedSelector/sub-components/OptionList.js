import React from "react";
import { FixedSizeList as List } from "react-window";
import InfiniteLoader from "react-window-infinite-loader";
import AutoSizer from "react-virtualized-auto-sizer";
import CustomScrollbarsVirtualList from "@docspace/components/scrollbar/custom-scrollbars-virtual-list";

import Option from "./Option";

const OptionList = ({
  listOptionsRef,
  options,
  isOptionChecked,
  isMultiSelect,
  onOptionChange,
  onLinkClick,
  isItemLoaded,
  itemCount,
  loadMoreItems,
  isFirstLoad,
}) => {
  const renderOption = React.useCallback(
    ({ index, style }) => {
      const isLoaded = isItemLoaded(index);

      if (!isLoaded) {
        if (!isFirstLoad) {
          return (
            <div style={style}>
              <Option isLoader={true} countLoaderRows={2} />
            </div>
          );
        }

        return <Option isLoader={true} />;
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
