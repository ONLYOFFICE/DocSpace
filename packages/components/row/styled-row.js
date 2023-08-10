import styled, { css } from "styled-components";

import { tablet } from "../utils/device";
import Base from "../themes/base";
import { isMobile } from "react-device-detect";
import { getCorrectFourValuesStyle } from "../utils/rtlUtils";

const StyledRow = styled.div`
  cursor: default;
  position: relative;
  min-height: ${(props) => props.theme.row.minHeight};
  width: ${(props) => props.theme.row.width};
  border-bottom: ${(props) =>
    props.withoutBorder ? "none" : "2px solid transparent"};

  ${(props) =>
    !props.withoutBorder &&
    css`
      ::after {
        position: absolute;
        display: block;
        bottom: 0px;
        width: 100%;
        height: 1px;
        background-color: ${(props) => props.theme.row.borderBottom};
        content: "";
      }
    `}

  display: flex;
  flex-direction: row;
  flex-wrap: nowrap;

  justify-content: flex-start;
  align-items: center;
  align-content: center;

  .row-loader {
    ${({ theme }) =>
      theme.interfaceDirection === "rtl"
        ? `margin-right: 9px;`
        : `margin-left: 9px;`}
    padding: 0;
    display: flex;
    align-items: center;
    justify-items: center;
    min-width: 32px;
  }

  ${(props) =>
    props.mode == "modern" &&
    css`
      .checkbox {
        display: ${(props) => (props.checked ? "flex" : "none")};

        padding: ${getCorrectFourValuesStyle(
          "10px 0px 10px 8px",
          props.theme.interfaceDirection
        )};
        ${props.theme.interfaceDirection === "rtl"
          ? `margin-right: -4px;`
          : `margin-left: -4px;`}
      }

      .styled-element {
        display: ${(props) => (props.checked ? "none" : "flex")};
      }
    `}
`;
StyledRow.defaultProps = { theme: Base };

const StyledContent = styled.div`
  display: flex;
  flex-basis: 100%;

  min-width: ${(props) => props.theme.row.minWidth};

  @media ${tablet} {
    white-space: nowrap;
    overflow: ${(props) => props.theme.row.overflow};
    text-overflow: ${(props) => props.theme.row.textOverflow};
    height: ${(props) => props.theme.rowContent.height};
  }
`;
StyledContent.defaultProps = { theme: Base };

const StyledCheckbox = styled.div`
  display: flex;
  flex: 0 0 16px;
  height: 56px;
  max-height: 56px;
  justify-content: center;
  align-items: center;

  min-width: 41px;
  width: 41px;
  ${(props) =>
    props.mode == "modern" &&
    !isMobile &&
    css`
      :hover {
        .checkbox {
          display: flex;
          opacity: 1;
          user-select: none;
        }
        .styled-element {
          display: none;
        }
      }
    `}
`;

const StyledElement = styled.div`
  flex: 0 0 auto;
  display: flex;
  ${({ theme }) =>
    theme.interfaceDirection === "rtl"
      ? `
      margin-left: ${theme.row.element.marginRight};
      margin-right: ${theme.row.element.marginLeft};
        `
      : `
      margin-right: ${theme.row.element.marginRight};
      margin-left: ${theme.row.element.marginLeft};
        `}
  user-select: none;

  .react-svg-icon svg {
    margin-top: 4px;
  }
  /* .react-svg-icon.is-edit svg {
    margin: 4px 0 0 28px;
  } */
`;
StyledElement.defaultProps = { theme: Base };

const StyledContentElement = styled.div`
  margin-top: 6px;
  user-select: none;
`;

const StyledOptionButton = styled.div`
  display: flex;
  width: ${(props) => props.spacerWidth && props.spacerWidth};
  justify-content: flex-end;

  .expandButton > div:first-child {
    padding: ${({ theme }) =>
      getCorrectFourValuesStyle(
        theme.row.optionButton.padding,
        theme.interfaceDirection
      )};

    ${({ theme }) =>
      theme.interfaceDirection === "rtl"
        ? `margin-left: 0px;`
        : `margin-right: 0px;`}

    @media (min-width: 1024px) {
      ${({ theme }) =>
        theme.interfaceDirection === "rtl"
          ? `margin-left: -1px;`
          : `margin-right: -1px;`}
    }
    @media (max-width: 516px) {
      ${({ theme }) =>
        theme.interfaceDirection === "rtl"
          ? `padding-right: 10px;`
          : `padding-left: 10px;`}
    }
  }

  //margin-top: -1px;
  @media ${tablet} {
    margin-top: unset;
  }
`;
StyledOptionButton.defaultProps = { theme: Base };

export {
  StyledOptionButton,
  StyledContentElement,
  StyledElement,
  StyledCheckbox,
  StyledContent,
  StyledRow,
};
