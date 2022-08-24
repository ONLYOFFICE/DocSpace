import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import Backdrop from "@docspace/components/backdrop";
import Aside from "@docspace/components/aside";

import Header from "./sub-components/header";
import HeaderNav from "./sub-components/header-nav";
import HeaderUnAuth from "./sub-components/header-unauth";
import { I18nextProvider, withTranslation } from "react-i18next";
import { withRouter } from "react-router";

import Loaders from "@docspace/common/components/Loaders";
import { LayoutContextConsumer } from "../Layout/context";
import { isMobileOnly } from "react-device-detect";
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

class NavMenu extends React.Component {
  constructor(props) {
    super(props);
    this.timeout = null;

    const {
      isBackdropVisible,
      isNavHoverEnabled,
      isNavOpened,
      isAsideVisible,
    } = props;

    this.state = {
      isBackdropVisible,
      isNavOpened,
      isAsideVisible,
      isNavHoverEnabled,
    };
  }

  backdropClick = () => {
    this.setState({
      isBackdropVisible: false,
      isNavOpened: false,
      isAsideVisible: false,
      isNavHoverEnabled: !this.state.isNavHoverEnabled,
    });
  };

  showNav = () => {
    this.setState({
      isBackdropVisible: true,
      isNavOpened: true,
      isAsideVisible: false,
      isNavHoverEnabled: false,
    });
  };

  clearNavTimeout = () => {
    if (this.timeout == null) return;
    clearTimeout(this.timeout);
    this.timeout = null;
  };

  handleNavMouseEnter = () => {
    if (!this.state.isNavHoverEnabled) return;
    this.timeout = setTimeout(() => {
      this.setState({
        isBackdropVisible: false,
        isNavOpened: true,
        isAsideVisible: false,
      });
    }, 1000);
  };

  handleNavMouseLeave = () => {
    if (!this.state.isNavHoverEnabled) return;
    this.clearNavTimeout();
    this.setState({
      isBackdropVisible: false,
      isNavOpened: false,
      isAsideVisible: false,
    });
  };

  toggleAside = () => {
    this.clearNavTimeout();
    this.setState({
      isBackdropVisible: true,
      isNavOpened: false,
      isAsideVisible: true,
      isNavHoverEnabled: false,
    });
  };

  render() {
    const { isBackdropVisible, isNavOpened, isAsideVisible } = this.state;

    const {
      isAuthenticated,
      isLoaded,
      asideContent,
      history,
      isDesktop,
      preparationPortalDialogVisible,
      isFrame,
      showHeader,
    } = this.props;

    const isAsideAvailable = !!asideContent;
    const hideHeader = isDesktop || (!showHeader && isFrame);
    //console.log("NavMenu render", this.state, this.props);
    const isPreparationPortal =
      history.location.pathname === "/preparation-portal";
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
              onClick={this.backdropClick}
              withBackground={true}
            />

            {!hideHeader &&
              (isLoaded && isAuthenticated ? (
                <>
                  {!isPreparationPortal && <HeaderNav />}
                  <Header
                    isPreparationPortal={isPreparationPortal}
                    isNavOpened={isNavOpened}
                    onClick={this.showNav}
                    onNavMouseEnter={this.handleNavMouseEnter}
                    onNavMouseLeave={this.handleNavMouseLeave}
                    toggleAside={this.toggleAside}
                    backdropClick={this.backdropClick}
                  />
                </>
              ) : !isLoaded && isAuthenticated ? (
                <Loaders.Header />
              ) : (
                <HeaderUnAuth />
              ))}

            {isAsideAvailable && (
              <Aside visible={isAsideVisible} onClick={this.backdropClick}>
                {asideContent}
              </Aside>
            )}
            {preparationPortalDialogVisible && <PreparationPortalDialog />}
          </StyledContainer>
        )}
      </LayoutContextConsumer>
    );
  }
}

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

const NavMenuWrapper = inject(({ auth, backup }) => {
  const { settingsStore, isAuthenticated, isLoaded, language } = auth;
  const { isDesktopClient: isDesktop, frameConfig, isFrame } = settingsStore;
  const { preparationPortalDialogVisible } = backup;
  return {
    isAuthenticated,
    isLoaded,
    isDesktop,
    language,
    preparationPortalDialogVisible,
    showHeader: frameConfig?.showHeader,
    isFrame,
  };
})(observer(withTranslation(["NavMenu", "Common"])(withRouter(NavMenu))));

export default () => (
  <I18nextProvider i18n={i18n}>
    <NavMenuWrapper />
  </I18nextProvider>
);
