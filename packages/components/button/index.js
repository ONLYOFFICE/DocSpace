import React from "react";
import PropTypes from "prop-types";
import Loader from "../loader";
import StyledButton from "./styled-button";
import Base from "../themes/base";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

// eslint-disable-next-line no-unused-vars, react/prop-types
const Button = React.forwardRef((props, ref) => {
  const { primary, size, isLoading, icon, label } = props;
  return (
    <ColorTheme {...props} innerRef={ref} elementType={ThemeType.Button}>
      <div className="button-content">
        {(isLoading || icon) &&
          (isLoading ? (
            <Loader
              type="oval"
              size={
                size === "medium" ? "20px" : size === "normal" ? "16px" : "12px"
              }
              color={primary ? "#FFFFFF" : "#333333"}
              className="loader"
            />
          ) : (
            <div className="icon">{icon}</div>
          ))}
        {label}
      </div>
    </ColorTheme>
  );
});

Button.propTypes = {
  /** Button text */
  label: PropTypes.string,
  /** Tells when the button should be primary */
  primary: PropTypes.bool,
  /** Size of button.     

   The normal size equals 36px and 40px in height on the Desktop and Touchcreen devices. */
  size: PropTypes.oneOf([
    "extraSmall",
    "small",
    "normal",
    "medium",
    "normalDesktop",
    "normalTouchscreen",
  ]),
  /** Scale width of button to 100% */
  scale: PropTypes.bool,
  /** Icon node element */
  icon: PropTypes.node,
  /** Button tab index */
  tabIndex: PropTypes.number,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts CSS style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Tells when the button should present a hovered state */
  isHovered: PropTypes.bool,
  /** Disable hover effect */
  disableHover: PropTypes.bool,
  /** Tells when the button should present a clicked state */
  isClicked: PropTypes.bool,
  /** Tells when the button should present a disabled state */
  isDisabled: PropTypes.bool,
  /** Tells when the button should show loader icon */
  isLoading: PropTypes.bool,
  /** Sets the nim width of the button */
  minwidth: PropTypes.string,
  /** What the button will trigger when clicked */
  onClick: PropTypes.func,
};

Button.defaultProps = {
  label: "",
  primary: false,
  size: "extraSmall",
  theme: Base,
  scale: false,
  icon: null,
  tabIndex: -1,
  minwidth: null,
  isHovered: false,
  disableHover: false,
  isClicked: false,
  isDisabled: false,
  isLoading: false,
};

Button.displayName = "Button";
export default Button;
