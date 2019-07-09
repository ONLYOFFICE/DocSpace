import React from 'react'
import styled from 'styled-components';
import BadgedIcon from './badged-icon'
import ProfileActions from './profile-actions'

const Wrapper = styled.div`
  display: flex;
  padding: 0 16px;
  align-items: center;
  position: absolute;
  right: 0;
  height: 56px;
  z-index: 300;

  & > div {
    margin-left: 16px;
  }
`;

const HeaderIcons = props =>
  <Wrapper>
    {
      props.modules.map(module => 
        <BadgedIcon
          key={module.id}
          iconName={module.iconName}
          badgeNumber={module.notifications}
          onClick={module.onClick}
          onBadgeClick={module.onBadgeClick}
        />
      )
    }
    {
      props.user && <ProfileActions userActions={props.userActions} user={props.user}/>
    }
  </Wrapper>

export default HeaderIcons;