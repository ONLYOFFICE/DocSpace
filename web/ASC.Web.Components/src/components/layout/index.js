import React from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components';
import device from './sub-components/device'
import Backdrop from './sub-components/backdrop'
import HeaderIcons from './sub-components/header-icons'
import HeaderMenu from './sub-components/header-menu'
import NavLogoItem from './sub-components/nav-logo-item'
import NavItem from './sub-components/nav-item'


const Wrapper = styled.div`
  display: flex;

  @media ${device.tablet} {
    display: block;
  }
`;

const Header = styled.div`
  background-color: #0f4071;
  min-height: 100vh;
  z-index: 200;
  min-width: ${props => props.isNavigationOpen ? '240px' : '56px'};

  @media ${device.tablet} {
    min-height: 56px;
    position: absolute;
    width: 100%;
  }
`;

const Navigation = styled.div`
  background-color: #0f4071;  
  width: ${props => props.isNavigationOpen ? '240px' : 'auto'};
  position: fixed;
  min-height: 100vh;
  overflow-x: hidden;
  overflow-y: auto;

  @media ${device.tablet} {
    display: ${props => props.isNavigationOpen ? 'block' : 'none'};
    top: 0px;
    width: 240px;
  }
`;

const Content = styled.div`
  min-height: 100vh;
  width: 100vw;

  @media ${device.tablet} {
    padding-top: 56px;
  }
`;

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
      isNavigationHoverEnabled: props.isNavigationHoverEnabled,
      isNavigationOpen: props.isNavigationOpen,
      currentUser: props.currentUser || null,
      currentUserActions: props.currentUserActions || [],
      currentModule: currentModule,
      currentModuleId: props.currentModuleId,
      availableModules: props.availableModules || [],
      isolateModules: isolateModules,
      mainModules: mainModules,
      totalNotifications: totalNotifications
    };
  };

  toggleTabletNav = () => {
    this.setState({
      isNavigationOpen: !this.state.isNavigationOpen,
      isNavigationHoverEnabled: !this.state.isNavigationHoverEnabled
    });
  };

  handleDesctopNavHover = () => {
    if(!this.state.isNavigationHoverEnabled) return;
    this.setState({
      isNavigationOpen: !this.state.isNavigationOpen
    });
  }

  toggleDesctopNav = () => {
    this.setState({
      isNavigationHoverEnabled: !this.state.isNavigationHoverEnabled
    });
  };

  render() {
    return (
      <Wrapper>
        <Backdrop isNavigationOpen={this.state.isNavigationOpen} onClick={this.toggleTabletNav}/>
        <Header isNavigationOpen={this.state.isNavigationOpen}>
          <HeaderIcons
            modules={this.state.isolateModules}
            user={this.state.currentUser}
            userActions={this.state.currentUserActions}
          />
          <HeaderMenu
            totalNotifications={this.state.totalNotifications}
            onClick={this.toggleTabletNav}
            currentModule={this.state.currentModule}
          />
          <Navigation
            isNavigationOpen={this.state.isNavigationOpen}
            onMouseEnter={this.handleDesctopNavHover}
            onMouseLeave={this.handleDesctopNavHover}
          >
            <NavLogoItem
              isNavigationOpen={this.state.isNavigationOpen}
              onClick={this.toggleDesctopNav}
            />
            {
              this.state.mainModules
                .map(item => 
                  <NavItem
                    seporator={!!item.seporator}
                    key={item.id}
                    isOpen={this.state.isNavigationOpen}
                    active={item.id == this.state.currentModuleId}
                    iconName={item.iconName}
                    badgeNumber={item.notifications}
                    onClick={item.onClick}
                    onBadgeClick={item.onBadgeClick}
                  >
                    {item.title}
                  </NavItem>
                )
            }
          </Navigation>
        </Header>
        <Content>{this.props.children}</Content>
      </Wrapper>
    );
  }
}

Layout.propTypes = {
  isNavigationHoverEnabled: PropTypes.bool,
  isNavigationOpen: PropTypes.bool,
  currentUser: PropTypes.object,
  currentUserActions: PropTypes.array,
  availableModules: PropTypes.array,
  currentModuleId: PropTypes.string
}

Layout.defaultProps = {
  isNavigationHoverEnabled: true,
  isNavigationOpen: false
}

export default Layout