import React from "react";
import { CustomScrollbarsVirtualList } from "asc-web-components";
import AutoSizer from "react-virtualized-auto-sizer";
import { FixedSizeList as List } from "react-window";
import RowWrapper from "./RowWrapper";

import memoize from "memoize-one";

const PeopleList = ({
  peopleList,
  widthProp,
  selectGroup,
  isAdmin,
  currentUserId,
  context,
  isMobile,
  history,
  settings,
  getUserContextOptions,
  onContentRowSelect,
  needForUpdate,
}) => {
  console.log("PeopleList render");

  const createItemData = memoize(
    (
      peopleList,
      widthProp,
      selectGroup,
      isAdmin,
      currentUserId,
      context,
      isMobile,
      history,
      settings,
      getUserContextOptions,
      onContentRowSelect,
      needForUpdate
    ) => ({
      peopleList,
      widthProp,
      selectGroup,
      isAdmin,
      currentUserId,
      context,
      isMobile,
      history,
      settings,
      getUserContextOptions,
      onContentRowSelect,
      needForUpdate,
    })
  );

  const itemData = createItemData(
    peopleList,
    widthProp,
    selectGroup,
    isAdmin,
    currentUserId,
    context,
    isMobile,
    history,
    settings,
    getUserContextOptions,
    onContentRowSelect,
    needForUpdate
  );

  return (
    <AutoSizer>
      {({ height, width, style }) => (
        <List
          style={style}
          height={height}
          width={width}
          itemData={itemData}
          itemCount={peopleList.length}
          itemSize={49}
          outerElementType={CustomScrollbarsVirtualList}
        >
          {RowWrapper}
        </List>
      )}
    </AutoSizer>
  );
};

export default PeopleList;
