import React from 'react'
import styled from 'styled-components'
import Scrollbar from '../../scrollbar';

const StyledAside = styled.aside`
  background-color: #fff;
  height: 100%;
  overflow-x: hidden;
  overflow-y: auto;
  position: fixed;
  right: 0;
  top: 0;
  transform: translateX(${props => props.visible ? '0' : '240px'});
  transition: transform .3s ease-in-out;
  width: 240px;
  z-index: 400;
`;

const Aside = (props) => { 
  //console.log("Aside render");
  const { visible, children } = props;

  return (
    <StyledAside visible={visible}>
      <Scrollbar>
        {children}
      </Scrollbar>
    </StyledAside>
  );
}

export default Aside;