import React from "react";
import styled, { css } from "styled-components";

const StyledContainer = styled.div`
  box-sizing: border-box;

  width: 100%;

  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(274px, 1fr));

  grid-gap: 16px;

  overflow: hidden;

  margin-bottom: 30px;
`;

const TileContainer = ({ children }) => {
  return <StyledContainer>{children}</StyledContainer>;
};

export default TileContainer;
