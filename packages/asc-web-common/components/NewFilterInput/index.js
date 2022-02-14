import React from "react";
import { isMobile, isMobileOnly } from "react-device-detect";

import ViewSelector from "@appserver/components/view-selector";

import FilterButton from "./sub-components/FilterButton";

import { StyledFilterInput, StyledSearchInput } from "./StyledFilterInput";

const FilterInput = ({
  sectionWidth,
  placeholder,
  contextMenuHeader,
  selectedFilterData,
  viewAs,
  onChangeViewAs,
  viewSelectorVisible,
  getViewSettingsData,
  getFilterData,
  onFilter,
  onSearch,
  addUserHeader,
  ...props
}) => {
  const [viewSettings, setViewSettings] = React.useState([]);
  const [inputValue, setInputValue] = React.useState("");

  React.useEffect(() => {
    setInputValue(
      !!selectedFilterData.inputValue ? selectedFilterData.inputValue : ""
    );
  }, [selectedFilterData]);

  React.useEffect(() => {
    getViewSettingsData && setViewSettings(getViewSettingsData());
  }, [getViewSettingsData]);

  const onClearSearch = () => {
    onSearch && onSearch();
  };

  return (
    <StyledFilterInput sectionWidth={sectionWidth}>
      <StyledSearchInput
        placeholder={placeholder}
        value={inputValue}
        onChange={onSearch}
        onClearSearch={onClearSearch}
      />

      <FilterButton
        selectedFilterData={selectedFilterData}
        contextMenuHeader={contextMenuHeader}
        getFilterData={getFilterData}
        onFilter={onFilter}
        addUserHeader={addUserHeader}
      />

      {viewSettings && !isMobile && viewSelectorVisible && (
        <ViewSelector
          style={{ marginLeft: "8px" }}
          onChangeView={onChangeViewAs}
          viewAs={viewAs === "table" ? "row" : viewAs}
          viewSettings={viewSettings}
        />
      )}
    </StyledFilterInput>
  );
};

FilterInput.defaultProps = {
  viewSelectorVisible: true,
};

export default React.memo(FilterInput);
