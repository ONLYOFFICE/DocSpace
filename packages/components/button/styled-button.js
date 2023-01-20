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

  .btnIcon {
    svg {
      path {
        fill: ${(props) =>
          props.primary
            ? props.theme.button.color.primaryActive
            : props.theme.button.color.baseActive};
      }
    }
  }
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

  .btnIcon {
    svg {
      path {
        fill: ${(props) =>
          props.primary
            ? props.theme.button.color.primaryHover
            : props.theme.button.color.baseHover};
      }
    }
  }
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

  .btnIcon {
    svg {
      path {
        fill: ${(props) =>
          props.primary
            ? props.theme.button.color.primaryDisabled
            : props.theme.button.color.baseDisabled};
      }
    }
  }
`;

const heightStyle = (props) => props.theme.button.height[props.size];
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

const StyledButton = styled(ButtonWrapper).attrs((props) => ({
  disabled: props.isDisabled || props.isLoading ? "disabled" : "",
  tabIndex: props.tabIndex,
}))`
  position: relative;
  height: ${(props) => heightStyle(props)};
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

  ${(props) => props.scale && `width: 100%;`};
  min-width: ${(props) => props.minwidth && props.minwidth};

  padding: ${(props) => `${props.theme.button.padding[props.size]}`};

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

  ${(props) => (props.isDisabled || props.isLoading) && disableCss}

  &:focus {
    outline: ${(props) => props.theme.button.outline};
  }

  .loader {
    display: flex;
    justify-content: center;
    align-items: center;

    position: absolute;

    top: 0;
    left: 0;
    right: 0;
    bottom: 0;

    width: 100%;
    height: 100%;

    background-color: transparent;
  }

  .button-content {
    width: 100%;
    height: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
    gap: 8px;
    visibility: ${(props) => (props.isLoading ? "hidden" : "visible")};
  }

  .icon {
    width: auto;
    display: flex;
    align-items: center;
  }
`;

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

StyledButton.defaultProps = { theme: Base };

export default StyledButton;
