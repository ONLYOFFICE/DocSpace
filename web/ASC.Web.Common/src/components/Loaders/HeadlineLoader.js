import React from "react";
import styled from "styled-components";
import RectangleLoader from "./RectangleLoader";

import { utils } from "asc-web-components";
import { ShareAccessRights } from "../../constants";
const { desktop } = utils.device;

const StyledContainer = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 17px 67px 17px 17px;
  grid-template-rows: 1fr;
  grid-column-gap: 16px;
  margin-top: 20px;
  margin-bottom: 18px;
`;

const StyledBox = styled.div`
  display: grid;
  grid-template-columns: 17px 17px;
  grid-template-rows: 1fr;
  grid-column-gap: 8px;
`;

const HeadlineLoader = () => {
  return (
    <StyledContainer>
      <RectangleLoader width="17" height="17" />
      <RectangleLoader width="67" height="17" />
      <StyledBox>
        <RectangleLoader width="17" height="17" />
        <RectangleLoader width="17" height="17" />
      </StyledBox>
    </StyledContainer>
  );
};

export default HeadlineLoader;
