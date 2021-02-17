import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { PageLayout, utils, toastr, Loaders } from "asc-web-common";
import {
  ArticleHeaderContent,
  ArticleMainButtonContent,
  ArticleBodyContent,
} from "../../Article";
import { SectionHeaderContent, SectionBodyContent } from "./Section";
import { I18nextProvider, withTranslation } from "react-i18next";
import { createI18N } from "../../../helpers/i18n";
import { withRouter } from "react-router";
import { isChrome, isAndroid } from "react-device-detect";
import { inject, observer } from "mobx-react";
const i18n = createI18N({
  page: "Profile",
  localesPath: "pages/Profile",
});
const { changeLanguage } = utils;

class PureProfile extends React.Component {
  componentDidMount() {
    const {
      match,
      fetchProfile,
      profile,
      location,
      t,
      setDocumentTitle,
    } = this.props;
    const { userId } = match.params;
    isChrome && isAndroid && window && window.scroll(0, 0);
    setDocumentTitle(t("Profile"));

    const queryString = ((location && location.search) || "").slice(1);
    const queryParams = queryString.split("&");
    const arrayOfQueryParams = queryParams.map((queryParam) =>
      queryParam.split("=")
    );
    const linkParams = Object.fromEntries(arrayOfQueryParams);

    if (linkParams.email_change && linkParams.email_change === "success") {
      toastr.success(t("ChangeEmailSuccess"));
    }
    if (!profile) {
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
        {!isVisitor && isAdmin && (
          <PageLayout.ArticleMainButton>
            <ArticleMainButtonContent />
          </PageLayout.ArticleMainButton>
        )}
        {!isVisitor && (
          <PageLayout.ArticleBody>
            <ArticleBodyContent />
          </PageLayout.ArticleBody>
        )}

        <PageLayout.SectionHeader>
          {profile ? <SectionHeaderContent /> : <Loaders.SectionHeader />}
        </PageLayout.SectionHeader>

        <PageLayout.SectionBody>
          {profile ? <SectionBodyContent /> : <Loaders.ProfileView />}
        </PageLayout.SectionBody>
      </PageLayout>
    );
  }
}

const ProfileContainer = withTranslation()(withRouter(PureProfile));

const Profile = ({ language, ...rest }) => {
  useEffect(() => {
    changeLanguage(i18n, language);
  }, [language]);

  return (
    <I18nextProvider i18n={i18n}>
      <ProfileContainer {...rest} />
    </I18nextProvider>
  );
};

Profile.propTypes = {
  fetchProfile: PropTypes.func.isRequired,
  history: PropTypes.object.isRequired,
  isLoaded: PropTypes.bool,
  match: PropTypes.object.isRequired,
  profile: PropTypes.object,
  isAdmin: PropTypes.bool,
  language: PropTypes.string,
};

export default inject(({ auth, peopleStore }) => ({
  setDocumentTitle: auth.setDocumentTitle,
  isVisitor: auth.userStore.user.isVisitor,
  isLoaded: auth.isLoaded,
  isAdmin: auth.isAdmin,
  language: auth.language,
  resetProfile: peopleStore.targetUserStore.resetTargetUser,
  fetchProfile: peopleStore.targetUserStore.getTargetUser,
  profile: peopleStore.targetUserStore.targetUser,
}))(observer(Profile));
