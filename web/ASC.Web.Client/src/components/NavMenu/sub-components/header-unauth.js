import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import Box from "@appserver/components/box";
import RecoverAccess from "./recover-access-container";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";

const backgroundColor = "#0F4071";

const Header = styled.header`
  align-items: left;
  background-color: ${backgroundColor};
  display: flex;
  width: calc(100vw - 64px);
  height: 48px;
  justify-content: left;
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
    padding: 3px 20px 0 6px;
    cursor: pointer;
  }
`;

const HeaderUnAuth = ({
  enableAdmMess,
  wizardToken,
  isAuthenticated,
  isLoaded,
}) => {
  const { t } = useTranslation();

  return (
    <Header isLoaded={isLoaded} className="navMenuHeaderUnAuth">
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
                src={combineUrl(
                  AppServerConfig.proxyURL,
                  "/static/images/nav.logo.react.svg"
                )}
              />
              <img
                className="header-logo-icon"
                src={combineUrl(
                  AppServerConfig.proxyURL,
                  "/static/images/nav.logo.opened.react.svg"
                )}
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

export default inject(({ auth }) => {
  const { settingsStore, isAuthenticated, isLoaded } = auth;
  const { enableAdmMess, wizardToken } = settingsStore;
  return {
    enableAdmMess,
    wizardToken: wizardToken || "/",
    isAuthenticated,
    isLoaded,
  };
})(observer(HeaderUnAuth));
