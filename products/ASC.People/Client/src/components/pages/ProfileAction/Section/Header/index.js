import React from 'react';
import { connect } from 'react-redux';
import { withRouter } from "react-router";
import PropTypes from "prop-types";
import { IconButton, Text } from 'asc-web-components';
import { useTranslation } from 'react-i18next';

const wrapperStyle = {
  display: "flex",
  alignItems: "center"
};

const textStyle = {
  marginLeft: "16px"
};

const SectionHeaderContent = (props) => {
  const {profile, history, userType, settings} = props;
  const { t } = useTranslation();

  const headerText = profile && profile.displayName
    ? profile.displayName
    : userType === "user"
      ? t('NewEmployee')
      : t('NewGuest');

  return (
    <div style={wrapperStyle}>
      <IconButton iconName={'ArrowPathIcon'} size="16" onClick={() => history.push(settings.homepage)}/>
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
};

function mapStateToProps(state) {
  return {
    settings: state.auth.settings
  };
};

export default connect(mapStateToProps)(withRouter(SectionHeaderContent));