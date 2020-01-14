import React from "react";
import PropTypes from "prop-types";
import { Backdrop, Aside } from "asc-web-components";
import HeaderComponent from "./sub-components/header";
import Nav from "./sub-components/nav";
import Main from "./sub-components/main";
import HeaderNav from "./sub-components/header-nav";
import NavLogoItem from "./sub-components/nav-logo-item";
import NavItem from "./sub-components/nav-item";

class Layout extends React.Component {
  constructor(props) {
    super(props);
    this.timeout = null;
    this.state = this.mapPropsToState(props);
  }

  /*shouldComponentUpdate() {
    return false;
  }*/

  componentDidUpdate(prevProps) {
    //console.log("Layout componentDidUpdate");
    let currentHash = this.getPropsHash(this.props);
    let prevHash = this.getPropsHash(prevProps);
    if (currentHash !== prevHash) {
      console.log("Layout componentDidUpdate hasChanges");
      this.setState(this.mapPropsToState(this.props));
    }
  }

  getPropsHash = props => {
    let hash = "";
    if (props.currentModuleId) {
      hash += props.currentModuleId;
    }
    if (props.currentUser) {
      const { id, displayName, email, avatarSmall } = props.currentUser;
      hash += id + displayName + email + avatarSmall;
    }
    if (props.availableModules) {
      for (let i = 0, l = props.availableModules.length; i < l; i++) {
        let item = props.availableModules[i];
        hash += item.id + item.notifications;
      }
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
      isHeaderAvailable: mainModules.length > 0,
      isNavAvailable: mainModules.length > 0,
      isAsideAvailable: !!props.asideContent,

      isBackdropVisible: props.isBackdropVisible,
      isNavHoverEnabled: props.isNavHoverEnabled,
      isNavOpened: props.isNavOpened,
      isAsideVisible: props.isAsideVisible,

      onLogoClick: props.onLogoClick,
      asideContent: props.asideContent,

      currentUser: props.currentUser,
      currentUserActions: props.currentUserActions,

      availableModules: props.availableModules,
      isolateModules: isolateModules,
      mainModules: mainModules,

      currentModule: currentModule,
      currentModuleId: props.currentModuleId,

      totalNotifications: totalNotifications
    };

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
  }

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

  render() {
    //console.log("Layout render");

    return (
      <>
        {this.state.isBackdropAvailable && (
          <Backdrop
            visible={this.state.isBackdropVisible}
            onClick={this.backdropClick}
          />
        )}
        {this.state.isHeaderNavAvailable && (
          <HeaderNav
            modules={this.state.isolateModules}
            user={this.state.currentUser}
            userActions={this.state.currentUserActions}
          />
        )}
        {this.state.isHeaderAvailable && (
          <HeaderComponent
            badgeNumber={this.state.totalNotifications}
            onClick={this.showNav}
            currentModule={this.state.currentModule}
          />
        )}
        {this.state.isNavAvailable && (
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
        {this.state.isAsideAvailable && (
          <Aside
            visible={this.state.isAsideVisible}
            onClick={this.backdropClick}
          >
            {this.state.asideContent}
          </Aside>
        )}
        <Main fullscreen={!this.state.isNavAvailable}>
          {this.props.children}
        </Main>
      </>
    );
  }
}

Layout.propTypes = {
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
  currentModuleId: PropTypes.string
};

Layout.defaultProps = {
  isBackdropVisible: false,
  isNavHoverEnabled: true,
  isNavOpened: false,
  isAsideVisible: false,

  currentUser: null,
  currentUserActions: [],
  availableModules: []
};

export default Layout;
