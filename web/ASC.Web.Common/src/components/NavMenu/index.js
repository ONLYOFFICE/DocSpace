import React, { useEffect } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { Backdrop, Toast, Aside } from "asc-web-components";
import Header from "./sub-components/header";
import HeaderNav from "./sub-components/header-nav";
import HeaderUnAuth from "./sub-components/header-unauth";

import { I18nextProvider, withTranslation } from "react-i18next";
import i18n from "./i18n";
import { connect } from "react-redux";

import { withRouter } from "react-router";

import { getLanguage } from "../../store/auth/selectors";
import Loaders from "../Loaders";

const backgroundColor = "#0F4071";

const StyledContainer = styled.header`
  align-items: center;
  background-color: ${backgroundColor};
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

    const { isAuthenticated, isLoaded, asideContent, history } = this.props;

    const isAsideAvailable = !!asideContent;

    console.log("NavMenu render", this.state, this.props);

    return (
      <StyledContainer>
        <Toast />

        <Backdrop visible={isBackdropVisible} onClick={this.backdropClick} />

        {isLoaded && isAuthenticated ? (
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
        )}

        {isAsideAvailable && (
          <Aside visible={isAsideVisible} onClick={this.backdropClick}>
            {asideContent}
          </Aside>
        )}
      </StyledContainer>
    );
  }
}

NavMenu.propTypes = {
  isBackdropVisible: PropTypes.bool,
  isNavHoverEnabled: PropTypes.bool,
  isNavOpened: PropTypes.bool,
  isAsideVisible: PropTypes.bool,

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

function mapStateToProps(state) {
  const { isAuthenticated, isLoaded } = state.auth;

  return {
    isAuthenticated,
    isLoaded,

    language: getLanguage(state),
  };
}

export default connect(mapStateToProps)(withRouter(NavMenuWrapper));
