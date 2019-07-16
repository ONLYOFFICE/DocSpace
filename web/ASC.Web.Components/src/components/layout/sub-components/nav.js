import React from 'react'
import styled from 'styled-components'
import device from '../../device'
import { Scrollbars } from 'react-custom-scrollbars';

const backgroundColor = '#0F4071';

const StyledNav = styled.nav`
  background-color: ${backgroundColor};
  height: 100%;  
  left: 0;
  overflow-x: hidden;
  overflow-y: auto;
  position: fixed;
  top: 0;
  transition: width .3s ease-in-out;
  width: ${props => props.opened ? '240px' : '56px'};
  z-index: 200;

  @media ${device.tablet} {
    width: ${props => props.opened ? '240px' : '0'};
  }
`;

const Nav = props => <StyledNav opened={props.opened}>
  <Scrollbars style={{ width: props.opened ? 240 : 56 }} {...props}/>
</StyledNav>

export default Nav;