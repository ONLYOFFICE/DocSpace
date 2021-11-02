import React from "react";
import PropTypes from "prop-types";
import { ReactSVG } from "react-svg";

import DomHelpers from "../utils/domHelpers";

import { isMobile } from "react-device-detect";
import {
  isTablet as isTabletUtils,
  isMobile as isMobileUtils,
} from "../utils/device";

import { StyledMenuItem, StyledText, IconWrapper } from "./styled-menu-item";

import ArrowIcon from "./svg/folder-arrow.react.svg";
import ArrowMobileIcon from "./svg/folder-arrow.mobile.react.svg";
import NewContextMenu from "../new-context-menu";

//TODO: Add arrow type
const MenuItem = (props) => {
  const [hover, setHover] = React.useState(false);
  const [positionContextMenu, setPositionContextMenu] = React.useState(null);
  const itemRef = React.useRef(null);
  const cmRef = React.useRef(null);
  //console.log("MenuItem render");
  const {
    isHeader,
    isSeparator,
    label,
    icon,
    options,
    children,
    onClick,
    className,
  } = props;

  const onHover = () => {
    if (!cmRef.current) return;
    if (hover) {
      getPosition();
      cmRef.current.show(new Event("click"));
    } else {
      cmRef.current.hide(new Event("click"));
    }
  };

  const getPosition = () => {
    if (!cmRef.current) return;
    if (!itemRef.current) return;
    const outerWidth = DomHelpers.getOuterWidth(itemRef.current);
    const offset = DomHelpers.getOffset(itemRef.current);

    setPositionContextMenu({
      top: offset.top,
      left: offset.left + outerWidth + 10,
      width: outerWidth,
    });
  };

  React.useEffect(() => {
    onHover();
  }, [hover]);

  const onClickAction = (e) => {
    onClick && onClick(e);
  };

  return options ? (
    <StyledMenuItem
      {...props}
      className={className}
      onClick={onClickAction}
      ref={itemRef}
      onMouseEnter={() => setHover(true)}
      onMouseLeave={() => setHover(false)}
    >
      {icon && (
        <IconWrapper isHeader={isHeader}>
          <ReactSVG src={icon} className="drop-down-item_icon" />
        </IconWrapper>
      )}
      {isSeparator ? (
        <></>
      ) : label ? (
        <>
          <StyledText isHeader={isHeader} truncate={true}>
            {label}
          </StyledText>
          {isMobile || isTabletUtils() || isMobileUtils() ? (
            <ArrowMobileIcon className="arrow-icon" />
          ) : (
            <ArrowIcon className="arrow-icon" />
          )}
          <NewContextMenu
            ref={cmRef}
            model={options}
            withBackdrop={false}
            position={positionContextMenu}
          />
        </>
      ) : (
        children && children
      )}
    </StyledMenuItem>
  ) : (
    <StyledMenuItem {...props} className={className} onClick={onClickAction}>
      {icon && (
        <IconWrapper isHeader={isHeader}>
          <ReactSVG src={icon} className="drop-down-item_icon" />
        </IconWrapper>
      )}
      {isSeparator ? (
        <></>
      ) : label ? (
        <>
          <StyledText isHeader={isHeader} truncate={true}>
            {label}
          </StyledText>
        </>
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
  /** Menu item text */
  label: PropTypes.string,
  /** Menu item icon */
  icon: PropTypes.string,
  /** Tells when the menu item should display like arrow and open context menu */
  options: PropTypes.array,
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
