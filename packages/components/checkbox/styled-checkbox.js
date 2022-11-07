import styled, { css } from "styled-components";
import Base from "../themes/base";

const StyledLabel = styled.label`
  display: flex;
  align-items: center;
  position: relative;
  margin: 0;

  user-select: none;
  -o-user-select: none;
  -moz-user-select: none;
  -webkit-user-select: none;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  .checkbox {
    margin-right: 12px;
    overflow: visible;
    outline: none;
  }

  /* ${(props) =>
    props.isDisabled
      ? css`
          cursor: not-allowed;
        `
      : css`
          cursor: pointer;

          &:hover {
            svg {
              rect:first-child {
                stroke: ${(props) => props.theme.text.hoverColor};
              }
            }
          }
        `} */

  svg {
    ${(props) =>
      !props.isIndeterminate && !props.isDisabled
        ? css`
            rect {
              fill: ${(props) => props.theme.checkbox.fillColor};
              stroke: ${(props) => props.theme.checkbox.borderColor};
            }
            path {
              fill: ${(props) => props.theme.checkbox.arrowColor};
            }
            &:focus {
              outline: none;
              rect {
                stroke: ${(props) => props.theme.checkbox.focusColor};
              }
            }
          `
        : !props.isDisabled &&
          css`
            rect {
              fill: ${(props) => props.theme.checkbox.fillColor};
              stroke: ${(props) => props.theme.checkbox.borderColor};
            }
            }
            rect:last-child {
              fill: ${(props) => props.theme.checkbox.indeterminateColor};
              stroke: ${(props) => props.theme.checkbox.fillColor};
            }
          `}

    ${(props) =>
      props.isDisabled && !props.isIndeterminate
        ? css`
            rect {
              fill: ${(props) => props.theme.checkbox.disableFillColor};
              stroke: ${(props) => props.theme.checkbox.disableBorderColor};
            }
            path {
              fill: ${(props) => props.theme.checkbox.disableArrowColor};
            }
          `
        : props.isDisabled &&
          css`
            rect {
              fill: ${(props) => props.theme.checkbox.disableFillColor};
              stroke: ${(props) => props.theme.checkbox.disableBorderColor};
            }
            rect:last-child {
              fill: ${(props) =>
                props.theme.checkbox.disableIndeterminateColor};
            }
          `}
  }
  &:hover {
    ${(props) =>
      props.isDisabled
        ? css`
            cursor: not-allowed;
          `
        : !props.isIndeterminate
        ? css`
            cursor: pointer;

            rect:nth-child(2) {
              stroke: ${(props) => props.theme.checkbox.hoverBorderColor};
            }
          `
        : css`
          cursor: pointer;
          rect:nth-child(2) {
              stroke: ${(props) => props.theme.checkbox.hoverBorderColor};
            }
          rect:last-child {
              fill: ${(props) => props.theme.checkbox.hoverIndeterminateColor};
            `}
  }

  .wrapper {
    display: inline-block;
  }

  .checkbox-text {
    color: ${(props) =>
      props.isDisabled
        ? props.theme.text.disableColor
        : props.theme.text.color};
    margin-top: -2px;
  }

  .help-button {
    display: inline-block;
    margin-left: 4px;
  }
`;
StyledLabel.defaultProps = { theme: Base };

const HiddenInput = styled.input`
  opacity: 0.0001;
  position: absolute;
  right: 0;
  z-index: -1;
`;

export { StyledLabel, HiddenInput };
