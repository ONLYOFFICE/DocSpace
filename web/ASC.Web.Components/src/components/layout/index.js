import React, { useState } from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components';
import Avatar from '../avatar'
import DropDown from '../drop-down'
import DropDownItem from '../drop-down-item'
import { Icons } from '../icons'

const logoSrc = "https://static.onlyoffice.com/studio/tag/10.0.0/skins/default/images/onlyoffice_logo/light_small_general.svg";

const size = {
  mobile: "375px",
  tablet: "768px",
  desktop: "1024px"
};

const device = {
  mobile: `(max-width: ${size.mobile})`,
  tablet: `(max-width: ${size.tablet})`,
  desktop: `(max-width: ${size.desktop})`
};

const Wrapper = styled.div`
  display: flex;

  @media ${device.tablet} {
    display: block;
  }
`;

const Backdrop = styled.div`
  background-color: #000000;
  opacity: 0.3;
  z-index: 100;
  width: 100vw;
  height: 100vh;
  position: fixed;
  display: none;

  @media ${device.tablet} {
    display: ${props => props.visible ? 'block' : 'none'};
  }
`;

const HeaderIcons = styled.div`
  display: flex;
  padding: 0 16px;
  align-items: center;
  width: 100px;
  justify-content: space-between;
  position: absolute;
  z-index: 300;
  right: 0;
  height: 56px;
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

const Navigation = styled.div`
  background-color: #0f4071;  
  z-index: 200;
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
    display: block;
  }
`;

const NavItem = styled.div`
  color: #ffffff;
  display: flex;
  min-width: 56px;
  min-height: 56px;
  align-items: center;
  padding: 0 16px;
`;

const NavItemText = styled.div`
  margin-left: 16px;
`;

const NavItemSeporator= styled.div`
  border-bottom: 1px solid #3E668D;
  margin: 0 16px;
`;

const Content = styled.div`
  min-height: 100vh;
  width: 100vw;
  z-index: 0;

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
      <HeaderIcons>
        <Icons.ChatIcon/>
        <Avatar size="small" role="user" onClick={() => { toggle(!opened); }} />
        <DropDown isOpen={opened}>
          <DropDownItem label="Profile"/>
          <DropDownItem label="About"/>
          <DropDownItem label="Log out"/>
        </DropDown>
      </HeaderIcons>
      <Header visible={visible}>
        <MenuItem>
          <NavItem onClick={() => { setState(!visible); }}>
            <Icons.MenuIcon/>
            {visible ? <NavItemText><img alt="ONLYOFFICE" src={logoSrc}/></NavItemText> : ""}
          </NavItem>
        </MenuItem>
        <Navigation visible={visible}>
          <NavItem onClick={() => { setState(!visible); }}>
            <Icons.MenuIcon/>
            {visible ? <NavItemText><img alt="ONLYOFFICE" src={logoSrc}/></NavItemText> : ""}
          </NavItem>
          <NavItemSeporator/>
          <NavItem><Icons.DocumentsIcon/>{visible ? <NavItemText>Documents</NavItemText> : ""}</NavItem>
          <NavItem><Icons.MailIcon/>{visible ? <NavItemText>Mail</NavItemText> : ""}</NavItem>
          <NavItem><Icons.CalendarEmptyIcon/>{visible ? <NavItemText>Calendar</NavItemText> : ""}</NavItem>
        </Navigation>
      </Header>
      <Content visible={visible}>
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