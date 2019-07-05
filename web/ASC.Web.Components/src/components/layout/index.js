import React, { useState } from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components';
import Avatar from '../avatar'
import DropDown from '../drop-down'
import DropDownItem from '../drop-down-item'
import Badge from '../badge'
import { Icons } from '../icons'
import { Text } from '../text'

import device from './sub-components/device'
import Backdrop from './sub-components/backdrop'
import BadgedIcon from './sub-components/badged-icon'
import NavItem from './sub-components/nav-item'
import ProfileActions from './sub-components/profile-actions'


const logoSrc = "https://static.onlyoffice.com/studio/tag/10.0.0/skins/default/images/onlyoffice_logo/light_small_general.svg";

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

const HeaderIcons = styled.div`
  display: flex;
  padding: 0 16px;
  align-items: center;
  position: absolute;
  right: 0;
  height: 56px;
  z-index: 300;

  & > div {
    margin-left: 16px;
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

const MenuItem = styled.div`
  display: none;  

  @media ${device.tablet} {
    display: flex;
    padding: 16px;
    height: 56px;
    align-items: center;
  }
`;

const MenuHeader = styled(Text.MenuHeader)`
  margin: 0 0 0 16px;
  color: #FFFFFF;
`;

const LogoItem = styled.div`
  display: flex;
  min-width: 56px;
  min-height: 56px;
  align-items: center;
  padding: 0 16px;

  .logoitem-img {
    margin-left: 16px;
  }

  @media ${device.tablet} {
    .logoitem-menu {
      display: none; 
    }

    .logoitem-img {
      margin-left: 0;
    }
  }
`;

const Content = styled.div`
  min-height: 100vh;
  width: 100vw;

  @media ${device.tablet} {
    padding-top: 56px;
  }
`;

const Layout = props => {

  var chatModule = null,
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

  const [isNavigationOpen, toggle] = useState(props.isNavigationOpen);

  return (
    <Wrapper>
      <Backdrop isNavigationOpen={isNavigationOpen} onClick={() => { toggle(false); }}/>
      <Header isNavigationOpen={isNavigationOpen}>
        <HeaderIcons>
          { 
            chatModule && <BadgedIcon
              key={chatModule.id}
              iconName={chatModule.iconName}
              badgeNumber={chatModule.notifications}
              onClick={chatModule.onClick}
              onBadgeClick={chatModule.onBadgeClick}
            />
          }
          { props.currentUser && <ProfileActions {...props.currentUser}/> }
        </HeaderIcons>
        <MenuItem>
          <BadgedIcon
            iconName="MenuIcon"
            badgeNumber={totalNotifications}
            onClick={() => { toggle(!isNavigationOpen); }}
          />
          <MenuHeader>{currentModule && currentModule.name}</MenuHeader>
        </MenuItem>
        <Navigation isNavigationOpen={isNavigationOpen}>
          <LogoItem>
            <BadgedIcon
              className="logoitem-menu"
              iconName="MenuIcon"
              onClick={() => { toggle(!isNavigationOpen); }}
            />
            { isNavigationOpen ? <img className="logoitem-img" alt="ONLYOFFICE" src={logoSrc}/> : "" }
          </LogoItem>
          {
            props.availableModules
              .filter(item => item.id != props.chatModuleId)
              .map(item => 
                <NavItem
                  seporator={!!item.seporator}
                  key={item.id}
                  isOpen={isNavigationOpen}
                  active={item.id == props.currentModuleId}
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
      <Content>{props.children}</Content>
    </Wrapper>
  );
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