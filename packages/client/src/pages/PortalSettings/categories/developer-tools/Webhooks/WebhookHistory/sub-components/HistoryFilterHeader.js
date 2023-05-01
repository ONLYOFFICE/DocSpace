import React, { useState } from "react";
import styled from "styled-components";

import { Base } from "@docspace/components/themes";
import FilterReactSvrUrl from "PUBLIC_DIR/images/filter.react.svg?url";
import IconButton from "@docspace/components/icon-button";

import { useParams } from "react-router-dom";
import FilterDialog from "./FilterDialog";
import StatusBar from "./StatusBar";

const ListHeader = styled.header`
  display: flex;
  justify-content: space-between;
  align-items: center;
`;

const ListHeading = styled.h3`
  font-size: 16px;
  line-height: 22px;
  color: #333333;
  font-weight: 700;
  margin: 0;
`;

const FilterButton = styled.div`
  display: flex;
  box-sizing: border-box;
  flex-direction: row;
  justify-content: center;
  align-items: center;

  width: 32px;
  height: 32px;

  border: 1px solid #d0d5da;
  border-radius: 3px;
  cursor: pointer;

  svg {
    cursor: pointer;
  }

  :hover {
    border-color: #a3a9ae;
    svg {
      path {
        fill: ${(props) => props.theme.iconButton.hoverColor};
      }
    }
  }
`;

const HistoryFilterHeader = (props) => {
  const { applyFilters } = props;
  const { id } = useParams();

  const [isFiltersVisible, setIsFiltersVisible] = useState(false);
  const [isApplied, setIsApplied] = useState(false);

  const openFiltersModal = () => {
    setIsFiltersVisible(true);
  };

  const closeFiltersModal = () => {
    setIsFiltersVisible(false);
  };

  return (
    <div>
      <ListHeader>
        <ListHeading>Webhook {id}</ListHeading>

        <FilterButton onClick={openFiltersModal}>
          <IconButton iconName={FilterReactSvrUrl} size={16} />
        </FilterButton>
      </ListHeader>
      {isApplied && <StatusBar />}
      <FilterDialog
        visible={isFiltersVisible}
        closeModal={closeFiltersModal}
        applyFilters={applyFilters}
        isApplied={isApplied}
        setIsApplied={setIsApplied}
      />
    </div>
  );
};

FilterButton.defaultProps = { theme: Base };

export default HistoryFilterHeader;
