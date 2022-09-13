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

  width: 100%;
  min-height: 122px;
  height: auto;

  display: flex;

  box-sizing: border-box;

  flex-direction: column;

  ${(props) =>
    !props.isLast &&
    css`
      border-bottom: 1px solid #eceef1;
    `}

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
