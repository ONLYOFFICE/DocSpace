import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import styled from 'styled-components';
import { withTranslation } from "react-i18next";
import { Button } from "asc-web-components";
import { fetchProfile } from "../../../../../store/profile/actions";
import { setDocumentTitle } from "../../../../../helpers/utils";


const InfoContainer = styled.div`
  margin-bottom: 24px;
`;

class SectionBodyContent extends React.PureComponent{
  constructor(props) {
    super(props);
  }

  componentDidMount() {
    const { match, fetchProfile, t } = this.props;
    const { userId } = match.params;

    setDocumentTitle(t("ProfileAction"));

    if (userId) {
      fetchProfile(userId);
    }
  }

  componentDidUpdate(prevProps) {
    const { match, fetchProfile } = this.props;
    const { userId } = match.params;
    const prevUserId = prevProps.match.params.userId;

    if (userId !== undefined && userId !== prevUserId) {
      fetchProfile(userId);
    }
  }

  onBackClick = () => {
    const {profile, settings} = this.props

    debugger

    this.props.history.push(`${settings.homepage}/edit/${profile.userName}`)
  }

  render(){

    const {t, profile} = this.props

    return(
      <InfoContainer>
        {t("UploadNewPhoto")}
        <Button
          label={t("Back")}
          onClick={this.onBackClick}
          size="big"
          primary
        />
      </InfoContainer>
      
    )
  }
}

function mapStateToProps(state) {
  return {
    profile: state.profile.targetUser,
    settings: state.auth.settings
  };
}

export default connect(mapStateToProps, { fetchProfile })(withTranslation()(withRouter(SectionBodyContent)));
