import React from 'react';
import PropTypes from "prop-types";
import { Avatar } from 'asc-web-components';
import UserForm from './userForm';


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
            role={props.userType}
            editing={true}
            editLabel={"Add photo"}
            editAction={onEditAvatar}
          />
      }
      <UserForm initialValues={profile}/>
    </div>
  );
};

SectionBodyContent.propTypes = {
  profile: PropTypes.object,
  userType: PropTypes.oneOf(["user", "guest"])
};

SectionBodyContent.defaultProps = {
  profile: null,
  userType: "user"
}

export default SectionBodyContent;