import React from "react";

import IconButton from "@docspace/components/icon-button";
import { Base } from "@docspace/components/themes";

import FilterBlock from "./FilterBlock";

import StyledButton from "./StyledButton";

import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";
const FilterButton = ({
  t,
  onFilter,
  getFilterData,

  selectedFilterValue,

  filterHeader,
  selectorLabel,
}) => {
  const [showFilterBlock, setShowFilterBlock] = React.useState(false);

  const changeShowFilterBlock = React.useCallback(() => {
    setShowFilterBlock((value) => !value);
  }, [setShowFilterBlock]);

  return (
    <>
      <StyledButton onClick={changeShowFilterBlock}>
        <IconButton iconName="/static/images/filter.react.svg" size={16} />
        {selectedFilterValue && selectedFilterValue.length > 0 && (
          <ColorTheme type={ThemeType.IndicatorFilterButton} />
        )}
      </StyledButton>

      {showFilterBlock && (
        <FilterBlock
          t={t}
          filterHeader={filterHeader}
          selectedFilterValue={selectedFilterValue}
          hideFilterBlock={changeShowFilterBlock}
          getFilterData={getFilterData}
          onFilter={onFilter}
          selectorLabel={selectorLabel}
        />
      )}
    </>
  );
};

export default React.memo(FilterButton);
