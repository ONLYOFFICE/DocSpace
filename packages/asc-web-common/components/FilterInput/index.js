import React from "react";
import { isMobile, isMobileOnly } from "react-device-detect";

import {
  isMobile as isMobileUtils,
  isTablet as isTabletUtils,
} from "@appserver/components/utils/device";

import ViewSelector from "@appserver/components/view-selector";

import FilterButton from "./sub-components/FilterButton";
import SortButton from "./sub-components/SortButton";

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
  }) => {
    const [viewSettings, setViewSettings] = React.useState([]);
    const [inputValue, setInputValue] = React.useState("");

    React.useEffect(() => {
      const value = getViewSettingsData && getViewSettingsData();

      if (value) setViewSettings(value);
    }, [getViewSettingsData]);

    React.useEffect(() => {
      const value = getSelectedInputValue && getSelectedInputValue();

      if (value) setInputValue(value);
    }, [getSelectedInputValue]);

    const onClearSearch = React.useCallback(() => {
      onSearch && onSearch();
    }, [onSearch]);

    return (
      <StyledFilterInput>
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
          getSelectedFilterData={getSelectedFilterData}
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
      </StyledFilterInput>
    );
  }
);

FilterInput.defaultProps = {
  viewSelectorVisible: false,
};

export default FilterInput;
