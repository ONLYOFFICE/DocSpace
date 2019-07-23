import React from 'react';
import PropTypes from "prop-types";
import { IconButton, Text } from 'asc-web-components';


const wrapperStyle = {
  display: "flex",
  alignItems: "center"
};

const textStyle = {
  marginLeft: "16px"
};

const SectionHeaderContent = (props) => {
  const {profile, history, userType} = props;

  const headerText = profile
    ? profile.userName
    : userType === "user"
      ? "New guest"
      : "New employee";

  return (
    <div style={wrapperStyle}>
      <IconButton iconName={'ArrowPathIcon'} size="16" onClick={history.goBack}/>
      <Text.ContentHeader style={textStyle}>{headerText}</Text.ContentHeader>
    </div>
  );
};

SectionHeaderContent.propTypes = {
  profile: PropTypes.object,
  history: PropTypes.object.isRequired,
  userType: PropTypes.oneOf(["user", "guest"])
};

SectionHeaderContent.defaultProps = {
  profile: null,
  userType: "user"
}

export default SectionHeaderContent;