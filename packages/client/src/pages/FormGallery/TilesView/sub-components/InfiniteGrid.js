import React, { useEffect, useState } from "react";
import { inject, observer } from "mobx-react";
import InfiniteLoaderComponent from "@docspace/components/infinite-loader";
import { StyledCard, StyledItem } from "../StyledTileView";
import Loaders from "@docspace/common/components/Loaders";
import uniqueid from "lodash/uniqueId";

const Card = ({ children, countTilesInRow, ...rest }) => {
  const horizontalGap = 16;
  const fileHeight = 220 + horizontalGap;
  const cardHeight = fileHeight;

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

  const addItemToList = (key, clear) => {
    list.push(
      <Item key={key} className="isFile">
        {cards}
      </Item>
    );
    if (clear) cards = [];
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

  React.Children.map(children, (child) => {
    if (child) {
      if (cards.length && cards.length === countTilesInRow) {
        const listKey = uniqueid("list-item_");
        addItemToList(listKey, true);
      }

      const cardKey = uniqueid("card-item_");
      cards.push(
        <Card countTilesInRow={countTilesInRow} key={cardKey}>
          {child}
        </Card>
      );
    }
  });

  if (hasMoreFiles) {
    // If cards elements are full, it will add the full line of loaders
    if (cards.length === countTilesInRow) {
      addItemToList("loaded-row", true);
    }

    // Added line of loaders
    while (countTilesInRow > cards.length && cards.length !== countTilesInRow) {
      const key = `tiles-loader_${countTilesInRow - cards.length}`;
      cards.push(
        <Loaders.Tile
          key={key}
          className={"tiles-loader isFile"}
          isFolder={false}
        />
      );
    }

    addItemToList("loaded-row");
  } else if (cards.length) {
    // Adds loaders until the row is full
    const listKey = uniqueid("list-item_");
    addItemToList(listKey);
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

export default inject(({ auth, filesStore, oformsStore }) => {
  const {
    oformFiles,
    hasMoreForms,
    oformsFilterTotal,
    loadMoreForms,
  } = oformsStore;

  const { getCountTilesInRow } = filesStore;

  const filesLength = oformFiles.length;
  const { isVisible } = auth.infoPanelStore;

  return {
    filesList: oformFiles,
    hasMoreFiles: hasMoreForms,
    filterTotal: oformsFilterTotal,
    fetchMoreFiles: loadMoreForms,
    filesLength,
    getCountTilesInRow,
    isVisible,
  };
})(observer(InfiniteGrid));
