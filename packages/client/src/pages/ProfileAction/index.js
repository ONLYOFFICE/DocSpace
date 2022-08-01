import React from "react";
import PropTypes from "prop-types";
import Section from "@docspace/common/components/Section";
import Loaders from "@docspace/common/components/Loaders";
import toastr from "client/toastr";
import { linkOAuth } from "@docspace/common/api/people";
import { getAuthProviders } from "@docspace/common/api/settings";

import SectionUserBody from "./Section/Body/index";
import {
  SectionHeaderContent,
  // CreateUserForm,
  // UpdateUserForm,
  // AvatarEditorPage,
  // CreateAvatarEditorPage,
} from "./Section";
import { withTranslation } from "react-i18next";

import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";

class ProfileAction extends React.Component {
  componentDidMount() {
    const {
      match,
      fetchProfile,
      isEdit,
      setIsEditingForm,
      t,
      setDocumentTitle,
      setFirstLoad,
      profile,
      setLoadedProfile,
    } = this.props;
    const { userId } = match.params;
    setFirstLoad(false);
    this.documentElement = document.getElementsByClassName("hidingHeader");
    setDocumentTitle(t("ProfileAction"));
    if (isEdit) {
      setIsEditingForm(false);
    }
    if ((userId && !profile) || (profile && userId !== profile.userName)) {
      setLoadedProfile(false);
      fetchProfile(userId).finally(() => setLoadedProfile(true));
    }

    if (!this.loaded && this.documentElement) {
      for (var i = 0; i < this.documentElement.length; i++) {
        this.documentElement[i].style.transition = "none";
      }
    }
  }

  componentDidUpdate(prevProps) {
    const { match, fetchProfile } = this.props;
    const { userId } = match.params;
    const prevUserId = prevProps.match.params.userId;

    if (userId !== undefined && userId !== prevUserId) {
      fetchProfile(userId);
    }

    if (this.loaded && this.documentElement) {
      for (var i = 0; i < this.documentElement.length; i++) {
        this.documentElement[i].style.transition = "";
      }
    }
  }

  componentWillUnmount() {
    this.props.setIsEditTargetUser(false);
  }

  render() {
    // console.log("ProfileAction render");

    this.loaded = false;
    const { profile, match, isMy, tReady, showCatalog, isAdmin } = this.props;
    const { userId, type } = match.params;

    if (type) {
      this.loaded = true;
    } else if (profile) {
      this.loaded = profile.userName === userId || profile.id === userId;
    }

    return (
      <Section>
        <Section.SectionHeader>
          <SectionHeaderContent tReady={tReady} loaded={this.loaded} />
        </Section.SectionHeader>

        <Section.SectionBody>
          <SectionUserBody isMy={isMy} tReady={tReady} loaded={this.loaded} />
        </Section.SectionBody>
      </Section>
    );
  }
}

ProfileAction.propTypes = {
  setDocumentTitle: PropTypes.func.isRequired,
  isEdit: PropTypes.bool,
  setIsEditingForm: PropTypes.func.isRequired,
  fetchProfile: PropTypes.func.isRequired,
  profile: PropTypes.object,
  match: PropTypes.object.isRequired,
};

export default withRouter(
  inject(({ auth, peopleStore }) => {
    const { setDocumentTitle } = auth;
    const {
      usersStore,
      editingFormStore,
      targetUserStore,
      loadingStore,
    } = peopleStore;
    const { setProviders } = usersStore;
    const { isEdit, setIsEditingForm } = editingFormStore;
    const {
      getTargetUser: fetchProfile,
      targetUser: profile,
      setIsEditTargetUser,
    } = targetUserStore;
    const { setFirstLoad, setLoadedProfile } = loadingStore;

    return {
      setProviders,
      setDocumentTitle,
      isEdit,
      setIsEditingForm,
      fetchProfile,
      profile,
      setFirstLoad,
      setLoadedProfile,
      setIsEditTargetUser,
      isAdmin: auth.isAdmin,
      showCatalog: auth.settingsStore.showCatalog,
    };
  })(withTranslation(["ProfileAction", "Common"])(observer(ProfileAction)))
);
