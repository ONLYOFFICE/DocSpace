import React from "react";
import PropTypes from "prop-types";
import { Box } from "asc-web-components";
import styled from "styled-components";
import RecoverAccess from "./recover-access-container";
import { connect } from "react-redux";
import { useTranslation } from "react-i18next";

const backgroundColor = "#0F4071";

const Header = styled.header`
  align-items: center;
  background-color: ${backgroundColor};
  display: flex;
  width: calc(100vw - 64px);
  height: 56px;
  justify-content: center;
  padding: 0 32px;

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

const HeaderUnAuth = ({
  enableAdmMess,
  wizardToken,
  isAuthenticated,
  isLoaded,
}) => {
  //console.log("HeaderUnAuth render");

  const { t } = useTranslation();

  return (
    <Header>
      <Box
        displayProp="flex"
        justifyContent="space-between"
        alignItems="center"
        className="header-items-wrapper"
      >
        {!isAuthenticated && isLoaded ? (
          <div>
            <a className="header-logo-wrapper" href="/">
              <img
                className="header-logo-min_icon"
                src="images/nav.logo.react.svg"
              />
              <img
                className="header-logo-icon"
                src="images/nav.logo.opened.react.svg"
              />
            </a>
          </div>
        ) : (
          <></>
        )}

        <div>{enableAdmMess && !wizardToken && <RecoverAccess t={t} />}</div>
      </Box>
    </Header>
  );
};

HeaderUnAuth.displayName = "Header";

HeaderUnAuth.propTypes = {
  enableAdmMess: PropTypes.bool,
  wizardToken: PropTypes.string,
  isAuthenticated: PropTypes.bool,
  isLoaded: PropTypes.bool,
};

const mapStateToProps = (state) => {
  const { isAuthenticated, isLoaded, settings } = state.auth;
  const { enableAdmMess, wizardToken } = settings;

  return {
    enableAdmMess,
    wizardToken,
    isAuthenticated,
    isLoaded,
  };
};

export default connect(mapStateToProps)(HeaderUnAuth);
