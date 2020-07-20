import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import NavItem from "./nav-item";
import Headline from "../../Headline";
import { utils } from "asc-web-components";
import RecoverAccess from "../sub-components/recover-access";
import HeaderLogo from "../sub-components/header-logo";
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

  .header-module-title {
    display: block;
    font-size: 21px;
    line-height: 0;

    @media ${desktop} {
      display: none;
    }
  }
`;

const HeaderComponent = React.memo(props => {
  //console.log("Header render");
  const currentModule = props.currentModule && props.currentModule.title;
  const isAuthKey = localStorage.getItem("asc_auth_key") ? true : false;
  
  return (
    <Header module={currentModule}>
      {props.currentUser && <NavItem
        iconName="MenuIcon"
        badgeNumber={props.badgeNumber}
        onClick={props.onClick}
        noHover={true}
      />}
      <HeaderLogo module={module} logged={isAuthKey} />
      {!isAuthKey && <RecoverAccess t={props.t} className="recover-container" />}
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
  currentModule: PropTypes.object,
  currentUser: PropTypes.object,
  t: PropTypes.func.isRequired
};

export default HeaderComponent;
