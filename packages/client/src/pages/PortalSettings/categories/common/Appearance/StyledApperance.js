import { smallTablet } from "@docspace/components/utils/device";
import PlusThemeSvgUrl from "PUBLIC_DIR/images/plus.theme.svg?url";
import styled, { css } from "styled-components";

const StyledComponent = styled.div`
  padding-top: 3px;
  width: 100%;
  max-width: 575px;

  .header {
    font-weight: 700;
    font-size: 16px;
    line-height: 22px;
  }

  .preview-header {
    padding-bottom: 20px;
  }

  .theme-standard-container {
    padding-top: 21px;
  }

  .theme-name {
    font-size: 15px;
    line-height: 16px;
    font-weight: 600;
  }

  .theme-container {
    padding: 12px 0 24px 0;
    display: flex;
  }

  .custom-themes {
    display: flex;
  }

  .theme-add {
    width: 46px;
    height: 46px;
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            margin-left: 12px;
          `
        : css`
            margin-right: 12px;
          `}
    border-radius: 8px;
    cursor: pointer;
    background: ${(props) => (props.theme.isBase ? "#eceef1" : "#474747")}
      url(${PlusThemeSvgUrl}) no-repeat center;
  }

  .add-theme {
    background: #d0d5da;
    padding-top: 16px;
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            padding-right: 16px;
          `
        : css`
            padding-left: 16px;
          `}
    box-sizing: border-box;
  }

  .buttons-container {
    display: flex;
    padding-top: 24px;

    .button:not(:last-child) {
      ${(props) =>
        props.theme.interfaceDirection === "rtl"
          ? css`
              margin-left: 8px;
            `
          : css`
              margin-right: 8px;
            `}
    }
    @media ${smallTablet} {
      .button {
        width: 100%;
      }
    }

    ${({ isShowDeleteButton }) =>
      isShowDeleteButton &&
      css`
        @media ${smallTablet} {
          flex-direction: column;
          gap: 8px;
          margin: 0;

          .button:not(:last-child) {
            margin-right: 0px;
          }
        }
      `}
  }

  .check-img {
    ${(props) =>
      props.theme.interfaceDirection === "rtl"
        ? css`
            padding: 16px 15px 0 0;
          `
        : css`
            padding: 16px 0 0 15px;
          `}
    svg path {
      fill: ${(props) => props.colorCheckImg};
    }
  }
`;

const StyledTheme = styled.div`
  width: 46px;
  height: 46px;
  ${(props) =>
    props.theme.interfaceDirection === "rtl"
      ? css`
          margin-left: 12px;
        `
      : css`
          margin-right: 12px;
        `}
  border-radius: 8px;
  cursor: pointer;

  .check-hover {
    visibility: hidden;
  }

  &:hover {
    .check-hover {
      ${(props) =>
        props.theme.interfaceDirection === "rtl"
          ? css`
              padding: 16px 15px 0 0;
            `
          : css`
              padding: 16px 0 0 15px;
            `}
      visibility: visible;
      opacity: 0.5;
      svg path {
        fill: ${(props) => props.colorCheckImgHover};
      }
    }
  }
`;
export { StyledComponent, StyledTheme };
