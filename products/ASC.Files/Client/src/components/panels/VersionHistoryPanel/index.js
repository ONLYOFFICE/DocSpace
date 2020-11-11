import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";

import { Backdrop, Heading, Aside } from "asc-web-components";
import { api, utils, store } from "asc-web-common";

import { withTranslation, I18nextProvider } from "react-i18next";
import { createI18N } from "../../../helpers/i18n";

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

const { getSettings } = store.auth.selectors;

class PureVersionHistoryPanel extends React.Component {
  constructor(props) {
    super(props);
    this.state = { isLoading: true };
  }

  componentDidMount() {
    const { fileId } = this.props;
    if (fileId) {
      this.getFileVersions(fileId);
    }
  }

  getFileVersions = (fileId) => {
    api.files.getFileVersionInfo(fileId).then((versions) => {
      console.log(versions);
      this.setState({ versions: versions, isLoading: false });
    });
  };

  onClosePanelHandler = () => {
    this.props.onClose();
  };

  render() {
    const { isLoading, versions } = this.state;
    const { visible, settings } = this.props;
    const zIndex = 310;
    return (
      <StyledVersionHistoryPanel
        className="modal-dialog-aside"
        visible={visible}
      >
        <Backdrop
          onClick={this.onClosePanelHandler}
          visible={visible}
          zIndex={zIndex}
        />{" "}
        {!isLoading ? (
          <Aside className="header_aside-panel">
            <StyledContent>
              <StyledHeaderContent>
                <Heading
                  className="header_aside-panel-header"
                  size="medium"
                  truncate
                >
                  {this.state.versions && this.state.versions[0].title}
                </Heading>
              </StyledHeaderContent>

              <StyledBody>
                <SectionBodyContent
                  getFileVersions={this.getFileVersions}
                  versions={versions}
                  culture={settings.culture}
                />
              </StyledBody>
            </StyledContent>
          </Aside>
        ) : null}
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
  isLoaded: PropTypes.bool,
};

function mapStateToProps(state) {
  return {
    settings: getSettings(state),
  };
}

export default connect(mapStateToProps)(VersionHistoryPanel);
