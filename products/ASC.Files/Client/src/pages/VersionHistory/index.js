import React from "react";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import PageLayout from "@appserver/common/components/PageLayout";
import Loaders from "@appserver/common/components/Loaders";
import { withTranslation } from "react-i18next";
import {
  ArticleHeaderContent,
  ArticleBodyContent,
  ArticleMainButtonContent,
} from "../../components/Article";
import { SectionHeaderContent, SectionBodyContent } from "./Section";
//import { setDocumentTitle } from "../../../helpers/utils";
import { inject, observer } from "mobx-react";

class PureVersionHistory extends React.Component {
  componentDidMount() {
    const { isTabletView } = this.props;

    if (!isTabletView) {
      this.redirectToPanelView();
    }
  }

  componentDidUpdate(prevProps) {
    const { isTabletView } = this.props;
    if (isTabletView !== prevProps.isTabletView && !isTabletView) {
      this.redirectToPanelView();
    }
  }

  redirectToPanelView = () => {
    const { setIsVerHistoryPanel } = this.props;
    setIsVerHistoryPanel(true);
    this.redirectToHomepage();
  };

  redirectToHomepage = () => {
    const { history } = this.props;
    history.goBack();
  };

  render() {
    const { isLoading, versions, showProgressBar } = this.props;

    return (
      <PageLayout
        withBodyScroll={true}
        withBodyAutoFocus={true}
        headerBorderBottom={true}
        showSecondaryProgressBar={showProgressBar}
        secondaryProgressBarIcon="file"
        showSecondaryButtonAlert={false}
      >
        <PageLayout.ArticleHeader>
          <ArticleHeaderContent />
        </PageLayout.ArticleHeader>

        <PageLayout.ArticleMainButton>
          <ArticleMainButtonContent />
        </PageLayout.ArticleMainButton>

        <PageLayout.ArticleBody>
          <ArticleBodyContent />
        </PageLayout.ArticleBody>

        <PageLayout.SectionHeader>
          {versions && !isLoading ? (
            <SectionHeaderContent
              title={versions[0].title}
              onClickBack={this.redirectToHomepage}
            />
          ) : (
            <Loaders.SectionHeader title="version-history-title-loader" />
          )}
        </PageLayout.SectionHeader>

        <PageLayout.SectionBody>
          <SectionBodyContent />
        </PageLayout.SectionBody>
      </PageLayout>
    );
  }
}

const VersionHistory = withTranslation("VersionHistory")(PureVersionHistory);

VersionHistory.propTypes = {
  history: PropTypes.object.isRequired,
};

export default inject(({ auth, filesStore, versionHistoryStore }) => {
  const { filter, isLoading } = filesStore;
  const {
    setIsVerHistoryPanel,
    versions,
    showProgressBar,
  } = versionHistoryStore;

  return {
    isTabletView: auth.settingsStore.isTabletView,
    isLoading,
    filter,
    versions,
    showProgressBar,

    setIsVerHistoryPanel,
  };
})(withRouter(observer(VersionHistory)));
