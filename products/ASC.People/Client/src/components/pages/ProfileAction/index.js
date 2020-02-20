import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { Loader } from "asc-web-components";
import { PageLayout, utils } from "asc-web-common";
import {
  ArticleHeaderContent,
  ArticleMainButtonContent,
  ArticleBodyContent
} from "../../Article";
import {
  SectionHeaderContent,
  CreateUserForm,
  UpdateUserForm
} from "./Section";
import { fetchProfile } from "../../../store/profile/actions";
import i18n from "./i18n";
import { I18nextProvider, withTranslation } from "react-i18next";
const { changeLanguage } = utils;

class ProfileAction extends React.Component {
  componentDidMount() {
    const { match, fetchProfile, t } = this.props;
    const { userId } = match.params;

    document.title = `${t("ProfileAction")} – ${t("People")}`;

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
    const { profile, isVisitor, match } = this.props;
    const { userId, type } = match.params;

    changeLanguage(i18n);

    if (type) {
      loaded = true;
    } else if (profile) {
      loaded = profile.userName === userId || profile.id === userId;
    }

    const articleProps = isVisitor
      ? {}
      : {
          articleHeaderContent: <ArticleHeaderContent />,
          articleMainButtonContent: <ArticleMainButtonContent />,
          articleBodyContent: <ArticleBodyContent />
        };

    const sectionProps = loaded
      ? {
          sectionHeaderContent: <SectionHeaderContent />,
          sectionBodyContent: type ? <CreateUserForm /> : <UpdateUserForm />
        }
      : {
          sectionBodyContent: (
            <Loader className="pageLoader" type="rombs" size='40px' />
          )
        };

    return (
      <I18nextProvider i18n={i18n}>
        <PageLayout {...articleProps} {...sectionProps} />
      </I18nextProvider>
    );
  }
}

ProfileAction.propTypes = {
  match: PropTypes.object.isRequired,
  profile: PropTypes.object,
  fetchProfile: PropTypes.func.isRequired
};

const ProfileActionTranslate = withTranslation()(ProfileAction);

const ProfileActionContainer = props => {
  changeLanguage(i18n);

  return (
    <I18nextProvider i18n={i18n}>
      <ProfileActionTranslate {...props} />
    </I18nextProvider>
  );
};

function mapStateToProps(state) {
  return {
    profile: state.profile.targetUser,
    isVisitor: state.auth.user.isVisitor,
  };
}

export default connect(mapStateToProps, { fetchProfile })(ProfileActionContainer);
