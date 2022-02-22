import React from "react";

import IconButton from "@appserver/components/icon-button";

import FilterBlock from "./FilterBlock";

import StyledButton from "./StyledButton";

const FilterButton = ({
  selectedFilterData,
  contextMenuHeader,
  getFilterData,
  onFilter,
  headerLabel,
}) => {
  const [showFilterBlock, setShowFilterBlock] = React.useState(false);

  const changeShowFilterBlock = React.useCallback(() => {
    setShowFilterBlock((value) => !value);
  }, [setShowFilterBlock]);

  return (
    <>
      <StyledButton onClick={changeShowFilterBlock}>
        <IconButton iconName="/static/images/filter.react.svg" size={16} />
      </StyledButton>

      {showFilterBlock && (
        <FilterBlock
          contextMenuHeader={contextMenuHeader}
          selectedFilterData={selectedFilterData}
          hideFilterBlock={changeShowFilterBlock}
          getFilterData={getFilterData}
          onFilter={onFilter}
          headerLabel={headerLabel}
        />
      )}
    </>
  );
};

export default React.memo(FilterButton);
