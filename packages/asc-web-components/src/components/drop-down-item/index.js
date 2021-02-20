import React from "react";

import PropTypes from "prop-types";
import { Icons } from "../icons";
import { StyledDropdownItem, IconWrapper } from "./styled-drop-down-item";
import {ReactSVG} from "react-svg"
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
          {/* {React.createElement(Icons[icon], {
            size: "scale",
            className: "drop-down-item_icon",
          })} */}
          <ReactSVG src={icon} className="drop-down-item_icon"/>
        </IconWrapper>
      )}
      {isSeparator ? "\u00A0" : label ? label : children && children}
    </StyledDropdownItem>
  );
};

DropDownItem.propTypes = {
  isSeparator: PropTypes.bool,
  isHeader: PropTypes.bool,
  tabIndex: PropTypes.number,
  label: PropTypes.string,
  disabled: PropTypes.bool,
  icon: PropTypes.string,
  noHover: PropTypes.bool,
  onClick: PropTypes.func,
  children: PropTypes.any,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
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
