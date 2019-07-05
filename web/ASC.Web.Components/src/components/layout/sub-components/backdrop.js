import React from 'react'
import styled from 'styled-components';
import device from './device';

const StyledBackdrop = styled.div`
  background-color: rgba(0, 0, 0, 0.3);
  z-index: 100;
  width: 100vw;
  height: 100vh;
  position: fixed;
  display: none;

  @media ${device.tablet} {
    display: ${props => props.isNavigationOpen ? 'block' : 'none'};
  }
`;

const Backdrop = props => <StyledBackdrop {...props}/>

export default Backdrop;