import React from 'react'
import PropTypes from 'prop-types'
import Backdrop from './sub-components/backdrop'
import Header from './sub-components/header'
import Nav from './sub-components/nav'
import Aside from './sub-components/aside'
import Main from './sub-components/main'
import HeaderNav from './sub-components/header-nav'
import NavLogoItem from './sub-components/nav-logo-item'
import NavItem from './sub-components/nav-item'

class Layout extends React.Component {

  constructor(props) {
    super(props);

    let currentModule = null,
        isolateModules = [],
        mainModules = [],
        totalNotifications = 0,
        item = null;

    for (let i=0, l=props.availableModules.length; i<l; i++)
    {
      item = props.availableModules[i];

      if (item.id == props.currentModuleId)
        currentModule = item;

      if (item.isolateMode) {
        isolateModules.push(item);
      } else {
        mainModules.push(item);
        if (item.seporator) continue;
        totalNotifications+=item.notifications;
      }
    }

    this.state = {
      isBackdropOpen: props.isBackdropOpen,
      isNavigationHoverEnabled: props.isNavigationHoverEnabled,
      isNavigationOpen: props.isNavigationOpen,
      isAsideOpen: props.isAsideOpen,

      onLogoClick: props.onLogoClick,
      asideContent: props.asideContent,

      currentUser: props.currentUser || null,
      currentUserActions: props.currentUserActions || [],

      availableModules: props.availableModules || [],
      isolateModules: isolateModules,
      mainModules: mainModules,

      currentModule: currentModule,
      currentModuleId: props.currentModuleId,

      totalNotifications: totalNotifications
    };
  };

  backdropClick = () => {
    this.setState({
      isBackdropOpen: false,
      isNavigationOpen: false,
      isAsideOpen: false,
      isNavigationHoverEnabled: !this.state.isNavigationHoverEnabled
    });
  };

  showNav = () => {
    this.setState({
      isBackdropOpen: true,
      isNavigationOpen: true,
      isAsideOpen: false,
      isNavigationHoverEnabled: false
    });
  };

  handleNavHover = () => {
    if(!this.state.isNavigationHoverEnabled) return;
    
    this.setState({
      isBackdropOpen: false,
      isNavigationOpen: !this.state.isNavigationOpen,
      isAsideOpen: false
    });
  }

  toggleAside = () => {
    this.setState({
      isBackdropOpen: true,
      isNavigationOpen: false,
      isAsideOpen: true,
      isNavigationHoverEnabled: false
    });
  };

  render() {
    return (
      <>
        <Backdrop isOpen={this.state.isBackdropOpen} onClick={this.backdropClick}/>
        <HeaderNav
          modules={this.state.isolateModules}
          user={this.state.currentUser}
          userActions={this.state.currentUserActions}
        />
        <Header
          badgeNumber={this.state.totalNotifications}
          onClick={this.showNav}
          moduleTitle={this.state.currentModule.title}
        />
        <Nav
          isOpen={this.state.isNavigationOpen}
          onMouseEnter={this.handleNavHover}
          onMouseLeave={this.handleNavHover}
        >
          <NavLogoItem
            isOpen={this.state.isNavigationOpen}
            onClick={this.state.onLogoClick}
          />
          {
            this.state.mainModules.map(item => 
              <NavItem
                seporator={!!item.seporator}
                key={item.id}
                isOpen={this.state.isNavigationOpen}
                active={item.id == this.state.currentModuleId}
                iconName={item.iconName}
                badgeNumber={item.notifications}
                onClick={item.onClick}
                onBadgeClick={(e)=>{item.onBadgeClick(e); this.toggleAside();}}
              >
                {item.title}
              </NavItem>
            )
          }
        </Nav>
        <Aside isOpen={this.state.isAsideOpen} onClick={this.backdropClick}>{this.state.asideContent}</Aside>
        <Main>{this.props.children}</Main>
      </>
    );
  }
}

Layout.propTypes = {
  isBackdropOpen: PropTypes.bool,
  isNavigationHoverEnabled: PropTypes.bool,
  isNavigationOpen: PropTypes.bool,
  isAsideOpen: PropTypes.bool,

  onLogoClick: PropTypes.func,
  asideContent: PropTypes.oneOfType([PropTypes.arrayOf(PropTypes.node), PropTypes.node]),

  currentUser: PropTypes.object,
  currentUserActions: PropTypes.array,
  availableModules: PropTypes.array,
  currentModuleId: PropTypes.string
}

Layout.defaultProps = {
  isBackdropOpen: false,
  isNavigationHoverEnabled: true,
  isNavigationOpen: false,
  isAsideOpen: false
}

export default Layout