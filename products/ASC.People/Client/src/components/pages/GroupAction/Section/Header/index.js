import React from 'react';
import { connect } from 'react-redux';
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
  const {group, history, settings} = props;

  const headerText = "New department";

  return (
    <div style={wrapperStyle}>
      <IconButton iconName={'ArrowPathIcon'} size="16" onClick={() => history.push(settings.homepage)}/>
      <Text.ContentHeader style={textStyle}>{headerText}</Text.ContentHeader>
    </div>
  );
};

SectionHeaderContent.propTypes = {
  group: PropTypes.object,
  history: PropTypes.object.isRequired
};

SectionHeaderContent.defaultProps = {
  group: null
};

function mapStateToProps(state) {
  return {
    settings: state.auth.settings
  };
};

export default connect(mapStateToProps)(withRouter(SectionHeaderContent));