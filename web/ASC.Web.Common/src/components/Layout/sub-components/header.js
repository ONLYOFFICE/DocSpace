import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { utils } from "asc-web-components";
const { tablet } = utils.device;
import NavItem from "./nav-item";
import Headline from "../../Headline";

const backgroundColor = "#0F4071";

const Header = styled.header`
  align-items: center;
  background-color: ${backgroundColor};
  display: none;
  z-index: 185;
  position: absolute;
  width: 100vw;

  @media ${tablet} {
    display: flex;
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
      <Headline type="header" color="#FFFFFF">
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
