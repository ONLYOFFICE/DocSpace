import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import {
  isMobileOnly,
  isDesktop as isDesktopDevice,
} from "react-device-detect";
import { isMobile as isMobileUtils } from "@docspace/components/utils/device";
import Backdrop from "@docspace/components/backdrop";
import Aside from "@docspace/components/aside";

import Header from "./sub-components/header";
import HeaderNav from "./sub-components/header-nav";
import HeaderUnAuth from "./sub-components/header-unauth";
import { I18nextProvider, withTranslation } from "react-i18next";
import { useNavigate, useLocation } from "react-router-dom";

import Loaders from "@docspace/common/components/Loaders";
import { LayoutContextConsumer } from "../Layout/context";

import { inject, observer } from "mobx-react";
import i18n from "./i18n";
import PreparationPortalDialog from "../dialogs/PreparationPortalDialog";
import { Base } from "@docspace/components/themes";

const StyledContainer = styled.header`
  position: relative;
  align-items: center;
  background-color: ${(props) => props.theme.header.backgroundColor};

  ${(props) =>
    !props.isLoaded
      ? isMobileOnly &&
        css`
          position: static;

          margin-right: -16px; /* It is a opposite value of padding-right of custom scroll bar,
       so that there is no white bar in the header on loading. (padding-right: 16px)*/
        `
      : isMobileOnly &&
        css`
          .navMenuHeader,
          .profileMenuIcon,
          .navMenuHeaderUnAuth {
            position: absolute;
            z-index: 160;
            top: 0;
            // top: ${(props) => (props.isVisible ? "0" : "-48px")};

            transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
            -moz-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
            -ms-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
            -webkit-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
            -o-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
          }

          width: 100%;
        `}
`;

StyledContainer.defaultProps = { theme: Base };

const NavMenu = (props) => {
  const timeout = React.useRef(null);

  const navigate = useNavigate();
  const location = useLocation();

  const [showNavMenu, setShowNavMenu] = React.useState(
    isMobileOnly || isMobileUtils()
  );

  const [isBackdropVisible, setIsBackdropVisible] = React.useState(
    props.isBackdropVisible
  );
  const [isNavOpened, setIsNavOpened] = React.useState(props.isNavHoverEnabled);
  const [isAsideVisible, setIsAsideVisible] = React.useState(props.isNavOpened);
  const [isNavHoverEnabled, setIsNavHoverEnabled] = React.useState(
    props.isAsideVisible
  );

  const backdropClick = () => {
    setIsBackdropVisible(false);
    setIsNavOpened(false);
    setIsAsideVisible(false);
    setIsNavHoverEnabled((val) => !val);
  };

  const showNav = () => {
    setIsBackdropVisible(true);
    setIsNavOpened(true);
    setIsAsideVisible(false);
    setIsNavHoverEnabled(false);
  };

  const clearNavTimeout = () => {
    if (timeout.current === null) return;
    clearTimeout(timeout.current);
    timeout.current = null;
  };

  const handleNavMouseEnter = () => {
    if (!isNavHoverEnabled) return;
    timeout.current = setTimeout(() => {
      setIsBackdropVisible(false);
      setIsNavOpened(true);
      setIsAsideVisible(false);
    }, 1000);
  };

  const handleNavMouseLeave = () => {
    if (!isNavHoverEnabled) return;
    clearNavTimeout();
    setIsBackdropVisible(false);
    setIsNavOpened(false);
    setIsAsideVisible(false);
  };

  const toggleAside = () => {
    clearNavTimeout();
    setIsBackdropVisible(true);
    setIsNavOpened(false);
    setIsAsideVisible(true);
    setIsNavHoverEnabled(false);
  };

  const onResize = React.useCallback(() => {
    setShowNavMenu(isMobileUtils() || isMobileOnly);
  }, []);

  React.useEffect(() => {
    if (isDesktopDevice) window.addEventListener("resize", onResize);

    return () => window.removeEventListener("resize", onResize);
  }, []);

  const {
    isAuthenticated,
    isLoaded,
    asideContent,

    isDesktop,
    isFrame,
    showHeader,
  } = props;

  const isAsideAvailable = !!asideContent;
  const hideHeader = isDesktop || (!showHeader && isFrame);

  if (!showNavMenu) return <></>;

  const isPreparationPortal = location.pathname === "/preparation-portal";
  return (
    <LayoutContextConsumer>
      {(value) => (
        <StyledContainer
          isLoaded={isLoaded}
          isVisible={value.isVisible}
          isDesktop={hideHeader}
        >
          <Backdrop
            visible={isBackdropVisible}
            onClick={backdropClick}
            withBackground={true}
          />

          {!hideHeader &&
            (isLoaded && isAuthenticated ? (
              <>
                {!isPreparationPortal && <HeaderNav />}
                <Header
                  isPreparationPortal={isPreparationPortal}
                  isNavOpened={isNavOpened}
                  onClick={showNav}
                  onNavMouseEnter={handleNavMouseEnter}
                  onNavMouseLeave={handleNavMouseLeave}
                  toggleAside={toggleAside}
                  backdropClick={backdropClick}
                />
              </>
            ) : !isLoaded && isAuthenticated ? (
              <Loaders.Header />
            ) : (
              <HeaderUnAuth />
            ))}

          {isAsideAvailable && (
            <Aside visible={isAsideVisible} onClick={backdropClick}>
              {asideContent}
            </Aside>
          )}
        </StyledContainer>
      )}
    </LayoutContextConsumer>
  );
};

NavMenu.propTypes = {
  isBackdropVisible: PropTypes.bool,
  isNavHoverEnabled: PropTypes.bool,
  isNavOpened: PropTypes.bool,
  isAsideVisible: PropTypes.bool,
  isDesktop: PropTypes.bool,

  asideContent: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node,
  ]),

  isAuthenticated: PropTypes.bool,
  isLoaded: PropTypes.bool,
};

NavMenu.defaultProps = {
  isBackdropVisible: false,
  isNavHoverEnabled: true,
  isNavOpened: false,
  isAsideVisible: false,
  isDesktop: false,
};

const NavMenuWrapper = inject(({ auth }) => {
  const { settingsStore, isAuthenticated, isLoaded, language } = auth;
  const { isDesktopClient: isDesktop, frameConfig, isFrame } = settingsStore;

  return {
    isAuthenticated,
    isLoaded,
    isDesktop,
    language,

    showHeader: frameConfig?.showHeader,
    isFrame,
  };
})(observer(withTranslation(["NavMenu", "Common"])(NavMenu)));

export default () => (
  <I18nextProvider i18n={i18n}>
    <NavMenuWrapper />
  </I18nextProvider>
);
