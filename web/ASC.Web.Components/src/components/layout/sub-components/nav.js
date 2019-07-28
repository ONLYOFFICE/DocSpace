import React from 'react'
import styled from 'styled-components'
import device from '../../device'
import Scrollbar from '../../scrollbar';

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

const Nav = React.memo(props => { 
  console.log("Nav render");
  const { opened, onMouseEnter, onMouseLeave, children } = props;

  return (
    <StyledNav opened={opened} onMouseEnter={onMouseEnter} onMouseLeave={onMouseLeave}>
      <Scrollbar stype="smallWhite" style={{ width: opened ? 240 : 56 }}>
        {children}
      </Scrollbar>
    </StyledNav>
  );
});

export default Nav;