import React from 'react';
import { isMobile, isMobileOnly } from 'react-device-detect';

import ViewSelector from '@appserver/components/view-selector';

import FilterButton from './sub-components/FilterButton';

import { StyledFilterInput, StyledSearchInput } from './StyledFilterInput';

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
  ...props
}) => {
  const [viewSettings, setViewSettings] = React.useState([]);

  React.useEffect(() => {
    getViewSettingsData && setViewSettings(getViewSettingsData());
  }, [getViewSettingsData]);

  return (
    <StyledFilterInput sectionWidth={sectionWidth}>
      <StyledSearchInput placeholder={placeholder} />

      <FilterButton
        selectedFilterData={selectedFilterData}
        contextMenuHeader={contextMenuHeader}
        getFilterData={getFilterData}
        onFilter={onFilter}
      />

      {viewSettings && !isMobile && viewSelectorVisible && (
        <ViewSelector
          style={{ marginLeft: '8px' }}
          onChangeView={onChangeViewAs}
          viewAs={viewAs === 'table' ? 'row' : viewAs}
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
