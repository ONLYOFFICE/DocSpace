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

  const onEditAvatar = () => {};

  return (
    <div>
      {
        profile
        ? <Avatar
            size="max"
            role={getUserRole(profile)}
            source={profile.avatar}
            userName={profile.userName}
            editing={true}
            editLabel={"Edit photo"}
            editAction={onEditAvatar}
          />
        : <Avatar
            size="max"
            role="user"
            editing={true}
            editLabel={"Add photo"}
            editAction={onEditAvatar}
          />
      }
    </div>
  );
};

SectionBodyContent.propTypes = {
  profile: PropTypes.object
};

export default SectionBodyContent;