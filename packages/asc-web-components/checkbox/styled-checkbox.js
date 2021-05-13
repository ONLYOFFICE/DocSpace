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
      !props.isIndeterminate
        ? css`
            rect {
              fill: ${(props) =>
                props.color === "#FFFF"
                  ? props.theme.checkbox.fillColor
                  : props.color};
              stroke: ${(props) =>
                props.color === "#FFFF"
                  ? props.theme.checkbox.borderColor
                  : props.color};
            }
            path {
              fill: ${(props) =>
                props.color
                  ? props.color === "#FFFF"
                    ? props.theme.checkbox.arrowColor
                    : "white"
                  : props.theme.checkbox.arrowColor};
            }
          `
        : css`
            rect {
              fill: ${(props) =>
                props.color === "#FFFF"
                  ? props.theme.checkbox.fillColor
                  : props.color};
                  stroke: ${(props) =>
                    props.color === "#FFFF"
                      ? props.theme.checkbox.borderColor
                      : props.color};
            }
            }
            rect:last-child {
              fill: ${(props) =>
                props.color
                  ? props.color === "#FFFF"
                    ? props.theme.checkbox.indeterminateColor
                    : "white"
                  : props.theme.checkbox.indeterminateColor};
              stroke: ${(props) =>
                props.color === "#FFFF"
                  ? props.theme.checkbox.fillColor
                  : "none"};
            }
          `}

    ${(props) =>
      props.isDisabled && !props.isIndeterminate
        ? css`
            filter: ${(props) =>
              props.color !== "#FFFF" ? "opacity(30%)" : "opacity(100%)"};
            rect {
              fill: ${(props) =>
                props.color === "#FFFF"
                  ? props.theme.checkbox.disableFillColor
                  : props.color};
              stroke: ${(props) =>
                props.color === "#FFFF"
                  ? props.theme.checkbox.disableBorderColor
                  : props.color};
            }
            path {
              fill: ${(props) => props.theme.checkbox.disableArrowColor};
            }
          `
        : props.isDisabled &&
          css`
            filter: ${(props) =>
              props.color !== "#FFFF" ? "opacity(30%)" : "opacity(100%)"};
            rect:last-child {
              fill: ${(props) =>
                props.color === "#FFFF"
                  ? props.theme.checkbox.disableIndeterminateColor
                  : "rgba(255,255,255,0.7)"};
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
              stroke: ${(props) =>
                props.color === "#FFFF"
                  ? props.theme.checkbox.hoverBorderColor
                  : "rgba(0,0,0,0.2)"};
            }
          `
        : css`
          cursor: pointer;
           rect:nth-child(2) {
              stroke: ${(props) =>
                props.color === "#FFFF"
                  ? props.theme.checkbox.hoverBorderColor
                  : "rgba(0,0,0,0.2)"};
            }
          rect:last-child {
              fill: ${(props) =>
                props.color
                  ? props.isIndeterminate && props.color === "#FFFF"
                    ? props.theme.checkbox.hoverIndeterminateColor
                    : props.isIndeterminate
                    ? "white"
                    : props.color
                  : props.theme.checkbox.hoverIndeterminateColor};
            `}
  }

  .checkbox-text {
    color: ${(props) =>
      props.isDisabled
        ? props.theme.text.disableColor
        : props.theme.text.color};
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
