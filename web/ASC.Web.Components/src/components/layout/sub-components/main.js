import React from 'react'
import styled from 'styled-components'
import device from '../../device'

const StyledMain = styled.main` 
  height: 100vh;
  padding: 0 0 0 56px;
  width: 100vw;
  z-index: 0;
  display: flex;
  flex-direction: row;

  @media ${device.tablet} {
    padding: 56px 0 0 0;
  }
`;

const Main = props => <StyledMain {...props}/>

export default Main;