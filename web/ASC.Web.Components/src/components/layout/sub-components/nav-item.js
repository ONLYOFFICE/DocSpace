import React from 'react'
import styled, {css} from 'styled-components'
import Badge from '../../badge'
import { Icons } from '../../icons'

const baseColor = '#7A95B0',
      activeColor = '#FFFFFF';

const NavItemSeporator = styled.div`
  border-bottom: 1px solid ${baseColor};
  margin: 0 16px;
`;

const NavItemWrapper = styled.div`
  display: flex;
  min-width: 56px;
  min-height: 56px;
  align-items: center;
  padding: 0 16px;
  cursor: pointer;
  position: relative;
`;

const NavItemLabel = styled.div`
  font-weight: bold;
  font-size: 16px;  
  margin: 0 auto 0 16px;
  overflow: hidden;
  text-overflow: ellipsis;
  color: ${props => props.color};
  display: ${props => props.opened ? 'block' : 'none'};
`;

const badgeCss = css`
  position: absolute;
  top: 10px;
  right: 10px;
`;

const NavItemBadge = styled(Badge)`
  ${props => props.opened ? "" : badgeCss}
`;

const NavItem = props => {
  //console.log("NavItem render");
  const { seporator, opened, active, iconName, children, badgeNumber, onClick, onBadgeClick } = props;
  const color = active ? activeColor : baseColor;

  return (
    seporator
    ? <NavItemSeporator/>
    : <NavItemWrapper onClick={onClick}>
        {React.createElement(Icons[iconName], {size: "big", isfill: true, color: color})}
        {children && <NavItemLabel opened={opened} color={color}>{children}</NavItemLabel>}
        <NavItemBadge opened={opened} number={badgeNumber} onClick={onBadgeClick}/>
      </NavItemWrapper>
  );
};

export default NavItem;