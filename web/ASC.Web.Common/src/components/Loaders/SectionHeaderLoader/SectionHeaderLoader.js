import React from "react";
import styled from "styled-components";
import RectangleLoader from "../RectangleLoader/index";

import { utils } from "asc-web-components";
const { mobile, tablet } = utils.device;

const StyledContainer = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 100px 0fr 42px;
  grid-template-rows: 1fr;
  grid-column-gap: 16px;
  margin-top: 20px;
  margin-bottom: 18px;

  @media ${mobile}, ${tablet} {
    grid-template-columns: 100px 1fr 42px;
  }
`;

const StyledBox1 = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 17px 67px;
  grid-template-rows: 1fr;
  grid-column-gap: 16px;
`;

const StyledBox2 = styled.div`
  display: grid;
  grid-template-columns: 17px 17px;
  grid-template-rows: 1fr;
  grid-column-gap: 8px;
`;

const StyledSpacer = styled.div``;

const SectionHeaderLoader = (props) => {
  return (
    <StyledContainer>
      <StyledBox1>
        <RectangleLoader width="17" height="17" {...props} />
        <RectangleLoader width="67" height="17" {...props} />
      </StyledBox1>
      <StyledSpacer />
      <StyledBox2>
        <RectangleLoader width="17" height="17" {...props} />
        <RectangleLoader width="17" height="17" {...props} />
      </StyledBox2>
    </StyledContainer>
  );
};

export default SectionHeaderLoader;
