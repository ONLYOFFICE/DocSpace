import React from 'react'
import styled from 'styled-components';
import Badge from '../../badge'
import { Icons } from '../../icons'

const InlineWrapper = styled.div`
  position: relative;
  display: inline-block;
  cursor: pointer;
`;

const StyledBadge = styled(Badge)`
  position: absolute;
  right: -10px;
  top: -5px;
`;

const BadgedIcon = props => {
  const { onClick, onBadgeClick, className, iconName, color, badgeNumber } = props;

  return (
    <InlineWrapper onClick={onClick} className={className}>
      {React.createElement(Icons[iconName], {isfill: true, color: color})} 
      <StyledBadge number={badgeNumber} onClick={onBadgeClick}/>
    </InlineWrapper>
  );
};

export default BadgedIcon;