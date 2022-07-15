import React, { memo, useCallback } from "react";
import AutoSizer from "react-virtualized-auto-sizer";
import InfiniteLoader from "react-window-infinite-loader";
import { VariableSizeList as List, areEqual } from "react-window";
import CustomScrollbarsVirtualList from "../scrollbar/custom-scrollbars-virtual-list";
import Loaders from "@appserver/common/components/Loaders";
import { StyledCard, StyledItem } from "./StyledInfiniteLoader";

const GridComponent = ({
  hasMoreFiles,
  filesLength,
  itemCount,
  loadMoreItems,
  onScroll,
  countTilesInRow,
  children,
  className,
}) => {
  const isItemLoaded = useCallback(
    (index) => {
      return !hasMoreFiles || index * countTilesInRow < filesLength;
    },
    [filesLength, hasMoreFiles, countTilesInRow]
  );

  const renderTile = memo(({ data, index, style }) => {
    const { itemCount } = data;

    // This is the range of cards visible on this row, given the current width:
    const startIndex = index * countTilesInRow;
    const stopIndex = Math.min(itemCount - 1, startIndex + countTilesInRow - 1);
    let countLoaders = (stopIndex + 1) % countTilesInRow;

    const cards = [];
    // Header(Files)
    for (let i = startIndex; i <= stopIndex; i++) {
      if (children[i].props.className === "tile-items-heading") {
        cards.push(
          <div key={i} style={{ height: 20, gridColumn: "-1 / 1" }}>
            {children[i]}
          </div>
        );

        break;
      }

      //Cards
      cards.push(
        <StyledCard
          key={i}
          className="Card"
          style={{ height: getItemSize(index) }}
        >
          {children[i]}
        </StyledCard>
      );
    }

    //Loaders
    if (hasMoreFiles && cards.length) {
      while (countLoaders !== 0) {
        cards.push(
          <Loaders.Tile
            key={`tiles-loader_${countLoaders}`}
            className="tiles-loader"
            isFolder
          />
        );
        countLoaders--;
      }
    }

    return (
      <StyledItem className="Item" style={style}>
        {cards}
      </StyledItem>
    );
  }, areEqual);

  const getItemSize = (index) => {
    const newIndex = index * countTilesInRow;

    if (!children[newIndex]) return 0;

    const itemProps = children[newIndex].props;
    const isFile = itemProps?.className?.includes("file");
    const isFolder = itemProps?.className?.includes("folder");

    const horizontalGap = 16;
    const verticalGap = 14;

    const folderHeight = 64 + verticalGap;
    const fileHeight = 220 + horizontalGap;
    const titleHeight = 20 + horizontalGap;

    return isFolder ? folderHeight : isFile ? fileHeight : titleHeight;
  };

  const renderGrid = ({ height, width }) => {
    const itemsCount = children.length;

    const rowCount = Math.ceil(itemsCount / countTilesInRow) + 1;

    return (
      <InfiniteLoader
        isItemLoaded={isItemLoaded}
        itemCount={itemCount}
        loadMoreItems={loadMoreItems}
      >
        {({ onItemsRendered, ref }) => (
          <List
            onScroll={onScroll}
            className={className}
            height={height}
            itemCount={rowCount}
            itemSize={getItemSize}
            width={width}
            itemData={{ itemCount: itemsCount }}
            onItemsRendered={onItemsRendered}
            ref={ref}
            outerElementType={CustomScrollbarsVirtualList}
            overscanCount={3} //TODO:
          >
            {renderTile}
          </List>
        )}
      </InfiniteLoader>
    );
  };

  return <AutoSizer>{renderGrid}</AutoSizer>;
};

export default GridComponent;
