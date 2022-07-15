import React from "react";
import { inject, observer } from "mobx-react";
import InfiniteLoaderComponent from "@appserver/components/infinite-loader";

const InfiniteGrid = (props) => {
  const {
    children,
    hasMoreFiles,
    filterTotal,
    fetchMoreFiles,
    filesLength,
    className,
    getCountTilesInRow,
    ...rest
  } = props;

  const list = [];
  React.Children.map(
    children.props.children,
    (item) => item && list.push(item)
  );

  const countTilesInRow = getCountTilesInRow();

  return (
    <InfiniteLoaderComponent
      viewAs="tile"
      countTilesInRow={countTilesInRow}
      filesLength={filesLength}
      hasMoreFiles={hasMoreFiles}
      itemCount={filterTotal}
      loadMoreItems={fetchMoreFiles}
      className={`TileList ${className}`}
      {...rest}
    >
      {list}
    </InfiniteLoaderComponent>
  );
};

export default inject(({ filesStore }) => {
  const {
    filesList,
    hasMoreFiles,
    filterTotal,
    fetchMoreFiles,
    getCountTilesInRow,
  } = filesStore;

  const filesLength = filesList.length;

  return {
    filesList,
    hasMoreFiles,
    filterTotal,
    fetchMoreFiles,
    filesLength,
    getCountTilesInRow,
  };
})(observer(InfiniteGrid));
