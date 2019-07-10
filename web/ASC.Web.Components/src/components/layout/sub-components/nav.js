import React from 'react'
import styled from 'styled-components'
import device from './device'

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
  width: ${props => props.isOpen ? '240px' : '56px'};
  z-index: 200;

  @media ${device.tablet} {
    width: ${props => props.isOpen ? '240px' : '0'};
  }
`;

const Nav = props => <StyledNav {...props}/>

export default Nav;