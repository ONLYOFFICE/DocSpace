import React from 'react'
import styled from 'styled-components'

const StyledBackdrop = styled.div`
  background-color: rgba(0, 0, 0, 0.3);
  display: ${props => props.visible ? 'block' : 'none'};
  height: 100vh;
  position: fixed;
  width: 100vw;
  z-index: 100;
`;

const Backdrop = props => <StyledBackdrop {...props}/>

export default Backdrop;