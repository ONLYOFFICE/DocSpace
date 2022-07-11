import React, { memo, useCallback } from "react";
import PropTypes from "prop-types";
import AutoSizer from "react-virtualized-auto-sizer";
import InfiniteLoader from "react-window-infinite-loader";
import { FixedSizeList as List, areEqual } from "react-window";
import CustomScrollbarsVirtualList from "../scrollbar/custom-scrollbars-virtual-list";
import Loaders from "@appserver/common/components/Loaders";
import { StyledTableLoader, StyledRowLoader } from "./StyledInfiniteLoader";

const InfiniteLoaderComponent = ({
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
  const renderRow = memo(({ data, index, style }) => {
    const isLoaded = isItemLoaded(index + 1);
    if (!isLoaded) return getLoader(style);

    return <div style={style}>{data[index]}</div>;
  }, areEqual);

  const renderTile = memo(({ data, index, style }) => {
    //TODO:
    return <div style={style}>{data[index]}</div>;
  }, areEqual);

  const isItemLoaded = useCallback(
    (index) => !hasMoreFiles || index < filesLength,
    [filesLength, hasMoreFiles]
  );

  const renderTable = memo(({ data, index, style }) => {
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
        {data[index]}
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
      case "tile":
        return <></>;
        return;
      default:
        return;
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
          itemData={children}
          onItemsRendered={onItemsRendered}
          ref={ref}
          outerElementType={CustomScrollbarsVirtualList}
        >
          {viewAs === "table"
            ? renderTable
            : viewAs === "row"
            ? renderRow
            : renderTile}
        </List>
      )}
    </InfiniteLoader>
  );

  return <AutoSizer>{renderList}</AutoSizer>;
};

InfiniteLoaderComponent.propTypes = {
  viewAs: PropTypes.string.isRequired,
  hasMoreFiles: PropTypes.bool.isRequired,
  filesLength: PropTypes.number.isRequired,
  itemCount: PropTypes.number.isRequired,
  loadMoreItems: PropTypes.func.isRequired,
  itemSize: PropTypes.number.isRequired,
  children: PropTypes.any.isRequired,
  /** Called when the list scroll positions changes */
  onScroll: PropTypes.func,
};

export default InfiniteLoaderComponent;
