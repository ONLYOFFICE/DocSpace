import styled, { css } from "styled-components";

const StyledContainer = styled.div`
  width: 100%;
  height: 100%;

  padding: 0 16px;

  box-sizing: border-box;
`;

const StyledBlock = styled.div`
  padding: 12px 0 16px;

  margin-bottom: 4px;

  width: 100%;
  height: auto;

  display: flex;
  flex-direction: column;

  gap: 12px 8px;

  box-sizing: border-box;

  ${(props) =>
    !props.isLast &&
    css`
      border-bottom: 1px solid #eceef1;
    `}

  .row-loader {
    display: flex;

    align-items: center;
    flex-wrap: wrap;

    gap: 8px;
  }
`;

export { StyledContainer, StyledBlock };
