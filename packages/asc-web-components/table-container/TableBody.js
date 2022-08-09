import React from "react";
import { StyledTableBody } from "./StyledTableContainer";
import InfiniteLoaderComponent from "../infinite-loader";

const TableBody = (props) => {
  const {
    columnStorageName,
    fetchMoreFiles,
    children,
    filesLength,
    hasMoreFiles,
    itemCount,
    itemHeight,
    useReactWindow,
    onScroll,
  } = props;

  return useReactWindow ? (
    <StyledTableBody
      useReactWindow={useReactWindow}
      className="table-container_body"
    >
      <InfiniteLoaderComponent
        className="TableList"
        viewAs="table"
        hasMoreFiles={hasMoreFiles}
        filesLength={filesLength}
        itemCount={itemCount}
        loadMoreItems={fetchMoreFiles}
        columnStorageName={columnStorageName}
        itemSize={itemHeight}
        onScroll={onScroll}
      >
        {children}
      </InfiniteLoaderComponent>
    </StyledTableBody>
  ) : (
    <StyledTableBody className="table-container_body" {...props} />
  );
};

TableBody.defaultProps = {
  itemHeight: 40,
};

export default TableBody;
