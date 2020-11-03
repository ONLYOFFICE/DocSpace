import React from "react";
import styled from "styled-components";
import RectangleLoader from "./RectangleLoader";
import { utils } from "asc-web-components";
const { mobile } = utils.device;

const StyledFilter = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 1fr 95px;
  grid-template-rows: 1fr;
  grid-column-gap: 8px;

  @media ${mobile} {
    grid-template-columns: 1fr 50px;
  }
`;

const FilterLoader = () => {
  return (
    <StyledFilter>
      <RectangleLoader width="100%" height="32px" />
      <RectangleLoader width="100%" height="32px" />
    </StyledFilter>
  );
};

export default FilterLoader;
