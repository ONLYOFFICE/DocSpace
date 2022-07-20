import React, { memo, useCallback } from "react";
import AutoSizer from "react-virtualized-auto-sizer";
import InfiniteLoader from "react-window-infinite-loader";
import { FixedSizeList as List, areEqual } from "react-window";
import Scroll from "./Scroll";
import Loaders from "@appserver/common/components/Loaders";
import { StyledTableLoader, StyledRowLoader } from "./StyledInfiniteLoader";

const ListComponent = ({
  viewAs,
  hasMoreFiles,
  filesLength,
  itemCount,
  loadMoreItems,
  itemSize,
  onScroll,
  columnStorageName,
  children,
  className,
}) => {
  const renderRow = memo(({ index, style }) => {
    const isLoaded = isItemLoaded(index + 1);
    if (!isLoaded) return getLoader(style);

    return <div style={style}>{children[index]}</div>;
  }, areEqual);

  const isItemLoaded = useCallback(
    (index) => !hasMoreFiles || index < filesLength,
    [filesLength, hasMoreFiles]
  );

  const renderTable = memo(({ index, style }) => {
    const storageSize = localStorage.getItem(columnStorageName);

    const isLoaded = isItemLoaded(index + 1);
    if (!isLoaded) return getLoader(style);

    return (
      <div
        className="table-row-list-item"
        style={{
          ...style,
          display: "grid",
          gridTemplateColumns: storageSize,
        }}
      >
        {children[index]}
      </div>
    );
  }, areEqual);

  const getLoader = (style) => {
    switch (viewAs) {
      case "table":
        return (
          <StyledTableLoader
            style={style}
            className="table-container_body-loader"
          >
            <Loaders.TableLoader count={2} />
          </StyledTableLoader>
        );
      case "row":
        return (
          <StyledRowLoader style={style} className="row-loader">
            <Loaders.Rows count={2} />
          </StyledRowLoader>
        );
      default:
        return <></>;
    }
  };

  const renderList = ({ height, width }) => (
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
          width={width}
          itemSize={itemSize}
          itemCount={children.length}
          onItemsRendered={onItemsRendered}
          ref={ref}
          outerElementType={Scroll}
        >
          {viewAs === "table" ? renderTable : renderRow}
        </List>
      )}
    </InfiniteLoader>
  );

  return <AutoSizer>{renderList}</AutoSizer>;
};

export default ListComponent;
