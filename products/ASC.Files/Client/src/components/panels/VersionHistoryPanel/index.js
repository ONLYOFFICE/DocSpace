import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";

import { Backdrop, Heading, Aside } from "asc-web-components";
import { utils, Loaders, store } from "asc-web-common";

import { withTranslation, I18nextProvider } from "react-i18next";
import { createI18N } from "../../../helpers/i18n";

import { setIsVerHistoryPanel } from "../../../store/files/actions";
import {
  getVerHistoryFileId,
  getIsLoading,
  getFileVersions,
} from "../../../store/files/selectors";

import {
  StyledVersionHistoryPanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody,
} from "../StyledPanels";

import { SectionBodyContent } from "../../pages/VersionHistory/Section/";

const i18n = createI18N({
  page: "VersionHistory",
  localesPath: "pages/VersionHistory",
});

const { changeLanguage } = utils;

const { getIsTabletView, getSettingsHomepage } = store.auth.selectors;

class PureVersionHistoryPanel extends React.Component {
  componentDidUpdate(preProps) {
    const { isTabletView, fileId } = this.props;
    if (isTabletView !== preProps.isTabletView && isTabletView) {
      this.redirectToPage(fileId);
    }
  }

  redirectToPage = (fileId) => {
    const { history, homepage, setIsVerHistoryPanel } = this.props;
    setIsVerHistoryPanel(false);

    history.replace(`${homepage}/${fileId}/history`);
  };

  onClosePanelHandler = () => {
    this.props.onClose();
  };

  render() {
    //console.log("render versionHistoryPanel");

    const { visible, isLoading, versions } = this.props;
    const zIndex = 310;
    console.log(isLoading);
    return (
      <StyledVersionHistoryPanel
        className="version-history-modal-dialog"
        visible={visible}
        isLoading={true}
      >
        <Backdrop
          onClick={this.onClosePanelHandler}
          visible={visible}
          zIndex={zIndex}
          isAside={true}
        />
        <Aside className="version-history-aside-panel">
          <StyledContent>
            <StyledHeaderContent className="version-history-panel-header">
              {versions && !isLoading ? (
                <Heading
                  className="version-history-panel-heading"
                  size="medium"
                  truncate
                >
                  {versions[0].title}
                </Heading>
              ) : (
                <Loaders.ArticleHeader
                  className="loader-version-history"
                  height="28"
                  width="688"
                  title="version-history-header-loader"
                />
              )}
            </StyledHeaderContent>

            <StyledBody className="version-history-panel-body">
              <SectionBodyContent />
            </StyledBody>
          </StyledContent>
        </Aside>
      </StyledVersionHistoryPanel>
    );
  }
}

const VersionHistoryPanelContainer = withTranslation()(PureVersionHistoryPanel);

const VersionHistoryPanel = (props) => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);
  return (
    <I18nextProvider i18n={i18n}>
      <VersionHistoryPanelContainer {...props} />
    </I18nextProvider>
  );
};

VersionHistoryPanelContainer.propTypes = {
  fileId: PropTypes.string,
  visible: PropTypes.bool,
  setIsLoading: PropTypes.func,
  onClose: PropTypes.func,
};

function mapStateToProps(state) {
  return {
    fileId: getVerHistoryFileId(state),
    isTabletView: getIsTabletView(state),
    homepage: getSettingsHomepage(state),
    isLoading: getIsLoading(state),
    versions: getFileVersions(state),
  };
}

function mapDispatchToProps(dispatch) {
  return {
    setIsVerHistoryPanel: (isVisible) =>
      dispatch(setIsVerHistoryPanel(isVisible)),
  };
}

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(VersionHistoryPanel);
