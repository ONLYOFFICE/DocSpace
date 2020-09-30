import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { Backdrop, Toast, Aside } from "asc-web-components";
import Header from "./sub-components/header";
import Nav from "./sub-components/nav";
import HeaderNav from "./sub-components/header-nav";
import NavLogoItem from "./sub-components/nav-logo-item";
import NavItem from "./sub-components/nav-item";
import HeaderUnAuth from "./sub-components/header-unauth";

import { I18nextProvider, withTranslation } from "react-i18next";
import i18n from "./i18n";
import { connect } from "react-redux";

import { withRouter } from "react-router";
import { logout } from "../../store/auth/actions";
import {
  getCurrentUser,
  getLanguage,
  getIsolateModules,
  getMainModules,
  getCurrentProduct
} from "../../store/auth/selectors";

class NavMenu extends React.Component {
  constructor(props) {
    super(props);
    this.timeout = null;

    const {
      isBackdropVisible,
      isNavHoverEnabled,
      isNavOpened,
      isAsideVisible
    } = props;

    this.state = {
      isBackdropVisible,
      isNavOpened,
      isAsideVisible,
      isNavHoverEnabled
    };
  }

  getCurrentUserActions = () => {
    const { t } = this.props;

    const currentUserActions = [
      {
        key: "ProfileBtn",
        label: t("Profile"),
        onClick: this.onProfileClick,
        url: "/products/people/view/@self"
      },
      {
        key: "AboutBtn",
        label: t("AboutCompanyTitle"),
        onClick: this.onAboutClick,
        url: "/about"
      },
      {
        key: "LogoutBtn",
        label: t("LogoutButton"),
        onClick: this.onLogoutClick
      }
    ];

    return currentUserActions;
  };

  backdropClick = () => {
    this.setState({
      isBackdropVisible: false,
      isNavOpened: false,
      isAsideVisible: false,
      isNavHoverEnabled: !this.state.isNavHoverEnabled
    });
  };

  showNav = () => {
    this.setState({
      isBackdropVisible: true,
      isNavOpened: true,
      isAsideVisible: false,
      isNavHoverEnabled: false
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
        isAsideVisible: false
      });
    }, 1000);
  };

  handleNavMouseLeave = () => {
    if (!this.state.isNavHoverEnabled) return;
    this.clearNavTimeout();
    this.setState({
      isBackdropVisible: false,
      isNavOpened: false,
      isAsideVisible: false
    });
  };

  toggleAside = () => {
    this.clearNavTimeout();
    this.setState({
      isBackdropVisible: true,
      isNavOpened: false,
      isAsideVisible: true,
      isNavHoverEnabled: false
    });
  };

  onProfileClick = () => {
    const { history, homepage } = this.props;
    if (homepage == "/products/people") {
      history.push("/products/people/view/@self");
    } else {
      window.open("/products/people/view/@self", "_self");
    }
  };

  onAboutClick = () => {
    window.open("/about", "_self");
  };

  onLogoutClick = () => {
    this.props.logout();
  };

  render() {
    const { isBackdropVisible, isNavOpened, isAsideVisible } = this.state;

    const {
      isAuthenticated,
      isLoaded,
      isolateModules,
      mainModules,
      currentUser,
      asideContent,
      currentProduct
    } = this.props;

    const isBackdropAvailable = mainModules.length > 0 || !!asideContent;
    const isNavAvailable = mainModules.length > 0;
    const isAsideAvailable = !!asideContent;

    console.log("NavMenu render", this.state, this.props);

    return (
      <>
        <Toast />
        {isBackdropAvailable && (
          <Backdrop visible={isBackdropVisible} onClick={this.backdropClick} />
        )}

        {isAuthenticated ? (
          <>
            <HeaderNav
              modules={isolateModules}
              user={currentUser}
              userActions={this.getCurrentUserActions()}
            />
            <Header onClick={this.showNav} />
          </>
        ) : (
          isLoaded && <HeaderUnAuth />
        )}

        {isNavAvailable && (
          <Nav
            opened={isNavOpened}
            onMouseEnter={this.handleNavMouseEnter}
            onMouseLeave={this.handleNavMouseLeave}
          >
            <NavLogoItem opened={isNavOpened} onClick={this.onLogoClick} />
            {mainModules.map(item => (
              <NavItem
                separator={!!item.separator}
                key={item.id}
                opened={isNavOpened}
                active={item.id == currentProduct.id}
                iconName={item.iconName}
                iconUrl={item.iconUrl}
                badgeNumber={item.notifications}
                onClick={item.onClick}
                onBadgeClick={e => {
                  this.toggleAside();
                  item.onBadgeClick(e);
                }}
                url={item.url}
              >
                {item.title}
              </NavItem>
            ))}
          </Nav>
        )}

        {isAsideAvailable && (
          <Aside visible={isAsideVisible} onClick={this.backdropClick}>
            {asideContent}
          </Aside>
        )}
      </>
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
    PropTypes.node
  ]),

  currentUser: PropTypes.object,
  homepage: PropTypes.string,
  isAuthenticated: PropTypes.bool,
  isLoaded: PropTypes.bool,
  isolateModules: PropTypes.array,
  mainModules: PropTypes.array,
  currentProduct: PropTypes.object,

  history: PropTypes.object,
  t: PropTypes.func,
  logout: PropTypes.func
};

NavMenu.defaultProps = {
  isBackdropVisible: false,
  isNavHoverEnabled: true,
  isNavOpened: false,
  isAsideVisible: false,

  currentUser: null
};

const NavMenuTranslationWrapper = withTranslation()(NavMenu);

const NavMenuWrapper = props => {
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
  language: PropTypes.string.isRequired
};

function mapStateToProps(state) {
  const { isAuthenticated, isLoaded, settings } = state.auth;
  const { defaultPage, homepage } = settings;

  return {
    isAuthenticated,
    isLoaded,
    homepage,
    defaultPage: defaultPage || "/",
    currentUser: getCurrentUser(state),
    currentProduct: getCurrentProduct(state),
    isolateModules: getIsolateModules(state),
    mainModules: getMainModules(state),
    language: getLanguage(state)
  };
}

export default connect(mapStateToProps, { logout })(withRouter(NavMenuWrapper));
