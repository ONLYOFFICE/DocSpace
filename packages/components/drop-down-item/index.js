import React from "react";
import PropTypes from "prop-types";
import { ReactSVG } from "react-svg";

import RightArrowReactSvgUrl from "PUBLIC_DIR/images/right.arrow.react.svg?url";

import {
  StyledDropdownItem,
  IconWrapper,
  WrapperToggle,
} from "./styled-drop-down-item";
import ToggleButton from "../toggle-button";

const DropDownItem = (props) => {
  //console.log("DropDownItem render");
  const {
    isSeparator,
    label,
    icon,
    children,
    disabled,
    className,
    theme,
    fillIcon,
    isSubMenu,
    isActive,
    withoutIcon,
    noHover,
    height,
    isSelected,
    isActiveDescendant,
  } = props;

  const { withToggle, checked, onClick, ...rest } = props;

  const onClickAction = (e) => {
    onClick && !disabled && onClick(e);
  };

  const stopPropagation = (event) => {
    event.stopPropagation();
  };

  const onChange = (event) => {
    stopPropagation(event);
    onClickAction(event);
  };

  return (
    <StyledDropdownItem
      {...rest}
      noHover={noHover}
      className={className}
      onClick={onClickAction}
      disabled={disabled}
      isActive={isActive}
      isSelected={isSelected}
      withToggle={withToggle}
      isActiveDescendant={isActiveDescendant}
    >
      {icon && (
        <IconWrapper className="drop-down-icon">
          {!withoutIcon ? (
            !icon.includes("images/") ? (
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
            src={RightArrowReactSvgUrl}
            className="drop-down-item_icon"
          />
        </IconWrapper>
      )}

      {withToggle && (
        <WrapperToggle onClick={stopPropagation}>
          <ToggleButton isChecked={checked} onChange={onChange} noAnimation />
        </WrapperToggle>
      )}
    </StyledDropdownItem>
  );
};

DropDownItem.propTypes = {
  /** Sets the dropdown item to display as a separator */
  isSeparator: PropTypes.bool,
  /** Sets the dropdown to display as a header */
  isHeader: PropTypes.bool,
  /** Accepts tab-index */
  tabIndex: PropTypes.number,
  /** Dropdown item text */
  label: PropTypes.string,
  /** Sets the dropdown item to display as disabled */
  disabled: PropTypes.bool,
  /** Dropdown item icon */
  icon: PropTypes.string,
  /** Disables default style hover effect */
  noHover: PropTypes.bool,
  /** Sets an action that will be triggered when the dropdown item is clicked */
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
  isModern: PropTypes.bool,
  isActiveDescendant: PropTypes.bool,
  isSelected: PropTypes.bool,
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
