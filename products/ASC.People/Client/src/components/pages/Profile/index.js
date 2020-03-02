import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { Loader, toastr } from "asc-web-components";
import { PageLayout, utils } from "asc-web-common";
import { ArticleHeaderContent, ArticleMainButtonContent, ArticleBodyContent } from '../../Article';
import { SectionHeaderContent, SectionBodyContent } from './Section';
import { fetchProfile, resetProfile } from '../../../store/profile/actions';
import i18n from "./i18n";
import { I18nextProvider, withTranslation } from "react-i18next";
const { changeLanguage } = utils;

class PureProfile extends React.Component {

  constructor(props) {
    super(props);

    document.title = `${props.t("Profile")} – ${props.t("People")}`;

    this.state = {
      queryString: `${props.location.search.slice(1)}`
    };
  }

  componentDidMount() {
    const { match, fetchProfile, t } = this.props;
    const { userId } = match.params;

    const queryParams = this.state.queryString.split('&');
    const arrayOfQueryParams = queryParams.map(queryParam => queryParam.split('='));
    const linkParams = Object.fromEntries(arrayOfQueryParams);
    
    if (linkParams.email_change && linkParams.email_change === "success"){
      toastr.success(t('ChangeEmailSuccess'));
    }

    fetchProfile(userId);
  }

  componentDidUpdate(prevProps) {
    const { match, fetchProfile } = this.props;
    const { userId } = match.params;
    const prevUserId = prevProps.match.params.userId;

    if (userId !== undefined && userId !== prevUserId) {
      fetchProfile(userId);
    }
  }

  componentWillUnmount(){
    this.props.resetProfile();
  }

  render() {
    //console.log("Profile render")

    const { profile, isVisitor } = this.props;

    const articleProps = isVisitor ? {} : {
      articleHeaderContent: <ArticleHeaderContent />,
      articleMainButtonContent: <ArticleMainButtonContent />,
      articleBodyContent: <ArticleBodyContent />
    };

    const sectionProps = profile ? {
      sectionHeaderContent: <SectionHeaderContent />,
      sectionBodyContent: <SectionBodyContent />
    } : {
      sectionBodyContent: <Loader className="pageLoader" type="rombs" size='40px' />
    };

    return <PageLayout {...articleProps} {...sectionProps} withBodyAutoFocus={true}/>;
  };
};

const ProfileContainer = withTranslation()(PureProfile);

const Profile = (props) => { 

  changeLanguage(i18n);

  return <I18nextProvider i18n={i18n}><ProfileContainer {...props} /></I18nextProvider>
};

Profile.propTypes = {
  history: PropTypes.object.isRequired,
  match: PropTypes.object.isRequired,
  profile: PropTypes.object,
  fetchProfile: PropTypes.func.isRequired,
  isLoaded: PropTypes.bool
};

function mapStateToProps(state) {
  return {
    profile: state.profile.targetUser,
    isVisitor: state.auth.user.isVisitor,
  };
}

export default connect(mapStateToProps, {
  fetchProfile, resetProfile
})(Profile);