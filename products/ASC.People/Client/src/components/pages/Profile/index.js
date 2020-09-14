import React, { useEffect } from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { Loader, toastr } from "asc-web-components";
import { PageLayout, utils, store } from "asc-web-common";
import {
  ArticleHeaderContent,
  ArticleMainButtonContent,
  ArticleBodyContent
} from "../../Article";
import { SectionHeaderContent, SectionBodyContent } from "./Section";
import { fetchProfile, resetProfile } from "../../../store/profile/actions";
import { I18nextProvider, withTranslation } from "react-i18next";
import { createI18N } from "../../../helpers/i18n";
const i18n = createI18N({
  page: "Profile",
  localesPath: "pages/Profile"
});
const { changeLanguage } = utils;
const { isAdmin } = store.auth.selectors;

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

    const queryParams = this.state.queryString.split("&");
    const arrayOfQueryParams = queryParams.map(queryParam =>
      queryParam.split("=")
    );
    const linkParams = Object.fromEntries(arrayOfQueryParams);

    if (linkParams.email_change && linkParams.email_change === "success") {
      toastr.success(t("ChangeEmailSuccess"));
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

  componentWillUnmount() {
    this.props.resetProfile();
  }

  render() {
    //console.log("Profile render")
    const { profile, isVisitor, isAdmin } = this.props;

    return (
      <PageLayout withBodyAutoFocus={true}>
        {!isVisitor && (
          <PageLayout.ArticleHeader>
            <ArticleHeaderContent />
          </PageLayout.ArticleHeader>
        )}
        {(!isVisitor && isAdmin) && (
          <PageLayout.ArticleMainButton>
            <ArticleMainButtonContent />
          </PageLayout.ArticleMainButton>
        )}
        {!isVisitor && (
          <PageLayout.ArticleBody>
            <ArticleBodyContent />
          </PageLayout.ArticleBody>
        )}

        {profile && (
          <PageLayout.SectionHeader>
            <SectionHeaderContent />
          </PageLayout.SectionHeader>
        )}

        <PageLayout.SectionBody>
          {profile ? (
            <SectionBodyContent />
          ) : (
              <Loader className="pageLoader" type="rombs" size="40px" />
            )}
        </PageLayout.SectionBody>
      </PageLayout>
    );
  }
}

const ProfileContainer = withTranslation()(PureProfile);

const Profile = props => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  return (
    <I18nextProvider i18n={i18n}>
      <ProfileContainer {...props} />
    </I18nextProvider>
  );
};

Profile.propTypes = {
  fetchProfile: PropTypes.func.isRequired,
  history: PropTypes.object.isRequired,
  isLoaded: PropTypes.bool,
  match: PropTypes.object.isRequired,
  profile: PropTypes.object,
  isAdmin: PropTypes.bool
};

function mapStateToProps(state) {
  return {
    isVisitor: state.auth.user.isVisitor,
    profile: state.profile.targetUser,
    isAdmin: isAdmin(state.auth.user)
  };
}

export default connect(
  mapStateToProps,
  {
    fetchProfile,
    resetProfile
  }
)(Profile);
