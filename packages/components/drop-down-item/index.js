import React from "react";
import PropTypes from "prop-types";
import { ReactSVG } from "react-svg";

import { StyledDropdownItem, IconWrapper } from "./styled-drop-down-item";
import { isNull } from "lodash";

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
    fillIcon,
    isSubMenu,
    isActive,
    withoutIcon,
    noHover,
    height,
  } = props;

  const onClickAction = (e) => {
    onClick && !disabled && onClick(e);
  };

  return (
    <StyledDropdownItem
      {...props}
      noHover={noHover}
      className={className}
      onClick={onClickAction}
      disabled={disabled}
      isActive={isActive}
    >
      {icon && (
        <IconWrapper>
          {!withoutIcon ? (
            icon.includes("https://") || icon.includes("http://") ? (
              <img src={icon} />
            ) : (
              <ReactSVG
                src={icon}
                className={fillIcon ? "drop-down-item_icon" : ""}
              />
            )
          ) : null}
        </IconWrapper>
      )}

      {isSeparator ? "\u00A0" : label ? label : children && children}

      {isSubMenu && (
        <IconWrapper className="submenu-arrow">
          <ReactSVG
            src="/static/images/right.arrow.react.svg"
            className="drop-down-item_icon"
          />
        </IconWrapper>
      )}
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
  customHeight: PropTypes.number,
  customHeightTablet: PropTypes.number,
  textOverflow: PropTypes.bool,
  fillIcon: PropTypes.bool,
  isSubMenu: PropTypes.bool,
  isActive: PropTypes.bool,
  withoutIcon: PropTypes.bool,
};

DropDownItem.defaultProps = {
  isSeparator: false,
  isHeader: false,
  tabIndex: -1,
  label: "",
  disabled: false,
  noHover: false,
  textOverflow: false,
  fillIcon: true,
  isSubMenu: false,
  isActive: false,
  withoutIcon: false,
  height: 32,
  heightTablet: 36,
};

export default DropDownItem;
