import React from "react";
import { connect } from "react-redux";
import { CustomScrollbarsVirtualList } from "asc-web-components";
import AutoSizer from "react-virtualized-auto-sizer";
import { FixedSizeList as List } from "react-window";
import InfiniteLoader from "react-window-infinite-loader";
import RowWrapper from "./RowWrapper";

import memoize from "memoize-one";

import { loadMoreUsers } from "../../../../../store/people/actions";
import { getUsers } from "../../../../../store/people/selectors";

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
  filter,
  loadMoreUsers,
}) => {
  //console.log("PeopleList render");

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

  const isItemLoaded = (index) => !!peopleList[index];

  const loadMoreItems = () => {
    loadMoreUsers(filter);
  };

  return (
    <AutoSizer>
      {({ height, width, style }) => (
        <InfiniteLoader
          isItemLoaded={isItemLoaded}
          itemCount={filter.total}
          loadMoreItems={loadMoreItems}
        >
          {({ onItemsRendered, ref }) => (
            <List
              style={style}
              height={height}
              width={width}
              itemData={itemData}
              itemCount={peopleList.length}
              itemSize={itemData.isMobile ? 57 : 48}
              outerElementType={CustomScrollbarsVirtualList}
              onItemsRendered={onItemsRendered}
              ref={ref}
            >
              {RowWrapper}
            </List>
          )}
        </InfiniteLoader>
      )}
    </AutoSizer>
  );
};

const mapStateToProps = (state) => {
  const { filter } = state.people;

  return {
    filter,
    getUsers: getUsers(state),
  };
};

export default connect(mapStateToProps, {
  loadMoreUsers,
})(PeopleList);
