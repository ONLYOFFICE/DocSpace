import React, { useRef, useState, useEffect, useCallback } from "react";
import PropTypes from "prop-types";
import Column from "./Column";
import Footer from "./Footer";
import Header from "./Header";
import Body from "./Body";
import { FixedSizeList as List } from "react-window";
import InfiniteLoader from "react-window-infinite-loader";
import AutoSizer from "react-virtualized-auto-sizer";
import ReactTooltip from "react-tooltip";

import Avatar from "@appserver/components/avatar";
import Checkbox from "@appserver/components/checkbox";
import SearchInput from "@appserver/components/search-input";
import Loader from "@appserver/components/loader";
import Text from "@appserver/components/text";
import Tooltip from "@appserver/components/tooltip";
import Heading from "@appserver/components/heading";
import IconButton from "@appserver/components/icon-button";
import CustomScrollbarsVirtualList from "@appserver/components/scrollbar/custom-scrollbars-virtual-list";

import StyledSelector from "./StyledSelector";

const convertGroups = (items) => {
  if (!items) return [];

  const wrappedGroups = items.map(convertGroup);

  return wrappedGroups;
};

const convertGroup = (group) => {
  return {
    key: group.key,
    label: `${group.label} (${group.total})`,
    total: group.total,
    selected: 0,
  };
};

const getCurrentGroup = (items) => {
  const currentGroup = items.length > 0 ? items[0] : {};
  return currentGroup;
};

const Selector = (props) => {
  const {
    groups,
    selectButtonLabel,
    isDisabled,
    isMultiSelect,
    hasNextPage,
    options,
    isNextPageLoading,
    loadNextPage,
    selectedOptions,
    selectedGroups,
    groupsHeaderLabel,
    searchPlaceHolderLabel,
    emptySearchOptionsLabel,
    emptyOptionsLabel,
    loadingLabel,
    selectAllLabel,
    onSelect,
    getOptionTooltipContent,
    onSearchChanged,
    onGroupChanged,
    size,
    allowGroupSelection,
    embeddedComponent,
    showCounter,
    onArrowClick,
    headerLabel,
  } = props;

  const listOptionsRef = useRef(null);

  useEffect(() => {
    Object.keys(currentGroup).length === 0 &&
      setCurrentGroup(getCurrentGroup(convertGroups(groups)));
    resetCache();
  }, [searchValue, currentGroup, hasNextPage]);

  const [selectedOptionList, setSelectedOptionList] = useState(
    selectedOptions || []
  );

  const [selectedGroupList, setSelectedGroupList] = useState(
    selectedGroups || []
  );
  const [searchValue, setSearchValue] = useState("");

  const [currentGroup, setCurrentGroup] = useState(
    getCurrentGroup(convertGroups(groups))
  );

  const [groupHeader, setGroupHeader] = useState(null);

  useEffect(() => {
    if (groups.length === 1) setGroupHeader(groups[0]);
  }, [groups]);

  // Every row is loaded except for our loading indicator row.
  const isItemLoaded = useCallback(
    (index) => {
      return !hasNextPage || index < options.length;
    },
    [hasNextPage, options]
  );

  const onOptionChange = useCallback(
    (index, isChecked) => {
      const option = options[index];
      const newSelected = !isChecked
        ? [option, ...selectedOptionList]
        : selectedOptionList.filter((el) => el.key !== option.key);
      setSelectedOptionList(newSelected);

      if (!option.groups) return;

      const newSelectedGroups = [];
      const removedSelectedGroups = [];

      if (isChecked) {
        option.groups.forEach((g) => {
          let index = selectedGroupList.findIndex((sg) => sg.key === g);
          if (index > -1) {
            // exists
            const selectedGroup = selectedGroupList[index];
            const newSelected = selectedGroup.selected + 1;
            newSelectedGroups.push(
              Object.assign({}, selectedGroup, {
                selected: newSelected,
              })
            );
          } else {
            index = groups.findIndex((sg) => sg.key === g);
            if (index < 0) return;
            const notSelectedGroup = convertGroup(groups[index]);
            newSelectedGroups.push(
              Object.assign({}, notSelectedGroup, {
                selected: 1,
              })
            );
          }
        });
      } else {
        option.groups.forEach((g) => {
          let index = selectedGroupList.findIndex((sg) => sg.key === g);
          if (index > -1) {
            // exists
            const selectedGroup = selectedGroupList[index];
            const newSelected = selectedGroup.selected - 1;
            if (newSelected > 0) {
              newSelectedGroups.push(
                Object.assign({}, selectedGroup, {
                  selected: newSelected,
                })
              );
            } else {
              removedSelectedGroups.push(
                Object.assign({}, selectedGroup, {
                  selected: newSelected,
                })
              );
            }
          }
        });
      }

      selectedGroupList.forEach((g) => {
        const indexNew = newSelectedGroups.findIndex((sg) => sg.key === g.key);

        if (indexNew === -1) {
          const indexRemoved = removedSelectedGroups.findIndex(
            (sg) => sg.key === g.key
          );

          if (indexRemoved === -1) {
            newSelectedGroups.push(g);
          }
        }
      });

      setSelectedGroupList(newSelectedGroups);
    },
    [options, selectedOptionList, groups, selectedGroupList]
  );

  const resetCache = useCallback(() => {
    if (listOptionsRef && listOptionsRef.current) {
      listOptionsRef.current.resetloadMoreItemsCache(true);
    }
  }, [listOptionsRef]);

  const onSearchChange = useCallback((value) => {
    setSearchValue(value);
    onSearchChanged && onSearchChanged(value);
  });

  const onSearchReset = useCallback(() => {
    onSearchChanged && onSearchChange("");
  });

  const isOptionChecked = useCallback(
    (option) => {
      const checked =
        selectedOptionList.findIndex((el) => el.key === option.key) > -1 ||
        (option.groups &&
          option.groups.filter((gKey) => {
            const selectedGroup = selectedGroupList.find(
              (sg) => sg.key === gKey
            );

            if (!selectedGroup) return false;

            return selectedGroup.total === selectedGroup.selected;
          }).length > 0);

      return checked;
    },
    [selectedOptionList, selectedGroupList]
  );
  const onSelectOptions = (items) => {
    onSelect && onSelect(items);
  };

  const onAddClick = useCallback(() => {
    onSelectOptions(selectedOptionList);
  }, [selectedOptionList]);

  const onLinkClick = useCallback(
    (index) => {
      const option = options[index];

      if (!option) return;

      onSelectOptions([option]);
    },
    [options]
  );

  const renderOptionItem = useCallback(
    (index, style, option, isChecked, tooltipProps) => {
      return isMultiSelect ? (
        <div
          style={style}
          className="row-option"
          value={`${index}`}
          name={`selector-row-option-${index}`}
          onClick={() => onOptionChange(index, isChecked)}
          {...tooltipProps}
        >
          <div className="option-info">
            <Avatar
              className="option-avatar"
              role="user"
              size="min"
              source={option.avatarUrl}
              userName={option.label}
            />
            <Text
              className="option-text"
              truncate={true}
              noSelect={true}
              fontSize="14px"
            >
              {option.label}
            </Text>
          </div>
          <Checkbox
            id={option.key}
            value={`${index}`}
            isChecked={isChecked}
            className="option-checkbox"
          />
        </div>
      ) : (
        <div
          key={option.key}
          style={style}
          className="row-option"
          data-index={index}
          name={`selector-row-option-${index}`}
          onClick={() => onLinkClick(index)}
          {...tooltipProps}
        >
          <div className="option-info">
            {" "}
            <Avatar
              className="option-avatar"
              role="user"
              size="min"
              source={option.avatarUrl}
              userName={option.label}
            />
            <Text
              className="option-text"
              truncate={true}
              noSelect={true}
              fontSize="14px"
            >
              {option.label}
            </Text>
          </div>
        </div>
      );
    },
    [isMultiSelect, onOptionChange, onLinkClick]
  );

  const renderOptionLoader = useCallback(
    (style) => {
      return (
        <div style={style} className="row-option">
          <div key="loader">
            <Loader
              type="oval"
              size="16px"
              style={{
                display: "inline",
                marginRight: "10px",
              }}
            />
            <Text as="span" noSelect={true}>
              {loadingLabel}
            </Text>
          </div>
        </div>
      );
    },
    [loadingLabel]
  );

  // Render an item or a loading indicator.
  // eslint-disable-next-line react/prop-types
  const renderOption = useCallback(
    ({ index, style }) => {
      const isLoaded = isItemLoaded(index);

      if (!isLoaded) {
        return renderOptionLoader(style);
      }

      const option = options[index];
      const isChecked = isOptionChecked(option);
      let tooltipProps = {};

      ReactTooltip.rebuild();

      return renderOptionItem(index, style, option, isChecked, tooltipProps);
    },
    [
      isItemLoaded,
      renderOptionLoader,
      renderOptionItem,
      loadingLabel,
      options,
      isOptionChecked,
      isMultiSelect,
      onOptionChange,
      onLinkClick,
      getOptionTooltipContent,
    ]
  );

  const hasSelected = useCallback(() => {
    return selectedOptionList.length > 0 || selectedGroupList.length > 0;
  }, [selectedOptionList, selectedGroupList]);

  // If there are more items to be loaded then add an extra row to hold a loading indicator.
  const itemCount = hasNextPage ? options.length + 1 : options.length;

  // Only load 1 page of items at a time.
  // Pass an empty callback to InfiniteLoader in case it asks us to load more than once.
  const loadMoreItems = useCallback(
    (startIndex) => {
      if (isNextPageLoading) return;

      const options = {
        startIndex: startIndex || 0,
        searchValue: searchValue,
        currentGroup: currentGroup ? currentGroup.key : null,
      };

      loadNextPage && loadNextPage(options);
    },
    [isNextPageLoading, searchValue, currentGroup, options]
  );

  const getGroupSelectedOptions = useCallback(
    (group) => {
      const selectedGroup = selectedOptionList.filter(
        (o) => o.groups && o.groups.indexOf(group) > -1
      );

      if (group === "all") {
        selectedGroup.push(...selectedOptionList);
      }

      return selectedGroup;
    },
    [selectedOptionList]
  );

  const onGroupClick = useCallback(
    (index) => {
      const group = groups[index];

      setGroupHeader({ ...group });

      onGroupChanged && onGroupChanged(group);
      setCurrentGroup(group);
    },
    [groups, onGroupChanged]
  );

  const renderGroup = useCallback(
    ({ index, style }) => {
      const group = groups[index];

      const selectedOption = getGroupSelectedOptions(group.id);

      const isIndeterminate = selectedOption.length > 0;

      let label = group.label;

      if (isMultiSelect && selectedOption.length > 0) {
        label = `${group.label} (${selectedOption.length})`;
      }

      return (
        <div
          style={style}
          className="row-option"
          name={`selector-row-option-${index}`}
          onClick={() => onGroupClick(index)}
        >
          <div className="option-info">
            <Avatar
              className="option-avatar"
              role="user"
              size="min"
              source={group.avatarUrl}
              userName={group.label}
            />
            <Text
              className="option-text option-text__group"
              truncate={true}
              noSelect={true}
              fontSize="14px"
            >
              {label}
            </Text>
          </div>
          {isMultiSelect && (
            <Checkbox
              value={`${index}`}
              isIndeterminate={isIndeterminate}
              className="option-checkbox"
            />
          )}
        </div>
      );
    },
    [
      isMultiSelect,
      groups,
      currentGroup,
      selectedGroupList,
      selectedOptionList,
      getGroupSelectedOptions,
    ]
  );

  const renderGroupsList = useCallback(() => {
    if (groups.length === 0) return renderOptionLoader();
    return (
      <AutoSizer>
        {({ width, height }) => (
          <List
            className="options_list"
            height={height - 8}
            width={width + 8}
            itemCount={groups.length}
            itemSize={48}
            outerElementType={CustomScrollbarsVirtualList}
          >
            {renderGroup}
          </List>
        )}
      </AutoSizer>
    );
  }, [isMultiSelect, groups, selectedOptionList, getGroupSelectedOptions]);

  const renderGroupHeader = useCallback(() => {
    const selectedOption = getGroupSelectedOptions(groupHeader.id);

    const isIndeterminate = selectedOption.length > 0;

    let label = groupHeader.label;

    if (isMultiSelect && selectedOption.length > 0) {
      label = `${groupHeader.label} (${selectedOption.length})`;
    }
    return (
      <>
        <div className="row-option row-header">
          <div className="option-info">
            <Avatar
              className="option-avatar"
              role="user"
              size="min"
              source={groupHeader.avatarUrl}
              userName={groupHeader.label}
            />
            <Text
              className="option-text option-text__header"
              truncate={true}
              noSelect={true}
              fontSize="14px"
            >
              {label}
            </Text>
          </div>
          {isMultiSelect && (
            <Checkbox
              isIndeterminate={isIndeterminate}
              className="option-checkbox"
            />
          )}
        </div>
        <div className="option-separator"></div>
      </>
    );
  }, [isMultiSelect, groupHeader, selectedOptionList, getGroupSelectedOptions]);

  const onArrowClickAction = useCallback(() => {
    if (groupHeader && groups.length !== 1) {
      setGroupHeader(null);

      onGroupChanged && onGroupChanged([]);
      setCurrentGroup([]);
      return;
    }
    onArrowClick && onArrowClick();
  }, [groups, groupHeader, onArrowClick, onGroupChanged]);

  return (
    <StyledSelector
      options={options}
      groups={groups}
      isMultiSelect={isMultiSelect}
      allowGroupSelection={allowGroupSelection}
      hasSelected={hasSelected()}
      className="selector-wrapper"
    >
      <div className="header">
        <IconButton
          iconName="/static/images/arrow.path.react.svg"
          size="17"
          isFill={true}
          className="arrow-button"
          onClick={onArrowClickAction}
        />
        <Heading size="medium" truncate={true}>
          {headerLabel.replace("()", "")}
        </Heading>
      </div>
      <Column className="column-options" size={size}>
        <Header className="header-options">
          <SearchInput
            className="options_searcher"
            isDisabled={isDisabled}
            size="base"
            scale={true}
            isNeedFilter={false}
            placeholder={searchPlaceHolderLabel}
            value={searchValue}
            onChange={onSearchChange}
            onClearSearch={onSearchReset}
          />
        </Header>
        <Body className="body-options">
          {!groupHeader && !searchValue && groups ? (
            renderGroupsList()
          ) : (
            <>
              {!searchValue && renderGroupHeader()}
              {!hasNextPage && itemCount === 0 ? (
                <div className="row-option">
                  <Text>
                    {!searchValue ? emptyOptionsLabel : emptySearchOptionsLabel}
                  </Text>
                </div>
              ) : (
                <AutoSizer>
                  {({ width, height }) => (
                    <InfiniteLoader
                      ref={listOptionsRef}
                      isItemLoaded={isItemLoaded}
                      itemCount={itemCount}
                      loadMoreItems={loadMoreItems}
                    >
                      {({ onItemsRendered, ref }) => (
                        <List
                          className="options_list"
                          height={height - 25}
                          itemCount={itemCount}
                          itemSize={48}
                          onItemsRendered={onItemsRendered}
                          ref={ref}
                          width={width + 8}
                          outerElementType={CustomScrollbarsVirtualList}
                        >
                          {renderOption}
                        </List>
                      )}
                    </InfiniteLoader>
                  )}
                </AutoSizer>
              )}
            </>
          )}

          {getOptionTooltipContent && (
            <Tooltip
              id="user"
              offsetRight={90}
              getContent={getOptionTooltipContent}
            />
          )}
        </Body>
      </Column>
      <Footer
        className="footer"
        selectButtonLabel={headerLabel}
        showCounter={showCounter}
        isDisabled={isDisabled}
        isVisible={isMultiSelect && hasSelected()}
        onClick={onAddClick}
        embeddedComponent={embeddedComponent}
        selectedLength={selectedOptionList.length}
      />
    </StyledSelector>
  );
};

Selector.propTypes = {
  options: PropTypes.array,
  groups: PropTypes.array,

  hasNextPage: PropTypes.bool,
  isNextPageLoading: PropTypes.bool,
  loadNextPage: PropTypes.func,

  isDisabled: PropTypes.bool,
  isMultiSelect: PropTypes.bool,
  allowGroupSelection: PropTypes.bool,

  selectButtonLabel: PropTypes.string,
  selectAllLabel: PropTypes.string,
  searchPlaceHolderLabel: PropTypes.string,
  groupsHeaderLabel: PropTypes.string,
  emptySearchOptionsLabel: PropTypes.string,
  emptyOptionsLabel: PropTypes.string,
  loadingLabel: PropTypes.string,

  size: PropTypes.oneOf(["compact", "full"]),

  selectedOptions: PropTypes.array,
  selectedGroups: PropTypes.array,

  onSelect: PropTypes.func,
  onSearchChanged: PropTypes.func,
  onGroupChanged: PropTypes.func,
  getOptionTooltipContent: PropTypes.func,

  embeddedComponent: PropTypes.any,
};

Selector.defaultProps = {
  size: "full",
};

export default Selector;
