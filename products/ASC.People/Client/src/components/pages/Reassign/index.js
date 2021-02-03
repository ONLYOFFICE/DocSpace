import React from "react";
import { connect } from "react-redux";
// import PropTypes from "prop-types";
import { PageLayout, store } from "asc-web-common";
import {
  ArticleHeaderContent,
  ArticleMainButtonContent,
  ArticleBodyContent,
} from "../../Article";
// import { SectionHeaderContent } from './Section';
// import { fetchProfile } from '../../../store/profile/actions';
import { I18nextProvider } from "react-i18next";
import { SectionHeaderContent, SectionBodyContent } from "./Section";
import { createI18N } from "../../../helpers/i18n";
const i18n = createI18N({
  page: "Reassign",
  localesPath: "pages/Reassign",
});
const { isAdmin } = store.auth.selectors;

class Reassign extends React.Component {
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
    const { isAdmin } = this.props;
    console.log("Reassign render");

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
        <PageLayout>
          <PageLayout.ArticleHeader>
            <ArticleHeaderContent />
          </PageLayout.ArticleHeader>

          {isAdmin && (
            <PageLayout.ArticleMainButton>
              <ArticleMainButtonContent />
            </PageLayout.ArticleMainButton>
          )}

          <PageLayout.ArticleBody>
            <ArticleBodyContent />
          </PageLayout.ArticleBody>

          <PageLayout.SectionHeader>
            <SectionHeaderContent />
          </PageLayout.SectionHeader>

          <PageLayout.SectionBody>
            <SectionBodyContent />
          </PageLayout.SectionBody>
        </PageLayout>
      </I18nextProvider>
    );
  }
}

Reassign.propTypes = {
  // match: PropTypes.object.isRequired,
  // profile: PropTypes.object,
  // fetchProfile: PropTypes.func.isRequired
};

function mapStateToProps(state) {
  return {
    isAdmin: isAdmin(state),
    // profile: state.profile.targetUser
  };
}

export default connect(mapStateToProps, {})(Reassign);
