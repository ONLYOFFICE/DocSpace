import React, { useEffect, useState } from "react";
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

const Card = ({ children, countTilesInRow, ...rest }) => {
  const getItemSize = (child) => {
    const isFile = child?.props?.className?.includes("file");
    const isFolder = child?.props?.className?.includes("folder");
    const isRoom = child?.props?.className?.includes("room");

    const horizontalGap = 16;
    const verticalGap = 14;
    const headerMargin = 15;

    const folderHeight = 64 + verticalGap;
    const roomHeight = 122 + verticalGap;
    const fileHeight = 220 + horizontalGap;
    const titleHeight = 20 + headerMargin;

    if (isRoom) return roomHeight;
    if (isFolder) return folderHeight;
    if (isFile) return fileHeight;
    return titleHeight;
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
    ...rest
  } = props;

  const [countTilesInRow, setCountTilesInRow] = useState(getCountTilesInRow());

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

  const checkType = (useTempList = true) => {
    const card = cards[cards.length - 1];
    const listItem = list[list.length - 1];

    const isFile = useTempList
      ? card.props.children.props.className.includes("file")
      : listItem.props.className.includes("isFile");

    if (isFile) return "isFile";

    const isFolder = useTempList
      ? card.props.children.props.className.includes("folder")
      : listItem.props.className.includes("isFolder");

    if (isFolder) return "isFolder";

    return "isRoom";
  };

  const setTilesCount = () => {
    const newCount = getCountTilesInRow();
    if (countTilesInRow !== newCount) setCountTilesInRow(newCount);
  };

  const onResize = () => {
    setTilesCount();
  };

  useEffect(() => {
    setTilesCount();
    window.addEventListener("resize", onResize);

    return () => {
      window.removeEventListener("resize", onResize);
    };
  });

  React.Children.map(children.props.children, (child) => {
    if (child) {
      if (child.props.className === "tile-items-heading") {
        // If cards is not empty then put the cards into the list
        if (cards.length) {
          const type = checkType();

          addItemToList(`last-item-of_${type}`, type, true);
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
        const isRoom = child?.props?.className?.includes("room");
        const className = isFile ? "isFile" : isRoom ? "isRoom" : "isFolder";

        if (cards.length && cards.length === countTilesInRow) {
          const listKey = uniqueid("list-item_");
          addItemToList(listKey, className, true);
        }

        const cardKey = uniqueid("card-item_");
        cards.push(
          <Card countTilesInRow={countTilesInRow} key={cardKey}>
            {child}
          </Card>
        );
      }
    }
  });

  const type = checkType(!!cards.length);

  if (hasMoreFiles) {
    // If cards elements are full, it will add the full line of loaders
    if (cards.length === countTilesInRow) {
      addItemToList("loaded-row", type, true);
    }

    // Added line of loaders
    while (countTilesInRow > cards.length && cards.length !== countTilesInRow) {
      const key = `tiles-loader_${countTilesInRow - cards.length}`;
      cards.push(
        <Loaders.Tile
          key={key}
          className={`tiles-loader ${type}`}
          isFolder={type === "isFolder"}
        />
      );
    }

    addItemToList("loaded-row", type);
  } else if (cards.length) {
    // Adds loaders until the row is full
    const listKey = uniqueid("list-item_");
    addItemToList(listKey, type);
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
      {...rest}
    >
      {list}
    </InfiniteLoaderComponent>
  );
};

export default inject(
  ({ filesStore, treeFoldersStore, clientLoadingStore }) => {
    const {
      filesList,
      hasMoreFiles,
      filterTotal,
      fetchMoreFiles,
      getCountTilesInRow,
      roomsFilterTotal,
    } = filesStore;

    const { showBodyLoader } = clientLoadingStore;

    const { isRoomsFolder, isArchiveFolder } = treeFoldersStore;

    const filesLength = filesList.length;
    const isRooms = isRoomsFolder || isArchiveFolder;

    return {
      filesList,
      hasMoreFiles,
      filterTotal: isRooms ? roomsFilterTotal : filterTotal,
      fetchMoreFiles,
      filesLength,
      getCountTilesInRow,
      isLoading: showBodyLoader,
    };
  }
)(observer(InfiniteGrid));
