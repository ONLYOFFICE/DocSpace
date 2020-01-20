import React, { memo } from "react";
import PropTypes from 'prop-types';
import { Avatar } from 'asc-web-components';
import {
  StyledProfileMenu,
  MenuContainer,
  AvatarContainer,
  MainLabelContainer,
  LabelContainer,
  TopArrow
} from "./StyledProfileMenu";

// eslint-disable-next-line react/display-name
const ProfileMenu = memo(props => {
  const {
    displayName,
    email,
    avatarRole,
    avatarSource
  } = props;

  return (
    <StyledProfileMenu {...props}>
      <MenuContainer {...props}>
        <AvatarContainer>
          <Avatar
            size='medium'
            role={avatarRole}
            source={avatarSource}
            userName={displayName}
          />
        </AvatarContainer>
        <MainLabelContainer>
          {displayName}
        </MainLabelContainer>
        <LabelContainer>
          {email}
        </LabelContainer>
      </MenuContainer>
      <TopArrow />
    </StyledProfileMenu>
  );
});

ProfileMenu.propTypes = {
  avatarRole: PropTypes.oneOf(['owner', 'admin', 'guest', 'user']),
  avatarSource: PropTypes.string,
  className: PropTypes.string,
  displayName: PropTypes.string,
  email: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array])
};

export default ProfileMenu