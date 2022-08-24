import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import Box from "@docspace/components/box";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig } from "@docspace/common/constants";
import { Base } from "@docspace/components/themes";

const Header = styled.header`
  align-items: left;
  background-color: ${(props) => props.theme.header.backgroundColor};
  display: flex;
  width: 100vw;
  height: 48px;
  justify-content: center;

  .header-items-wrapper {
    width: 960px;

    @media (max-width: 768px) {
      width: 475px;
    }
    @media (max-width: 375px) {
      display: flex;
      align-items: center;
      justify-content: center;
      //padding: 0 16px;
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
    width: 100%;
    height: 100%;
    padding: 12px 0;
    cursor: pointer;
  }
`;

Header.defaultProps = { theme: Base };

const HeaderUnAuth = ({
  enableAdmMess,
  wizardToken,
  isAuthenticated,
  isLoaded,
}) => {
  const { t } = useTranslation("NavMenu");

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
                className="header-logo-icon"
                src={combineUrl(
                  AppServerConfig.proxyURL,
                  "/static/images/logo.docspace.react.svg"
                )}
              />
            </a>
          </div>
        ) : (
          <></>
        )}
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
    wizardToken,
    isAuthenticated,
    isLoaded,
  };
})(observer(HeaderUnAuth));
