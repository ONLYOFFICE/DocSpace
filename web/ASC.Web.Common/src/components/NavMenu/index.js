import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { Backdrop, Toast /*Aside*/, utils } from "asc-web-components";
import Header from "./sub-components/header";
import Nav from "./sub-components/nav";
import HeaderNav from "./sub-components/header-nav";
import NavLogoItem from "./sub-components/nav-logo-item";
import NavItem from "./sub-components/nav-item";
//import HeaderUnAuth from "./sub-components/header-unauth";

import { withTranslation } from "react-i18next";
import i18n from "./i18n";
import { connect } from "react-redux";
import { AUTH_KEY } from "../../constants";

import { withRouter } from "react-router";
import { logout } from "../../store/auth/actions";

class NavMenu extends React.Component {
  constructor(props) {
    super(props);
    this.timeout = null;
    this.state = this.mapPropsToState(props);
  }

  /*shouldComponentUpdate() {
    return false;
  }*/

  componentDidUpdate(prevProps) {
    //console.log("NavMenu componentDidUpdate");
    let currentHash = this.getPropsHash(this.props);
    let prevHash = this.getPropsHash(prevProps);
    if (currentHash !== prevHash) {
      //console.log("NavMenu componentDidUpdate hasChanges");
      this.setState(this.mapPropsToState(this.props));
    }
  }

  getPropsHash = props => {
    let hash = "";
    if (props.currentModuleId) {
      hash += props.currentModuleId;
    }
    if (props.currentUser) {
      const {
        id,
        displayName,
        email,
        avatarSmall,
        cultureName
      } = props.currentUser;
      hash += id + displayName + email + avatarSmall + cultureName;
    }
    if (props.availableModules) {
      for (let i = 0, l = props.availableModules.length; i < l; i++) {
        let item = props.availableModules[i];
        hash += item.id + item.notifications;
      }
    }
    if (props.availableModules && this.state.availableModules) {
      hash += utils.array.isArrayEqual(
        this.state.availableModules,
        props.availableModules
      );
    }
    return hash;
  };

  mapPropsToState = props => {
    let currentModule = null,
      isolateModules = [],
      mainModules = [],
      totalNotifications = 0,
      item = null;

    for (let i = 0, l = props.availableModules.length; i < l; i++) {
      item = props.availableModules[i];

      if (item.id == props.currentModuleId) currentModule = item;

      if (item.isolateMode) {
        isolateModules.push(item);
      } else {
        mainModules.push(item);
        if (item.separator) continue;
        totalNotifications += item.notifications;
      }
    }

    const newState = {
      isBackdropAvailable: mainModules.length > 0 || !!props.asideContent,
      isHeaderNavAvailable: isolateModules.length > 0 || !!props.currentUser,
      authHeader: localStorage.getItem(AUTH_KEY) ? true : false,
      isNavAvailable: mainModules.length > 0,
      isAsideAvailable: !!props.asideContent,

      isBackdropVisible: props.isBackdropVisible,
      isNavHoverEnabled: props.isNavHoverEnabled,
      isNavOpened: props.isNavOpened,
      isAsideVisible: props.isAsideVisible,

      onLogoClick: undefined,
      asideContent: props.asideContent,

      currentUser: props.currentUser,
      currentUserActions: undefined,

      availableModules: props.availableModules,
      isolateModules: isolateModules,
      mainModules: mainModules,

      currentModule: currentModule,
      currentModuleId: props.currentModuleId,

      totalNotifications: totalNotifications
    };

    const { hasChanges } = props;

    if (hasChanges) {
      const { t, currentUser } = props;

      const isUserDefined =
        Object.entries(currentUser).length > 0 &&
        currentUser.constructor === Object;

      const userActionProfileView = {
        key: "ProfileBtn",
        label: t("Profile"),
        onClick: this.onProfileClick,
        url: "/products/people/view/@self"
      };

      const currentUserActions = [
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

      isUserDefined && currentUserActions.unshift(userActionProfileView);

      newState.currentUserActions = currentUserActions;
      newState.onLogoClick = this.onLogoClick;
    }

    return newState;
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
    const { history, settings } = this.props;
    if (settings.homepage == "/products/people") {
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

  onLogoClick = () => {
    window.open("/", "_self");
  };

  render() {
    //console.log("NavMenu render");

    return (
      <>
        <Toast />
        {this.state.isBackdropAvailable && (
          <Backdrop
            visible={this.state.isBackdropVisible}
            onClick={this.backdropClick}
          />
        )}

        <HeaderNav
          modules={this.state.isolateModules}
          user={this.state.currentUser}
          userActions={this.state.currentUserActions}
        />

        <Header
          badgeNumber={this.state.totalNotifications}
          onClick={this.showNav}
          onLogoClick={this.state.onLogoClick}
          currentModule={this.state.currentModule}
          defaultPage={this.props.defaultPage}
        />

        <Nav
          opened={this.state.isNavOpened}
          onMouseEnter={this.handleNavMouseEnter}
          onMouseLeave={this.handleNavMouseLeave}
        >
          <NavLogoItem
            opened={this.state.isNavOpened}
            onClick={this.state.onLogoClick}
          />
          {this.state.mainModules.map(item => (
            <NavItem
              separator={!!item.separator}
              key={item.id}
              opened={this.state.isNavOpened}
              active={item.id == this.state.currentModuleId}
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

        {/* {this.state.isAsideAvailable && (
          <Aside
            visible={this.state.isAsideVisible}
            onClick={this.backdropClick}
          >
            {this.state.asideContent}
          </Aside>
        )} */}
      </>
    );
  }
}

NavMenu.propTypes = {
  isBackdropVisible: PropTypes.bool,
  isNavHoverEnabled: PropTypes.bool,
  isNavOpened: PropTypes.bool,
  isAsideVisible: PropTypes.bool,

  onLogoClick: PropTypes.func,
  asideContent: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node
  ]),
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node
  ]),

  currentUser: PropTypes.object,
  currentUserActions: PropTypes.array,
  availableModules: PropTypes.array,
  currentModuleId: PropTypes.string,
  defaultPage: PropTypes.string,
  t: PropTypes.func
};

NavMenu.defaultProps = {
  isBackdropVisible: false,
  isNavHoverEnabled: true,
  isNavOpened: false,
  isAsideVisible: false,

  currentUser: null,
  currentUserActions: [],
  availableModules: []
};

const NavMenuTranslationWrapper = withTranslation()(NavMenu);

const NavMenuWrapper = props => {
  const { language } = props;

  useEffect(() => {
    i18n.changeLanguage(language);
  }, [language]);

  return <NavMenuTranslationWrapper i18n={i18n} {...props} />;
};

NavMenuWrapper.propTypes = {
  language: PropTypes.string.isRequired
};

const getSeparator = id => {
  return {
    separator: true,
    id: id
  };
};

const toModuleWrapper = (item, iconName) => {
  return {
    id: item.id,
    title: item.title,
    iconName: item.iconName || iconName || "PeopleIcon", //TODO: Change to URL
    iconUrl: item.iconUrl,
    notifications: 0,
    url: item.link,
    onClick: e => {
      if (e) {
        window.open(item.link, "_self");
        e.preventDefault();
      }
    },
    onBadgeClick: e => console.log(iconName + " Badge Clicked", e)
  };
};

const getCustomModules = isAdmin => {
  if (!isAdmin) {
    return [];
  } // Temporarily hiding the settings module

  /*  const separator = getSeparator("nav-modules-separator");
    const settingsModuleWrapper = toModuleWrapper(
      {
        id: "settings",
        title: i18n.t('Settings'),
        link: "/settings"
      },
      "SettingsIcon"
    );
  
    return [separator, settingsModuleWrapper];*/ return [];
};

const getAvailableModules = (modules, currentUser) => {
  if (!modules.length) {
    return [];
  }

  const isUserAdmin = currentUser.isAdmin;
  const customModules = getCustomModules(isUserAdmin);
  const separator = getSeparator("nav-products-separator");
  const products = modules.map(m => toModuleWrapper(m));

  return [separator, ...products, ...customModules];
};

function mapStateToProps(state) {
  const { user, isAuthenticated, isLoaded, modules, settings } = state.auth;
  const { defaultPage, currentProductId } = settings;

  return {
    hasChanges: isAuthenticated && isLoaded,
    availableModules: getAvailableModules(modules, user),
    currentUser: user,
    currentModuleId: currentProductId,
    settings: settings,
    modules: modules,
    defaultPage: defaultPage || "/",
    language: user.cultureName || settings.culture
  };
}

export default connect(mapStateToProps, { logout })(withRouter(NavMenuWrapper));
