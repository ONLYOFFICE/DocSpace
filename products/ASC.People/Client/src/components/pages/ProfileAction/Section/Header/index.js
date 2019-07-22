import React from 'react';
import PropTypes from "prop-types";
import { IconButton, Text } from 'asc-web-components';

const config = require('../../../../../../package.json');

const wrapperStyle = {
  display: "flex",
  alignItems: "center"
};

const textStyle = {
  marginLeft: "16px"
};

const SectionHeaderContent = (props) => {
  const {profile, history} = props;

  const isEditProfile = profile && profile.id;

  const onGoBack = () => {
    window.location.href = history && history.location ? history.location.pathname : config.homepage;
  }

  const getHeaderText = () => {
    return isEditProfile ? profile.userName : "New employee";
  }

  return (
    <div style={wrapperStyle}>
      <IconButton iconName={'ProjectDocumentsUpIcon'} size="16" onClick={onGoBack}/>
      <Text.ContentHeader style={textStyle}>{getHeaderText()}</Text.ContentHeader>
    </div>
  );
};

SectionHeaderContent.propTypes = {
  profile: PropTypes.object.isRequired,
  history: PropTypes.object.isRequired,
  isLoaded: PropTypes.bool
};

export default SectionHeaderContent;