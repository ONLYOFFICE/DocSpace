import React from 'react';
import { withRouter } from "react-router";
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
      ? "New employee"
      : "New guest";

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

export default withRouter(SectionHeaderContent);