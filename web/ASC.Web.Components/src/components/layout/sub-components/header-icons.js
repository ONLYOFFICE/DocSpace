import React from 'react'
import styled from 'styled-components'
import NavItem from './nav-item'
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
    margin: 0 0 0 16px;
    padding: 0;
    min-width: 24px;
  }
`;

const HeaderIcons = props =>
  <Wrapper>
    {
      props.modules.map(module => 
        <NavItem
          key={module.id}
          isOpen={false}
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