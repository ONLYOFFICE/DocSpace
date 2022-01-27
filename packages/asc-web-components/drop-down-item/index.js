import React from "react";
import PropTypes from "prop-types";
import { ReactSVG } from "react-svg";

import { StyledDropdownItem, IconWrapper } from "./styled-drop-down-item";

const DropDownItem = (props) => {
  //console.log("DropDownItem render");
  const {
    isSeparator,
    label,
    icon,
    children,
    disabled,
    onClick,
    className,
    theme,
  } = props;

  const onClickAction = (e) => {
    onClick && !disabled && onClick(e);
  };

  return (
    <StyledDropdownItem
      {...props}
      className={className}
      onClick={onClickAction}
      disabled={disabled}
    >
      {icon && (
        <IconWrapper>
          <ReactSVG src={icon} className="drop-down-item_icon" />
        </IconWrapper>
      )}
      {isSeparator ? "\u00A0" : label ? label : children && children}
    </StyledDropdownItem>
  );
};

DropDownItem.propTypes = {
  /** Tells when the dropdown item should display like separator */
  isSeparator: PropTypes.bool,
  /** Tells when the dropdown item should display like header */
  isHeader: PropTypes.bool,
  /** Accepts tab-index */
  tabIndex: PropTypes.number,
  /** Dropdown item text */
  label: PropTypes.string,
  /** Tells when the dropdown item should display like disabled */
  disabled: PropTypes.bool,
  /** Dropdown item icon */
  icon: PropTypes.string,
  /** Disable default style hover effect */
  noHover: PropTypes.bool,
  /** What the dropdown item will trigger when clicked */
  onClick: PropTypes.func,
  /** Children elements */
  children: PropTypes.any,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Accepts css text-overflow */
  textOverflow: PropTypes.bool,
};

DropDownItem.defaultProps = {
  isSeparator: false,
  isHeader: false,
  tabIndex: -1,
  label: "",
  disabled: false,
  noHover: false,
  textOverflow: false,
};

export default DropDownItem;
