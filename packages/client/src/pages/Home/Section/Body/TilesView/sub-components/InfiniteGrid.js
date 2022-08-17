import React from "react";
import { inject, observer } from "mobx-react";
import InfiniteLoaderComponent from "@docspace/components/infinite-loader";
import { StyledCard, StyledItem, StyledHeaderItem } from "./StyledInfiniteGrid";
import Loaders from "@docspace/common/components/Loaders";
import uniqueid from "lodash/uniqueId";

const HeaderItem = ({ children, className, ...rest }) => {
  return (
    <StyledHeaderItem className={`${className} header-item`} {...rest}>
      {children}
    </StyledHeaderItem>
  );
};

const Card = ({ children, ...rest }) => {
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

  const cardHeight = getItemSize(children);

  return (
    <StyledCard className="Card" cardHeight={cardHeight} {...rest}>
      {children}
    </StyledCard>
  );
};

const Item = ({ children, className, ...rest }) => {
  return (
    <StyledItem className={`Item ${className}`} {...rest}>
      {children}
    </StyledItem>
  );
};

const InfiniteGrid = (props) => {
  const {
    children,
    hasMoreFiles,
    filterTotal,
    fetchMoreFiles,
    filesLength,
    className,
    getCountTilesInRow,
    selectedFolderId,
    ...rest
  } = props;

  const countTilesInRow = getCountTilesInRow();

  let cards = [];
  const list = [];

  const addItemToList = (key, className, clear) => {
    list.push(
      <Item key={key} className={className}>
        {cards}
      </Item>
    );
    if (clear) cards = [];
  };

  const checkIsFolder = (useTempList = true) => {
    const isFolder = useTempList
      ? cards.at(-1).props.children.props.className.includes("folder")
      : list.at(-1).props.className.includes("isFolder");
    return isFolder;
  };

  React.Children.map(children.props.children, (child) => {
    if (child) {
      if (child.props.className === "tile-items-heading") {
        // If cards is not empty then put the cards into the list
        if (cards.length) {
          const isFolder = checkIsFolder();

          addItemToList(
            `last-item-of_${isFolder ? "folders" : "files"}`,
            isFolder ? "isFolder" : "isFile",
            true
          );
        }

        list.push(
          <HeaderItem
            className={list.length ? "files_header" : "folder_header"}
            key="header_item"
          >
            {child}
          </HeaderItem>
        );
      } else {
        const isFile = child?.props?.className?.includes("file");
        const className = isFile ? "isFile" : "isFolder";

        if (cards.length && cards.length === countTilesInRow) {
          const listKey = uniqueid("list-item_");
          addItemToList(listKey, className, true);
        }

        const cardKey = uniqueid("card-item_");
        cards.push(<Card key={cardKey}>{child}</Card>);
      }
    }
  });

  const isFolder = checkIsFolder(!!cards.length);
  const otherClassName = isFolder ? "isFolder" : "isFile";

  if (hasMoreFiles) {
    // If cards elements are full, it will add the full line of loaders
    if (cards.length === countTilesInRow) {
      addItemToList("loaded-row", otherClassName, true);
    }

    // Added line of loaders
    while (cards.length !== countTilesInRow) {
      const key = `tiles-loader_${countTilesInRow - cards.length}`;
      cards.push(
        <Loaders.Tile
          key={key}
          className={`tiles-loader ${otherClassName}`}
          isFolder={isFolder}
        />
      );
    }

    addItemToList("loaded-row", otherClassName);
  } else if (cards.length) {
    // Adds loaders until the row is full
    const listKey = uniqueid("list-item_");
    addItemToList(listKey, otherClassName);
  }

  // console.log("InfiniteGrid render", list);

  return (
    <InfiniteLoaderComponent
      viewAs="tile"
      countTilesInRow={countTilesInRow}
      filesLength={filesLength}
      hasMoreFiles={hasMoreFiles}
      itemCount={hasMoreFiles ? list.length + 1 : list.length}
      loadMoreItems={fetchMoreFiles}
      className={`TileList ${className}`}
      selectedFolderId={selectedFolderId}
      {...rest}
    >
      {list}
    </InfiniteLoaderComponent>
  );
};

export default inject(({ filesStore, selectedFolderStore }) => {
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
    selectedFolderId: selectedFolderStore.id,
  };
})(observer(InfiniteGrid));
