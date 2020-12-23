import React, { useEffect } from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { PageLayout, Loaders, utils, api, store } from "asc-web-common";
import { withTranslation, I18nextProvider } from "react-i18next";
import {
  ArticleHeaderContent,
  ArticleBodyContent,
  ArticleMainButtonContent,
} from "../../Article";
import { SectionHeaderContent, SectionBodyContent } from "./Section";
import { createI18N } from "../../../helpers/i18n";
//import { setDocumentTitle } from "../../../helpers/utils";
import {
  setIsVerHistoryPanel,
  setFilesFilter,
} from "../../../store/files/actions";
import {
  getFilter,
  getIsLoading,
  getFileVersions,
} from "../../../store/files/selectors";

const i18n = createI18N({
  page: "VersionHistory",
  localesPath: "pages/VersionHistory",
});

const { changeLanguage } = utils;
const { getIsTabletView } = store.auth.selectors;

class PureVersionHistory extends React.Component {
  constructor(props) {
    super(props);
  }

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
    const { setFilesFilter, filter } = this.props;
    setFilesFilter(filter);
  };

  render() {
    const { isLoading, versions } = this.props;

    return (
      <PageLayout
        withBodyScroll={true}
        withBodyAutoFocus={true}
        headerBorderBottom={true}
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

const VersionHistoryContainer = withTranslation()(PureVersionHistory);

const VersionHistory = (props) => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);
  return (
    <I18nextProvider i18n={i18n}>
      <VersionHistoryContainer {...props} />
    </I18nextProvider>
  );
};

VersionHistory.propTypes = {
  history: PropTypes.object.isRequired,
};

function mapStateToProps(state) {
  return {
    isLoading: getIsLoading(state),
    isTabletView: getIsTabletView(state),
    filter: getFilter(state),
    versions: getFileVersions(state),
  };
}

const mapDispatchToProps = (dispatch) => {
  return {
    setIsVerHistoryPanel: (isVisible) =>
      dispatch(setIsVerHistoryPanel(isVisible)),
    setFilesFilter: (filter) => dispatch(setFilesFilter(filter)),
  };
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(withRouter(VersionHistory));
