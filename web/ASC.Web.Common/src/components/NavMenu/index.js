import React, { useEffect } from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import { Backdrop, Toast, Aside, utils } from "asc-web-components";
import Header from "./sub-components/header";
import HeaderNav from "./sub-components/header-nav";
import HeaderUnAuth from "./sub-components/header-unauth";
import { I18nextProvider, withTranslation } from "react-i18next";
import i18n from "./i18n";
import { withRouter } from "react-router";
//import { getLanguage, isDesktopClient } from "../../store/auth/selectors";
import Loaders from "../Loaders";
import { LayoutContextConsumer } from "../Layout/context";
import { isMobile } from "react-device-detect";
import { inject, observer } from "mobx-react";

const backgroundColor = "#0F4071";

const StyledContainer = styled.header`
  align-items: center;
  background-color: ${backgroundColor};

  ${(props) =>
    !props.isLoaded
      ? isMobile &&
        css`
          position: static;

          margin-right: -16px; /* It is a opposite value of padding-right of custom scroll bar,
       so that there is no white bar in the header on loading. (padding-right: 16px)*/
        `
      : isMobile &&
        css`
          .navMenuHeader,
          .profileMenuIcon,
          .navMenuHeaderUnAuth {
            position: fixed;
            z-index: 160;
            top: ${(props) => (props.isVisible ? "0" : "-56px")};

            transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
            -moz-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
            -ms-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
            -webkit-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
            -o-transition: top 0.3s cubic-bezier(0, 0, 0.8, 1);
          }

          width: 100%;
        `}
`;

class NavMenu extends React.Component {
  constructor(props) {
    super(props);
    this.timeout = null;

    const {
      isBackdropVisible,
      isNavHoverEnabled,
      isNavOpened,
      isAsideVisible,
      isDesktop,
    } = props;

    this.state = {
      isBackdropVisible,
      isNavOpened,
      isAsideVisible,
      isNavHoverEnabled,
      isDesktop,
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
    const {
      isBackdropVisible,
      isNavOpened,
      isAsideVisible,
      isDesktop,
    } = this.state;

    const { isAuthenticated, isLoaded, asideContent, history } = this.props;

    const isAsideAvailable = !!asideContent;

    //console.log("NavMenu render", this.state, this.props);

    return (
      <LayoutContextConsumer>
        {(value) => (
          <StyledContainer isLoaded={isLoaded} isVisible={value.isVisible}>
        <Toast />

        <Backdrop
          visible={isBackdropVisible}
          onClick={this.backdropClick}
          withBackground={true}
        />

        {!isDesktop &&
          (isLoaded && isAuthenticated ? (
            <>
              <HeaderNav history={history} />
              <Header
                isNavOpened={isNavOpened}
                onClick={this.showNav}
                onNavMouseEnter={this.handleNavMouseEnter}
                onNavMouseLeave={this.handleNavMouseLeave}
                toggleAside={this.toggleAside}
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

  history: PropTypes.object,
};

NavMenu.defaultProps = {
  isBackdropVisible: false,
  isNavHoverEnabled: true,
  isNavOpened: false,
  isAsideVisible: false,
  isDesktop: false,
};

const NavMenuTranslationWrapper = withTranslation()(NavMenu);

const NavMenuWrapper = (props) => {
  const { language } = props;

  useEffect(() => {
    i18n.changeLanguage(language);
  }, [language]);

  return (
    <I18nextProvider i18n={i18n}>
      <NavMenuTranslationWrapper {...props} />
    </I18nextProvider>
  );
};

NavMenuWrapper.propTypes = {
  language: PropTypes.string.isRequired,
};

export default inject(({ auth }) => {
  const { settingsStore, isAuthenticated, isLoaded, language } = auth;
  const { isDesktopClient: isDesktop } = settingsStore;
  return {
    isAuthenticated,
    isLoaded,
    isDesktop,
    language,
  };
})(withRouter(observer(NavMenuWrapper)));
