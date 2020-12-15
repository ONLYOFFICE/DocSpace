import React from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import Loader from "../loader";

const activeCss = css`
  background-color: ${(props) => (props.primary ? "#1F97CA" : "#ECEEF1")};
  color: ${(props) => (props.primary ? "#ffffff" : "#333333")};

  ${(props) =>
    !props.primary &&
    css`
      border: 1px solid #2da7db;
      box-sizing: border-box;
    `}
`;

const hoverCss = css`
  background-color: ${(props) => (props.primary ? "#3DB8EC" : "#FFFFFF")};
  color: ${(props) => (props.primary ? "#ffffff" : "#333333")};

  ${(props) =>
    !props.primary &&
    css`
      border: 1px solid #2da7db;
      box-sizing: border-box;
    `}
`;

// eslint-disable-next-line no-unused-vars, react/prop-types
const ButtonWrapper = ({
  primary,
  scale,
  size,
  isHovered,
  isClicked,
  isDisabled,
  isLoading,
  label,
  innerRef,
  minWidth,
  ...props
}) => <button ref={innerRef} type="button" {...props}></button>;

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
  isLoading: PropTypes.bool,

  onClick: PropTypes.func,
};

const StyledButton = styled(ButtonWrapper).attrs((props) => ({
  disabled: props.isDisabled || props.isLoading ? "disabled" : "",
  tabIndex: props.tabIndex,
}))`
  height: ${(props) =>
    (props.size === "large" && "44px") ||
    (props.size === "big" && "36px") ||
    (props.size === "medium" && "32px") ||
    (props.size === "base" && "24px")};

  line-height: ${(props) =>
    (props.size === "large" && "20px") ||
    (props.size === "big" && "20px") ||
    (props.size === "medium" && "18px") ||
    (props.size === "base" && "15px")};

  font-size: ${(props) =>
    (props.size === "large" && "16px") ||
    (props.size === "big" && "14px") ||
    (props.size === "medium" && "13px") ||
    (props.size === "base" && "12px")};

  color: ${(props) =>
    (props.primary && "#FFFFFF") ||
    (!props.isDisabled ? "#333333" : "#ECEEF1")};

  background-color: ${(props) =>
    !props.isDisabled || props.isLoading
      ? props.primary
        ? "#2DA7DB"
        : "#FFFFFF"
      : props.primary
      ? "#A6DCF2"
      : "#FFFFFF"};

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

  font-family: "Open Sans", normal;
  border: none;
  margin: 0;
  display: inline-block;
  font-weight: 600;
  text-align: center;
  text-decoration: none;
  vertical-align: middle;
  border-radius: 3px;
  -moz-border-radius: 3px;
  -webkit-border-radius: 3px;
  touch-callout: none;
  -o-touch-callout: none;
  -moz-touch-callout: none;
  -webkit-touch-callout: none;
  user-select: none;
  -o-user-select: none;
  -moz-user-select: none;
  -webkit-user-select: none;
  stroke: none;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  ${(props) =>
    !props.primary &&
    css`
      border: 1px solid;
      box-sizing: border-box;
      border-color: ${(props) =>
        !props.isDisabled && !props.isLoading ? "#D0D5DA" : "#ECEEF1"};
    `}

  ${(props) =>
    !props.isDisabled &&
    !props.isLoading &&
    (props.isHovered
      ? hoverCss
      : css`
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
    outline: none;
  }

  .btnIcon,
  .loader {
    display: inline-block;
    vertical-align: text-top;
  }

  .loader {
    vertical-align: ${(props) =>
      props.size === "large" || props.size === "base"
        ? "middle"
        : props.size === "medium"
        ? "text-bottom"
        : "text-top"};
  }

  ${(props) =>
    props.label &&
    css`
      .btnIcon,
      .loader {
        padding-right: 4px;
      }
    `}
`;

const Icon = ({ size, primary, icon, isHovered }) => (
  <div className="btnIcon">
    {icon &&
      React.cloneElement(icon, {
        isfill: true,
        size: size === "large" ? "large" : size === "big" ? "medium" : "small",
        color: icon.props.color
          ? isHovered
            ? icon.props.hoveredcolor
            : icon.props.color
          : primary
          ? "#FFFFFF"
          : "#333333",
      })}
  </div>
);

Icon.propTypes = {
  icon: PropTypes.node,
  size: PropTypes.string,
  primary: PropTypes.bool,
};

Icon.defaultProps = {
  icon: null,
};

const Button = React.forwardRef((props, ref) => {
  const { primary, size, isLoading, icon, label, isHovered } = props;
  const iconProps = { primary, size, icon, isHovered };
  return (
    <StyledButton innerRef={ref} {...props}>
      {isLoading || icon ? (
        isLoading ? (
          <Loader
            type="oval"
            size={size === "large" ? "18px" : size === "big" ? "16px" : "14px"}
            color={primary ? "#FFFFFF" : "#333333"}
            className="loader"
          />
        ) : (
          <Icon {...iconProps} />
        )
      ) : (
        ""
      )}
      {label}
    </StyledButton>
  );
});

Button.propTypes = {
  label: PropTypes.string,
  primary: PropTypes.bool,
  size: PropTypes.oneOf(["base", "medium", "big", "large"]),
  scale: PropTypes.bool,
  icon: PropTypes.node,

  tabIndex: PropTypes.number,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),

  isHovered: PropTypes.bool,
  isClicked: PropTypes.bool,
  isDisabled: PropTypes.bool,
  isLoading: PropTypes.bool,

  minwidth: PropTypes.string,

  onClick: PropTypes.func,
};

Button.defaultProps = {
  label: "",
  primary: false,
  size: "base",
  scale: false,
  icon: null,

  tabIndex: -1,

  minwidth: null,

  isHovered: false,
  isClicked: false,
  isDisabled: false,
  isLoading: false,
};

Button.displayName = "Button";

export default Button;
