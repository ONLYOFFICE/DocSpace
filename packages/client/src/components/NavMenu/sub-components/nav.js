import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import Scrollbar from "@docspace/components/scrollbar";
import { isMobileOnly, isMobile } from "react-device-detect";
import { Base } from "@docspace/components/themes";

const StyledNav = styled.nav`
  background-color: ${(props) => props.theme.nav.backgroundColor};
  height: 100%;
  left: 0;
  overflow-x: hidden;
  overflow-y: auto;
  position: fixed;
  top: 0;
  transition: width 0.3s ease-in-out;
  width: ${(props) => (props.opened ? "240px" : "0")};
  z-index: 200;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  .version-box {
    position: absolute;

    @media (orientation: landscape) {
      position: ${isMobileOnly && "relative"};
      margin-top: 16px;
    }

    ${(props) =>
      props.numberOfModules &&
      `@media (max-height: ${props.numberOfModules * 52 + 80}px) {
      position: ${!isMobile && "relative"};
      margin-top: 16px;
    }`}

    bottom: 8px;
    left: 16px;

    white-space: nowrap;
    a:focus {
      outline: 0;
    }
  }
`;

StyledNav.defaultProps = { theme: Base };

const StyledScrollbar = styled(Scrollbar)`
  width: ${(props) => (props.opened ? 240 : 56)};
`;

const Nav = React.memo((props) => {
  //console.log("Nav render");
  const {
    opened,
    onMouseEnter,
    onMouseLeave,
    children,
    numberOfModules,
  } = props;
  return (
    <StyledNav
      opened={opened}
      onMouseEnter={onMouseEnter}
      onMouseLeave={onMouseLeave}
      numberOfModules={numberOfModules}
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
    PropTypes.node,
  ]),
};

export default Nav;
