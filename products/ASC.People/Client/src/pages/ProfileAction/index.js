import React from "react";
import PropTypes from "prop-types";
import PageLayout from "@appserver/common/components/PageLayout";
import Loaders from "@appserver/common/components/Loaders";
import toastr from "studio/toastr";
import { linkOAuth } from "@appserver/common/api/people";
import { getAuthProviders } from "@appserver/common/api/settings";
import {
  ArticleHeaderContent,
  ArticleMainButtonContent,
  ArticleBodyContent,
} from "../../components/Article";
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
    } = this.props;
    const { userId } = match.params;
    this.documentElement = document.getElementsByClassName("hidingHeader");
    setDocumentTitle(t("ProfileAction"));

    if (isEdit) {
      setIsEditingForm(false);
    }
    if (userId) {
      fetchProfile(userId);
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

  render() {
    console.log("ProfileAction render");

    this.loaded = false;
    const { profile, match } = this.props;
    const { userId, type } = match.params;

    if (type) {
      this.loaded = true;
    } else if (profile) {
      this.loaded = profile.userName === userId || profile.id === userId;
    }

    return (
      <PageLayout>
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
          {this.loaded ? <SectionHeaderContent /> : <Loaders.SectionHeader />}
        </PageLayout.SectionHeader>

        <PageLayout.SectionBody>
          {this.loaded ? (
            <SectionUserBody />
          ) : (
            <Loaders.ProfileView isEdit={false} />
          )}
        </PageLayout.SectionBody>
      </PageLayout>
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
  inject(({ auth, peopleStore }) => ({
    setProviders: peopleStore.usersStore.setProviders,
    setDocumentTitle: auth.setDocumentTitle,
    isEdit: peopleStore.editingFormStore.isEdit,
    setIsEditingForm: peopleStore.editingFormStore.setIsEditingForm,
    fetchProfile: peopleStore.targetUserStore.getTargetUser,
    profile: peopleStore.targetUserStore.targetUser,
  }))(withTranslation("ProfileAction")(observer(ProfileAction)))
);
