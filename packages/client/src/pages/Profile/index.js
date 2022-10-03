import React from "react";
import PropTypes from "prop-types";
import Section from "@docspace/common/components/Section";
import toastr from "@docspace/components/toast/toastr";

import { SectionHeaderContent, SectionBodyContent } from "./Section";
import { withRouter } from "react-router";
import withCultureNames from "@docspace/common/hoc/withCultureNames";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

class Profile extends React.Component {
  componentDidMount() {
    const {
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
    const userId = "@self";

    setFirstLoad(false);
    setIsEditTargetUser(false);

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

  render() {
    //console.log("Profile render");

    const { profile, showCatalog, isAdmin } = this.props;

    return (
      <Section isInfoPanelAvailable={false} withBodyAutoFocus viewAs="profile">
        <Section.SectionHeader>
          <SectionHeaderContent profile={profile} />
        </Section.SectionHeader>

        <Section.SectionBody>
          <SectionBodyContent profile={profile} />
        </Section.SectionBody>
      </Section>
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
      fetchProfile,
      profile,
      setFirstLoad,
      setIsLoading,
      isEditTargetUser,
      setIsEditTargetUser,
      setLoadedProfile,
      showCatalog: auth.settingsStore.showCatalog,
    };
  })(
    observer(withTranslation(["Profile", "Common"])(withCultureNames(Profile)))
  )
);
