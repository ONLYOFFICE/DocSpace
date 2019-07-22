import React from 'react';
import PropTypes from "prop-types";
import { Avatar } from 'asc-web-components';


const SectionBodyContent = (props) => {
  const {profile, history} = props;

  const isEditProfile = profile && profile.id;

  const getUserRole = (user) => {
    if(user.isOwner) return "owner";
    if(user.isAdmin) return "admin";
    if(user.isVisitor) return "guest";
    return "user";
  };

  const getEditAvatarLabel = () => {
    return isEditProfile ? "Edity photo" : "Add photo";
  }

  const onEditAvatar = () => {};

  return (
    <div>
      <Avatar
          size="max"
          role={getUserRole(profile)}
          source={profile.avatarBig}
          userName={profile.userName}
          editing={true}
          editLabel={getEditAvatarLabel()}
          editAction={onEditAvatar}
        />
    </div>
  );
};

SectionBodyContent.propTypes = {
  profile: PropTypes.object.isRequired,
  isLoaded: PropTypes.bool
};

export default SectionBodyContent;