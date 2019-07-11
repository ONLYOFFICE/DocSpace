import React from 'react'
import styled from 'styled-components';
import { Icons } from '../../icons';

const LogoItem = styled.div`
  display: flex;
  min-width: 56px;
  min-height: 56px;
  align-items: center;
  padding: 0 16px;
  cursor: pointer;
`;

const NavLogoItem = props => {
  const navLogoIconStyle = {
    display: props.opened ? 'none' : 'block'
  };

  const navLogoOpenedIconStyle = {
    display: props.opened ? 'block' : 'none',
    maxHeight: '24px',
    width: 'auto'
  };

  return (
    <LogoItem>
      <Icons.NavLogoIcon style={navLogoIconStyle} onClick={props.onClick}/>
      <Icons.NavLogoOpenedIcon style={navLogoOpenedIconStyle} onClick={props.onClick}/>
    </LogoItem>
  );
};

export default NavLogoItem;