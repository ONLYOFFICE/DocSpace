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
  min-width: ${props => props.visible ? '240px' : '56px'};

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
  width: 100px;
  justify-content: space-between;
  position: absolute;
  right: 0;
  height: 56px;
  z-index: 300;
`;

const Navigation = styled.div`
  background-color: #0f4071;  
  width: ${props => props.visible ? '240px' : 'auto'};
  position: fixed;
  min-height: 100vh;
  overflow-x: hidden;
  overflow-y: auto;

  @media ${device.tablet} {
    display: ${props => props.visible ? 'block' : 'none'};
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

  const [visible, setState] = useState(props.visible);
  const [opened, toggle] = useState(props.opened);

  return (
    <Wrapper>
      <Backdrop visible={visible} onClick={() => { setState(false); toggle(false); }}/>
      <Header visible={visible}>
        <HeaderIcons>
          <BadgedIcon iconName="ChatIcon" badgeNumber={10} onClick={() => {}}/>
          <Avatar size="small" role="user" onClick={() => { toggle(!opened); }} />
          <DropDown isUserPreview withArrow direction='right' isOpen={opened}>
            <DropDownItem isUserPreview role='user' source='' userName='Jane Doe' label='janedoe@gmail.com'/>
            <DropDownItem label="Profile"/>
            <DropDownItem label="About"/>
            <DropDownItem label="Log out"/>
          </DropDown>
        </HeaderIcons>
        <MenuItem>
          <BadgedIcon iconName="MenuIcon" badgeNumber={10} onClick={() => { setState(!visible); }}/>
          <MenuHeader>Documents</MenuHeader>
        </MenuItem>
        <Navigation visible={visible}>
          <LogoItem>
            <BadgedIcon className="logoitem-menu" iconName="MenuIcon" badgeNumber={10} onClick={() => { setState(!visible); }}/>
            { visible ? <img className="logoitem-img" alt="ONLYOFFICE" src={logoSrc}/> : "" }
          </LogoItem>
          <NavItem seporator={true}></NavItem>
          <NavItem active={true} opened={visible} iconName="DocumentsIcon" badgeNumber={3} onClick={() => {}}>Documents</NavItem>
          <NavItem active={false} opened={visible} iconName="MailIcon" badgeNumber={7} onClick={() => {}}>Mail</NavItem>
        </Navigation>
      </Header>
      <Content>
        {props.children}
      </Content>
    </Wrapper>
  );
}

Layout.propTypes = {
  visible: PropTypes.bool
}

Layout.defaultProps = {
  visible: false
}

export default Layout