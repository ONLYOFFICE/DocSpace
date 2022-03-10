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

const FilterInput = ({
  t,
  sectionWidth,
  getFilterData,
  getSortData,
  getViewSettingsData,
  getSelectedFilterData,
  onFilter,
  onSearch,
  onSort,
  onChangeViewAs,
  viewAs,
  placeholder,
  contextMenuHeader,
  headerLabel,
  viewSelectorVisible,
  isRecentFolder,
  isFavoritesFolder,
  ...props
}) => {
  const [viewSettings, setViewSettings] = React.useState([]);
  const [selectedFilterData, setSelectedFilterData] = React.useState([]);

  const [inputValue, setInputValue] = React.useState("");

  const getSelectedFilterDataAction = React.useCallback(async () => {
    const data = await getSelectedFilterData();

    setSelectedFilterData(data);
    setInputValue(!!data.inputValue ? data.inputValue : "");
  }, [getSelectedFilterData]);

  React.useEffect(() => {
    getSelectedFilterDataAction();
  }, [getSelectedFilterData]);

  React.useEffect(() => {
    getViewSettingsData && setViewSettings(getViewSettingsData());
  }, [getViewSettingsData]);

  const onClearSearch = () => {
    onSearch && onSearch();
  };

  return (
    <StyledFilterInput {...props} sectionWidth={sectionWidth}>
      <StyledSearchInput
        placeholder={placeholder}
        value={inputValue}
        onChange={onSearch}
        onClearSearch={onClearSearch}
      />

      <FilterButton
        t={t}
        selectedFilterData={selectedFilterData}
        contextMenuHeader={contextMenuHeader}
        getFilterData={getFilterData}
        onFilter={onFilter}
        headerLabel={headerLabel}
      />

      {viewSettings &&
      !isMobile &&
      viewSelectorVisible &&
      !isMobileUtils() &&
      !isTabletUtils() ? (
        <ViewSelector
          style={{ marginLeft: "8px" }}
          onChangeView={onChangeViewAs}
          viewAs={viewAs === "table" ? "row" : viewAs}
          viewSettings={viewSettings}
        />
      ) : (
        <>
          {(isMobile || isTabletUtils() || isMobileUtils()) && (
            <SortButton
              t={t}
              selectedFilterData={selectedFilterData}
              getSortData={getSortData}
              onChangeViewAs={onChangeViewAs}
              viewAs={viewAs === "table" ? "row" : viewAs}
              viewSettings={viewSettings}
              onSort={onSort}
              viewSelectorVisible={viewSelectorVisible}
              isRecentFolder={isRecentFolder}
              isFavoritesFolder={isFavoritesFolder}
            />
          )}
        </>
      )}
    </StyledFilterInput>
  );
};

FilterInput.defaultProps = {
  viewSelectorVisible: false,
};

export default React.memo(FilterInput);
