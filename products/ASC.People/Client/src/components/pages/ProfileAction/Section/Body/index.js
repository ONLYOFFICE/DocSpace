import React from 'react';
import PropTypes from "prop-types";
import { Avatar } from 'asc-web-components';


const SectionBodyContent = (props) => {
  const {profile} = props;

  const getUserRole = (user) => {
    if(user.isOwner) return "owner";
    if(user.isAdmin) return "admin";
    if(user.isVisitor) return "guest";
    return "user";
  };

  const avatarLabel = profile && profile.id ? "Edit photo" : "Add photo";

  const onEditAvatar = () => {};

  return (
    <div>
      <Avatar
          size="max"
          role={getUserRole(profile)}
          source={profile.avatarBig}
          userName={profile.userName}
          editing={true}
          editLabel={avatarLabel}
          editAction={onEditAvatar}
        />
    </div>
  );
};

SectionBodyContent.propTypes = {
  profile: PropTypes.object.isRequired
};

export default SectionBodyContent;