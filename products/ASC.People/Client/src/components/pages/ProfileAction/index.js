import React, { useEffect } from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { Loader } from "asc-web-components";
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
import { fetchProfile } from "../../../store/profile/actions";
import { setIsEditingForm } from "../../../store/people/actions";
import { I18nextProvider, withTranslation } from "react-i18next";
import { createI18N } from "../../../helpers/i18n";
import { setDocumentTitle } from "../../../helpers/utils";
import { withRouter } from "react-router";
const i18n = createI18N({
  page: "ProfileAction",
  localesPath: "pages/ProfileAction",
});
const { changeLanguage } = utils;
const { isAdmin } = store.auth.selectors;

class ProfileAction extends React.Component {
  componentDidMount() {
    const { match, fetchProfile, isEdit, setIsEditingForm, t } = this.props;
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

          {loaded && (
            <PageLayout.SectionHeader>
              <SectionHeaderContent />
            </PageLayout.SectionHeader>
          )}

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

function mapStateToProps(state) {
  return {
    isVisitor: state.auth.user.isVisitor,
    profile: state.profile.targetUser,
    isAdmin: isAdmin(state),
    isEdit: state.people.editingForm.isEdit,
    avatarEditorIsOpen: state.people.avatarEditorIsOpen,
  };
}

export default connect(mapStateToProps, { fetchProfile, setIsEditingForm })(
  ProfileActionContainer
);
