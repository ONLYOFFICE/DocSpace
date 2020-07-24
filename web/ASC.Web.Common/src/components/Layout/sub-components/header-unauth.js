import React from "react";
import PropTypes from "prop-types";
import { Box } from "asc-web-components";
import styled from "styled-components";

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
  }

  .header-logo-min_icon {
    display: none;
    cursor: pointer;
    width: 24px;
    height: 24px;
  }

  .header-logo-icon {
    width: 146px;
    height: 24px;
    position: relative;
    padding: 4px 20px 0 6px;
    cursor: pointer;
  }
`;

const HeaderUnauth = () => {
  //console.log("Header render");
  //const currentModule = props.currentModule && props.currentModule.title;
  return (
    <Header>

      <Box>

        <Box>
          <a className="header-logo-wrapper" href="/">
            <img className="header-logo-min_icon" src="images/nav.logo.react.svg" />
            <img
              className="header-logo-icon"
              src="images/nav.logo.opened.react.svg"
            />
          </a>
        </Box>

        <Box>
          Recover
        </Box>

      </Box>

    </Header>
  );
};

HeaderUnauth.displayName = "Header";

HeaderUnauth.propTypes = {
  badgeNumber: PropTypes.number,
  onClick: PropTypes.func,
  onLogoClick: PropTypes.func,
  currentModule: PropTypes.object
};

export default HeaderUnauth;
