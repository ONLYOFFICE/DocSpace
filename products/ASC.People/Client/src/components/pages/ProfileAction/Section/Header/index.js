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
  const {profile, history} = props;
  const headerText = profile ? profile.userName : "New employee";

  return (
    <div style={wrapperStyle}>
      <IconButton iconName={'ProjectDocumentsUpIcon'} size="16" onClick={history.goBack}/>
      <Text.ContentHeader style={textStyle}>{headerText}</Text.ContentHeader>
    </div>
  );
};

SectionHeaderContent.propTypes = {
  profile: PropTypes.object,
  history: PropTypes.object.isRequired
};

export default SectionHeaderContent;