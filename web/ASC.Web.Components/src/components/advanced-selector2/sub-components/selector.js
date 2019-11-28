import React, { useRef, useState, useEffect } from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import Checkbox from "../../checkbox";
import Link from "../../link";
import ComboBox from "../../combobox";
import SearchInput from "../../search-input";
import Loader from "../../loader";
import { Text } from "../../text";
import Tooltip from "../../tooltip";
import CustomScrollbarsVirtualList from "../../scrollbar/custom-scrollbars-virtual-list";
import ADSelectorColumn from "./column";
import ADSelectorFooter from "./footer";
import ADSelectorHeader from "./header";
import ADSelectorBody from "./body";
import { FixedSizeList as List } from "react-window";
import InfiniteLoader from "react-window-infinite-loader";
import AutoSizer from "react-virtualized-auto-sizer";
import ReactTooltip from "react-tooltip";
import HelpButton from "../../help-button";

/* eslint-disable no-unused-vars */
/* eslint-disable react/prop-types */
const Container = ({ displayType, groups, isMultiSelect, ...props }) => (
  <div {...props} />
);
/* eslint-enable react/prop-types */
/* eslint-enable no-unused-vars */

const StyledContainer = styled(Container)`
  display: grid;

  ${props =>
    props.displayType === "dropdown"
      ? css`
          grid-auto-rows: max-content;
          grid-template-areas: "column-options column-groups" "footer footer";

          .column-groups {
            grid-area: column-groups;

            ${props =>
              props.groups && props.groups.length > 0
                ? css`
                    border-left: 1px solid #eceef1;
                  `
                : ""}

            display: grid;
            /* background-color: gold; */
            padding: 16px 16px 0 16px;
            grid-row-gap: 16px;

            grid-template-columns: 1fr;
            grid-template-rows: 30px 1fr;
            grid-template-areas: "header-groups" "body-groups";

            .header-groups {
              grid-area: header-groups;
              /* background-color: white; */
            }

            .body-groups {
              grid-area: body-groups;
              margin-left: -8px;
              /* background-color: white; */

              .row-block {
                padding-left: 8px;

                .group_checkbox {
                    display: inline-block;
                }
              }
            }
          }
        `
      : css`
          height: 100%;
          grid-template-columns: 1fr;
          grid-template-rows: 1fr 69px;
          grid-template-areas: "column-options" "footer";
        `}

  .column-options {
    grid-area: column-options;

    display: grid;
    /* background-color: red; */
    padding: 16px 16px 0 16px;
    grid-row-gap: 16px;

    grid-template-columns: 1fr;
    grid-template-rows: 60px 1fr;
    grid-template-areas: "header-options" "body-options";

    .header-options {
      grid-area: header-options;
      /* background-color: white; */

      display: grid;
      grid-row-gap: 12px;
      grid-template-columns: 1fr;
      grid-template-rows: 30px 30px;
      grid-template-areas: "options_searcher" "options_group_selector";

      .options_searcher {
        grid-area: options_searcher;
      }

      .options_group_selector {
        grid-area: options_group_selector;
      }
    }

    .body-options {
      grid-area: body-options;
      margin-left: -8px;
      /* background-color: white; */

      .row-block {
        padding-left: 8px;

        .option-info {
          position: absolute;
          top: 10px;
          right: 10px;
        }
      }
    }
  }

  .row-block {
    line-height: 32px;
    cursor: pointer;

    &:hover {
      background-color: #f8f9f9;
    }
  }

  .row-block.selected {
    background-color: #eceef1;
  }

  .footer {
    grid-area: footer;
  }
`;

const ADSelector = props => {
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
    onSelect,
    getOptionTooltipContent,
    onSearchChanged,
    onGroupChanged
  } = props;

  //console.log("options", options); 
  //console.log("hasNextPage", hasNextPage); 
  //console.log("isNextPageLoading", isNextPageLoading);

  const listOptionsRef = useRef(null);
  const listGroupsRef = useRef(null);

  useEffect(() => {
    resetCache();
  }, [searchValue, currentGroup, hasNextPage])

  const [selectedOptionList, setSelectedOptionList] = useState(
    selectedOptions || []
  );

  const [selectedGroupList, setSelectedGroupList] = useState(
    selectedGroups || []
  );
  const [searchValue, setSearchValue] = useState("");

  const convertGroups = items => {
    if (!items) return [];

    const wrappedGroups = items.map(convertGroup);

    return wrappedGroups;
  };

  const convertGroup = group => {
    return {
      key: group.key,
      label: `${group.label} (${group.total})`,
      total: group.total,
      selected: 0
    };
  };

  const getCurrentGroup = items => {
    const currentGroup = items.length > 0 ? items[0] : "No groups";
    return currentGroup;
  };

  const [currentGroup, setCurrentGroup] = useState(
    getCurrentGroup(convertGroups(groups))
  );

  // Every row is loaded except for our loading indicator row.
  const isItemLoaded = index => {
    return !hasNextPage || index < options.length;
  };

  const onOptionChange = e => {
    const option = options[+e.target.value];
    const newSelected = e.target.checked
      ? [option, ...selectedOptionList]
      : selectedOptionList.filter(el => el.key !== option.key);
    setSelectedOptionList(newSelected);

    const newSelectedGroups = [];
    const removedSelectedGroups = [];

    if (e.target.checked) {
      option.groups.forEach(g => {
        let index = selectedGroupList.findIndex(sg => sg.key === g);
        if (index > -1) {
          // exists
          const selectedGroup = selectedGroupList[index];
          const newSelected = selectedGroup.selected + 1;
          newSelectedGroups.push(
            Object.assign({}, selectedGroup, {
              selected: newSelected
            })
          );
        } else {
          index = groups.findIndex(sg => sg.key === g);
          const notSelectedGroup = convertGroup(groups[index]);
          newSelectedGroups.push(
            Object.assign({}, notSelectedGroup, {
              selected: 1
            })
          );
        }
      });
    } else {
      option.groups.forEach(g => {
        let index = selectedGroupList.findIndex(sg => sg.key === g);
        if (index > -1) {
          // exists
          const selectedGroup = selectedGroupList[index];
          const newSelected = selectedGroup.selected - 1;
          if (newSelected > 0) {
            newSelectedGroups.push(
              Object.assign({}, selectedGroup, {
                selected: newSelected
              })
            );
          } else {
            removedSelectedGroups.push(
              Object.assign({}, selectedGroup, {
                selected: newSelected
              })
            );
          }
        }
      });
    }

    selectedGroupList.forEach(g => {
      const indexNew = newSelectedGroups.findIndex(sg => sg.key === g.key);

      if (indexNew === -1) {
        const indexRemoved = removedSelectedGroups.findIndex(
          sg => sg.key === g.key
        );

        if (indexRemoved === -1) {
          newSelectedGroups.push(g);
        }
      }
    });

    setSelectedGroupList(newSelectedGroups);
  };

  const onGroupChange = e => {
    const group = convertGroup(groups[+e.target.value]);
    group.selected = e.target.checked ? group.total : 0;
    const newSelectedGroups = e.target.checked
      ? [group, ...selectedGroupList]
      : selectedGroupList.filter(el => el.key !== group.key);
    //console.log("onGroupChange", item);
    setSelectedGroupList(newSelectedGroups);

    onGroupSelect(group);

    if (e.target.checked) {
      //const newSelectedOptions = [];
      //options.forEach(o => o.groups.forEach(gKey => group.))
      //setSelectedOptionList()
      //TODO: Implement  setSelectedOptionList changes
    }
  };

  const resetCache = () => {
    if (listOptionsRef && listOptionsRef.current) {
      listOptionsRef.current.resetloadMoreItemsCache(true);
    }
  };

  const onGroupSelect = group => {
    if(!currentGroup || !group || currentGroup.key === group.key)
        return;

    setCurrentGroup(group);
    onGroupChanged && onGroupChanged(group);
  };

  const onSearchChange = value => {
    setSearchValue(value);
    onSearchChanged && onSearchChanged(value);
  };

  const onSearchReset = () => {
    onSearchChange("");
  };

  const onSelectOptions = items => {
    onSelect && onSelect(items);
  };

  const isOptionChecked = option => {
    const checked =
      selectedOptionList.findIndex(el => el.key === option.key) > -1 ||
      option.groups.filter(gKey => {
        const selectedGroup = selectedGroupList.find(sg => sg.key === gKey);

        if (!selectedGroup) return false;

        return selectedGroup.total === selectedGroup.selected;
      }).length > 0;

    return checked;
  };

  // Render an item or a loading indicator.
  // eslint-disable-next-line react/prop-types
  const renderOption = ({ index, style }) => {
    let content;
    const isLoaded = isItemLoaded(index);
    let tooltipProps = {};
    if (!isLoaded) {
      content = (
        <div key="loader">
          <Loader
            type="oval"
            size={16}
            style={{
              display: "inline",
              marginRight: "10px"
            }}
          />
          <Text.Body as="span">{loadingLabel}</Text.Body>
        </div>
      );
    } else {
      const option = options[index];
      const isChecked = isOptionChecked(option);

      if (displayType === "dropdown")
        tooltipProps = { "data-for": "user", "data-tip": index };

      ReactTooltip.rebuild();
      //console.log("Item render", item, checked, selected);
      content = isMultiSelect ? (
        <>
          <Checkbox
            id={option.key}
            value={`${index}`}
            label={option.label}
            isChecked={isChecked}
            className="option_checkbox"
            onChange={onOptionChange}
          />
          {displayType === "aside" && (
            <HelpButton
              id={`info-${option.key}`}
              className="option-info"
              iconName="InfoIcon"
              color="#D8D8D8"
              getContent={getOptionTooltipContent}
              place="top"
              offsetLeft={160}
              dataTip={`${index}`}
            />
          )}
        </>
      ) : (
        <Link
          as="span"
          key={option.key}
          truncate={true}
          className="option_link"
          onClick={() => onSelectOptions([option])}
        >
          {option.label}
        </Link>
      );
    }

    return (
      <div style={style} className="row-block" {...tooltipProps}>
        {content}
      </div>
    );
  };

  const isGroupChecked = group => {
    const selectedGroup = selectedGroupList.find(g => g.key === group.key);
    return !!selectedGroup;
  };

  const isGroupIndeterminate = group => {
    const selectedGroup = selectedGroupList.find(g => g.key === group.key);
    return (
      selectedGroup &&
      selectedGroup.selected > 0 &&
      group.total !== selectedGroup.selected
    );
  };

  const getGroupSelected = group => {
    const selectedGroup = selectedGroupList.find(g => g.key === group.key);
    return isGroupIndeterminate(group)
      ? selectedGroup.selected
      : isGroupChecked(group)
      ? group.total
      : 0;
  };

  // eslint-disable-next-line react/prop-types
  const renderGroup = ({ index, style }) => {
    const group = groups[index];

    const isChecked = isGroupChecked(group);
    const isIndeterminate = isGroupIndeterminate(group);
    const isSelected = currentGroup.key === group.key;
    const selected = getGroupSelected(group);

    return (
      <div
        style={style}
        className={`row-block${isSelected ? " selected" : ""}`}
      >
        {isMultiSelect && (
          <Checkbox
            id={group.key}
            value={`${index}`}
            isChecked={isChecked}
            isIndeterminate={isIndeterminate}
            className="group_checkbox"
            onChange={onGroupChange}
          />
        )}
        <Link
          as="span"
          key={group.key}
          truncate={true}
          className="group_link"
          onClick={() => onGroupSelect(group)}
        >
          {isMultiSelect
            ? `${group.label} (${group.total}/${selected})`
            : group.label}
        </Link>
      </div>
    );
  };

  const hasSelected = () => {
    return selectedOptionList.length > 0 || selectedGroupList.length > 0;
  };

  // If there are more items to be loaded then add an extra row to hold a loading indicator.
  const itemCount = hasNextPage ? options.length + 1 : options.length;

  // Only load 1 page of items at a time.
  // Pass an empty callback to InfiniteLoader in case it asks us to load more than once.
  const loadMoreItems = startIndex => {
    if (isNextPageLoading) return;

    const options = {
      startIndex: startIndex || 0,
      searchValue: searchValue,
      currentGroup: currentGroup ? currentGroup.key : null
    };

    console.log("loadMoreItems", options);

    loadNextPage &&
      loadNextPage(options);
  };

  return (
    <StyledContainer
      displayType={displayType}
      groups={groups}
      isMultiSelect={isMultiSelect}
    >
      <ADSelectorColumn className="column-options" displayType={displayType}>
        <ADSelectorHeader className="header-options">
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
            <ComboBox
              className="options_group_selector"
              isDisabled={isDisabled}
              options={groups}
              selectedOption={currentGroup}
              dropDownMaxHeight={200}
              scaled={true}
              scaledOptions={true}
              size="content"
              onSelect={onGroupSelect}
            />
          )}
        </ADSelectorHeader>
        <ADSelectorBody className="body-options">
          <InfiniteLoader
            ref={listOptionsRef}
            isItemLoaded={isItemLoaded}
            itemCount={itemCount}
            loadMoreItems={loadMoreItems}
          >
            {({ onItemsRendered, ref }) => (
              <AutoSizer>
                {({ width, height }) => (
                  <List
                    className="options_list"
                    height={height}
                    itemCount={itemCount}
                    itemSize={32}
                    onItemsRendered={onItemsRendered}
                    ref={ref}
                    width={width + 8}
                    outerElementType={CustomScrollbarsVirtualList}
                  >
                    {renderOption}
                  </List>
                )}
              </AutoSizer>
            )}
          </InfiniteLoader>
          {!hasNextPage && itemCount === 0 && (
            <div className="row-block">
              <Text.Body>
                {!searchValue ? emptyOptionsLabel : emptySearchOptionsLabel}
              </Text.Body>
            </div>
          )}
          <Tooltip
            id="user"
            offsetRight={90}
            getContent={getOptionTooltipContent}
          />
        </ADSelectorBody>
      </ADSelectorColumn>
      {displayType === "dropdown" && groups && groups.length > 0 && (
        <ADSelectorColumn className="column-groups" displayType={displayType}>
          <ADSelectorHeader className="header-groups">
            <Text.Body
              as="p"
              className="group_header"
              fontSize={15}
              isBold={true}
            >
              {groupsHeaderLabel}
            </Text.Body>
          </ADSelectorHeader>
          <ADSelectorBody className="body-groups">
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
          </ADSelectorBody>
        </ADSelectorColumn>
      )}
      <ADSelectorFooter
        className="footer"
        selectButtonLabel={selectButtonLabel}
        isDisabled={isDisabled}
        isVisible={isMultiSelect && hasSelected()}
      />
    </StyledContainer>
  );
};

ADSelector.propTypes = {
  options: PropTypes.array,
  groups: PropTypes.array,

  hasNextPage: PropTypes.bool,
  isNextPageLoading: PropTypes.bool,
  loadNextPage: PropTypes.func,

  isDisabled: PropTypes.bool,
  isMultiSelect: PropTypes.bool,

  selectButtonLabel: PropTypes.string,
  selectAllLabel: PropTypes.string,
  searchPlaceHolderLabel: PropTypes.string,
  groupsHeaderLabel: PropTypes.string,
  emptySearchOptionsLabel: PropTypes.string,
  emptyOptionsLabel: PropTypes.string,
  loadingLabel: PropTypes.string,

  //size: PropTypes.oneOf(["compact", "full"]),
  displayType: PropTypes.oneOf(["dropdown", "aside"]),

  selectedOptions: PropTypes.array,
  selectedGroups: PropTypes.array,

  onSelect: PropTypes.func,
  onSearchChanged: PropTypes.func,
  onGroupChanged: PropTypes.func,
  getOptionTooltipContent: PropTypes.func
};

export default ADSelector;
