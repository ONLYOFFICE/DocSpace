import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { PageLayout, Loader } from "asc-web-components";
import { ArticleHeaderContent, ArticleMainButtonContent, ArticleBodyContent } from '../../Article';
import { SectionHeaderContent, SectionBodyContent } from './Section';
import { setProfile, fetchProfile, resetProfile } from '../../../store/profile/actions';

class Profile extends React.Component {
  constructor(props) {
    super(props);
  }

  componentDidMount() {
    const { match, fetchProfile } = this.props;
    const { userId } = match.params;

    fetchProfile(userId);
  }

  componentDidUpdate(prevProps) {
    const { match, fetchProfile } = this.props;
    const { userId } = match.params;
    const prevUserId = prevProps.match.params.userId;

    if (userId !== prevUserId) {
      fetchProfile(userId);
    }
  }

  componentWillUnmount() {
    this.props.resetProfile();
  }

  render() {
    console.log("Profile render")

    const { profile } = this.props;
    return (
      profile
        ? <PageLayout
          articleHeaderContent={<ArticleHeaderContent />}
          articleMainButtonContent={<ArticleMainButtonContent />}
          articleBodyContent={<ArticleBodyContent />}
          sectionHeaderContent={
            <SectionHeaderContent profile={profile} />
          }
          sectionBodyContent={
            <SectionBodyContent profile={profile} />
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
  };
};

Profile.propTypes = {
  history: PropTypes.object.isRequired,
  match: PropTypes.object.isRequired,
  profile: PropTypes.object,
  isLoaded: PropTypes.bool
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
})(Profile);