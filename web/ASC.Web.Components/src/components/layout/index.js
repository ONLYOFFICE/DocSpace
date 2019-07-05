import React, { useState } from 'react'
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

    let chatModule = null,
      currentModule = null,
      totalNotifications = 0,
      index = props.availableModules.length,
      item = null;

    while (index) {
      index--;
      item = props.availableModules[index];

      if(item.seporator) continue;

      if(item.id == props.currentModuleId)
        currentModule = item;

      if(item.id == props.chatModuleId)
        chatModule = item;
      else
        totalNotifications+=item.notifications;
    }

    this.state = {
      chatModule: chatModule,
      chatModuleId: props.chatModuleId,
      currentModule: currentModule,
      currentUser: props.currentUser || null,
      totalNotifications: totalNotifications,
      isNavigationOpen: props.isNavigationOpen,
      availableModules: props.availableModules || []
    };
  };

  toggle = () => {
    console.log(this.state.isNavigationOpen);
    this.setState({ isNavigationOpen: !this.state.isNavigationOpen });
    console.log(this.state.isNavigationOpen);
  };

  render() {
    return (
      <Wrapper>
        <Backdrop isNavigationOpen={this.state.isNavigationOpen} onClick={this.toggle}/>
        <Header isNavigationOpen={this.state.isNavigationOpen}>
          <HeaderIcons chatModule={this.state.chatModule} currentUser={this.state.currentUser}/>
          <HeaderMenu totalNotifications={this.state.totalNotifications} onClick={this.toggle} currentModule={this.state.currentModule}/>
          <Navigation isNavigationOpen={this.state.isNavigationOpen}>
            <NavLogoItem isNavigationOpen={this.state.isNavigationOpen} onClick={this.toggle}/>
            {
              this.state.availableModules
                .filter(item => item.id != this.state.chatModuleId)
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
                    {item.name}
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
  isNavigationOpen: PropTypes.bool,
  currentUser: PropTypes.object,
  availableModules: PropTypes.array,
  currentModuleId: PropTypes.string,
  chatModuleId: PropTypes.string
}

Layout.defaultProps = {
  isNavigationOpen: false
}

export default Layout