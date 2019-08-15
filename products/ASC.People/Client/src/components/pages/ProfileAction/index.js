import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { Backdrop, PageLayout, Loader } from "asc-web-components";
import { ArticleHeaderContent, ArticleMainButtonContent, ArticleBodyContent } from '../../Article';
import { SectionHeaderContent, SectionBodyContent } from './Section';
import { setProfile, fetchProfile, resetProfile } from '../../../store/profile/actions';

class ProfileAction extends React.Component {
  constructor(props) {
    super(props);
  }

  componentDidMount() {
    const { match, setProfile, fetchProfile } = this.props;
    const { userId, type } = match.params;

    if (!userId) {
      setProfile({ isVisitor: type === "guest" });
    }
    else {
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
    }
    else if (userId !== prevUserId) {
      fetchProfile(userId);
    }
  }

  componentWillUnmount() {
    this.props.resetProfile();
  }

  render() {
    console.log("ProfileAction render")

    const { profile, match } = this.props;
    const { type } = match.params;

    return (
      profile
        ? <PageLayout
          articleHeaderContent={<ArticleHeaderContent />}
          articleMainButtonContent={<ArticleMainButtonContent />}
          articleBodyContent={<ArticleBodyContent />}
          sectionHeaderContent={
            <SectionHeaderContent profile={profile} userType={type} />
          }
          sectionBodyContent={
            <SectionBodyContent profile={profile} userType={type} />
          }
        />
        : <PageLayout
          articleHeaderContent={<ArticleHeaderContent />}
          articleMainButtonContent={<ArticleMainButtonContent />}
          articleBodyContent={<ArticleBodyContent />}
          sectionBodyContent={
            <Loader className="pageLoader" type="rombs" size={40} />
          }
        />
    );
  }
}

ProfileAction.propTypes = {
  match: PropTypes.object.isRequired,
  profile: PropTypes.object,
  fetchProfile: PropTypes.func.isRequired,
  setProfile: PropTypes.func.isRequired
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