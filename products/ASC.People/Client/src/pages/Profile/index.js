import React from "react";
import PropTypes from "prop-types";
import PageLayout from "@appserver/common/components/PageLayout";
import toastr from "studio/toastr";
import {
  ArticleHeaderContent,
  ArticleMainButtonContent,
  ArticleBodyContent,
} from "../../components/Article";
import { SectionHeaderContent, SectionBodyContent } from "./Section";
import { withRouter } from "react-router";

import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

class Profile extends React.Component {
  componentDidMount() {
    const {
      match,
      fetchProfile,
      profile,
      location,
      t,
      setDocumentTitle,
      setFirstLoad,
      setIsLoading,
      setIsEditTargetUser,
      setLoadedProfile,
    } = this.props;
    let { userId } = match.params;

    setFirstLoad(false);
    setIsEditTargetUser(false);

    if (!userId) userId = "@self";

    setDocumentTitle(t("Common:Profile"));
    this.documentElement = document.getElementsByClassName("hidingHeader");
    const queryString = ((location && location.search) || "").slice(1);
    const queryParams = queryString.split("&");
    const arrayOfQueryParams = queryParams.map((queryParam) =>
      queryParam.split("=")
    );
    const linkParams = Object.fromEntries(arrayOfQueryParams);

    if (linkParams.email_change && linkParams.email_change === "success") {
      toastr.success(t("ChangeEmailSuccess"));
    }
    if (!profile || profile.userName !== userId) {
      setIsLoading(true);
      setLoadedProfile(false);
      fetchProfile(userId).finally(() => {
        setIsLoading(false);
        setLoadedProfile(true);
      });
    }

    if (!profile && this.documentElement) {
      for (var i = 0; i < this.documentElement.length; i++) {
        this.documentElement[i].style.transition = "none";
      }
    }
  }

  componentDidUpdate(prevProps) {
    const { match, fetchProfile, profile, setIsLoading } = this.props;
    const { userId } = match.params;
    const prevUserId = prevProps.match.params.userId;

    if (userId !== undefined && userId !== prevUserId) {
      setIsLoading(true);
      fetchProfile(userId).finally(() => setIsLoading(false));
    }

    if (profile && this.documentElement) {
      for (var i = 0; i < this.documentElement.length; i++) {
        this.documentElement[i].style.transition = "";
      }
    }
  }

  componentWillUnmount() {
    const { isEditTargetUser } = this.props;
    if (!isEditTargetUser) {
      this.props.resetProfile();
    }
  }

  render() {
    //console.log("Profile render");

    const { profile } = this.props;

    return (
      <PageLayout withBodyAutoFocus>
        <PageLayout.ArticleHeader>
          <ArticleHeaderContent />
        </PageLayout.ArticleHeader>

        <PageLayout.ArticleMainButton>
          <ArticleMainButtonContent />
        </PageLayout.ArticleMainButton>

        <PageLayout.ArticleBody>
          <ArticleBodyContent />
        </PageLayout.ArticleBody>

        <PageLayout.SectionHeader>
          <SectionHeaderContent profile={profile} />
        </PageLayout.SectionHeader>

        <PageLayout.SectionBody>
          <SectionBodyContent profile={profile} />
        </PageLayout.SectionBody>
      </PageLayout>
    );
  }
}

Profile.propTypes = {
  fetchProfile: PropTypes.func.isRequired,
  history: PropTypes.object.isRequired,
  match: PropTypes.object.isRequired,
  profile: PropTypes.object,
  isAdmin: PropTypes.bool,
  language: PropTypes.string,
};

export default withRouter(
  inject(({ auth, peopleStore }) => {
    const { setDocumentTitle, isAdmin, language } = auth;
    const { targetUserStore, loadingStore } = peopleStore;
    const {
      resetTargetUser: resetProfile,
      getTargetUser: fetchProfile,
      targetUser: profile,
      isEditTargetUser,
      setIsEditTargetUser,
    } = targetUserStore;
    const { setFirstLoad, setIsLoading, setLoadedProfile } = loadingStore;
    return {
      setDocumentTitle,
      isAdmin,
      language,
      resetProfile,
      fetchProfile,
      profile,
      setFirstLoad,
      setIsLoading,
      isEditTargetUser,
      setIsEditTargetUser,
      setLoadedProfile,
    };
  })(observer(withTranslation(["Profile", "Common"])(Profile)))
);
