import React from 'react';
import PropTypes from "prop-types";
import UserForm from './Form/userForm'


const SectionBodyContent = (props) => {
  const {profile, userType} = props;

  // if(profile.birthday)
  //   profile.birthday = new Date(profile.birthday).toLocaleDateString();

  // if(profile.workFrom)
  //   profile.workFrom = new Date(profile.workFrom).toLocaleDateString();

  return (
    <UserForm initialValues={profile} userType={userType}/>
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