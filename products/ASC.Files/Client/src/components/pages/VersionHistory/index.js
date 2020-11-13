import React, { useEffect } from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { isMobile } from "react-device-detect";
import { Loader } from "asc-web-components";
import { PageLayout, utils, api, store } from "asc-web-common";
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
  setFirstLoad,
  setVisibilityVersionHistoryPanel,
  setVersionHistoryFileId,
} from "../../../store/files/actions";
import { getIsLoading } from "../../../store/files/selectors";
const i18n = createI18N({
  page: "VersionHistory",
  localesPath: "pages/VersionHistory",
});

const { changeLanguage } = utils;
const { getSettingsHomepage } = store.auth.selectors;
class PureVersionHistory extends React.Component {
  constructor(props) {
    super(props);

    const { match } = props;
    const { fileId } = match.params;

    this.state = {
      fileId,
      versions: null,
    };
  }

  componentDidMount() {
    const {
      match,
      history,
      homepage,
      setVisibilityVersionHistoryPanel,
      setVersionHistoryFileId,
    } = this.props;
    const { fileId } = match.params;

    //setDocumentTitle(t("GroupAction"));
    if (fileId) {
      if (!isMobile && window.innerWidth > 1024) {
        setVisibilityVersionHistoryPanel(true);
        setVersionHistoryFileId(fileId);
        history.push(`${homepage}`);
      } else {
        this.getFileVersions(fileId);
      }
    }
  }

  componentDidUpdate(prevProps) {
    if (this.props.isLoading !== prevProps.isLoading) {
      if (this.props.isLoading) {
        utils.showLoader();
      } else {
        utils.hideLoader();
      }
    }
  }

  getFileVersions = (fileId) => {
    const { setFirstLoad } = this.props;
    api.files.getFileVersionInfo(fileId).then((versions) => {
      setFirstLoad(false);
      this.setState({ versions });
    });
  };

  render() {
    const { versions } = this.state;

    return versions ? (
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

        <PageLayout.SectionHeader borderBottom={true}>
          <SectionHeaderContent title={versions && versions[0].title} />
        </PageLayout.SectionHeader>

        <PageLayout.SectionBody>
          <SectionBodyContent
            getFileVersions={this.getFileVersions}
            versions={versions}
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
    homepage: getSettingsHomepage(state),
  };
}

const mapDispatchToProps = (dispatch) => {
  return {
    setFirstLoad: (firstLoad) => dispatch(setFirstLoad(firstLoad)),
    setVisibilityVersionHistoryPanel: (isVisible) =>
      dispatch(setVisibilityVersionHistoryPanel(isVisible)),
    setVersionHistoryFileId: (fileId) =>
      dispatch(setVersionHistoryFileId(fileId)),
  };
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(withRouter(VersionHistory));
