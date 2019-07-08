import React from 'react'
import styled from 'styled-components';
import device from './device';
import { Icons } from '../../icons';

const logoSrc = "https://static.onlyoffice.com/studio/tag/10.0.0/skins/default/images/onlyoffice_logo/light_small_general.svg";

const LogoItem = styled.div`
  display: flex;
  min-width: 56px;
  min-height: 56px;
  align-items: center;
  padding: 0 16px;

  .logoitem-img {
    margin-left: 16px;
  }

  @media ${device.tablet} {
    .logoitem-menu {
      display: none; 
    }

    .logoitem-img {
      margin-left: 0;
    }
  }
`;

const NavLogoItem = props => {
  return (
    <LogoItem>
      <Icons.MenuIcon className="logoitem-menu" onClick={props.onClick}/>
      { props.isNavigationOpen ? <img className="logoitem-img" alt="ONLYOFFICE" src={logoSrc}/> : "" }
    </LogoItem>
  );
};

export default NavLogoItem;