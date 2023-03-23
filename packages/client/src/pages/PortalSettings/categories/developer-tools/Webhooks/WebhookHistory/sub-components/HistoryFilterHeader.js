import React from "react";
import styled from "styled-components";

import { Base } from "@docspace/components/themes";
import FilterReactSvrUrl from "PUBLIC_DIR/images/filter.react.svg?url";
import IconButton from "@docspace/components/icon-button";

import { useParams } from "react-router-dom";

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

const HistoryFilterHeader = () => {
  const { id } = useParams();

  return (
    <ListHeader>
      <ListHeading>Webhook {id}</ListHeading>

      <FilterButton>
        <IconButton iconName={FilterReactSvrUrl} size={16} />
      </FilterButton>
    </ListHeader>
  );
};

FilterButton.defaultProps = { theme: Base };

export default HistoryFilterHeader;
