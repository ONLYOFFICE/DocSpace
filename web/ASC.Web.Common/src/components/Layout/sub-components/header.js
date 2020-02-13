import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import NavItem from "./nav-item";
import Headline from "../../Headline";
import { Icons } from "asc-web-components"

const backgroundColor = "#0F4071";

const Header = styled.header`
  align-items: center;
  background-color: ${backgroundColor};
  display: flex;
  z-index: 185;
  position: absolute;
  width: 100vw;

  .header-module-title {
    display: block;
    font-size: 21px;

    @media(min-width: 1024px) {
      display: none;
    }
   }

  .header-logo-min_icon {
    display: none;

    @media(max-width: 620px) {
      padding: 0 12px 0 0px;
      display: block;
    }
  }

  .header-logo-icon {
    width: 160px;
    position: relative;
    padding: 0 12px 0 0px;

    @media(max-width: 620px) {
      display: none;
    }
  }
`;

const HeaderComponent = React.memo(props => {
  //console.log("Header render");
  return (
    <Header>
      <NavItem
        iconName="MenuIcon"
        badgeNumber={props.badgeNumber}
        onClick={props.onClick}
        noHover={true}
      />
      <Icons.NavLogoIcon className="header-logo-min_icon" />
      <Icons.NavLogoOpenedIcon className="header-logo-icon" />
      <Headline className="header-module-title" type="header" color="#FFFFFF">
        {props.currentModule && props.currentModule.title}
      </Headline>
    </Header>
  );
});

HeaderComponent.displayName = "Header";

HeaderComponent.propTypes = {
  badgeNumber: PropTypes.number,
  onClick: PropTypes.func,
  currentModule: PropTypes.object
};

export default HeaderComponent;
