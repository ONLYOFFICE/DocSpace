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
    sectionWidth,
    getFilterData,
    getSortData,
    getViewSettingsData,
    getSelectedFilterData,
    getSelectedSortData,
    getSelectedInputValue,
    onFilter,
    onSearch,
    onSort,
    onChangeViewAs,
    viewAs,
    placeholder,
    view,
    contextMenuHeader,
    headerLabel,
    viewSelectorVisible,
    isFavoritesFolder,
    isRecentFolder,

    isLoading,
  }) => {
    const [viewSettings, setViewSettings] = React.useState([]);
    const [inputValue, setInputValue] = React.useState("");
    const [selectedFilterData, setSelectedFilterData] = React.useState([]);

    const getSelectedFilterDataAction = React.useCallback(async () => {
      const data = await getSelectedFilterData();

      setSelectedFilterData(data);
    }, [getSelectedFilterData]);

    React.useEffect(() => {
      getSelectedFilterDataAction();
    }, [getSelectedFilterData]);

    React.useEffect(() => {
      const value = getViewSettingsData();

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
          selectedFilterData={selectedFilterData}
          getFilterData={getFilterData}
          getSelectedFilterData={getSelectedFilterData}
          onFilter={onFilter}
          contextMenuHeader={contextMenuHeader}
          headerLabel={headerLabel}
        />
        {!isRecentFolder && (
          <SortButton
            t={t}
            getSortData={getSortData}
            getSelectedSortData={getSelectedSortData}
            onChangeViewAs={onChangeViewAs}
            view={view}
            viewAs={viewAs === "table" ? "row" : viewAs}
            viewSettings={viewSettings}
            onSort={onSort}
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
            onChangeView={onChangeViewAs}
            viewAs={viewAs === "table" ? "row" : viewAs}
            viewSettings={viewSettings}
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
