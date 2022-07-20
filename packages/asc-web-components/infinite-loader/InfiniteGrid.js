import React from "react";
import { inject, observer } from "mobx-react";
import InfiniteLoaderComponent from "./InfiniteLoader";
import { StyledCard, StyledItem } from "./StyledInfiniteLoader";
import Loaders from "@appserver/common/components/Loaders";

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

  const countTilesInRow = getCountTilesInRow();

  let temp = [];
  const list = [];

  const getItemSize = (child) => {
    const isFile = child?.props?.className?.includes("file");
    const isFolder = child?.props?.className?.includes("folder");

    const horizontalGap = 16;
    const verticalGap = 14;
    const headerMargin = 15;

    const folderHeight = 64 + verticalGap;
    const fileHeight = 220 + horizontalGap;
    const titleHeight = 20 + headerMargin;

    return isFolder ? folderHeight : isFile ? fileHeight : titleHeight;
  };

  React.Children.map(children.props.children, (child, index) => {
    if (child) {
      //debugger;
      //console.log("child.key", child.key);

      if (child.props.className === "tile-items-heading") {
        //if after folder files title, push item to list
        if (temp.length) {
          const isFolder = temp
            .at(-1)
            .props.children.props.className.includes("folder");
          const otherClassName2 = isFolder ? "Item isFolder" : "Item isFile";

          // console.log("temp", temp);
          // console.log("temp", `item_${temp.at(-1).key}`);

          list.push(
            <StyledItem
              key={`item_${temp.at(-1).key}`}
              className={otherClassName2}
            >
              {temp}
            </StyledItem>
          );
          temp = [];
        }

        list.push(
          <div
            key={`item_${child.key || index}`}
            className="Item header-item"
            style={{ height: 20, gridColumn: "-1 / 1" }}
          >
            {child}
          </div>
        );
      } else {
        const isFile = child?.props?.className?.includes("file");
        const className = isFile ? "Item isFile" : "Item isFolder";

        const item = (
          <StyledCard
            key={`item_${child.key || index}`}
            className="Card"
            style={{ height: getItemSize(child) }}
          >
            {child}
          </StyledCard>
        );

        if (temp.length && temp.length === countTilesInRow) {
          list.push(
            <StyledItem
              key={`item_${child.key || index}`}
              className={className}
            >
              {temp}
            </StyledItem>
          );
          temp = [];
        }
        temp.push(item);
      }
    }
  });

  // TODO: inf-scroll loaders
  // refactoring

  const isFolder = temp.length
    ? temp.at(-1).props.children.props.className.includes("folder")
    : list.at(-1).props.className.includes("isFolder");

  const otherClassName = isFolder ? "isFolder" : "isFile";

  if (hasMoreFiles) {
    if (temp.length === countTilesInRow) {
      list.push(
        <StyledItem key={155} className={otherClassName}>
          {temp}
        </StyledItem>
      );
      temp = [];
    }

    while (temp.length !== countTilesInRow) {
      temp.push(
        <Loaders.Tile
          key={`tiles-loader_${countTilesInRow - temp.length}`}
          className={`tiles-loader ${otherClassName}`}
          isFolder={isFolder}
        />
      );
    }

    list.push(
      <StyledItem key={156} className={otherClassName}>
        {temp}
      </StyledItem>
    );
  } else if (temp.length) {
    const key = temp.at(-1).key;
    list.push(
      <StyledItem key={key} className={otherClassName}>
        {temp}
      </StyledItem>
    );
  }

  //console.log("InfiniteGrid render", list);

  return (
    <InfiniteLoaderComponent
      viewAs="tile"
      countTilesInRow={countTilesInRow}
      filesLength={filesLength}
      hasMoreFiles={hasMoreFiles}
      itemCount={filterTotal / countTilesInRow} //TODO: - count headers
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
