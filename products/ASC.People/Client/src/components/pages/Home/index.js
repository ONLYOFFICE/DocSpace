import React from "react";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
//import { RequestLoader } from "asc-web-components";
import { PageLayout, utils } from "asc-web-common";
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
import { isMobile } from "react-device-detect";
import { inject, observer } from "mobx-react";

class Home extends React.Component {
  componentDidUpdate(prevProps) {
    if (this.props.isLoading !== prevProps.isLoading) {
      if (this.props.isLoading) {
        utils.showLoader();
      } else {
        utils.hideLoader();
      }
    }
  }

  onLoading = (status) => {
    this.props.setIsLoading(status);
  };

  render() {
    console.log("Home render");
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
          <SectionHeaderContent onLoading={this.onLoading} />
        </PageLayout.SectionHeader>

        <PageLayout.SectionFilter>
          <SectionFilterContent onLoading={this.onLoading} />
        </PageLayout.SectionFilter>

        <PageLayout.SectionBody>
          <SectionBodyContent isMobile={isMobile} onLoading={this.onLoading} />
        </PageLayout.SectionBody>

        <PageLayout.SectionPaging>
          <SectionPagingContent onLoading={this.onLoading} />
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
