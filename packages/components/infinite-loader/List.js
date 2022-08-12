import React, { useCallback } from "react";
import { InfiniteLoader, WindowScroller, AutoSizer } from "react-virtualized";
import Loaders from "@docspace/common/components/Loaders";
import { StyledList } from "./StyledInfiniteLoader";

const ListComponent = ({
  viewAs,
  hasMoreFiles,
  filesLength,
  itemCount,
  onScroll,
  loadMoreItems,
  itemSize,
  columnStorageName,
  children,
  className,
  scroll,
}) => {
  const renderRow = ({ key, index, style }) => {
    const isLoaded = isItemLoaded({ index: index + 2 });
    if (!isLoaded) return getLoader(style, key);

    return (
      <div className="row-list-item window-item" style={style} key={key}>
        {children[index]}
      </div>
    );
  };

  const isItemLoaded = useCallback(
    ({ index }) => !hasMoreFiles || index < filesLength,
    [filesLength, hasMoreFiles]
  );

  const renderTable = ({ index, style, key }) => {
    const storageSize = localStorage.getItem(columnStorageName);

    const isLoaded = isItemLoaded({ index: index + 2 });
    if (!isLoaded) return getLoader(style, key);

    return (
      <div
        className="table-list-item window-item"
        style={{
          ...style,
          display: "grid",
          gridTemplateColumns: storageSize,
        }}
        key={key}
      >
        {children[index]}
      </div>
    );
  };

  const getLoader = (style, key) => {
    switch (viewAs) {
      case "table":
        return (
          <Loaders.TableLoader
            key={key}
            style={style}
            className="table-container_body-loader"
            count={1}
          />
        );
      case "row":
        return (
          <Loaders.Rows
            key={key}
            style={style}
            className="row-loader"
            count={1}
          />
        );
      default:
        return <></>;
    }
  };

  return (
    <InfiniteLoader
      isRowLoaded={isItemLoaded}
      rowCount={itemCount}
      loadMoreRows={loadMoreItems}
    >
      {({ onRowsRendered, registerChild }) => (
        <WindowScroller scrollElement={scroll}>
          {({ height, isScrolling, onChildScroll, scrollTop }) => {
            if (height === undefined) {
              height = scroll.getBoundingClientRect().height;
            }

            return (
              <AutoSizer>
                {({ width }) => (
                  <StyledList
                    autoHeight
                    height={height}
                    onRowsRendered={onRowsRendered}
                    ref={registerChild}
                    rowCount={children.length}
                    rowHeight={itemSize}
                    rowRenderer={viewAs === "table" ? renderTable : renderRow}
                    width={width}
                    className={className}
                    isScrolling={isScrolling}
                    onChildScroll={onChildScroll}
                    scrollTop={scrollTop}
                    overscanRowCount={3}
                    onScroll={onScroll}
                  />
                )}
              </AutoSizer>
            );
          }}
        </WindowScroller>
      )}
    </InfiniteLoader>
  );
};

export default ListComponent;
