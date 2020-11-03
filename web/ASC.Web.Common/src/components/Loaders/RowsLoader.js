import React from "react";
import styled from "styled-components";
import RectangleLoader from "./RectangleLoader";

import { utils } from "asc-web-components";
const { isMobile, isTablet } = utils.device;

const StyledRow = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 16px 22px 1fr;
  grid-template-rows: 1fr;
  grid-column-gap: 8px;
  margin-bottom: 32px;
  justify-items: center;
  align-items: center;
`;

const StyledBox = styled.div`
  width: 100%;
  display: grid;
  grid-template-columns: 1fr;
  grid-template-rows: 16px 12px;
  grid-row-gap: 4px;
  justify-items: left;
  align-items: left;
`;

const RowBlock = () => {
  const numberLines = !isMobile() && !isTablet() ? 1 : 2;

  if (numberLines === 1) {
    return <RectangleLoader width="100%" height="16" />;
  } else {
    return (
      <StyledBox>
        <RectangleLoader width="80%" height="16" />
        <RectangleLoader width="100%" height="12" />
      </StyledBox>
    );
  }
};

const Row = () => {
  const rectangleSize = !isMobile() && !isTablet() ? 22 : 32;
  return (
    <StyledRow>
      <RectangleLoader width="16" height="16" />
      <RectangleLoader width={rectangleSize} height={rectangleSize} />
      <RowBlock />
    </StyledRow>
  );
};

const RowsLoader = () => {
  return (
    <div>
      <Row />
      <Row />
      <Row />
      <Row />
      <Row />
    </div>
  );
};

export default RowsLoader;
