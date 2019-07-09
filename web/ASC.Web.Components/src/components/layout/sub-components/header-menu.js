import React from 'react'
import styled from 'styled-components';
import BadgedIcon from './badged-icon'
import device from './device'
import { Text } from '../../text'

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

const HeaderMenu = props =>
  <MenuItem>
    <BadgedIcon
      iconName="MenuIcon"
      badgeNumber={props.totalNotifications}
      onClick={props.onClick}
    />
    <MenuHeader>{props.currentModule && props.currentModule.title}</MenuHeader>
  </MenuItem>

export default HeaderMenu;