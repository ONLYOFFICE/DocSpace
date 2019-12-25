import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { utils, Scrollbar } from "asc-web-components";
const { tablet } = utils.device;

const backgroundColor = "#0F4071";

const StyledNav = styled.nav`
  background-color: ${backgroundColor};
  height: 100%;
  left: 0;
  overflow-x: hidden;
  overflow-y: auto;
  position: fixed;
  top: 0;
  transition: width 0.3s ease-in-out;
  width: ${props => (props.opened ? "240px" : "56px")};
  z-index: 200;

  @media ${tablet} {
    width: ${props => (props.opened ? "240px" : "0")};
  }
`;

const StyledScrollbar = styled(Scrollbar)`
  width: ${props => (props.opened ? 240 : 56)};
`;

const Nav = React.memo(props => {
  //console.log("Nav render");
  const { opened, onMouseEnter, onMouseLeave, children } = props;

  return (
    <StyledNav
      opened={opened}
      onMouseEnter={onMouseEnter}
      onMouseLeave={onMouseLeave}
    >
      <StyledScrollbar stype="smallWhite">{children}</StyledScrollbar>
    </StyledNav>
  );
});

Nav.displayName = "Nav";

Nav.propTypes = {
  opened: PropTypes.bool,
  onMouseEnter: PropTypes.func,
  onMouseLeave: PropTypes.func,
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node
  ])
};

export default Nav;
