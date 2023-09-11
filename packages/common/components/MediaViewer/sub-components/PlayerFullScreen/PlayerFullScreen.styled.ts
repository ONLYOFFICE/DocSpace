import styled, { css } from "styled-components";

export const PlayerFullSceenWrapper = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  min-width: 48px;
  height: 48px;

  ${props =>
    props.theme.interfaceDirection === "rtl"
      ? css`
          padding-right: 10px;
        `
      : css`
          padding-left: 10px;
        `}

  &:hover {
    cursor: pointer;
  }
`;
