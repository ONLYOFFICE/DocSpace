import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { tablet } from "../../../utils/device";
import NavItem from "./nav-item";
import Heading from "../../heading";

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

  .heading {
    margin: 0;
    line-height: 56px;
    font-weight: 700;
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
      />
      <Heading className="heading" size='xlarge' color="#FFFFFF">
        {props.currentModule && props.currentModule.title}
      </Heading>
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
