import React from "react";
import i18n from "../../i18n";
import PeopleStore from "../../store/PeopleStore";

import PropTypes from "prop-types";
import PageLayout from "@appserver/common/components/PageLayout";
import toastr from "studio/toastr";
import Loaders from "@appserver/common/components/Loaders";
import { withRouter } from "react-router";

import { Provider as PeopleProvider, inject, observer } from "mobx-react";
import { I18nextProvider, withTranslation } from "react-i18next";
import { SectionBodyContent, SectionHeaderContent } from "../Profile/Section";

class My extends React.Component {
  componentDidMount() {
    const { fetchProfile, profile, location, t, setDocumentTitle } = this.props;

    setDocumentTitle(t("Profile"));
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
    if (!profile) {
      fetchProfile("@self");
    }

    if (!profile && this.documentElement) {
      for (var i = 0; i < this.documentElement.length; i++) {
        this.documentElement[i].style.transition = "none";
      }
    }
  }

  componentWillUnmount() {
    this.props.resetProfile();
  }

  render() {
    //console.log("My Profile render");

    const { profile, tReady } = this.props;

    return (
      <PageLayout withBodyAutoFocus>
        <PageLayout.SectionHeader>
          {profile && tReady ? (
            <SectionHeaderContent isMy={true} />
          ) : (
            <Loaders.SectionHeader />
          )}
        </PageLayout.SectionHeader>

        <PageLayout.SectionBody>
          {profile && tReady ? (
            <SectionBodyContent isMy={true} />
          ) : (
            <Loaders.ProfileView />
          )}
        </PageLayout.SectionBody>
      </PageLayout>
    );
  }
}

My.propTypes = {
  fetchProfile: PropTypes.func.isRequired,
  history: PropTypes.object.isRequired,
  match: PropTypes.object.isRequired,
  profile: PropTypes.object,
  language: PropTypes.string,
};

const MyProfile = withRouter(
  inject(({ auth, peopleStore }) => ({
    setDocumentTitle: auth.setDocumentTitle,
    language: auth.language,
    resetProfile: peopleStore.targetUserStore.resetTargetUser,
    fetchProfile: peopleStore.targetUserStore.getTargetUser,
    profile: peopleStore.targetUserStore.targetUser,
  }))(observer(withTranslation("Profile")(My)))
);

const peopleStore = new PeopleStore();

export default (props) => (
  <PeopleProvider peopleStore={peopleStore}>
    <I18nextProvider i18n={i18n}>
      <MyProfile {...props} />
    </I18nextProvider>
  </PeopleProvider>
);
