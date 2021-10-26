import React from "react";
import PropTypes from "prop-types";
import { ReactSVG } from "react-svg";

import { StyledMenuItem, StyledText, IconWrapper } from "./styled-menu-item";

//TODO: Add arrow type
const MenuItem = (props) => {
  //console.log("MenuItem render");
  const {
    isHeader,
    isSeparator,
    label,
    icon,
    children,
    onClick,
    className,
  } = props;

  const onClickAction = (e) => {
    onClick && onClick(e);
  };

  return (
    <StyledMenuItem {...props} className={className} onClick={onClickAction}>
      {icon && (
        <IconWrapper isHeader={isHeader}>
          <ReactSVG src={icon} className="drop-down-item_icon" />
        </IconWrapper>
      )}
      {isSeparator ? (
        <></>
      ) : label ? (
        <StyledText isHeader={isHeader} truncate={true}>
          {label}
        </StyledText>
      ) : (
        children && children
      )}
    </StyledMenuItem>
  );
};

MenuItem.propTypes = {
  /** Tells when the menu item should display like separator */
  isSeparator: PropTypes.bool,
  /** Tells when the menu item should display like header */
  isHeader: PropTypes.bool,
  /** Accepts tab-index */
  tabIndex: PropTypes.number,
  /** menu item text */
  label: PropTypes.string,
  /** menu item icon */
  icon: PropTypes.string,
  /** Disable default style hover effect */
  noHover: PropTypes.bool,
  /** What the menu item will trigger when clicked */
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

MenuItem.defaultProps = {
  isSeparator: false,
  isHeader: false,
  noHover: false,
  textOverflow: false,
  tabIndex: -1,
  label: "",
};

export default MenuItem;
