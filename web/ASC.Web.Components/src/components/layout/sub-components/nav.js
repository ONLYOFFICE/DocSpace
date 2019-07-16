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

const renderNavThumbVertical = ({ style, ...props }) => 
  <div {...props} style={{ ...style, backgroundColor: 'rgba(256, 256, 256, 0.2)', width: '2px', marginLeft: '2px', borderRadius: 'inherit'}}/>

const Nav = (props) => { 
  const { opened, onMouseEnter, onMouseLeave, children } = props;

  return (
    <StyledNav opened={opened} onMouseEnter={onMouseEnter} onMouseLeave={onMouseLeave}>
      <Scrollbars renderThumbVertical={renderNavThumbVertical} style={{ width: opened ? 240 : 56 }}>
        {children}
      </Scrollbars>
    </StyledNav>
  );
}

export default Nav;