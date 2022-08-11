import styled, { css } from "styled-components";

import RectangleLoader from "../RectangleLoader";

const StyledContainer = styled.div`
  width: 100%;
  height: 100%;

  padding: 0 16px;

  box-sizing: border-box;
`;

const StyledBlock = styled.div`
  padding: 12px 0 6px;

  margin-bottom: 6px;

  width: 100%;
  height: auto;

  display: flex;

  flex-direction: column;

  border-bottom: 1px solid #eceef1;

  .row-loader {
    display: flex;

    align-items: center;
  }

  .loader-item {
    margin-bottom: 12px;
    margin-right: 8px;
  }
`;

export { StyledContainer, StyledBlock };
