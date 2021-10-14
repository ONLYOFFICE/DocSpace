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

import Checkbox from "@appserver/components/checkbox";
import Link from "@appserver/components/link";
import ComboBox from "@appserver/components/combobox";
import SearchInput from "@appserver/components/search-input";
import Loader from "@appserver/components/loader";
import Text from "@appserver/components/text";
import Tooltip from "@appserver/components/tooltip";
import CustomScrollbarsVirtualList from "@appserver/components/scrollbar/custom-scrollbars-virtual-list";
import HelpButton from "@appserver/components/help-button";

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
    displayType,
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
  } = props;

  //console.log("options", options);
  //console.log("hasNextPage", hasNextPage);
  //console.log("isNextPageLoading", isNextPageLoading);

  const listOptionsRef = useRef(null);
  const listGroupsRef = useRef(null);

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

  const [selectedAll, setSelectedAll] = useState(false);

  const [currentGroup, setCurrentGroup] = useState(
    getCurrentGroup(convertGroups(groups))
  );

  // Every row is loaded except for our loading indicator row.
  const isItemLoaded = useCallback(
    (index) => {
      return !hasNextPage || index < options.length;
    },
    [hasNextPage, options]
  );

  const onOptionChange = useCallback(
    (e) => {
      const option = options[+e.target.value];
      const newSelected = e.target.checked
        ? [option, ...selectedOptionList]
        : selectedOptionList.filter((el) => el.key !== option.key);
      setSelectedOptionList(newSelected);

      if (!option.groups) return;

      const newSelectedGroups = [];
      const removedSelectedGroups = [];

      if (e.target.checked) {
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

  const onGroupChange = useCallback(
    (e) => {
      const group = convertGroup(groups[+e.target.value]);
      group.selected = e.target.checked ? group.total : 0;
      const newSelectedGroups = e.target.checked
        ? [group, ...selectedGroupList]
        : selectedGroupList.filter((el) => el.key !== group.key);
      //console.log("onGroupChange", item);
      setSelectedGroupList(newSelectedGroups);

      onGroupSelect(group);

      if (e.target.checked) {
        //const newSelectedOptions = [];
        //options.forEach(o => o.groups.forEach(gKey => group.))
        //setSelectedOptionList()
        //TODO: Implement  setSelectedOptionList changes
      }
    },
    [groups, selectedGroupList, currentGroup]
  );

  const resetCache = useCallback(() => {
    if (listOptionsRef && listOptionsRef.current) {
      listOptionsRef.current.resetloadMoreItemsCache(true);
    }
  }, [listOptionsRef]);

  const onGroupSelect = useCallback(
    (group) => {
      if (!currentGroup || !group || currentGroup.key === group.key) {
        return;
      }

      setCurrentGroup(group);
      onGroupChanged && onGroupChanged(group);

      if (displayType === "aside" && isMultiSelect) {
        setSelectedAll(isGroupChecked(group));
      }
    },
    [displayType, isMultiSelect, currentGroup]
  );

  const onSelectAllChange = useCallback(() => {
    const checked = !selectedAll;
    //console.log("onSelectAllChange", checked);
    setSelectedAll(checked);

    if (!currentGroup) return;

    const group = convertGroup(currentGroup);

    if (!group) return;

    group.selected = checked ? group.total : 0;
    const newSelectedGroups = checked
      ? [group, ...selectedGroupList]
      : selectedGroupList.filter((el) => el.key !== group.key);

    setSelectedGroupList(newSelectedGroups);
  }, [selectedAll, currentGroup, selectedGroupList]);

  const onSearchChange = useCallback((value) => {
    setSearchValue(value);
    onSearchChanged && onSearchChanged(value);
  });

  const onSearchReset = useCallback(() => {
    onSearchChanged && onSearchChange("");
  });

  const onSelectOptions = (items) => {
    onSelect && onSelect(items);
  };

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

  const onLinkClick = useCallback(
    (e) => {
      const index = e.target.dataset.index;
      if (!index) return;

      const option = options[index];

      if (!option) return;

      onSelectOptions([option]);
    },
    [options]
  );

  const onAddClick = useCallback(() => {
    onSelectOptions(selectedOptionList);
  }, [selectedOptionList]);

  const renderOptionItem = useCallback(
    (index, style, option, isChecked, tooltipProps) => {
      return isMultiSelect ? (
        <div style={style} className="row-option" {...tooltipProps}>
          <Checkbox
            id={option.key}
            value={`${index}`}
            label={option.label}
            isChecked={isChecked}
            className="option_checkbox"
            truncate={true}
            title={option.label}
            onChange={onOptionChange}
          />
          {displayType === "aside" && getOptionTooltipContent && (
            <HelpButton
              id={`info-${option.key}`}
              className="option-info"
              iconName="/static/images/info.react.svg"
              color="#D8D8D8"
              getContent={getOptionTooltipContent}
              place="top"
              offsetLeft={150}
              offsetRight={0}
              offsetTop={60}
              offsetBottom={0}
              dataTip={`${index}`}
              displayType="dropdown"
            />
          )}
        </div>
      ) : (
        <Link
          key={option.key}
          data-index={index}
          isTextOverflow={true}
          style={style}
          className="row-option"
          {...tooltipProps}
          onClick={onLinkClick}
          noHover
        >
          {option.label}
          {displayType === "aside" && getOptionTooltipContent && (
            <HelpButton
              id={`info-${option.key}`}
              className="option-info"
              iconName="/static/images/info.react.svg"
              color="#D8D8D8"
              getContent={getOptionTooltipContent}
              place="top"
              offsetLeft={150}
              offsetRight={0}
              offsetTop={60}
              offsetBottom={0}
              dataTip={`${index}`}
              displayType="dropdown"
            />
          )}
        </Link>
      );
    },
    [
      isMultiSelect,
      onOptionChange,
      onLinkClick,
      displayType,
      getOptionTooltipContent,
    ]
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
            <Text as="span">{loadingLabel}</Text>
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

      if (displayType === "dropdown")
        tooltipProps = { "data-for": "user", "data-tip": index };

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
      displayType,
      isMultiSelect,
      onOptionChange,
      onLinkClick,
      getOptionTooltipContent,
    ]
  );

  const isGroupChecked = useCallback(
    (group) => {
      const selectedGroup = selectedGroupList.find((g) => g.key === group.key);
      return !!selectedGroup;
    },
    [selectedGroupList]
  );

  const isGroupIndeterminate = useCallback(
    (group) => {
      const selectedGroup = selectedGroupList.find((g) => g.key === group.key);
      return (
        selectedGroup &&
        selectedGroup.selected > 0 &&
        group.total !== selectedGroup.selected
      );
    },
    [selectedGroupList]
  );

  const getGroupSelected = useCallback(
    (group) => {
      const selectedGroup = selectedGroupList.find((g) => g.key === group.key);
      return isGroupIndeterminate(group)
        ? selectedGroup.selected
        : isGroupChecked(group)
        ? group.total
        : 0;
    },
    [selectedGroupList]
  );

  const getGroupLabel = useCallback(
    (group) => {
      const selected = getGroupSelected(group);
      return isMultiSelect && allowGroupSelection
        ? `${group.label} (${group.total}/${selected})`
        : group.label;
    },
    [isMultiSelect, allowGroupSelection]
  );

  const getSelectorGroups = useCallback(
    (groups) => {
      return groups.map((group) => {
        return {
          ...group,
          label: getGroupLabel(group),
        };
      });
    },
    [groups]
  );

  const onLinkGroupClick = useCallback(
    (e) => {
      const index = e.target.dataset.index;
      if (!index) return;

      const group = groups[index];

      if (!group) return;

      onGroupSelect(group);
    },
    [groups, currentGroup]
  );

  // eslint-disable-next-line react/prop-types
  const renderGroup = useCallback(
    ({ index, style }) => {
      const group = groups[index];

      const isChecked = isGroupChecked(group);
      const isIndeterminate = isGroupIndeterminate(group);
      const isSelected = currentGroup.key === group.key;
      const label = getGroupLabel(group);

      return (
        <Link
          key={group.key}
          data-index={index}
          isTextOverflow={true}
          onClick={onLinkGroupClick}
          title={label}
          style={style}
          className={`row-group${isSelected ? " selected" : ""}`}
          noHover
        >
          {isMultiSelect && allowGroupSelection && (
            <Checkbox
              id={group.key}
              value={`${index}`}
              isChecked={isChecked}
              isIndeterminate={isIndeterminate}
              className="group_checkbox"
              truncate={true}
              onChange={onGroupChange}
            />
          )}
          {label}
        </Link>
      );
    },
    [
      groups,
      currentGroup,
      isMultiSelect,
      selectedGroupList,
      allowGroupSelection,
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

      //setLastIndex(startIndex);

      //console.log("loadMoreItems", options);

      loadNextPage && loadNextPage(options);
    },
    [isNextPageLoading, searchValue, currentGroup, options]
  );

  return (
    <StyledSelector
      displayType={displayType}
      options={options}
      groups={groups}
      isMultiSelect={isMultiSelect}
      allowGroupSelection={allowGroupSelection}
      hasSelected={hasSelected()}
      className="selector-wrapper"
    >
      <Column className="column-options" displayType={displayType} size={size}>
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
          {displayType === "aside" && groups && groups.length > 0 && (
            <>
              <ComboBox
                className="options_group_selector"
                isDisabled={isDisabled}
                options={getSelectorGroups(groups)}
                selectedOption={currentGroup}
                dropDownMaxHeight={220}
                scaled={true}
                scaledOptions={true}
                size="content"
                isDefaultMode={false}
                onSelect={onGroupSelect}
              />
              {isMultiSelect &&
                allowGroupSelection &&
                options &&
                options.length > 0 && (
                  <Checkbox
                    className="options_group_select_all"
                    label={selectAllLabel}
                    isChecked={selectedAll}
                    isIndeterminate={false}
                    truncate={true}
                    onChange={onSelectAllChange}
                  />
                )}
            </>
          )}
        </Header>
        <Body className="body-options">
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
                    height={height}
                    itemCount={itemCount}
                    itemSize={36}
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

          {!hasNextPage && itemCount === 0 && (
            <div className="row-option">
              <Text>
                {!searchValue ? emptyOptionsLabel : emptySearchOptionsLabel}
              </Text>
            </div>
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
      {displayType === "dropdown" && groups && groups.length > 0 && (
        <>
          <div className="splitter"></div>
          <Column
            className="column-groups"
            displayType={displayType}
            size={size}
          >
            <Header className="header-groups">
              <Text
                as="p"
                className="group_header"
                fontSize="15px"
                fontWeight={600}
              >
                {groupsHeaderLabel}
              </Text>
            </Header>
            <Body className="body-groups">
              <AutoSizer>
                {({ height, width }) => (
                  <List
                    className="group_list"
                    height={height}
                    width={width + 8}
                    itemSize={32}
                    itemCount={groups.length}
                    itemData={groups}
                    outerElementType={CustomScrollbarsVirtualList}
                    ref={listGroupsRef}
                  >
                    {renderGroup}
                  </List>
                )}
              </AutoSizer>
            </Body>
          </Column>
        </>
      )}
      <Footer
        className="footer"
        selectButtonLabel={selectButtonLabel}
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
  displayType: PropTypes.oneOf(["dropdown", "aside"]),

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
