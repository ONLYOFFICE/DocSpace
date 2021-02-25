import React from "react";
import PropTypes from "prop-types";
import Loader from "../loader";
import StyledButton from "./styled-button";
import Base from "../themes/base";

// eslint-disable-next-line no-unused-vars, react/prop-types

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
  disableHover: PropTypes.bool,
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
