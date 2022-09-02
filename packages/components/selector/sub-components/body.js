import React from "react";
import InfiniteLoader from "react-window-infinite-loader";
import { FixedSizeList as List } from "react-window";

import CustomScrollbarsVirtualList from "../../scrollbar/custom-scrollbars-virtual-list";

import Search from "./search";
import SelectAll from "./select-all";
import Item from "./item";

import { StyledSelectorBody } from "../StyledSelector";
import EmptyScreen from "./empty-screen";

const CONTAINER_PADDING = 32;
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

  loadMoreItems,
  hasNextPage,
  totalItems,
}) => {
  const [bodyHeight, setBodyHeight] = React.useState(null);

  const bodyRef = React.useRef(null);
  const listOptionsRef = React.useRef(null);

  const itemsCount = items.length;
  const withSearch = isSearch || itemsCount > 0;

  const resetCache = React.useCallback(() => {
    if (listOptionsRef && listOptionsRef.current) {
      listOptionsRef.current.resetloadMoreItemsCache(true);
    }
  }, [listOptionsRef.current]);

  const onBodyRef = React.useCallback(
    (node) => {
      if (node) {
        node.addEventListener("resize", onBodyResize);
        bodyRef.current = node;
        setBodyHeight(node.offsetHeight);
      }
    },
    [onBodyResize]
  );

  const onBodyResize = React.useCallback(() => {
    if (bodyRef.current.offsetHeight) {
      setBodyHeight(bodyRef.current.offsetHeight);
    }
  }, [bodyRef?.current?.offsetHeight]);

  const isItemLoaded = React.useCallback(
    (index) => {
      return !hasNextPage || index < itemsCount;
    },
    [hasNextPage, itemsCount]
  );

  React.useEffect(() => {
    return () => {
      bodyRef?.current?.removeEventListener("resize", onBodyResize);
    };
  }, [onBodyResize]);

  React.useEffect(() => {
    resetCache();
  }, [resetCache, hasNextPage]);

  let listHeight = bodyHeight - CONTAINER_PADDING;

  if (withSearch) listHeight -= SEARCH_HEIGHT;
  if (footerVisible) listHeight -= FOOTER_HEIGHT;
  if (isMultiSelect && withSelectAll && !isSearch)
    listHeight -= SELECT_ALL_HEIGHT;

  return (
    <StyledSelectorBody ref={onBodyRef}>
      {withSearch && (
        <Search
          placeholder={placeholder}
          value={value}
          onSearch={onSearch}
          onClearSearch={onClearSearch}
        />
      )}

      {itemsCount === 0 ? (
        <EmptyScreen
          withSearch={withSearch}
          image={emptyScreenImage}
          header={emptyScreenHeader}
          description={emptyScreenDescription}
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
                  itemData={{ items, onSelect, isMultiSelect }}
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
