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
      props.chatModule && <BadgedIcon
        key={props.chatModule.id}
        iconName={props.chatModule.iconName}
        badgeNumber={props.chatModule.notifications}
        onClick={props.chatModule.onClick}
        onBadgeClick={props.chatModule.onBadgeClick}
      />
    }
    { props.currentUser && <ProfileActions {...props.currentUser}/> }
  </Wrapper>

export default HeaderIcons;