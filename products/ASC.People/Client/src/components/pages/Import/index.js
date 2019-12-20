import React from "react";
import { connect } from "react-redux";
// import PropTypes from "prop-types";
import { PageLayout } from "asc-web-common";
import { ArticleHeaderContent, ArticleMainButtonContent, ArticleBodyContent } from '../../Article';
// import { SectionHeaderContent } from './Section';
// import { fetchProfile } from '../../../store/profile/actions';
import i18n from "./i18n";
import { I18nextProvider } from "react-i18next";
import { SectionHeaderContent, SectionBodyContent } from './Section';

class Import extends React.Component {

  componentDidMount() {
    // const { match, fetchProfile } = this.props;
    // const { userId } = match.params;

    // if (userId) {
    //   fetchProfile(userId);
    // }
  }

  componentDidUpdate(prevProps) {
    // const { match, fetchProfile } = this.props;
    // const { userId } = match.params;
    // const prevUserId = prevProps.match.params.userId;

    // if (userId !== undefined && userId !== prevUserId) {
    //   fetchProfile(userId);
    // }
  }

  render() {
    //console.log("Import render")

    // let loaded = false;
    // const { profile, match } = this.props;
    // const { userId, type } = match.params;

    // if (type) {
    //   loaded = true;
    // } else if (profile) {
    //   loaded = profile.userName === userId || profile.id === userId;
    // }

    return (
      <I18nextProvider i18n={i18n}>
        <PageLayout
          articleHeaderContent={<ArticleHeaderContent />}
          articleMainButtonContent={<ArticleMainButtonContent />}
          articleBodyContent={<ArticleBodyContent />}
          sectionHeaderContent={<SectionHeaderContent />}
          sectionBodyContent={<SectionBodyContent />}
        />
      </I18nextProvider>
    );
  }
}


Import.propTypes = {
  // match: PropTypes.object.isRequired,
  // profile: PropTypes.object,
  // fetchProfile: PropTypes.func.isRequired
};

function mapStateToProps(state) {
  return {
    // profile: state.profile.targetUser
  };
}


export default connect(mapStateToProps, {
})(Import);