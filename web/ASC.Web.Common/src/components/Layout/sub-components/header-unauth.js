import React from "react";
import PropTypes from "prop-types";
import { Box } from "asc-web-components";
import styled from "styled-components";
import RecoverAccess from "./recover-access-container";
import { connect } from "react-redux";

const backgroundColor = "#0F4071";

const Header = styled.header`
  align-items: center;
  background-color: ${backgroundColor};
  display: flex;
  justify-content: center;
  z-index: 185;
  position: absolute;
  width: 100vw;
  height: 56px;

  .header-items-wrapper {
    width: 960px;

    @media (max-width: 768px) {
      width: 475px;
    }
    @media (max-width: 375px) {
      width: 311px;
    }
  }

  .header-logo-wrapper {
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
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

const HeaderUnauth = ({ t, enableAdmMess }) => {

  //console.log("Header render");


  return (
    <Header>
      <Box displayProp="flex" justifyContent="space-between" alignItems="center" className="header-items-wrapper">
        <div>
          <a className="header-logo-wrapper" href="/">
            <img className="header-logo-min_icon" src="images/nav.logo.react.svg" />
            <img
              className="header-logo-icon"
              src="images/nav.logo.opened.react.svg"
            />
          </a>
        </div>

        <div>
          {enableAdmMess && <RecoverAccess t={t} />}
        </div>
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

function mapStateToProps(state) {
  return {
    enableAdmMess: state.auth.settings.enableAdmMess
  };
}

export default connect(mapStateToProps, null)(HeaderUnauth);
