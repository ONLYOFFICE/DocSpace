import React from "react";
import styled from "styled-components";
import RectangleLoader from "../RectangleLoader/index";
import CircleLoader from "../CircleLoader/index";

const StyledHeader = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 24px 168px 1fr 24px 36px;
  grid-template-rows: 1fr;
  grid-column-gap: 16px;
  margin: 16px;
`;

const StyledSpacer = styled.div``;

const HeaderLoader = (props) => {
  return (
    <StyledHeader>
      <RectangleLoader
        width="24"
        height="24"
        backgroundColor="#fff"
        foregroundColor="#fff"
        {...props}
      />
      <RectangleLoader
        width="168"
        height="24"
        backgroundColor="#fff"
        foregroundColor="#fff"
        {...props}
      />
      <StyledSpacer />
      <RectangleLoader
        width="24"
        height="24"
        backgroundColor="#fff"
        foregroundColor="#fff"
        {...props}
      />
      <StyledSpacer />
    </StyledHeader>
  );
};

export default HeaderLoader;
