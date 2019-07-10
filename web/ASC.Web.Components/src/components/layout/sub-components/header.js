import React from 'react'
import styled from 'styled-components'
import device from './device'
import NavItem from './nav-item'
import { Text } from '../../text'

const backgroundColor = '#0F4071';

const StyledHeader = styled.header`
  align-items: center;  
  background-color: ${backgroundColor};
  display: none;
  z-index: 200;
  position: absolute;
  width: 100vw;

  @media ${device.tablet} {
    display: flex;
  }
`;

const HeaderText = styled(Text.MenuHeader)`
  margin: 0;
  color: #FFFFFF;
`;

const Header = props => <StyledHeader>
  <NavItem
    isOpen={false}
    iconName="MenuIcon"
    badgeNumber={props.badgeNumber}
    onClick={props.onClick}
  />
  <HeaderText>{props.moduleTitle}</HeaderText>
</StyledHeader>

export default Header;