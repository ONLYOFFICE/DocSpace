import styled, { css } from "styled-components";

export const StyledHeader = styled.div`
  position: relative;

  display: grid;
  grid-template-columns: ${props =>
    props.showContextButton ? "auto auto auto 1fr" : "auto 1fr"};
  align-items: center;

  @media (max-width: 1024px) {
    grid-template-columns: ${props =>
      props.showContextButton ? "auto 1fr auto" : "auto 1fr"};
  }

  .action-button {
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: 16px;
          `
        : css`
            margin-left: 16px;
          `}
    @media (max-width: 1024px) {
      ${props =>
        props.theme.interfaceDirection === "rtl"
          ? css`
              margin-right: auto;
            `
          : css`
              margin-left: auto;
            `}
      & > div:first-child {
        ${props =>
          props.theme.interfaceDirection === "rtl"
            ? css`
                padding: 8px 0px 8px 16px;
                margin-left: -16px;
              `
            : css`
                padding: 8px 16px 8px 0px;
                margin-right: -16px;
              `}
      }
    }
  }
  .arrow-button {
    ${props =>
      props.theme.interfaceDirection === "rtl" &&
      css`
        transform: scaleX(-1);
      `}
    @media (max-width: 1024px) {
      padding: 8px 16px 8px 16px;
      margin-left: -16px;
      margin-right: -16px;
    }
  }

  .header-headline {
    ${props =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-right: 16px;
          `
        : css`
            margin-left: 16px;
          `}
  }
`;
