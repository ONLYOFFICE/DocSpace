import React from "react";
import styled from "styled-components";

import IconButton from "@docspace/components/icon-button";
import { Base } from "@docspace/components/themes";

import FilterBlock from "./FilterBlock";

import StyledButton from "./StyledButton";

const Indicator = styled.div`
  border-radius: 50%;
  width: 8px;
  height: 8px;

  background: ${(props) => props.theme.filterInput.filter.indicatorColor};

  position: absolute;
  top: 25px;
  left: 25px;

  z-index: 10;
`;

Indicator.defaultProps = { theme: Base };

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
        {selectedFilterValue && selectedFilterValue.length > 0 && <Indicator />}
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
