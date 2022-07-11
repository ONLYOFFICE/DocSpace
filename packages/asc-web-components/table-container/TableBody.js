import React, { useCallback, memo } from "react";
import AutoSizer from "react-virtualized-auto-sizer";
import { FixedSizeList as List, areEqual } from "react-window";
import InfiniteLoader from "react-window-infinite-loader";
import Loaders from "@appserver/common/components/Loaders";

import CustomScrollbarsVirtualList from "../scrollbar/custom-scrollbars-virtual-list";
import { StyledTableBody, StyledLoader } from "./StyledTableContainer";

const TableBody = (props) => {
  const {
    columnStorageName,
    fetchMoreFiles,
    children,
    filesListLength,
    hasMoreFiles,
    itemCount,
    useReactWindow,
  } = props;

  const renderTable = memo(({ data, index, style }) => {
    const storageSize = localStorage.getItem(columnStorageName);

    const isLoaded = isItemLoaded(index + 1);

    if (!isLoaded) {
      return (
        <StyledLoader style={style} className="table-container_body-loader">
          <Loaders.TableLoader count={2} />
        </StyledLoader>
      );
    }

    return (
      <div
        className="renderTable"
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

  const isItemLoaded = useCallback(
    (index) => !hasMoreFiles || index < filesListLength,
    [filesListLength, hasMoreFiles]
  );

  const renderList = ({ height, width }) => (
    <InfiniteLoader
      isItemLoaded={isItemLoaded}
      itemCount={itemCount}
      loadMoreItems={fetchMoreFiles}
    >
      {({ onItemsRendered, ref }) => (
        <List
          className="TableList"
          height={height}
          width={width}
          itemSize={40}
          itemCount={children.length}
          itemData={children}
          onItemsRendered={onItemsRendered}
          ref={ref}
          outerElementType={CustomScrollbarsVirtualList}
        >
          {renderTable}
        </List>
      )}
    </InfiniteLoader>
  );

  return useReactWindow ? (
    <StyledTableBody
      useReactWindow={useReactWindow}
      className="table-container_body"
    >
      <AutoSizer>{renderList}</AutoSizer>
    </StyledTableBody>
  ) : (
    <StyledTableBody className="table-container_body" {...props} />
  );
};

export default TableBody;
