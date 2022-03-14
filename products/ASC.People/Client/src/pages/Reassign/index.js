import React from "react";
// import PropTypes from "prop-types";
import PageLayout from "@appserver/common/components/PageLayout";
import {
  ArticleHeaderContent,
  ArticleMainButtonContent,
  ArticleBodyContent,
} from "../../components/Article";
// import { SectionHeaderContent } from './Section';
import { SectionHeaderContent, SectionBodyContent } from "./Section";
import { inject, observer } from "mobx-react";

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
    // console.log("Reassign render");

    // let loaded = false;
    // const { profile, match } = this.props;
    // const { userId, type } = match.params;

    // if (type) {
    //   loaded = true;
    // } else if (profile) {
    //   loaded = profile.userName === userId || profile.id === userId;
    // }

    return (
      <PageLayout>
        <PageLayout.SectionHeader>
          <SectionHeaderContent />
        </PageLayout.SectionHeader>

        <PageLayout.SectionBody>
          <SectionBodyContent />
        </PageLayout.SectionBody>
      </PageLayout>
    );
  }
}

Reassign.propTypes = {
  // match: PropTypes.object.isRequired,
  // profile: PropTypes.object,
  // fetchProfile: PropTypes.func.isRequired
};

export default inject(({ auth }) => ({
  isAdmin: auth.isAdmin,
}))(observer(Reassign));
