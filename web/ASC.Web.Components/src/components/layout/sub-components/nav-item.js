import React from 'react'
import styled from 'styled-components';
import Badge from '../../badge'
import { Icons } from '../../icons'
import BadgedIcon from './badged-icon'

const baseColor = '#7A95B0',
      activeColor = '#FFFFFF';

const Wrapper = styled.div`
  display: flex;
  min-width: 56px;
  min-height: 56px;
  align-items: center;
  padding: 0 16px;
  cursor: pointer;
  color: ${props => props.color};
`;

const Label = styled.div`
  font-weight: bold;
  font-size: 16px;  
  margin-left: 16px;
  margin-right: auto;
  overflow: hidden;
  text-overflow: ellipsis;
`;

const Seporator = styled.div`
  border-bottom: 1px solid ${baseColor};
  margin: 0 16px;
`;

const NavItem = props => {
  const { seporator, active, isOpen, iconName, children, badgeNumber, onClick, onBadgeClick } = props;
  const color = active ? activeColor : baseColor;

  return (
    seporator
    ? <Seporator/>
    : <Wrapper color={color} onClick={onClick}>
        {
          isOpen
          ? <>
              {React.createElement(Icons[iconName], {isfill: true, color: color})}
              <Label>{children}</Label>
              <Badge number={badgeNumber} onClick={onBadgeClick}/>
            </>
          : <BadgedIcon iconName={iconName} badgeNumber={badgeNumber} color={color} onBadgeClick={onBadgeClick}/>
        }
      </Wrapper>
  );
};

export default NavItem;