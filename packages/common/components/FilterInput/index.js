import React from "react";
import { isMobile, isMobileOnly } from "react-device-detect";

import {
  isMobile as isMobileUtils,
  isTablet as isTabletUtils,
} from "@docspace/components/utils/device";

import ViewSelector from "@docspace/components/view-selector";
import Link from "@docspace/components/link";

import FilterButton from "./sub-components/FilterButton";
import SortButton from "./sub-components/SortButton";
import SelectedItem from "@docspace/components/selected-item";

import { StyledFilterInput, StyledSearchInput } from "./StyledFilterInput";

const FilterInput = React.memo(
  ({
    t,
    onFilter,
    getFilterData,
    getSelectedFilterData,
    onSort,
    getSortData,
    getSelectedSortData,
    view,
    viewAs,
    viewSelectorVisible,
    getViewSettingsData,
    onChangeViewAs,
    placeholder,
    onSearch,
    getSelectedInputValue,

    filterHeader,
    selectorLabel,
    clearAll,

    isRecentFolder,
    removeSelectedItem,

    isPersonalRoom,
    isRooms,
    isAccounts,
    filterTitle,
    sortByTitle,

    clearSearch,
    setClearSearch,

    onSortButtonClick,
    onClearFilter,
  }) => {
    const [viewSettings, setViewSettings] = React.useState([]);
    const [inputValue, setInputValue] = React.useState("");
    const [selectedFilterValue, setSelectedFilterValue] = React.useState(null);
    const [selectedItems, setSelectedItems] = React.useState(null);

    const mountRef = React.useRef(true);

    React.useEffect(() => {
      const value = getViewSettingsData && getViewSettingsData();

      if (value) setViewSettings(value);
    }, [getViewSettingsData]);

    React.useEffect(() => {
      if (clearSearch) {
        setInputValue("");
        onClearFilter && onClearFilter();
        setClearSearch(false);
      }
    }, [clearSearch]);

    React.useEffect(() => {
      const value = getSelectedInputValue && getSelectedInputValue();

      setInputValue(value);
    }, [getSelectedInputValue]);

    React.useEffect(() => {
      getSelectedFilterDataAction();
    }, [getSelectedFilterDataAction, getSelectedFilterData]);

    const getSelectedFilterDataAction = React.useCallback(async () => {
      const value = await getSelectedFilterData();

      if (!mountRef.current) return;
      setSelectedFilterValue(value);

      const newSelectedItems = [];

      value.forEach((item) => {
        if (item.isMultiSelect) {
          const newKeys = item.key.map((oldKey) => ({
            key: oldKey.key ? oldKey.key : oldKey,
            group: item.group,
            label: oldKey.label ? oldKey.label : oldKey,
          }));

          return newSelectedItems.push(...newKeys);
        }

        return newSelectedItems.push({ ...item });
      });

      setSelectedItems(newSelectedItems);
    }, [getSelectedFilterData]);

    const onClearSearch = React.useCallback(() => {
      onSearch && onSearch();
    }, [onSearch]);

    const removeSelectedItemAction = React.useCallback(
      (key, label, group) => {
        const newItems = selectedItems
          .map((item) => ({ ...item }))
          .filter((item) => item.key != key);

        setSelectedItems(newItems);

        removeSelectedItem({ key, group });
      },
      [selectedItems, removeSelectedItem],
    );

    React.useEffect(() => {
      return () => {
        mountRef.current = false;
      };
    }, []);

    return (
      <StyledFilterInput>
        <div className="filter-input_filter-row">
          <StyledSearchInput
            placeholder={placeholder}
            value={inputValue}
            onChange={onSearch}
            onClearSearch={onClearSearch}
            id="filter_search-input"
          />
          <FilterButton
            t={t}
            id="filter-button"
            onFilter={onFilter}
            getFilterData={getFilterData}
            selectedFilterValue={selectedFilterValue}
            filterHeader={filterHeader}
            selectorLabel={selectorLabel}
            isPersonalRoom={isPersonalRoom}
            isRooms={isRooms}
            isAccounts={isAccounts}
            title={filterTitle}
          />
          {!isRecentFolder && (
            <SortButton
              t={t}
              id="sort-by-button"
              onSort={onSort}
              getSortData={getSortData}
              getSelectedSortData={getSelectedSortData}
              view={view}
              viewAs={viewAs === "table" ? "row" : viewAs}
              viewSettings={viewSettings}
              onChangeViewAs={onChangeViewAs}
              onSortButtonClick={onSortButtonClick}
              viewSelectorVisible={
                viewSettings &&
                viewSelectorVisible &&
                (isMobile || isMobileUtils() || isTabletUtils())
              }
              title={sortByTitle}
            />
          )}
          {((viewSettings &&
            !isMobile &&
            viewSelectorVisible &&
            !isMobileUtils() &&
            !isTabletUtils()) ||
            isRecentFolder) && (
            <ViewSelector
              id={viewAs === "tile" ? "view-switch--row" : "view-switch--tile"}
              style={{ marginLeft: "8px" }}
              viewAs={viewAs === "table" ? "row" : viewAs}
              viewSettings={viewSettings}
              onChangeView={onChangeViewAs}
              isFilter={true}
            />
          )}
        </div>
        {selectedItems && selectedItems.length > 0 && (
          <div className="filter-input_selected-row">
            {selectedItems.map((item) => (
              <SelectedItem
                key={`${item.key}_${item.group}`}
                propKey={item.key}
                label={item.selectedLabel ? item.selectedLabel : item.label}
                group={item.group}
                onClose={removeSelectedItemAction}
                onClick={removeSelectedItemAction}
              />
            ))}
            {selectedItems.filter((item) => item.label).length > 1 && (
              <Link
                className={"clear-all-link"}
                isHovered
                fontWeight={600}
                isSemitransparent
                type="action"
                onClick={clearAll}>
                {t("Common:ClearAll")}
              </Link>
            )}
          </div>
        )}
      </StyledFilterInput>
    );
  },
);

FilterInput.defaultProps = {
  viewSelectorVisible: false,
};

export default FilterInput;
