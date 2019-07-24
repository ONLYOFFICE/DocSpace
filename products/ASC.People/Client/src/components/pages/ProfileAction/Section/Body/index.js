import React from 'react';
import PropTypes from "prop-types";
import UserForm from './Form/userForm'


const SectionBodyContent = (props) => {
  const {profile, userType} = props;

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