import React from "react";
import styled from "styled-components";
import RectangleLoader from "../RectangleLoader/index";

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

const RowBlock = (props) => {
  if (!isMobile() && !isTablet()) {
    return <RectangleLoader width="100%" height="16" {...props} />;
  } else {
    return (
      <StyledBox>
        <RectangleLoader width="80%" height="16" {...props} />
        <RectangleLoader width="100%" height="12" {...props} />
      </StyledBox>
    );
  }
};

const Row = (props) => {
  const rectangleSize = !isMobile() && !isTablet() ? 22 : 32;
  return (
    <StyledRow>
      <RectangleLoader width="16" height="16" {...props} />
      <RectangleLoader
        width={rectangleSize}
        height={rectangleSize}
        {...props}
      />
      <RowBlock {...props} />
    </StyledRow>
  );
};

const RowsLoader = (props) => {
  return (
    <div>
      <Row {...props} />
      <Row {...props} />
      <Row {...props} />
      <Row {...props} />
      <Row {...props} />
      <Row {...props} />
    </div>
  );
};

export default RowsLoader;
