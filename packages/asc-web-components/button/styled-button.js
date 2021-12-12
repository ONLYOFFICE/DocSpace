import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import NoUserSelect from "../utils/commonStyles";
import Base from "../themes/base";

const activeCss = css`
  background-color: ${(props) =>
    props.primary
      ? props.theme.button.backgroundColor.primaryActive
      : props.theme.button.backgroundColor.baseActive};

  color: ${(props) =>
    props.primary
      ? props.theme.button.color.primaryActive
      : props.theme.button.color.baseActive};

  ${(props) =>
    !props.primary
      ? css`
          border: ${(props) => props.theme.button.border.baseActive};
          box-sizing: ${(props) => props.theme.button.boxSizing};
        `
      : css`
          border: ${(props) => props.theme.button.border.primaryActive};
          box-sizing: ${(props) => props.theme.button.boxSizing};
        `}
`;

const hoverCss = css`
  background-color: ${(props) =>
    props.primary
      ? props.theme.button.backgroundColor.primaryHover
      : props.theme.button.backgroundColor.baseHover};

  color: ${(props) =>
    props.primary
      ? props.theme.button.color.primaryHover
      : props.theme.button.color.baseHover};

  ${(props) =>
    !props.primary
      ? css`
          border: ${(props) => props.theme.button.border.baseHover};
          box-sizing: ${(props) => props.theme.button.boxSizing};
        `
      : css`
          border: ${(props) => props.theme.button.border.primaryHover};
          box-sizing: ${(props) => props.theme.button.boxSizing};
        `}
`;

const disableCss = css`
  background-color: ${(props) =>
    props.primary
      ? props.theme.button.backgroundColor.primaryDisabled
      : props.theme.button.backgroundColor.baseDisabled};

  color: ${(props) =>
    props.primary
      ? props.theme.button.color.primaryDisabled
      : props.theme.button.color.baseDisabled};

  ${(props) =>
    !props.primary
      ? css`
          border: ${(props) => props.theme.button.border.baseDisabled};
          box-sizing: ${(props) => props.theme.button.boxSizing};
        `
      : css`
          border: ${(props) => props.theme.button.border.primaryDisabled};
          box-sizing: ${(props) => props.theme.button.boxSizing};
        `}
`;

const heightStyle = (props) => props.theme.button.height[props.size];
const lineHeightStyle = (props) => props.theme.button.lineHeight[props.size];
const fontSizeStyle = (props) => props.theme.button.fontSize[props.size];

const ButtonWrapper = ({
  primary,
  scale,
  size,
  isHovered,
  isClicked,
  isDisabled,
  disableHover,
  isLoading,
  label,
  innerRef,
  minWidth,
  ...props
}) => {
  return <button ref={innerRef} type="button" {...props}></button>;
};

ButtonWrapper.propTypes = {
  label: PropTypes.string,
  primary: PropTypes.bool,
  size: PropTypes.oneOf(["base", "medium", "big", "large"]),
  scale: PropTypes.bool,
  icon: PropTypes.node,

  tabIndex: PropTypes.number,

  isHovered: PropTypes.bool,
  isClicked: PropTypes.bool,
  isDisabled: PropTypes.bool,
  disableHover: PropTypes.bool,
  isLoading: PropTypes.bool,

  onClick: PropTypes.func,
};
const StyledButton = styled(ButtonWrapper).attrs((props) => ({
  disabled: props.isDisabled || props.isLoading ? "disabled" : "",
  tabIndex: props.tabIndex,
}))`
  height: ${(props) => heightStyle(props)};
  line-height: ${(props) => lineHeightStyle(props)};
  font-size: ${(props) => fontSizeStyle(props)};

  color: ${(props) =>
    !props.primary
      ? props.theme.button.color.base
      : props.theme.button.color.primary};

  background-color: ${(props) =>
    props.primary
      ? props.theme.button.backgroundColor.primary
      : props.theme.button.backgroundColor.base};

  border: ${(props) =>
    props.primary
      ? props.theme.button.border.primary
      : props.theme.button.border.base};

  ${(props) => props.scale && `width: 100%;`}

  padding: ${(props) =>
    (props.size === "large" &&
      (props.primary
        ? props.icon
          ? props.label
            ? "11px 24px 13px 24px"
            : "11px 11px 13px 11px"
          : props.label
          ? "12px 20px 12px 20px"
          : "0px"
        : props.icon
        ? props.label
          ? "10px 24px 13px 24px"
          : "10px 11px 13px 11px"
        : props.label
        ? "11px 20px 12px 20px"
        : "0px")) ||
    (props.size === "big" &&
      (props.primary
        ? props.icon
          ? props.label
            ? "8px 24px 9px 24px"
            : "8px 10px 9px 10px"
          : props.label
          ? "8px 16px 8px 16px"
          : "0px"
        : props.icon
        ? props.label
          ? "7px 24px 9px 24px"
          : "7px 10px 9px 10px"
        : props.label
        ? "7px 16px 8px 16px"
        : "0px")) ||
    (props.size === "medium" &&
      (props.primary
        ? props.icon
          ? props.label
            ? "6px 24px 7px 24px"
            : "6px 10px 7px 10px"
          : props.label
          ? "7px 16px 7px 16px"
          : "0px"
        : props.icon
        ? props.label
          ? "5px 24px 7px 24px"
          : "5px 10px 7px 10px"
        : props.label
        ? "6px 16px 7px 16px"
        : "0px")) ||
    (props.size === "base" &&
      (props.primary
        ? props.icon
          ? props.label
            ? "3px 20px 5px 20px"
            : "3px 5px 5px 5px"
          : props.label
          ? "4px 12px 5px 12px"
          : "0px"
        : props.icon
        ? props.label
          ? "2px 20px 5px 20px"
          : "2px 5px 5px 5px"
        : props.label
        ? "3px 12px 5px 12px"
        : "0px"))};

  ${(props) => (props.minwidth ? `min-width: ${props.minwidth};` : null)}

  cursor: ${(props) =>
    props.isDisabled || props.isLoading ? "default !important" : "pointer"};

  font-family: ${(props) => props.theme.fontFamily};
  margin: ${(props) => props.theme.margin};
  display: ${(props) => props.theme.button.display};
  font-weight: ${(props) => props.theme.button.fontWeight};
  text-align: ${(props) => props.theme.button.textAlign};
  text-decoration: ${(props) => props.theme.button.textDecoration};
  vertical-align: ${(props) => props.theme.button.verticalAlign};
  border-radius: ${(props) => props.theme.button.borderRadius};
  -moz-border-radius: ${(props) => props.theme.button.borderRadius};
  -webkit-border-radius: ${(props) => props.theme.button.borderRadius};

  ${NoUserSelect};

  stroke: ${(props) => props.theme.button.stroke};
  overflow: ${(props) => props.theme.button.overflow};
  text-overflow: ${(props) => props.theme.button.textOverflow};
  white-space: ${(props) => props.theme.button.whiteSpace};
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  ${(props) =>
    !props.isDisabled &&
    !props.isLoading &&
    (props.isHovered
      ? hoverCss
      : !props.disableHover &&
        css`
          &:hover {
            ${hoverCss}
          }
        `)}

  ${(props) =>
    !props.isDisabled &&
    !props.isLoading &&
    (props.isClicked
      ? activeCss
      : css`
          &:active {
            ${activeCss}
          }
        `)}

  ${(props) => props.isDisabled && disableCss}


    &:focus {
    outline: ${(props) => props.theme.button.outline};
  }

  .btnIcon,
  .loader {
    display: ${(props) => props.theme.button.display};
    vertical-align: ${(props) => props.theme.button.topVerticalAlign};
  }

  .loader {
    svg {
      stroke: ${(props) =>
        props.primary
          ? props.theme.button.loader.primary
          : props.theme.button.loader.base};
    }
    vertical-align: ${(props) =>
      props.size === "large" || props.size === "base"
        ? props.theme.button.middleVerticalAlign
        : props.size === "medium"
        ? props.theme.button.bottomVerticalAlign
        : props.theme.button.topVerticalAlign};
  }

  ${(props) =>
    props.label &&
    css`
      .btnIcon,
      .loader {
        padding-right: ${(props) => props.theme.button.paddingRight};
      }
    `}
`;

StyledButton.defaultProps = { theme: Base };

export default StyledButton;
