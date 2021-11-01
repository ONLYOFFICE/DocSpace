import React from "react";
import MyProfileI18n from "./i18n";
import PeopleStore from "../../store/PeopleStore";

import PropTypes from "prop-types";
import PageLayout from "@appserver/common/components/PageLayout";
import toastr from "studio/toastr";
import Loaders from "@appserver/common/components/Loaders";
import { withRouter } from "react-router";

import { Provider as PeopleProvider, inject, observer } from "mobx-react";
import { I18nextProvider, withTranslation } from "react-i18next";
import {
  SectionBodyContent as ViewBodyContent,
  SectionHeaderContent as ViewHeaderContent,
} from "../Profile/Section";
import { SectionHeaderContent as EditHeaderContent } from "../ProfileAction/Section";
import EditBodyContent from "../ProfileAction/Section/Body";

class My extends React.Component {
  componentDidMount() {
    const {
      fetchProfile,
      profile,
      location,
      t,
      setDocumentTitle,
      setLoadedProfile,
      setIsLoading,
      setFirstLoad,
    } = this.props;

    setDocumentTitle(t("Common:Profile"));
    setFirstLoad(false);

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
      setIsLoading(true);
      setLoadedProfile(false);
      fetchProfile("@self").finally(() => {
        setIsLoading(false);
        setLoadedProfile(true);
      });
    }
  }

  componentWillUnmount() {
    this.props.resetProfile();
  }

  render() {
    const { profile, tReady, location } = this.props;

    const isEdit = (location && location.search === "?action=edit") || false;

    //console.log("My Profile render", this.props, isEdit);

    return (
      <PageLayout withBodyAutoFocus>
        <PageLayout.SectionHeader>
          {isEdit ? (
            <EditHeaderContent isMy={true} tReady={tReady} />
          ) : (
            <ViewHeaderContent isMy={true} tReady={tReady} />
          )}
        </PageLayout.SectionHeader>

        <PageLayout.SectionBody>
          {isEdit ? (
            <EditBodyContent isMy={true} tReady={tReady} />
          ) : (
            <ViewBodyContent isMy={true} tReady={tReady} />
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
    setLoadedProfile: peopleStore.loadingStore.setLoadedProfile,
    setIsLoading: peopleStore.loadingStore.setIsLoading,
    setFirstLoad: peopleStore.loadingStore.setFirstLoad,
  }))(withTranslation(["Profile", "ProfileAction"])(observer(My)))
);

const peopleStore = new PeopleStore();

export default ({ i18n, ...rest }) => {
  return (
    <PeopleProvider peopleStore={peopleStore}>
      <I18nextProvider i18n={MyProfileI18n}>
        <MyProfile {...rest} />
      </I18nextProvider>
    </PeopleProvider>
  );
};
