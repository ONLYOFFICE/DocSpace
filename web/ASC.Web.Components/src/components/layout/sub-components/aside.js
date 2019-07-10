import React from 'react'
import styled from 'styled-components'

const StyledAside = styled.aside`
  background-color: #fff;
  height: 100%;
  overflow-x: hidden;
  overflow-y: auto;
  position: fixed;
  right: 0;
  top: 0;
  transform: translateX(${props => props.isOpen ? '0' : '240px'});
  transition: transform .3s ease-in-out;
  width: 240px;
  z-index: 400;
`;

const Aside = props => <StyledAside {...props}/>

export default Aside;