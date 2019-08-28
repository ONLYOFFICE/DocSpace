import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { PageLayout, Loader } from "asc-web-components";
import { ArticleHeaderContent, ArticleMainButtonContent, ArticleBodyContent } from '../../Article';
import { SectionHeaderContent, CreateUserForm, UpdateUserForm } from './Section';
import { setProfile, fetchProfile, resetProfile } from '../../../store/profile/actions';
import i18n from "./i18n";
import { I18nextProvider } from "react-i18next";

class ProfileAction extends React.Component {
  componentDidMount() {
    const { match, setProfile, fetchProfile } = this.props;
    const { userId, type } = match.params;

    if (!userId) {
      setProfile({ isVisitor: type === "guest" });
    } else {
      fetchProfile(userId);
    }
  }

  componentDidUpdate(prevProps) {
    const { match, setProfile, fetchProfile } = this.props;
    const { userId, type } = match.params;
    const prevUserId = prevProps.match.params.userId;
    const prevType = prevProps.match.params.type;

    if (!userId && type !== prevType) {
      setProfile({ isVisitor: type === "guest" });
    } else if (userId !== prevUserId) {
      fetchProfile(userId);
    }
  }

  render() {
    console.log("ProfileAction render")

    const { profile } = this.props;

    return (
      <I18nextProvider i18n={i18n}>
        {profile
        ? <PageLayout
          articleHeaderContent={<ArticleHeaderContent />}
          articleMainButtonContent={<ArticleMainButtonContent />}
          articleBodyContent={<ArticleBodyContent />}
          sectionHeaderContent={<SectionHeaderContent />}
          sectionBodyContent={profile.id ? <UpdateUserForm /> : <CreateUserForm />}
        />
        : <PageLayout
          articleHeaderContent={<ArticleHeaderContent />}
          articleMainButtonContent={<ArticleMainButtonContent />}
          articleBodyContent={<ArticleBodyContent />}
          sectionBodyContent={<Loader className="pageLoader" type="rombs" size={40} />}
          />}
      </I18nextProvider>
    );
  }
}

ProfileAction.propTypes = {
  match: PropTypes.object.isRequired,
  profile: PropTypes.object,
  setProfile: PropTypes.func.isRequired,
  fetchProfile: PropTypes.func.isRequired,
  resetProfile: PropTypes.func.isRequired
};

function mapStateToProps(state) {
  return {
    profile: state.profile.targetUser
  };
}

export default connect(mapStateToProps, {
  setProfile,
  fetchProfile,
  resetProfile
})(ProfileAction);