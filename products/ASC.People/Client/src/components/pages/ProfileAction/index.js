import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { PageLayout, utils, store, Loaders } from "asc-web-common";
import {
  ArticleHeaderContent,
  ArticleMainButtonContent,
  ArticleBodyContent,
} from "../../Article";
import {
  SectionHeaderContent,
  CreateUserForm,
  UpdateUserForm,
  AvatarEditorPage,
  CreateAvatarEditorPage,
} from "./Section";
import { I18nextProvider, withTranslation } from "react-i18next";
import { createI18N } from "../../../helpers/i18n";
import { withRouter } from "react-router";
import { inject, observer } from "mobx-react";
const i18n = createI18N({
  page: "ProfileAction",
  localesPath: "pages/ProfileAction",
});
const { changeLanguage } = utils;

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

    setDocumentTitle(t("ProfileAction"));
    changeLanguage(i18n);
    if (isEdit) {
      setIsEditingForm(false);
    }
    if (userId) {
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

  render() {
    console.log("ProfileAction render");

    let loaded = false;
    const {
      profile,
      isVisitor,
      match,
      isAdmin,
      avatarEditorIsOpen,
    } = this.props;
    const { userId, type } = match.params;

    if (type) {
      loaded = true;
    } else if (profile) {
      loaded = profile.userName === userId || profile.id === userId;
    }

    return (
      <I18nextProvider i18n={i18n}>
        <PageLayout>
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
            {loaded ? <SectionHeaderContent /> : <Loaders.SectionHeader />}
          </PageLayout.SectionHeader>

          <PageLayout.SectionBody>
            {loaded ? (
              type ? (
                avatarEditorIsOpen ? (
                  <CreateAvatarEditorPage />
                ) : (
                  <CreateUserForm />
                )
              ) : avatarEditorIsOpen ? (
                <AvatarEditorPage />
              ) : (
                <UpdateUserForm />
              )
            ) : (
              <Loaders.ProfileView isEdit={false} />
            )}
          </PageLayout.SectionBody>
        </PageLayout>
      </I18nextProvider>
    );
  }
}

ProfileAction.propTypes = {
  fetchProfile: PropTypes.func.isRequired,
  match: PropTypes.object.isRequired,
  profile: PropTypes.object,
  isAdmin: PropTypes.bool,
};

const ProfileActionTranslate = withTranslation()(withRouter(ProfileAction));

const ProfileActionContainer = (props) => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  return (
    <I18nextProvider i18n={i18n}>
      <ProfileActionTranslate {...props} />
    </I18nextProvider>
  );
};

export default inject(({ auth, peopleStore }) => ({
  setDocumentTitle: auth.setDocumentTitle,
  isAdmin: auth.isAdmin,
  isVisitor: auth.userStore.user.isVisitor,
  isEdit: peopleStore.editingFormStore.isEdit,
  setIsEditingForm: peopleStore.editingFormStore.setIsEditingForm,
  fetchProfile: peopleStore.targetUserStore.getTargetUser,
  profile: peopleStore.targetUserStore.targetUser,
  avatarEditorIsOpen: peopleStore.avatarEditorStore.visible,
}))(observer(ProfileActionContainer));
