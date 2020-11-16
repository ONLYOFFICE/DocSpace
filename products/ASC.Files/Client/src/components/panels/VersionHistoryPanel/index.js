import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";

import { Backdrop, Heading, Aside } from "asc-web-components";
import { api, utils, Loaders } from "asc-web-common";

import { withTranslation, I18nextProvider } from "react-i18next";
import { createI18N } from "../../../helpers/i18n";

import { setIsLoading } from "../../../store/files/actions";
import {
  getIsLoading,
  getVersionHistoryFileId,
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

class PureVersionHistoryPanel extends React.Component {
  constructor(props) {
    super(props);
    this.state = { versions: {} };
  }

  componentDidMount() {
    const { fileId } = this.props;
    if (fileId) {
      this.getFileVersions(fileId);
    }
  }

  getFileVersions = (fileId) => {
    const { setIsLoading } = this.props;

    api.files.getFileVersionInfo(fileId).then((versions) => {
      this.setState({ versions: versions }, () => setIsLoading(false));
    });
  };

  onClosePanelHandler = () => {
    this.props.onClose();
  };

  render() {
    //console.log("render versionHistoryPanel");

    const { versions } = this.state;
    const { visible, isLoading } = this.props;
    const zIndex = 310;

    return (
      <StyledVersionHistoryPanel
        className="version-history-modal-dialog"
        visible={visible}
      >
        <Backdrop
          onClick={this.onClosePanelHandler}
          visible={visible}
          zIndex={zIndex}
        />
        <Aside className="version-history-aside-panel">
          {!isLoading && Object.keys(versions).length > 0 ? (
            <StyledContent>
              <StyledHeaderContent className="version-history-panel-header">
                <Heading
                  className="version-history-panel-heading"
                  size="medium"
                  truncate
                >
                  {versions[0].title}
                </Heading>
              </StyledHeaderContent>

              <StyledBody className="version-history-panel-body">
                <SectionBodyContent
                  getFileVersions={this.getFileVersions}
                  versions={versions}
                />
              </StyledBody>
            </StyledContent>
          ) : (
            <StyledContent>
              <StyledHeaderContent className="version-history-panel-header">
                <Loaders.ArticleHeader
                  height="28"
                  width="688"
                  title="version-history-header-loader"
                />
              </StyledHeaderContent>
              <StyledBody className="version-history-panel-body">
                <Loaders.HistoryRows title="version-history-body-loader" />
              </StyledBody>
            </StyledContent>
          )}
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
  isLoading: PropTypes.bool,
  fileId: PropTypes.string,
  visible: PropTypes.bool,
  setIsLoading: PropTypes.func,
  onClose: PropTypes.func,
};

function mapStateToProps(state) {
  return {
    isLoading: getIsLoading(state),
    fileId: getVersionHistoryFileId(state),
  };
}

function mapDispatchToProps(dispatch) {
  return {
    setIsLoading: (isLoading) => dispatch(setIsLoading(isLoading)),
  };
}

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(VersionHistoryPanel);
