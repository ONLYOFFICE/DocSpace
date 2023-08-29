import React from "react";
import InfiniteLoader from "react-window-infinite-loader";
import { FixedSizeList as List } from "react-window";

import CustomScrollbarsVirtualList from "../../../scrollbar/custom-scrollbars-virtual-list";

import Text from "../../../text";

import Search from "../Search";
import SelectAll from "../SelectAll";
import Item from "../Item";
import EmptyScreen from "../EmptyScreen";

import StyledBody from "./StyledBody";
import { BodyProps } from "./Body.types";
import BreadCrumbs from "../BreadCrumbs";

const CONTAINER_PADDING = 16;
const HEADER_HEIGHT = 54;
const BREAD_CRUMBS_HEIGHT = 38;
const SEARCH_HEIGHT = 44;
const BODY_DESCRIPTION_TEXT_HEIGHT = 32;
const SELECT_ALL_HEIGHT = 73;
const FOOTER_HEIGHT = 73;
const FOOTER_WITH_NEW_NAME_HEIGHT = 145;
const FOOTER_WITH_CHECKBOX_HEIGHT = 181;

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
  withBreadCrumbs,
  breadCrumbs,
  onSelectBreadCrumb,
  breadCrumbsLoader,
  withSearch,
  isBreadCrumbsLoading,
  withFooterInput,
  withFooterCheckbox,
  descriptionText,
  withHeader,
}: BodyProps) => {
  const [bodyHeight, setBodyHeight] = React.useState(0);

  const bodyRef = React.useRef<HTMLDivElement>(null);
  const listOptionsRef = React.useRef<any>(null);

  const itemsCount = hasNextPage ? items.length + 1 : items.length;

  const resetCache = React.useCallback(() => {
    if (listOptionsRef && listOptionsRef.current) {
      listOptionsRef.current.resetloadMoreItemsCache(true);
    }
  }, [listOptionsRef.current]);

  const onBodyResize = React.useCallback(() => {
    if (bodyRef && bodyRef.current) {
      setBodyHeight(bodyRef.current.offsetHeight);
    }
  }, [bodyRef?.current?.offsetHeight]);

  const isItemLoaded = React.useCallback(
    (index: number) => {
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

  if (withSearch || isSearch || itemsCount > 0) listHeight -= SEARCH_HEIGHT;

  if (withBreadCrumbs) listHeight -= BREAD_CRUMBS_HEIGHT;

  if (isMultiSelect && withSelectAll && !isSearch)
    listHeight -= SELECT_ALL_HEIGHT;

  if (!!descriptionText) listHeight -= BODY_DESCRIPTION_TEXT_HEIGHT;

  return (
    <StyledBody
      ref={bodyRef}
      footerHeight={
        withFooterCheckbox
          ? FOOTER_WITH_CHECKBOX_HEIGHT
          : withFooterInput
          ? FOOTER_WITH_NEW_NAME_HEIGHT
          : FOOTER_HEIGHT
      }
      headerHeight={HEADER_HEIGHT}
      footerVisible={footerVisible}
      withHeader={withHeader}
    >
      {withBreadCrumbs ? (
        isBreadCrumbsLoading ? (
          breadCrumbsLoader
        ) : (
          <BreadCrumbs
            breadCrumbs={breadCrumbs}
            onSelectBreadCrumb={onSelectBreadCrumb}
            isLoading={isLoading}
          />
        )
      ) : null}

      {isBreadCrumbsLoading ? (
        searchLoader
      ) : withSearch || isSearch || (itemsCount > 0 && withSearch) ? (
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
          withSearch={isSearch && !!value}
          image={emptyScreenImage}
          header={emptyScreenHeader}
          description={emptyScreenDescription}
          searchImage={searchEmptyScreenImage}
          searchHeader={searchEmptyScreenHeader}
          searchDescription={searchEmptyScreenDescription}
        />
      ) : (
        <>
          {!!descriptionText && (
            <Text className="body-description-text">{descriptionText}</Text>
          )}
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
    </StyledBody>
  );
};

export default Body;
