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
    maxHeight: "24px",
    width: "auto"
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
