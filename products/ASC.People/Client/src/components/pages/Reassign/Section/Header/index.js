import React from "react";
import { connect } from "react-redux";
import { Header, IconButton } from "asc-web-components";
import { withRouter } from "react-router";
import { useTranslation } from 'react-i18next';

const wrapperStyle = {
  display: "flex",
  alignItems: "center"
};

const textStyle = {
  marginLeft: "16px",
  marginRight: "16px"
};

const SectionHeaderContent = props => {
  const { history, settings } = props;
  const { t } = useTranslation();

  return (
    <div style={wrapperStyle}>
      <div style={{ width: "16px" }}>
        <IconButton
          iconName={"ArrowPathIcon"}
          color="#A3A9AE"
          size="16"
          onClick={() => history.push(settings.homepage)}
        />
      </div>
      <Header type="content" truncate={true} style={textStyle}>
        {/* {profile.displayName}
        {profile.isLDAP && ` (${t('LDAPLbl')})`}
        -  */}
        {t('ReassignmentData')}
      </Header>
    </div>
  );
};

function mapStateToProps(state) {
  return {
    profile: state.profile.targetUser,
    settings: state.auth.settings
  };
}

export default connect(mapStateToProps)(withRouter(SectionHeaderContent));
