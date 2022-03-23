import React from "react";
import styled from "styled-components";

import IconButton from "@appserver/components/icon-button";
import { Base } from "@appserver/components/themes";

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

  z-index: 3;
`;

Indicator.defaultProps = { theme: Base };

const FilterButton = ({
  t,
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

  // console.log(selectedFilterData.filterValues);

  return (
    <>
      <StyledButton onClick={changeShowFilterBlock}>
        <IconButton iconName="/static/images/filter.react.svg" size={16} />
        {selectedFilterData.filterValues &&
          selectedFilterData.filterValues.length > 0 && <Indicator />}
      </StyledButton>

      {showFilterBlock && (
        <FilterBlock
          t={t}
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
