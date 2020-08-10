import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import NavItem from "./nav-item";
import Headline from "../../Headline";
import { utils } from "asc-web-components";
const { desktop } = utils.device;

const backgroundColor = "#0F4071";

const Header = styled.header`
  align-items: center;
  background-color: ${backgroundColor};
  display: flex;
  z-index: 185;
  position: absolute;
  width: 100vw;
  height: 56px;
  .header-logo-wrapper {
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  }
  .header-module-title {
    display: block;
    font-size: 21px;
    line-height: 0;
    @media ${desktop} {
      display: none;
    }
  }
  .header-logo-min_icon {
    display: none;
    cursor: pointer;
    
    width: 24px;
    height: 24px;
    @media (max-width: 620px) {
      padding: 0 12px 0 0;
      display: ${props => props.module && "block"};
    }
  }
  .header-logo-icon {
    width: 146px;
    height: 24px;
    position: relative;
    padding: 4px 20px 0 6px;
    cursor: pointer;
    @media (max-width: 620px) {
      display: ${props => (props.module ? "none" : "block")};
      padding: 0px 20px 0 6px;
    }
  }
`;

const HeaderComponent = React.memo(props => {
  //console.log("Header render");
  const currentModule = props.currentModule && props.currentModule.title;
  return (
    <Header module={currentModule}>
      <NavItem
        iconName="MenuIcon"
        badgeNumber={props.badgeNumber}
        onClick={props.onClick}
        noHover={true}
      />
      <a className="header-logo-wrapper" href="/">
        <img className="header-logo-min_icon" src="images/nav.logo.react.svg" />
        <img
          className="header-logo-icon"
          src="images/nav.logo.opened.react.svg"
        />
      </a>
      <Headline className="header-module-title" type="header" color="#FFF">
        {props.currentModule && props.currentModule.title}
      </Headline>
    </Header>
  );
});

HeaderComponent.displayName = "Header";

HeaderComponent.propTypes = {
  badgeNumber: PropTypes.number,
  onClick: PropTypes.func,
  onLogoClick: PropTypes.func,
  currentModule: PropTypes.object
};

export default HeaderComponent;