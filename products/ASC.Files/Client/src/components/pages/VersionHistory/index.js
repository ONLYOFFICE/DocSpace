import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { RequestLoader, Loader } from "asc-web-components";
import { PageLayout, utils, api } from "asc-web-common";
import { withTranslation, I18nextProvider } from "react-i18next";
import i18n from "./i18n";

import {
  ArticleHeaderContent,
  ArticleBodyContent,
  ArticleMainButtonContent
} from "../../Article";
import { SectionHeaderContent, SectionBodyContent } from "./Section";

const { changeLanguage } = utils;

class PureVersionHistory extends React.Component {
  constructor(props) {
    super(props);

    const { match } = props;
    const { fileId } = match.params;

    this.state = {
      isLoading: false,
      fileId,
      versions: null
    };
  }

  componentDidMount() {
    const { match, t } = this.props;
    const { fileId } = match.params;

    //document.title = `${t("GroupAction")} – ${t("People")}`;

    if (fileId) {
      this.getFileVersions(fileId);
    }
  }

  getFileVersions = fileId => {
    api.files.getFileVersionInfo(fileId)
      .then((versions) => this.setState({ versions }))
  }
  
  onLoading = status => {
    this.setState({ isLoading: status });
  };

  render() {
    const { versions } = this.state;
    const { t, settings } = this.props;

    return (
      <>
        <RequestLoader
          visible={this.state.isLoading}
          zIndex={256}
          loaderSize="16px"
          loaderColor={"#999"}
          label={`${t("LoadingProcessing")} ${t("LoadingDescription")}`}
          fontSize="12px"
          fontColor={"#999"}
        />
        {versions ? (
          <PageLayout
            withBodyScroll={true}
            withBodyAutoFocus={true}
          >
            <PageLayout.ArticleHeader>
              <ArticleHeaderContent />
            </PageLayout.ArticleHeader>

            <PageLayout.ArticleMainButton>
              <ArticleMainButtonContent
                onLoading={this.onLoading}
                startUpload={this.startUpload}
              />
            </PageLayout.ArticleMainButton>

            <PageLayout.ArticleBody>
              <ArticleBodyContent
                onLoading={this.onLoading}
                isLoading={this.state.isLoading}
              />
            </PageLayout.ArticleBody>

            <PageLayout.SectionHeader>
              <SectionHeaderContent title={versions && versions[0].title} />
            </PageLayout.SectionHeader>

            <PageLayout.SectionBody>
              <SectionBodyContent
                getFileVersions={this.getFileVersions}
                onLoading={this.onLoading}
                versions={versions}
                culture={settings.culture}
              />
            </PageLayout.SectionBody>
          </PageLayout>
        ) : (
            <PageLayout>
              <PageLayout.ArticleHeader>
                <ArticleHeaderContent />
              </PageLayout.ArticleHeader>

              <PageLayout.ArticleMainButton>
                <ArticleMainButtonContent />
              </PageLayout.ArticleMainButton>

              <PageLayout.ArticleBody>
                <ArticleBodyContent />
              </PageLayout.ArticleBody>

              <PageLayout.SectionBody>
                <Loader className="pageLoader" type="rombs" size="40px" />
              </PageLayout.SectionBody>
            </PageLayout>
          )}
      </>
    );
  }
}

const VersionHistoryContainer = withTranslation()(PureVersionHistory);

const VersionHistory = props => {
  changeLanguage(i18n);
  return (
    <I18nextProvider i18n={i18n}>
      <VersionHistoryContainer {...props} />
    </I18nextProvider>
  );
};

VersionHistory.propTypes = {
  history: PropTypes.object.isRequired,
  isLoaded: PropTypes.bool
};

function mapStateToProps(state) {
  return {
    settings: state.auth.settings,
    isLoaded: state.auth.isLoaded
  };
}

export default connect(mapStateToProps)(withRouter(VersionHistory));
