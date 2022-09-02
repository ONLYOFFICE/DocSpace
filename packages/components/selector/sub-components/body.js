import React from "react";
import { FixedSizeList as List } from "react-window";

import CustomScrollbarsVirtualList from "../../scrollbar/custom-scrollbars-virtual-list";

import Search from "./search";
import SelectAll from "./select-all";
import Item from "./item";

import { StyledSelectorBody } from "../StyledSelector";

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
}) => {
  const [bodyHeight, setBodyHeight] = React.useState(null);

  const bodyRef = React.useRef(null);

  const withSearch = isSearch || items.length > 0;

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

  React.useEffect(() => {
    return () => {
      bodyRef?.current?.removeEventListener("resize", onBodyResize);
    };
  }, [onBodyResize]);

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
        <List
          className="items-list"
          height={listHeight}
          width={"100%"}
          itemCount={items.length}
          itemData={{ items, onSelect, isMultiSelect }}
          itemSize={48}
          outerElementType={CustomScrollbarsVirtualList}
        >
          {Item}
        </List>
      )}
    </StyledSelectorBody>
  );
};

export default Body;
