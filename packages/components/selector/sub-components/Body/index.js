import React from "react";
import InfiniteLoader from "react-window-infinite-loader";
import { FixedSizeList as List } from "react-window";

import CustomScrollbarsVirtualList from "../../../scrollbar/custom-scrollbars-virtual-list";

import Search from "../Search";
import SelectAll from "../SelectAll";
import Item from "../Item";
import EmptyScreen from "../EmptyScreen";

import { StyledSelectorBody } from "../../StyledSelector";

const CONTAINER_PADDING = 32;
const HEADER_HEIGHT = 54;
const SEARCH_HEIGHT = 44;
const SELECT_ALL_HEIGHT = 73;
const FOOTER_HEIGHT = 73;

const Body = ({
  footerVisible,
  isSearch,
  isAllIndeterminate,
  isAllChecked,
  placeholder,
  value,
  onSearch,
  onClearSearch,
  items,
  onSelect,
  isMultiSelect,
  withSelectAll,
  selectAllLabel,
  selectAllIcon,
  onSelectAll,
  emptyScreenImage,
  emptyScreenHeader,
  emptyScreenDescription,
  searchEmptyScreenImage,
  searchEmptyScreenHeader,
  searchEmptyScreenDescription,
  loadMoreItems,
  hasNextPage,
  totalItems,
  isLoading,
  searchLoader,
  rowLoader,
}) => {
  const [bodyHeight, setBodyHeight] = React.useState(null);

  const bodyRef = React.useRef(null);
  const listOptionsRef = React.useRef(null);

  const itemsCount = hasNextPage ? items.length + 1 : items.length;
  const withSearch = isSearch || itemsCount > 0;

  const resetCache = React.useCallback(() => {
    if (listOptionsRef && listOptionsRef.current) {
      listOptionsRef.current.resetloadMoreItemsCache(true);
    }
  }, [listOptionsRef.current]);

  // const onBodyRef = React.useCallback(
  //   (node) => {
  //     if (node) {
  //       node.addEventListener("resize", onBodyResize);
  //       bodyRef.current = node;
  //       setBodyHeight(node.offsetHeight);
  //     }
  //   },
  //   [onBodyResize]
  // );

  const onBodyResize = React.useCallback(() => {
    setBodyHeight(bodyRef.current.offsetHeight);
  }, [bodyRef?.current?.offsetHeight]);

  const isItemLoaded = React.useCallback(
    (index) => {
      return !hasNextPage || index < itemsCount;
    },
    [hasNextPage, itemsCount]
  );

  React.useEffect(() => {
    window.addEventListener("resize", onBodyResize);
    return () => {
      window.removeEventListener("resize", onBodyResize);
    };
  }, []);

  React.useEffect(() => {
    onBodyResize();
  }, [isLoading, footerVisible]);

  React.useEffect(() => {
    resetCache();
  }, [resetCache, hasNextPage]);

  let listHeight = bodyHeight - CONTAINER_PADDING;

  if (withSearch) listHeight -= SEARCH_HEIGHT;

  if (isMultiSelect && withSelectAll && !isSearch)
    listHeight -= SELECT_ALL_HEIGHT;

  console.log(isSearch);

  return (
    <StyledSelectorBody
      ref={bodyRef}
      footerHeight={FOOTER_HEIGHT}
      headerHeight={HEADER_HEIGHT}
      footerVisible={footerVisible}
    >
      {isLoading && !isSearch ? (
        searchLoader
      ) : withSearch ? (
        <Search
          placeholder={placeholder}
          value={value}
          onSearch={onSearch}
          onClearSearch={onClearSearch}
        />
      ) : null}

      {isLoading ? (
        rowLoader
      ) : itemsCount === 0 ? (
        <EmptyScreen
          withSearch={isSearch && value}
          image={emptyScreenImage}
          header={emptyScreenHeader}
          description={emptyScreenDescription}
          searchImage={searchEmptyScreenImage}
          searchHeader={searchEmptyScreenHeader}
          searchDescription={searchEmptyScreenDescription}
        />
      ) : (
        <>
          {isMultiSelect && withSelectAll && !isSearch && (
            <SelectAll
              label={selectAllLabel}
              icon={selectAllIcon}
              isChecked={isAllChecked}
              isIndeterminate={isAllIndeterminate}
              onSelectAll={onSelectAll}
              isLoading={isLoading}
              rowLoader={rowLoader}
            />
          )}

          {bodyHeight && (
            <InfiniteLoader
              ref={listOptionsRef}
              isItemLoaded={isItemLoaded}
              itemCount={totalItems}
              loadMoreItems={loadMoreItems}
            >
              {({ onItemsRendered, ref }) => (
                <List
                  className="items-list"
                  height={listHeight}
                  width={"100%"}
                  itemCount={itemsCount}
                  itemData={{
                    items,
                    onSelect,
                    isMultiSelect,
                    rowLoader,
                    isItemLoaded,
                  }}
                  itemSize={48}
                  onItemsRendered={onItemsRendered}
                  ref={ref}
                  outerElementType={CustomScrollbarsVirtualList}
                >
                  {Item}
                </List>
              )}
            </InfiniteLoader>
          )}
        </>
      )}
    </StyledSelectorBody>
  );
};

export default Body;
