import styled, { css } from "styled-components";
import Base from "../themes/base";

const StyledSwitchButton = styled.label`
  width: 64px;
  height: 32px;
  position: relative;
  box-sizing: border-box;
  svg {
    rect {
      fill: ${(props) => props.theme.switchButton.fillColor};
    }
    ${(props) =>
      !props.disabled
        ? !props.checked
          ? css`
              path:nth-child(2) {
                fill: ${(props) => props.theme.switchButton.checkedFillColor};
              }
              path:nth-child(3) {
                fill: ${(props) => props.theme.switchButton.fillColor};
              }
              path:last-child {
                fill: ${(props) => props.theme.switchButton.checkedFillColor};
              }
            `
          : css`
              path:nth-child(2) {
                fill: ${(props) => props.theme.switchButton.checkedFillColor};
              }
              path:nth-child(3) {
                fill: ${(props) => props.theme.switchButton.checkedFillColor};
              }
              path:last-child {
                fill: ${(props) => props.theme.switchButton.fillColor};
              }
            `
        : css`
            rect {
              fill: ${(props) => props.theme.switchButton.fillColorDisabled};
              stroke: ${(props) => props.theme.switchButton.disabledFillColor};
            }
            path:nth-child(2) {
              fill: ${(props) => props.theme.switchButton.disabledFillColor};
            }
            path:nth-child(3) {
              fill: ${(props) =>
                props.theme.switchButton.disabledFillColorInner};
            }
            path:last-child {
              fill: ${(props) =>
                props.theme.switchButton.disabledFillColorInner};
            }
          `}
  }
  &:hover {
    ${(props) =>
      props.disabled
        ? css`
            cursor: default;
          `
        : css`
            cursor: pointer;
            rect:first-child {
              stroke: ${(props) => props.theme.switchButton.hoverBorderColor};
            }
          `}
  }
`;

const StyledHiddenInput = styled.input`
  opacity: 0.0001;
  position: absolute;
  right: 0;
  z-index: -1;
`;

StyledSwitchButton.defaultProps = { theme: Base };

export { StyledSwitchButton, StyledHiddenInput };
