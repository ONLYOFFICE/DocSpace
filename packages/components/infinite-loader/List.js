import React, { useCallback, useEffect, createRef } from "react";
import { InfiniteLoader, WindowScroller } from "react-virtualized";
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
  columnInfoPanelStorageName,
  children,
  className,
  scroll,
  infoPanelVisible,
}) => {
  const loaderRef = createRef();

  const renderRow = ({ key, index, style }) => {
    const isLoaded = isItemLoaded({ index });
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
    const storageSize = infoPanelVisible
      ? localStorage.getItem(columnInfoPanelStorageName)
      : localStorage.getItem(columnStorageName);

    const isLoaded = isItemLoaded({ index });
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
      ref={loaderRef}
    >
      {({ onRowsRendered, registerChild }) => (
        <WindowScroller scrollElement={scroll}>
          {({ height, isScrolling, onChildScroll, scrollTop }) => {
            if (height === undefined) {
              height = scroll.getBoundingClientRect().height;
            }

            const viewId =
              viewAs === "table" ? "table-container" : "rowContainer";

            const width =
              document.getElementById(viewId)?.getBoundingClientRect().width ??
              0;

            return (
              <StyledList
                autoHeight
                height={height}
                onRowsRendered={onRowsRendered}
                ref={registerChild}
                rowCount={hasMoreFiles ? children.length + 2 : children.length}
                rowHeight={itemSize}
                rowRenderer={viewAs === "table" ? renderTable : renderRow}
                width={width}
                className={className}
                isScrolling={isScrolling}
                onChildScroll={onChildScroll}
                scrollTop={scrollTop}
                overscanRowCount={3}
                onScroll={onScroll}
                viewAs={viewAs}
              />
            );
          }}
        </WindowScroller>
      )}
    </InfiniteLoader>
  );
};

export default ListComponent;
