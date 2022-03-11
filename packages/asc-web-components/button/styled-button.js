import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import NoUserSelect from "../utils/commonStyles";
import Base from "../themes/base";
import { isDesktop } from "../utils/device";

const activeCss = css`
  background-color: ${(props) =>
    props.primary
      ? props.theme.button.backgroundColor.primaryActive
      : props.theme.button.backgroundColor.baseActive};

  color: ${(props) =>
    props.primary
      ? props.theme.button.color.primary
      : props.theme.button.color.base};

  ${(props) =>
    !props.primary &&
    css`
      border: ${(props) => props.theme.button.border.baseActive};
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
      ? props.theme.button.color.primary
      : props.theme.button.color.base};

  ${(props) =>
    !props.primary &&
    css`
      border: ${(props) => props.theme.button.border.baseHover};
      box-sizing: ${(props) => props.theme.button.boxSizing};
    `}
`;

const heightStyle = (props) => props.theme.button.height[props.size];
const fontSizeStyle = (props) => props.theme.button.fontSize[props.size];

const ButtonWrapper = ({ innerRef, ...props }) => {
  return <button ref={innerRef} type="button" {...props}></button>;
};

ButtonWrapper.propTypes = {
  label: PropTypes.string,
  primary: PropTypes.bool,
  size: PropTypes.oneOf(["extraSmall", "small", "normal", "medium"]),
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
  size:
    props.size === "normal"
      ? isDesktop()
        ? "normalDesktop"
        : "normalTouchscreen"
      : props.size,
}))`
  height: ${(props) => heightStyle(props)};
  font-size: ${(props) => fontSizeStyle(props)};

  color: ${(props) =>
    (props.primary && props.theme.button.color.primary) ||
    (!props.isDisabled
      ? props.theme.button.color.base
      : props.theme.button.color.disabled)};

  background-color: ${(props) =>
    !props.isDisabled || props.isLoading
      ? props.primary
        ? props.theme.button.backgroundColor.primary
        : props.theme.button.backgroundColor.base
      : props.primary
      ? props.theme.button.backgroundColor.primaryDisabled
      : props.theme.button.backgroundColor.base};

  ${(props) => props.scale && `width: 100%;`}

  padding: ${(props) => `${props.theme.button.padding[props.size]}`};

  ${(props) => (props.minwidth ? `min-width: ${props.minwidth};` : null)}

  cursor: ${(props) =>
    props.isDisabled || props.isLoading ? "default !important" : "pointer"};

  font-family: ${(props) => props.theme.fontFamily};
  border: none;
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
    !props.primary &&
    css`
      box-sizing: ${(props) => props.theme.button.boxSizing};
      border: ${(props) =>
        !props.isDisabled && !props.isLoading
          ? props.theme.button.border.base
          : props.theme.button.border.baseDisabled};
    `}

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
  
    &:focus {
    outline: ${(props) => props.theme.button.outline};
  }

  .button-content {
    display: flex;
    justify-content: center;
    align-items: center;
    gap: 4px;
  }

  .btnIcon,
  .loader {
    display: flex;
    justify-content: center;
    align-items: center;
  }

  .loader {
    vertical-align: ${(props) =>
      props.size === "normalTouchscreen" || props.size === "extraSmall"
        ? props.theme.button.middleVerticalAlign
        : props.size === "small"
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
