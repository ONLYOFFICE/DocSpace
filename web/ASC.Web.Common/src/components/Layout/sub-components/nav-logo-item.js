import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { Icons } from "asc-web-components";

const LogoItem = styled.div`
  display: flex;
  min-width: 56px;
  min-height: 56px;
  align-items: center;
  padding: 0 16px;
  cursor: pointer;
`;

const NavLogoItem = React.memo(props => {
  //console.log("NavLogoItem render");
  const navLogoIconStyle = {
    display: props.opened ? "none" : "block"
  };

  const navLogoOpenedIconStyle = {
    display: props.opened ? "block" : "none",
    width: "141px",
    minWidth: "141px",
    height: "22px",
    minHeight: "22px",
    position: "absolute",
    top: "17px",
    left: "13px"
  };

  return (
    <LogoItem>
      <Icons.NavLogoIcon style={navLogoIconStyle} onClick={props.onClick} />
      <Icons.NavLogoOpenedIcon
        style={navLogoOpenedIconStyle}
        onClick={props.onClick}
      />
    </LogoItem>
  );
});

NavLogoItem.displayName = "NavLogoItem";

NavLogoItem.propTypes = {
  opened: PropTypes.bool,
  onClick: PropTypes.func
};

export default NavLogoItem;
