import React from "react";
import MyProfileI18n from "./i18n";
import PeopleStore from "../../store/PeopleStore";

import PropTypes from "prop-types";
import Section from "@docspace/common/components/Section";
import toastr from "@docspace/components/toast/toastr";
import { withRouter } from "react-router";

import { Provider as PeopleProvider, inject, observer } from "mobx-react";
import { I18nextProvider, withTranslation } from "react-i18next";
import {
  SectionBodyContent as ViewBodyContent,
  SectionHeaderContent as ViewHeaderContent,
} from "../Profile/Section";
import EditBodyContent from "../ProfileAction/Section/Body";
import Link from "@docspace/components/link";
import { Trans } from "react-i18next";

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
    const { tReady, location } = this.props;

    const isEdit = (location && location.search === "?action=edit") || false;

    //console.log("My Profile render", this.props, isEdit);

    return (
      <Section withBodyAutoFocus>
        <Section.SectionHeader>
          {isEdit ? (
            <SectionHeaderContent isMy={true} tReady={tReady} />
          ) : (
            <ViewHeaderContent isMy={true} tReady={tReady} />
          )}
        </Section.SectionHeader>

        <Section.SectionBody>
          {isEdit ? (
            <EditBodyContent isMy={true} tReady={tReady} />
          ) : (
            <ViewBodyContent isMy={true} tReady={tReady} />
          )}
        </Section.SectionBody>
      </Section>
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
