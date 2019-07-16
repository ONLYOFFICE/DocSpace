import React from 'react'
import styled from 'styled-components'
import { Scrollbars } from 'react-custom-scrollbars';

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

const renderAsideThumbVertical = ({ style, ...props }) => 
  <div {...props} style={{ ...style, backgroundColor: 'rgba(0, 0, 0, 0.1)', width: '2px', marginLeft: '2px', borderRadius: 'inherit'}}/>

const Aside = props => <StyledAside visible={props.visible}>
  <Scrollbars
    renderThumbVertical={renderAsideThumbVertical}
    {...props}
  />
</StyledAside>

export default Aside;