import React, { useRef, useState, useEffect, useCallback } from "react";
import PropTypes from "prop-types";

import Header from "./Header";
import Search from "./Search";
import GroupList from "./GroupList";
import GroupHeader from "./GroupHeader";
import OptionList from "./OptionList";
import Option from "./Option";

import Footer from "./Footer";

import Text from "@docspace/components/text";
import Tooltip from "@docspace/components/tooltip";

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
    selectedCount: 0,
  };
};

const getCurrentGroup = (items) => {
  const currentGroup = items.length > 0 ? items[0] : {};
  return currentGroup;
};

const Selector = (props) => {
  const {
    groups,
    isDisabled,
    isMultiSelect,
    hasNextPage,
    options,
    isNextPageLoading,
    loadNextPage,
    selectedOptions,
    selectedGroups,
    searchPlaceHolderLabel,
    emptySearchOptionsLabel,
    emptyOptionsLabel,
    loadingLabel,
    onSelect,
    getOptionTooltipContent,
    onSearchChanged,
    onGroupChanged,
    size,
    embeddedComponent,
    showCounter,
    onArrowClick,
    headerLabel,
    total,
    isFirstLoad,
  } = props;

  const listOptionsRef = useRef(null);

  useEffect(() => {
    Object.keys(currentGroup).length === 0 &&
      setCurrentGroup(getCurrentGroup(convertGroups(groups)));
    resetCache();
  }, [searchValue, currentGroup, hasNextPage]);

  const resetCache = useCallback(() => {
    if (listOptionsRef && listOptionsRef.current) {
      listOptionsRef.current.resetloadMoreItemsCache(true);
    }
  }, [listOptionsRef]);

  const [selectedOptionList, setSelectedOptionList] = useState(
    selectedOptions || []
  );

  const [searchValue, setSearchValue] = useState("");

  const [groupList, setGroupList] = useState([]);

  const [currentGroup, setCurrentGroup] = useState(
    getCurrentGroup(convertGroups(groups))
  );

  const [groupHeader, setGroupHeader] = useState(null);

  useEffect(() => {
    if (groups.length === 0) return;

    const newGroupList = [...groups];

    if (
      groups.length === 1 &&
      selectedOptions &&
      selectedOptions.length === 0
    ) {
      return setGroupHeader(newGroupList[0]);
    }

    if (selectedOptions && selectedOptions.length === 0) {
      return setGroupList(newGroupList);
    }

    if (selectedOptions) {
      newGroupList[0].selectedCount = selectedOptions.length;

      if (groups.length === 1) return setGroupHeader(newGroupList[0]);
      selectedOptions.forEach((option) => {
        option?.groups?.forEach((group) => {
          const groupIndex = newGroupList.findIndex(
            (newGroup) => group === newGroup.id
          );

          if (groupIndex > -1) {
            newGroupList[groupIndex].selectedCount =
              newGroupList[groupIndex].selectedCount + 1;
          }
        });
      });
    }
    if (groups.length === 1) return setGroupHeader(newGroupList[0]);
    setGroupList(newGroupList);
  }, [groups, selectedOptions]);

  useEffect(() => {
    if (total) {
      setGroupHeader({ ...groupHeader, total: total });

      const newGroupList = groupList;

      if (newGroupList.length > 0) {
        newGroupList.find(
          (group) => group.key === groupHeader.key
        ).total = total;
      }

      setGroupList(newGroupList);
    }
  }, [total]);

  const onSearchChange = useCallback(
    (value) => {
      setSearchValue(value);
      onSearchChanged && onSearchChanged(value);
    },
    [onSearchChanged]
  );

  const onSearchReset = useCallback(() => {
    onSearchChanged && onSearchChange("");
  }, [onSearchChanged]);

  // Every row is loaded except for our loading indicator row.
  const isItemLoaded = useCallback(
    (index) => {
      return !hasNextPage || index < options.length;
    },
    [hasNextPage, options]
  );

  const onOptionChange = useCallback(
    (idx, isChecked) => {
      const indexList = Array.isArray(idx) ? idx : [idx];

      let newSelected = selectedOptionList;
      let newGroupList = groupList;
      let newGroupHeader = { ...groupHeader };

      indexList.forEach((index) => {
        newGroupHeader.selectedCount = isChecked
          ? newGroupHeader.selectedCount - 1
          : newGroupHeader.selectedCount + 1;

        const option = options[index];

        newSelected = !isChecked
          ? [option, ...newSelected]
          : newSelected.filter((el) => el.key !== option.key);

        if (!option.groups) {
          setSelectedOptionList(newSelected);
          setGroupHeader(newGroupHeader);
          return;
        }

        if (newGroupList.length) {
          newGroupList[0].selectedCount = isChecked
            ? newGroupList[0].selectedCount - 1
            : newGroupList[0].selectedCount + 1;

          option.groups.forEach((group) => {
            const groupIndex = newGroupList.findIndex(
              (item) => item.key === group
            );

            if (groupIndex > 0) {
              newGroupList[groupIndex].selectedCount = isChecked
                ? newGroupList[groupIndex].selectedCount - 1
                : newGroupList[groupIndex].selectedCount + 1;
            }
          });
        }
      });

      setSelectedOptionList(newSelected);
      setGroupList(newGroupList);
      setGroupHeader(newGroupHeader);
    },
    [options, groupList, selectedOptionList, groupHeader]
  );

  const isOptionChecked = useCallback(
    (option) => {
      const checked = selectedOptionList.find(
        (item) => item.key === option.key
      );

      return !!checked;
    },
    [selectedOptionList]
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

  // If there are more items to be loaded then add an extra row to hold a loading indicator.

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

  const onSelectAll = useCallback(() => {
    const currentSelectedOption = [];
    selectedOptionList.forEach((selectedOption) => {
      options.forEach((option, idx) => {
        if (option.key === selectedOption.key) currentSelectedOption.push(idx);
      });
    });

    if (currentSelectedOption.length > 0) {
      return onOptionChange(currentSelectedOption, true);
    }

    onOptionChange(
      options.map((item, index) => index),
      false
    );
  }, [onOptionChange, selectedOptionList, options]);

  const onGroupClick = useCallback(
    (index) => {
      const group = groupList[index];

      setGroupHeader({ ...group });

      onGroupChanged && onGroupChanged(group);
      setCurrentGroup(group);
    },
    [groupList, onGroupChanged]
  );

  const onArrowClickAction = useCallback(() => {
    if (groupHeader && groups.length !== 1) {
      setGroupHeader(null);

      onGroupChanged && onGroupChanged([]);
      setCurrentGroup([]);
      return;
    }

    onArrowClick && onArrowClick();
  }, [groups, groupHeader && groupHeader.label, onArrowClick, onGroupChanged]);

  const renderGroupsList = useCallback(() => {
    if (groupList.length === 0) {
      return <Option isLoader={true} />;
    }

    return (
      <GroupList
        groupList={groupList}
        isMultiSelect={isMultiSelect}
        onGroupClick={onGroupClick}
      />
    );
  }, [isMultiSelect, groupList, onGroupClick]);

  const itemCount = hasNextPage ? options.length + 1 : options.length;
  const hasSelected = selectedOptionList.length > 0;

  return (
    <StyledSelector
      isMultiSelect={isMultiSelect}
      hasSelected={hasSelected}
      className="selector-wrapper"
    >
      <Header
        headerLabel={headerLabel}
        onArrowClickAction={onArrowClickAction}
      />
      <div style={{ height: "100%" }} className="column-options" size={size}>
        <Search
          isDisabled={isDisabled}
          placeholder={searchPlaceHolderLabel}
          value={searchValue}
          onSearchChange={onSearchChange}
          onClearSearch={onSearchReset}
        />
        <div style={{ width: "100%", height: "100%" }} className="body-options">
          {!groupHeader && !searchValue && groups ? (
            renderGroupsList()
          ) : (
            <>
              {!searchValue && (
                <>
                  <GroupHeader
                    {...groupHeader}
                    onSelectAll={onSelectAll}
                    isMultiSelect={isMultiSelect}
                    isIndeterminate={
                      groupHeader.selectedCount > 0 &&
                      groupHeader.selectedCount !== groupHeader.total
                    }
                    isChecked={
                      groupHeader.total !== 0 &&
                      groupHeader.total === groupHeader.selectedCount
                    }
                  />
                  <div className="option-separator"></div>
                </>
              )}
              {!hasNextPage && itemCount === 0 ? (
                <div className="row-option">
                  <Text>
                    {!searchValue ? emptyOptionsLabel : emptySearchOptionsLabel}
                  </Text>
                </div>
              ) : (
                <OptionList
                  listOptionsRef={listOptionsRef}
                  options={options}
                  itemCount={itemCount}
                  isMultiSelect={isMultiSelect}
                  onOptionChange={onOptionChange}
                  onLinkClick={onLinkClick}
                  isItemLoaded={isItemLoaded}
                  isOptionChecked={isOptionChecked}
                  loadMoreItems={loadMoreItems}
                  isFirstLoad={isFirstLoad}
                />
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
        </div>
      </div>
      <Footer
        className="footer"
        selectButtonLabel={headerLabel}
        showCounter={showCounter}
        isDisabled={isDisabled}
        isVisible={isMultiSelect && hasSelected}
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
  isFirstLoad: PropTypes.bool,

  selectButtonLabel: PropTypes.string,
  selectAllLabel: PropTypes.string,
  searchPlaceHolderLabel: PropTypes.string,
  groupsHeaderLabel: PropTypes.string,
  emptySearchOptionsLabel: PropTypes.string,
  emptyOptionsLabel: PropTypes.string,
  loadingLabel: PropTypes.string,

  selectedOptions: PropTypes.array,
  selectedGroups: PropTypes.array,

  onSelect: PropTypes.func,
  onSearchChanged: PropTypes.func,
  onGroupChanged: PropTypes.func,
  getOptionTooltipContent: PropTypes.func,

  embeddedComponent: PropTypes.any,
};

export default React.memo(Selector);
