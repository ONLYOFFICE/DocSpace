import React from "react";
import { isMobile, isMobileOnly } from "react-device-detect";

import {
  isMobile as isMobileUtils,
  isTablet as isTabletUtils,
} from "@docspace/components/utils/device";

import ViewSelector from "@docspace/components/view-selector";

import FilterButton from "./sub-components/FilterButton";
import SortButton from "./sub-components/SortButton";
import SelectedItem from "./sub-components/SelectedItem";

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

    isRecentFolder,
    removeSelectedItem,
  }) => {
    const [viewSettings, setViewSettings] = React.useState([]);
    const [inputValue, setInputValue] = React.useState("");
    const [selectedFilterValue, setSelectedFilterValue] = React.useState(null);
    const [selectedItems, setSelectedItems] = React.useState(null);

    React.useEffect(() => {
      const value = getViewSettingsData && getViewSettingsData();

      if (value) setViewSettings(value);
    }, [getViewSettingsData]);

    React.useEffect(() => {
      const value = getSelectedInputValue && getSelectedInputValue();

      if (value) setInputValue(value);
    }, [getSelectedInputValue]);

    React.useEffect(() => {
      getSelectedFilterDataAction();
    }, [getSelectedFilterDataAction, getSelectedFilterData]);

    const getSelectedFilterDataAction = React.useCallback(async () => {
      const value = await getSelectedFilterData();

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
      [selectedItems, removeSelectedItem]
    );

    return (
      <StyledFilterInput>
        <div className="filter-input_filter-row">
          <StyledSearchInput
            placeholder={placeholder}
            value={inputValue}
            onChange={onSearch}
            onClearSearch={onClearSearch}
          />
          <FilterButton
            t={t}
            onFilter={onFilter}
            getFilterData={getFilterData}
            selectedFilterValue={selectedFilterValue}
            filterHeader={filterHeader}
            selectorLabel={selectorLabel}
          />
          {!isRecentFolder && (
            <SortButton
              t={t}
              onSort={onSort}
              getSortData={getSortData}
              getSelectedSortData={getSelectedSortData}
              view={view}
              viewAs={viewAs === "table" ? "row" : viewAs}
              viewSettings={viewSettings}
              onChangeViewAs={onChangeViewAs}
              viewSelectorVisible={
                viewSettings &&
                viewSelectorVisible &&
                (isMobile || isMobileUtils() || isTabletUtils())
              }
            />
          )}
          {((viewSettings &&
            !isMobile &&
            viewSelectorVisible &&
            !isMobileUtils() &&
            !isTabletUtils()) ||
            isRecentFolder) && (
            <ViewSelector
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
                {...item}
                removeSelectedItem={removeSelectedItemAction}
              />
            ))}
          </div>
        )}
      </StyledFilterInput>
    );
  }
);

FilterInput.defaultProps = {
  viewSelectorVisible: false,
};

export default FilterInput;
