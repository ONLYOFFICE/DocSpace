import React, { useCallback, useEffect, createRef } from "react";
import { InfiniteLoader, WindowScroller } from "react-virtualized";
import { StyledList } from "./StyledInfiniteLoader";

const GridComponent = ({
  hasMoreFiles,
  filesLength,
  itemCount,
  loadMoreItems,
  onScroll,
  countTilesInRow,
  children,
  className,
  scroll,
  selectedFolderId,
}) => {
  const loaderRef = createRef();

  useEffect(() => {
    setTimeout(() => loaderRef?.current?.resetLoadMoreRowsCache(true), 0);
  }, [loaderRef, selectedFolderId]);

  const isItemLoaded = useCallback(
    ({ index }) => {
      return !hasMoreFiles || (index + 1) * countTilesInRow < filesLength;
    },
    [filesLength, hasMoreFiles, countTilesInRow]
  );

  const renderTile = ({ index, style, key }) => {
    return (
      <div className="window-item" style={style} key={key}>
        {children[index]}
      </div>
    );
  };

  const getItemSize = ({ index }) => {
    const itemClassNames = children[index]?.props?.className;
    const isFile = itemClassNames?.includes("isFile");
    const isFolder = itemClassNames?.includes("isFolder");
    const isFolderHeader = itemClassNames?.includes("folder_header");

    const horizontalGap = 16;
    const verticalGap = 14;
    const headerMargin = 15;

    const folderHeight = 64 + verticalGap;
    const fileHeight = 220 + horizontalGap;
    const titleHeight = 20 + headerMargin + (isFolderHeader ? 0 : 11);

    return isFolder ? folderHeight : isFile ? fileHeight : titleHeight;
  };

  return (
    <InfiniteLoader
      isRowLoaded={isItemLoaded}
      rowCount={itemCount}
      loadMoreRows={loadMoreItems}
      ref={loaderRef}
    >
      {({ onRowsRendered, registerChild }) => (
        <WindowScroller scrollElement={scroll}>
          {({ height, isScrolling, onChildScroll, scrollTop }) => {
            if (height === undefined) {
              height = scroll.getBoundingClientRect().height;
            }

            const width =
              document.getElementById("tileContainer")?.getBoundingClientRect()
                .width ?? 0;

            return (
              <StyledList
                autoHeight
                height={height}
                onRowsRendered={onRowsRendered}
                ref={registerChild}
                rowCount={children.length}
                rowHeight={getItemSize}
                rowRenderer={renderTile}
                width={width}
                className={className}
                isScrolling={isScrolling}
                onChildScroll={onChildScroll}
                scrollTop={scrollTop}
                overscanRowCount={3}
                onScroll={onScroll}
              />
            );
          }}
        </WindowScroller>
      )}
    </InfiniteLoader>
  );
};

export default GridComponent;
