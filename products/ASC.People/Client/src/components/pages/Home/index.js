import React from "react";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
//import RequestLoader from "@appserver/components/request-loader";
import PageLayout from "@appserver/common/components/PageLayout";
import { showLoader, hideLoader } from "@appserver/common/utils";
import {
  ArticleHeaderContent,
  ArticleBodyContent,
  ArticleMainButtonContent,
} from "../../Article";
import {
  SectionHeaderContent,
  SectionBodyContent,
  SectionFilterContent,
  SectionPagingContent,
} from "./Section";
import { inject, observer } from "mobx-react";
import { isMobile } from "react-device-detect";

class Home extends React.Component {
  componentDidUpdate(prevProps) {
    if (this.props.isLoading !== prevProps.isLoading) {
      if (this.props.isLoading) {
        showLoader();
      } else {
        hideLoader();
      }
    }
  }

  render() {
    //console.log("Home render");
    const { isLoaded, isAdmin, isHeaderVisible } = this.props;

    return (
      <PageLayout
        withBodyScroll={true}
        withBodyAutoFocus={!isMobile}
        isLoaded={isLoaded}
        isHeaderVisible={isHeaderVisible}
      >
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

        <PageLayout.SectionFilter>
          <SectionFilterContent />
        </PageLayout.SectionFilter>

        <PageLayout.SectionBody>
          <SectionBodyContent />
        </PageLayout.SectionBody>

        <PageLayout.SectionPaging>
          <SectionPagingContent />
        </PageLayout.SectionPaging>
      </PageLayout>
    );
  }
}

Home.propTypes = {
  history: PropTypes.object.isRequired,
  isLoaded: PropTypes.bool,
  isAdmin: PropTypes.bool,
};

export default inject(({ auth, peopleStore }) => ({
  isLoaded: auth.isLoaded,
  isAdmin: auth.isAdmin,
  isLoading: peopleStore.isLoading,
  setIsLoading: peopleStore.setIsLoading,
  isHeaderVisible: auth.settingsStore.isHeaderVisible,
}))(observer(withRouter(Home)));
